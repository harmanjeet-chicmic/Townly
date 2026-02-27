using RealEstateInvesting.Application.Common.Interfaces;
using RealEstateInvesting.Domain.Entities;
using RealEstateInvesting.Domain.Enums;
using RealEstateInvesting.Application.Common.Errors;
using RealEstateInvesting.Application.Common.Exceptions;
namespace RealEstateInvesting.Application.Kyc;

public class SubmitKycHandler
{
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;
    private readonly IKycRecordRepository _kycRepository;
    public SubmitKycHandler(
        ICurrentUser currentUser,
        IUserRepository userRepository,
        IKycRecordRepository kycRepository)
    {
        _currentUser = currentUser;
        _userRepository = userRepository;
        _kycRepository = kycRepository;
    }

    public async Task HandleAsync(SubmitKycCommand command, CancellationToken cancellationToken = default)
    {
        // 1️⃣ Blocked user check
        if (_currentUser.IsBlocked)
            throw new InvalidOperationException("Blocked users cannot submit KYC.");

        // 2️⃣ Prevent duplicate pending KYC
        if (_currentUser.KycStatus == KycStatus.Pending)
            throw new InvalidOperationException("KYC is already under review.");
            if (_currentUser.KycStatus == KycStatus.Approved)
            throw new InvalidOperationException("KYC is already Approved");
        //if(command.DateOfBirth<=)

        var hasPending = await _kycRepository.HasPendingKycAsync(_currentUser.UserId, cancellationToken);
        if (hasPending)
            throw new InvalidOperationException("A pending KYC already exists.");
        // 3️⃣ Date of Birth validation

        var today = DateTime.UtcNow.Date;

        // ❌ Future DOB check
        if (command.DateOfBirth.Date > today)
            throw new  BusinessException(ErrorCodes.DobGreaterThanCurrentDate , ErrorMessages.DobGreaterThanCurrentDate);

        // 🔞 Minimum age check (18 years)
        var age = today.Year - command.DateOfBirth.Year;

        // Adjust if birthday hasn't occurred yet this year
        if (command.DateOfBirth.Date > today.AddYears(-age))
            age--;

        if (age < 18)
            throw new BusinessException(
                    ErrorCodes.DobAgeLessThan18,
                    ErrorMessages.DobAgeLessThan18);


        // 3️⃣ Load user aggregate
        var user = await _userRepository.GetByIdAsync(_currentUser.UserId, cancellationToken)
            ?? throw new InvalidOperationException("User not found.");

        // 4️⃣ Create immutable KYC record
        var kycRecord = KycRecord.Submit(
            user.Id,
            command.FullName,
            command.DateOfBirth,
            command.FullAddress,
            command.DocumentType,
            command.DocumentUrl,
            command.SelfieUrl
        );

        // 5️⃣ Update user KYC state
        user.UpdateKycStatus(KycStatus.Pending);

        // 6️⃣ Persist changes
        await _kycRepository.AddAsync(kycRecord, cancellationToken);
        await _userRepository.UpdateAsync(user, cancellationToken);
    }
}
