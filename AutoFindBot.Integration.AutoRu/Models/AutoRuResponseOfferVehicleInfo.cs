using Newtonsoft.Json;

namespace AutoFindBot.Integration.AutoRu.Models;

public class AutoRuResponseOfferVehicleInfo
{
    [JsonProperty("mark_info")]
    public AutoRuResponseOfferVehicleInfoMarkInfo MarkInfo { get; set; }
    
    [JsonProperty("model_info")]
    public AutoRuResponseOfferVehicleInfoModelInfo ModelInfo { get; set; }
    
    [JsonProperty("super_gen")]
    public AutoRuResponseOfferVehicleInfoSuperGen SuperGen { get; set; }
    
    [JsonProperty("tech_param")]
    public AutoRuResponseOfferVehicleInfoTechParam TechParam { get; set; }
}