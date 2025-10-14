using Microsoft.AspNetCore.Mvc;
using DBHandler.Context;
using ApiHandler.Services;

namespace ApiHandler.Controllers;

[ApiController]
[Route("")]
public class IndexController : ControllerBase
{
    private readonly DBHandlerContext primaryContext;
    private readonly LoginDbContext loginContext;

    public IndexController(DBHandlerContext primaryContext, LoginDbContext loginContext)
    {
        this.primaryContext = primaryContext;
        this.loginContext = loginContext;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var results = await ConnectionStatusService.CheckAllAsync(primaryContext, loginContext);

        return Ok(new
        {
            connections = results
        });
    }
}
