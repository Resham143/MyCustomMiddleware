using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomMiddleWare.MiddleWare.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace CustomMiddleWare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RequireStudentHeader]
    public class StudentController : ControllerBase
    {
      [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            return Ok("This is a student-only endpoint!");
        }
    }
}
