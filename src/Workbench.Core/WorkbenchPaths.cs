namespace Workbench.Core;

public static class WorkbenchPaths
{
    public static string FindRepositoryRoot(string? start = null)
    {
        DirectoryInfo? directory = new(start ?? Directory.GetCurrentDirectory());

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Mechnain.PicoGK.Workbench.sln")) ||
                Directory.Exists(Path.Combine(directory.FullName, "generators")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return Directory.GetCurrentDirectory();
    }

    public static string Timestamp()
    {
        return DateTimeOffset.Now.ToString("yyyyMMdd_HHmmss");
    }

    public static string SanitizeSegment(string value)
    {
        string cleaned = string.Join("_", value.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
        cleaned = cleaned.Replace(' ', '_').Trim('_');
        return string.IsNullOrWhiteSpace(cleaned) ? "default" : cleaned;
    }
}
