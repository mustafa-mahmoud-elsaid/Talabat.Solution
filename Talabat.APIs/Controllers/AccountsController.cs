using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.Extensions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Controllers
{

    public class AccountsController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }
        [HttpPost("Register")]
        public async Task<ActionResult<UserToReturnDTO>> Register(RegisterToGetDTO model)
        {
            if (CheckEmailExists(model.Email).Result.Value)
                return BadRequest(new ApiValidationErrorResponse() { Errors = new string[] { "this email already in use" } });
            var user = new ApplicationUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                UserName = model.Email.Split("@")[0]
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return BadRequest(new ApiResponse(400));
            return Ok(new UserToReturnDTO()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _tokenService.CreateTokenAsync(user, _userManager)
            });
        }
        [HttpPost("Login")]
        public async Task<ActionResult<UserToReturnDTO>> Login(LoginToGetDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null) return Unauthorized(new ApiResponse(401));
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized(new ApiResponse(401));
            return Ok(new UserToReturnDTO()
            {
                DisplayName = user.DisplayName,
                Email = user.Email!,
                Token = await _tokenService.CreateTokenAsync(user, _userManager)
            });
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserToReturnDTO>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email!);
            return Ok(new UserToReturnDTO()
            {
                DisplayName = user!.DisplayName,
                Email = user.Email!,
                Token = await _tokenService.CreateTokenAsync(user, _userManager),
            });
        }
        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDTO>> GetUserAddress()
        {
            var user = await _userManager.FindUseWithAddressAsync(User);
            return Ok(_mapper.Map<UserAddress, AddressDTO>(user.Address));
        }
        [ProducesResponseType(typeof(AddressDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [Authorize, HttpPut("address")]
        public async Task<ActionResult<AddressDTO>> UpdateUserAddress(AddressDTO model)
        {
            var address = _mapper.Map<AddressDTO, UserAddress>(model);
            var user = await _userManager.FindUseWithAddressAsync(User);
            user.Address.FirstName= address.FirstName;
            user.Address.LastName= address.LastName;
            user.Address.City= address.City;
            user.Address.Street= address.Street;
            user.Address.Country= address.Country;
            user.Address.UserId =user.Id;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(new ApiResponse(400));
            return Ok(model);
        }
        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExists(string email)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }
    }
}
