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
        return _db.Catetories.Any(category => category.Id == id);
    }

    public bool Exists(string name)
    {
        return _db.Catetories.Any(category => category.Name.ToLower().Trim() == name.Trim());
    }

    public bool CreateCategory(Category category)
    {
        category.CreatedAt = DateTime.Now;
        _db.Catetories.Add(category);
        return Save();
    }

    public bool DeleteCategory(Category catetory)
    {
        _db.Catetories.Remove(catetory);
        return Save();
    }
    public ICollection<Category> GetCatetories()
    {
        return _db.Catetories.OrderBy(category => category.Name).ToList();
    }

    public Category GetCategory(int id)
    {
        return _db.Catetories.FirstOrDefault(category => category.Id == id) ?? throw new InvalidOperationException($"No se ha encontrado categoría con id: {id}");
    }

    public bool Save()
    {
        return _db.SaveChanges() >= 0;
    }

    public bool UpdateCategory(Category catetory)
    {
        catetory.CreatedAt = DateTime.Now;
        _db.Catetories.Update(catetory);
        return Save();
    }
}
