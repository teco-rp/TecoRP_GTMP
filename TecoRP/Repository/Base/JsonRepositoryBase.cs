using GrandTheftMultiplayer.Server.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TecoRP.Repository.Base
{
    public class JsonRepositoryBase<TModel> : IBaseRepository<TModel>
    {
        private IList<TModel> _current;

        public JsonRepositoryBase()
        {

        }
        public JsonRepositoryBase(string path)
        {
            Path = path;
        }

        public string Path { get; set; }

        public IList<TModel> Current
        {
            get
            {
                if (_current == null)
                    _current = ReadAll();
                return _current;
            }
        }

        public void Add(TModel model)
        {
            Current.Add(model);
            SaveChanges();
        }

        public Task AddAsync(TModel model)
        {
            Current.Add(model);
            return SaveChangesAsync();
        }

        public IList<TModel> Get(Func<TModel, bool> func = null)
        {
            if (func == null)
                return Current;
            return Current.Where(func).ToList();
        }

        public TModel GetSingle(Func<TModel, bool> func)
        {
            return Current.FirstOrDefault(func);
        }
        public IList<TModel> ReadAll()
        {
            if (!File.Exists(Path))
                if (!Directory.GetParent(Path).Exists)
                {
                    Directory.GetParent(Path).Create();
                    return new List<TModel>();
                }
            if (File.Exists(Path))
            {
                var json = File.ReadAllText(Path);
                return JsonConvert.DeserializeObject<List<TModel>>(json);
            }
            return new List<TModel>();
        }

        public void Remove(TModel value)
        {
            Current.Remove(value);
            SaveChanges();
        }

        public Task RemoveAsync(TModel value)
        {
            Current.Remove(value);
            return SaveChangesAsync();
        }

        public void Update(TModel value)
        {
            SaveChanges();
        }

        public Task UpdateAsync(TModel value)
        {
            return SaveChangesAsync();
        }

        public void SaveChanges()
        {
            API.shared.consoleOutput($"[REPO]  {typeof(TModel).Name} started saving...");
            var _directory = Directory.GetParent(Path);
            if (!_directory.Exists)
                _directory.Create();
            var json = JsonConvert.SerializeObject(Current);
            File.WriteAllText(Path, json);
            API.shared.consoleOutput($"[REPO] {Current.Count} {typeof(TModel).Name} datas saved successfully to {Path}.");
        }

        public Task SaveChangesAsync()
        {
            return Task.Run(() => SaveChanges());
        }
    }
}
