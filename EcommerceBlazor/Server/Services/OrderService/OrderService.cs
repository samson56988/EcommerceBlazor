using EcommerceBlazor.Server.Services.AuthService;
using EcommerceBlazor.Server.Services.CartService;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EcommerceBlazor.Server.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly DataContext _dataContext;
        private readonly ICartService _carservice;
        private readonly IAuthService _authservice;
        public OrderService(DataContext dataContext,ICartService cartService,IAuthService authService)
        {

            _dataContext = dataContext;
            _carservice = cartService;
            _authservice = authService;
        }

        public async Task<ServiceResponse<List<OrderOverviewResponse>>> GetOrders()
        {
            var response =  new ServiceResponse<List<OrderOverviewResponse>>();
            var orders = await _dataContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == _authservice.GetUserID())
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var orderResponse = new List<OrderOverviewResponse>();
            orders.ForEach(o => orderResponse.Add(new OrderOverviewResponse
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalPrice,
                product = o.OrderItems.Count > 1?
                $"{o.OrderItems.First().Product.Title} and" +
                $"{o.OrderItems.Count -1} more ....":
                o.OrderItems.First().Product.Title,
                ImageUrl = o.OrderItems.First().Product.ImageUrl
            }));

            response.Data = orderResponse;

            return response;
        }

        public async Task<ServiceResponse<bool>> PlaceOrder()
        {
            var products = (await _carservice.GetDbCartProducts()).Data;
            decimal totalPrice = 0;
            products.ForEach(products => totalPrice += products.Price * products.Quantity);

            var orderItems = new List<OrderItem>();

            products.ForEach(products => orderItems.Add(new OrderItem
            {
                ProductId = products.ProductId,
                ProductTypeId = products.ProductTypeId,
                Quantity = products.Quantity,
                TotalPrice = products.Price * products.Quantity
            }));

            var order = new Order
            {
                UserId = _authservice.GetUserID(),
                OrderDate = DateTime.Now,
                TotalPrice = totalPrice,
                OrderItems = orderItems
            };

            _dataContext.Orders.Add(order);

            _dataContext.CartItems.RemoveRange(_dataContext.CartItems
                .Where(ci => ci.UserId ==_authservice.GetUserID()));

            await _dataContext.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true };

        }
    }
}
