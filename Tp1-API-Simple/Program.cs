using System.Runtime.InteropServices;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () =>
{
    // 1. Check Non-Root
    bool isRoot = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && getuid() == 0;

    // 2. Check Timezone (On vérifie l'ID et le nom standard)
    string tzId = TimeZoneInfo.Local.Id;
    string tzName = TimeZoneInfo.Local.StandardName;

    bool isParis = tzId == "Europe/Paris"
                   || tzName.Contains("Paris")
                   || tzName.Contains("Central European");

    // 3. Check Memory Limit (Vérifie si une limite < 200MB est imposée)
    // Sous Linux, .NET lit les cgroups
    long memLimit = Process.GetCurrentProcess().MaxWorkingSet.ToInt64();
    bool hasMemLimit = memLimit > 0 && memLimit < 500_000_000; // Vérifie si bridé

    // 4. Check Read-Only Filesystem
    bool isReadOnly = false;
    try
    {
        File.WriteAllText("/app/test_readonly.txt", "test");
        File.Delete("/app/test_readonly.txt");
    }
    catch
    {
        isReadOnly = true; // Si l'écriture échoue, c'est que c'est bien Read-Only !
    }
    var report = $"""
    ==========================================
    DOCKER DIAGNOSTIC
    ==========================================
    [{(isRoot ? "Non" : "OK")}] SECURITY : Running as Non-Root
    [{(isReadOnly ? "OK" : "Non")}] SECURITY : Immutable FileSystem (Read-only)
    [{(isParis ? "OK" : "Non")}] CONFIG   : Timezone set to Europe/Paris
    [{(hasMemLimit ? "OK" : "Non")}] RESOURCE : Memory Quota enforced (<500MB)
    ==========================================
    SYSTEM TIME : {DateTime.Now:dd/MM/yyyy HH:mm:ss}
    RESULTAT    : {(!isRoot && isReadOnly && isParis && hasMemLimit ? "PASSED" : "FAILED")}
    ==========================================
    """;

    return Results.Text(report);
});

app.Run();

[DllImport("libc")] static extern uint getuid();