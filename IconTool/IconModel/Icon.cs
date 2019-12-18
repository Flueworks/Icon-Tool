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

        public void Read(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);

            Reserved = reader.ReadInt16();
            Type = reader.ReadInt16();
            Count = reader.ReadInt16();

            Images = new List<Image>(Count);

            for (int j = 0; j < Count; j++)
            {
                Image img = new Image();
                img.Read(stream);
                Images.Add(img);
            }

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
