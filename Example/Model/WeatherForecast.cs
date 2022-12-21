using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Raydreams.API.Example.Model;

/// <summary></summary>
public class ForecastDTO
{
    [JsonProperty("date")]
    public DateOnly Date { get; set; }

    [JsonProperty( "c" )]
    public int TemperatureC { get; set; }

    [JsonProperty( "f" )]
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    [JsonProperty( "summary" )]
    public string Summary { get; set; }
}

/// <summary>These classes were auto-generaated from the NWS Forecast JSON response</summary>
public class WeatherForecast
{
    [JsonProperty( "@context" )]
    public List<object> context { get; set; }
    public string type { get; set; }
    public Geometry geometry { get; set; }
    public Properties properties { get; set; }
}

public class Properties
{
    public DateTime updated { get; set; }
    public string units { get; set; }
    public string forecastGenerator { get; set; }
    public DateTime generatedAt { get; set; }
    public DateTime updateTime { get; set; }
    public string validTimes { get; set; }
    public Elevation elevation { get; set; }
    public List<Period> periods { get; set; }
}

public class Period
{
    public int number { get; set; }
    public string name { get; set; }
    public DateTime startTime { get; set; }
    public DateTime endTime { get; set; }
    public bool isDaytime { get; set; }
    public int temperature { get; set; }
    public string temperatureUnit { get; set; }
    public string temperatureTrend { get; set; }
    public string windSpeed { get; set; }
    public string windDirection { get; set; }
    public string icon { get; set; }
    public string shortForecast { get; set; }
    public string detailedForecast { get; set; }
}

public class Elevation
{
    public string unitCode { get; set; }
    public double value { get; set; }
}

public class Geometry
{
    public string type { get; set; }
    public List<List<List<double>>> coordinates { get; set; }
}


