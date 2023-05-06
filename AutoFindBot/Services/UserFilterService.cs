using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using AutoFindBot.Repositories;
using Microsoft.Extensions.Logging;

namespace AutoFindBot.Services;

public class UserFilterService : IUserFilterService
{
    private readonly IUserFilterRepository _userFilterRepository;
    private readonly ILogger<UserFilterService> _logger;

    public UserFilterService(
        IUserFilterRepository userFilterRepository,
        ILogger<UserFilterService> logger)
    {
        _logger = logger;
        _userFilterRepository = userFilterRepository;
    }
    
    public async Task<List<UserFilter>> GetByUserAsync(AppUser appUser)
    {
        return await _userFilterRepository.GetByUserAsync(appUser);
    }
}