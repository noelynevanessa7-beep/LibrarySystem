using Lab3.Models;

namespace Lab3.Controllers;

/// <summary>Who is logged in. Controllers call Require() so a user can only do what their role allows (REQ-1.3).</summary>
public static class Session
{
    public static User CurrentUser { get; private set; }

    public static void Start(User user) => CurrentUser = user;
    public static void End() => CurrentUser = null;

    public static void Require(params string[] roles)
    {
        if (CurrentUser == null || !roles.Contains(CurrentUser.Role))
            throw new AppException("Access denied");
    }
}
