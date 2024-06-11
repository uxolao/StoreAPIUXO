using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreAPIUXO.Data;
using StoreAPIUXO.Models;

namespace StoreAPIUXO.Controllers;
// [Authorize]
// [Authorize(Roles = UserRolesModel.Admin)]
[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    // create object of ApplicationDbContext
    private readonly ApplicationDbContext _context;
    // create Constuctor to get ApplicationDbContext
    public CategoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    //test writing connecting database function
    // GET: /api/Category/testconnectdb
    [HttpGet("testconnectdb")]
    public void TestConnection()
    {
        if(_context.Database.CanConnect())
        {
            Response.WriteAsync("Connection Success");
        }
        else
        {
            Response.WriteAsync("Connection Fail");
        }
    }

    //function to query data from Category
    [HttpGet]
    public ActionResult GetCategories()
    {
        var categories = _context.category.ToList();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public ActionResult<category> GetCategories(int id)
    {
        var category = _context.category.Find(id);
        if(category == null)
        {
            return NotFound();
        }
        return Ok(category);
    }

    [HttpPost]
    public ActionResult<category> AddCategory([FromBody] category category)
    {
        _context.category.Add(category);
        _context.SaveChanges();
        return Ok(category);
    }

     [HttpPut]
    public ActionResult<category> UpdateCategory(int id, [FromBody] category category)
    {
        var cat = _context.category.Find(id);
        if(cat == null)
        {
            return NotFound();
        }
        cat.categoryname = category.categoryname;
        // cat.categorystatus = category.categorystatus;
        _context.SaveChanges();
        return Ok(cat);
    }

    [HttpDelete("{id}")]
    public ActionResult<category> DeleteCategory(int id)
    {
        var cat = _context.category.Find(id);
        if(cat == null)
        {
            return NotFound();
        }
        _context.category.Remove(cat);
        _context.SaveChanges();
        return Ok(cat);
    }
}