using AppG.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace AppG
{
    public class CustomMap :Map
    {
        public List<CustomPin> CustomPins { get; set; }

    }
}
