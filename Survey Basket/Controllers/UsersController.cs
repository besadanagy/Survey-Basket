
namespace Survey_Basket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService userService = userService;
        [HttpGet("")]
        [HasPermission(Permissions.GetUsers)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            return Ok(await userService.GetAllAsync(cancellationToken));
        }
        [HttpGet("{id}")]
        [HasPermission(Permissions.GetUsers)]
        public async Task<IActionResult> Get(string id)
        {
            var result = await userService.GetAsync(id);
            return result.IsSuccess ? Ok(result.Value):result.ToProblem();
        }
        [HttpPost("")]
        [HasPermission(Permissions.addUsers)]
        public async Task<IActionResult> Add(CreateUserRequest request,CancellationToken cancellationToken)
        {
            var result = await userService.AddAsync(request,cancellationToken);
            return result.IsSuccess ? CreatedAtAction(nameof(Get), new {result.Value.Id},result.Value) : result.ToProblem();
        }
        [HttpPut("{id}")]
        [HasPermission(Permissions.updateUsers)]
        public async Task<IActionResult> Update(string id,UpdateUserRequest request,CancellationToken cancellationToken)
        {
            var result = await userService.UpdateAsync(id,request,cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [HttpPut("{id}/Toggle-Status")]
        [HasPermission(Permissions.updateUsers)]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var result = await userService.ToggleStatusAsync(id);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [HttpPut("{id}/UnLock")]
        [HasPermission(Permissions.updateUsers)]
        public async Task<IActionResult> UnLock(string id)
        {
            var result = await userService.UnLockAsync(id);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
