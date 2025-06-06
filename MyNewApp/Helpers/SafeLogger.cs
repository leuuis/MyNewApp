using Microsoft.Extensions.Logging;

namespace MyNewApp.Helpers
{
    public static class SafeLogger
    {
        public static void LogInfoSafe(ILogger logger, string message, object? data = null)
        {
            logger.LogInformation("{Message} {SanitizedData}", message, Sanitize(data));
        }

        public static void LogErrorSafe(ILogger logger, Exception ex, string message, object? data = null)
        {
            logger.LogError(ex, "{Message} {SanitizedData}", message, Sanitize(data));
        }

        private static object Sanitize(object? data)
        {
            if (data is null) return "";
            var json = System.Text.Json.JsonSerializer.Serialize(data);

            json = json
                .Replace("password", "***", StringComparison.OrdinalIgnoreCase)
                .Replace("token", "***", StringComparison.OrdinalIgnoreCase)
                .Replace("authorization", "***", StringComparison.OrdinalIgnoreCase);

            return json;
        }
    }
}