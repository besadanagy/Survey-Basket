using Survey_Basket.Contracts.Common;

namespace Survey_Basket.Controllers
{
    [Route("api/polls/{pollId}/[controller]")]
    [ApiController]
    public class QuestionController(IQuestionService questionServices) : ControllerBase
    {
        private readonly IQuestionService questionServices = questionServices;
        [HttpGet("{id}")]
        [HasPermission(Permissions.GetQuestions)]
        public async Task<IActionResult> Get(int pollId, int id, CancellationToken cancellationToken)
        {
            var result = await questionServices.GetAsync(pollId, id, cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }
        [HttpGet]
        [HasPermission(Permissions.GetQuestions)]
        public async Task<IActionResult> GetAll(int pollId,[FromQuery] RequestFilters request, CancellationToken cancellationToken)
        {
            var result = await questionServices.GetAllAsync(pollId,request, cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }
        [HttpPost]
        [HasPermission(Permissions.addQuestions)]
        public async Task<IActionResult> Add(int pollId, [FromBody] QuestionRequest request, CancellationToken cancellationToken)
        {
            var result = await questionServices.AddAsync(pollId, request, cancellationToken);

            if (result.IsSuccess)
            {
                return CreatedAtAction(
                    nameof(Get),
                    new { pollId, id = result.Value.Id },   
                    result.Value                           
                );
            }

            return result.IsSuccess ? 
                RedirectToAction(nameof(GetAll),cancellationToken) 
                : result.ToProblem();

        }

        [HttpPut("{id}")]
        [HasPermission(Permissions.updateQuestions)]
        public async Task<IActionResult> Update(int pollId, int id, [FromBody] QuestionRequest request, CancellationToken cancellationToken)
        {
            var result = await questionServices.UpdateAsync(pollId, id, request, cancellationToken);

          
            return result.IsSuccess ? RedirectToAction(
                    nameof(Get),
                    new { pollId, id, cancellationToken }
                ) : result.ToProblem();
        }

        [HttpPut("{id}/toggle")]
        [HasPermission(Permissions.updateQuestions)]
        public async Task<IActionResult> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default)
        {
            var result = await questionServices.ToggleStatusAsync(pollId, id, cancellationToken);
            return result.IsSuccess
                ? Ok(result)
                : result.ToProblem();
        }
    }
}
