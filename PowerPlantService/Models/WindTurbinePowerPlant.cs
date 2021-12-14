namespace PowerPlantService.Models
{
    public class WindTurbinePowerPlant : PowerPlant
    {
        public decimal WindPercentage { get; set; }

        public override decimal PMax()
        {
            return base.PMax() * WindPercentage / 100;
        }

        public override decimal PMin()
        {
            return base.PMin() * WindPercentage / 100;
        }

        public WindTurbinePowerPlant(string name, FuelType fuelType, decimal efficiency, decimal pMin, decimal pMax,
        decimal windPercentage) : base(name,
        fuelType, efficiency, pMin, pMax, 0)
        {
            WindPercentage = windPercentage;
        }
    }
}
