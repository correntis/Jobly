namespace UsersService.Domain.Exceptions
{
    public class ValidationException : Exception
    {
        public IEnumerable<ValidationError> Errors { get; set; }
        public ValidationException()
        {
            Errors = [];
        }

        public ValidationException(IEnumerable<ValidationError> errors)
            : base("Validation error")
        {
            Errors = errors;
        }

        public ValidationException(IEnumerable<ValidationError> errors, Exception inner)
            : base("Validation error", inner)
        {
            Errors = errors;
        }
    }
}
