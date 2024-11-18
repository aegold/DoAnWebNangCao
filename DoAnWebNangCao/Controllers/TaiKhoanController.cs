using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.IO;
using System.Drawing.Printing;
using DoAnWebNangCao.Models;

namespace QuanLyTaiKhoan.Controllers
{
    public class TaiKhoanController : Controller
    {
        DanhMucModelDataContext _dataShopDataContext = new DanhMucModelDataContext("Data Source=aegold;Initial Catalog=MobileShopDB;Integrated Security=True;TrustServerCertificate=True");

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Edit()
        {
            return View();
        }
        public ActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetListUsers(int pageNumber, int pageSize)
        {
            Debug.WriteLine("Đã đến đây");

            // Truy vấn kết hợp giữa bảng Products và ProductCatalogues
            var listUsers = _dataShopDataContext.Users
                .Join(_dataShopDataContext.Roles,  // Thực hiện JOIN với bảng ProductCatalogues
                      us => us.RoleID,  // Trường CatalogueID trong bảng Products
                      rl => rl.RoleID,  // Trường CatalogueID trong bảng ProductCatalogues
                      (us, rl) => new
                      {
                          us.UserID,
                          us.Account,
                          us.Password,
                          RoleName = rl.RoleName,
                          us.Name,
                          us.PhoneNumber,
                          us.Address,
                          us.Email,
                      })
                .OrderBy(us => us.UserID)
                .Skip((pageNumber - 1) * pageSize)  // Thực hiện phân trang
                .Take(pageSize)  // Giới hạn số lượng sản phẩm theo pageSize
                .ToList();

            if (listUsers == null || listUsers.Count == 0)
            {
                Debug.WriteLine("Không có sản phẩm.");
            }

            // Lấy tổng số sản phẩm
            var tongSoDanhMuc = _dataShopDataContext.Users.Count();

            // Trả về dữ liệu dưới dạng JSON, bao gồm cả danh sách sản phẩm và tổng số sản phẩm
            return Json(new { listUsers, tongSoDanhMuc }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetUserById(string id)
        {
            var user = _dataShopDataContext.Users.FirstOrDefault(x => x.UserID == id);
            if (user == null)
            {
                return Json(new { success = false, message = "Tài khoản không tồn tại." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true, user = new { user.UserID, user.Account, user.Password, user.RoleID, user.Name ,user.PhoneNumber, user.Address, user.Email } }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SuaTaiKhoan(string ID, string Account, string Password, string Role, string Name, string Phone, string Address, string Email)
        {
            var userObj = _dataShopDataContext.Users.FirstOrDefault(x => x.UserID == ID);
            if (userObj == null)
            {
                return Json(new { success = false, message = "Tài khoản không tồn tại." });
            }

            // Update the user properties
            userObj.Account = Account;
            userObj.Password = Password;
            userObj.Name = Name;
            userObj.PhoneNumber = Phone;
            userObj.Address = Address;
            userObj.Email = Email;
            _dataShopDataContext.SubmitChanges();
            return Json(new { success = true, message = "Sửa tài khoản thành công!" });
        }

        [HttpPost]
        public JsonResult XoaTaiKhoan(string id)
        {
            var UserObj = _dataShopDataContext.Users.FirstOrDefault(x => x.UserID == id);
            if (UserObj == null)
            {
                return Json(new { success = false, message = "Không có tài khoản." });
            }

            _dataShopDataContext.Users.DeleteOnSubmit(UserObj);
            _dataShopDataContext.SubmitChanges();

            return Json(new { success = true, message = "Tài khoản đã được xóa thành công." });
        }
        [HttpPost]
        public JsonResult ThemTaiKhoan(string Account, string Password, string Role, string Name, string Phone, string Address, string Email)
        {
            try
            {
                var newUser = new User
                {
                    UserID = Guid.NewGuid().ToString(),
                    Account = Account,
                    Password = Password,
                    RoleID = (Role == "Admin") ? true : false, // Chuyển đổi "Admin" => true, "Customer" => false
                    Name = Name,
                    PhoneNumber = Phone,
                    Address = Address,
                    Email = Email
                };

                _dataShopDataContext.Users.InsertOnSubmit(newUser);
                _dataShopDataContext.SubmitChanges();

                return Json(new { success = true, message = "Thêm tài khoản thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }

        }
        //TÌM KIẾM
        public JsonResult SearchUsersByName(string name, int pageNumber, int pageSize)
        {
            var userList = _dataShopDataContext.Users
                .Where(us => us.Name.Contains(name))
                .Join(_dataShopDataContext.Roles,  // Thực hiện JOIN với bảng ProductCatalogues
                      us => us.RoleID,  // Trường CatalogueID trong bảng Products
                      rl => rl.RoleID,  // Trường CatalogueID trong bảng ProductCatalogues
                      (us, rl) => new
                      {
                          us.UserID,
                          us.Account,
                          us.Password,
                          RoleName = rl.RoleName,
                          us.Name,
                          us.PhoneNumber,
                          us.Address,
                          us.Email,
                      })
                .OrderBy(us => us.UserID)
                .Skip((pageNumber - 1) * pageSize)  // Thực hiện phân trang
                .Take(pageSize)  // Giới hạn số lượng sản phẩm theo pageSize
                .ToList();
            var totalUsers = _dataShopDataContext.Users
                .Where(us => us.Name.Contains(name))
                .Count();
            return Json(new { listUsers = userList, tongSoDanhMuc = totalUsers }, JsonRequestBehavior.AllowGet);
        }
    }
}