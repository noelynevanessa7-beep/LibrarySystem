using System.Data;
using Lab3.Models;

namespace Lab3.Controllers;

public static class ReservationController
{
    // REQ-3.1: returns the member's place in the waiting list
    public static int Reserve(int bookId)
    {
        Session.Require("member");
        ReservationRepository.ProcessHolds();

        if (BookRepository.Find(bookId) == null) throw new AppException("This book no longer exists.");
        if (ReservationRepository.HasActive(bookId, Session.CurrentUser.Id))
            throw new AppException("You have already reserved this book.");
        if (BookRepository.FreeCopies(bookId) > 0)
            throw new AppException("This book has an available copy, so it cannot be reserved. Ask the librarian to lend it to you.");

        return ReservationRepository.Create(bookId, Session.CurrentUser.Id);
    }

    // REQ-3.2: the members behind move up because places are counted live
    public static void Cancel(int reservationId)
    {
        Session.Require("member");
        var r = ReservationRepository.Find(reservationId);
        if (r == null || r.MemberId != Session.CurrentUser.Id || (r.Status != "waiting" && r.Status != "ready"))
            throw new AppException("You can only cancel your own active reservations.");

        ReservationRepository.SetStatus(reservationId, "cancelled");
        ReservationRepository.ProcessHolds();   // a cancelled "ready" copy goes to the next member
    }

    public static DataTable Mine()
    {
        Session.Require("member");
        ReservationRepository.ProcessHolds();
        return ReservationRepository.ForMember(Session.CurrentUser.Id);
    }
}
