using System;
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
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }


        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var messageFromRep = await _repo.GetMessage(id);
            if (messageFromRep == null)
                return NotFound();
            return Ok(messageFromRep);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery] MessageParams messageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageParams.UserId = userId;
            var messagesFromRepo = await _repo.GetMessagesForUser(messageParams);
            var messages = _mapper.Map<IEnumerable<MessageToRetrunDto>>(messagesFromRepo);

            Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize,
                messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);

            return Ok(messages);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await _repo.GetMessageThread(userId, recipientId);
            var messageThread = _mapper.Map<IEnumerable<MessageToRetrunDto>>(messageFromRepo);

            return Ok(messageThread);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
            // doing this GetUser populates the sender var so that we can use its info in the return message
            var sender = await _repo.GetUser(userId);
            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            messageForCreationDto.SenderId = userId;
            var recipient = await _repo.GetUser(messageForCreationDto.RecipientId);

            if (recipient == null) 
            {
                return BadRequest("Cound not find recipient");
            }

            var message = _mapper.Map<Message>(messageForCreationDto);
            _repo.Add(message);

            if (await _repo.SaveAll())
            {
                var messageToReturn = _mapper.Map<MessageToRetrunDto>(message);   
                return CreatedAtRoute("GetMessage", 
                    new {userId, id = message.Id}, messageToReturn);
            }
            throw new Exception("Creating the message failed on save");

        }


        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await _repo.GetMessage(id);

            if (messageFromRepo.SenderId == userId)
            {
                messageFromRepo.SenderDeleted = true;
            }
            if (messageFromRepo.RecipientId == userId)
            {
                messageFromRepo.RecipientDeleted = true;
            }

            if (messageFromRepo.RecipientDeleted && messageFromRepo.SenderDeleted)
            {
                _repo.Delete(messageFromRepo);
            }
            if (await _repo.SaveAll())
                return NoContent();
            
            throw new Exception("Error Deleting Message becase Save failed");
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            var message = await _repo.GetMessage(id);
            if (message.RecipientId != userId)
                return Unauthorized();
            message.IsRead = true;
            message.DateRead = DateTime.Now;

            await _repo.SaveAll();
            return NoContent();
        }

    }
}