﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentValidation;
using PortEval.Application.Core.Common.BulkImport;
using Xunit;

namespace PortEval.Tests.Unit.CoreTests.BulkImportExport;

internal class TestRow
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime Created { get; set; }
    public decimal Value { get; set; }
}

internal class TestRowValidator : AbstractValidator<TestRow>
{
    public TestRowValidator()
    {
        RuleFor(t => t.Name)
            .NotEmpty();
    }
}

internal class TestImportProcessor : ImportProcessor<TestRow, TestRowValidator>
{
    public bool ImportFinished { get; private set; }
    public bool AsyncImportFinished { get; private set; }
    public int ItemsProcessed { get; private set; }

    public TestImportProcessor()
    {
        OnImportFinish = () => { ImportFinished = true; };
        OnImportFinishAsync = () =>
        {
            AsyncImportFinished = true;
            return Task.CompletedTask;
        };
    }

    protected override Task<ProcessedRowErrorLogEntry<TestRow>> ProcessItem(TestRow row)
    {
        ItemsProcessed++;
        return Task.FromResult(new ProcessedRowErrorLogEntry<TestRow>(row));
    }
}

internal class TestThrowingImportProcessor : ImportProcessor<TestRow, TestRowValidator>
{
    protected override Task<ProcessedRowErrorLogEntry<TestRow>> ProcessItem(TestRow row)
    {
        throw new InvalidOperationException();
    }
}

public class ImportProcessorTests
{
    private readonly IFixture _fixture;

    public ImportProcessorTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
    }

    [Fact]
    public async Task ProcessingImport_ProcessesEachItem_WhenItemsAreValid()
    {
        var data = _fixture.CreateMany<TestRow>(5);
        var sut = _fixture.Create<TestImportProcessor>();

        await sut.ImportRecordsAsync(data);

        Assert.Equal(5, sut.ItemsProcessed);
    }

    [Fact]
    public async Task ProcessingImport_CallsOnImportFinishCallback_WhenSuccessfullyFinished()
    {
        var data = _fixture.CreateMany<TestRow>(5);
        var sut = _fixture.Create<TestImportProcessor>();

        await sut.ImportRecordsAsync(data);

        Assert.True(sut.ImportFinished);
    }

    [Fact]
    public async Task ProcessingImport_CallsAsyncOnImportFinishCallback_WhenSuccessfullyFinished()
    {
        var data = _fixture.CreateMany<TestRow>(5);
        var sut = _fixture.Create<TestImportProcessor>();

        await sut.ImportRecordsAsync(data);

        Assert.True(sut.AsyncImportFinished);
    }

    [Fact]
    public async Task ProcessingImport_ReturnsErrorLogWithoutErrors_WhenItemsAreValid()
    {
        var data = _fixture.CreateMany<TestRow>(5);
        var sut = _fixture.Create<TestImportProcessor>();

        var result = await sut.ImportRecordsAsync(data);

        Assert.All(result.ErrorLog, entry => Assert.False(entry.IsError));
    }

    [Fact]
    public async Task ProcessingImport_ReturnsErrorLogWithErrors_WhenItemsFailToValidate()
    {
        var invalidRow = _fixture
            .Build<TestRow>()
            .With(r => r.Name, "")
            .Create();
        var sut = _fixture.Create<TestImportProcessor>();

        var result = await sut.ImportRecordsAsync(new List<TestRow> { invalidRow });

        Assert.True(result.ErrorLog.First().IsError);
    }

    [Fact]
    public async Task ProcessingImport_ReturnsErrorLogWithErrors_WhenItemProcessingThrows()
    {
        var data = _fixture.CreateMany<TestRow>(5);
        var sut = _fixture.Create<TestThrowingImportProcessor>();

        var result = await sut.ImportRecordsAsync(data);

        Assert.All(result.ErrorLog, entry => Assert.True(entry.IsError));
    }
}