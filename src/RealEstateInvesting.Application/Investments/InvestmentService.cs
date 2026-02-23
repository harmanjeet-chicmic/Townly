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
using RealEstateInvesting.Application.Common.Exceptions;
using RealEstateInvesting.Application.Common.Errors;

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
                ?? throw new BusinessException(
                    ErrorCodes.UserNotFound,
                    ErrorMessages.UserNotFound);

            if (user.KycStatus != KycStatus.Approved)
                throw new BusinessException(
                    ErrorCodes.KycRequired,
                    ErrorMessages.KycRequired);

            if (user.IsBlocked)
                throw new BusinessException(
                    ErrorCodes.UserBlocked,
                    ErrorMessages.UserBlocked);

            // 2️⃣ Property validation
            var property = await _propertyRepository.GetByIdAsync(dto.PropertyId)
                ?? throw new BusinessException(
                    ErrorCodes.PropertyNotFound,
                    ErrorMessages.PropertyNotFound);

            if (property.OwnerUserId == userId)
                throw new BusinessException(
                    ErrorCodes.OwnPropertyInvestment,
                    ErrorMessages.OwnPropertyInvestment);

            if (property.Status != PropertyStatus.Active)
                throw new BusinessException(
                    ErrorCodes.PropertyNotActive,
                    ErrorMessages.PropertyNotActive);

            // 3️⃣ Shares availability
            var investedShares =
                await _investmentRepository.GetTotalSharesInvestedAsync(property.Id);

            var availableShares = property.TotalUnits - investedShares;

            if (dto.Shares > 10000)
                throw new BusinessException(
                    ErrorCodes.ExcessShares,
                    "Cannot buy more than 10000 shares.");

            if (dto.Shares <= 0)
                throw new BusinessException(
                    ErrorCodes.InsufficientShares,
                    "Investment shares must be greater than zero.");

            if (dto.Shares > availableShares)
                throw new BusinessException(
                    ErrorCodes.InsufficientShares,
                    ErrorMessages.InsufficientShares);

            // 4️⃣ Price calculation (USD source of truth)
            var pricePerShareUsd =
                property.ApprovedValuation / property.TotalUnits;

            var investmentUsd =
                decimal.Round(dto.Shares * pricePerShareUsd, 2);

            var ethUsdRate = await _priceFeed.GetEthUsdPriceAsync();
            if (ethUsdRate <= 0)
                throw new BusinessException(
                    ErrorCodes.InvalidEthPrice,
                    ErrorMessages.InvalidEthPrice);

            // 🔥 Flat Platform Fee (0.05 ETH)
            const decimal PlatformFeeEth = 0.05m;

            var investmentEth =
                decimal.Round(investmentUsd / ethUsdRate, 8);

            var platformFeeEth = PlatformFeeEth;

            var platformFeeUsd =
                decimal.Round(platformFeeEth * ethUsdRate, 2);

            var totalRequiredEth =
                decimal.Round(investmentEth + platformFeeEth, 8);

            // 5️⃣ Token check & deduction
            var tokenBalance =
                await _userTokenBalanceRepository.GetByUserIdAsync(userId);

            if (tokenBalance == null || tokenBalance.Available < totalRequiredEth)
                throw new BusinessException(
                    ErrorCodes.InsufficientTokens,
                    ErrorMessages.InsufficientTokens);

            tokenBalance.Deduct(totalRequiredEth);

            var tokenTx = TokenTransaction.Create(
                userId: userId,
                amount: totalRequiredEth,
                type: "Deduct",
                reference: $"Property:{property.Id}");

            await _tokenTransactionRepository.AddAsync(tokenTx);

            // 6️⃣ Create investment
            var investment = Investment.Create(
                userId,
                property.Id,
                dto.Shares,
                pricePerShareUsd,
                ethUsdRate);

            await _investmentRepository.AddAsync(investment);

            // 7️⃣ Ledger transactions
            var transactions = new List<Transaction>
{
    // 🔹 Investor (Investment - type 1)
    Transaction.CreateWithEth(
        userId: userId,
        propertyId: property.Id,
        type: TransactionType.Investment,
        amountUsd: investmentUsd,
        ethAmount: investmentEth,
        ethUsdRate: ethUsdRate,
        referenceId: investment.Id),

    // 🔹 Owner (Income - type 2)
    Transaction.CreateWithEth(
        userId: property.OwnerUserId,
        propertyId: property.Id,
        type: TransactionType.RentalIncome,
        amountUsd: investmentUsd,
        ethAmount: investmentEth,
        ethUsdRate: ethUsdRate,
        referenceId: investment.Id)
};

            await _transactionRepository.AddRangeAsync(transactions);

            // 8️⃣ Sold-out detection
            if (availableShares - dto.Shares == 0)
            {
                property.MarkSoldOut();
                await _propertyRepository.UpdateAsync(property);
                isSoldOut = true;
            }

            // 9️⃣ Commit
            await _unitOfWork.CommitAsync();

            // 🔔 Owner notified
            await _notificationService.CreateAsync(
                property.OwnerUserId,
                NotificationType.InvestmentReceived,
                "New Investment Received",
                $"You received a new investment of ${investmentUsd:N2} in \"{property.Name}\".",
                property.Id);

            // 🔔 Sold-out notification (your existing logic)
            if (isSoldOut)
            {
                await _notificationService.CreateAsync(
                    property.OwnerUserId,
                    NotificationType.PropertySoldOut,
                    "Property Sold Out",
                    $"Your property \"{property.Name}\" is now fully sold out.",
                    property.Id);
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
