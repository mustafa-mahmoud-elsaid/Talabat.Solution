using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Text;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Attributes
{
    public class RedisCacheAttribute(int durationInSecondes = 60) : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var service = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
            var cacheKey = GenerateCacheKey(context.HttpContext.Request);
            var value = await service.GetAsync(cacheKey);
            if (value is not null)
            {
                context.Result = new ContentResult()
                {
                    Content = value,
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                };
                return;
            }
            var result = await next.Invoke();
            if(result.Result is OkObjectResult okObjectResult)
            {
                await service.SetAsync(cacheKey, okObjectResult, TimeSpan.FromSeconds(durationInSecondes));
            }
        }

        private string GenerateCacheKey(HttpRequest request)
        {
            var key = new StringBuilder();
            key.Append(request.Path);
            foreach (var item in request.Query.OrderBy(Q => Q.Key))
                key.Append($"{item.Key}-{item.Value}|");
            return key.ToString();
        }
    }
}
