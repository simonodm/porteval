using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PortEval.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.IO;

namespace PortEval.Infrastructure.Configurations
{
    internal class ExchangeConfiguration : IEntityTypeConfiguration<Exchange>
    {
        public void Configure(EntityTypeBuilder<Exchange> builder)
        {
            builder
                .HasKey(e => e.Symbol);
            builder
                .Property(e => e.Symbol)
                .HasMaxLength(32);
            builder
                .Property(e => e.Name)
                .HasMaxLength(64);
            builder.HasData(SeedExchanges());
        }

        private List<Exchange> SeedExchanges()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentDirectory, "Static/exchanges.json");

            using var reader = new StreamReader(path);
            var json = reader.ReadToEnd();
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.Converters.Add(new ExchangeSeedJsonConverter());
            var result = JsonConvert.DeserializeObject<List<Exchange>>(json, serializerSettings);

            return result;
        }
    }

    internal class ExchangeSeedJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Write operation not supported.");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var data = JArray.Load(reader);
            var result = new List<Exchange>();
            foreach (var exchange in data)
            {
                var symbol = (string)exchange["symbol"];
                var name = (string)exchange["name"];

                var createdExchange = Exchange.Create(symbol, name);
                result.Add(createdExchange);
            }

            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<Exchange>);
        }

        public override bool CanWrite => false;
    }
}
