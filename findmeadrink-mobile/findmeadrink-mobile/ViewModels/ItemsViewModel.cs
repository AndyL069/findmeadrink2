using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using findmeadrink_mobile.Models;
using findmeadrink_mobile.Views;
using Xamarin.Essentials;
using System.Windows.Input;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using Xamarin.Forms.Maps;
using Map = Xamarin.Forms.Maps.Map;

namespace findmeadrink_mobile.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public Xamarin.Essentials.Location Location { get; set; }

        public ICommand FindDrinkCommand { get; }

        public Map Map { get; set; }


        public ItemsViewModel()
        {
            Title = "Browse";
            FindDrinkCommand = new Command(FindMeADrink);
            OnStartup();
        }

        async void OnStartup()
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Medium);
            var location = await Geolocation.GetLocationAsync(request);
            Location = location;
            if (location != null)
            {
                //Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                Position position = new Position(location.Latitude, location.Longitude);
                MapSpan mapSpan = new MapSpan(position, 0.01, 0.01);
                Map = new Map(mapSpan);
            }
        }

        async void FindMeADrink()
        {
            HttpClient httpClient = new HttpClient();
            string baseUrl = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=";
            //string location = "50.1204088,8.6649881";
            string apiKey = "AIzaSyAaofiKriE1N3bPDVHyp_Jy5GuhVX55bho";
            var result = await httpClient.GetAsync(baseUrl + Location.Latitude + "," + Location.Longitude + "&radius=1000&type=bar&key=" + apiKey);
            var responseString = await result.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<Example>(responseString);
        }
    }

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Northeast
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Southwest
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Viewport
    {
        public Northeast northeast { get; set; }
        public Southwest southwest { get; set; }
    }

    public class Geometry
    {
        public Location location { get; set; }
        public Viewport viewport { get; set; }
    }

    public class OpeningHours
    {
        public bool open_now { get; set; }
    }

    public class Photo
    {
        public int height { get; set; }
        public IList<string> html_attributions { get; set; }
        public string photo_reference { get; set; }
        public int width { get; set; }
    }

    public class PlusCode
    {
        public string compound_code { get; set; }
        public string global_code { get; set; }
    }

    public class Result
    {
        public string business_status { get; set; }
        public Geometry geometry { get; set; }
        public string icon { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public OpeningHours opening_hours { get; set; }
        public IList<Photo> photos { get; set; }
        public string place_id { get; set; }
        public PlusCode plus_code { get; set; }
        public int price_level { get; set; }
        public double rating { get; set; }
        public string reference { get; set; }
        public string scope { get; set; }
        public IList<string> types { get; set; }
        public int user_ratings_total { get; set; }
        public string vicinity { get; set; }
    }

    public class Example
    {
        public IList<object> html_attributions { get; set; }
        public string next_page_token { get; set; }
        public IList<Result> results { get; set; }
        public string status { get; set; }
    }
}