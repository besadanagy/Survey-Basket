namespace Survey_Basket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PollsController(IPollService pollService) : ControllerBase
    {
        private readonly IPollService _pollService = pollService;

        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _pollService.GetAll();
            return result.IsFailure
                ? result.ToProblem(StatusCodes.Status404NotFound)
                : Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _pollService.GetById(id);
            return result.IsFailure
                ? result.ToProblem(StatusCodes.Status404NotFound)
                : Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PollRequest poll)
        {
            var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value;

            if (userId is null)
                return Problem("User ID not found in token", statusCode: 401);

            var result = await _pollService.Create(poll, userId);
            return result.IsFailure
                ? result.ToProblem(StatusCodes.Status403Forbidden)
                : CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Modify(int id, [FromBody] PollRequest poll)
        {
            var result = await _pollService.Update(id, poll);
            return result.IsFailure
                ? result.ToProblem(StatusCodes.Status400BadRequest)
                : Ok("Updated Done");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _pollService.Delete(id);
            return result.IsFailure
                ? result.ToProblem(StatusCodes.Status404NotFound)
                : Ok("Deleted Done");
        }
    }
}
