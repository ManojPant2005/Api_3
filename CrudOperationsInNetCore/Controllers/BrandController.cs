using CrudOperationsInNetCore.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CrudOperationsInNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly BrandContext _dbContext;


        public BrandController(BrandContext dbContext)
        {
            _dbContext = dbContext;
        }
        [Route("GetBrands")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brand>>> GetBrands()
        {
            if (_dbContext.Brands == null)
            {
                return NotFound();
            }

            return await _dbContext.Brands.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Brand>> GetBrand(int id)
        {
            if (_dbContext.Brands == null)
            {
                return NotFound();
            }
            var brand = await _dbContext.Brands.FirstOrDefaultAsync(x=> x.ID == id);
            var ids = await _dbContext.Brands.Where(x => x.Name == "values").Select(x => x.ID).ToListAsync();
            if (brand == null)
            {
                return NotFound();
            }

            return brand;
        }

        [Route("InsertBrands")]
        [HttpPost]
        public async Task<ActionResult<Brand>> PostBrand(Brand brand)
        {
            _dbContext.Brands.Add(brand);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBrand), new { id = brand.ID }, brand);
        }

        [HttpPut]
        public async Task<ActionResult> PutBrand(int id, Brand brand)
        {
            if(id != brand.ID)
            {
                return BadRequest();
            }
         _dbContext.Entry(brand).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if(!BrandAvailable(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok();

        }
        private bool BrandAvailable(int id)
        {
            return (_dbContext.Brands?.Any(x => x.ID == id)).GetValueOrDefault();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            if(_dbContext.Brands ==  null)
            {
                return NotFound();
            }

            var brand = await _dbContext.Brands.FindAsync(id);
            if(brand == null)
            {
                return NotFound();
            }

            _dbContext.Brands.Remove(brand);

            await _dbContext.SaveChangesAsync();

            return Ok();
        }


    }
}
