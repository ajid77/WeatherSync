using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using GTA;
using GTA.Native;

public class WeatherSync : Script
{
    private string apiKey;
    private string location;

    public WeatherSync()
    {
        // Load settings from .ini file
        LoadSettings();

        Tick += OnTick;
    }

    private void LoadSettings()
    {
        ScriptSettings settings = ScriptSettings.Load("scripts\\WeatherSync.ini");
        apiKey = settings.GetValue("Settings", "apiKey", "YOUR_API_KEY");
        location = settings.GetValue("Settings", "location", "YOUR_LOCATION");
    }

    private async void OnTick(object sender, EventArgs e)
    {
        string weather = await GetWeather();
        SetGameWeather(weather);
        await Task.Delay(60000); // Update every minute
    }

    private async Task<string> GetWeather()
    {
        using (HttpClient client = new HttpClient())
        {
            string url = $"http://api.openweathermap.org/data/2.5/weather?q={location}&appid={apiKey}&units=metric";
            string response = await client.GetStringAsync(url);
            JObject weatherData = JObject.Parse(response);
            string condition = weatherData["weather"][0]["main"].ToString();

            return condition;
        }
    }

    private void SetGameWeather(string condition)
    {
        string weatherType;

        switch (condition.ToLower())
        {
            case "clear":
            case "sunny":
                weatherType = "CLEAR";
                break;
            case "clouds":
            case "cloudy":
                weatherType = "CLOUDS";
                break;
            case "rain":
                weatherType = "RAIN";
                break;
            case "thunderstorm":
                weatherType = "THUNDER";
                break;
            case "fog":
                weatherType = "FOG";
                break;
            case "snow":
                weatherType = "SNOW";
                break;
            default:
                weatherType = "CLEAR"; // Default to clear if condition doesn't match
                break;
        }

        Function.Call(Hash.SET_WEATHER_TYPE_NOW_PERSIST, weatherType);
    }
}
