using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreAPIUXO.Data;
using StoreAPIUXO.Models;

namespace StoreAPIUXO.Controllers;
// [Authorize]
// [Authorize(Roles = UserRolesModel.Admin)]
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
// create object of ApplicationDbContext
    private readonly ApplicationDbContext _context;
    // create Constuctor to get ApplicationDbContext
    public ProductController(ApplicationDbContext context)
    {
        _context = context;
    }

    //function to query data from product
    [HttpGet]
    public ActionResult GetProducts()
    {
        var products = _context.product.ToList();
        return Ok(products);
    }

    [HttpGet("{productid}")]
    public ActionResult<product> GetProducts(int id)
    {
        var product = _context.product.Find(id);
        if(product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    [HttpPost]
    public ActionResult<product> AddProduct([FromBody] product product)
    {
        _context.product.Add(product);
        _context.SaveChanges();
        return Ok(product);
    }

     [HttpPut]
    public ActionResult<product> UpdateProduct(int id, [FromBody] product product)
    {
        var pro = _context.product.Find(id);
        if(pro == null)
        {
            return NotFound();
        }
        pro.productname = product.productname;
        // cat.productstatus = product.productstatus;
        _context.SaveChanges();
        return Ok(pro);
    }

    [HttpDelete("{productid}")]
    public ActionResult<product> DeleteProduct(int id)
    {
        var pro = _context.product.Find(id);
        if(pro == null)
        {
            return NotFound();
        }
        _context.product.Remove(pro);
        _context.SaveChanges();
        return Ok(pro);
    }

}