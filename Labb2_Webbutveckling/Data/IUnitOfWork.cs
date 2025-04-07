using Labb2_Webbutveckling.Data;

public interface IUnitOfWork : IDisposable
{
    IProductRepository ProductRepository { get; }
    ICustomerRepository CustomerRepository { get; }
    IOrderRepository OrderRepository { get; }
    IAdminRepository AdminRepository { get; }
    Task SaveChangesAsync();
}