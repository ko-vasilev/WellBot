using AutoMapper;
using WellBot.Domain.Users.Entities;
using WellBot.UseCases.Users.Common.Dtos;

namespace WellBot.UseCases.Users
{
    /// <summary>
    /// User mapping profile.
    /// </summary>
    public class UserMappingProfile : Profile
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public UserMappingProfile()
        {
            CreateMap<User, UserDto>();
        }
    }
}
