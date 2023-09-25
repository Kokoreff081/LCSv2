using LcsServer.DatabaseLayer;
using Microsoft.AspNetCore.Mvc;

namespace LcsServer.Controllers;

public class LogsController : Controller
{
    private DesignTimeDbContextFactory _db;

    public LogsController(DesignTimeDbContextFactory db)
    {
        _db = db;
    }
    
    [HttpGet]
    public JsonResult Index()
    {
        using (var db = _db.CreateDbContext(null))
        {
            var logs = db.Events.ToList();
            return new JsonResult(logs);
        }

        
    }
}