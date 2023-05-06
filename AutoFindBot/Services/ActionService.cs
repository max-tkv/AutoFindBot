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

    public async Task<Entities.Action> GetLastByUserAsync(AppUser appUser) =>
        await _actionRepository.GetLastByUserAsync(appUser);

    public async Task AddAsync(Entities.Action newAction)
    {
        await _actionRepository.AddAsync(newAction);
    }
}