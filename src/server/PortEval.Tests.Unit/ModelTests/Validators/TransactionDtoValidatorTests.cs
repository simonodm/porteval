﻿using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentValidation;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.Validators;
using Xunit;

namespace PortEval.Tests.Unit.ModelTests.Validators;

public class TransactionDtoValidatorTests
{
    public static IEnumerable<object[]> ValidTransactionData = new List<object[]>
    {
        new object[] { 1, 1m, 112m, DateTime.Parse("2022-10-04 02:24"), "" },
        new object[] { 4, -20m, 193.54m, DateTime.Parse("2021-06-22 12:59"), "Test Note" },
        new object[] { 8, 14.2m, 11.11m, DateTime.Parse("2016-02-29 17:01"), "Test Note 2" }
    };

    private readonly IFixture _fixture;

    public TransactionDtoValidatorTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
    }

    [Theory]
    [MemberData(nameof(ValidTransactionData))]
    public void Validate_ValidatesSuccessfully_WhenTransactionDataIsValid(int positionId, decimal amount,
        decimal price, DateTime time, string note)
    {
        var transaction = _fixture.Build<TransactionDto>()
            .With(t => t.PositionId, positionId)
            .With(t => t.Amount, amount)
            .With(t => t.Price, price)
            .With(t => t.Time, time)
            .With(t => t.Note, note)
            .Create();

        var sut = _fixture.Create<TransactionDtoValidator>();

        var validationResult = sut.Validate(transaction);

        Assert.True(validationResult.IsValid);
    }

    [Fact]
    public void Validate_FailsValidation_WhenPositionIdIsMissing()
    {
        var transaction = _fixture.Build<TransactionDto>()
            .With(t => t.PositionId, 0)
            .Create();

        var sut = _fixture.Create<TransactionDtoValidator>();

        var validationResult = sut.Validate(transaction);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(transaction.PositionId));
    }

    [Fact]
    public void Validate_FailsValidation_WhenPriceIsZero()
    {
        var transaction = _fixture.Build<TransactionDto>()
            .With(t => t.Price, 0)
            .Create();

        var sut = _fixture.Create<TransactionDtoValidator>();

        var validationResult = sut.Validate(transaction);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(transaction.Price));
    }

    [Fact]
    public void Validate_FailsValidation_WhenPriceIsNegative()
    {
        var transaction = _fixture.Build<TransactionDto>()
            .With(t => t.Price, -1)
            .Create();

        var sut = _fixture.Create<TransactionDtoValidator>();

        var validationResult = sut.Validate(transaction);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(transaction.Price));
    }

    [Fact]
    public void Validate_FailsValidation_WhenPriceIsTooLarge()
    {
        var transaction = _fixture.Build<TransactionDto>()
            .With(t => t.Price, decimal.MaxValue)
            .Create();

        var sut = _fixture.Create<TransactionDtoValidator>();

        var validationResult = sut.Validate(transaction);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(transaction.Price));
    }

    [Fact]
    public void Validate_FailsValidation_WhenAmountIsZero()
    {
        var transaction = _fixture.Build<TransactionDto>()
            .With(t => t.Amount, 0)
            .Create();

        var sut = _fixture.Create<TransactionDtoValidator>();

        var validationResult = sut.Validate(transaction);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(transaction.Amount));
    }

    [Fact]
    public void Validate_FailsValidation_WhenAmountIsTooLarge()
    {
        var transaction = _fixture.Build<TransactionDto>()
            .With(t => t.Amount, decimal.MaxValue)
            .Create();

        var sut = _fixture.Create<TransactionDtoValidator>();

        var validationResult = sut.Validate(transaction);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(transaction.Amount));
    }

    [Fact]
    public void Validate_FailsValidation_WhenNoteIsLongerThan255Characters()
    {
        var transaction = _fixture.Build<TransactionDto>()
            .With(t => t.Note, string.Join(string.Empty, _fixture.CreateMany<string>(50)))
            .Create();

        var sut = _fixture.Create<TransactionDtoValidator>();
        sut.ClassLevelCascadeMode = CascadeMode.Continue;

        var validationResult = sut.Validate(transaction);

        Assert.False(validationResult.IsValid);
        Assert.Contains(validationResult.Errors, e => e.PropertyName == nameof(transaction.Note));
    }
}