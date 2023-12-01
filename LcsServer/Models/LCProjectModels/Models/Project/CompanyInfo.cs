namespace LcsServer.Models.LCProjectModels.Models.Project;

public class CompanyInfo
{
    public const string Delimiter = "---\\\\\\---";

    public CompanyInfo()
    {

    }

    public CompanyInfo(string formattedString)
    {
        if (!string.IsNullOrEmpty(formattedString))
        {
            string[] splittedString = formattedString.Split(new[] { Delimiter }, StringSplitOptions.None);
            if (splittedString.Length >= 5)
            {
                Name = splittedString[0];
                Company = splittedString[1];
                Address = splittedString[2];
                Phone = splittedString[3];
                Email = splittedString[4];
            }
        }
    }

    public string Name { get; set; }
    public string Company { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }

    public override string ToString()
    {
        return $"{Name}{Delimiter}{Company}{Delimiter}{Address}{Delimiter}{Phone}{Delimiter}{Email}";
    }
}