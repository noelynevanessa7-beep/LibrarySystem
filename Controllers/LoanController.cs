using System.Data;
using Lab3.Models;

namespace Lab3.Controllers;

public static class LoanController
{
    // REQ-4.1: returns the due date
    public static DateTime Issue(int bookId, string memberNumber)
    {
        Session.Require("librarian");
        if (string.IsNullOrWhiteSpace(memberNumber))
            throw new AppException("Enter the member's student or staff number.");

        var member = UserRepository.FindByMemberNumber(memberNumber.Trim());
        if (member == null || member.Role != "member")
            throw new AppException("No member found with that number.");

        var book = BookRepository.Find(bookId) ?? throw new AppException("This book no longer exists.");
        ReservationRepository.ProcessHolds();

        // The member the copy is kept for may take it
        var hold = ReservationRepository.ReadyHold(bookId, member.Id);
        if (hold != null)
        {
            ReservationRepository.SetStatus(hold.Id, "collected");
            return LoanRepository.Issue(bookId, member.Id);
        }

        if (BookRepository.FreeCopies(bookId) <= 0)
        {
            if (book.Copies - BookRepository.ActiveLoans(bookId) > 0)
                throw new AppException("The only free copy is kept for another member.");
            throw new AppException("No copy of this book is available.");
        }
        return LoanRepository.Issue(bookId, member.Id);
    }

    // REQ-4.2
    public static void Return(int loanId)
    {
        Session.Require("librarian");
        var loan = LoanRepository.Find(loanId);
        if (loan == null || loan.Returned) throw new AppException("This loan is already closed.");

        LoanRepository.MarkReturned(loanId);
        ReservationRepository.ProcessHolds();   // keep the copy for the first member in line
    }

    // REQ-4.3
    public static void Renew(int loanId)
    {
        Session.Require("member");
        var loan = LoanRepository.Find(loanId);
        if (loan == null || loan.MemberId != Session.CurrentUser.Id || loan.Returned)
            throw new AppException("You can only renew your own current loans.");
        if (loan.Renewed)
            throw new AppException("This loan was already renewed once.");
        if (ReservationRepository.OthersWaiting(loan.BookId, Session.CurrentUser.Id))
            throw new AppException("This loan cannot be renewed because another member has reserved the book.");

        LoanRepository.Renew(loanId);
    }

    public static DataTable Mine()
    {
        Session.Require("member");
        return LoanRepository.ForMember(Session.CurrentUser.Id);
    }

    public static DataTable Active()
    {
        Session.Require("librarian");
        return LoanRepository.Active();
    }
}
