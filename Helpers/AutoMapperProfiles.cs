using System;
using System.Linq;
using AutoMapper;
using DatingAppMvc.Dtos;
using DatingAppMvc.Models;

namespace DatingAppMvc.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            //convention, if the names are the same between the Dto and
            //the database then you dont need to map
            //for example Username and Id
            CreateMap<User, UserForListDto>()
                .ForMember(
                    dest => dest.PhotoUrl,
                    opt => opt.MapFrom(
                    src => src.Photos.FirstOrDefault(
                    prop => prop.IsMain).Url))
                .ForMember(
                    dest => dest.Age,
                    opt => opt.MapFrom(
                    src => src.DateOfBirth.CalculateAge())
            );

            CreateMap<User, UserForDetailedDto>()
                .ForMember(
                    dest => dest.PhotoUrl,
                    opt => opt.MapFrom(
                    src => src.Photos.FirstOrDefault(
                    prop => prop.IsMain).Url))
                 .ForMember(
                    dest => dest.Age,
                    opt => opt.MapFrom(
                    src => src.DateOfBirth.CalculateAge())
              );
            CreateMap<Photo, PhotosForDetailedDto>();
        }
    }
}
