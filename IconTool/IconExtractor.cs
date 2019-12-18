using System.IO;
using IconTool.IconModel;

namespace IconTool
{
    public class IconExtractor
    {
        public static void Extract(string source, string output)
        {
            using var stream = File.OpenRead(source);

            var icon = Icon.Load(stream);

            Directory.CreateDirectory(output);

            foreach (var image in icon.Images)
            {
                var imageWidth = (image.Width == 0 ? 256 : image.Width);
                var filename = $"x{imageWidth}.png";
                var path = Path.Combine(output, filename);

                File.WriteAllBytes(path, image.ImageData);
            }
        }
    }
}