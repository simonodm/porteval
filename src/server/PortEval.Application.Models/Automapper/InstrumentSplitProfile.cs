using AutoMapper;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Models.Automapper
{
    public class InstrumentSplitProfile : Profile
    {
        public InstrumentSplitProfile()
        {
            CreateMap<InstrumentSplit, InstrumentSplitDto>()
                .ForMember(s => s.Status,
                    opt => opt.MapFrom(s => s.ProcessingStatus));
            CreateMap<InstrumentSplitDto, InstrumentSplit>();
        }
    }
}
