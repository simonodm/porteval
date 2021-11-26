using AutoMapper;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Models.Automapper
{
    public class InstrumentProfile : Profile
    {
        public InstrumentProfile()
        {
            CreateMap<Instrument, InstrumentDto>();
            CreateMap<InstrumentPrice, InstrumentPriceDto>();
        }
    }
}
