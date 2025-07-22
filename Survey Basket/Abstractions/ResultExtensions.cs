namespace Survey_Basket.Abstractions
{
    public static class ResultExtensions
    {
        public static ObjectResult ToProblem(this Result result,int StatusCode)
        {
            if (result.IsSuccess)
                throw new InvalidOperationException("Can not convert seccuess result to problem");
            var problem = Results.Problem(statusCode: StatusCode);
            var ProblemDetails = problem.GetType().GetProperty("ProblemDetails")?.GetValue(problem) as ProblemDetails;

            ProblemDetails!.Extensions=new Dictionary<string, object?>
            {
                {
                    "error",new []{result.Error }
                }
            };
            return new ObjectResult(ProblemDetails);
        }
    }
}
