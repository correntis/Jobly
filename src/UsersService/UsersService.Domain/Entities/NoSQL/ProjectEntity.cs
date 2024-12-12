namespace UsersService.Domain.Entities.NoSQL
{
    public class ProjectEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Technologies { get; set; }
        public string Link { get; set; }
    }
}
