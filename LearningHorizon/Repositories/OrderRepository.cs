using LearningHorizon.Data;
using LearningHorizon.Data.Models;
using LearningHorizon.Interfaces;
using System.Text;
using System.Text.Json;

namespace LearningHorizon.Repositories
{
    public class OrderRepository : GenericRepository<Order> , IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public OrderRepository(ApplicationDbContext context, IConfiguration configuration, IHttpClientFactory httpClientFactory) : base(context)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<string> purchaseCourse(Order order, User user)
        {
            string authToken = await GetAuthToken();
            string paymobOrderId = await CreateOrder(authToken, order);
            order.paymobOrderId = paymobOrderId;
            string paymentKey = await GetPaymentKey(authToken, order, user, paymobOrderId);
            string iframeUrl = _configuration["LocalPayment:IFrameLink"] + paymentKey;
            return iframeUrl;
        }

        private async Task<string> GetAuthToken()
        {
            var data = new
            {
                api_key = this._configuration["LocalPayment:api_key"]
            };
            var response = await _httpClient.PostAsync("https://accept.paymob.com/api/auth/tokens",
                                                        new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"));

            var responseContent = await response.Content.ReadAsStringAsync();

            var json = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);

            var token = json["token"].ToString();

            return token;
            //return JsonSerializer.Deserialize<Dictionary<string, object>>(await (await this._httpClient.PostAsync("https://accept.paymob.com/api/auth/tokens", (HttpContent)new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"))).Content.ReadAsStringAsync())["token"].ToString();
        }

        private async Task<string> CreateOrder(string authToken, Order order)
        {
            var data = new
            {
                auth_token = authToken,
                delivery_needed = "false",
                amount_cents = $"{order.totalAmount * 100M}",
                currency = "EGP",
                items = Array.Empty<Object>()
            };

            var response = await _httpClient.PostAsync("https://accept.paymob.com/api/ecommerce/orders",
                                                        new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"));

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var json = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse);

            return json["id"].ToString();

            //return JsonSerializer.Deserialize<Dictionary<string, object>>(await (await this._httpClient.PostAsync("https://accept.paymob.com/api/ecommerce/orders", (HttpContent)new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"))).Content.ReadAsStringAsync())["id"].ToString();
        }

        private async Task<string> GetPaymentKey(string authToken, Order order, User user, string paymentOrderId)
        {
            var buldingData = new
            {
                email = user.email,
                first_name = user.firstName,
                last_name = user.lastName,
                city = user.city ?? "NA",
                country = user.country ?? "NA",
                apartment = "NA",
                floor = "NA",
                street = "NA",
                building = "NA",
                phone_number = "NA",
                shipping_method = "NA",
                postal_code = "NA",
                state = "NA"
            };
            var data = new
            {
                auth_token = authToken,
                amount_cents = $"{order.totalAmount *100M}",
                expiration = 3600,
                order_id = paymentOrderId,
                billing_data = buldingData,
                currency = "EGP",
                integration_id = _configuration["LocalPayment:integration_id"],
                redirect_url = _configuration["LocalPayment:redirect_url"]
            };

            var response = await _httpClient.PostAsync("https://accept.paymob.com/api/acceptance/payment_keys",
                                                        new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"));

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<Dictionary<string, object>>(content);

            return json["token"].ToString();

            //return JsonSerializer.Deserialize<Dictionary<string, object>>(await (await this._httpClient.PostAsync("https://accept.paymob.com/api/acceptance/payment_keys", (HttpContent)new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json"))).Content.ReadAsStringAsync())["token"].ToString();
        }
    }
}
