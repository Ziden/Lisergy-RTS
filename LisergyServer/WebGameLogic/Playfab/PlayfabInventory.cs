using PlayFab;
using PlayFab.ServerModels;

namespace WebGameLogic.Playfab
{
    public class PlayfabInventory //: IPlayerInventory
    {
        private string _playfabId;

        public PlayfabInventory(string userId)
        {
            _playfabId = userId;
        }

        public async Task AddItem(int id, int amt)
        {
            var response = await PlayFabServerAPI.AddUserVirtualCurrencyAsync(new AddUserVirtualCurrencyRequest()
            {
                Amount = amt,
                PlayFabId = _playfabId,
                VirtualCurrency = id.ToString()
            });
            if (response.Error != null) throw new Exception(response.Error.GenerateErrorReport());
        }

        public async Task<bool> ConsumeItem(int id, int amt)
        {
            var response = await PlayFabServerAPI.SubtractUserVirtualCurrencyAsync(new SubtractUserVirtualCurrencyRequest()
            {
                PlayFabId = _playfabId,
                VirtualCurrency = id.ToString(),
                Amount = amt
            });
            if (response.Error != null) throw new Exception(response.Error.GenerateErrorReport());
            return response.Result.BalanceChange == amt;
        }

        public async Task<IReadOnlyDictionary<int, int>> GetItems()
        {
            var response = await PlayFabServerAPI.GetUserInventoryAsync(new GetUserInventoryRequest()
            {
                PlayFabId = _playfabId,   
            });
            if (response.Error != null) throw new Exception(response.Error.GenerateErrorReport());
            var result = response.Result;
            var dict = new Dictionary<int, int>();
            foreach(var i in result.VirtualCurrency)
            {
                var itemId = Int32.Parse(i.Key);     
                dict[itemId] = i.Value;
            }
            return dict;
        }
    }
}
