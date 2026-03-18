# Property Suite Create – Request body

**Endpoint:** `POST /api/property-suite/create` (Admin only)

**Configured addresses (Amoy):**

| Key | Address |
|-----|---------|
| Stablecoin | `0xF39906465Ac54E2370c4a189Af83b143f99F7010` |
| Identity Registry (global) | `0x192738Fb0FF12Aa5564FAF18dEa315ccdF5A98ec` |
| Real Estate Registry | `0xB79a38247D66369fD9A5dF844Eac0fe94a88abd1` |
| Real Estate Marketplace | `0x4f087f31e47F6EC53e3eAaE7Eb7234B7A459DfBA` |
| TrexFactory | `0x1bF1199343E76ef017D4Ae5d1c6d37f7B08C7F35` |
| RealEstateVaultFactory | `0xe1b264D5d06F7a3F21d9Db8b5F99c154a8adBd2A` |
| ClaimIssuer | `0x443B2e65cA081616556746679866b455bc96281D` |

---

## Request body (minimal)

Replace `VaultIdentityAddress` and `MarketplaceIdentityAddress` with your **OnchainID identity proxy** addresses (from `deployments/interact-state.json` after running `interact-amoy.ts`: `ID_Vault`, `ID_Marketplace`). Optionally add `ID_Alice` (and other users) in `additionalIdentities`.

```json
{
  "propertyId": "AMOY-RWA-014",
  "propertyPriceUsdc": "150",
  "mintAmount": "2000",
  "vaultIdentityAddress": "<FROM interact-state.json: ID_Vault>",
  "marketplaceIdentityAddress": "<FROM interact-state.json: ID_Marketplace>",
  "identityCountryCode": 42
}
```

---

## Request body (with optional fields)

```json
{
  "propertyId": "AMOY-RWA-014",
  "propertyPriceUsdc": "150",
  "mintAmount": "2000",
  "metadataUri": "ipfs://meta-AMOY-RWA-014",
  "vaultIdentityAddress": "<ID_Vault from interact-state.json>",
  "marketplaceIdentityAddress": "<ID_Marketplace from interact-state.json>",
  "identityCountryCode": 42,
  "additionalIdentities": [
    {
      "walletAddress": "0xAliceWalletAddress",
      "identityContractAddress": "<ID_Alice from interact-state.json>",
      "countryCode": 42
    }
  ]
}
```

---

## cURL example

```bash
curl -X POST "https://your-api/api/property-suite/create" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_ADMIN_JWT" \
  -d '{
    "propertyId": "AMOY-RWA-014",
    "propertyPriceUsdc": "150",
    "mintAmount": "2000",
    "vaultIdentityAddress": "0xYourVaultIdentityProxy",
    "marketplaceIdentityAddress": "0xYourMarketplaceIdentityProxy",
    "identityCountryCode": 42
  }'
```

---

## Files

- **`property-suite-create-request.json`** – Template with placeholders (replace `REPLACE_*` with values from `interact-state.json`).
- **`property-suite-create-request-example.json`** – Example with placeholder identity address; replace with your real vault/marketplace identity proxies before use.
