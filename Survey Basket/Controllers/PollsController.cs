
namespace Survey_Basket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollsController : ControllerBase
    {
        private readonly IPollService PollService;

        public PollsController(IPollService PollService)
        {
            this.PollService = PollService;
        }

        [HttpGet("")]
        public IActionResult GetAll()
        {
            return PollService.GetAllPolls() is null ? NotFound() :Ok(PollService.GetAllPolls());
        }
        [Route("{id}")]
        [HttpGet]
        public IActionResult GetById(int id)
        {
            return PollService.GetPollById(id)is null?NotFound(): Ok(PollService.GetPollById(id));
        }
        [HttpPost]
        public IActionResult Create(Poll poll)
        {
           var newpoll= PollService.CreatePoll(poll);
            return CreatedAtAction(nameof(GetById),new {id=newpoll.Id},newpoll);
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (PollService.GetPollById(id) is null)
                return NotFound();
            PollService.DeletePoll(id);
            return  Ok("Deleted Done");
        }
        [HttpPut]
        public IActionResult Modify(Poll Poll)
        {
            PollService.UpdatePoll(Poll);
            return Ok(PollService.GetPollById(Poll.Id));
        }


    }
}
