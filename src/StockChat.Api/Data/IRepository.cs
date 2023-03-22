using System.Linq.Expressions;
using StockChat.Api.Models;

namespace StockChat.Api.Data;

public interface IRepository<T>
{
    Task<List<T>> GetFiltered(Expression<Func<T, bool>> filter);
    Task Register(T entity);
}