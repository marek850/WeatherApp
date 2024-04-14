using Newtonsoft.Json;
using System;
using WeCast.Helper;
using forekast.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.ComponentModel;

namespace forekast.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AirQuality : ContentPage, INotifyPropertyChanged
    {

        public AirQuality()
        {
            InitializeComponent();
            this.Appearing += OnLoad;
            Lat = Preferences.Get("lat", "48.1486");
            Lon = Preferences.Get("long", "17.1077");
            GetAirInfo();
            BindingContext = this;

        }

        private void OnLoad(object sender, EventArgs e)
        {
            Lat = Preferences.Get("lat", "48.1486");
            Lon = Preferences.Get("long", "17.1077");
            GetAirInfo();
        }

        

        private double? _aqi = 1;
        [DefaultValue(2)]
        public double AQI {
            get
            {
                return _aqi.Value;
            }
            set
            {
                if (_aqi.Value != value)
                {
                    _aqi = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Lat { get; private set; }
        public string Lon { get; private set; }
       
        private async void GetAirInfo()
        {
            var url = $"https://api.openweathermap.org/data/2.5/air_pollution?lat={Lat}&lon={Lon}&appid=7fe0d7566fe9241f6ab1f3dd1e0b2d3e";
            var result = await ApiCall.GetApi(url);
            if (result.IsSuccessful)
            {
                try
                {
                    var airInformations = JsonConvert.DeserializeObject<AirPollution>(result.Response);
                    AQI = airInformations.List[0].Main.Aqi;
                    noText.Text = airInformations.List[0].Components.no2.ToString();
                    soText.Text = airInformations.List[0].Components.so2.ToString();
                    pmText.Text = airInformations.List[0].Components.pm10.ToString();
                    pm2Text.Text = airInformations.List[0].Components.pm2_5.ToString();
                    oText.Text = airInformations.List[0].Components.o3.ToString();
                    coText.Text = airInformations.List[0].Components.co.ToString();


                }
                catch (Exception e)
                {
                    await DisplayAlert("Informácie o kvalite ovzdusia", "Chyba v spracovaní dát"+e.StackTrace, "OK");
                }
            }
            else
            {
                await DisplayAlert("Informácie o kvalite ovzdusia", "Žiadne informácie o kvalite ovzdusia neboli nájdené", "OK");
            }
        }
    }
}