using Microsoft.AspNetCore.Components;
using System.Net;

namespace EventsManagement.Blazor.Services
{
    public class AuthHttpHandler : DelegatingHandler
    {
        private readonly NavigationManager _navigationManager;

        public AuthHttpHandler(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // حذف token و هدایت به صفحه لاگین
                _navigationManager.NavigateTo("/login", forceLoad: true);
            }

            return response;
        }
    }
}
