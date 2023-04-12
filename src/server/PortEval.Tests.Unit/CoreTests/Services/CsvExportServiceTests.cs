using AutoFixture;
using AutoFixture.AutoMoq;
using PortEval.Application.Core.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.Services
{
    internal class CsvConversionTestType
    {
        public string FirstColumn { get; set; }
        public string SecondColumn { get; set; }
        public int ThirdColumn { get; set; }
        public decimal FourthColumn { get; set; }
        public DateTime FifthColumn { get; set; }
    }

    public class CsvExportServiceTests
    {
        private IFixture _fixture;

        public CsvExportServiceTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        [Fact]
        public void CsvConversion_GeneratesCorrectHeader()
        {
            var sut = _fixture.Create<CsvExportService>();

            var convertedBytes = sut.ConvertToCsv(Enumerable.Empty<CsvConversionTestType>());

            var bytesAsString = Encoding.Default.GetString(convertedBytes.Response);
            var lines = bytesAsString.Split("\r\n");
            var header = GenerateExpectedHeader();
            Assert.Equal(header, lines[0]);
        }

        [Fact]
        public void CsvConversion_GeneratesCorrectNumberOfRows()
        {
            var rows = _fixture.CreateMany<CsvConversionTestType>(5);
            var sut = _fixture.Create<CsvExportService>();

            var convertedBytes = sut.ConvertToCsv(rows);
            var bytesAsString = Encoding.Default.GetString(convertedBytes.Response);
            var lines = bytesAsString.Split("\r\n");

            Assert.Equal(7, lines.Length);
        }

        [Fact]
        public void CsvConversion_GeneratesCorrectNumberOfColumns()
        {
            var rows = _fixture.CreateMany<CsvConversionTestType>(1);
            var sut = _fixture.Create<CsvExportService>();

            var convertedBytes = sut.ConvertToCsv(rows);
            var bytesAsString = Encoding.Default.GetString(convertedBytes.Response);
            var lines = bytesAsString.Split("\r\n");

            Assert.Equal(5, lines[0].Split(",").Length);
        }

        [Fact]
        public void CsvConversion_SerializesCorrectStringValues()
        {
            var rows = _fixture.CreateMany<CsvConversionTestType>(1);
            var sut = _fixture.Create<CsvExportService>();

            var convertedBytes = sut.ConvertToCsv(rows);
            var bytesAsString = Encoding.Default.GetString(convertedBytes.Response);
            var lines = bytesAsString.Split("\r\n");

            Assert.Equal(rows.First().FirstColumn, lines[1].Split(",")[0]);
        }

        [Fact]
        public void CsvConversion_SerializesCorrectIntegerValues()
        {
            var rows = _fixture.CreateMany<CsvConversionTestType>(1);
            var sut = _fixture.Create<CsvExportService>();

            var convertedBytes = sut.ConvertToCsv(rows);
            var bytesAsString = Encoding.Default.GetString(convertedBytes.Response);
            var lines = bytesAsString.Split("\r\n");

            Assert.Equal(rows.First().ThirdColumn.ToString(), lines[1].Split(",")[2]);
        }

        [Fact]
        public void CsvConversion_SerializesCorrectDecimalValues()
        {
            var rows = _fixture.CreateMany<CsvConversionTestType>(1);
            var sut = _fixture.Create<CsvExportService>();

            var convertedBytes = sut.ConvertToCsv(rows);
            var bytesAsString = Encoding.Default.GetString(convertedBytes.Response);
            var lines = bytesAsString.Split("\r\n");

            Assert.Equal(rows.First().FourthColumn.ToString(CultureInfo.CurrentCulture), lines[1].Split(",")[3]);
        }

        [Fact]
        public void CsvConversion_SerializesInvariantCultureDateTimeValues()
        {
            var rows = _fixture.CreateMany<CsvConversionTestType>(1);
            var sut = _fixture.Create<CsvExportService>();

            var convertedBytes = sut.ConvertToCsv(rows);
            var bytesAsString = Encoding.Default.GetString(convertedBytes.Response);
            var lines = bytesAsString.Split("\r\n");

            Assert.Equal(
                rows.First().FifthColumn,
                DateTime.Parse(lines[1].Split(",")[4], CultureInfo.InvariantCulture),
                TimeSpan.FromSeconds(1));
        }

        private string GenerateExpectedHeader()
        {
            return string.Join(",", "FirstColumn", "SecondColumn", "ThirdColumn", "FourthColumn", "FifthColumn");
        }
    }
}