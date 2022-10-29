using System;
using System.Globalization;
using System.Linq;
using System.Text;
using AutoFixture;
using AutoFixture.AutoMoq;
using PortEval.Application.Services;
using Xunit;

namespace PortEval.Tests.UnitTests.Services
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
        [Fact]
        public void CsvConversion_GeneratesCorrectHeader()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            var sut = fixture.Create<CsvExportService>();

            var convertedBytes = sut.ConvertToCsv(Enumerable.Empty<CsvConversionTestType>());

            var bytesAsString = Encoding.Default.GetString(convertedBytes);
            var lines = bytesAsString.Split("\r\n");
            var header = GenerateExpectedHeader();
            Assert.Equal(header, lines[0]);
        }

        [Fact]
        public void CsvConversion_GeneratesCorrectNumberOfRows()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            var rows = fixture.CreateMany<CsvConversionTestType>(5);
            var sut = fixture.Create<CsvExportService>();

            var convertedBytes = sut.ConvertToCsv(rows);
            var bytesAsString = Encoding.Default.GetString(convertedBytes);
            var lines = bytesAsString.Split("\r\n");

            Assert.Equal(7, lines.Length);
        }

        [Fact]
        public void CsvConversion_GeneratesCorrectNumberOfColumns()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            var rows = fixture.CreateMany<CsvConversionTestType>(1);
            var sut = fixture.Create<CsvExportService>();

            var convertedBytes = sut.ConvertToCsv(rows);
            var bytesAsString = Encoding.Default.GetString(convertedBytes);
            var lines = bytesAsString.Split("\r\n");

            Assert.Equal(5, lines[0].Split(",").Length);
        }

        [Fact]
        public void CsvConversion_SerializesCorrectStringValues()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            var rows = fixture.CreateMany<CsvConversionTestType>(1);
            var sut = fixture.Create<CsvExportService>();

            var convertedBytes = sut.ConvertToCsv(rows);
            var bytesAsString = Encoding.Default.GetString(convertedBytes);
            var lines = bytesAsString.Split("\r\n");

            Assert.Equal(rows.First().FirstColumn, lines[1].Split(",")[0]);
        }

        [Fact]
        public void CsvConversion_SerializesCorrectIntegerValues()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            var rows = fixture.CreateMany<CsvConversionTestType>(1);
            var sut = fixture.Create<CsvExportService>();

            var convertedBytes = sut.ConvertToCsv(rows);
            var bytesAsString = Encoding.Default.GetString(convertedBytes);
            var lines = bytesAsString.Split("\r\n");

            Assert.Equal(rows.First().ThirdColumn.ToString(), lines[1].Split(",")[2]);
        }

        [Fact]
        public void CsvConversion_SerializesCorrectDecimalValues()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            var rows = fixture.CreateMany<CsvConversionTestType>(1);
            var sut = fixture.Create<CsvExportService>();

            var convertedBytes = sut.ConvertToCsv(rows);
            var bytesAsString = Encoding.Default.GetString(convertedBytes);
            var lines = bytesAsString.Split("\r\n");

            Assert.Equal(rows.First().FourthColumn.ToString(CultureInfo.CurrentCulture), lines[1].Split(",")[3]);
        }

        [Fact]
        public void CsvConversion_SerializesInvariantCultureDateTimeValues()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var rows = fixture.CreateMany<CsvConversionTestType>(1);
            var sut = fixture.Create<CsvExportService>();

            var convertedBytes = sut.ConvertToCsv(rows);
            var bytesAsString = Encoding.Default.GetString(convertedBytes);
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