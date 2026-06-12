namespace backend.Features.Categories
{
    public class CategoryResponseDto
    {
        public string ParentCategory { get; set; } = string.Empty;

        public List<CategoryItemDto> Categories { get; set; } = [];
    }
}
