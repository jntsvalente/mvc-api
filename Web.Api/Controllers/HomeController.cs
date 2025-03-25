﻿using Microsoft.AspNetCore.Mvc;

namespace Web.Api.Controllers;

[ApiController]
[Route("")]
public class HomeController : ControllerBase
{
    [HttpGet("")]
    public IActionResult Get(
        [FromServices] IConfiguration config)
    {
        var env = config.GetValue<string>("env");
        return Ok(new
        {
            environment = env
        });
    }
}