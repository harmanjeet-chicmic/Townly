using System;
using System.Collections.Generic;
using System.Text;

namespace RealEstateInvesting.Application
{
    /// <summary>
    /// Response for the create property suite API.
    /// </summary>
    public class CreatePropertySuiteApiResponse
    {
        public string TokenAddress { get; set; } = default!;
        public string VaultAddress { get; set; } = default!;
        public string IdentityRegistryAddress { get; set; } = default!;
        public string DeploySuiteTxHash { get; set; } = default!;
        public string DeployVaultTxHash { get; set; } = default!;
        public string RegisterPropertyTxHash { get; set; } = default!;
        public IReadOnlyList<string> IdentityTxHashes { get; set; } = Array.Empty<string>();
        public string? UnpauseTxHash { get; set; }
        public string MintTxHash { get; set; } = default!;
        public string? BindComplianceTxHash { get; set; }
    }
}
