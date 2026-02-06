using Api.Services;
using Api.Services.Repositories;
using Api.Shared;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    NotificationRepository notificationRepository, 
    UserRepository userRepository,
    JwtTokenService jwtTokenService,
    IConfiguration config) : ControllerBase
{
    private readonly NotificationRepository _notificationRepository = notificationRepository;
    private readonly UserRepository _userRepository = userRepository;
    private readonly JwtTokenService _jwtTokenService = jwtTokenService;
    private readonly string _smsFromNumber = config.GetConfig("SMS_FROM_NUMBER");

    [HttpPost("send-verification-code")]
    public async Task<IActionResult> PostSendVerificationCode([FromBody] SendVerificationCodeRequest request)
    {
        //SendSms(payload)
        await _notificationRepository.AddAsync(
            new() {
                NotificationSource = NotificationSource.Sms,
                NotificationType = NotificationType.Verification,
                From = _smsFromNumber,
                To = request.To,
                Subject = RandomNumberGenerator.GetInt32(1000, 10000).ToString(),
            }
        );
        return Ok();
    }

    [HttpPost("verify-verification-code")]
    public async Task<IActionResult> PostVerifyVerificationCode([FromBody] VerifyVerificationCodeRequest request)
    {
        var model = await _notificationRepository.GetLastRecentAsync(request.To);
        if (model is null || model.Subject != request.Code)
        {
            return BadRequest("Invalid verification code.");
        }

        var user = await _userRepository.GetAsync(request.To);
        user ??= await _userRepository.AddAsync(new() { Phone = request.To, Slug = request.To});

        var token = _jwtTokenService.GenerateToken(user);

        return Ok(new { token });
    }
}

public class SendVerificationCodeRequest
{
    [Required]
    public string To { get; set; } = string.Empty;
}
public class VerifyVerificationCodeRequest
{
    [Required]
    public string To { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;
}