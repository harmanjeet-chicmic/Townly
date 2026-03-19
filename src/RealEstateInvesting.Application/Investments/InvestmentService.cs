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

            // 4️⃣ Price calculation 
            var pricePerShareUsd =
                property.ApprovedValuation / property.TotalUnits;

            var investmentUsd =
                dto.Shares * pricePerShareUsd;

            var ethUsdRate = await _priceFeed.GetEthUsdPriceAsync();
            if (ethUsdRate <= 0)
                throw new BusinessException(
                    ErrorCodes.InvalidEthPrice,
                    ErrorMessages.InvalidEthPrice);

            // 🔥 ETH calculation 
            var investmentEth =
                Math.Round(investmentUsd / ethUsdRate, 8);

            Console.WriteLine("============invested amount from service", investmentEth);

            const decimal PlatformFeeEth = 0.05m;

            var totalRequiredEth =
                investmentEth + PlatformFeeEth;


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
            Console.WriteLine("==== DEBUG BEFORE CREATE ====");
            Console.WriteLine($"Shares: {dto.Shares}");
            Console.WriteLine($"PricePerShareUsd: {pricePerShareUsd}");
            Console.WriteLine($"InvestmentUsd: {investmentUsd}");
            Console.WriteLine($"EthUsdRate: {ethUsdRate}");
            Console.WriteLine($"InvestmentEth (SERVICE): {investmentEth}");

            var investment = Investment.Create(
                userId,
                property.Id,
                dto.Shares,
                pricePerShareUsd,
                ethUsdRate,
                investmentEth);

            await _investmentRepository.AddAsync(investment);
            Console.WriteLine("==== DEBUG AFTER SAVE ====");
            Console.WriteLine($"Stored EthAmount: {investment.EthAmountAtExecution}");

            // 7️⃣ Ledger transactions 
            var transactions = new List<Transaction>
        {
            Transaction.CreateWithEth(
                userId: userId,
                propertyId: property.Id,
                type: TransactionType.Investment,
                amountUsd: investmentUsd,
                ethAmount: investmentEth,
                ethUsdRate: ethUsdRate,
                referenceId: investment.Id),

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

           
            if (availableShares - dto.Shares == 0)
            {
                property.MarkSoldOut();
                await _propertyRepository.UpdateAsync(property);
                isSoldOut = true;
            }

            // 9️⃣ Commit
            await _unitOfWork.CommitAsync();

          
            await _notificationService.CreateAsync(
                property.OwnerUserId,
                NotificationType.InvestmentReceived,
                "New Investment Received",
                $"You received a new investment of ${investmentUsd:N2} in \"{property.Name}\".",
                property.Id);

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
