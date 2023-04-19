using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PortEval.Domain.Models.Entities;

namespace PortEval.Infrastructure.Configurations;

internal class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder
            .HasKey(c => c.Code);
        builder
            .Property(c => c.Code)
            .HasMaxLength(3);
        builder
            .Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(64);
        builder
            .Property(c => c.Symbol)
            .IsRequired()
            .HasMaxLength(4);
        builder
            .OwnsOne(c => c.TrackingInfo);
        builder
            .HasData(SeedCurrencies());
        builder
            .Property(c => c.Version)
            .IsConcurrencyToken();
    }

    private List<Currency> SeedCurrencies()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var path = Path.Combine(currentDirectory, "Static/currencies.json");

        using var reader = new StreamReader(path);
        var json = reader.ReadToEnd();
        var serializerSettings = new JsonSerializerSettings();
        serializerSettings.Converters.Add(new CurrencySeedJsonConverter());
        var result = JsonConvert.DeserializeObject<List<Currency>>(json, serializerSettings);

        return result;
    }
}

internal class CurrencySeedJsonConverter : JsonConverter
{
    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException("Write operation not supported.");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var data = JArray.Load(reader);
        var result = new List<Currency>();
        foreach (var currency in data)
        {
            var code = (string)currency["code"];
            var name = (string)currency["name"];
            var symbol = (string)currency["symbol"];

            var isDefault = code == "USD";

            var createdCurrency = Currency.Create(code, name, symbol, isDefault);

            result.Add(createdCurrency);
        }

        return result;
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(List<Currency>);
    }
}