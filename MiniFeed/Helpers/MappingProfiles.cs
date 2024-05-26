using AutoMapper;
using MiniFeed.DTO;
using MiniFeed.Models;

namespace MiniFeed.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<UserRegistrationRequest, User>();
            CreateMap<User, RetrievedUserResponse>();
            CreateMap<CreatePostRequest, Post>();
            CreateMap<Post, GetPostResponse>();
        }
    }
}
