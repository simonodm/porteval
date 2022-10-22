using AutoMapper;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Models.Automapper
{
    public class PortfolioProfile : Profile
    {
        public PortfolioProfile()
        {
            CreateMap<Portfolio, PortfolioDto>();
        }
    }
}
