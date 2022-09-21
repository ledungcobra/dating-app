﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  [Authorize]
  public class UsersController : BaseApiController
  {
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
      _userRepository = userRepository;
      _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
      var users = await _userRepository.GetMembersAsync();
      return Ok(users);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
      var user = await _userRepository.GetMemberAsync(username);
      var userToReturn = _mapper.Map<MemberDto>(user);
      return Ok(userToReturn);
    }

    [HttpPut]
    public async Task<ActionResult<MemberDto>> UpdateMember(MemberUpdateDto memberUpdateDto)
    {
      var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var user = await _userRepository.GetUserByUsernameAsync(username);
      _mapper.Map(memberUpdateDto, user);
      _userRepository.Update(user);
      if (await _userRepository.SaveAllAsync()) return NoContent();
      return BadRequest("Failed to update user");
    }
  }
}