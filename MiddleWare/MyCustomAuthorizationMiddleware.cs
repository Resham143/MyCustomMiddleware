using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CustomMiddleWare.MiddleWare.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomMiddleWare.MiddleWare
{
    public class MyCustomAuthorizationMiddleware(ILogger<MyCustomAuthorizationMiddleware> logger) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate requestDelegate)
        {

            if (!await ValidateUserAuthentication(context))
                return;

            await requestDelegate(context);
        }

        private async Task<bool> ValidateUserType(HttpContext context, string userType)
        {
            bool status = false;
            switch (userType)
            {
                case "student" or "teacher" or "parent":
                    status = true;
                    break;
                default:
                    logger.LogInformation("user type is not provided.");


                    await context.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Title = "Mandatory headers are missing",
                        Status = StatusCodes.Status203NonAuthoritative
                    });
                    break;

            }
            return status;
        }

        private async Task<bool> ValidateUserAuthentication(HttpContext context)
        {
            bool result = false;

            var endpoint = context.GetEndpoint();

            var userType = context.Request.Headers["usertype"];
            var requiresTeacher = endpoint?.Metadata.GetMetadata<RequireTeacherHeaderAttribute>() != null;

            var requiresStudent = endpoint?.Metadata.GetMetadata<RequireStudentHeaderAttribute>() != null;

            var requiresParent = endpoint?.Metadata.GetMetadata<RequireParentHeaderAttribute>() != null;

            if (requiresTeacher && userType.ToString().ToLowerInvariant() != "teacher")
            {
                result = await ValidateUserType(context, "");
            }

            else if (requiresStudent && userType.ToString().ToLowerInvariant() != "student")
            {
                result = await ValidateUserType(context, "");
            }

            else if (requiresParent && userType.ToString().ToLowerInvariant() != "parent")
            {
                result = await ValidateUserType(context, "");
            }

            else

                result = true;


            return result;

        }
    }
}
