// using RealEstateInvesting.Application.Common.Interfaces;
// using RealEstateInvesting.Application.Investments.Dtos;
// using RealEstateInvesting.Domain.Entities;
// using RealEstateInvesting.Domain.Enums;

// namespace RealEstateInvesting.Application.Investments;

// public class InvestmentService
// {
//     private readonly IUserRepository _userRepository;
//     private readonly IPropertyRepository _propertyRepository;
//     private readonly IInvestmentRepository _investmentRepository;
//     private readonly ITransactionRepository _transactionRepository;
//     private readonly IUnitOfWork _unitOfWork;
//     private readonly IPriceFeed _priceFeed;



//     public InvestmentService(
//         IUserRepository userRepository,
//         IPropertyRepository propertyRepository,
//         IInvestmentRepository investmentRepository,
//         ITransactionRepository transactionRepository,
//         IUnitOfWork unitOfWork,
//         IPriceFeed priceFeed)
//     {
//         _userRepository = userRepository;
//         _propertyRepository = propertyRepository;
//         _investmentRepository = investmentRepository;
//         _transactionRepository = transactionRepository;
//         _unitOfWork = unitOfWork;
//         _priceFeed = priceFeed;
//     }

//     public async Task<Guid> InvestAsync(Guid userId, CreateInvestmentDto dto)
//     {
//         await _unitOfWork.BeginTransactionAsync();

//         try
//         {
//             // 1️⃣ User validation
//             var user = await _userRepository.GetByIdAsync(userId)
//                 ?? throw new InvalidOperationException("User not found.");
//             // Console.WriteLine("====================================== kyc status "+user.KycStatus);
//             // Console.WriteLine("================================user id ==="+user.Id);

//             if (user.KycStatus != KycStatus.Approved)
//                 throw new InvalidOperationException("KYC approval required.");

//             if (user.IsBlocked)
//                 throw new InvalidOperationException("User is blocked.");

//             // 2️⃣ Property validation
//             var property = await _propertyRepository.GetByIdAsync(dto.PropertyId)
//                 ?? throw new InvalidOperationException("Property not found.");

//             if (property.OwnerUserId == userId)
//                 throw new InvalidOperationException("Cannot invest in own property.");

//             if (property.Status != PropertyStatus.Active)
//                 throw new InvalidOperationException("Property is not open for investment.");

//             // 3️⃣ Shares availability (checked INSIDE transaction)
//             var investedShares =
//                 await _investmentRepository.GetTotalSharesInvestedAsync(property.Id);

//             var availableShares = property.TotalUnits - investedShares;

//             if (dto.Shares <= 0)
//                 throw new InvalidOperationException("Shares must be greater than zero.");

//             if (dto.Shares > availableShares)
//                 throw new InvalidOperationException("Not enough shares available.");

//             // 4️⃣ Price per share (USD – source of truth)
//             var pricePerShare =
//                 property.ApprovedValuation / property.TotalUnits;
//             var ethUsdRate = await _priceFeed.GetEthUsdPriceAsync();

//             // 5️⃣ Create investment
//             var investment = Investment.Create(
//                 userId,
//                 property.Id,
//                 dto.Shares,
//                 pricePerShare,
//                 ethUsdRate
//             );


//             await _investmentRepository.AddAsync(investment);

//             // 6️⃣ Create transactions (ledger entries)
//             var transactions = new List<Transaction>
//         {
//             // Investor transaction
//             Transaction.Create(
//                 userId: userId,
//                 type: TransactionType.Investment,
//                 amount: investment.TotalAmount,
//                 referenceId: investment.Id,
//                 propertyId: property.Id,
//                 currency: "USD"
//             ),

//             // Property owner transaction
//             Transaction.Create(
//                 userId: property.OwnerUserId,
//                 type: TransactionType.Investment,
//                 amount: investment.TotalAmount,
//                 referenceId: investment.Id,
//                 propertyId: property.Id,
//                 currency: "USD"
//             )
//         };

//             await _transactionRepository.AddRangeAsync(transactions);

//             // 7️⃣ Auto mark sold out (boundary condition)
//             if (availableShares - dto.Shares == 0)
//             {
//                 property.MarkSoldOut();
//                 await _propertyRepository.UpdateAsync(property);
//             }

//             await _unitOfWork.CommitAsync();
//             return investment.Id;
//         }
//         catch
//         {
//             await _unitOfWork.RollbackAsync();
//             throw;
//         }
//     }

// }

using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Application.Investments.Dtos;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Domain.Enums;
using RealEstateInvesting.Application.Notifications.Interfaces;

namespace RealEstateInvesting.Application.Investments;

public class InvestmentService
{
    private readonly IUserRepository _userRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IInvestmentRepository _investmentRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUserTokenBalanceRepository _userTokenBalanceRepository;
    private readonly ITokenTransactionRepository _tokenTransactionRepository;
    private readonly INotificationService _notificationService;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IPriceFeed _priceFeed;

    public InvestmentService(
        IUserRepository userRepository,
        IPropertyRepository propertyRepository,
        IInvestmentRepository investmentRepository,
        ITransactionRepository transactionRepository,
        IUserTokenBalanceRepository userTokenBalanceRepository,
        ITokenTransactionRepository tokenTransactionRepository,
        IUnitOfWork unitOfWork,
        IPriceFeed priceFeed,
        INotificationService notificationService)
    {
        _userRepository = userRepository;
        _propertyRepository = propertyRepository;
        _investmentRepository = investmentRepository;
        _transactionRepository = transactionRepository;
        _userTokenBalanceRepository = userTokenBalanceRepository;
        _tokenTransactionRepository = tokenTransactionRepository;
        _unitOfWork = unitOfWork;
        _priceFeed = priceFeed;
        _notificationService = notificationService;
    }

    public async Task<Guid> InvestAsync(Guid userId, CreateInvestmentDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();
        var isSoldOut = false;

        try
        {
            // 1️⃣ User validation
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new InvalidOperationException("User not found.");

            if (user.KycStatus != KycStatus.Approved)
                throw new InvalidOperationException("KYC approval required.");

            if (user.IsBlocked)
                throw new InvalidOperationException("User is blocked.");

            // 2️⃣ Property validation
            var property = await _propertyRepository.GetByIdAsync(dto.PropertyId)
                ?? throw new InvalidOperationException("Property not found.");

            if (property.OwnerUserId == userId)
                throw new InvalidOperationException("Cannot invest in own property.");

            if (property.Status != PropertyStatus.Active)
                throw new InvalidOperationException("Property is not open for investment.");

            // 3️⃣ Shares availability (inside transaction)
            var investedShares =
                await _investmentRepository.GetTotalSharesInvestedAsync(property.Id);

            var availableShares = property.TotalUnits - investedShares;

            if (dto.Shares <= 0)
                throw new InvalidOperationException("Shares must be greater than zero.");

            if (dto.Shares > availableShares)
                throw new InvalidOperationException("Not enough shares available.");

            // 4️⃣ Price calculation (USD is source of truth)
            var pricePerShareUsd =
                property.ApprovedValuation / property.TotalUnits;

            var ethUsdRate = await _priceFeed.GetEthUsdPriceAsync();
            Console.WriteLine("===================ETH===========" + ethUsdRate);
            if (ethUsdRate <= 0)
                throw new InvalidOperationException("Invalid ETH price feed.");

            // 🔥 5️⃣ TOKEN CHECK & DEDUCTION (ETH units)
            var totalUsd = decimal.Round(dto.Shares * pricePerShareUsd, 2);
            var requiredTokensEth = decimal.Round(totalUsd / ethUsdRate, 8);

            var tokenBalance =
                await _userTokenBalanceRepository.GetByUserIdAsync(userId);
            Console.WriteLine("===========TToken Balance ==============" + tokenBalance.Available);
            if (tokenBalance == null || tokenBalance.Available < requiredTokensEth)
                throw new InvalidOperationException("Insufficient token balance.");

            // Deduct tokens
            tokenBalance.Deduct(requiredTokensEth);

            // Audit token deduction
            var tokenTx = TokenTransaction.Create(
                userId: userId,
                amount: requiredTokensEth,
                type: "Deduct",
                reference: $"Property:{property.Id}"
            );

            await _tokenTransactionRepository.AddAsync(tokenTx);

            // 6️⃣ Create investment
            var investment = Investment.Create(
                userId,
                property.Id,
                dto.Shares,
                pricePerShareUsd,
                ethUsdRate
            );

            await _investmentRepository.AddAsync(investment);

            // 7️⃣ Create USD ledger transactions
            var transactions = new List<Transaction>
            {
                // Investor ledger
                Transaction.CreateInvestment(
    userId: userId,
    propertyId: property.Id,
    amountUsd: investment.TotalAmount,
    ethAmount: investment.EthAmountAtExecution,
    ethUsdRate: investment.EthUsdRateAtExecution,
    referenceId: investment.Id
),

                // Property owner ledger
                Transaction.CreateInvestment(
    userId: property.OwnerUserId,
    propertyId: property.Id,
    amountUsd: investment.TotalAmount,
    ethAmount: investment.EthAmountAtExecution,
    ethUsdRate: investment.EthUsdRateAtExecution,
    referenceId: investment.Id
),
Transaction.CreateInvestment(
    userId: property.OwnerUserId,
    propertyId: property.Id,
    amountUsd: investment.TotalAmount,
    ethAmount: investment.EthAmountAtExecution,
    ethUsdRate: investment.EthUsdRateAtExecution,
    referenceId: investment.Id
),

            };

            await _transactionRepository.AddRangeAsync(transactions);

            // 8️⃣ Auto mark sold out
            if (availableShares - dto.Shares == 0)
            {
                property.MarkSoldOut();
                await _propertyRepository.UpdateAsync(property);
                isSoldOut = true;
            }


            // 9️⃣ Commit everything
            await _unitOfWork.CommitAsync();

            // 🔔 Property sold-out notification
            if (isSoldOut)
            {
                await _notificationService.CreateAsync(
                    property.OwnerUserId,
                    NotificationType.PropertySoldOut,
                    "Property Sold Out",
                    $"Your property \"{property.Name}\" is now fully sold out.",
                    property.Id
                );
            }

            return investment.Id;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
