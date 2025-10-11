using Infrastructure.Data;

namespace Infrastructure.UnitOfWorks
{
    public class PlaceOrderUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public PlaceOrderUnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        // public IGenericRepository<Order> Orders { get; } = new GenericRepository<Order>(dbContext);
        // public IGenericRepository<Product> Products { get; } = new GenericRepository<Product>(dbContext);


        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}