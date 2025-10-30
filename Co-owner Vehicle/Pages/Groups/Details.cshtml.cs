using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Co_owner_Vehicle.Pages.Groups
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly IGroupService _groupService;
        private readonly IExpenseService _expenseService;
        private readonly IBookingService _bookingService;
        private readonly ICommonFundService _fundService;
        private readonly IUserService _userService;
        private readonly IVotingService _votingService;

        public DetailsModel(
            IGroupService groupService,
            IExpenseService expenseService,
            IBookingService bookingService,
            ICommonFundService fundService,
            IUserService userService,
            IVotingService votingService)
        {
            _groupService = groupService;
            _expenseService = expenseService;
            _bookingService = bookingService;
            _fundService = fundService;
            _userService = userService;
            _votingService = votingService;
        }

        public CoOwnerGroup? Group { get; set; }
        public List<GroupMember> Members { get; set; } = new();
        public List<OwnershipShare> Shares { get; set; } = new();

        // Sidebar stats & common fund
        public class GroupStats
        {
            public decimal MonthExpense { get; set; }
            public int BookingCount { get; set; }
            public int TotalKm { get; set; }
            public DateTime CreatedAt { get; set; }
            public decimal FundBalance { get; set; }
        }

        public GroupStats Stats { get; set; } = new();

        // Add member form
        [BindProperty]
        public int? SelectedUserId { get; set; }
        [BindProperty]
        public decimal? NewMemberShare { get; set; }
        [BindProperty]
        public MemberRole NewMemberRole { get; set; } = MemberRole.Member;
        public List<User> UsersForAdd { get; set; } = new();

        // Create voting form
        [BindProperty]
        public string? VotingTitle { get; set; }
        [BindProperty]
        public string? VotingDescription { get; set; }
        [BindProperty]
        public DecisionType VotingDecisionType { get; set; } = DecisionType.Other;
        [BindProperty]
        public DateTime? VotingEndDate { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();
            Group = await _groupService.GetGroupByIdAsync(id.Value);
            if (Group == null) return NotFound();
            Members = await _groupService.GetGroupMembersAsync(id.Value);
            Shares = await _groupService.GetOwnershipSharesAsync(id.Value);

            // Compute stats
            var today = DateTime.UtcNow.Date;
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var expenses = await _expenseService.GetExpensesByGroupIdAsync(id.Value);
            var monthExpense = expenses
                .Where(e => e.ExpenseDate >= monthStart)
                .Sum(e => e.Amount);

            var vehicleId = Group.VehicleId;
            var bookings = await _bookingService.GetBookingsByVehicleIdAsync(vehicleId);
            var bookingCount = bookings.Count(b => b.CreatedAt >= monthStart);

            var totalKm = Group.Vehicle?.CurrentMileage ?? 0;

            var fund = await _fundService.GetFundByGroupIdAsync(id.Value);
            var fundBalance = fund != null ? await _fundService.GetFundBalanceAsync(fund.CommonFundId) : 0;

            Stats = new GroupStats
            {
                MonthExpense = monthExpense,
                BookingCount = bookingCount,
                TotalKm = totalKm,
                CreatedAt = Group.CreatedAt,
                FundBalance = fundBalance
            };
            var allUsers = await _userService.GetAllUsersAsync();
            var memberUserIds = Members.Select(m => m.UserId).ToHashSet();
            UsersForAdd = allUsers.Where(u => u.IsActive && !memberUserIds.Contains(u.UserId)).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostCreateVotingAsync(int id)
        {
            Group = await _groupService.GetGroupByIdAsync(id);
            if (Group == null) return NotFound();

            if (string.IsNullOrWhiteSpace(VotingTitle))
            {
                ModelState.AddModelError(string.Empty, "Tiêu đề voting không được để trống");
                return await OnGetAsync(id);
            }

            var session = new VotingSession
            {
                CoOwnerGroupId = id,
                Title = VotingTitle!,
                Description = VotingDescription ?? string.Empty,
                DecisionType = VotingDecisionType,
                Status = VotingStatus.Active,
                StartDate = DateTime.UtcNow,
                EndDate = VotingEndDate ?? DateTime.UtcNow.AddDays(7)
            };

            await _votingService.CreateSessionAsync(session);
            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostAddMemberAsync(int id)
        {
            Group = await _groupService.GetGroupByIdAsync(id);
            if (Group == null) return NotFound();

            if (!SelectedUserId.HasValue)
            {
                ModelState.AddModelError(string.Empty, "Vui lòng chọn người dùng");
                return await OnGetAsync(id);
            }

            var user = await _userService.GetUserByIdAsync(SelectedUserId.Value);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Không tìm thấy người dùng");
                return await OnGetAsync(id);
            }

            // Xác định phần trăm muốn cấp cho thành viên mới (mặc định 50%)
            var newMemberPercentage = NewMemberShare.HasValue && NewMemberShare.Value > 0
                ? Math.Min(100m, NewMemberShare.Value)
                : 50m;

            // Lấy danh sách thành viên và shares hiện tại
            var groupMembers = await _groupService.GetGroupMembersAsync(id);
            var shares = await _groupService.GetOwnershipSharesAsync(id);
            var owner = groupMembers.FirstOrDefault(m => m.Role == MemberRole.Owner);

            // Xác định người bị trừ phần trăm: ưu tiên owner, nếu không có thì người có % cao nhất
            var donorUserId = owner?.UserId
                ?? shares.OrderByDescending(s => s.Percentage).FirstOrDefault()?.UserId;

            if (donorUserId.HasValue)
            {
                var donorShare = shares.FirstOrDefault(s => s.UserId == donorUserId.Value)
                                ?? await _groupService.GetUserOwnershipShareAsync(id, donorUserId.Value);
                var donorCurrent = donorShare?.Percentage ?? 100m;
                var donorUpdated = Math.Max(0m, donorCurrent - newMemberPercentage);
                await _groupService.UpdateOwnershipShareAsync(id, donorUserId.Value, donorUpdated, donorShare?.InvestmentAmount);
            }

            // Thêm thành viên mới với tỷ lệ đã chọn
            await _groupService.AddMemberToGroupAsync(id, user.UserId, NewMemberRole);
            await _groupService.UpdateOwnershipShareAsync(id, user.UserId, newMemberPercentage, null);

            // Chuẩn hóa tổng sở hữu về đúng 100% (nếu sai lệch do dữ liệu cũ)
            var updatedShares = await _groupService.GetOwnershipSharesAsync(id);
            var total = updatedShares.Sum(s => s.Percentage);
            if (Math.Abs(total - 100m) > 0.01m)
            {
                // Điều chỉnh cổ phần của người có tỷ lệ lớn nhất để tổng = 100
                var biggest = updatedShares.OrderByDescending(s => s.Percentage).FirstOrDefault();
                if (biggest != null)
                {
                    var adjust = 100m - total;
                    var newBiggest = Math.Max(0m, biggest.Percentage + adjust);
                    await _groupService.UpdateOwnershipShareAsync(id, biggest.UserId, newBiggest, biggest.InvestmentAmount);
                }
            }

            TempData["SuccessMessage"] = "Đã thêm thành viên vào nhóm";
            return RedirectToPage(new { id });
        }

    }
}

