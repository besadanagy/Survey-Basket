using Survey_Basket.Contracts.Polls;

namespace Survey_Basket.Mapping
{
    public class MappingConfigration : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Poll, PollResponse>()
                .Map(dest => dest.Information, src => $"{src.Title} : {src.Description}");
        }
    }
}
