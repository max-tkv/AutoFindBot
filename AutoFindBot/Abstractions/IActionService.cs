using System.Linq.Expressions;
using AutoFindBot.Entities;

namespace AutoFindBot.Abstractions;

public interface IActionService
{
    Task<Entities.Action> GetLastByUserAsync(AppUser appUser);
    Task<Entities.Action> GetLastActionByCommandNameAsync(AppUser appUser, string commandName);
    Task<Entities.Action> GetLastByFilter(Expression<Func<Entities.Action, bool>> predicate);
    Task AddAsync(Entities.Action newAction);
}