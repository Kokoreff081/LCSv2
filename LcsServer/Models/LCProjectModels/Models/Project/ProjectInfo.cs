using System.Globalization;
using LCSVersionControl;

namespace LcsServer.Models.LCProjectModels.Models.Project;

public class ProjectInfo
{
    public ProjectInfo()
    {
        Date = DateTime.Now;
        AssembledBy = new CompanyInfo();
        Customer = new CompanyInfo();
    }

    public string Title { get; set; }
    public DateTime Date { get; set; }
    public CompanyInfo AssembledBy { get; set; }
    public CompanyInfo Customer { get; set; }
    public string Description { get; set; }
    public string ObjectAddress { get; set; }

    public string ObjectCoordinates
    {
        get => _objectCoordinates;
        set
        {
            _objectCoordinates = value;
            if (_objectCoordinates == null)
            {
                return;
            }
            string[] aa = _objectCoordinates.Split(',');
                
            if (aa.Length != 2)
            {
                return;
            }

            const NumberStyles style = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;// | NumberStyles.AllowThousands;
            if (double.TryParse(aa[0].Trim(), style, CultureInfo.InvariantCulture, out double latitude))
            {
                _latitude = latitude.Clamped(-90,90);
            }
            if (double.TryParse(aa[1].Trim(), style, CultureInfo.InvariantCulture, out double longitude))
            {
                _longitude = longitude.Clamped(-180,180);
            }
        }
    }

    private string _objectCoordinates;
    private double _longitude = 30.284397420470217; // main INTILED office
    private double _latitude = 59.909681404823885;
        
    public (double latitude, double longitude) GetCoordinates()
    {
        return (_latitude, _longitude);
    }
}