using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebGameLogic.Playfab
{
    [Serializable]
    public class FunctionArgument
    {
        // Empty implementation
    }

    [Serializable]
    public class CloudscriptRequest<T> where T : FunctionArgument, new()
    {
        [Required(ErrorMessage = "Caller entity profile is required")]
        public PlayfabEntityProfile? CallerEntityProfile { get; set; }

        [DisallowNull]
        public T FunctionArgument { get; set; } = new T();

        public string PlayfabId => CallerEntityProfile?.Lineage?.MasterPlayerAccountId ?? string.Empty;

        public CloudscriptRequest() { }

        public CloudscriptRequest(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
            }
            
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
