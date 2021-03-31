using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;
using RestSharp;
using System.Collections;
using Xamarin.Essentials;

namespace NcoVApp
{
    public partial class MainPage : ContentPage
    {
        public static double lat;
        public static double lng;
        public MainPage()
        {
            InitializeComponent();
            pckrSEARCH.Items.Add("Search");
            pckrSEARCH.Items.Add("Search via current location");
        }

        public class Stats
        {
            [JsonProperty("country")]
            public dynamic Country { get; set; }

            [JsonProperty("latest_stat_by_country")]
            public List<latest_stat_by_country> LatestStatByCountry { get; set; }


            public partial class latest_stat_by_country
            {
                [JsonProperty("id")]
                public dynamic Id { get; set; }

                [JsonProperty("country_name")]
                public dynamic CountryName { get; set; }

                [JsonProperty("total_cases")]
                public dynamic TotalCases { get; set; }

                [JsonProperty("new_cases")]
                public dynamic NewCases { get; set; }

                [JsonProperty("active_cases")]
                public dynamic ActiveCases { get; set; }

                [JsonProperty("total_deaths")]
                public dynamic TotalDeaths { get; set; }

                [JsonProperty("new_deaths")]
                public dynamic NewDeaths { get; set; }

                [JsonProperty("total_recovered")]
                public dynamic TotalRecovered { get; set; }

                [JsonProperty("serious_critical")]
                public dynamic SeriousCritical { get; set; }

                [JsonProperty("region")]
                public object Region { get; set; }

                [JsonProperty("total_cases_per1m")]
                public dynamic TotalCasesPer1M { get; set; }

                [JsonProperty("record_date")]
                public DateTimeOffset RecordDate { get; set; }
            }
        }

      

        public void getStats()
        {
            var client = new RestClient(string.Format("https://coronavirus-monitor.p.rapidapi.com/coronavirus/latest_stat_by_country.php?country={0}", entryCOUNTRY.Text));
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "coronavirus-monitor.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "d7b1359095msh2f3d1cf03fadfc9p17d1dcjsnb8086ea2665c");
            request.RequestFormat = DataFormat.Json;
            IRestResponse Response = client.Execute(request);
            var model = JsonConvert.DeserializeObject<Stats>(Response.Content);
            var output = model;

            labelSTATS.Text = "Statistics for: " + output.LatestStatByCountry[0].CountryName; 
            labelTOTALCASE.Text = output.LatestStatByCountry[0].TotalCases.ToString();
            labelNEWCASES.Text = output.LatestStatByCountry[0].NewCases.ToString();
            labelDEATHS.Text = output.LatestStatByCountry[0].TotalDeaths.ToString();
            labelRECOVER.Text = output.LatestStatByCountry[0].TotalRecovered.ToString();
            labelDATE.Text = "Update as of: " + output.LatestStatByCountry[0].RecordDate.ToString("dddd, MMMM dd, yyyy hh:mm:ss tt");

        }

        public async Task getStatsByGPSAsync()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Lowest);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    lng = location.Longitude;
                    lat = location.Latitude;
                }
            }
            catch (FeatureNotSupportedException)
            {
                await DisplayAlert("Alert", "Your devices does not support locating via GPS!", "Okay");
            }
            catch (FeatureNotEnabledException)
            {
                await DisplayAlert("Alert", "GPS is not enabled! Please enable to use this feature!", "Okay");
            }
            catch (PermissionException)
            {
                await DisplayAlert("Alert", "Accept the permission to use this feature.", "Okay");
            }
            catch (Exception)
            {
                await DisplayAlert("Alert", "Unable to get location!", "Okay");
            }
        }

        public async Task ReverseGeoCodingAsync()
        {
            try
            { 
                var placemarks = await Geocoding.GetPlacemarksAsync(lat, lng);
                 var placemark = placemarks?.FirstOrDefault();
                if (placemark != null)
                {
                    var geocodeAddress = placemark.CountryName;
                }
                entryCOUNTRY.Text = placemark.CountryName;
            }
            catch (FeatureNotSupportedException)
            {
                await DisplayAlert("Alert", "Your devices does not support locating via GPS!", "Okay");
            }
            catch (Exception)
            {
                await DisplayAlert("Alert", "Unable to get location!", "Okay");
            }

        }
        private async void PckrSEARCH_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pckrSEARCH.SelectedIndex == 0)
            {
                pckrSEARCH.TextColor = Color.Transparent;
                try
                {
                    getStats();
                }
                catch (System.ArgumentOutOfRangeException)
                {
                    await DisplayAlert("Alert", "Please make sure that you have an internet connection or you spelled the country's name right!", "Okay");
                }
                catch (System.NullReferenceException)
                {
                    await DisplayAlert("Alert", "Please make sure that you have an internet connection or you spelled the country's name right!", "Okay");
                }
            }
            else if (pckrSEARCH.SelectedIndex == 1)
            {
                pckrSEARCH.TextColor = Color.Transparent;
                try
                {
                    await getStatsByGPSAsync();
                    await ReverseGeoCodingAsync();
                    getStats();
                }

                catch (FeatureNotSupportedException)
                {
                    await DisplayAlert("Alert", "Your devices does not support locating via GPS!", "Okay");
                }
                catch (FeatureNotEnabledException)
                {
                    await DisplayAlert("Alert", "GPS is not enabled! Please enable to use this feature!", "Okay");
                }
                catch (PermissionException)
                {
                    await DisplayAlert("Alert", "Accept the permission to use this feature.", "Okay");
                }
                catch (Exception)
                {
                    await DisplayAlert("Alert", "Unable to get location!", "Okay");
                }
            }
        }

        private void EntryCOUNTRY_Completed(object sender, EventArgs e)
        {
            pckrSEARCH.Focus();
        }
    }
}