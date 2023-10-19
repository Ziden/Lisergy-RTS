using GameData;
using PlayFab;
using PlayFab.AdminModels;

namespace WebGameLogic.Playfab
{
    /// <summary>
    /// Sets up playfab data. Creates currencies etc to make sure playfab is in sync.
    /// </summary>
    public class PlayfabSetup
    {
        private GameSpec _specs;

        public PlayfabSetup(GameSpec specs)
        {
            _specs = specs;
        }

        /// <summary>
        /// Sync game specs to playfab data
        /// </summary>
        public async Task SetupPlayfab()
        {
            var serialized = WebSerializer.Serialize(_specs);
            await PlayFabServerAPI.SetTitleDataAsync(new PlayFab.ServerModels.SetTitleDataRequest()
            {
                Key = "GameData",
                Value = serialized
            });

            var response = await PlayFabAdminAPI.ListVirtualCurrencyTypesAsync(new ListVirtualCurrencyTypesRequest());
            if (response.Error != null) throw new Exception(response.Error.GenerateErrorReport());
            var playfabTypes = new HashSet<int>(response.Result.VirtualCurrencies.Select(s => Int32.Parse(s.CurrencyCode)));
            var specTypes = new HashSet<int>(_specs.Items.Keys.Select(k => (int)k));

            // Get all on spec but not on playfab
            var needsToAdd = specTypes.Except(playfabTypes);
            await PlayFabAdminAPI.AddVirtualCurrencyTypesAsync(new AddVirtualCurrencyTypesRequest()
            {
                VirtualCurrencies = needsToAdd.Select(id => new VirtualCurrencyData()
                {
                    CurrencyCode = id.ToString(),
                    DisplayName = _specs.Items[(ushort)id].Name,
                    InitialDeposit = 0,
                    RechargeMax = 0,
                    RechargeRate = 0
                }).ToList()
            });
            // TODO: Delete unused ones
        }
    }
}
