public static class CurrentUser
{
    public static string UserID { get; set; }
    public static string Username { get; set; }
    public static string PasswordHash { get; set; }
    public static string Role { get; set; }
    public static string Fullname { get; set; }
    public static string PhoneNumber { get; set; }
    public static string sex { get; set; } // Use "Giới Tính" for gender
    public static DateTime? DateOfBirth { get; set; }
    public static string SinhVienID { get; set; } // Added SinhVienID
}
