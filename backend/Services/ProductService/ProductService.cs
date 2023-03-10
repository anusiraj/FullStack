namespace NetCoreDemo.Services;

using NetCoreDemo.Models;
using NetCoreDemo.DTOs;
using NetCoreDemo.Db;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using NetCoreDemo.Repositories;

public class ProductService : CrudService<Product, ProductDTO>, IProductService
{
    public ProductService(AppDbContext dbContext, IProductRepo repo) : base(dbContext, repo)
    {
    }

    public async override Task<Product?> GetAsync(int id)
    {
          var product = await base.GetAsync(id);
          if (product is null)
          {
              return null;
          }
          await _dbContext.Entry(product).Reference(s => s.Category).LoadAsync();
          return product;
    }

  public async Task<ICollection<Product>> GetByNameAsync(string name, string keyword)
    {
        var query = _dbContext.Products.Where(c => true);

        if (name is not null)
        {
          query = query.Where(c => c.Name.ToLower() == name.ToLower());
        }
        if (keyword is not null && !string.IsNullOrEmpty(keyword))
        {
          query = query.Where(c => c.Name.ToLower().Contains(keyword.ToLower()));
        }
        return await query.OrderByDescending(c => c.CreatedAt).Include(p => p.Category).ToListAsync();
    }

    public async Task<ICollection<Product>> GetByPriceAsync(double price)
    {
        var query = _dbContext.Products.Where(c => true);
        query = query.Where(c => c.Price == price);

        return await query.OrderByDescending(c => c.CreatedAt).Include(p => p.Category).ToListAsync();
    }

    public async Task<ICollection<Product>> GetByPriceRangeAsync(double min, double max)
    {
        var query = _dbContext.Products.Where(c => true);
        query = query.Where(c => c.Price >= min && c.Price <= max);

        return await query.OrderByDescending(c => c.CreatedAt).Include(p => p.Category).ToListAsync();
    }

    public async Task<ICollection<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        var query = _dbContext.Products.Where(p => true);
        query = query.Where(p => p.CategoryId == categoryId);

        return await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
    }
}