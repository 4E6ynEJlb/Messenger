using Domain;

namespace Application.Models.Internal
{
    public class DataValidationException : HttpException
    {
        public DataValidationException(string fieldName)
            : base(string.Format(ErrorMessages.VALIDATION_ERROR, fieldName), System.Net.HttpStatusCode.BadRequest)
        {
        }
    }
}
