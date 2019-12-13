using System;
using System.Collections.Generic;
using System.Linq;
using DatingAppMvc.Models;
using Newtonsoft.Json;

namespace DatingAppMvc.Data
{
    public class Seed
    {
        public static void SeedUsers(DataContext context)
        {
            Console.WriteLine("Determining if I need to seed the database");
            if (!context.Users.Any())
            {   
                
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);
                foreach (var user in users)
                {
                    Console.WriteLine("Loading new seed database");
                    byte[] passwordHash, passwordSalt;
                    CreatePasswordHash("password", out passwordHash, out passwordSalt);

                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    user.Username = user.Username.ToLower();
                    context.Users.Add(user);
                }
                context.SaveChanges();
            }
        }
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //there are many ways to encrypt passords this is just one suggestion

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
