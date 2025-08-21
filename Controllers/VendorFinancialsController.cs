using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using irevlogix_backend.Data;
using System.Security.Claims;

namespace irevlogix_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VendorFinancialsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VendorFinancialsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{vendorId:int}")]
        public async Task<IActionResult> GetVendorFinancialSummary([FromRoute] int vendorId)
        {
            var clientId = User.FindFirst("ClientId")?.Value;
            if (string.IsNullOrEmpty(clientId)) return Unauthorized();

            var salesData = await _context.ProcessedMaterialSales
                .Where(x => x.VendorId == vendorId && x.ClientId == clientId)
                .ToListAsync();

            if (!salesData.Any())
            {
                return Ok(new
                {
                    totalRevenue = 0m,
                    averageDaysToPay = 0,
                    averageFreightCost = 0m,
                    averageLoadingCost = 0m,
                    averageQuantity = 0m,
                    unpaidInvoicesCount = 0
                });
            }

            var totalRevenue = salesData.Where(x => x.InvoiceTotal.HasValue).Sum(x => x.InvoiceTotal.Value);

            var paidInvoices = salesData.Where(x => x.InvoiceDate.HasValue && x.DateInvoicePaid.HasValue).ToList();
            var averageDaysToPay = 0;
            if (paidInvoices.Any())
            {
                var totalDays = paidInvoices.Sum(x => (x.DateInvoicePaid.Value - x.InvoiceDate.Value).Days);
                averageDaysToPay = (int)Math.Round((double)totalDays / paidInvoices.Count);
            }

            var averageFreightCost = salesData.Where(x => x.FreightCost.HasValue).Any() 
                ? salesData.Where(x => x.FreightCost.HasValue).Average(x => x.FreightCost.Value) 
                : 0m;

            var averageLoadingCost = salesData.Where(x => x.LoadingCost.HasValue).Any() 
                ? salesData.Where(x => x.LoadingCost.HasValue).Average(x => x.LoadingCost.Value) 
                : 0m;

            var averageQuantity = salesData.Where(x => x.SalesQuantity.HasValue).Any() 
                ? salesData.Where(x => x.SalesQuantity.HasValue).Average(x => x.SalesQuantity.Value) 
                : 0m;

            var unpaidInvoicesCount = salesData.Count(x => x.InvoiceDate.HasValue && !x.DateInvoicePaid.HasValue);

            return Ok(new
            {
                totalRevenue,
                averageDaysToPay,
                averageFreightCost,
                averageLoadingCost,
                averageQuantity,
                unpaidInvoicesCount
            });
        }
    }
}
