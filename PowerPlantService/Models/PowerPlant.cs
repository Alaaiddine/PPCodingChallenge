using System;

namespace PowerPlantService.Models
{
    public class PowerPlant
    {
        public string Name { get; }

        public FuelType FuelType { get; }
        public decimal Efficiency { get; }
        public decimal Production { get; set; }
        public decimal FuelPrice { get; set; }
        public bool Disabled { get; internal set; }
        private readonly decimal _pMax;
        private readonly decimal _pMin;

        public PowerPlant(string name, FuelType fuelType, decimal efficiency, decimal pMin, decimal pMax,
        decimal fuelPrice)
        {
            Disabled = false;
            _pMax = pMax;
            _pMin = pMin;

            Name = name;
            FuelType = fuelType;
            Efficiency = efficiency;
            FuelPrice = fuelPrice;
        }

        public virtual decimal PMax()
        {
            return _pMax;
        }

        public virtual decimal PMin()
        {
            return _pMin;
        }

        public decimal PricePerMwh()
        {
            return FuelPrice / Efficiency;
        }

        public decimal TryProduce(decimal remainingProduction)
        {
            //When Requested production is less than PMin we can only produce Pmin
            if (remainingProduction < PMin())
            {
                Production = PMin();
                return remainingProduction - PMin();
            }

            //When Requested production is between Pmin and Pmax we can Produce the remaining Production.
            if (remainingProduction < PMax())
            {
                Production = remainingProduction;
                return 0;
            }

            // When the requested production is more than requested we can produce Pmax.
            Production = PMax();
            return remainingProduction - PMax();
        }

        /// <summary>
        /// Try Reduce can be called when remaining Production is negative, which means that there is a over production
        /// In that case the Function will try to reduce the production to maximum Pmin to absorb the over production.
        /// </summary>
        /// <param name="overProduction"></param>
        /// <returns>The Function will return the overProduction after reduction.</returns>
        public decimal TryReduce(decimal overProduction)
        {
            var maxReduction = Production - PMin();
            if (maxReduction == 0 || overProduction == 0) return overProduction;

            // when the overProduction is more or equal to maxReduction.
            // We can reduce Production to Min and decrease the over production.
            if (overProduction >= maxReduction)
            {
                Production -= maxReduction;
                return overProduction - maxReduction;
            }

            // when the overProduction is less that maxReduction, Bingo we found the solution.
            Production -= overProduction;
            return 0;
        }

        public void SwitchOff()
        {
            Disabled = true;
        }

        public void SwitchOn()
        {
            Disabled = false;
        }
    }

    public enum FuelType
    {
        Unspecified,
        GasFired,
        TurboJet,
        WindTurbine,
    }
}




