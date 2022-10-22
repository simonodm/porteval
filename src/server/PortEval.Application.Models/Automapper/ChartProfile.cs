using AutoMapper;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.DTOs.Enums;
using PortEval.Domain.Models.Entities;
using System;

namespace PortEval.Application.Models.Automapper
{
    public class ChartProfile : Profile
    {
        public ChartProfile()
        {
            CreateMap<Chart, ChartDto>()
                .ForMember(dest => dest.IsToDate,
                    opt => opt.MapFrom(src => src.DateRange.IsToDate))
                .ForMember(dest => dest.ToDateRange,
                    opt => opt.MapFrom(src => src.DateRange.ToDateRange))
                .ForMember(dest => dest.Type,
                    opt => opt.MapFrom(src => src.TypeConfiguration.Type))
                .ForMember(dest => dest.CurrencyCode,
                    opt => opt.MapFrom(src => src.TypeConfiguration.CurrencyCode))
                .ForMember(dest => dest.Frequency,
                    opt => opt.MapFrom(src => src.TypeConfiguration.Frequency));

            CreateMap<ChartLine, ChartLineDto>()
                .IncludeAllDerived()
                .ForMember(dest => dest.Type,
                    opt => opt.MapFrom(src => GetChartLineType(src)));
            CreateMap<ChartLinePortfolio, ChartLineDto>();
            CreateMap<ChartLinePosition, ChartLineDto>();
            CreateMap<ChartLineInstrument, ChartLineDto>();
        }

        private ChartLineType GetChartLineType(ChartLine line)
        {
            return line switch
            {
                ChartLinePortfolio _ => ChartLineType.Portfolio,
                ChartLinePosition _ => ChartLineType.Position,
                ChartLineInstrument _ => ChartLineType.Instrument,
                _ => throw new ArgumentException("Unrecognized chart line type provided.")
            };
        }
    }
}
