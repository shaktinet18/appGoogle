using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace AppG.Pages
{
    public partial class MapPage : ContentPage
    {
        SearchBar searchBar;
        Label resultsLabel;
        CustomPin LocationPin;
        StackLayout stack = new StackLayout { Spacing = 0 };
        public MapPage()
        {

            InitializeComponent();

            var pin = new CustomPin
            {
                Pin = new Pin
                {
                    Type = PinType.Place,
                    Position = new Position(37.79752, -122.40183),
                    Label = "Xamarin San Francisco Office",
                    Address = "394 Pacific Ave, San Francisco CA"
                },
                Id = "Xamarin",
                Url = "http://xamarin.com/about/"
            };

            customMap.CustomPins = new List<CustomPin> { pin };
            customMap.Pins.Add(pin.Pin);
            customMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(37.79752, -122.40183), Distance.FromMiles(1.0)));

            //var MyMap = new Map(
            //MapSpan.FromCenterAndRadius(
            //        new Position(37, -122), Distance.FromMiles(0.3)))
            //{
            //    IsShowingUser = true,
            //    HeightRequest = 100,
            //    WidthRequest = 960,
            //    VerticalOptions = LayoutOptions.FillAndExpand
            //};

            //var oMap = new CustomMap {
            //    MapType = MapType.Street,
            //    WidthRequest=App.ScreenWidth,
            //    HeightRequest=App.ScreenHeight

            //};
            //MyMap.MapType = MapType.Satellite;

            //resultsLabel = new Label
            //{
            //    Text = "Result will appear here.",
            //    VerticalOptions = LayoutOptions.FillAndExpand,
            //    FontSize = 25
            //};

            //searchBar = new SearchBar
            //{
            //    Placeholder = "Enter search term",
            //    SearchCommand = new Command(() => { resultsLabel.Text = "Result: " + searchBar.Text + " is what you asked for."; })
            //};


            //stack.Children.Add(map);
            //Content = stack;
            //MyMap = new Map();
            //MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(37, -122), Distance.FromMiles(1)));            
            //stack.Children.Add(searchBar);

            //GetCurrenLocation(MyMap);
            //Locator();




        }

        internal async void Locator()
        {
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;
                var position = await locator.GetPositionAsync(TimeSpan.FromMilliseconds(10000));

                LocationPin = new CustomPin();

                //var pin = GetPin(position);
                LocationPin.Pin = new Pin
                {
                    Type = PinType.Place,
                    Position = new Position(position.Latitude, position.Longitude),
                    Label = "",
                    Address = ""
                };

                // LocationMap.Pins.Add(LocationPin.Pin);
                // LocationMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude),
                //Distance.FromMiles(1)));
                //To Remove Pins 
                //for (int x = 1; x < LocationMap.Pins.Count; x++)
                //{ LocationMap.Pins.RemoveAt(1); }//.Remove(customMap.Pins[x]); }
                //                                 // To customize pins

                //for (int x = 1; x < LocationMap.Pins.Count; x++)
                //{   LocationMap.Pins.}
            }
            catch (Exception ex)
            {

                throw;
            }


        }

        public async void GetCurrenLocation(Map MyMap)
        {
            GPSMapper gp = new GPSMapper();
            Task<Plugin.Geolocator.Abstractions.Position> ta = gp.GetCurrentLocation();
            Plugin.Geolocator.Abstractions.Position ps = await ta;
            Position currentPos = new Position(ps.Latitude, ps.Longitude);
            MyMap.Pins.Add(new Pin { Type = PinType.Place, Position = currentPos, Label = "Ezimax", Address = "Noida" });


            // var position = new Position(37, -122); // Latitude, Longitude
            //var pin = new Pin
            //{
            //    Type = PinType.Place,
            //    Position = currentPos,
            //    Label = "custom pin",
            //    Address = "custom detail info"
            //};
            //MyMap.Pins.Add(pin);

            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(currentPos, Distance.FromMiles(100)));

            
           
            stack.Children.Add(MyMap);
            Content = stack;
        }
    }
}
