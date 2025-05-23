using System.Net;

namespace MobyLabWebProgramming.Core.Errors;

public static class ErrorCodeMappings
{
    public static readonly Dictionary<ErrorCodes, HttpStatusCode> CodeToStatus = new()
    {
        { ErrorCodes.Unknown, HttpStatusCode.InternalServerError },
        { ErrorCodes.TechnicalError, HttpStatusCode.InternalServerError },
        { ErrorCodes.EntityNotFound, HttpStatusCode.NotFound },
        { ErrorCodes.PhysicalFileNotFound, HttpStatusCode.NotFound },
        { ErrorCodes.UserAlreadyExists, HttpStatusCode.Conflict },
        { ErrorCodes.WrongPassword, HttpStatusCode.BadRequest },
        { ErrorCodes.CannotAdd, HttpStatusCode.Forbidden },
        { ErrorCodes.CannotUpdate, HttpStatusCode.Forbidden },
        { ErrorCodes.CannotDelete, HttpStatusCode.Forbidden },
        { ErrorCodes.MailSendFailed, HttpStatusCode.InternalServerError },
        { ErrorCodes.EmailNotConfirmed, HttpStatusCode.BadRequest },
        { ErrorCodes.InvalidValue, HttpStatusCode.BadRequest },
        {ErrorCodes.EntityAlreadyExists, HttpStatusCode.Conflict }
    };
}