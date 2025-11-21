namespace MeandAI.Api;

public static class Settings
{
    public static string DatabaseConnectionString =>
        Environment.GetEnvironmentVariable("MEANDAI_DB_CONNECTION")
        ?? string.Empty;
}
