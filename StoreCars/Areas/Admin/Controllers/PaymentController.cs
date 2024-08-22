using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreCars.Models;
using StoreCars.ViewModel;
namespace StoreCars.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PaymentController : Controller
    {
        private readonly DataCarContext _context;
        public PaymentController(DataCarContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        // GET: Admin/Transactions
        public IActionResult Transactions()
        {
            var completedPayments = _context.Payments
                .Include(p => p.Oders)
                    .ThenInclude(o => o.Account)
                .Include(p => p.PaymentMethods)
                .Where(p => p.Status == 1) // Lọc ra các giao dịch đã thanh toán
                .ToList();

            return View(completedPayments);
        }
    }
}
