using System.Collections.Generic;
using System.IO;

namespace IconTool.IconModel
{
    public class Icon
    {
        public short Reserved { get; private set; } = 0; // Must always be 0
        public short Type { get; set; } = 1; // 1 = .ico, 2 = .cur
        public short Count { get; set; }

        public List<Image> Images { get; set; } = new List<Image>();

        public static Icon Load(Stream stream)
        {
            Icon icon = new Icon();
            BinaryReader reader = new BinaryReader(stream);

            icon.Reserved = reader.ReadInt16();
            icon.Type = reader.ReadInt16();
            icon.Count = reader.ReadInt16();

            icon.Images = new List<Image>(icon.Count);

            for (int j = 0; j < icon.Count; j++)
            {
                Image img = new Image();
                img.Read(stream);
                icon.Images.Add(img);
            }

            return icon;
        }

        public void Write(Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);

            writer.Write(Reserved);
            writer.Write(Type);
            writer.Write(Count);

            var offset = 6 + (16 * Count);

            foreach (var image in Images)
            {
                image.Offset = offset;
                image.Write(stream);
                offset += image.Size;
            }

            foreach (var image in Images)
            {
                writer.Write(image.ImageData, 0, image.Size);
            }
        }
    }
}
