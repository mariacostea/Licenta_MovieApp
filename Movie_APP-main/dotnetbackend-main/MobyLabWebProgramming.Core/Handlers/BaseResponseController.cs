using System.Net;
using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Core.Handlers;

/// <summary>
/// This class contains methods for controllers to set the response objects and status codes in a easier way, inherit it in the controller instead of the plain ControllerBase.
/// </summary>
public abstract class BaseResponseController : ControllerBase
{
    /// <summary>
    /// Notice that the following methods adapt the responses or errors to a ActionResult with a status code that will be serialized into the HTTP response body.
    /// </summary>
    protected ActionResult<RequestResponse> ErrorMessageResult(ServerException serverException) =>
        StatusCode((int)serverException.Status, RequestResponse.FromError(ErrorMessage.FromException(serverException))); // The StatusCode method of the controller base will
                                                                                                                                // set the given HTTP status code in the response and will serialize
                                                                                                                                // the response object.

    protected ActionResult<RequestResponse> ErrorMessageResult(ErrorMessage? errorMessage = null) =>
        StatusCode((int)(errorMessage?.Status ?? HttpStatusCode.InternalServerError), RequestResponse.FromError(errorMessage));

    protected ActionResult<RequestResponse<T>> ErrorMessageResult<T>(ErrorMessage? errorMessage = null) =>
        StatusCode((int)(errorMessage?.Status ?? HttpStatusCode.InternalServerError), RequestResponse<T>.FromError(errorMessage));

    protected ActionResult<RequestResponse> FromServiceResponse(ServiceResponse response) =>
        response.Error == null ? Ok(RequestResponse.OkRequestResponse) : ErrorMessageResult(response.Error); // The Ok method of the controller base will set the
                                                                                                               // HTTP status code in the response to 200 Ok and will
                                                                                                               // serialize the response object.

    protected ActionResult<RequestResponse<T>> FromServiceResponse<T>(ServiceResponse<T> response) =>
        response.Error == null ? Ok(RequestResponse<T>.FromServiceResponse(response)) : ErrorMessageResult<T>(response.Error);

    protected ActionResult<RequestResponse> OkRequestResponse() => Ok(RequestResponse.OkRequestResponse);
}