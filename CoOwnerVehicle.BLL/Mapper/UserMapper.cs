using AutoMapper;
using Co_owner_Vehicle.Models;
using CoOwnerVehicle.BLL.DTOs.User;

namespace CoOwnerVehicle.BLL.Mapper
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<User, UserDto>()
                .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.FirstName + " " + s.LastName));

            CreateMap<UserCreateDto, User>()
                .ForMember(d => d.PasswordHash, opt => opt.Ignore())
                .ForMember(d => d.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(d => d.IsVerified, opt => opt.MapFrom(_ => false))
                .ForMember(d => d.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UserUpdateDto, User>()
                .ForAllMembers(opt => opt.Condition((src, dest, val) => val != null));
        }
    }
}

