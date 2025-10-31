using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Helpers;
using Co_owner_Vehicle.Models;

namespace Co_owner_Vehicle.Filters
{
    public class NotificationPageFilter : IPageFilter
    {
        private readonly CoOwnerVehicleDbContext _context;

        public NotificationPageFilter(CoOwnerVehicleDbContext context)
        {
            _context = context;
        }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            // Not used
        }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            // Not used
        }

        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            if (context.Result is PageResult pageResult)
            {
                // Only populate notifications for authenticated users
                if (context.HttpContext.User.Identity?.IsAuthenticated == true)
                {
                    var userId = GetUserId(context.HttpContext);
                    if (userId > 0)
                    {
                        try
                        {
                            var notifications = LoadNotifications(userId);
                            pageResult.ViewData["Notifications"] = ConvertToViewModel(notifications);
                        }
                        catch
                        {
                            // Silently ignore database errors
                            pageResult.ViewData["Notifications"] = new List<dynamic>();
                        }
                    }
                }
            }
        }

        private int GetUserId(HttpContext httpContext)
        {
            return UserHelper.GetCurrentUserId(httpContext.User);
        }

        private List<Notification> LoadNotifications(int userId)
        {
            try
            {
                return _context.Notifications
                    .AsNoTracking()
                    .Where(n => n.UserId == userId && n.Status == NotificationStatus.Unread)
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(10)
                    .ToList();
            }
            catch
            {
                // Return empty list if database error
                return new List<Notification>();
            }
        }

        private List<dynamic> ConvertToViewModel(List<Notification> notifications)
        {
            var result = new List<dynamic>();

            foreach (var notification in notifications)
            {
                var (icon, color) = GetNotificationStyle(notification.Type);
                result.Add(new
                {
                    Title = notification.Title,
                    Sub = notification.Message,
                    Icon = icon,
                    Color = color
                });
            }

            return result;
        }

        private (string icon, string color) GetNotificationStyle(NotificationType type)
        {
            return type switch
            {
                NotificationType.BookingApproved => ("bi-check-circle", "success"),
                NotificationType.BookingRejected => ("bi-x-circle", "danger"),
                NotificationType.BookingReminder => ("bi-calendar", "info"),
                NotificationType.NewExpense => ("bi-receipt", "primary"),
                NotificationType.ExpenseApproved => ("bi-check-circle", "success"),
                NotificationType.ExpenseRejected => ("bi-x-circle", "danger"),
                NotificationType.PaymentReminder => ("bi-exclamation-circle", "warning"),
                NotificationType.PaymentReceived => ("bi-credit-card", "success"),
                NotificationType.VehicleMaintenance => ("bi-wrench", "info"),
                NotificationType.VotingSession => ("bi-ballot", "primary"),
                NotificationType.GroupInvitation => ("bi-person-plus", "primary"),
                _ => ("bi-bell", "secondary")
            };
        }
    }
}

