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
        private const string k_CleanArg = "clean";
        private const string k_RebuildArg = "rebuild";
        private const string k_BuildArg = "build";
        private const string k_HelpArg = "help";

        static void Main(string[] args)
        {
            var root = BuildConfiguration(args);

            var env = root.GetSection(k_EnvironmentPath).Value;
            if (!string.IsNullOrEmpty(env))
                Environment.CurrentDirectory = env;

            var source = root.GetSection(k_SourcePathArg).Value;
            var destination = root.GetSection(k_DestinationPathArg).Value;
            var libraries = root.GetSection(k_LibrariesPathArg).Value;

            var clean = GetSectionFromLongOrShortVersion(root, k_CleanArg);
            var rebuild = GetSectionFromLongOrShortVersion(root, k_RebuildArg);
            var build = GetSectionFromLongOrShortVersion(root, k_BuildArg);
            var help = GetSectionFromLongOrShortVersion(root, k_HelpArg);

            if (help)
            {
                PrintHelpMessage();
                return;
            }

            var gen = new PageGenerator(new TagCollector());

            if (!string.IsNullOrEmpty(source))
            {
                gen.SourceFolder = source;
                Environment.CurrentDirectory = source;
            }

            if (!string.IsNullOrEmpty(destination))
                gen.DestinationFolder = destination;

            if (!string.IsNullOrEmpty(libraries))
                gen.LibrariesFolder = libraries;

            if (clean && build)
                rebuild = true;

            if (rebuild || build) 
                gen.ScanFilesForHtml();

            if (rebuild)
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
            }
        }

        private static bool GetSectionFromLongOrShortVersion(IConfiguration root, string arg)
        {
            var value = string.IsNullOrEmpty(root.GetSection(arg).Value) ? root.GetSection(arg[0] + "").Value : root.GetSection(arg).Value;
            return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase);
        }

        private static IConfiguration BuildConfiguration(string[] args)
        {
            var builder = new ConfigurationBuilder();

            if (File.Exists(k_ConfigFile))
                builder.AddJsonFile(k_ConfigFile);

            builder.AddCommandLine(args);

            var argsDictionary = new Dictionary<string, string>
            {
                { "-" + k_CleanArg[0], true + "" },
                { "-" + k_RebuildArg[0], true + "" },
                { "-" + k_BuildArg[0], true + ""},
                { "-" + k_HelpArg[0], true + ""},
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

--{k_EnvironmentPath}=<path> {k_Tabs2} Current directory or environment path. By default environment path will be set to be the same as destination.
--{k_SourcePathArg}=<path> {k_Tabs2} Source path with HTML files which need to be built.
--{k_DestinationPathArg}=<path> {k_Tabs1} Destination path where all built HTML are placed
--{k_LibrariesPathArg}=<path> {k_Tabs2} Libraries folder path where libs, css, js files can be stored. Will be copied to destination path after build.

-c, -{k_CleanArg} {k_Tabs3} Clean destination directory
-r, -{k_RebuildArg} {k_Tabs3} Rebuild website and copy libraries
-b, -{k_BuildArg} {k_Tabs3} Build complete website, will not copy libraries if present
-h, -{k_HelpArg} {k_Tabs3} Show this help screen
";

    }
}
