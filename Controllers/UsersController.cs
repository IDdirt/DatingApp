﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingAppMvc.Data;
using DatingAppMvc.Dtos;
using DatingAppMvc.Helpers;
using DatingAppMvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingAppMvc.Controllers
{

    // This logs the user clicking on anything belop to update "last active"
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UsersController: ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _repo.GetUser(currentUserId);
            userParams.UserId = currentUserId;

            if (string.IsNullOrEmpty(userParams.Gender)) 
            {
                userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            }


            var users = await _repo.GetUsers(userParams);
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize,
                users.TotalCount, users.TotalPages);
            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailedDto>(user);
            return Ok(userToReturn);

        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
                
            var userFromRepo = await _repo.GetUser(id);

            _mapper.Map(userForUpdateDto, userFromRepo);
            if (await _repo.SaveAll())
                return NoContent();
            throw new Exception($"Updating user {id} failed on save");
        }

        [HttpPost("{id}/like/{recipientId}")]

        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var like = await _repo.GetLike(id, recipientId);

            if (like != null)
                return BadRequest("You already like this person");

            if (await _repo.GetUser(recipientId) == null)
                return NotFound("cannot find this person");
            
            like = new Like
            {
                LikerId = id,
                LikeeId = recipientId
            };

            // This is sync and does not store the info into the database yet.
            _repo.Add<Like>(like);
            // this is the async save
            if (await _repo.SaveAll())
                return Ok();
            
            return BadRequest("failed to save new like request");
        }
    }
}