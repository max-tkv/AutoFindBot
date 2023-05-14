using System.Linq.Expressions;
using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using AutoFindBot.Repositories;

namespace AutoFindBot.Services;

public class ActionService : IActionService
{
    private readonly IActionRepository _actionRepository;

    public ActionService(IActionRepository actionRepository)
    {
        _actionRepository = actionRepository;
    }

    public async Task<Entities.Action?> GetLastByUserAsync(
        AppUser appUser, 
        CancellationToken stoppingToken = default) =>
        await _actionRepository.GetLastByUserAsync(appUser, stoppingToken);

    public async Task AddAsync(
        Entities.Action newAction, 
        CancellationToken stoppingToken = default)
    {
        await _actionRepository.AddAsync(newAction, stoppingToken);
    }
}