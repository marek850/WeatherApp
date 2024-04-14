
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace forekast
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Preferences.Set("long", "19.15324");
            Preferences.Set("lat", "48.57442");
            Preferences.Set("Location", "Bratislava");
        }
    }
}
