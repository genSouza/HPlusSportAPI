using HPlusSportAPI.Infrastructure;
using HPlusSportAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HPlusSportAPI.Services
{
    public class ProductService
    {

        private readonly ShopContext _context;

        public ProductService(ShopContext context)
        {
            _context = context;

            _context.Database.EnsureCreated();
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync(ProductQueryParameters queryParameters)
        {
            IQueryable<Product> products = _context.Products;

            //price filter
            if (queryParameters.MinPrice != null && queryParameters.MaxPrice != null)
            {
                products = products.Where(p => p.Price >= queryParameters.MinPrice.Value &&
                                          p.Price <= queryParameters.MaxPrice.Value);
            }

            //sku filter
            if (!string.IsNullOrEmpty(queryParameters.Sku))
            {
                products = products.Where(p => p.Sku == queryParameters.Sku);
            }

            //name filter
            if (!string.IsNullOrEmpty(queryParameters.Name))
            {
                products = products.Where(p => p.Name.Contains(queryParameters.Name, StringComparison.InvariantCultureIgnoreCase));
            }

            //order filter
            if (!string.IsNullOrEmpty(queryParameters.SortBy))
            {
                if (typeof(Product).GetProperty(queryParameters.SortBy) != null)
                {
                    products = products.OrderByCustom(queryParameters.SortBy, queryParameters.SortOrder);
                }
            }

            //pagination
            products = products
                      .Skip(queryParameters.Size * (queryParameters.Page - 1))
                      .Take(queryParameters.Size);
            return await products.ToListAsync();
        }

        public async Task<Product> FindProductByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> AddProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProductAsync(int id, Product product)
        {
            try
            {
                var _product = await FindProductByIdAsync(id);

                if (_product != null)
                {
                    _context.Entry(_product).State = EntityState.Detached;
                }

                _context.Entry(product).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return product;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<Product> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return product;
        }
    }
}
