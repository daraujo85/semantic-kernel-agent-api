using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;

public class WeatherPlugin
{
    private readonly HttpClient _httpClient = new();
    private readonly string _apiKey;

    public WeatherPlugin(string apiKey)
    {
        _apiKey = apiKey;
    }

    [KernelFunction, Description("Get weather info for a given city")]
    public async Task<string> GetWeatherAsync(string city)
    {
        var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric&lang=pt_br";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return $"Não foi possível obter a previsão para {city}.";

        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);

        var weather = json.RootElement.GetProperty("weather")[0].GetProperty("description").GetString();
        var temp = json.RootElement.GetProperty("main").GetProperty("temp").GetDecimal();

        return $"Em {city}, o tempo está '{weather}' com temperatura de {temp}°C.";
    }
}