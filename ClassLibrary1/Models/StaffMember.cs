namespace ClassLibrary1.Models;

public partial class StaffMember
{
    public int MemberId { get; set; }

    public string Password { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? EmailAddress { get; set; }

    public int? Role { get; set; }
}
