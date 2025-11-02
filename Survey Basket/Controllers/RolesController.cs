
namespace Survey_Basket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController(IRoleService roleService) : ControllerBase
    {
        private readonly IRoleService roleService = roleService;
        [HttpGet("")]
        [HasPermission(Permissions.GetRoles)]
        public async Task<IActionResult> GetAllAsync([FromQuery] bool? includeDisabled , CancellationToken cancellationToken)
        {
           var roles=await roleService.GetAllAsync(includeDisabled, cancellationToken);
            return Ok(roles);
        }
        [HttpGet("{Id}")]
        [HasPermission(Permissions.GetRoles)]
        public async Task<IActionResult> GetByIdAsync(string Id)
        {
           var result=await roleService.GetAsync(Id);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpPost("")]
        [HasPermission(Permissions.addRoles)]
        public async Task<IActionResult> AddAsync(RoleRequest request)
        {
           var result=await roleService.AddAsync(request);
            return result.IsSuccess ? CreatedAtAction(nameof(GetByIdAsync), new{result.Value.Id },result.Value) : result.ToProblem();
        }
        [HttpPut("{id}")]
        [HasPermission(Permissions.updateRoles)]
        public async Task<IActionResult> UpdateAsync(string id,RoleRequest request)
        {
           var result=await roleService.UpadteAsync(id,request);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
        [HttpPut("Toggle-Status/{id}")]
        [HasPermission(Permissions.updateRoles)]
        public async Task<IActionResult> ToggleStatusAsync(string id)
        {
           var result=await roleService.ToggleStatusAsync(id);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
           
}
