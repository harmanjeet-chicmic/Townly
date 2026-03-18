using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateInvesting.API.Contracts;
using RealEstateInvesting.Application;
using RealEstateInvesting.Application.Common.Interfaces;
using System.Security.Claims;

namespace RealEstateInvesting.API.Controllers;

/// <summary>
/// Single API for the complete property creation flow: deploy T-REX suite, vault, register property, setup identities, mint and bind compliance.
/// </summary>
[ApiController]
[Route("api/property-suite")]
[Authorize(Roles = "Admin")]
public class PropertySuiteController : ControllerBase
{
    private readonly ICreatePropertySuiteOnChainService _createPropertySuite;
    private readonly ILogger<PropertySuiteController> _logger;  

    public PropertySuiteController(ICreatePropertySuiteOnChainService createPropertySuite, ILogger<PropertySuiteController> logger)
    {
        _createPropertySuite = createPropertySuite;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new property on-chain in one call: 1) Deploy T-REX token suite, 2) Deploy vault, 3) Register in registry, 4) Verify identities (vault, marketplace, optional users), 5) Unpause if needed, mint to vault, bind vault to compliance.
    /// Requires TRex:TrexFactoryAddress, RealEstateVaultFactoryAddress, ClaimIssuerAddress, and deployer wallet (TRex:PrivateKey) with factory owner and agent roles.
    /// </summary>
    [HttpPost("create")]
    [ProducesResponseType(typeof(CreatePropertySuiteApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePropertySuiteApiRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.PropertyId) || string.IsNullOrWhiteSpace(request.PropertyPriceUsdc) || string.IsNullOrWhiteSpace(request.MintAmount))
                return BadRequest(new { message = "PropertyId, PropertyPriceUsdc, and MintAmount are required." });
            if (string.IsNullOrWhiteSpace(request.VaultIdentityAddress) || string.IsNullOrWhiteSpace(request.MarketplaceIdentityAddress))
                return BadRequest(new { message = "VaultIdentityAddress and MarketplaceIdentityAddress are required." });

            var appRequest = new CreatePropertySuiteRequest
            {
                PropertyId = request.PropertyId,
                PropertyPriceUsdc = request.PropertyPriceUsdc,
                MintAmount = request.MintAmount,
                MetadataUri = request.MetadataUri,
                VaultIdentityAddress = request.VaultIdentityAddress,
                MarketplaceIdentityAddress = request.MarketplaceIdentityAddress,
                IdentityCountryCode = request.IdentityCountryCode,
                AdditionalIdentities = request.AdditionalIdentities?.Select(a => new IdentityEntry
                {
                    WalletAddress = a.WalletAddress,
                    IdentityContractAddress = a.IdentityContractAddress,
                    CountryCode = a.CountryCode
                }).ToList()
            };

            var result = await _createPropertySuite.CreatePropertySuiteAsync(appRequest, cancellationToken).ConfigureAwait(false);

            return Ok(new CreatePropertySuiteApiResponse
            {
                TokenAddress = result.TokenAddress,
                VaultAddress = result.VaultAddress,
                IdentityRegistryAddress = result.IdentityRegistryAddress,
                DeploySuiteTxHash = result.DeploySuiteTxHash,
                DeployVaultTxHash = result.DeployVaultTxHash,
                RegisterPropertyTxHash = result.RegisterPropertyTxHash,
                IdentityTxHashes = result.IdentityTxHashes,
                UnpauseTxHash = result.UnpauseTxHash,
                MintTxHash = result.MintTxHash,
                BindComplianceTxHash = result.BindComplianceTxHash
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating property suite");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error creating property suite" });
        }
       
    }
}


