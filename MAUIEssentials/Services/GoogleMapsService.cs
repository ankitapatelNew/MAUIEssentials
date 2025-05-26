using System.Net.Http.Headers;
using MAUIEssentials.AppCode.Helpers;
using MAUIEssentials.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MAUIEssentials.Services
{
    public class GoogleMapsService
    {
        private HttpClient CreateClient()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("Enter your google map base api url")
            };

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }

        public async Task<GooglePlaceAutoCompleteResult> GetPlaces(string text)
        {
            GooglePlaceAutoCompleteResult? results = null;

            try
            {
                using (var httpClient = CreateClient())
                {
                    var response = await httpClient.GetAsync($"api/place/autocomplete/json?input={Uri.EscapeUriString(text)}&components=country%3AUK&key={"Enter your google map api key"}").ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        if (!string.IsNullOrWhiteSpace(json) && json != "ERROR")
                        {
                            results = await Task.Run(() =>
                               JsonConvert.DeserializeObject<GooglePlaceAutoCompleteResult>(json)
                            ).ConfigureAwait(false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }

            return results;
        }

        public async Task<GooglePlace> GetPlaceDetails(string placeId)
        {
            GooglePlace? result = null;

            try
            {
                using (var httpClient = CreateClient())
                {
                    var response = await httpClient.GetAsync($"api/place/details/json?placeid={Uri.EscapeUriString(placeId)}&key={"Enter your google map api key"}").ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        if (!string.IsNullOrWhiteSpace(json) && json != "ERROR")
                        {
                            result = new GooglePlace(JObject.Parse(json));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogException();
            }

            return result;
        }
    }    
}