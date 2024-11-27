namespace VacanciesService.Domain.Exceptions
{
    public class EntityAlreadyExistException : Exception
    {
        public EntityAlreadyExistException()
        {
        }

        public EntityAlreadyExistException(string message)
            : base(message)
        {
        }

        public EntityAlreadyExistException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
