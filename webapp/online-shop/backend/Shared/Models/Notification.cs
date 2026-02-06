using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public enum NotificationSource
{
    Sms,
    Email,
    Unknown
}

public enum NotificationType
{
    Verification,
    Advertisment,
    Info,
    Unknown
}

public class Notification : Base
{
    [Required]
    public NotificationSource NotificationSource { get; set; } = NotificationSource.Unknown;
    
    [Required]
    public NotificationType NotificationType { get; set; } = NotificationType.Unknown;

    [Required]
    public string Subject { get; set; } = string.Empty;
    
    [Required]
    public string Body { get; set; } = string.Empty;
    
    [Required]
    public string From { get; set; } = string.Empty;
    
    [Required]
    public string To { get; set; } = string.Empty;
}
