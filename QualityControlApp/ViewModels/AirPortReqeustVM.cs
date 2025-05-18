using QualityControlApp.Models.Entities;

namespace QualityControlApp.ViewModels
{
    public class AirPortReqeustVM
    {

        public AirPortRequest AirPortRequest { get; set; }
        public FileType FileType { get; set; }
        public List<FileUploadModel> FileTypes { get; set; } = new();
    }

    public class FileUploadModel
    {
        public Guid  FileTypeId { get; set; }
        public IFormFile File { get; set; }
    }  
}