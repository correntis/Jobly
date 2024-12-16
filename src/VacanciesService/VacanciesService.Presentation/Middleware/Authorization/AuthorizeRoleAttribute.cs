namespace VacanciesService.Presentation.Middleware.Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class AuthorizeRoleAttribute : Attribute
    {
        public string Roles { get; set; }
    }
}
