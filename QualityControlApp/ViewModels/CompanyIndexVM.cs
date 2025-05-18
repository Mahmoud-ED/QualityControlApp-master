using QualityControlApp.Models.Entities;

namespace QualityControlApp.ViewModels
{
    public class CompanyIndexVM
    {
        public List<Company> Companies { get; set; }
        public List<CompanyType> CompanyTypes { get; set; }
    }
}
