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
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            if (dict == null) return "";

            var sensitiveKeys = new[] { "password", "token", "authorization", "secret", "key" };

            foreach (var key in sensitiveKeys)
            {
                var match = dict.Keys.FirstOrDefault(k => k.Equals(key, StringComparison.OrdinalIgnoreCase));
                if (match != null)
                {
                    dict[match] = "***";
                }
            }

            return System.Text.Json.JsonSerializer.Serialize(dict);
        }
    }
}