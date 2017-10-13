using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using AppG;
using AppG.Droid;
using Xamarin.Forms.Maps.Android;
using AppG.Pages;
using Android.Gms.Maps;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Maps;
using Android.Gms.Maps.Model;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace AppG.Droid
{
    public class CustomMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter, GoogleMap.IOnMarkerDragListener, GoogleMap.IOnMapClickListener
    {
        List<CustomPin> customPins;
        bool isDrawn;
        CustomMap cMap;

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                NativeMap.InfoWindowClick -= OnInfoWindowClick;
               // NativeMap.MapClick -= NativeMap_MapClick;
                NativeMap.MarkerDragEnd -= NativeMap_MarkerDragEnd;
                NativeMap.MarkerDragStart -= NativeMap_MarkerDragStart;
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                customPins = formsMap.CustomPins;
                Control.GetMapAsync(this);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName.Equals("VisibleRegion") && !isDrawn)
            {
                NativeMap.Clear();
                NativeMap.InfoWindowClick += OnInfoWindowClick;
                NativeMap.SetInfoWindowAdapter(this);

                foreach (var pin in customPins)
                {
                    var marker = new MarkerOptions();
                    marker.SetPosition(new LatLng(pin.Pin.Position.Latitude, pin.Pin.Position.Longitude));
                    marker.SetTitle(pin.Pin.Label);
                    marker.SetSnippet(pin.Pin.Address);
                    marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pin));
                    marker.Draggable(true);
                  //  NativeMap.MapClick += NativeMap_MapClick;
                    NativeMap.MarkerDragEnd += NativeMap_MarkerDragEnd;
                    NativeMap.MarkerDragStart += NativeMap_MarkerDragStart;
                    NativeMap.AddMarker(marker);
                }
                isDrawn = true;
              
            }
        }

        private void NativeMap_MarkerDragStart(object sender, GoogleMap.MarkerDragStartEventArgs e)
        {
            OnMarkerDragStart(e.Marker);
        }

        private void NativeMap_MarkerDragEnd(object sender, GoogleMap.MarkerDragEndEventArgs e)
        {
            OnMarkerDragEnd(e.Marker);
        }

        private void NativeMap_MapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            OnMapClick(e.Point);
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            if (changed)
            {
                isDrawn = false;
            }
        }

        void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            var customPin = GetCustomPin(e.Marker);
            if (customPin == null)
            {
                throw new Exception("Custom pin not found");
            }

            if (!string.IsNullOrWhiteSpace(customPin.Url))
            {
                var url = Android.Net.Uri.Parse(customPin.Url);
                var intent = new Intent(Intent.ActionView, url);
                intent.AddFlags(ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);
            }
        }

        public Android.Views.View GetInfoContents(Marker marker)
        {
            var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
            if (inflater != null)
            {
                Android.Views.View view;

                var customPin = GetCustomPin(marker);
                if (customPin == null)
                {
                    throw new Exception("Custom pin not found");
                }

                if (customPin.Id == "Xamarin")
                {
                    view = inflater.Inflate(Resource.Layout.XamarinMapInfoWindow, null);
                }
                else
                {
                    view = inflater.Inflate(Resource.Layout.MapInfoWindow, null);
                }

                var infoTitle = view.FindViewById<TextView>(Resource.Id.InfoWindowTitle);
                var infoSubtitle = view.FindViewById<TextView>(Resource.Id.InfoWindowSubtitle);

                if (infoTitle != null)
                {
                    infoTitle.Text = marker.Title;
                }
                if (infoSubtitle != null)
                {
                    infoSubtitle.Text = marker.Snippet;
                }

                return view;
            }
            return null;
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }

        CustomPin GetCustomPin(Marker annotation)
        {
            var position = new Position(annotation.Position.Latitude, annotation.Position.Longitude);
            foreach (var pin in customPins)
            {
                if (pin.Pin.Position == position)
                {
                    return pin;
                }
            }
            return null;
        }

        public void OnMarkerDrag(Marker marker)
        {
            throw new NotImplementedException();
        }

        public void OnMarkerDragEnd(Marker marker)
        {
            LatLng dragPosition = marker.Position;
            double dragLat = dragPosition.Latitude;
            double dragLong = dragPosition.Longitude;
            // Log.i("info", "on drag end :" + dragLat + " dragLong :" + dragLong);

            // Toast.makeText(getApplicationContext(), "Marker Dragged..!", Toast.LENGTH_LONG).show();

            var marker2 = new MarkerOptions();
            marker2.SetPosition(new LatLng(dragLat, dragLong));
            marker2.SetTitle("New Point");
           // marker2.SetSnippet("New Address");
            marker2.SetSnippet("Latitude:" + dragLat + " Longitude:" + dragLong);
            marker2.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pin));
            marker2.Draggable(true);

            NativeMap.AddMarker(marker2);

           // NativeMap.Clear();
            var pin = new CustomPin
            {
                Pin = new Pin
                {
                    Type = PinType.Place,
                    Position = new Position(dragLat, dragLong),
                    Label = "Xamarin San Francisco Office",
                    Address = "394 Pacific Ave, San Francisco CA"
                },
                Id = "Xamarin",
                Url = "http://xamarin.com/about/"
            };
            customPins.Add(pin);
        }

        public void OnMarkerDragStart(Marker marker)
        {
            
        }

        public void OnMapClick(LatLng point)
        {
            NativeMap.AnimateCamera(CameraUpdateFactory.NewLatLng(point));
            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(point.Latitude, point.Longitude));
            marker.SetTitle("New Title");
            marker.SetSnippet("Latitude:"+ point.Latitude+ " Longitude:"+ point.Latitude);
            marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pin));
            NativeMap.AddMarker(marker);

            NativeMap.Clear();
            var pin = new CustomPin
            {
                Pin = new Pin
                {
                    Type = PinType.Place,
                    Position = new Position(point.Latitude, point.Longitude),
                    Label = "Xamarin San Francisco Office",
                    Address = "394 Pacific Ave, San Francisco CA"
                },
                Id = "Xamarin",
                Url = "http://xamarin.com/about/"
            };
            customPins.Add(pin);
            //cMap.an
            // map.animateCamera(CameraUpdateFactory.newLatLng(arg0));
            // NewCameraPosition

        }
    }
}