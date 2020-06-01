using Newtonsoft.Json;

namespace LocationData.Google.Place
{
    public class PlaceApiResult<T> : GoogleApiQueryResult<T>
    {
        [JsonProperty("html_attributions")]
        public object[] HtmlAttributions { get; set; }
    }
}
