using System.Linq.Expressions;
using AutoFindBot.Entities;
using Action = AutoFindBot.Entities.Action;

namespace AutoFindBot.Repositories;

public interface IActionRepository : IRepository<Entities.Action>
{
    Task<Entities.Action> AddAsync(Entities.Action action);
    Task<Entities.Action> GetLastAsync(long id);
    Task<Entities.Action> GetLastByUserAsync(AppUser user);
    Task<Entities.Action> GetLastByUserAsync(AppUser user, string command);
    Task<Entities.Action> GetLastByFilterAsync(Expression<Func<Action,bool>> predicate);
    Task<int> GetNumberOfRequestsByUserAsync(AppUser user);
}