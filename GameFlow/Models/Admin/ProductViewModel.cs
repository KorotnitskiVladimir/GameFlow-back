using GameFlow.Migrations;
using Category = GameFlow.Data.Category;

namespace GameFlow.Models.Admin;

public class ProductViewModel
{
    public ProductFormModel? FormModel { get; set; }

    public List<Category> Categories { get; set; } = new();
}