using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreCars.Models;

namespace StoreCars.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, NhanVien")]
    public class HomeController : Controller
    {
        private DataCarContext _context; public static string? image;
        public INotyfService _notyfService { get; }
        private readonly IConfiguration _configuration;
        public HomeController(DataCarContext repo, INotyfService notyfService, IConfiguration configuration)
        {
            _context = repo;
            _notyfService = notyfService;
            _configuration = configuration;
        }
        [Route("admin")]
        public async Task< IActionResult> Index()
        {
            ///truy vấn dữ liệu 
         
            var donhuy =await _context.Oders.Where(x => x.Status == 5).ToArrayAsync();
            var User =await _context.Accounts.Where(x => x.RoleId == 3).ToArrayAsync();
            var DoanhDoanhThu = await _context.Oders.Where(x => x.Status != 1 || x.Status != 5).ToListAsync();
            var sanpham = await _context.Products.Where(x => x.Status == 1).ToListAsync();
            var Giamgia = await _context.Discounts.Where(x => x.Status == 1).ToListAsync();
            var soLuongLoaiSanPham = await _context.ProductTypes.CountAsync();
            var soLuongTaiKhoanQuanLy = await _context.Accounts.CountAsync(a => a.RoleId == 1+2);
            var soLuongDonHangHoanThanh = await _context.Oders.CountAsync(x => x.Status == 4);

            //// tính toán dữ liệu 
            int soluongSP = sanpham.Count();
            decimal? tongTien = DoanhDoanhThu.Sum(x => x.Total);
            int tongdonhang = donhuy.Count() + DoanhDoanhThu.Count();
            int soLuong = User.Count();

            ViewBag.User = soLuong;
            ViewBag.DoanhThu = tongTien;
            ViewBag.Sanpham = soluongSP;
            ViewBag.DonHuy = donhuy.Count();
            ViewBag.TongDonHang = tongdonhang;
            ViewBag.MaGiamGia = Giamgia.Count();
            ViewBag.SoLuongLoaiSanPham = soLuongLoaiSanPham;
            ViewBag.SoLuongTaiKhoanQuanLy = soLuongTaiKhoanQuanLy;
            ViewBag.SoLuongDonHangHoanThanh = soLuongDonHangHoanThanh;
            return View();
        }
    }
}
