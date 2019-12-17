using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IconUtil
{
    class Icon
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

    class Image
    {

        public byte Height { get; set; }
        public byte Width { get; set; }
        public byte Colors { get; set; }
        public byte Reserved { get; private set; } = 0; // should be 0

        /// <summary>
        /// In ICO format: Specifies color planes. Should be 0 or 1.
        /// In CUR format: Specifies the horizontal coordinates of the hotspot in number of pixels from the left.
        /// </summary>
        public short Planes { get; set; }
        /// <summary>
        /// In ICO format: Specifies bits per pixel. If the images are png, this data is stored in the png data
        /// In CUR format: Specifies the vertical coordinates of the hotspot in number of pixels from the top.
        /// </summary>
        public short BitPerPixel { get; set; }
        public int Size { get; set; }
        public int Offset { get; set; }

        public byte[] ImageData { get; set; }

        public void Read(Stream s)
        {
            BinaryReader reader = new BinaryReader(s);
            Height = reader.ReadByte();
            Width = reader.ReadByte();
            Colors = reader.ReadByte();
            Reserved = reader.ReadByte();
            Planes = reader.ReadInt16();
            BitPerPixel = reader.ReadInt16();
            Size = reader.ReadInt32();
            Offset = reader.ReadInt32();

            var position = s.Position;

            ImageData = new byte[Size];
            s.Seek(Offset, SeekOrigin.Begin);
            int bytesread = s.Read(ImageData, 0, Size);
            s.Seek(position, SeekOrigin.Begin);
        }

        public void Write(Stream s)
        {
            BinaryWriter writer = new BinaryWriter(s);

            writer.Write(Height);
            writer.Write(Width);
            writer.Write(Colors);
            writer.Write(Reserved);
            writer.Write(Planes);
            writer.Write(BitPerPixel);
            writer.Write(Size);
            writer.Write(Offset);
        }
    }
}
