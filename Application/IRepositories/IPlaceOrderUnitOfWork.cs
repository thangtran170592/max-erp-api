namespace Application.IRepositories
{
    public interface IPlaceOrderUnitOfWork : IDisposable
    {
        // IGenericRepository<Order> Orders { get; }
        // IGenericRepository<Product> Products { get; }
        Task<int> SaveChangesAsync();
    }
}