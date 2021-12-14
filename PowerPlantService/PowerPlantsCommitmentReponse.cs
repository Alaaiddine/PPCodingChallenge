using Newtonsoft.Json;
using System.Collections.Generic;

namespace PowerPlantService
{
    //public class UnitCommitmentReponse
    //{
    //    public List<UnitCommitment> UnitCommitments = new();
    //}

    public class PowerPlantsCommitmentReponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("p")]
        public decimal Power { get; set; }
    }
}
