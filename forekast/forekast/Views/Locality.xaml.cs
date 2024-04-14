using forekast.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace forekast.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Locality : ContentPage
	{
        private readonly List<CityInfo> cities;

        public Locality ()
		{
			InitializeComponent ();
            cities = new List<CityInfo>();
            localities.ItemsSource = cities;
            LoadCities();
            
		}

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
			localities.ItemsSource = cities.Where(c => c.Name.ToLower().StartsWith(e.NewTextValue.ToLower()));
        }
        private async void LoadCities()
        {

            try
            {
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(Locality)).Assembly;

                Stream stream = assembly.GetManifestResourceStream("forekast.cities.csv");
                using (var reader = new System.IO.StreamReader(stream))
                {
                    reader.ReadLine();
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        string[] values = line.Split(',');

                        if (values.Length == 4)
                        {
                            string name = values[0];
                            double lon = Convert.ToDouble(values[1]);
                            double lat = Convert.ToDouble(values[2]);
                            string country = values[3];

                            CityInfo cityData = new CityInfo(name, lon, lat, country);
                            cities.Add(cityData);
                        }
                    }

                }
            } catch (Exception e)
            {
                await DisplayAlert("Nacitanie lokalit", "Žiadne informácie o lokalitach neboli nacitane" + e.StackTrace, "OK");
            }

        }
        async void OnImageButtonClicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "GetCurrentLocation");
            await Shell.Current.GoToAsync("///weather");

        }
        private async void ListViewSelected(object sender, SelectedItemChangedEventArgs e)
		{
			var city = e.SelectedItem as CityInfo;
			
            Preferences.Set("long", city.Lon1);
            Preferences.Set("lat", city.Lat1);
            Preferences.Set("Location", city.Name);
            await Shell.Current.GoToAsync("///weather");
        }
        
    }
}