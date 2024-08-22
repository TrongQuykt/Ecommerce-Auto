using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreCars.Models;

namespace StoreCars.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, NhanVien")]

    public class OrderController : Controller
    {
        private DataCarContext _context; public static string? image;
        public INotyfService _notyfService { get; }
        private readonly IConfiguration _configuration;
        public OrderController(DataCarContext repo, INotyfService notyfService, IConfiguration configuration)
        {
            _context = repo;
            _notyfService = notyfService;
            _configuration = configuration;
        }
        [Route("Admin/Order/XacNhan")]
        public async Task< IActionResult> Index()
        {
            var donhang = await _context.Oders.Include(x => x.Account).ThenInclude(x => x.Addresses).Where(x => x.Status == 2).ToListAsync();
            return View(donhang);
        }

        //public async Task<IActionResult> ConfimOrder(int id)
        //{
        //    var oder =await _context.Oders.FirstOrDefaultAsync(x=>x.OdersId == id);
        //    if (oder == null)
        //    {
        //        return NotFound();
        //    }
        //    oder.Status = 3;
        //    await _context.SaveChangesAsync();
        //    _notyfService.Success("Xác nhận thành công");
        //    return RedirectToAction("Index");
        //}
        public async Task<IActionResult> ConfimOrder(int id)
        {
            var order = await _context.Oders.FirstOrDefaultAsync(o => o.OdersId == id);
            if (order == null)
            {
                return NotFound();
            }

            if (order.Status == 2) // Chỉ xác nhận khi đơn hàng đang chờ xác nhận
            {
                order.Status = 3; // Đang vận chuyển
                await _context.SaveChangesAsync();
                _notyfService.Success("Đơn hàng đã được xác nhận vận chuyển.");
            }
            else
            {
                _notyfService.Warning("Đơn hàng không ở trạng thái chờ xác nhận.");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CanOrderby(int id)
        {
            var oder =await _context.Oders.FirstOrDefaultAsync(x=>x.OdersId == id);
            if (oder == null)
            {
                return NotFound();
            }
            oder.Status = 5;
            await _context.SaveChangesAsync();
            _notyfService.Success("Hủy đơn thành công");
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> CanOrder()
        {
            var donhang = await _context.Oders.Include(x => x.Account).ThenInclude(x => x.Addresses).Where(x => x.Status == 5).ToListAsync();
            return View(donhang);
        }

        public async Task<IActionResult> OrderShip()
        {
            var donhang = await _context.Oders.Include(x => x.Shipments).Include(x => x.Account).ThenInclude(x => x.Addresses).Where(x => x.Status == 3 && x.Shipments.FirstOrDefault().Status == 1).ToListAsync();
            return View(donhang);
        }
        public async Task<IActionResult> ConfimShip(int id)
        {
            var ship = await _context.Shipments.FirstOrDefaultAsync(x => x.OdersId == id);
            if (ship == null)
            {
                return NotFound();
            }
            ship.Status = 2;
            await _context.SaveChangesAsync();
            _notyfService.Success("Xác nhận giao hàng thành công");
            return RedirectToAction("OrderShip");
        }


        public async Task<IActionResult> OrderDone()
        {
            var donhang = await _context.Oders.Include(x => x.Shipments).Include(x => x.Account).
                ThenInclude(x => x.Addresses)
                .Where(x => x.Status == 4 && x.Shipments.FirstOrDefault().Status == 2).ToListAsync();
            return View(donhang);
        }
        public async Task<IActionResult> OrderDetails(int id)
        {
            //var order = await _context.Oders.Include(o => o.OderItems)
            //                                .ThenInclude(od => od.Product)
            //                                .FirstOrDefaultAsync(o => o.OdersId == id);
            var order = await _context.Oders
                           .Include(o => o.Account)
                               .ThenInclude(a => a.Addresses)
                           .Include(o => o.OderItems)
                               .ThenInclude(od => od.Product)
                           .FirstOrDefaultAsync(o => o.OdersId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
