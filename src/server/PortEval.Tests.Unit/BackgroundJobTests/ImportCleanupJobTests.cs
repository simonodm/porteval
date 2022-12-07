﻿using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using Moq;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.BackgroundJobs.DatabaseCleanup;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.BackgroundJobTests
{
    public class ImportCleanupJobTests
    {
        [Fact]
        public async Task Run_RemovesImportDatabaseEntriesOlderThan24h()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var importsToRemove = new List<DataImport>
            {
                new DataImport(Guid.NewGuid(), TemplateType.Portfolios, DateTime.UtcNow.AddHours(-24)),
                new DataImport(Guid.NewGuid(), TemplateType.Portfolios, DateTime.UtcNow.AddHours(-48)),
                new DataImport(Guid.NewGuid(), TemplateType.Portfolios, DateTime.UtcNow.AddHours(-72)),
            };
            var imports = new List<DataImport>()
            {
                new DataImport(Guid.NewGuid(), TemplateType.Portfolios, DateTime.UtcNow.AddHours(-5)),
                new DataImport(Guid.NewGuid(), TemplateType.Portfolios, DateTime.UtcNow.AddHours(-3)),
            };

            imports.AddRange(importsToRemove);

            var dataImportRepository = fixture.Freeze<Mock<IDataImportRepository>>();
            dataImportRepository
                .Setup(m => m.ListAllAsync())
                .ReturnsAsync(imports);

            var sut = fixture.Create<ImportCleanupJob>();

            await sut.Run();

            foreach(var import in importsToRemove)
            {
                dataImportRepository.Verify(m => m.DeleteAsync(import.Id), Times.Once());
            }
        }

        [Fact]
        public async Task Run_RemovesImportFilesOlderThan24h()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var storagePath = "/storage/";

            fixture.Customizations.Add(new TypeRelay(typeof(IFileSystem), typeof(MockFileSystem)));

            var importsToRemove = new List<DataImport>
            {
                new DataImport(Guid.NewGuid(), TemplateType.Portfolios, DateTime.UtcNow.AddHours(-24)),
                new DataImport(Guid.NewGuid(), TemplateType.Portfolios, DateTime.UtcNow.AddHours(-48)),
                new DataImport(Guid.NewGuid(), TemplateType.Portfolios, DateTime.UtcNow.AddHours(-72)),
            };
            var imports = new List<DataImport>()
            {
                new DataImport(Guid.NewGuid(), TemplateType.Portfolios, DateTime.UtcNow.AddHours(-5)),
                new DataImport(Guid.NewGuid(), TemplateType.Portfolios, DateTime.UtcNow.AddHours(-3)),
            };

            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddDirectory(storagePath);

            imports.AddRange(importsToRemove);
            foreach(var import in imports)
            {
                import.AddErrorLog(Path.Combine(storagePath, $"{import.Id}_log.csv"));
                fileSystem.AddFile(import.ErrorLogPath, import.Id.ToString());
            }

            var dataImportRepository = fixture.Freeze<Mock<IDataImportRepository>>();
            dataImportRepository
                .Setup(m => m.ListAllAsync())
                .ReturnsAsync(imports);

            var sut = fixture.Create<ImportCleanupJob>();

            await sut.Run();

            foreach(var import in importsToRemove)
            {
                Assert.False(fileSystem.FileExists(import.ErrorLogPath));
            }
        }
    }
}