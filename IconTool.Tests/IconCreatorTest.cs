using System;
using System.IO;
using IconTool.IconModel;
using Xunit;

namespace IconTool.Tests
{
    public class IconCreatorTest
    {
        public IconCreatorTest()
        {
            ExpectedFullIcon = File.ReadAllBytes("FullIcon.ico");
            Expected3Icons = File.ReadAllBytes("3Icons.ico");
        }

        public byte[] Expected3Icons { get; set; }

        public byte[] ExpectedFullIcon { get; set; }

        [Fact]
        public void CreateIconFromDirectoryTest()
        {
            var icon = IconTool.IconCreator.CreateIconFromDirectory("Icons");

            using MemoryStream stream = new MemoryStream();
            icon.Write(stream);

            Assert.Equal(ExpectedFullIcon, stream.ToArray());
        }

        [Fact]
        public void CreateIconFromImagesTest()
        {
            var icon = IconCreator.CreateIconFromFiles(new[]
            {
                "Icons/x128.png",
                "Icons/x64.png",
                "Icons/x16.png",
            });

            using MemoryStream stream = new MemoryStream();
            icon.Write(stream);

            Assert.Equal(Expected3Icons, stream.ToArray());
        }
        
    }
}
