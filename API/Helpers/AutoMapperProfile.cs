using AutoMapper;
using Business.Models;
using Data.Entities;
using System.Linq;

namespace API.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, MemberDTO>()
                .ForMember(dest => dest.PhotoUrl, opt =>
                opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));

            CreateMap<Photo, PhotoDTO>().ReverseMap();
            CreateMap<UpdateMemberDto, ApplicationUser>();
            CreateMap<UserRegisterDTO, ApplicationUser>();

            CreateMap<Message, MessageDto>()
                .ForMember(dest=> dest.SenderPhotoUrl,opt=> 
                opt.MapFrom(src=> src.Sender.Photos.FirstOrDefault(x=> x.IsMain).Url))

                .ForMember(dest=> dest.RecipeintPhotoUrl, opt => 
                opt.MapFrom(src=> src.Recipeint.Photos.FirstOrDefault(x=> x.IsMain).Url));
        }
    }
}
