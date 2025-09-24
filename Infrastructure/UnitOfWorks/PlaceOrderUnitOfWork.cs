using Infrastructure.Data;

namespace Infrastructure.UnitOfWorks
{
    public class PlaceOrderUnitOfWork(ApplicationDbContext dbContext)
    {
        // private readonly ApplicationDbContext _dbContext = dbContext;
        // public IGenericRepository<Order> Orders { get; } = new GenericRepository<Order>(dbContext);
        // public IGenericRepository<Product> Products { get; } = new GenericRepository<Product>(dbContext);


        // public void Dispose()
        // {
        //     _dbContext.Dispose();
        // }

        // public async Task<int> SaveChangesAsync()
        // {
        //     return await _dbContext.SaveChangesAsync();
        // }
    }
}