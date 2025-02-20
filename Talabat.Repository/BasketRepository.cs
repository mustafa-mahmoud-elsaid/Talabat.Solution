using StackExchange.Redis;
using System.Text.Json;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;

namespace Talabat.Repository
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _database;

        public BasketRepository(IConnectionMultiplexer connection)
        {
            _database = connection.GetDatabase();
        }
        public async Task<CustomerBasket?> GetBasketAsync(string basketId)
        {
            var result = await _database.StringGetAsync(basketId);
            return result.IsNull ? null : JsonSerializer.Deserialize<CustomerBasket>(result!);
        }
        public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket customerBasket)
        {
            var basketJson = JsonSerializer.Serialize(customerBasket);
            var isCreatedOrUpdated = await _database.StringSetAsync(customerBasket.Id , basketJson, TimeSpan.FromDays(3));
            if (isCreatedOrUpdated)
                return await GetBasketAsync(customerBasket.Id);
            return null;
        }
        public async Task<bool> DeleteBasketAsync(string basketId)
        {
            return await _database.KeyDeleteAsync(basketId);
        }

    }
}
