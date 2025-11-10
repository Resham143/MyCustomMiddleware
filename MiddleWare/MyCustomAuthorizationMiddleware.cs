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
        private (Type AttributeType, string RequiredType)[] roleRequirements =
           [
        (typeof(RequireTeacherHeaderAttribute), "teacher"),
        (typeof(RequireStudentHeaderAttribute), "student"),
        (typeof(RequireParentHeaderAttribute), "parent")
           ];

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
            bool result = true;
            Endpoint? endpoint = context.GetEndpoint();



            string userType = context.Request.Headers["usertype"].ToString().ToLowerInvariant();

            (Type AttributeType, string RequiredType) = roleRequirements.FirstOrDefault(x => x.RequiredType.Contains(userType));

            string route = string.Empty;


            if (AttributeType != null && userType == RequiredType)
            {
                result = await ValidateUserType(context, userType); // validate asynchronously
            }
            else
            {
                result = await ValidateUserType(context, "");
            }
            return result; // If no restrictions or userType matches
        }
    }
}
