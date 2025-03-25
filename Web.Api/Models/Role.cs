namespace Web.Api.Models;
public class Role
{
    public const string Admin = "Admin";
    public const string Member = "Member";
    public const int AdminId = 1;
    public const int MemberId = 2;

    public int Id { get; set; }
    public string Name { get; set; } = "";

    public IList<User>? Users { get; set; }
}