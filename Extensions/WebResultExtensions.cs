using Jolia.Core.Results;
using System.Web.Http;

namespace Jolia.AspNetCore.Extensions
{
    public static class WebResultExtensions
    {
        public static NegotiatedContentResult<WebResult> NegotiatedContentResult(this WebResult WebResult)
        {
            return new NegotiatedContentResult<WebResult>(WebResult.StatusCode, WebResult);
        }
    }
}
