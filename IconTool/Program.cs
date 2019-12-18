using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;

namespace IconTool
{
    class Program
    {
        static int Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                SetupCreateCommand(),
                SetupExtractCommand(),
            };

            rootCommand.Description = "Utility to create icons and extract images from icon files";

            return rootCommand.InvokeAsync(args).Result;
        }

        private static Command SetupCreateCommand()
        {
            var createCommand = new Command("create", "Creates an icon");

            var outputOption = new Option<FileInfo>("--output")
            {
                Argument = new Argument<FileInfo>(() => new FileInfo("icon.ico")),
                Required = false,
            };
            outputOption.AddAlias("-o");
            createCommand.AddOption(outputOption);


            var sourceOption = new Option<DirectoryInfo>("--source")
            {
                Argument = new Argument<DirectoryInfo>(),
                Required = false,
            };
            sourceOption.AddAlias("-s");
            createCommand.AddOption(sourceOption);


            var imagesOption = new Option<IEnumerable<FileInfo>>("--images")
            {
                Argument = new Argument<IEnumerable<FileInfo>>(),
                Required = false
            };
            imagesOption.AddAlias("-i");
            createCommand.AddOption(imagesOption);

            createCommand.Handler = CommandHandler.Create<FileInfo, DirectoryInfo, IEnumerable<FileInfo>>(Create);

            return createCommand;
        }

        private static Command SetupExtractCommand()
        {
            var extractCommand = new Command("extract", "Extracts the icons from an ico file");
            
            var sourceOption = new Option<FileInfo>("--source")
            {
                Argument = new Argument<FileInfo>(),
                Required = true,
            };
            sourceOption.AddAlias("-s");
            extractCommand.AddOption(sourceOption);

            var outputOption = new Option<DirectoryInfo>("--output")
            {
                Argument = new Argument<DirectoryInfo>(),
                Required = false,
            };
            outputOption.AddAlias("-o");
            extractCommand.AddOption(outputOption);

            extractCommand.Handler = CommandHandler.Create<FileInfo, DirectoryInfo>(Extract);
            return extractCommand;
        }

        private static void Create(FileInfo output, DirectoryInfo source, IEnumerable<FileInfo> images)
        {
            bool createFromFolder = images == null;

            using var stream = output.OpenWrite();
            if (createFromFolder)
            {
                if (source == null)
                {
                    source = new DirectoryInfo(".");
                }
                if (source.Exists)
                {
                    Console.WriteLine($"Creating icon at {output.Name} from folder {source.Name}");
                    var icon = IconCreator.CreateIconFromDirectory(source.FullName);
                    icon.Write(stream);
                }
                else
                {
                    throw new ArgumentException("Could not find part of the path");
                }
            }
            else
            {
                Console.WriteLine($"Creating icon at {output.Name} from {images.Count()} images");
                var icon = IconCreator.CreateIconFromFiles(images.Select(x => x.FullName));
                icon.Write(stream);
            }
        }

        private static void Extract(FileInfo source, DirectoryInfo output)
        {
            if (source == null)
            {
                new ArgumentNullException(nameof(source));
            }
            if (output == null)
            {
                var filename = Path.GetFileNameWithoutExtension(source.Name);
                output = new DirectoryInfo(filename);
            }

            Console.WriteLine($"Extracting icon {source.Name} to folder {output.Name}");

            IconExtractor.Extract(source.FullName, output.FullName);

        }
    }
}
