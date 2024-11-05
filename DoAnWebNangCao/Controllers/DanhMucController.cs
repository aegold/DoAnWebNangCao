using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoAnWebNangCao.Models;
using System.Diagnostics;

namespace DoAnWebNangCao.Controllers
{
    public class DanhMucController : Controller
    {
        DanhMucModelDataContext _danhMucContext = new DanhMucModelDataContext("Data Source=aegold;Initial Catalog=MobileShopDB;Integrated Security=True;TrustServerCertificate=True");
        // GET: DanhMucList
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult GetListDanhMuc(int pageNumber, int pageSize)
        {
            Debug.WriteLine(pageNumber);
            var listDanhMuc = _danhMucContext.ProductCatalogues
                             .OrderBy(sp => sp.CatalogueID)
                             .Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();
            if (listDanhMuc == null)
            {   
                Debug.WriteLine("loi");
            }
            var tongSoDanhMuc = _danhMucContext.ProductCatalogues.Count();
            return Json(new { listDanhMuc, tongSoDanhMuc }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TaoDanhMuc(string tenDanhMuc)
        {
            try
            {
                var newDanhMuc = new ProductCatalogue();
                newDanhMuc.CatalogueName = tenDanhMuc;
                _danhMucContext.ProductCatalogues.InsertOnSubmit(newDanhMuc);
                _danhMucContext.SubmitChanges();
                return Json(new { success = true, message = "Danh mục thêm thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
            
        }

        public JsonResult GetObjectByID(int id)
        {
            Debug.WriteLine(id);
            var catalogueObj = _danhMucContext.ProductCatalogues.FirstOrDefault(x => x.CatalogueID == id);
            if (catalogueObj == null)
            {
                return Json(new { success = false, message = "Danh mục không tồn tại." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, danhMuc = catalogueObj }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult SuaDanhMuc(int id,string ten)
        {
            var catalogueObj = _danhMucContext.ProductCatalogues.FirstOrDefault(x => x.CatalogueID == id);
            if(catalogueObj == null)
            {
                return Json(new { success = false, message = "Danh mục không tồn tại." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                catalogueObj.CatalogueName = ten;
                _danhMucContext.SubmitChanges();
                return Json(new { success = true, message = "Danh mục sửa thành công!" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult XoaDanhMuc(int id)
        {
            var catalogueObj = _danhMucContext.ProductCatalogues.FirstOrDefault(x => x.CatalogueID == id);
            if (catalogueObj == null)
            {
                return Json(new { success = false, message = "Danh mục không tồn tại." });
            }

            _danhMucContext.ProductCatalogues.DeleteOnSubmit(catalogueObj);
            _danhMucContext.SubmitChanges();

            return Json(new { success = true, message = "Danh mục đã được xóa thành công." });
        }
    }
}