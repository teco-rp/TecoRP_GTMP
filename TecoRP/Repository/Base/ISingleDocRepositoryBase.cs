using System.Collections.Generic;
using System.Threading.Tasks;
using TecoRP.Models;

namespace TecoRP.Repository.Base
{
    public interface ISingleDocRepositoryBase<TModel> where TModel : ISingleDocBase
    {
        string DirectoryPath { get; set; }

        void Add(TModel value);
        Task AddAsync(TModel value);
        IEnumerable<TModel> Get();
        TModel GetSingle(string identifier);
        void Remove(string identifier);
        Task RemoveAsync(string identifier);
        void Update(TModel value);
        Task UpdateAsync(TModel value);
    }
}