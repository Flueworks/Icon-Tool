using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace IconUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            // Usecase 1: Run the tool in a folder to gather all the png files starting with x and combine them into an icon file

            var outputFile = args.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(outputFile))
            {
                outputFile = "icon.ico";
            }

            var currentDirectory = Directory.GetCurrentDirectory();
            var icon = CreateIconFromDirectory(currentDirectory);

            try
            {
                using var fileStream = File.OpenWrite(outputFile);
                icon.Write(fileStream);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
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
