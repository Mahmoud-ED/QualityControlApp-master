namespace QualityControlApp.Models.Entities
{
    public class Location :BaseEntity
    {

      public string Name { get; set; }

        public List<CompanyQuestion>? CompanyQuestion { get; set; }

    }
}
