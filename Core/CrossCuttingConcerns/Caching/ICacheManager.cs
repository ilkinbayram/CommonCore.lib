namespace CommonCore.CrossCuttingConcerns.Caching
{
    public interface ICacheManager
    {
        Task<List<T>?> GetListAsync<T>(string key, int duration = 5, bool hasExpiration = false) where T : class;
        Task<T?> GetListItemAsync<T>(string key, int index, int duration = 5, bool hasExpiration = false) where T : class;
        Task AddListItemAsync<T>(string key, T data, int duration = 5, bool hasExpiration = true) where T : class;
        Task RemoveListItemAsync<T>(string key, T data, int duration = 5, bool hasExpiration = false) where T : class;
        Task TerminateListAsync(string key);
        Task<bool> KeyExistAsync(string key);

        List<T>? GetList<T>(string key, int duration = 5, bool hasExpiration = false) where T : class;
        T? GetListItem<T>(string key, int index, int duration = 5, bool hasExpiration = false) where T : class;
        void AddListItem<T>(string key, T data, int duration = 5, bool hasExpiration = true) where T : class;
        void RemoveListItem<T>(string key, T data, int duration = 5, bool hasExpiration = false) where T : class;
        void TerminateList(string key);
        bool KeyExist(string key);

        void Add(string key, object data, int duration = 60, bool forceToCache = false);
        T Get<T>(string key);
        object Get(string key);
        bool IsAdd(string key);
        void Remove(string key);
        void RemoveByPattern(string pattern);
    }
}
