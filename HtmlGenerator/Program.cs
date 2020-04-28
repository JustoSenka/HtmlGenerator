using HtmlGenerator.Generator;
using HtmlGenerator.Tags;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace HtmlGenerator
{
    class Program
    {
        private const string k_ConfigFile = "config.json";

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

            if (string.IsNullOrEmpty(source))
                gen.SourceFolder = source;

            if (string.IsNullOrEmpty(destination))
                gen.DestinationFolder = destination;

            if (string.IsNullOrEmpty(libraries))
                gen.LibrariesFolder = libraries;

            if (clean && build)
                rebuild = true;

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
        }

        private static bool GetSectionFromLongOrShortVersion(IConfiguration root, string arg)
        {
            var value = string.IsNullOrEmpty(root.GetSection(arg).Value) ? root.GetSection(arg[0] + "").Value : root.GetSection(arg).Value;
            return value.Equals("True", StringComparison.InvariantCultureIgnoreCase);
        }

        private static IConfiguration BuildConfiguration(string[] args)
        {
            var builder = new ConfigurationBuilder();

            if (File.Exists(k_ConfigFile))
                builder.AddJsonFile(k_ConfigFile);

            builder.AddCommandLine(args);

            return builder.Build();
        }

        private static void PrintHelpMessage()
        {
            Console.WriteLine(k_HelpMessage);
        }

        private static readonly string k_HelpMessage = $@"
--{k_SourcePathArg}=<path> \\t\\t\\t Source path with HTML files which need to be built.
--{k_DestinationPathArg}=<path> \\t\\t\\t Destination path where all built HTML are placed
--{k_LibrariesPathArg}=<path> \\t\\t\\t Libraries folder path where libs, css, js files can be stored. Will be copied to destination path after build.

-c, -{k_CleanArg} \\t\\t\\t Clean destination directory
-r, -{k_RebuildArg} \\t\\t\\t Rebuild website and copy libraries
-b, -{k_BuildArg} \\t\\t\\t Build complete website, will not copy libraries if present
-h, -{k_HelpArg} \\t\\t\\t Show this help screen
";
    }
}
