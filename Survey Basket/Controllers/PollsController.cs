
using Microsoft.AspNetCore.RateLimiting;

namespace Survey_Basket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollsController(IPollService pollService) : ControllerBase
    {
        private readonly IPollService _pollService = pollService;

        [HttpGet("")]
        [HasPermission(Permissions.GetPolls)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            return Ok(await _pollService.GetAllAsync(cancellationToken));
        }

        [HttpGet("{id}")]
        [HasPermission(Permissions.GetPolls)]
        public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _pollService.GetAsync(id, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpGet("current")]
        [Authorize(Roles = DefaultRoles.member)]
        [EnableRateLimiting("userlimit")]
        public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
        {
            return Ok(await _pollService.GetCurrentAsync(cancellationToken));
        }
        [HttpPost("")]
        [HasPermission(Permissions.addPolls)]
        public async Task<IActionResult> Add([FromBody] PollRequest request,
        CancellationToken cancellationToken)
        {
            var result = await _pollService.AddAsync(request, cancellationToken);

            return result.IsSuccess ? CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value) : result.ToProblem();
        }



        [HttpPut("{id}")]
        [HasPermission(Permissions.updatePolls)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request,
       CancellationToken cancellationToken)
        {
            var result = await _pollService.UpdateAsync(id, request, cancellationToken);

            return result.IsSuccess ? RedirectToAction(
                    nameof(Get),
                    new { id, cancellationToken }
                ) : result.ToProblem();
        }

        [HttpDelete("{id}")]
        [HasPermission(Permissions.deletePolls)]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _pollService.DeleteAsync(id, cancellationToken);

            return result.IsSuccess ? Ok("Was Deleted") : result.ToProblem();
        }

        [HttpPut("{id}/togglePublish")]
        [HasPermission(Permissions.updatePolls)]
        public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _pollService.TogglePublishStatusAsync(id, cancellationToken);

            return result.IsSuccess ? RedirectToAction(
                    nameof(Get),
                    new { id ,cancellationToken}
                ) : result.ToProblem();
        }
    }
}