using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Icon = IconTool.IconModel.Icon;
using Image = IconTool.IconModel.Image;

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

        private static Command SetupExtractCommand()
        {
            var extractCommand = new Command("extract", "Extracts the icons from an ico file");
            extractCommand.Handler = CommandHandler.Create(() => { Console.WriteLine("Extract - Not implemented yet"); });
            return extractCommand;
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
                    var icon = CreateIconFromDirectory(source.FullName);
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
                var icon = GenerateIconFromFiles(images.Select(x => x.FullName));
                icon.Write(stream);
            }
        }

        /// <summary>
        /// Creates an icon from all .png files matching the regex x\d+\.png in the specified directory.
        /// </summary>
        /// <param name="directory">Directory containing png files</param>
        /// <param name="outputFile">The output path for the icon</param>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        private static Icon CreateIconFromDirectory(string directory)
        {
            var files = Directory.GetFiles(directory, "x*.png");

            var fileRegex = new Regex("x\\d+\\.png");

            List<string> acceptedFiles = new List<string>();

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                if (fileName == null)
                    continue;

                try
                {
                    if (fileRegex.IsMatch(fileName))
                    {
                        acceptedFiles.Add(file);
                    }
                }
                catch (RegexMatchTimeoutException)
                {
                    continue;
                }
            }

            if (acceptedFiles.Count == 0)
            {
                throw new Exception("Could not find any suitable images");
            }

            var icon = GenerateIconFromFiles(acceptedFiles);

            return icon;
        }

        private static Icon GenerateIconFromFiles(IEnumerable<string> files)
        {
            Icon icon = new Icon { Type = 1 };

            foreach (var file in files)
            {
                if (!File.Exists(file))
                {
                    continue;
                }

                try
                {
                    var image = CreateImageFromFile(file);
                    icon.Count++;
                    icon.Images.Add(image);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                    // skip file
                }
            }

            return icon;
        }

        /// <summary>
        /// CreateImageFromFile
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        private static Image CreateImageFromFile(string file)
        {
            Image image = new Image
            {
                Colors = 0,
                Planes = 1
            };

            var stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite);
            image.Size = (int)stream.Length;
            image.ImageData = new byte[image.Size];
            stream.Read(image.ImageData, 0, image.Size);
            stream.Close();

            Bitmap bitmap = new Bitmap(file);
            var width = bitmap.Size.Width;
            var height = bitmap.Size.Height;
            bitmap.Dispose();

            // 0 means image width is 256 pixels
            if (width == 256)
                width = 0;
            if (height == 256)
                height = 0;

            // todo: Add checks for width x height

            image.Height = (byte)height;
            image.Width = (byte)width;
            return image;
        }
    }
}
