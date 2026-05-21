using Labb2_Webbutveckling.Configuration;
using Labb2_Webbutveckling.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Labb2_Webbutveckling.Service_Backend
{
    public class OrderConfirmationEmailService : IOrderConfirmationEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<OrderConfirmationEmailService> _logger;
        public OrderConfirmationEmailService(
            IOptions<EmailSettings> settings,
            ILogger<OrderConfirmationEmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }
        public async Task SendOrderConfirmationAsync(Order order, Customer customer)
        {
            if (!_settings.Enabled)
            {
                _logger.LogInformation(
                    "Order {OrderId}: email disabled (Email:Enabled = false).",
                    order.OrderId);
                return;
            }
            if (string.IsNullOrWhiteSpace(customer.Email))
            {
                _logger.LogWarning("Order {OrderId}: customer has no email.", order.OrderId);
                return;
            }
            var message = BuildMessage(order, customer);
            using var client = new SmtpClient();
            await client.ConnectAsync(
                _settings.Host,
                _settings.Port,
                _settings.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
            if (!string.IsNullOrWhiteSpace(_settings.UserName))
            {
                await client.AuthenticateAsync(_settings.UserName, _settings.Password);
            }
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            _logger.LogInformation(
                "Order confirmation sent for order {OrderId} to {Email}.",
                order.OrderId,
                customer.Email);
        }
        private MimeMessage BuildMessage(Order order, Customer customer)
        {
            var total = order.OrderItems.Sum(i => i.Price * i.Quantity);
            var customerName = $"{customer.FirstName} {customer.LastName}";

            var body = new System.Text.StringBuilder();
            body.AppendLine($"Hej {customerName},");
            body.AppendLine();
            body.AppendLine("Tack för din beställning!");
            body.AppendLine($"Ordernummer: {order.OrderId}");
            body.AppendLine($"Datum: {order.OrderDate:yyyy-MM-dd}");
            body.AppendLine($"Leveransadress: {customer.Street}, {customer.ZipCode} {customer.City}");
            body.AppendLine();
            body.AppendLine("Produkter:");

            foreach (var item in order.OrderItems)
            {
                var name = item.Product?.Name ?? $"Produkt #{item.ProductNumber}";
                var lineTotal = item.Price * item.Quantity;
                body.AppendLine($"- {name}");
                body.AppendLine($"  {item.Quantity} st x {item.Price:N2} kr = {lineTotal:N2} kr");
            }

            body.AppendLine();
            body.AppendLine($"Totalt: {total:N2} kr");
            body.AppendLine();
            body.AppendLine("Med vänliga hälsningar,");
            body.AppendLine(_settings.FromName);

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
            email.To.Add(MailboxAddress.Parse(customer.Email));
            email.Subject = $"Orderbekräftelse #{order.OrderId}";
            email.Body = new TextPart("plain") { Text = body.ToString() };

            return email;
        }
    }
}
