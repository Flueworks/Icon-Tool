using System.IO;
using Xunit;

namespace IconTool.Tests
{
    public class IconExtractorTest
    {
        [Fact]
        public void TestExtraction()
        {
            var testextractionfolder = "TestExtractionFolder";

            Directory.Delete(testextractionfolder, true);

            IconExtractor.Extract("FullIcon.ico", testextractionfolder);

            var directoryInfo = new DirectoryInfo(testextractionfolder);

            Assert.True(directoryInfo.Exists);

            var files = directoryInfo.GetFiles();
            Assert.Equal(files.Length, 7);

            foreach (var file in files)
            {
                // Verify that no data has been lost
                var correctFile = Path.Combine("Icons", file.Name);
                var correctBytes = File.ReadAllBytes(correctFile);
                var testBytes = File.ReadAllBytes(file.FullName);

                Assert.Equal(correctBytes, testBytes);
            }
        }
    }
}