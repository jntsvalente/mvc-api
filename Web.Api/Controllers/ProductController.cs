using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Api.Extensions;
using Web.Api.Interfaces;
using Web.Api.Models;
using Web.Api.ViewModels;

namespace Web.Api.Controllers;
public class ProductController : ControllerBase
{
    [Authorize(Roles = Role.Member)]
    [HttpGet("v1/products")]
    public async Task<IActionResult> GetAsync(
        [FromServices] IApiDataContext context,
        CancellationToken cancellationToken)
    {
        try 
        {
            var products = await context.Products.ToListAsync(cancellationToken);
            return Ok(new ResultViewModel<List<Product>>(products));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<List<Product>>("ER23D - Internal server failure"));
        }        
    }

    [Authorize(Roles = Role.Member)]
    [HttpGet("v1/products/{productId:int}")]
    public async Task<IActionResult> GetByIdAsync(
        [FromRoute] int productId,
        [FromServices] IApiDataContext context,
        CancellationToken cancellationToken)
    {
        try
        {
            var product = await context.Products.SingleOrDefaultAsync(x => x.Id == productId, cancellationToken);
            if (product == null)
                return NotFound(new ResultViewModel<Product>("Product not found"));

            return Ok(new ResultViewModel<Product>(product));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<Product>("F5R8G - Internal server failure"));
        }
    }

    [Authorize(Roles = Role.Member)]
    [HttpPost("v1/products")]
    public async Task<IActionResult> PostAsync(
        [FromBody] ProductEditorViewModel model,
        [FromServices] IApiDataContext context,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<Product>(ModelState.GetErrors()));
        try
        {
            var product = new Product
            {
                Name = model.Name,
                Price = model.Price,
                Description = model.Description
            };
            await context.Products.AddAsync(product, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return Created($"v1/products/{product.Id}", new ResultViewModel<Product>(product));
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new ResultViewModel<Product>("9958R - It was not possible to create a product"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<Product>("00HY1 - Internal server failure"));
        }
    }

    [Authorize(Roles = Role.Member)]
    [HttpPut("v1/products/{productId:int}")]
    public async Task<IActionResult> PutAsync(
        [FromRoute] int productId,
        [FromBody] ProductEditorViewModel model,
        [FromServices] IApiDataContext context,
        CancellationToken cancellationToken)
    {
        try
        {
            var product = await context.Products.SingleOrDefaultAsync(x => x.Id == productId, cancellationToken);

            if (product == null)
                return NotFound(new ResultViewModel<Product>("Product not found"));

            product.Name = model.Name;
            product.Price = model.Price;
            product.Description = model.Description;

            context.Products.Update(product);
            await context.SaveChangesAsync(cancellationToken);

            return Ok(new ResultViewModel<Product>(product));
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new ResultViewModel<Product>("F522S - It was not possible to update the product"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<Product>("Y7Y7U - Internal server failure"));
        }
    }

    [Authorize(Roles = Role.Admin)]
    [HttpDelete("v1/products/{productId:int}")]
    public async Task<IActionResult> DeleteAsync(
        [FromRoute] int productId,
        [FromServices] IApiDataContext context,
        CancellationToken cancellationToken)
    {
        try
        {
            var product = await context.Products.SingleOrDefaultAsync(x => x.Id == productId, cancellationToken);

            if (product == null)
                return NotFound(new ResultViewModel<Product>("Product not found"));

            context.Products.Remove(product);
            await context.SaveChangesAsync(cancellationToken);

            return Ok(new ResultViewModel<Product>(product));
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new ResultViewModel<Product>("NNM15 - It was not possible to delete the product"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<Product>("TI4U5 - Internal server failure"));
        }
    }
}