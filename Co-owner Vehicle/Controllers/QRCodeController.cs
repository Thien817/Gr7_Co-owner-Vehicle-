using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;
using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using Microsoft.EntityFrameworkCore;

namespace Co_owner_Vehicle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Staff,Admin")]
    public class QRCodeController : ControllerBase
    {
        private readonly CoOwnerVehicleDbContext _context;
        private readonly ILogger<QRCodeController> _logger;

        public QRCodeController(CoOwnerVehicleDbContext context, ILogger<QRCodeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Generate QR code data for a vehicle
        /// </summary>
        [HttpGet("generate/{vehicleId}")]
        public async Task<IActionResult> GenerateQRCode(int vehicleId)
        {
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            if (vehicle == null)
                return NotFound(new { message = "Vehicle not found" });

            // Generate QR code data with vehicle info, timestamp, and signature
            var timestamp = DateTime.UtcNow;
            var data = $"{vehicle.VehicleId}|{vehicle.LicensePlate}|{timestamp:yyyyMMddHHmmss}";
            
            // Simple signature (in production, use proper digital signature)
            var signature = GenerateSignature(data);
            var qrData = $"{data}|{signature}";

            return Ok(new 
            { 
                qrData = qrData,
                vehicleId = vehicle.VehicleId,
                vehicleName = $"{vehicle.Brand} {vehicle.Model}",
                licensePlate = vehicle.LicensePlate,
                timestamp = timestamp
            });
        }

        /// <summary>
        /// Process scanned QR code
        /// </summary>
        [HttpPost("scan")]
        public async Task<IActionResult> ScanQRCode([FromBody] ScanQRRequest request)
        {
            if (string.IsNullOrEmpty(request.QRData))
                return BadRequest(new { message = "QR data is required" });

            try
            {
                // Parse QR data
                var parts = request.QRData.Split('|');
                if (parts.Length < 4)
                    return BadRequest(new { message = "Invalid QR code format" });

                var vehicleId = int.Parse(parts[0]);
                var licensePlate = parts[1];
                var timestampStr = parts[2];
                var signature = parts[3];

                // Validate signature
                var dataToCheck = $"{vehicleId}|{licensePlate}|{timestampStr}";
                var expectedSignature = GenerateSignature(dataToCheck);
                if (signature != expectedSignature)
                    return BadRequest(new { message = "Invalid QR code signature" });

                var vehicle = await _context.Vehicles.FindAsync(vehicleId);
                if (vehicle == null)
                    return NotFound(new { message = "Vehicle not found" });

                if (vehicle.LicensePlate != licensePlate)
                    return BadRequest(new { message = "Vehicle license plate mismatch" });

                // Get current user
                var userIdClaim = User.FindFirst("UserId")?.Value 
                    ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
                    return Unauthorized(new { message = "User not authenticated" });

                // Determine if this is check-in or check-out based on latest record
                var latestRecord = await _context.CheckInOutRecords
                    .Where(r => r.VehicleId == vehicleId && r.UserId == userId)
                    .OrderByDescending(r => r.CheckTime)
                    .FirstOrDefaultAsync();

                var checkInOutType = latestRecord == null || latestRecord.Type == CheckInOutType.CheckOut
                    ? CheckInOutType.CheckIn
                    : CheckInOutType.CheckOut;

                // Create check-in/out record
                var record = new CheckInOutRecord
                {
                    VehicleId = vehicleId,
                    UserId = userId,
                    Type = checkInOutType,
                    Status = CheckInOutStatus.Completed,
                    CheckTime = DateTime.UtcNow,
                    QRCodeData = request.QRData,
                    Notes = request.Notes,
                    Mileage = request.Mileage,
                    VehicleCondition = request.VehicleCondition,
                    ProcessedBy = userId,
                    ProcessedAt = DateTime.UtcNow
                };

                _context.CheckInOutRecords.Add(record);
                await _context.SaveChangesAsync();

                return Ok(new 
                { 
                    message = "Check-in/Check-out recorded successfully",
                    recordId = record.CheckInOutRecordId,
                    type = checkInOutType.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing QR code scan");
                return StatusCode(500, new { message = "Error processing QR code", error = ex.Message });
            }
        }

        private string GenerateSignature(string data)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data + "secret_key"));
            return Convert.ToBase64String(hashBytes).Substring(0, 16); // Use first 16 chars as signature
        }
    }

    public class ScanQRRequest
    {
        public string QRData { get; set; } = string.Empty;
        public int? Mileage { get; set; }
        public string? VehicleCondition { get; set; }
        public string? Notes { get; set; }
    }
}

