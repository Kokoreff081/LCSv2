using System;
using System.Linq;
using LcsServer.DatabaseLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

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
            var logs = _db.Events.OrderByDescending(o=>o.dateTime).ToList();
            return new JsonResult(logs);
        }
    }
}