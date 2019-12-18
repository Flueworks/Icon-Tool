using System.IO;

namespace IconTool.IconModel
{
    public class Image
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