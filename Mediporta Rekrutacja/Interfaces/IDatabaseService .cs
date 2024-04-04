public interface IDatabaseService
{
    public List<PercentageOfTags> GetPercentageOfTags(int page, int size);
    public List<StackOverflowTag> GetTags(int page, int size, TagColumn sort = TagColumn.id, SortingType direction = SortingType.asc);
    public Task FillTagTable(List<StackOverflowTag> tags, int startId);
}