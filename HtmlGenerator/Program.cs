using HtmlGenerator.Generator;
using HtmlGenerator.Tags;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HtmlGenerator
{
    class Program
    {
        private const string k_ConfigFile = "config.json";

        private const string k_EnvironmentPath = "currentDirectory";
        private const string k_SourcePathArg = "sourcePath";
        private const string k_DestinationPathArg = "destinationPath";
        private const string k_LibrariesPathArg = "librariesPath";
        private const string k_InteractiveArg = "interactive";
        private const string k_CleanArg = "clean";
        private const string k_RebuildArg = "rebuild";
        private const string k_BuildArg = "build";
        private const string k_HelpArg = "help";

        static void Main(string[] args)
        {
            Logger.LogMessage($"Page Generator started with args: {string.Join(", ", args)}");
            Logger.LogMessage($"Starting in: {Environment.CurrentDirectory}");

            var root = BuildConfiguration(args);

            var env = root.GetSection(k_EnvironmentPath).Value;
            if (!string.IsNullOrEmpty(env))
                Environment.CurrentDirectory = env;

            var source = root.GetSection(k_SourcePathArg).Value;
            var destination = root.GetSection(k_DestinationPathArg).Value;
            var libraries = root.GetSection(k_LibrariesPathArg).Value;

            var interactive = GetSectionFromLongOrShortVersion(root, k_InteractiveArg);
            var clean = GetSectionFromLongOrShortVersion(root, k_CleanArg);
            var rebuild = GetSectionFromLongOrShortVersion(root, k_RebuildArg);
            var build = GetSectionFromLongOrShortVersion(root, k_BuildArg);
            var help = GetSectionFromLongOrShortVersion(root, k_HelpArg);

            if (help)
            {
                PrintHelpMessage();
                return;
            }

            // Convert to absolute paths, since environment dir will be changed later
            var gen = new PageGenerator(new TagCollector())
            {
                SourceFolder = string.IsNullOrEmpty(source) ? "" : Path.GetFullPath(source),
                DestinationFolder = string.IsNullOrEmpty(destination) ? "" : Path.GetFullPath(destination),
                LibrariesFolder = string.IsNullOrEmpty(libraries) ? "" : Path.GetFullPath(libraries)
            };

            // Print current paths for easier debugging
            Console.WriteLine();
            Logger.LogMessage($"Source directory set to: {source} ('{gen.SourceFolder}')");
            Logger.LogMessage($"Destination directory set to: {destination} ('{gen.DestinationFolder}')");
            Logger.LogMessage($"Libraries directory set to: {libraries} ('{gen.LibrariesFolder}')");


            if (!Directory.Exists(gen.DestinationFolder))
            {
                Logger.LogError($"Destination directory not found: {gen.DestinationFolder}");
                gen.ReportErrors();
                return;
            }

            // Change environment current directory
            try
            {
                Environment.CurrentDirectory = gen.SourceFolder;
                Logger.LogMessage($"Current directory changed to: {Environment.CurrentDirectory}");
            }
            catch
            {
                Logger.LogError($"Source directory not found: {gen.SourceFolder}");
                gen.ReportErrors();
                return;
            }

            Console.WriteLine();
            RunGenerator(gen, interactive, clean, rebuild, build);

            gen.ReportErrors();
        }

        private static void RunGenerator(PageGenerator gen, bool interactive, bool clean, bool rebuild, bool build)
        {
            if (clean && build)
                rebuild = true;

            if (rebuild || build)
                gen.ScanFilesForHtml();

            if (interactive)
            {
                new Interactive(gen).Run(gen.SourceFolder);
                return;
            }
            else if (rebuild)
            {
                gen.CleanBuild();
                gen.BuildWebpage();
            }
            else if (build)
            {
                gen.BuildWebpage();
            }
            else if (clean)
            {
                gen.CleanBuild();
            }
            else
            {
                Console.WriteLine("\nMissing command line arguments or config:");
                PrintHelpMessage();
                return;
            }

            return;
        }

        private static bool GetSectionFromLongOrShortVersion(IConfiguration root, string arg)
        {
            var value = string.IsNullOrEmpty(root.GetSection(arg).Value) ? root.GetSection(arg[0] + "").Value : root.GetSection(arg).Value;
            return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase);
        }

        private static IConfiguration BuildConfiguration(string[] args)
        {
            var builder = new ConfigurationBuilder();

            var configFilePath = Path.GetFullPath(k_ConfigFile);
            if (File.Exists(configFilePath))
            {
                builder.AddJsonFile(configFilePath);
                Logger.LogMessage("Successfully read config from: " + configFilePath);
            }
            else
                Logger.LogMessage("No configuration file found: " + configFilePath);

            builder.AddCommandLine(args);

            var argsDictionary = new Dictionary<string, string>
            {
                { "-" + k_InteractiveArg[0], true + "" },
                { "-" + k_CleanArg[0], true + "" },
                { "-" + k_RebuildArg[0], true + "" },
                { "-" + k_BuildArg[0], true + ""},
                { "-" + k_HelpArg[0], true + ""},
                { "-" + k_InteractiveArg, true + ""},
                { "-" + k_CleanArg, true + ""},
                { "-" + k_RebuildArg, true + ""},
                { "-" + k_BuildArg, true + ""},
                { "-" + k_HelpArg, true + ""},
            }.Where(pair => args.Contains(pair.Key))
            .Select(pair => KeyValuePair.Create(pair.Key.Trim('-'), pair.Value));

            builder.AddInMemoryCollection(argsDictionary);
            return builder.Build();
        }


        private static void PrintHelpMessage()
        {
            Console.WriteLine(k_HelpMessage);
        }

        private const string k_Tabs1 = "\t";
        private const string k_Tabs2 = "\t\t";
        private const string k_Tabs3 = "\t\t\t";
        private static readonly string k_HelpMessage = $@"
Help screen with command line arguments for Html Generator:

--{k_EnvironmentPath}=<path> {k_Tabs2} Current directory or environment path. By default environment path will be set to be the same as Source Path.
--{k_SourcePathArg}=<path> {k_Tabs2} Source path with HTML files which need to be built.
--{k_DestinationPathArg}=<path> {k_Tabs1} Destination path where all built HTML are placed
--{k_LibrariesPathArg}=<path> {k_Tabs2} Libraries folder path where libs, css, js files can be stored. Will be copied to destination path after build.

-i, -{k_InteractiveArg} {k_Tabs2} Start interactive continious build. Builds website everytime a file in source directory is modified.
-c, -{k_CleanArg} {k_Tabs3} Clean destination directory
-r, -{k_RebuildArg} {k_Tabs3} Rebuild website and copy libraries
-b, -{k_BuildArg} {k_Tabs3} Build complete website, will not copy libraries if present
-h, -{k_HelpArg} {k_Tabs3} Show this help screen
";

    }
}
