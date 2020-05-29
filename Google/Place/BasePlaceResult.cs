using Newtonsoft.Json;

namespace LocationData.Google.Place
{
    public class BasePlaceResult : BaseQueryResult
    {
        [JsonProperty("html_attributions")]
        public object[] HtmlAttributions { get; set; }
    }
}
