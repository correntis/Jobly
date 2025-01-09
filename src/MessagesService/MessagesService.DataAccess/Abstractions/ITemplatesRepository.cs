namespace MessagesService.DataAccess.Abstractions
{
    public interface ITemplatesRepository
    {
        Task<string> GetTemplateByEvent(string eventName);
        Task<string> GetTemplateByType(int notificationType);
    }
}