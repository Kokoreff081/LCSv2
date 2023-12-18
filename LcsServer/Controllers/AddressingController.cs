using LcsServer.Models.LCProjectModels.Managers;
using LcsServer.Models.LCProjectModels.Models.Project;
using LcsServer.Models.ProjectModels;
using LightControlServiceV._2.DevicePollingService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LcsServer.Controllers;

public class AddressingController:Controller
{
    private ProjectChanger _pChanger;
    private readonly IConfiguration Configuration;
    private AddressingManager _addressingManager;
    private WebAddressingData _webModel;

    public AddressingController(IConfiguration _configuration, ProjectChanger pChanger,
        AddressingManager addressingManager)
    {
        _pChanger = pChanger;
        Configuration = _configuration;
        _addressingManager = addressingManager;

        _webModel = GetAddressingData();
    }

    private WebAddressingData GetAddressingData()
    {
        var result = new WebAddressingData();
        result.LCLamps = _pChanger.CurrentProject.LCLamps;
        result.ToTreeTable = MakeTreeTableData(_pChanger.CurrentProject.LCLamps);
        return result;
    }

    private List<AddressingTreeTableWeb> MakeTreeTableData(List<LCLampFront> list)
    {
        var result = new List<AddressingTreeTableWeb>();
        foreach (var item in list)
        {
            var ports = item.Children;
            var firstLevel = new AddressingTreeTableWeb()
            {
                key = "0",
                data = new ColumnsToAddressingTT()
                {
                    Name = item.Name,
                    LampAddress = item.LampAddress,
                    IpAddress = item.IpAddress,
                    ColorsCount = item.ColorsCount
                },
                children = new List<AddressingTreeTableWeb>()
            };
            for (int i = 0; i < ports.Count; i++)
            {
                var lampList = new List<AddressingTreeTableWeb>();
                var port = ports[i];
                int counter = 0;
                foreach (var lamp in port.Children)
                {
                    lampList.Add(new AddressingTreeTableWeb()
                    {
                        key = "0-" + i.ToString() + "-" + counter.ToString(),
                        data = new ColumnsToAddressingTT()
                        {
                            Name = lamp.Name,
                            LampAddress = lamp.LampAddress,
                            IpAddress = firstLevel.data.IpAddress,
                            ColorsCount = lamp.ColorsCount,
                        } 
                            
                    });
                    counter++;
                }
                firstLevel.children.Add(new AddressingTreeTableWeb()
                {
                    key = "0-" + i.ToString(),
                    data = new ColumnsToAddressingTT()
                    {
                        Name = port.Name,
                        ColorsCount = port.ColorsCount,
                        IpAddress = firstLevel.data.IpAddress,
                        LampAddress = port.LampAddress
                    },
                    children = lampList
                });
            }
            result.Add(firstLevel);
        }

        return result;
    }
    [HttpGet]
    [Authorize]
    [Route("/[controller]/[action]")]
    public string Index()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new IPEndPointConverter());
        settings.Converters.Add(new IPAddressConverter());
        //settings.Formatting = Formatting.Indented;
        var result = JsonConvert.SerializeObject(_webModel, settings);
        
        return result;
    }
}