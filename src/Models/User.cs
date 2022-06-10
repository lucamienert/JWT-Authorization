using System.ComponentModel.DataAnnotations;

namespace Api.Models;

public class User
{
    [Required]
    public string Username { get; set; } = string.Empty;

    public byte[] PasswordHash { get; set; }
    
    public byte[] PasswordSalt { get; set; }
}