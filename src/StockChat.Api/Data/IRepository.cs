using System.Linq.Expressions;
using StockChat.Api.Models;

namespace StockChat.Api.Data;

public interface IRepository<T> where T : IEntity
{
    Task<List<T>> GetAll();
    Task<List<T>> GetFiltered(Expression<Func<T, bool>> filter);
    Task<string> Register(T entity);
}