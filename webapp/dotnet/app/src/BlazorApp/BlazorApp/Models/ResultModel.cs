namespace BlazorApp.Models;

public class ResultModel
{
    public bool IsSuccess { get; }
    public List<dynamic>? Value { get; }
    public Dictionary<string, object>? Metadata { get; }
    public ErrorModel? Error { get; }

    private ResultModel(
        bool isSuccess,
        List<dynamic>? value,
        Dictionary<string, object>? metadata,
        ErrorModel? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Metadata = metadata;
        Error = error;
    }

    public static ResultModel Success(
        List<dynamic>? value = default,
        Dictionary<string, object>? metadata = default) =>
            new(true, value, metadata, null);

    public static ResultModel Failure(ErrorModel error) => new(false, default, default, error);
}

public record ResponseModel(
    long Took,
    int? TotalRecords,
    DateTimeOffset ReceivedAt,
    object? Metadata,
    object? Data
);

public record ErrorResponseModel(
    int StatusCode,
    string Error,
    string Message,
    DateTimeOffset Timestamp,
    long DurationMs
);
