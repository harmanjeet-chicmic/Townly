// using RealEstateInvesting.Application.Common.Interfaces;
// using RealEstateInvesting.Application.Transactions.Dtos;
// using RealEstateInvesting.Domain.Entities;

// namespace RealEstateInvesting.Application.Transactions;

// public class TokenPurchaseService
// {
//     private readonly ITokenPurchaseRepository _repository;
//     //private readonly ILogger<TokenPurchaseService> _logger;

//     public TokenPurchaseService(
//         ITokenPurchaseRepository repository
//        )
//     {
//         _repository = repository;
//         //_logger = logger;
//     }

//     public async Task<Guid> CreateTokenPurchaseAsync(Guid userId, CreateTokenPurchaseDto dto, CancellationToken cancellationToken = default)
//     {
//         //_logger.LogInformation("Creating TokenPurchase for user {UserId}, property {PropertyId}", userId, dto.PropertyId);

//         var purchase = TokenPurchase.Create(
//             dto.PropertyId,
//             dto.TransactionHash,
//             dto.BuyerAddress,
//             dto.SellerAddress,
//             dto.Shares,
//             dto.Amount,
//             dto.Status,
//             dto.ErrorMessage,
//             userId
//         );

//         await _repository.AddAsync(purchase, cancellationToken);

//         return purchase.Id;
//     }
// }
