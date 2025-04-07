namespace Labb2_Webbutveckling.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ECommerceDbContext _context;
        public IProductRepository ProductRepository { get; }
        public ICustomerRepository CustomerRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public IAdminRepository AdminRepository { get; }

        public UnitOfWork(ECommerceDbContext context)
        {
            _context = context;
            ProductRepository = new ProductRepository(_context);
            CustomerRepository = new CustomerRepository(_context);
            OrderRepository = new OrderRepository(_context);
            AdminRepository = new AdminRepository(_context);
        }


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}