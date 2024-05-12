using PlayFab;
using System.ComponentModel.DataAnnotations;

namespace WebGameLogic.Playfab
{
    [Serializable]
    public class FunctionArgument
    {
        
    }

    [Serializable]
    public class CloudscriptRequest<T>
    {
        [Required(ErrorMessage = "Caller entity profile is required")]
        public PlayfabEntityProfile? CallerEntityProfile { get; set; }

        public FunctionArgument FunctionArgument { get; set; }

        public string PlayfabId => CallerEntityProfile?.Lineage?.MasterPlayerAccountId!;

        public CloudscriptRequest() {}

        public CloudscriptRequest(string userId)
        {
            CallerEntityProfile = new PlayfabEntityProfile()
            {
                Lineage = new PlayfabLineage()
                {
                    MasterPlayerAccountId = userId,
                    TitlePlayerAccountId = userId
                },
                Entity = new PlayfabEntity()
                {
                    Id = userId
                }
            };
        }
    }

    [Serializable]
    public class PlayfabEntityProfile
    {
        [Required(ErrorMessage = "Entity is Required")]
        public PlayfabEntity? Entity { get; set; }

        [Required(ErrorMessage = "Lineage is Required")]
        public PlayfabLineage? Lineage { get; set; }
    }

    [Serializable]
    public class PlayfabEntity
    {
        [Required(ErrorMessage = "Entity ID is required")]
        public string? Id { get; set; }
    }

    [Serializable]
    public class PlayfabLineage
    {
        [Required(ErrorMessage = "MasterPlayerAccountId is required")]
        public string? MasterPlayerAccountId { get; set; }

        [Required(ErrorMessage = "TitlePlayerAccountId is required")]
        public string? TitlePlayerAccountId { get; set; }
    }
}
