using System.Data;
using Lab3.Models;

namespace Lab3.Controllers;

public static class ReportController
{
    // REQ-5.1
    public static DataTable Overdue()
    {
        Session.Require("librarian");
        return LoanRepository.Overdue();
    }

    // REQ-5.2: one reminder, 2 days before the due date, never for a returned book
    public static int SendReminders()
    {
        int sent = 0;
        foreach (var loan in LoanRepository.DueForReminder())
        {
            NotificationRepository.Add(loan.MemberId,
                $"Reminder: \"{loan.Title}\" is due on {loan.Due:yyyy-MM-dd}. Please return it or renew it.");
            LoanRepository.MarkReminded(loan.LoanId);
            sent++;
        }
        return sent;
    }

    /// <summary>Background work: expire old holds, keep copies for the next member, send reminders.</summary>
    public static void RunMaintenance()
    {
        ReservationRepository.ProcessHolds();
        SendReminders();
    }
}
