using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecoRP.Helpers;
using TecoRP.Models;

namespace TecoRP.Repository.Base
{
    public class JsonSingleDocRepositoryBase<TModel> : ISingleDocRepositoryBase<TModel> where TModel : ISingleDocBase
    {
        public JsonSingleDocRepositoryBase(string directoryPath)
        {
            DirectoryPath = directoryPath;
        }

        public string DirectoryPath { get; set; }


        public void Add(TModel value)
        {
            var path = System.IO.Path.Combine(DirectoryPath, value.UniqueName);
            if (File.Exists(path))
                throw new ArgumentException("This registry is already exist!");

            var dir = Directory.GetParent(path);
            if (!dir.Exists)
                dir.Create();
            File.WriteAllText(path, value.ToJson(true));
        }

        public Task AddAsync(TModel value)
        {
            return Task.Run(() => Add(value));
        }

        public IEnumerable<TModel> Get()
        {
            foreach (var item in Directory.EnumerateFiles(DirectoryPath))
            {
                yield return File.ReadAllText(item).FromJson<TModel>();
            }
        }

        public TModel GetSingle(string identifier)
        {
            var path = Path.Combine(DirectoryPath, identifier);
            if (!File.Exists(path))
                return default;
            return File.ReadAllText(path).FromJson<TModel>();
        }

        public void Remove(string identifier)
        {
            var path = Path.Combine(DirectoryPath, identifier);
            if (File.Exists(path))
                File.Delete(path);
        }

        public Task RemoveAsync(string identifier)
        {
            return Task.Run(() => Remove(identifier));
        }

        public void Update(TModel value)
        {
            var path = Path.Combine(DirectoryPath, value.UniqueName);
            if (!File.Exists(path))
                throw new ArgumentException("This isn't exist!");

            File.WriteAllText(path, value.ToJson(true));
        }

        public Task UpdateAsync(TModel value)
        {
            return Task.Run(() => Update(value));
        }
    }
}
