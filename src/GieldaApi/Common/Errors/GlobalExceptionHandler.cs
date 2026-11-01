using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GieldaApi.Common.Errors;

// Jedno miejsce na obsługę błędów. Każdy błąd wraca w standardzie ProblemDetails (RFC 9457):
// { "type": ..., "title": ..., "status": 404, "detail": ... }
public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Nie znaleziono"),
            ConflictException => (StatusCodes.Status409Conflict, "Konflikt"),
            _ => (StatusCodes.Status500InternalServerError, "Błąd serwera"),
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Nieobsłużony wyjątek");
        }

        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                // Szczegółów błędu 500 nie pokazujemy klientowi – mogą zdradzać wnętrze systemu.
                Detail = status == StatusCodes.Status500InternalServerError ? null : exception.Message,
            },
        });
    }
}
