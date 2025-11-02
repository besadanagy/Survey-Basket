
namespace Survey_Basket.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = DefaultRoles.member)]
    public class AccountController(IUserService userService) : ControllerBase
    {
        private readonly IUserService userService = userService;

        [HttpGet("")]
        public async Task< IActionResult> Info()
        {
            var result = await userService.GetProfileAsync(User.GetUserId()!);
            return Ok(result.Value);
        }
        [HttpPut("Info")]
        public async Task< IActionResult> UpdateInfo(UpdateProfileRequest request)
        {
            var result = await userService.UpdateProfileAsync(User.GetUserId()!,request);
            return Ok(result);
        }
        [HttpPut("Change-Password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var result = await userService.ChangePasswordAsync(User.GetUserId()!, request);

            return result.IsSuccess ?
                Ok(result)
                : result.ToProblem();
        }
        
    }
}
