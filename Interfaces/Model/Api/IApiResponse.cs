namespace portfolio_api.Interfaces.Model.Api;

public interface IApiResponse
{
    bool Success    { get; set; }
    string Message  { get; set; }
    string? Error   { get; set; }
    int StatusCode  { get; set; }
}