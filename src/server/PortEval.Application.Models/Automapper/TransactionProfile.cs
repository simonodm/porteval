using AutoMapper;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Models.Automapper
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<Transaction, TransactionDto>()
                .ForMember(dest => dest.PortfolioId,
                    opt => opt.MapFrom(src => src.Position.PortfolioId));
        }
    }
}
