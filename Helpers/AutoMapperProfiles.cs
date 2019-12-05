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
            //for example the Username and Id fields
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
            CreateMap<UserForUpdateDto, User>();
            CreateMap<Photo, PhotoForRetrunDto>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<UserForRegisterDto, User>();
            CreateMap<MessageForCreationDto, Message>().ReverseMap();
            CreateMap<Message, MessageToRetrunDto>()
            .ForMember(m => m.SenderPhotoUrl, opt => opt.MapFrom( u => u.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
            .ForMember(m => m.RecipientPhotoUrl, opt => opt.MapFrom( u => u.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));
        }
    }
}
