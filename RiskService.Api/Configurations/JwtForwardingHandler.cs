namespace RiskService.Api.Configurations;

public class JwtForwardingHandler(
    IHttpContextAccessor httpContextAccessor)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        string? authorizationHeader =
            httpContextAccessor
                .HttpContext?
                .Request
                .Headers
                .Authorization
                .FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(authorizationHeader))
        {
            request.Headers.TryAddWithoutValidation(
                "Authorization",
                authorizationHeader);
        }

        return await base.SendAsync(
            request,
            cancellationToken);
    }
}