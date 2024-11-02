namespace UsersService.Domain.Entities.NoSQL
{
    public class Project
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Technologies { get; set; }
        public string Link { get; set; }
    }
}
