namespace LcsServer.Models.LCProjectModels.GlobalBase.Settings;

public enum SettingsType: byte
{
    Application,
    Engineering,
    LightSettings,
    LightSettingsViewerOnly,
    //RenderingSettings_Application,
    RenderingSettings_Project,
    Project,
    Converter,
    ViewPortSettings,
    RenderTool,
    ReportSettings,
    GameModeSettings,
    VideoMaker,
    Hardware,
    Scheduler,
    //RemoteButtons,
    Modbus,
    REST,
}