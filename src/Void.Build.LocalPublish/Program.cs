/* Copyright (C) 2023 Tomat & Contributors
 * 
 * Licensed under the GNU Lesser General Public License, version 2.1; you may
 * not use this file except in compliance with the License.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License
 * for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NuGet.Configuration;

namespace Void.Build.LocalPublish;

internal static class Program {
    // constants
    private const string src = "src";
    private const string csproj = ".csproj";

    // settings
    private const string configuration = "Release";
    private const string nuget_repository = "VoidLocalSources";

    // projects
    private const string void_build = "Void.Build";
    private const string void_build_nuget = "Void.Build.NuGet";
    private const string void_build_localpublish = "Void.Build.LocalPublish";
    private const string void_optimizer = "Void.Optimizer";

    // frameworks
    private const string netstandard2_0 = "netstandard2.0";
    private const string net6 = "net6.0";
    private const string net7 = "net7.0";

    public static void Main() {
        // Normalize the working directory to produce reliable results no matter
        // where this project is run from. Useful for both running from the
        // publish.sh script and running from the IDE.
        var cwd = NormalizeWorkingDirectory(Directory.GetCurrentDirectory());
        var srcDir = Path.Combine(cwd, src);

        Console.WriteLine("cwd: " + cwd);
        Console.WriteLine("srcDir: " + srcDir);

        FindOrCreateLocalNuGetRepository(cwd);

        var nugetCache = GetNuGetCache();
        DeleteNuGetCaches(
            nugetCache,
            "Tomat." + void_build_nuget
        );

        DeletePackages(
            srcDir,
            void_build_nuget
        );

        BuildProjects(
            srcDir,
            void_build,
            void_build_nuget
        );

        PublishPackages(
            srcDir,
            void_build_nuget
        );
    }

    private static string NormalizeWorkingDirectory(string cwd) {
        if (!Directory.Exists(cwd))
            throw new DirectoryNotFoundException($"Directory '{cwd}' not found!");

        if (File.Exists(cwd))
            throw new DirectoryNotFoundException($"File '{cwd}' is not a directory!");

        var dirName = Path.GetFileName(cwd);

        string up(int levels) {
            var result = cwd;
            for (var i = 0; i < levels; i++)
                result = Path.Combine(result, "..");
            return result;
        }

        switch (dirName) {
            // We can assume this is the root directory since it matches the
            // repository name.
            case "ror2":
                return cwd;

            case src:
                return up(1);

            case void_build:
            case void_build_nuget:
            case void_build_localpublish:
            case void_optimizer:
                return up(2);

            case netstandard2_0:
            case net6:
            case net7:
                return up(5);

            default:
                var files = Directory.GetFiles(cwd);

                // Assume that this is the root directory if publish.sh is
                // present. I would also check for ./.git/, but there's no
                // guarantee that the user would have cloned the repository
                // instead of just downloading an archive, and there's no reason
                // to enforce it either.

                if (files.Contains("publish.sh"))
                    return cwd;

                throw new DirectoryNotFoundException($"Directory '{cwd}' was not detected as the root directory and was not able to be traversed!");
        }
    }

    private static string GetNuGetCache() {
        string unix() {
            var xdgCacheHome = Environment.GetEnvironmentVariable("XDG_CACHE_HOME");
            if (!string.IsNullOrEmpty(xdgCacheHome))
                return Path.Combine(xdgCacheHome, "NuGetPackages");

            var home = Environment.GetEnvironmentVariable("HOME");
            if (!string.IsNullOrEmpty(home))
                return Path.Combine(home, ".cache", "NuGetPackages");

            throw new PlatformNotSupportedException("Unsupported platform; contribute support for your OS' paths!");
        }

        return Environment.OSVersion.Platform switch {
            PlatformID.Win32NT => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages"),
            PlatformID.MacOSX => unix(),
            PlatformID.Unix => unix(),
            _ => throw new PlatformNotSupportedException("Unsupported platform; contribute support for your OS' paths!")
        };
    }

    private static void DeleteNuGetCaches(string cacheDir, params string[] packages) {
        packages = packages.Select(x => x.ToLowerInvariant()).ToArray();
        
        var cache = new DirectoryInfo(cacheDir);
        var pkgDirs = cache.GetDirectories().Where(x => packages.Contains(x.Name));

        foreach (var pkgDir in pkgDirs) {
            Console.WriteLine($"Deleting '{pkgDir.FullName}'...");
            pkgDir.Delete(true);
        }
    }

    private static void DeletePackages(string srcDir, params string[] projectNames) {
        foreach (var projectName in projectNames) {
            var dir = new DirectoryInfo(Path.Combine(srcDir, projectName));

            foreach (var nupkg in dir.GetFiles("*.nupkg", SearchOption.AllDirectories)) {
                Console.WriteLine($"Deleting '{nupkg.FullName}'...");
                nupkg.Delete();
            }
        }
    }

    private static void BuildProjects(string srcDir, params string[] projectFiles) {
        foreach (var project in projectFiles) {
            var projectFile = Path.Combine(srcDir, project, project + csproj);
            Console.WriteLine($"Building '{projectFile}'...");
            var build = new ProcessStartInfo("dotnet", $"build {projectFile} -c {configuration}") {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            var buildProcess = Process.Start(build);
            buildProcess?.WaitForExit();
        }
    }

    private static void FindOrCreateLocalNuGetRepository(string cwd) {
        var settings = Settings.LoadDefaultSettings(null);
        var sourceProvider = new PackageSourceProvider(settings);
        var nugetRepo = sourceProvider.LoadPackageSources().FirstOrDefault(x => x.Name == nuget_repository);

        if (nugetRepo is null) {
            Console.WriteLine($"NuGet repository '{nuget_repository}' does not exist. Creating...");

            var repoDir = Path.Combine(cwd, "nuget");
            if (File.Exists(repoDir))
                throw new DirectoryNotFoundException($"File '{repoDir}' is not a directory!");

            if (!Directory.Exists(repoDir))
                Directory.CreateDirectory(repoDir);

            // This is more reliable than just interfacing with the
            // NuGet.Protocol API directly. And by that, I mean I broke
            // something and it caused a catastrophic issue with NuGet that I am
            // too scared to accidentally unleash onto other people, as it
            // transcends past this project into every other project...
            // Just brilliant.
            var addSource = new ProcessStartInfo("dotnet", $"nuget add source \"{repoDir + "\\" + Path.DirectorySeparatorChar}\" --name \"{nuget_repository}\"") {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            var addSourceProcess = Process.Start(addSource);
            addSourceProcess?.WaitForExit();
        }
        else if (!nugetRepo.IsEnabled) {
            Console.WriteLine($"NuGet repository '{nuget_repository}' exists but is not enabled. Enabling...");
            sourceProvider.EnablePackageSource(nuget_repository);
        }
        else {
            Console.WriteLine($"NuGet repository '{nuget_repository}' exists and is already enabled.");
        }
    }

    private static void PublishPackages(string srcDir, params string[] projectNames) {
        foreach (var projectName in projectNames) {
            var dir = new DirectoryInfo(Path.Combine(srcDir, projectName));

            foreach (var nupkg in dir.GetFiles("*.nupkg", SearchOption.AllDirectories)) {
                Console.WriteLine($"Publishing '{nupkg}'...");
                var publish = new ProcessStartInfo("dotnet", $"nuget push {nupkg} -s {nuget_repository}") {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                };
                var publishProcess = Process.Start(publish);
                publishProcess?.WaitForExit();
            }
        }
    }
}
