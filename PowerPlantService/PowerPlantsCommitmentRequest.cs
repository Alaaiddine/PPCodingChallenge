using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PowerPlantService
{
    public class PowerPlantsCommitmentRequest : IRequest<List<PowerPlantsCommitmentReponse>>
    {
        [JsonPropertyName("load")]
        public decimal Load { get; set; }
        [JsonPropertyName("fuels")]
        public Fuels Fuels { get; set; }
        [JsonPropertyName("powerplants")]
        public List<PowerPlant> PowerPlants { get; set; }
    }


    public class Fuels
    {
        /// <summary>
        /// gas(euro/MWh)
        /// </summary>
        [JsonPropertyName("gas(euro/MWh)")]
        public decimal Gas { get; set; }
        /// <summary>
        /// kerosine(euro/MWh)
        /// </summary>
        [JsonPropertyName("kerosine(euro/MWh)")]
        public decimal Kerosine { get; set; }
        /// <summary>
        /// co2(euro/ton)
        /// </summary>
        [JsonPropertyName("co2(euro/ton)")]
        public decimal Co2 { get; set; }
        /// <summary>
        /// wind(%)
        /// </summary>
        [JsonPropertyName("wind(%)")]
        public decimal Wind { get; set; }
    }

    public class PowerPlant
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("efficiency")]
        public decimal Efficiency { get; set; }
        [JsonPropertyName("pmin")]
        public decimal PMin { get; set; }
        [JsonPropertyName("pmax")]
        public decimal PMax { get; set; }
    }

}
