using System.ComponentModel.DataAnnotations;

namespace Web.Api.ViewModels;
public class ProductEditorViewModel
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "The product must contain a price")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "The product must contain a description")]
    public string Description { get; set; } = "";
}