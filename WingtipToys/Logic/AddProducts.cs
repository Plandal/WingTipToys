using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WingtipToys.Model;

namespace WingtipToys.Logic
{
  public class AddProducts
  {
    public bool AddProduct(string ProductName, string ProductDesc, string ProductPrice, string ProductCategory, string ProductImagePath)
    {
      var myProduct = new Products();
      myProduct.ProductName = ProductName;
      myProduct.Description = ProductDesc;
      myProduct.UnitPrice = Convert.ToDouble(ProductPrice);
      myProduct.ImagePath = ProductImagePath;
      myProduct.CategoryID = Convert.ToInt32(ProductCategory);

      using (wingtiptoysEntities _db = new wingtiptoysEntities())
      {
        // Add product to DB.
        _db.Products.Add(myProduct);
        _db.SaveChanges();
      }
      // Success.
      return true;
    }
  }
}