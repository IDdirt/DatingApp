﻿using System;
namespace DatingAppMvc.Dtos
{
    public class UserForListDto
    {
        public string Username { get; set; }
        public int Id { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhotoUrl { get; set; }
    }
}
