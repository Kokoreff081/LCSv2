using System.ComponentModel;
using LightCAD.UI.Strings;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;

public enum ModbusTypes
{
    [Description(nameof(Resources.EffectModbusTypes_Fill))]
    ModbusFill,
    
    // [Description(nameof(Resources.EffectModbusTypes_Transition))]
    // ModbusTransition,
    
    ModbusStatic
}