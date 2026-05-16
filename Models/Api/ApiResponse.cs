using portfolio_api.Interfaces.Model.Api;
namespace portfolio_api.Models.Api;

public class ApiResponse : IApiResponse
{
    public bool Success    { get; set; }
    public string Message  { get; set; } = string.Empty;
    public string? Error   { get; set; }
    public int StatusCode  { get; set; }
}