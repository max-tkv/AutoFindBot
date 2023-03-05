using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using Microsoft.Extensions.Logging;

namespace AutoFindBot.Services;

public class UserFilterService : IUserFilterService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserFilterService> _logger;

    public UserFilterService(
        IUnitOfWork unitOfWork,
        ILogger<UserFilterService> logger)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<List<UserFilter>> GetByUserAsync(AppUser appUser)
    {
        return await _unitOfWork.UserFilters.GetByUserAsync(appUser);
    }
}