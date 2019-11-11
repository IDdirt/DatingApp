using System;
using System.ComponentModel.DataAnnotations;

namespace DatingAppMvc.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage ="You must have a password between 4 and 8 charactors")]
        public string Password { get; set; }
    }
}
