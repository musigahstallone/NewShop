using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewCatalog.Data;
using NewCatalog.Models;

namespace NewCatalog.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class CatalogController(CatalogContext context, IHostEnvironment environment) : ControllerBase
{
    private readonly CatalogContext _context = context;
    private readonly IHostEnvironment _environment = environment;

    // GET: api/Catalogs
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CatalogItem>>> GetCatalogItem()
    {
        return await _context.CatalogItem.ToListAsync();
    }

    // GET: api/Catalogs/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CatalogItem>> GetCatalogItem(int id)
    {
        var catalogItem = await _context.CatalogItem.FindAsync(id);

        if (catalogItem == null)
        {
            return NotFound();
        }

        return catalogItem;
    }

    // PUT: api/Catalogs/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCatalogItem(int id, CatalogItem catalogItem)
    {
        if (id != catalogItem.Id)
        {
            return BadRequest();
        }

        _context.Entry(catalogItem).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CatalogItemExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Catalogs
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<CatalogItem>> PostCatalogItem(CatalogItem catalogItem)
    {
        _context.CatalogItem.Add(catalogItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetCatalogItem", new { id = catalogItem.Id }, catalogItem);
    }

    // DELETE: api/Catalogs/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCatalogItem(int id)
    {
        var catalogItem = await _context.CatalogItem.FindAsync(id);
        if (catalogItem == null)
        {
            return NotFound();
        }

        _context.CatalogItem.Remove(catalogItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("items/type/all/brand/{catalogBrandId?}")]
    public async Task<ActionResult<CatalogItemsPage>> GetCatalogItems(int? catalogBrandId, int? before, int? after, int pageSize = 8)
    {
        var itemsOnPage = await _context.GetCatalogItemsCompiledAsync(catalogBrandId, before, after, pageSize);

        var (firstId, nextId) = itemsOnPage switch
        {
        [] => (0, 0),
        [var only] => (only.Id, only.Id),
        [var first, .., var last] => (first.Id, last.Id)
        };

        return new CatalogItemsPage(firstId, nextId, itemsOnPage.Count < pageSize, itemsOnPage.Take(pageSize));
    }

    // New endpoint for getting item image
    [HttpGet("items/{catalogItemId:int}/image")]
    public async Task<IActionResult> GetItemImage(int catalogItemId)
    {
        var item = await _context.CatalogItem.FindAsync(catalogItemId);

        if (item is null)
            return NotFound();

        var path = Path.Combine(_environment.ContentRootPath, "Images", item.PictureFileName);

        if (!System.IO.File.Exists(path))
            return NotFound();

        var fileContents = await System.IO.File.ReadAllBytesAsync(path);

        return File(fileContents, "image/jpeg");
    }


    // CRUD operations for CatalogBrands

    [HttpGet("brands")]
    public async Task<ActionResult<IEnumerable<CatalogBrand>>> GetCatalogBrands()
    {
        return await _context.CatalogBrand.ToListAsync();
    }

    [HttpGet("brands/{id}")]
    public async Task<ActionResult<CatalogBrand>> GetCatalogBrandById(int id)
    {
        var catalogBrand = await _context.CatalogBrand.FindAsync(id);

        if (catalogBrand == null)
        {
            return NotFound();
        }

        return catalogBrand;
    }

    [HttpPost("brands")]
    public async Task<ActionResult<CatalogBrand>> PostCatalogBrand(CatalogBrand catalogBrand)
    {
        _context.CatalogBrand.Add(catalogBrand);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCatalogBrandById), new { id = catalogBrand.Id }, catalogBrand);
    }

    [HttpPut("brands/{id}")]
    public async Task<IActionResult> PutCatalogBrand(int id, CatalogBrand catalogBrand)
    {
        if (id != catalogBrand.Id)
        {
            return BadRequest();
        }

        _context.Entry(catalogBrand).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CatalogBrandExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("brands/{id}")]
    public async Task<IActionResult> DeleteCatalogBrand(int id)
    {
        var catalogBrand = await _context.CatalogBrand.FindAsync(id);
        if (catalogBrand == null)
        {
            return NotFound();
        }

        _context.CatalogBrand.Remove(catalogBrand);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // CRUD operations for CatalogTypes

    [HttpGet("types")]
    public async Task<ActionResult<IEnumerable<CatalogType>>> GetCatalogTypes()
    {
        return await _context.CatalogType.ToListAsync();
    }

    [HttpGet("types/{id}")]
    public async Task<ActionResult<CatalogType>> GetCatalogTypeById(int id)
    {
        var catalogType = await _context.CatalogType.FindAsync(id);

        if (catalogType == null)
        {
            return NotFound();
        }

        return catalogType;
    }

    [HttpPost("types")]
    public async Task<ActionResult<CatalogType>> PostCatalogType(CatalogType catalogType)
    {
        _context.CatalogType.Add(catalogType);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCatalogTypeById), new { id = catalogType.Id }, catalogType);
    }

    [HttpPut("types/{id}")]
    public async Task<IActionResult> PutCatalogType(int id, CatalogType catalogType)
    {
        if (id != catalogType.Id)
        {
            return BadRequest();
        }

        _context.Entry(catalogType).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CatalogTypeExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("types/{id}")]
    public async Task<IActionResult> DeleteCatalogType(int id)
    {
        var catalogType = await _context.CatalogType.FindAsync(id);
        if (catalogType == null)
        {
            return NotFound();
        }

        _context.CatalogType.Remove(catalogType);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CatalogBrandExists(int id)
    {
        return _context.CatalogBrand.Any(e => e.Id == id);
    }

    private bool CatalogTypeExists(int id)
    {
        return _context.CatalogType.Any(e => e.Id == id);
    }

    private bool CatalogItemExists(int id)
    {
        return _context.CatalogItem.Any(e => e.Id == id);
    }
}
