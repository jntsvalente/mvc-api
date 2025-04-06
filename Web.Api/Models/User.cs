namespace Web.Api.Models;
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";

    public IList<Role>? Roles { get; set; }
}