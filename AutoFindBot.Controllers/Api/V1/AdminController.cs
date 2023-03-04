using Microsoft.AspNetCore.Mvc;

namespace AutoFindBot.Controllers.Api.V1
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        public AdminController()
        {
        }

        // [HttpPost("create-category")]
        // public async Task<IActionResult> CreateCategory(Category category)
        // {
        //     var result = await _context.Categories.AddAsync(category);
        //     return Ok(result.Entity.Id);
        // }
    }
}