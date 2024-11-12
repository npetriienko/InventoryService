using AutoMapper;
using InventoryService.Models;
using InventoryService.Models.Responses;

namespace InventoryService.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ItemModel, ItemContractResponse>().ReverseMap();
    }
}