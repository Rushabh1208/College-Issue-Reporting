namespace backend.Features.Students
{
    public class ImportResultDto
    {
        public int TotalRows { get; set; }
        public int ImportedRows { get; set; }
        public int SkippedRows { get; set; }
        public int DuplicateRows { get; set; }
        public List<ImportErrorDto> Errors { get; set; } = [];
    }
}
