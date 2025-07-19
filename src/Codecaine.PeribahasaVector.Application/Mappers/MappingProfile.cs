using AutoMapper;
using Codecaine.PeribahasaVector.Application.ViewModels;
using Codecaine.PeribahasaVector.Domain.Entities;

namespace Codecaine.PeribahasaVector.Application.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add your mappings here
            // For example:
            // CreateMap<SourceEntity, DestinationEntity>();
            // CreateMap<CreatePeribahasaCommand, Peribahasa>();
             CreateMap<Peribahasa, PeribahasaViewModel>();
        }
    }
}
