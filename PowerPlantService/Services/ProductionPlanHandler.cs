using MediatR;
using PowerPlantService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PowerPlantService.Services
{
    public class ProductionPlanHandler : IRequestHandler<PowerPlantsCommitmentRequest, List<PowerPlantsCommitmentReponse>>
    {
       
        public Task<List<PowerPlantsCommitmentReponse>> Handle(PowerPlantsCommitmentRequest request, CancellationToken cancellationToken)
        {
            var powerPlantsWithNoneWindTurbines = request.PowerPlants.Where(pp => pp.Type != "windturbine")
               .Select(p =>
                   new Models.PowerPlant(p.Name, (FuelType)Enum.Parse(typeof(FuelType), p.Type, true), p.Efficiency, p.PMin, p.PMax, GetPrice(p.Type, request.Fuels))
                   ).ToList();

            var windTurbinesPowerPlants = request.PowerPlants.Where(pp => pp.Type == "windturbine")
                .Select(p =>
                    new WindTurbinePowerPlant(p.Name, (FuelType)Enum.Parse(typeof(FuelType), p.Type, true), p.Efficiency, p.PMin, p.PMax, GetPrice(p.Type, request.Fuels))
                    ).ToList();

            var powerPlants = powerPlantsWithNoneWindTurbines.Union(windTurbinesPowerPlants);

            var allIndexes = Enumerable.Range(0, powerPlants.Count()).ToArray();
            var allCombinations = Combinations(allIndexes).ToList();

            List<ProductionResponse> bestSolution = null;
            for (var depth = 0; depth < powerPlants.Count(); depth++)
            {
                var solutions = new List<List<ProductionResponse>>();
                foreach (var combinationsDepth in allCombinations.Where(w => w.Length == depth))
                    solutions.Add(TryFindSolution(request.Load, powerPlants.ToList(), combinationsDepth));
                if (bestSolution != null)
                    solutions.Add(bestSolution);

                bestSolution = GetBestSolution(solutions, request.Load);


                if (bestSolution.Sum(s => s.Production) == request.Load)
                    break;
            }


            var response = bestSolution.Select(bs => new PowerPlantsCommitmentReponse
            {
                Name = bs.Name,
                Power = bs.Production
            }).ToList();

            return Task.FromResult(response);
        }

        public static IEnumerable<T[]> Combinations<T>(IEnumerable<T> source)
        {
            if (null == source)
                throw new ArgumentNullException(nameof(source));

            var data = source.ToArray();

            return Enumerable
            .Range(0, 1 << (data.Length))
            .Select(index => data
            .Where((v, i) => (index & (1 << i)) != 0)
            .ToArray());
        }

        private static List<ProductionResponse> TryFindSolution(decimal load, List<Models.PowerPlant> powerPlants, int[] powerPlantsToSwitchOff)
        {
            //Init
            for (var index = 0; index < powerPlants.Count(); index++)
            {
                var pp = powerPlants[index];
                pp.Production = 0;
                pp.SwitchOn();

                if (powerPlantsToSwitchOff.Contains(index))
                    pp.SwitchOff();
            }
            var remainingProduction = load;
            foreach (var powerPlant in powerPlants.Where(w => !w.Disabled)
            .OrderBy(powerPlant => powerPlant.PricePerMwh()))
            {
                remainingProduction = powerPlant.TryProduce(remainingProduction);
                if (remainingProduction == 0)
                    break;
            }

            // In case of OverProduction we try to reduce the production.
            if (remainingProduction < 0)
                // This time using unversed merit Order.
                foreach (var powerPlant in powerPlants.Where(w => !w.Disabled)
                .OrderByDescending(powerPlant => powerPlant.PricePerMwh()))
                {
                    remainingProduction = -powerPlant.TryReduce(Math.Abs(remainingProduction));
                    if (remainingProduction == 0)
                        break;
                }

            return powerPlants.Where(pp => pp.Production > 0).Select(s => new ProductionResponse
            {
                Production = s.Production,
                Cost = s.FuelPrice * s.Production,
                Name = s.Name
            }).ToList();
        }

        private static List<ProductionResponse> GetBestSolution(List<List<ProductionResponse>> solutions, decimal targetLoad)
        {
            return
            solutions.Where(r => r.Sum(s => s.Production) == targetLoad).OrderBy(o => o.Sum(s => s.Cost))
            .FirstOrDefault() ??
            solutions.Where(r => r.Sum(s => s.Production) >= targetLoad)
            .OrderBy(o => o.Sum(s => s.Cost)).ThenBy(o => o.Sum(s => s.Production)).FirstOrDefault() ?? solutions
            .OrderByDescending(o => o.Sum(s => s.Production)).ThenBy(o => o.Sum(s => s.Cost)).FirstOrDefault();
        }


        private static decimal GetPrice(string type, Fuels fuels)
        {
            switch (type)
            {
                case "gasfired":
                    return fuels.Gas;
                case "turbojet":
                    return fuels.Kerosine;
                case "windturbine":
                    return fuels.Wind;
                default:
                    throw new InvalidOperationException("Input provided is not valid");
            }
        }

        //Task<UnitCommitmentReponse> Handle(UnitCommitmentRequest request, CancellationToken cancellationToken)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
