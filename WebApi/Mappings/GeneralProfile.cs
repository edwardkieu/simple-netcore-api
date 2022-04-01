using AutoMapper;
using WebApi.Data.Entities;
using WebApi.ViewModels.Account;
using WebApi.ViewModels.TestTable;

namespace WebApi.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
           CreateMap<AppUser, AppUserViewModel>().ReverseMap();
           //CreateMap<TestTable, TestTableViewModel>().ReverseMap();
        }
    }
}
