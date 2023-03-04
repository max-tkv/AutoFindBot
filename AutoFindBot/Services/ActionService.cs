using System.Linq.Expressions;
using AutoFindBot.Abstractions;
using AutoFindBot.Entities;

namespace AutoFindBot.Services;

public class ActionService : IActionService
{
    private readonly IUnitOfWork _unitOfWork;

    public ActionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Entities.Action> GetLastByUserAsync(AppUser appUser) =>
        await _unitOfWork.Actions.GetLastByUserAsync(appUser);
    
    public async Task<Entities.Action> GetLastByFilter(Expression<Func<Entities.Action, bool>> predicate) =>
        await _unitOfWork.Actions.GetLastByFilterAsync(predicate);

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
        await _unitOfWork.Actions.AddAsync(newAction);
        await _unitOfWork.SaveChangesAsync();
    }
}