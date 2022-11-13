using AutoMapper;

namespace Swiftly.User.Api.Data.Provider.EF
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserEntity>()
                .ForMember(m => m.PasswordHash, opt => opt.Condition(src => !string.IsNullOrEmpty(src.PasswordHash)));
            CreateMap<UserEntity, User>()
                .ForMember(m => m.PasswordHash, opt => opt.Ignore());
        }
    }
}
