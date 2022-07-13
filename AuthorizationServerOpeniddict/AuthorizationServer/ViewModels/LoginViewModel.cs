using System.ComponentModel.DataAnnotations;

namespace AuthorizationServer.ViewModels;

public class LoginViewModel
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
    public string ReturnURL { get; set; }
}
