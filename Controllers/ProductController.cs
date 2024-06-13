using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using StoreAPIUXO.Data;
using StoreAPIUXO.Models;

namespace StoreAPIUXO.Controllers;

[Authorize] // ต้องเข้าสู่ระบบก่อนเข้าถึง Controller นี้
// [Authorize(Roles = UserRolesModel.Admin)] // ต้องเข้าสู่ระบบด้วยบทบาท Admin เท่านั้น
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    // สร้าง Object ของ ApplicationDbContext
    private readonly ApplicationDbContext _context;

    private readonly IWebHostEnvironment _env;
    public ProductController(
        ApplicationDbContext context,
        IWebHostEnvironment env
    )
    {
        _context = context;
        _env = env;
    }

    // ฟังก์ชันสำหรับอ่านข้อมูลจาก product
    // GET: /api/Product
    [HttpGet]
    public ActionResult GetProducts(
        [FromQuery] int page=1, 
        [FromQuery] int limit=100,
        [FromQuery] string? searchQuery=null,
        [FromQuery] int? selectedCategory=null
    )
    {
        int skip = (page - 1) * limit;

        // var products = _context.products.ToList();
        // อ่านข้อมูลจากตาราง products join กับ categories
        var query = _context.product.Join(
            _context.category,
            p => p.categoryid,
            c => c.categoryid,
            (p, c) => new
            {
                p.productid,
                p.productname,
                p.unitprice,
                p.unitinstock,
                p.productpicture,
                p.createddate,
                p.modifieddate,
                p.categoryid,
                c.categoryname
            }
        );

        // กรณีมีการค้นหาข้อมูล
        if(!string.IsNullOrEmpty(searchQuery))
        {
            query = query.Where(p => EF.Functions.ILike(p.productname!, $"%{searchQuery}%"));
        }

        // กรณีมีการค้นหาข้อมูลตาม category
        if(selectedCategory.HasValue){
            query = query.Where(p => p.categoryid == selectedCategory.Value);
        }

        // นับจำนวนข้อมูลทั้งหมด
        var totalRecords = query.Count();

        var products = query
                    .OrderByDescending(p => p.productid)
                    .Skip(skip)
                    .Take(limit)
                    .ToList();

        // ส่งข้อมูลกลับไปให้ผู้ใช้งาน
        return Ok(new {
            Total = totalRecords,
            Products = products
        });
    }

    // ฟังก์ชันสำหรับอ่านข้อมูลจาก product ตาม id
    // GET: /api/Product/1
    [HttpGet("{id}")]
    public ActionResult<product> GetProduct(int id)
    {
        // อ่านข้อมูลจากตาราง product ตาม id
        var product = _context.product.Find(id);

        // ถ้าไม่พบข้อมูลจะแสดงข้อความว่าไม่พบข้อมูล
        if (product == null)
        {
            return NotFound();
        }

        // ส่งข้อมูลกลับไปให้ผู้ใช้งาน
        return Ok(product);
    }

    // ฟังก์ชันสำหรับเพิ่มข้อมูล product
    // POST: /api/Product
    [HttpPost]
    public async Task<ActionResult<product>> AddProduct([FromForm] product product, IFormFile? image)
    {
        // เพิ่มข้อมูล product
        _context.product.Add(product);

        // in cate upload file
        if(image != null)
        {
            //
            string filename = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

            string uploadPath = Path.Combine(_env.WebRootPath,"uploads");
        

            if(!Directory.Exists(uploadPath)){

            }

            using(var fileStream = new FileStream(
                Path.Combine(uploadPath, filename),
                FileMode.Create)
            ){
                await image.CopyToAsync(fileStream);
            }

            product.productpicture = filename;
                        
        }
        else
        {
            product.productpicture = "noimg.jpg";
        }
        _context.SaveChanges();
        // บันทึกข้อมูลลงฐานข้อมูล
        _context.SaveChanges();

        // ส่งข้อมูลกลับไปให้ผู้ใช้งาน
        return Ok(product);
    }

    // ฟังก์ชันสำหรับการแก้ไขข้อมูล product
    // PUT: /api/Product/1
    [HttpPut("{id}")]
    public async Task<ActionResult<product>> UpdateProduct(int id, [FromForm] product product, IFormFile? image)
    {
        // ค้นหาข้อมูลจากตาราง Products ตาม ID
        var prod = _context.product.Find(id);

        // ถ้าไม่พบข้อมูลจะแสดงข้อความว่าไม่พบข้อมูล
        if (prod == null)
        {
            return NotFound();
        }

        // แก้ไขข้อมูลในตาราง Products
        prod.productname = product.productname;
        prod.unitprice = product.unitprice;
        prod.unitinstock = product.unitinstock;
        prod.categoryid = product.categoryid;
    
        _context.SaveChanges();

        // ส่งข้อมูลกลับไปให้ผู้ใช้งาน
        return Ok(prod);
    }

    // ฟังก์ชันการลบข้อมูล product
    // DELETE: /api/Product/1
    [HttpDelete("{id}")]
    public ActionResult<product> DeleteProduct(int id)
    {
        // ค้นหาข้อมูลจากตาราง Products ตาม ID
        var prod = _context.product.Find(id);

        // ถ้าไม่พบข้อมูลจะแสดงข้อความว่าไม่พบข้อมูล
        if (prod == null)
        {
            return NotFound();
        }

        if(prod.productpicture != "noimg.jpg"){
            string uploadPath = Path.Combine(_env.WebRootPath, "uploads");

            System.IO.File.Delete(Path.Combine(uploadPath, prod.productpicture!));
        }

        // ลบข้อมูลในตาราง Products
        _context.product.Remove(prod);
        _context.SaveChanges();

        // ส่งข้อมูลกลับไปให้ผู้ใช้งาน
        return Ok(prod);
    }

}