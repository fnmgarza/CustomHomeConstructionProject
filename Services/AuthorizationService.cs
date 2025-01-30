namespace CustomHomeConstructionProjects.Services
{
    using Duende.IdentityServer.Extensions;
    using Microsoft.AspNetCore.Http;

    public class AuthorizationService: IAuthorizationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            string? userId = user.GetSubjectId();

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User ID is missing.");
            }

            return userId;
        }
    }

}
