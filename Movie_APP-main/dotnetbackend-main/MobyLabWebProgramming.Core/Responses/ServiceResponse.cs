using System.Net;
using MobyLabWebProgramming.Core.Errors;

namespace MobyLabWebProgramming.Core.Responses;

/// <summary>
/// These classes are used as responses from service methods as either a success responses or as error responses.
/// </summary>
public class ServiceResponse
{
    public ErrorMessage? Error { get; private init; }
    public bool IsOk => Error == null;

    public static ServiceResponse FromError(ErrorMessage? error) => new() { Error = error };
    public static ServiceResponse<T> FromError<T>(ErrorMessage? error) => new() { Error = error };
    public static ServiceResponse ForSuccess() => new();
    public static ServiceResponse<T> ForSuccess<T>(T data) => new() { Result = data };
    public ServiceResponse ToResponse<T>(T result) => Error == null ? ForSuccess(result) : FromError(Error);

    protected ServiceResponse() { }
}

public class ServiceResponse<T> : ServiceResponse
{
    public T? Result { get; init; }
    public ServiceResponse ToResponse() => Error == null ? ForSuccess() : FromError(Error);

    protected internal ServiceResponse() { }
}

/// <summary>
/// These are extension methods for the ServiceResponse classes.
/// They can be used to functionally process data within the ServiceResponse object.
/// </summary>
public static class ServiceResponseExtension
{
    public static ServiceResponse<TOut> Map<TIn, TOut>(this ServiceResponse<TIn> response, Func<TIn, TOut> selector) where TIn : class where TOut : class =>
        response.Result != null ? ServiceResponse.ForSuccess(selector(response.Result)) : ServiceResponse.FromError<TOut>(response.Error);

    public static async Task<ServiceResponse<TOut>> MapAsync<TIn, TOut>(this ServiceResponse<TIn> response, Func<TIn, Task<TOut>> selector) where TIn : class where TOut : class =>
        response.Result != null ? ServiceResponse.ForSuccess(await selector(response.Result)) : ServiceResponse.FromError<TOut>(response.Error);

    public static ServiceResponse<PagedResponse<TOut>> Map<TIn, TOut>(this ServiceResponse<PagedResponse<TIn>> response, Func<TIn, TOut> selector) =>
        response.Result != null ? ServiceResponse.ForSuccess(response.Result.Map(selector)) : ServiceResponse.FromError<PagedResponse<TOut>>(response.Error);

    public static ServiceResponse<TOut> FlatMap<TIn, TOut>(this ServiceResponse<TIn> response, Func<TIn, ServiceResponse<TOut>> selector) where TIn : class where TOut : class =>
        response.Result != null ? selector(response.Result) : ServiceResponse.FromError<TOut>(response.Error);

    public static ServiceResponse<TOut> FlatMap<TOut>(this ServiceResponse response, Func<ServiceResponse<TOut>> selector) where TOut : class =>
        response.Error == null ? selector() : ServiceResponse.FromError<TOut>(response.Error);

    public static ServiceResponse<TIn> Flatten<TIn>(this ServiceResponse<ServiceResponse<TIn>> response) where TIn : class =>
        response.Result ?? ServiceResponse.FromError<TIn>(response.Error);

    public static ServiceResponse Flatten(this ServiceResponse<ServiceResponse> response) =>
        response.Result ?? ServiceResponse.FromError(response.Error);

    public static ServiceResponse<T> ToServiceResponse<T>(this T data) => ServiceResponse.ForSuccess(data);
    public static ServiceResponse<T> ToServiceError<T>(this ErrorMessage error) => ServiceResponse.FromError<T>(error);
    public static ServiceResponse ToServiceError(this ErrorMessage error) => ServiceResponse.FromError(error);

    public static ServiceResponse ToServiceResponseFromException(this Exception ex) =>
        ServiceResponse.FromError(ex is ServerException serverException
            ? ErrorMessage.FromException(serverException)
            : new ErrorMessage(HttpStatusCode.InternalServerError, "A unexpected error occurred!"));
    
    public static ServiceResponse<T> ToServiceResponseFromException<T>(this Exception ex) =>
        ServiceResponse.FromError<T>(ex is ServerException serverException
            ? ErrorMessage.FromException(serverException)
            : new ErrorMessage(HttpStatusCode.InternalServerError, "A unexpected error occurred!"));
}
