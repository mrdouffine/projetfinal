namespace GestionConge.Client.Handlers;

using GestionConge.Client.Services;
using System.Net.Http.Headers;


    public class CustomAuthorizationHandler : DelegatingHandler
    {
        private readonly AuthService _authService;

        public CustomAuthorizationHandler(AuthService authService)
        {
            _authService = authService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Get the token from your custom AuthService
            var token = await _authService.GetJwtTokenAsync();

            if (!string.IsNullOrEmpty(token))
            {
                // Attach the JWT to the Authorization header
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Continue the request pipeline
            return await base.SendAsync(request, cancellationToken);
        }
    }
