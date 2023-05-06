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
    
    public async Task<Entities.Action> GetLastByFilter(Expression<Func<Entities.Action, bool>> predicate) =>
        await _actionRepository.GetLastByFilterAsync(predicate);

    public async Task<Entities.Action> GetLastActionByCommandNameAsync(AppUser appUser, string commandName)
    {
        return commandName switch
        {
            //CommandNames.FindFilmsCommand => await _unitOfWork.Actions.GetLastQueryFilmAsync(appUser),
            _ => throw new ArgumentOutOfRangeException(nameof(commandName), commandName, null)
        };
    }

    public async Task AddAsync(Entities.Action newAction)
    {
        await _actionRepository.AddAsync(newAction);
    }
}