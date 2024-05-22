using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Security.Claims;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{

    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrdersController(IOrderService orderService , IMapper mapper , IHttpContextAccessor httpContextAccessor)
        {
            _orderService = orderService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        [ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult<OrderToReturnDto>> CreateOrder(OrderDto orderDto)
        {
            var email = _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new ApiResponse(400, "Invalid token, email claim not found."));
            }
            var address =  _mapper.Map<AddressDto, Adsress>(orderDto.ShippingAddress);

           var order =  await _orderService.CreateOrderAsync(orderDto.BasketId,orderDto.DeliveryMethodId,address);

            if (order is null) return BadRequest(new ApiResponse(400));

            order.BuyerEmail = email;
            await _orderService.SaveOrderAsync(order);

            return Ok(_mapper.Map<Core.Entities.Order_Aggregate.Order, OrderToReturnDto>(order));
            
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
        {
            var buyemail = _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(buyemail))
            {
                return BadRequest(new ApiResponse(400, "Invalid token, email claim not found."));
            }
            var orders = await _orderService.GetOrdersForUserAsync(buyemail);
            if (orders == null || !orders.Any())
            {
                return NotFound(new ApiResponse(404, "No orders found for this user."));
            }
            return Ok(_mapper.Map<IReadOnlyList<Core.Entities.Order_Aggregate.Order>, IReadOnlyList<OrderToReturnDto>>(orders));
        }

        [ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderForUser(int id)
        {
            var email = _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new ApiResponse(400, "Invalid token, email claim not found."));
            }
            var orders = await _orderService.GetOrderByIdForUserAsync(email, id);
            if (orders is null) return NotFound(new ApiResponse(404));

        
            //var items = orders.Items.Select(i => new OrderItemDto()
            //{
            //    Id = i.Id,
            //    ProductId = i.Product.ProductId,
            //    ProductName = i.Product.ProductName,
            //    PictureUrl = i.Product.PictureUrl,
            //    Price = i.Price,
            //    Quantity = i.Quantity,
                
            //}).ToList();
            //var orderToReturn = new OrderToReturnDto
            //{
            //    BuyerEmail = orders.BuyerEmail,
            //    OrderDate = orders.OrderDate,
            //    ShippingAddress = orders.ShippingAddress,
            //    DeliveryMethod = orders.DeliveryMethod.ShortName,
            //    DeliveryMethodCost = orders.DeliveryMethod.Cost,
            //    Status = orders.Status.ToString(),
            //    Subtotal = orders.Subtotal,
            //    Items = items,
            //    Total = orders.GetTotal(),  
                
            //};
    
            //return Ok(orderToReturn);

            return Ok(_mapper.Map<Core.Entities.Order_Aggregate.Order, OrderToReturnDto>(orders)); 
        }

        [Authorize]
        [HttpGet("deliverymethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethod()
        {
            var deliveryMethods = await _orderService.GetDeliveryMethodAsync();
            return Ok(deliveryMethods);
        }
    }
}
