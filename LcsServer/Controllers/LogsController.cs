using LcsServer.DatabaseLayer;
using Microsoft.AspNetCore.Mvc;

namespace LcsServer.Controllers;

public class LogsController : Controller
{
    private DatabaseContext _db;
    private IServiceProvider _serviceProvider;

    public LogsController(IServiceProvider serviceProvider)
    {
        //_db = db;
        _serviceProvider = serviceProvider;
    }
    
    [HttpGet]
    public JsonResult Index()
    {
        var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
        using (var scope = scopeFactory.CreateScope())
        {
            _db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var logs = _db.Events.ToList();
            return new JsonResult(logs);
        }
    }
}