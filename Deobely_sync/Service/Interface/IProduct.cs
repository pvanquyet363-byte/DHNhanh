using Deobely_sync.Model;

namespace Deobely_sync.Service.Interface
{
    public interface IProduct
    {
        Task<List<ProductModel>> GetProduct();
        Task<int> CreateProduct(ProductInsertModel model);
    }
}
