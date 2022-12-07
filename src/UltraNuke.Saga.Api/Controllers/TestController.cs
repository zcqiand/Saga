using Microsoft.AspNetCore.Mvc;
using System.Text;
using UltraNuke.Saga.Api.Services;

namespace UltraNuke.Saga.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class TestController : ControllerBase
{

    private readonly ProgramServices programServices;
    private readonly ILogger<TestController> logger;

    public TestController(ILogger<TestController> logger)
    {
        this.programServices = new ProgramServices(logger);
        this.logger = logger;
    }

    [HttpGet]
    public string GetExample()
    {
        programServices.Main();
        return String.Empty;
    }
}