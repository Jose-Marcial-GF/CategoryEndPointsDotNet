using System;
using ApiEcommerce.Model;
using ApiEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ApiEcommerce.Repository;

public class ProductRepository : IProductRepository
{

    private readonly ApplicationDbContext _db;

    public ProductRepository(ApplicationDbContext db)
    {
        _db = db;
        
    }
    public bool BuyProduct(string name, int amount)
    {
        if (string.IsNullOrWhiteSpace(name) || amount <= 0)
        {
            return false;
        }

        Product? product = _db.Products.FirstOrDefault(product => product.Name.ToLower().Trim() == name.ToLower().Trim());
        if(product == null || product.Stock < amount)
        {
           return false; 
        }

        product.Stock -= amount;
        _db.Products.Update(product);
        return Save();
    }

    public bool CreateProduct(Product product)
    {
        if (product == null)
        {
            return false;
        }
        product.CreationDate  = DateTime.Now;
        _db.Products.Add(product);
        return Save();
    }


    public bool DeleteProduct(Product product)
    {
        if (product == null)
        {
            return false;
        }
        _db.Products.Remove(product);
        return Save();
            
    }

    public bool Exists(int id)
    {
        if( id < 0)
        {
            return false;
        }

        return _db.Products.Any(p => p.Id == id);
    }

    public bool Exists(string name)
    {
        if( string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        return _db.Products.Any(p => p.Name.ToLower().Trim() == name.ToLower().Trim());
    }

    public Product? GetProduct(int id)
    {
        if( id <= 0)
        {
            return null;
        }
        return _db.Products.Include(product => product.Category).FirstOrDefault(product => product.Id == id);
    }

    public ICollection<Product> GetProducts()
    {
        return _db.Products.Include(product => product.Category).OrderBy(product => product.Name).ToList();

    }

    public ICollection<Product> GetProductsForCategory(int categoryId)
    {
        if (categoryId <= 0)
        {
            return new List<Product>();
        }
        return _db.Products.Include(product => product.Category).Where(product => product.CategoryId == categoryId).OrderBy(product => product.Name).ToList<Product>();

    }

    public bool Save()
    {
        return _db.SaveChanges() >= 0;
    }

    public ICollection<Product> SearchProducts(string searchTerm)
    {
        IQueryable<Product> query = _db.Products;
        if(!string.IsNullOrEmpty(searchTerm))
        {
            query  = query.Where(product => product.Name.ToLower().Trim().Contains(searchTerm.ToLower().Trim()) 
            || product.Description.ToLower().Trim().Contains(searchTerm.ToLower().Trim()));
        }
        return query.Include(product => product.Category).OrderBy(p => p.Name).ToList();
    }

    public bool UpdateProduct(Product product)
    {
        if(product == null)
        {
            return false;
        }
         _db.Products.Update(product);
        return Save();
    }
}
