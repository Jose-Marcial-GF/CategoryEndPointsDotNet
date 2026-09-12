using System;

namespace ApiEcommerce.Repository.IRepository;

public interface ICategoryRepository
{
 ICollection<Category> GetCatetories();
 Category GetCategory(int id);
 bool Exists(int id);

 bool Exists(String name);

 bool CreateCategory(Category catetory);
 bool UpdateCategory(Category catetory);
 bool DeleteCategory(Category catetory);
 bool Save();


}
