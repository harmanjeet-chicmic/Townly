# T-REX Backend Integration Status

Status of .NET implementation vs the Backend Integration Guide (T-REX / Amoy) and the **T-REX Real Estate Tokenization Developer Guide** (Add Property / Buy Property flows).

---

## Alignment with T-REX Real Estate Tokenization Developer Guide

### 1. "Add Property" Flow (from Developer Guide)

The guide describes four steps when a property is added (tokenized):

| Step | Developer Guide | Our implementation |
|------|-----------------|--------------------|
| 1. Deploy T-REX Suite | Factory deploys ERC-3643 token suite (token, identity registry, compliance). | **Implemented** via **single API** `POST api/property-suite/create`: calls TREXFactory.deployTREXSuite, parses TREXSuiteDeployed for token + IR. |
| 2. Deploy Property Vault | Vault Factory deploys `RealEstateVault` for the token + stablecoin, sets `pricePerShare`. | **Implemented** in same API: VaultFactory.deployVault, then vaults(token). |
| 3. Register Property NFT | `registry.registerProperty(issuer, metadataUri, tokenAddress, vaultAddress)` — mints NFT linking property to token and vault. | **Implemented** in same API: registry.registerProperty. Also available separately via services; audit in `OnChainPropertyRegistrations`. |
| 4. Supply the Vault | Set identity for vault, mint property tokens to vault (ready to sell). | **Implemented** in same API: registerIdentity on token's IR for vault + marketplace (+ optional users), unpause if needed, token.mint(vault, amount), compliance.addModule(vault). |

**Summary:** The **full Add Property flow** is available in **one API**: `POST api/property-suite/create` (Admin). It runs: deploy T-REX suite → deploy vault → register property → verify identities (vault, marketplace, optional users) → unpause, mint to vault, bind vault to compliance. Requires TRex:TrexFactoryAddress, RealEstateVaultFactoryAddress, ClaimIssuerAddress, and deployer (TRex:PrivateKey) with factory owner and agent roles.

---

### 2. "Buy Property" Flow (from Developer Guide)

The guide describes: Identity/KYC → Fund wallet → Approve marketplace → Execute purchase.

| Step | Developer Guide | Our implementation |
|------|-----------------|--------------------|
| 1. Identity & KYC | OnchainID in token’s Identity Registry; ClaimIssuer mints KYC claim. | **Partially covered.** We use the **global** Identity Registry: `updateIdentity`, `updateCountry`, and when the contract supports it **`registerIdentity(user, identity, country)`** in one tx (`POST api/admin/kyc/on-chain/register-identity`); plus `isVerified(user)` (Flow 1). We do not implement OnchainID deployment or ClaimIssuer claim minting; we assume the identity contract and claims are set up elsewhere. |
| 2. Fund wallet | User holds stablecoin + gas. | User responsibility; we do not fund wallets. |
| 3. Approve Marketplace | User approves marketplace to spend stablecoin. | **Implemented (Flow 5).** Backend (TRex:PrivateKey wallet) approves stablecoin for the marketplace before calling `buyShares`. |
| 4. Execute Purchase | User calls `marketplace.buyShares(tokenAddress, amount)`. | **Implemented (Flow 5).** We call `buyShares` and record the purchase in `OnChainSharePurchases`. |

**What happens inside the marketplace (from the guide):** The marketplace resolves the vault for the token, gets `pricePerShare`, computes total cost, pulls stablecoin from the user via `transferFrom`, then calls `vault.buyFor(user, amount)` so the vault sends property tokens to the user (after T-REX compliance checks).

**Summary:** We implement **steps 3 and 4** (approve + buyShares), with an optional KYC check (`isVerified`) before buying. Identity/OnchainID/ClaimIssuer setup is outside this API.

---

### 3. Sell Property Tokens

The Developer Guide focuses on Add and Buy; we also implement **selling** (Flow 6): approve property token for the marketplace, then `marketplace.sellShares(propertyTokenAddress, amount)`, with audit in `OnChainShareSales`.

---

## Flow & feature status

| Flow / feature | MD description | Backend status | DB / audit | Notes |
|----------------|----------------|----------------|------------|--------|
| **Setup – RPC & keys** | `RPC_URL`, `PRIVATE_KEY`, contract addresses | **Done** | — | `TRexOptions` + `TRex` in appsettings; Identity + RealEstate registry addresses. |
| **Identity Registry ABI** | isVerified, updateIdentity, updateCountry, registerIdentity | **Done** | — | `IdentityRegistryContractService`. |
| **RealEstateRegistry ABI** | registerProperty, propertyTokens, propertyVaults | **Done** | — | `RealEstateRegistryContractService`. |
| **RealEstateMarketplace ABI** | buyShares, sellShares | **Done** | — | `RealEstateMarketplaceContractService`. Flow 6 sellShares also implemented. |
| **ERC20 ABI** | approve, balanceOf | **Done** | — | `ERC20ContractService` for stablecoin & property tokens. |
| **Flow 1 – KYC: Check verified** | identityRegistry.isVerified(address) | **Done** | No DB | `GET api/kyc/on-chain/verified` (user), `GET api/admin/kyc/on-chain/verified/{address}` (admin). |
| **Flow 2 – KYC: Update identity** | updateIdentity(user, identityContract) | **Done** | **Done** | Contract call + record in `OnChainKycActions`. `POST api/admin/kyc/on-chain/identity`. |
| **Flow 3 – KYC: Set country** | updateCountry(user, countryCode) | **Done** | **Done** | Contract call + record in `OnChainKycActions`. `POST api/admin/kyc/on-chain/country`. |
| **Flow 4 – Register property** | registry.registerProperty(to, uri, token, vault) | **Done** | **Done** | Contract call + record in `OnChainPropertyRegistrations`. `POST api/admin/properties/on-chain/register`. propertyTokens/propertyVaults view calls available. |
| **Add Property Step 4 – Supply vault** | updateIdentity(vault, identity); token.mint(vault, amount) | **Done** | **Done** | `POST api/admin/properties/on-chain/supply-vault`. Optional vault identity, then mint; audit in `OnChainVaultSupplies`. |
| **Flow 5 – Buy shares** | KYC check → approve stablecoin → buyShares | **Done** | **Done** | `POST api/investments/on-chain/buy`. Audit in `OnChainSharePurchases`. |
| **Flow 6 – Sell shares** | approve property token → sellShares | **Done** | **Done** | `POST api/investments/on-chain/sell`. Audit in `OnChainShareSales`. |
| **KYC – registerIdentity** | registerIdentity(user, identity, country) in one tx | **Done** | **Done** | When contract supports it: `POST api/admin/kyc/on-chain/register-identity`. Audit in `OnChainKycActions` (action type RegisterIdentity). |
| **Full property suite (one API)** | Deploy suite + vault + register + identities + mint + bind | **Done** | — | `POST api/property-suite/create`. Uses TREXFactory, VaultFactory, Registry, token IR, ComplianceToken, ModularCompliance. |

---

## What is done (summary)

- **Flow 1:** On-chain KYC “is verified” (read-only, no DB).
- **Flow 2:** On-chain update identity (contract + DB audit in `OnChainKycActions`).
- **Flow 3:** On-chain set country (contract + DB audit in `OnChainKycActions`).
- **Flow 4:** On-chain register property (contract + DB audit in `OnChainPropertyRegistrations`); view helpers for property token/vault.
- **Add Property Step 4:** Supply vault: optional updateIdentity(vault, identity), then token.mint(vault, amount); audit in `OnChainVaultSupplies`. `IComplianceTokenContractService` (mint) + `ISupplyVaultOnChainService`.
- **Flow 5:** On-chain buy shares: KYC check → approve stablecoin → buyShares; audit in `OnChainSharePurchases`. ERC20 + Marketplace contract services.
- **Flow 6:** On-chain sell shares: approve property token → sellShares; audit in `OnChainShareSales`.
- **KYC registerIdentity:** When Identity Registry supports it: registerIdentity(user, identity, country) in one tx; audit in `OnChainKycActions` (RegisterIdentity).
- **Config:** TRex section (RpcUrl, PrivateKey, IdentityRegistry, RealEstateRegistry, StablecoinAddress, RealEstateMarketplaceAddress).
- **Comments:** XML summaries on interfaces, services, controllers, and repository for these flows.

---

## What we have done (summary)

- **KYC (Identity Registry):** Check if wallet is verified (`isVerified`), register/update identity (`updateIdentity`), set country (`updateCountry`). All write operations stored in `OnChainKycActions`.
- **Add Property (steps 3 & 4):** Register property in RealEstateRegistry (`registerProperty(...)`). Audit in `OnChainPropertyRegistrations`. **Supply vault:** optional vault identity + `token.mint(vault, amount)`; audit in `OnChainVaultSupplies`. View helpers: `propertyTokens(propertyId)`, `propertyVaults(propertyId)`.
- **Buy Property:** Verify KYC → approve stablecoin for marketplace → `buyShares(propertyTokenAddress, amount)`. Audit in `OnChainSharePurchases`. **Identity:** `updateIdentity`/`updateCountry` and, when supported, `registerIdentity(user, identity, country)` in one call.
- **Sell Property:** Approve property token for marketplace → `sellShares(propertyTokenAddress, amount)`. Audit in `OnChainShareSales`.
- **Config:** TRex (RpcUrl, PrivateKey, IdentityRegistry, RealEstateRegistry, StablecoinAddress, RealEstateMarketplaceAddress). All contract addresses configurable (Amoy defaults).
- **API:** Admin endpoints for on-chain register, **supply-vault**, buy/sell, and KYC (identity, country, **register-identity**).

**Assumptions / out of scope:** We do not deploy T-REX suites or vaults (steps 1–2). We assume token and vault exist before `registerProperty` and **supply-vault**; the TRex:PrivateKey wallet must have minter/agent role on the property token for supply-vault. We do not deploy OnchainID or ClaimIssuer; we assume identity contracts and claims are set up elsewhere. Our backend may use a single executor wallet for approve/buy/sell/supply; that wallet must hold the required stablecoin/tokens and have the necessary contract roles.

---

## What is left to do

All integration flows from the Backend Integration Guide (T-REX) are implemented, including **Supply the Vault** (Add Property Step 4) and **registerIdentity** (Buy Property Step 1 when contract supports it). Optional: event listeners or jobs to sync on-chain state; or separate services/scripts for T-REX suite deployment and vault deployment (Add Property steps 1–2).

---

## Key types and endpoints (quick reference)

| Functionality | Interface / service | Endpoint(s) |
|---------------|--------------------|-------------|
| Flow 1 (read verified) | `IOnChainKycService.IsVerifiedAsync` | `GET api/kyc/on-chain/verified`, `GET api/admin/kyc/on-chain/verified/{address}` |
| Flow 2 (update identity) | `IOnChainKycService.UpdateIdentityOnChainAsync` | `POST api/admin/kyc/on-chain/identity` |
| Flow 3 (set country) | `IOnChainKycService.UpdateCountryOnChainAsync` | `POST api/admin/kyc/on-chain/country` |
| Full property suite | `ICreatePropertySuiteOnChainService.CreatePropertySuiteAsync` | `POST api/property-suite/create` |
| Flow 4 (register property) | `IPropertyRegistrationOnChainService.RegisterPropertyOnChainAsync` | Used inside property-suite; no standalone admin endpoint. |
| Supply vault (Add Property Step 4) | `ISupplyVaultOnChainService.SupplyVaultOnChainAsync` | Used inside property-suite; no standalone admin endpoint. |
| Flow 5 (buy shares) | `IBuySharesOnChainService.BuySharesOnChainAsync` | `POST api/investments/on-chain/buy` |
| Flow 6 (sell shares) | `ISellSharesOnChainService.SellSharesOnChainAsync` | `POST api/investments/on-chain/sell` |
| KYC registerIdentity | `IOnChainKycService.RegisterIdentityOnChainAsync` | `POST api/admin/kyc/on-chain/register-identity` |
| Property token/vault (view) | `IRealEstateRegistryContractService.GetPropertyTokenAsync` / `GetPropertyVaultAsync` | Use from services; no dedicated HTTP endpoint yet. |

---

## DB tables used for on-chain audit

| Table | Purpose |
|-------|--------|
| **OnChainKycActions** | Flow 2 & 3: identity/country updates (wallet, tx hash, admin, optional UserId). |
| **OnChainPropertyRegistrations** | Flow 4: property registrations (to, uri, token, vault, tx hash, optional PropertyId, optional on-chain property ID). |
| **OnChainVaultSupplies** | Add Property Step 4: vault supply (vaultAddress, tokenAddress, amountMintedRaw, identityTxHash?, mintTxHash, optional PropertyId). |
| **OnChainSharePurchases** | Flow 5: share purchases (userId, userWallet, propertyToken, amounts, approveTxHash, buyTxHash). |
| **OnChainShareSales** | Flow 6: share sales (userId, userWallet, propertyToken, amountOfSharesRaw, approveTxHash, sellTxHash). |

Apply migrations so these tables exist: `AddOnChainKycActions`, `AddOnChainPropertyRegistrations`, `AddOnChainVaultSupplies`, `AddOnChainSharePurchases`, `AddOnChainShareSales`.
