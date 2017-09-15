using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WingtipToys.Logic;
using WingtipToys.Model;

namespace WingtipToys.Controllers
{
    public class ProductsController : Controller
    {
        private wingtiptoysEntities db = new wingtiptoysEntities();

        // GET: /Products/
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Categories);
            return View(products.ToList());
        }

        public ActionResult AddToCard(int? id){
            int productId;
            if (!String.IsNullOrEmpty(id.ToString()) && int.TryParse(id.ToString(), out productId))
            {
                using (ShoppingCartActions usersShoppingCart = new ShoppingCartActions())
                {
                    usersShoppingCart.AddToCart(Convert.ToInt16(id.ToString()));
                }

            }
            else
            {
                throw new Exception("ERROR : It is illegal to load AddToCart.aspx without setting a ProductId.");
            }
            return RedirectToAction("Index");
        }

        // GET: /Products/Details/5
        [Authorize(Roles = "admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // GET: /Products/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: /Products/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,ProductName,Description,ImagePath,UnitPrice,CategoryID")] Products products, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {

                //products.ImagePath = ImageFile != null ? ImageFile.FileName : null;
                db.Products.Add(products);
                db.SaveChanges();


                if (ImageFile != null)
                {
                    db.Entry(products).GetDatabaseValues();
                    int id = products.ProductID;
                    string fileName = id.ToString() + "_" + ImageFile.FileName;
                    string relativePath = @"~\Content\Upload\" + fileName;
                    ImageFile.SaveAs(Server.MapPath(relativePath));

                    Products productsImg = db.Products.Find(id);
                    productsImg.ImagePath = fileName;
                    db.Entry(productsImg).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", products.CategoryID);
            return View(products);
        }

        // GET: /Products/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", products.CategoryID);
            return View(products);
        }

        // POST: /Products/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,ProductName,Description,ImagePath,UnitPrice,CategoryID")] Products products, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {

                if (ImageFile != null)
                {
                    int id = products.ProductID;
                    string fileName = id.ToString() + "_" + ImageFile.FileName;
                    string relativePath = @"~\Content\Upload\" + fileName;
                    ImageFile.SaveAs(Server.MapPath(relativePath));
                    products.ImagePath = fileName;
                }
                else
                {
                    products.ImagePath = null;
                }

                db.Entry(products).State = EntityState.Modified;
                db.SaveChanges();




                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", products.CategoryID);
            return View(products);
        }

        // GET: /Products/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // POST: /Products/Delete/5
        [Authorize(Roles = "admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Products products = db.Products.Find(id);
            db.Products.Remove(products);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
