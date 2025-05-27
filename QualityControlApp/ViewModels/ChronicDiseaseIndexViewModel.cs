// Create a new folder e.g., QualityControlApp/ViewModels (if it doesn't exist)
// QualityControlApp.ViewModels.ChronicDiseaseIndexViewModel.cs

using QualityControlApp.Models.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // For potential ViewModel-specific annotations

namespace QualityControlApp.ViewModels
{
    public class ChronicDiseaseIndexViewModel
    {
        public IEnumerable<ChronicDisease> ChronicDiseases { get; set; }

        // This will be used for the "Add New" form
        // We can re-use the ChronicDisease model directly or create a specific CreateChronicDiseaseInputModel
        // For simplicity, we'll re-use ChronicDisease here.
        public ChronicDisease NewChronicDisease { get; set; }

        public ChronicDiseaseIndexViewModel()
        {
            ChronicDiseases = new List<ChronicDisease>();
            NewChronicDisease = new ChronicDisease(); // Initialize for the form
        }
    }
}