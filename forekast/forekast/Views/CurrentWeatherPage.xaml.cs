using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using forekast.Views;
using Newtonsoft.Json;
using WeCast.Helper;
using WeCast.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeCast.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CurrentWeatherPage : ContentPage
    {
        public CurrentWeatherPage()
        {
            InitializeComponent();
            _ = GetCoordinate();
            this.Appearing += OnLoad;
            MessagingCenter.Subscribe<Locality>(this, "GetCurrentLocation", async (sender) =>
            {

                await GetCoordinate();
                
            });
        }
        private void OnLoad(object sender, EventArgs e)
        {
            Location = Preferences.Get("Location", "Bratislava");
            GetWeatherInfo();
            GetForecastInfo();
            GetIntraDayForecastInfo();
            GetBackgroundImage();

        }
        private string Location { get; set; } ="Bratislava";
        private double Latitude { get; set; }
        private double Longtitude { get; set; }

        private async Task GetCoordinate()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(20));
              
                var location = await Geolocation.GetLocationAsync(request);
                if (location != null)
                {
                    Latitude = location.Latitude;  
                    Longtitude = location.Longitude;
                    Location = await GetCity(location);
                    Preferences.Set("Location", Location);
                    Preferences.Set("long", Longtitude);
                    Preferences.Set("lat", Latitude);
                    GetWeatherInfo();
                    GetForecastInfo();
                    GetIntraDayForecastInfo();
                    GetBackgroundImage();

                }
            }
            catch (Exception e)
            {
                await DisplayAlert("Lokalizacia", "Chyba v ziskani aktualnej polohy" + e.StackTrace, "OK");
            }
        }

        private async Task<string> GetCity(Location location)
        {
            var placemarks = await Geocoding.GetPlacemarksAsync(location);

            var currentPlace = placemarks?.FirstOrDefault();
            if (currentPlace != null)
            {
                return $"{currentPlace.SubLocality}, {currentPlace.CountryName}";
            }

            return null;
        }

        private async void GetWeatherInfo()
        {
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={Location}&appid=7fe0d7566fe9241f6ab1f3dd1e0b2d3e&units=metric";
            var result = await ApiCall.GetApi(url);
            if (result.IsSuccessful)
            {
                try
                {
                    var weatherInformations = JsonConvert.DeserializeObject<WeatherInfo>(result.Response);
                    descriptionText.Text = weatherInformations.weather[0].description.ToUpper();
                    iconImage.Source = $"w{weatherInformations.weather[0].icon}";
                    cityText.Text = weatherInformations.name.ToUpper();
                    temperatureText.Text = weatherInformations.main.temp.ToString("0");
                    humidityText.Text = $"{weatherInformations.main.humidity}%";
                    pressureText.Text = $"{weatherInformations.main.pressure} hpa";
                    windText.Text = $"{weatherInformations.wind.speed} m/s";
                    cloudinessText.Text = $"{weatherInformations.clouds.all}%";

                    var dt = DateTime.Now;
                    dayText.Text = dt.ToString("dddd dd MMM yyyy");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            else
            {
                await DisplayAlert("Informácie o počasí", "Žiadne informácie o počasí neboli nájdené", "OK");
            }
        }
        

        private async void GetForecastInfo()
        {
            var url = $"https://api.openweathermap.org/data/2.5/forecast?q={Location}&appid=7fe0d7566fe9241f6ab1f3dd1e0b2d3e&units=metric";
            var result = await ApiCall.GetApi(url);
            if (result.IsSuccessful)
            {
                try
                {
                    var forcastInfo = JsonConvert.DeserializeObject<ForecastInfo>(result.Response);

                    List<List> allList = new List<List>();

                    foreach (var list in forcastInfo.list)
                    {
                        var date = DateTime.Parse(list.dt_txt);

                        if (date > DateTime.Today && date.Hour == 12 && date.Minute == 0 && date.Second == 0)
                            allList.Add(list);
                    }

                    dayOneText.Text = DateTime.Parse(allList[0].dt_txt).ToString("dddd");
                    dateOneText.Text = DateTime.Parse(allList[0].dt_txt).ToString("dd MMM");
                    iconOneImage.Source = $"w{allList[0].weather[0].icon}";
                    temperatureOneText.Text = allList[0].main.temp.ToString("0");

                    dayTwoText.Text = DateTime.Parse(allList[1].dt_txt).ToString("dddd");
                    dateTwoText.Text = DateTime.Parse(allList[1].dt_txt).ToString("dd MMM");
                    iconTwoImage.Source = $"w{allList[1].weather[0].icon}";
                    temperatureTwoText.Text = allList[1].main.temp.ToString("0");

                    dayThreeText.Text = DateTime.Parse(allList[2].dt_txt).ToString("dddd");
                    dateThreeText.Text = DateTime.Parse(allList[2].dt_txt).ToString("dd MMM");
                    iconThreeImage.Source = $"w{allList[2].weather[0].icon}";
                    temperatureThreeText.Text = allList[2].main.temp.ToString("0");

                    dayFourText.Text = DateTime.Parse(allList[3].dt_txt).ToString("dddd");
                    dateFourText.Text = DateTime.Parse(allList[3].dt_txt).ToString("dd MMM");
                    iconFourImage.Source = $"w{allList[3].weather[0].icon}";
                    temperatureFourText.Text = allList[3].main.temp.ToString("0");

                }
                catch (Exception ex)
                {
                    await DisplayAlert("Weather Info", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Weather Info", "No forecast information found", "OK");
            }
        }

        private async void GetBackgroundImage()
        {
            var url = $"https://api.pexels.com/v1/search?query={Location}&per_page=15&page=1";
            var results = await ApiCall.GetApi(url, "iBZrtykEFfsCMaH3UwzYID8kUsvwJwtZ0vOyBncn4zC4MN83Cvy31Qle");
            if (results.IsSuccessful)
            {
                var backgroundInfo = JsonConvert.DeserializeObject<BackgroundInfo>(results.Response);
                if (backgroundInfo != null && backgroundInfo.photos.Length > 0)
                {
                    bgImg.Source = ImageSource.FromUri(new Uri(backgroundInfo.photos[new Random().Next(backgroundInfo.photos.Length)].src.medium));
                }
                
            }
        }
        private async void GetIntraDayForecastInfo()
        {
            var url = $"https://api.openweathermap.org/data/2.5/forecast?q={Location}&appid=7fe0d7566fe9241f6ab1f3dd1e0b2d3e&units=metric";
            var result = await ApiCall.GetApi(url);
            if (result.IsSuccessful)
            {
                try
                {
                    var forcastInfo = JsonConvert.DeserializeObject<ForecastInfo>(result.Response);

                    List<List> allList = new List<List>();

                    foreach (var list in forcastInfo.list)
                    {
                        var date = DateTime.Parse(list.dt_txt);

                        if (date > DateTime.Now && date < DateTime.Now.AddHours(24))
                            allList.Add(list);
                    }

                    hourOneDay.Text = DateTime.Parse(allList[0].dt_txt).DayOfWeek.ToString().Substring(0, 3);
                    hourOneText.Text = DateTime.Parse(allList[0].dt_txt).ToString("HH");
                    iconHourOneImage.Source = $"w{allList[0].weather[0].icon}";
                    temperatureHourOneText.Text = allList[0].main.temp.ToString("0");

                    hourTwoDay.Text = DateTime.Parse(allList[1].dt_txt).DayOfWeek.ToString().Substring(0, 3);
                    hourTwoText.Text = DateTime.Parse(allList[1].dt_txt).ToString("HH");
                    iconHourTwoImage.Source = $"w{allList[1].weather[0].icon}";
                    temperatureHourTwoText.Text = allList[1].main.temp.ToString("0");

                    hourThreeDay.Text = DateTime.Parse(allList[2].dt_txt).DayOfWeek.ToString().Substring(0, 3);
                    hourThreeText.Text = DateTime.Parse(allList[2].dt_txt).ToString("HH");
                    iconHourThreeImage.Source = $"w{allList[2].weather[0].icon}";
                    temperatureHourThreeText.Text = allList[2].main.temp.ToString("0");

                    hourFourDay.Text = DateTime.Parse(allList[3].dt_txt).DayOfWeek.ToString().Substring(0, 3);
                    hourFourText.Text = DateTime.Parse(allList[3].dt_txt).ToString("HH");
                    iconHourFourImage.Source = $"w{allList[3].weather[0].icon}";
                    temperatureHourFourText.Text = allList[3].main.temp.ToString("0");

                    hourFiveDay.Text = DateTime.Parse(allList[4].dt_txt).DayOfWeek.ToString().Substring(0, 3);
                    hourFiveText.Text = DateTime.Parse(allList[4].dt_txt).ToString("HH");
                    iconHourFiveImage.Source = $"w{allList[4].weather[0].icon}";
                    temperatureHourFiveText.Text = allList[4].main.temp.ToString("0");

                    hourSixDay.Text = DateTime.Parse(allList[5].dt_txt).DayOfWeek.ToString().Substring(0, 3);
                    hourSixText.Text = DateTime.Parse(allList[5].dt_txt).ToString("HH");
                    iconHourSixImage.Source = $"w{allList[5].weather[0].icon}";
                    temperatureHourSixText.Text = allList[5].main.temp.ToString("0");

                    hourSevenDay.Text = DateTime.Parse(allList[6].dt_txt).DayOfWeek.ToString().Substring(0, 3);
                    hourSevenText.Text = DateTime.Parse(allList[6].dt_txt).ToString("HH");
                    iconHourSevenImage.Source = $"w{allList[6].weather[0].icon}";
                    temperatureHourSevenText.Text = allList[6].main.temp.ToString("0");

                    hourEightDay.Text = DateTime.Parse(allList[7].dt_txt).DayOfWeek.ToString().Substring(0, 3);
                    hourEightText.Text = DateTime.Parse(allList[7].dt_txt).ToString("HH");
                    iconHourEightImage.Source = $"w{allList[7].weather[0].icon}";
                    temperatureHourEightText.Text = allList[7].main.temp.ToString("0");

                }
                catch (Exception ex)
                {
                    await DisplayAlert("Weather Info", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Weather Info", "No forecast information found", "OK");
            }
        }

    }
}