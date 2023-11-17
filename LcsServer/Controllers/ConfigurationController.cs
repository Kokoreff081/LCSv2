using System;
using System.Linq;
using LcsServer.DatabaseLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LcsServer.Models.RequestModels;
using Microsoft.Extensions.Configuration;

namespace LcsServer.Controllers;

public class ConfigurationController : Controller
{
    private readonly IConfiguration Configuration;
    private readonly DesignTimeDbContextFactory _db;

    public ConfigurationController(IConfiguration config, DesignTimeDbContextFactory context)
    {
        Configuration = config;
        _db = context;
    }
    /// <summary>
    /// Get base config variables
    /// </summary>
    /// <returns></returns>
    [Authorize]
    public JsonResult Index()
    {
        var config = new FrontConfig();
        using (var db = _db.CreateDbContext(null))
        {
            config.IsRdmEnabled = Configuration.GetValue<bool>("RdmDiscoveryForbiddenGlobal");
            config.NewLogEntries = db.Events.Where(w => w.State == "Unread").ToList().Count;
            config.BaseUrl = Environment.GetEnvironmentVariable("VUE_APP_MAINURL");
        }

        return Json(config);
    }
}