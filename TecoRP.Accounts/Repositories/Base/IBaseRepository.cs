using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TecoRP.Accounts.Repository.Base
{
    public interface IBaseRepository<TModel>
    {
        string Path { get; set; }

        IList<TModel> Current { get; }

        IList<TModel> Get(Func<TModel, bool> func = null);

        TModel GetSingle(Func<TModel, bool> func);

        void Add(TModel value);
        Task AddAsync(TModel value);

        void Update(TModel value);
        Task UpdateAsync(TModel value);

        void Remove(TModel value);
        Task RemoveAsync(TModel value);

        void SaveChanges();
        Task SaveChangesAsync();
    }
}