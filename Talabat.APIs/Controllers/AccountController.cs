using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.APIs.Extentions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{

    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _usermanger;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> user, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
        {
            _usermanger = user;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("Register")]

        public async Task<ActionResult<UserDto>> Register(RegisterDto Model)
        {

            if (CheckEmailExists(Model.Email).Result.Value)
            {
                return BadRequest(new ApiResponse(400, "Email Is Already In Use"));
            }
            var User = new AppUser()
            {
                DisplayName = Model.DisplayName,
                Email = Model.Email,
                UserName = Model.Email.Split('@')[0],
                PhoneNumber = Model.PhoneNumber,

            };

            var Result = await _usermanger.CreateAsync(User, Model.Password);

            if (!Result.Succeeded) return BadRequest(new ApiResponse(400));

            var ReturnedUser = new UserDto()
            {
                DisplayName = User.DisplayName,
                Email = User.Email,
                Token = await _tokenService.GetTokenAsync(User, _usermanger)

            };

            return Ok(ReturnedUser);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var User = await _usermanger.FindByEmailAsync(model.Email);

            if (User is null) return Unauthorized(new ApiResponse(401));

            var Result = await _signInManager.CheckPasswordSignInAsync(User, model.Password, false);

            if (!Result.Succeeded) return Unauthorized(new ApiResponse(401));
            var ReturnedUser = new UserDto()
            {
                DisplayName = User.DisplayName,
                Email = User.Email,
                Token = await _tokenService.GetTokenAsync(User, _usermanger)
            };
            return Ok(ReturnedUser);
        }

        [Authorize]
        [HttpGet]

        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var user = await _usermanger.FindByEmailAsync(email);

            var ReturnedUser = new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _tokenService.GetTokenAsync(user, _usermanger)
            };

            return Ok(ReturnedUser);
        }


        [Authorize]
        [HttpGet("address")]

        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var user = await _usermanger.FindUserWithAdressAsync(User);

            return Ok(_mapper.Map<AddressDto>(user.address));
        }
        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<Address>> UpdateUserAddress(AddressDto address)
        {

             var UpdatedAddress = _mapper.Map<Address>(address);

            var user = await _usermanger.FindUserWithAdressAsync(User);

            UpdatedAddress.Id = user.address.Id;

            user.address = UpdatedAddress;
            
            var result =  await _usermanger.UpdateAsync(user);

            if (!result.Succeeded) return BadRequest(new ApiValidationErrorResponse() { Errors = result.Errors.Select(e => e.Description) });
            return Ok(address);  
        }



        [HttpGet("EmailExists")]
        public async Task<ActionResult<bool>> CheckEmailExists(string email)
        {
            return await _usermanger.FindByEmailAsync(email) is not null;
        }


    }
}
