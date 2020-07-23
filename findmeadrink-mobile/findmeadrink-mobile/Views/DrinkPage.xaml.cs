using System.ComponentModel;
using Xamarin.Forms;
using findmeadrink_mobile.ViewModels;
using Xamarin.Forms.Maps;
using Xamarin.Essentials;
using System.Net.Http;
using Newtonsoft.Json;
using System;
using Pin = Xamarin.Forms.Maps.Pin;
using Microsoft.AppCenter.Crashes;
using System.Threading.Tasks;

namespace findmeadrink_mobile.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class DrinkPage : ContentPage
    {
        DrinkViewModel viewModel;
        public HttpClient httpClient { get; }
        public Xamarin.Essentials.Location location { get; set; }
        public string barName { get; set; }

        public DrinkPage()
        {
            httpClient = new HttpClient();
            InitializeComponent();
            BindingContext = viewModel = new DrinkViewModel();
            OnStartup();
        }

        async void OnStartup()
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Best);
            location = await Geolocation.GetLocationAsync(request);
            Position position = new Position(location.Latitude, location.Longitude);
            MapSpan mapSpan = new MapSpan(position, 0.01, 0.01);
            CityMap.MoveToRegion(mapSpan);
        }

        private async void FindADrink(object sender, EventArgs e)
        {
            try
            {
                LocationsResponse responseObject = await GetLocations();
                Random random = new Random();
                var randomNumber = random.Next(0, responseObject.results.Count - 1);
                if (responseObject.results.Count > 0)
                {
                    location = new Xamarin.Essentials.Location(responseObject.results[randomNumber].geometry.location.lat, responseObject.results[randomNumber].geometry.location.lng);
                    Position position = new Position(location.Latitude, location.Longitude);
                    barName = responseObject.results[randomNumber].name;
                    MapSpan mapSpan = new MapSpan(position, 0.01, 0.01);
                    Pin pin = new Pin
                    {
                        Label = barName,
                        Address = responseObject.results[randomNumber].vicinity,
                        Type = PinType.Place,
                        Position = position
                    };

                    CityMap.MoveToRegion(mapSpan);
                    ResultLabel.Text = "YOUR NEXT DRINK IS AT " + barName;
                    CityMap.Pins.Clear();
                    CityMap.Pins.Add(pin);
                    RouteButton.IsVisible = true;
                    
                }

                else
                {
                    await Application.Current.MainPage.DisplayAlert("ResultsError", "No results", "Dismiss");
                }
            }

            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                await Application.Current.MainPage.DisplayAlert("MainError", ex.Message, "Dismiss");
            }
        }

        private async void RouteToBar(object sender, EventArgs e)
        {
            var options = new MapLaunchOptions { Name = barName };
            await Xamarin.Essentials.Map.OpenAsync(location, options);
        }

        private async Task<LocationsResponse> GetLocations()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                string baseUrl = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=";
                string apiKey = "AIzaSyAaofiKriE1N3bPDVHyp_Jy5GuhVX55bho";
                string lat = location.Latitude.ToString().Replace(',', '.');
                string lng = location.Longitude.ToString().Replace(',', '.');
                string query = baseUrl + lat + "," + lng + "&radius=1000&type=bar&key=" + apiKey;
                response = await httpClient.GetAsync(query);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    LocationsResponse responseObject = JsonConvert.DeserializeObject<LocationsResponse>(responseString);
                    return responseObject;
                }
            }

            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                await Application.Current.MainPage.DisplayAlert("HttpError", ex.Message, "Dismiss");
            }

            await Application.Current.MainPage.DisplayAlert("Result not successful", response.StatusCode.ToString(), "Dismiss");
            return null;
        }
    }

}