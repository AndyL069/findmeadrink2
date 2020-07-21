using System.ComponentModel;
using Xamarin.Forms;
using findmeadrink_mobile.ViewModels;
using Xamarin.Forms.Maps;
using Xamarin.Essentials;
using Map = Xamarin.Forms.Maps.Map;
using System.Net.Http;
using Newtonsoft.Json;

namespace findmeadrink_mobile.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;
        Xamarin.Essentials.Location location;
        Map map;
        public ItemsPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new ItemsViewModel();
            OnStartup();
        }

        async void OnStartup()
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Best);
            var location = await Geolocation.GetLocationAsync(request);
            //location = new Xamarin.Essentials.Location(50.1118331, 8.6608024);
            Position position = new Position(location.Latitude, location.Longitude);
            MapSpan mapSpan = new MapSpan(position, 0.01, 0.01);
            map = new Map(mapSpan);
            MainGrid.Children.Add(map, 0, 0);
            Button button = new Button
            {
                BackgroundColor = Color.FromHex("FF1D7A88"),
                Text = "Find me a drink",
                TextColor = Color.White,
                Command = new Command(FindADrink)
            };
            MainGrid.Children.Add(button, 0, 1);
        }

        async void FindADrink()
        {
            HttpClient httpClient = new HttpClient();
            string baseUrl = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=";
            //string location = "50.1204088,8.6649881";
            string apiKey = "AIzaSyAaofiKriE1N3bPDVHyp_Jy5GuhVX55bho";
            var result = await httpClient.GetAsync(baseUrl + location.Latitude + "," + location.Longitude + "&radius=1000&type=bar&key=" + apiKey);
            var responseString = await result.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<Example>(responseString);
            Pin pin = new Pin
            {
                Label = responseObject.results[0].name,
                Address = "",
                Type = PinType.Place,
                Position = new Position(responseObject.results[0].geometry.location.lat, responseObject.results[0].geometry.location.lng)
            };
            Position position = new Position(responseObject.results[0].geometry.location.lat, responseObject.results[0].geometry.location.lng);
            MapSpan mapSpan = new MapSpan(position, 0.01, 0.01);
            map = new Map(mapSpan);
            map.Pins.Add(pin);
        }
    }

}