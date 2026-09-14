using System;
using ApiEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiEcommerce.Repository;

public class CategoryRepositoy : ICategoryRepository
{

    private readonly AplicationDbContext _db;

    public CategoryRepositoy(AplicationDbContext db)
    {
        _db=db;
    }

    public bool Exists(int id)
    {
        return _db.Categories.Any(category => category.Id == id);
    }

    public bool Exists(string name)
    {
        return _db.Categories.Any(category => category.Name.ToLower().Trim() == name.Trim());
    }

    public bool CreateCategory(Category category)
    {
        category.CreatedAt = DateTime.Now;
        _db.Categories.Add(category);
        return Save();
    }

    public bool DeleteCategory(Category catetory)
    {
        _db.Categories.Remove(catetory);
        return Save();
    }
    public ICollection<Category> GetCategories()
    {
        return _db.Categories.OrderBy(category => category.Name).ToList();
    }

    public Category? GetCategory(int id)
    {
        return _db.Categories.FirstOrDefault(category => category.Id == id);
    }

    public bool Save()
    {
        return _db.SaveChanges() >= 0;
    }

    public bool UpdateCategory(Category catetory)
    {
        catetory.CreatedAt = DateTime.Now;
        _db.Categories.Update(catetory);
        return Save();
    }
}
