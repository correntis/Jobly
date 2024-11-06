namespace UsersService.API.Middleware.Authentication
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class AuthorizeRoleAttribute : Attribute
    {
        public string Roles { get; set; }
    }
}
