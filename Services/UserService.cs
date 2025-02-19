namespace be_magang.Services
{
    public interface IUserService
    {
        int GetUserId();
    }

    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            return userId != null ? int.Parse(userId) : throw new UnauthorizedAccessException("User ID tidak ditemukan!");
        }
    }

}
