using QualityControlApp.Models.Entities;

namespace QualityControlApp.ViewModels
{
    public class CompanyCreateVM
    {
        public Company? Company { get; set; }
        public List<CompanyType> CompanyTypes { get; set; }
    }
}
