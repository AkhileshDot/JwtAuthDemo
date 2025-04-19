using AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        //Entity to DTO
        CreateMap<User, UserDto>();

        //Custom mapping using a tuple as the source
        CreateMap<(User user, string accessToken, RefreshToken refreshToken), LoginResponseDto>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.user))
            .ForMember(dest => dest.AccessToken, opt => opt.MapFrom(src => src.accessToken))
            .ForMember(dest => dest.RefreshToken, opt => opt.MapFrom(src => src.refreshToken.Token));
    }

}
