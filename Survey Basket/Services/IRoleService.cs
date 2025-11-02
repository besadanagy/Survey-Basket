
namespace Survey_Basket.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisabled = false, CancellationToken cancellationToken=default);
        Task<Result<RoleDetailsResponse>> GetAsync(string Id);
        Task<Result<RoleDetailsResponse>> AddAsync(RoleRequest request);
        Task<Result> UpadteAsync(string id, RoleRequest request);
        Task<Result> ToggleStatusAsync(string id);





    }
}
