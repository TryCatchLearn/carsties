using System.ComponentModel.DataAnnotations;

namespace IdentityService.Pages.Account.Register;

public class InputModel
{
    [Required] public string? Email { get; set; }
    [Required] public string? Username { get; set; }
    [Required] public string? FullName { get; set; }
    [Required] public string? Password { get; set; }
    public string? ReturnUrl { get; set; }
    public string? Button { get; set; }
}