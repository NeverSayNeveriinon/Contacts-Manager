namespace Web.Middlewares;

public class CustomExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
        
    private readonly ILogger<CustomExceptionHandlingMiddleware> _logger;

    public CustomExceptionHandlingMiddleware(RequestDelegate next, ILogger<CustomExceptionHandlingMiddleware> logger)
    {
        _next = next;
            
        _logger = logger;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            if (ex.InnerException is not null)
            {
                _logger.LogError("~~~ {ExceptionType} : {ExceptionMessage}",ex.InnerException.GetType().ToString(),
                    ex.InnerException.Message);
            }
            else
            {
                _logger.LogError("~~~ {ExceptionType} : {ExceptionMessage}",ex.GetType().ToString(), ex.Message);
            }

            // In Case of using built-in 'UseExceptionHandler' 
            throw;
        }
    }
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class CustomExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomExceptionHandlingMiddleware>();
    }
}