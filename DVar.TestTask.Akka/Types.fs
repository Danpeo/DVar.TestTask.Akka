namespace DVar.TestTask.Akka.WeatherModels

type WeatherCondition =
  { Text: string
    Icon: string
    Code: int }

type Current =
  { Temp_C: float
    Temp_F: float
    Is_Day: int
    Last_Updated: string
    Condition: WeatherCondition
    Wind_Mph: float
    Wind_Kph: float
    Wind_Degree: float
    Wind_Dir: string
    Humidity: float
    Cloud: float
    Feelslike_C: float
    Feelslike_F: float }

type Location =
  { Name: string
    Region: string
    Country: string
    Lat: float
    Lon: float
    Tz_Id: string
    LocaltimeEpoch: int
    LocalTime: string }

type Day =
  { Maxtemp_C: float
    Avgtemp_C: float
    Condition: WeatherCondition }

type Forecastday = { Date: string; Day: Day }

type Forecast = { Forecastdats: Forecastday array }

type GetCurrentResponse =
  { Location: Location; Current: Current }

type GetCurrentRequest =
  { SearchQuery: string
    ApiKey: string
    Language: string option }

type GetCurrentMessage =
  | GetCurrentRequest of GetCurrentRequest
  | CancelRequest

type GetForecastRequest =
  { Days: int
    GetCurrentRequest: GetCurrentRequest }
  
type GetForecastMessage =
  | GetForecastRequest of GetForecastRequest
  | CancelForecastRequest
