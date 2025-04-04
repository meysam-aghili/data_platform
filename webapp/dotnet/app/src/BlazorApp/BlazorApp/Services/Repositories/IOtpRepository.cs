using BlazorApp.Models;

namespace BlazorApp.Services.Repositories;

public interface IOtpRepository
{
    Task<OtpBsonDocument> IssueAsync(string username);
    Task UseAsync(OtpBsonDocument otp);
    Task<OtpBsonDocument?> GetAsync(string otp);
}
