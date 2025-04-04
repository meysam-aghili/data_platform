using BlazorApp.Models;
using BlazorApp.Shared;
using BlazorApp.Services.Mongo;


namespace BlazorApp.Services.Repositories;

public class OtpRepository(IMongoService<OtpBsonDocument> otps) : IOtpRepository
{
    public async Task<OtpBsonDocument> IssueAsync(string username)
    {
        var otp = new OtpBsonDocument
        {
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            Username = username.ToLower(),
            OtpString = (username + DateTime.UtcNow.ToString() + Random.Shared.Next().ToString()).ToSHA256().ToLower()
        };
        await otps.CreateAsync(otp);
        return otp;
    }

    public async Task UseAsync(OtpBsonDocument otp)
    {
        otp.Used = true;
        await otps.UpdateAsync(otp);
    }

    public async Task<OtpBsonDocument?> GetAsync(string otp) =>
        await Task.FromResult((
            from o in otps.Query()
            where o.OtpString == otp
            select o
        ).FirstOrDefault());
}
