using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using StoreCars.Models;
using WebBanOto.Extension;
using WebBanOto.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using MimeKit;
using System.Security.Cryptography;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace WebBanSon.Controllers
{
    public class AccountController : Controller
    {

        private readonly DataCarContext _context;
        public static string image;
        public INotyfService _notyfService { get; }
        public AccountController(DataCarContext context, INotyfService notyfService)
        {

            _context = context;
            _notyfService = notyfService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string user, string pass)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var password = pass.ToMD5();
            // Kiểm tra tên đăng nhập và mật khẩu
            var account = await _context.Accounts.Include(x => x.Addresses).FirstOrDefaultAsync(u => u.UserName == user && u.Password == password);
            if (account == null)
            {
                // Tên đăng nhập hoặc mật khẩu không đúng
                _notyfService.Error("Thông tin đăng nhập không chính xác");
                return RedirectToAction("Login", "Account");
            }
            if (account?.RoleId == 1 || account?.RoleId == 2)
            {
                _notyfService.Error("Tài khoản của bạn là tài khoản Admin");
                return RedirectToAction("Index", "Home");
            }
            if (account?.Status == 2)
            {
                _notyfService.Error("Tài khoản đã bị khóa");
                return RedirectToAction("Login", "Account");
            }
            if (account != null)
            {
                // Lưu thông tin người dùng vào cookie xác thực
                List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, account.FullName),
                        new Claim("UserName" , account.UserName),
                        new Claim("Id" , account.AccountId.ToString()),
                        new Claim("Avartar", "/contents/Images/User/" + account.Avartar), // Thêm đường dẫn đến ảnh đại diện vào claims
                        //new Claim("Avartar", "/contents/Images/Admin/" + account.Avartar) // Thêm đường dẫn đến ảnh đại diện vào claims
                    };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                _notyfService.Success("Đăng nhập thành công");
                // Chuyển hướng đến trang chủ
                return RedirectToAction("Index", "Home");
            }
            else
            {
                _notyfService.Warning("Tên đăng nhập hoặc mật khẩu không đúng");
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _notyfService.Success("Đăng xuất thành công");
            return RedirectToAction("Index", "Home");
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(String email)
        {
            if (email == null)
            {
                return NotFound();
            }
            var user = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == email);
            
            if (user == null)
            {
                _notyfService.Error("Email ko tồn tại trong hệ thống");
                return View();
            }

            var token = Guid.NewGuid().ToString();

            user.ResetToken = token;
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(10);
            HttpContext.Session.SetString("ResetToken", token);
            HttpContext.Session.SetString("Email", email);

            // guiwr mail 
            var _email = new MimeMessage();
            _email.From.Add(new MailboxAddress("ASUS TUF Gaming F15 FX506HCB_FX506HCB", "trongquykt11@gmail.com"));
            _email.To.Add(MailboxAddress.Parse($"{email}"));
            _email.Subject = "Xác nhận mã độc";
            _email.Body = new TextPart("plain")
            {
                Text = $"Để sao lưu tất cả các cookies, vui lòng sử dụng mã này để xác minh với máy chủ: {token}"
            };
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("vyquy633@gmail.com", "tmbwwcizvetibzle"); 
            smtp.Send(_email);
            smtp.Disconnect(true);

            _notyfService.Success("Yêu cầu đặt lại mật khẩu thành công!!!");
           
            return RedirectToAction("ResetPassword","Account");
        }

        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string token , string pass , string newpass)
        {
            if(pass != newpass)
            {
                _notyfService.Error("Mật khẩu không khớp");
                return View();
            }
            var resetToken = HttpContext.Session.GetString("ResetToken");
           
            var email = HttpContext.Session.GetString("Email");

            var user = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == email);
            if (resetToken != token)
            {
                _notyfService.Error("Token không hợp lệ hoặc hết hạn");
                return View();
            }

            user.Password = pass.ToMD5();
            await _context.SaveChangesAsync();
            _notyfService.Success("Đổi mật khẩu thành công");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }
       
        [HttpPost]
        public async Task<IActionResult> Register(Account account)
        {
            if (account.UserName?.Length < 6)
            {
                _notyfService.Error("Tài khoản không bé hơn 6 kí tự");
                return View(account);
            }
            if (account.Password?.Length < 6)
            {
                _notyfService.Error("Mật khẩu không bé hơn 6 kí tự");
                return View(account);
            } 
            if (account.Phone?.Length != 10)
            {
                _notyfService.Error("Số điện thoại là 10 số");
                return View(account);
            }
            var exaccount = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == account.Email || x.UserName == account.UserName);
            if (exaccount != null)
            {
                _notyfService.Error("Email hoặc Username đã tồn tại");
                return View(account);
            }
            account.Avartar = "";
            account.Password = (account.Password)?.ToMD5();
            account.Status = 1;
            account.AccountTypeId = 1;
            account.Point = 0;
            account.RoleId = 3;

            _context.Update(account);
            await _context.SaveChangesAsync();
            _notyfService.Success("Đăng ký thành công");
            return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        public async Task<IActionResult> Create(string streest, string ward, string distrist, string city, string contry , string url)
        {
            var Idclam = User.Claims.SingleOrDefault(c => c.Type == "Id");
            int Id = 0;
            if (Idclam != null)
            { Id = Int32.Parse(Idclam.Value); }

            Address address = new Address
            {
                Street = streest,
                City = city,
                Ward = ward,
                District = distrist,
                Country = contry,
                AccountId = Id
            };
            _context.Update(address);
            await _context.SaveChangesAsync();
            _notyfService.Success("Thêm địa chỉ thành công");
            if (url != null)
            {
                return Redirect(url);
            }
            return RedirectToAction("Profile", "Account");
        }
        public async Task<IActionResult> Edit(int id)
        {
            var address = await _context.Addresses.FirstOrDefaultAsync(x => x.AddressId == id);
            return Ok(address);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int addresid, string streest, string ward, string distrist, string city, string contry)
        {
            var addresr = await _context.Addresses.FirstOrDefaultAsync(x => x.AddressId == addresid);
            if (addresr == null)
            {
                return NotFound();
            }
            addresr.Street = streest;
            addresr.City = city;
            addresr.Ward = ward;
            addresr.District = distrist;
            addresr.Country = contry;
            await _context.SaveChangesAsync();
            _notyfService.Success("Sửa địa chỉ thành công");
            return RedirectToAction("Profile", "Account");
        }

      
        [HttpPost]
        public async Task<IActionResult> DeleteAd(int addid)
        {
            var addresr = await _context.Addresses.FirstOrDefaultAsync(x => x.AddressId == addid);
            if (addresr == null)
            {
                return NotFound();
            }
            _context.Addresses.Remove(addresr);
            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa địa chỉ thành công");
            return RedirectToAction("Profile", "Account");
        }
        public async Task<IActionResult> Profile()
        {
            var Idclam = User.Claims.SingleOrDefault(c => c.Type == "Id");
            int Id = 0;

            if (Idclam != null)
            { Id = Int32.Parse(Idclam.Value); }
            var donhang =await _context.Oders.Where(x=>x.AccountId == Id)
                .Include(x=>x.OderItems).ThenInclude(x=>x.Product).Include(x=>x.Account).ThenInclude(x=>x.Addresses).ToListAsync();
            if (donhang == null)
            {
                return NotFound();  
            }
            var account = await _context.Accounts.Include(x => x.Addresses).FirstOrDefaultAsync(x => x.AccountId == Id);

            if (account == null)
            {
                return NotFound();
            }
            ViewBag.Donhang = donhang;
            ViewBag.Addresses = await _context.Addresses.Where(x => x.AccountId == Id).ToArrayAsync();
            ViewBag.AvatarPath = "/contents/Images/User/" + account.Avartar; // Đường dẫn ảnh đại diện
            //ViewBag.AvatarPath = "/contents/Images/Admin/" + account.Avartar; // Đường dẫn ảnh đại diện

            return View(await _context.Accounts.Include(x => x.Addresses).FirstOrDefaultAsync(x => x.AccountId == Id));
        }
		public async Task<IActionResult> HistoryOrder()
		{
			var Idclam = User.Claims.SingleOrDefault(c => c.Type == "Id");
			int Id = 0;
			if (Idclam != null)
			{ Id = Int32.Parse(Idclam.Value); }
			var donhang = await _context.Oders.Where(x => x.AccountId == Id)
				.Include(x => x.OderItems).ThenInclude(x => x.Product).Include(x => x.Account).ThenInclude(x => x.Addresses).ToListAsync();
			if (donhang == null)
			{
				return NotFound();
			}
			ViewBag.Donhang = donhang;
			ViewBag.Addresses = await _context.Addresses.Where(x => x.AccountId == Id).ToArrayAsync();


			return View(await _context.Accounts.Include(x => x.Addresses).FirstOrDefaultAsync(x => x.AccountId == Id));
		}
		public async Task<IActionResult> EditProfile()
        {
            var Idclam = User.Claims.SingleOrDefault(c => c.Type == "Id");
            int Id = 0;
            if (Idclam != null)
            { Id = Int32.Parse(Idclam.Value); }

            return View(await _context.Accounts.Include(x => x.Addresses).FirstOrDefaultAsync(x => x.AccountId == Id));
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(Account account, IFormFile fAvatar)
        {
            if (account.UserName?.Length < 6)
            {
                _notyfService.Error("Tài khoản không bé hơn 6 kí tự");
                return View(account);
            }
            if (account.Password?.Length < 6)
            {
                _notyfService.Error("Mật khẩu không bé hơn 6 kí tự");
                return View(account);
            }
            if (account.Phone?.Length != 10)
            {
                _notyfService.Error("Số điện thoại là 10 số");
                return View(account);
            }
            var khachang = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == account.AccountId);
            if (khachang == null)
            {
                return NotFound();
            }
            var ktemail = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId != account.AccountId
                && (x.Email == account.Email));
            if (ktemail != null)
            {
                _notyfService.Error("Email đã tồn tại trong hệ thống!");
                return View(khachang);
            }
            if (fAvatar != null)
            {
                string extennsion = Path.GetExtension(fAvatar.FileName);
                image = Utilities.ToUrlFriendly(khachang.UserName) + extennsion;
                khachang.Avartar = await Utilities.UploadFile(fAvatar, @"User", image.ToLower());
            }
         

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Lưu thông tin người dùng vào cookie xác thực
            List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, account.FullName),
                        new Claim("UserName" , account.UserName),
                        new Claim("Id" , account.AccountId.ToString()),
                        new Claim("Avartar", "~/contents/Images/User/" + khachang.Avartar) // Thêm đường dẫn đến ảnh đại diện vào claims
                    };
            //   Response.Cookies.Append("AnhDaiDien", "Images/GiaoVien/" + user.AnhDaiDien);
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            khachang.Email = account.Email;
            khachang.FullName = account.FullName;
            khachang.Gender = account.Gender;
            khachang.Phone = account.Phone;
            khachang.Birthday = account.Birthday;

            _notyfService.Success("Sửa thành công!");
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");

        }
        public async Task<IActionResult> CanOrderby(int id)
        {
            var oder = await _context.Oders.FirstOrDefaultAsync(x => x.OdersId == id);
            if (oder == null)
            {
                return NotFound();
            }
            oder.Status = 5;
            await _context.SaveChangesAsync();
            _notyfService.Success("Hủy đơn thành công");
            return RedirectToAction("HistoryOrder");
        }
        public async Task<IActionResult> ConfirmOrder(int id)
        {
            var order = await _context.Oders.FirstOrDefaultAsync(o => o.OdersId == id);
            if (order == null)
            {
                return NotFound();
            }
            if (order.Status == 3) // Giả sử trạng thái 3 là đang vận chuyển
            {
                order.Status = 4; // Trạng thái 4 là đã nhận
                await _context.SaveChangesAsync();
                _notyfService.Success("Đơn hàng đã được xác nhận nhận!");
            }
            else
            {
                _notyfService.Warning("Đơn hàng không thể xác nhận trong trạng thái này!");
            }
            return RedirectToAction("HistoryOrder");
        }
        public async Task<IActionResult> OrderDetail(int id)
        {
            var order = await _context.Oders.Include(x => x.OderItems)
                                            .ThenInclude(x => x.Product)
                                            .FirstOrDefaultAsync(o => o.OdersId == id);

            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }



    }
}
