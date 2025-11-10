using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CustomMiddleWare.MiddleWare.Attributes;
using CustomMiddleWare.Models;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Logs;

namespace CustomMiddleWare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RequireParentHeader]
    public class ParentController : ControllerBase
    {
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            return Ok("This is a parent-only endpoint!");
        }

        [HttpPost("profile")]
        public IActionResult GetProfile([FromBody] ProfileModel model)
        {
            return Ok(new SuccessResponseModel<ProfileModel> { Data = model});
        }
    }
}
