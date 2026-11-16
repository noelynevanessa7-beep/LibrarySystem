using System.Data;
using Lab3.Models;

namespace Lab3.Controllers;

public static class NotificationController
{
    public static DataTable Mine()
    {
        Session.Require("member");
        return NotificationRepository.ForMember(Session.CurrentUser.Id);
    }

    public static void MarkAllRead()
    {
        Session.Require("member");
        NotificationRepository.MarkAllRead(Session.CurrentUser.Id);
    }
}
