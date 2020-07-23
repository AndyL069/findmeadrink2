using System.ComponentModel;
using Xamarin.Forms;
using findmeadrink_mobile.ViewModels;
using Xamarin.Forms.Maps;
using Xamarin.Essentials;
using System.Net.Http;
using Newtonsoft.Json;
using System;

namespace findmeadrink_mobile.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;
        Xamarin.Essentials.Location location;
        string barName;

        public ItemsPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new ItemsViewModel();
            OnStartup();
        }

        async void OnStartup()
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Best);
            location = await Geolocation.GetLocationAsync(request);
            //location = new Xamarin.Essentials.Location(50.1118331, 8.6608024);
            Position position = new Position(location.Latitude, location.Longitude);
            MapSpan mapSpan = new MapSpan(position, 0.01, 0.01);
            CityMap.MoveToRegion(mapSpan);
        }

        private async void FindADrink(object sender, EventArgs e)
        {
            HttpClient httpClient = new HttpClient();
            string baseUrl = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=";
            string apiKey = "AIzaSyAaofiKriE1N3bPDVHyp_Jy5GuhVX55bho";
            var result = await httpClient.GetAsync(baseUrl + location.Latitude + "," + location.Longitude + "&radius=1000&type=bar&key=" + apiKey);
            var responseString = await result.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<Example>(responseString);
            Random random = new Random();
            var randomNumber = random.Next(0, responseObject.results.Count - 1);
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
            ResultLabel.Text = "YOUR NEXT DRINK IS AT: " + barName;
            CityMap.Pins.Clear();
            CityMap.Pins.Add(pin);
            RouteButton.IsVisible = true;
        }

        private async void RouteToBar(object sender, EventArgs e)
        {
            var location = new Xamarin.Essentials.Location(50.1271753, 8.6660032);
            var options = new MapLaunchOptions { Name = barName };
            await Xamarin.Essentials.Map.OpenAsync(location, options);
        }
    }

}