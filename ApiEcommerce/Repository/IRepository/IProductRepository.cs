using System;
using ApiEcommerce.Model;

namespace ApiEcommerce.Repository.IRepository;

public interface IProductRepository
{

    ICollection<Product> GetProducts();
    ICollection<Product> GetProductsForCategory(int id);

    ICollection<Product> SearchProducts(string searchTerm);
    Product? GetProduct( int id);

    bool BuyProduct(string name, int amount); 
    bool Exists(int id);
    bool Exists(string name);

    bool CreateProduct(Product product);
    bool UpdateProduct(Product product);
    bool DeleteProduct(Product product);

    bool Save();





}
