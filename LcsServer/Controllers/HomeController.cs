﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LcsServer.Models;
using Microsoft.AspNetCore.Authorization;

namespace LcsServer.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
   
}