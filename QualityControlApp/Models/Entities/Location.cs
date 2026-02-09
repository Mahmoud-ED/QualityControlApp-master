namespace QualityControlApp.Models.Entities
{
    public class Location :BaseEntity
    {

      public string Name { get; set; }
      public string Latitude { get; set; }
      public string Longitude { get; set; }

        public List<CompanyQuestion>? CompanyQuestion { get; set; }

    }
}
