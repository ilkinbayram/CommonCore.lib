using CommonCore.CrossCuttingConcerns.Caching;
using Core.CrossCuttingConcerns.Caching.Redis.Config;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace Core.CrossCuttingConcerns.Caching.Redis
{
    public class RedisCacheManager : ICacheManager
    {
        private readonly string _redisHost;
        private readonly string _redisPort;
        private readonly RedisCacheConfig _config;
        private ConnectionMultiplexer _redis;

        public RedisCacheManager(IOptions<RedisCacheConfig> config)
        {
            _config = config.Value;

            _redisHost = _config.Host;
            _redisPort = _config.Port;

            Connect();
        }

        protected IDatabase DB => GetDb(0);

        private void Connect()
        {
            var configString = $"{_redisHost}:{_redisPort}";

            _redis = ConnectionMultiplexer.Connect(configString);
        }

        private IDatabase GetDb(int db)
        {
            return _redis.GetDatabase(db);
        }

        public async Task AddListItemAsync<T>(string key, T data, int duration = 60, bool hasExpiration = true) where T : class
        {
            var mapObj = JsonSerializer.Serialize(data);
            var db = this.DB;
            await db.ListRightPushAsync(key, mapObj);
            if (hasExpiration)
            {
                await db.KeyExpireAsync(key, TimeSpan.FromSeconds(duration));
            }
        }

        public async Task RemoveListItemAsync<T>(string key, T data, int duration = 60, bool hasExpiration = false) 
            where T : class
        {
            var mapObj = JsonSerializer.Serialize(data);
            var db = this.DB;
            bool checkExist = await db.KeyExistsAsync(key);
            if (checkExist)
            {
                await db.ListRemoveAsync(key, mapObj);
            }
            
            if (hasExpiration)
            {
                await db.KeyExpireAsync(key, TimeSpan.FromSeconds(duration));
            }
        }

        public void RemoveListItem<T>(string key, T data, int duration = 60, bool hasExpiration = false)
            where T : class
        {
            var mapObj = JsonSerializer.Serialize(data);
            var db = this.DB;
            bool checkExist = db.KeyExists(key);
            if (checkExist)
            {
                db.ListRemove(key, mapObj);
            }

            if (hasExpiration)
            {
                db.KeyExpire(key, TimeSpan.FromSeconds(duration));
            }
        }

        public async Task TerminateListAsync(string key)
        {
            var db = this.DB;
            bool checkExist = await db.KeyExistsAsync(key);
            if (checkExist)
            {
                await db.KeyDeleteAsync(key);
            }
        }

        public void TerminateList(string key)
        {
            var db = this.DB;
            bool checkExist = db.KeyExists(key);
            if (checkExist)
            {
                db.KeyDelete(key);
            }
        }

        public async Task<List<T>?> GetListAsync<T>(string key, int duration = 60, bool hasExpiration = false) where T : class 
        {
            try
            {
                var db = this.DB;
                bool checkKey = await db.KeyExistsAsync(key);

                if (checkKey)
                {
                    var result = (await db.ListRangeAsync(key)).ToList().Select(x=>JsonSerializer.Deserialize<T>(x)).ToList();

                    if (hasExpiration)
                    {
                        await db.KeyExpireAsync(key, TimeSpan.FromSeconds(duration));
                    }

                    return result;
                }
            }
            catch (Exception)
            {
                throw new Exception("Internal Server Error: Key or index is not found or incorrect when operation is proceeding");
            }

            return null;
        }

        public async Task<T?> GetListItemAsync<T>(string key, int index, int duration = 60, bool hasExpiration = false) where T : class
        {
            try
            {
                var db = this.DB;
                bool checkKey = await db.KeyExistsAsync(key);

                if (checkKey)
                {
                    var data = await db.ListGetByIndexAsync(key, index);
                    T result = JsonSerializer.Deserialize<T>(data);

                    if (hasExpiration)
                    {
                        await db.KeyExpireAsync(key, TimeSpan.FromSeconds(duration));
                    }

                    return result;
                }
            }
            catch (Exception)
            {
                throw new Exception("Internal Server Error: Key or index is not found or incorrect when operation is proceeding");
            }

            return null;
        }



        public void AddListItem<T>(string key, T data, int duration = 60, bool hasExpiration = true) where T : class
        {
            var mapObj = JsonSerializer.Serialize(data);
            var db = this.DB;
            db.ListRightPush(key, mapObj);
            if (hasExpiration)
            {
                db.KeyExpire(key, TimeSpan.FromSeconds(duration));
            }
        }

        public List<T>? GetList<T>(string key, int duration = 60, bool hasExpiration = false) where T : class
        {
            try
            {
                var db = this.DB;
                bool checkKey = db.KeyExists(key);

                if (checkKey)
                {
                    var result = db.ListRange(key).ToList().Select(x => JsonSerializer.Deserialize<T>(x)).ToList();

                    if (hasExpiration)
                    {
                        db.KeyExpire(key, TimeSpan.FromSeconds(duration));
                    }

                    return result;
                }
            }
            catch (Exception)
            {
                throw new Exception("Internal Server Error: Key or index is not found or incorrect when operation is proceeding");
            }

            return null;
        }

        public T? GetListItem<T>(string key, int index, int duration = 60, bool hasExpiration = false) where T : class
        {
            try
            {
                var db = this.DB;
                bool checkKey = db.KeyExists(key);

                if (checkKey)
                {
                    var data = db.ListGetByIndex(key, index);
                    T result = JsonSerializer.Deserialize<T>(data);

                    if (hasExpiration)
                    {
                        db.KeyExpire(key, TimeSpan.FromSeconds(duration));
                    }

                    return result;
                }
            }
            catch (Exception)
            {
                throw new Exception("Internal Server Error: Key or index is not found or incorrect when operation is proceeding");
            }

            return null;
        }

        public async Task<bool> KeyExistAsync(string key)
        {
            return await this.DB.KeyExistsAsync(key);
        }

        public bool KeyExist(string key)
        {
            return this.DB.KeyExists(key);
        }



        public object Get(string key)
        {
            var db = this.DB;
            if (!db.KeyExists(key)) return null;

            return db.StringGet(key);
        }

        public bool IsAdd(string key)
        {
            return this.DB.KeyExists(key);
        }

        public void Remove(string key)
        {
            this.DB.KeyDelete(key);
        }

        public void RemoveByPattern(string pattern)
        {
            var db = this.DB;
            // Tüm anahtarları desenle eşleştirelim.
            // NOT: Büyük veritabanları için "KEYS" yerine "SCAN" komutunu kullanmayı düşünün.
            var script = @"
    local keys = redis.call('KEYS', ARGV[1])
    for i=1,#keys,5000 do
        redis.call('del', unpack(keys, i, math.min(i+4999, #keys)))
    end
    return keys
    ";

            db.ScriptEvaluate(script, values: new RedisValue[] { pattern });
        }

        public T Get<T>(string key)
        {
            try
            {
                var db = this.DB;

                var value = db.StringGet(key);
                return JsonSerializer.Deserialize<T>(value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Add(string key, object data, int duration = 60, bool forceToCache = false)
        {
            var db = this.DB;

            if (forceToCache)
            {
                db.KeyDelete(key);
                db.StringSet(key, JsonSerializer.Serialize(data));
                db.KeyExpire(key, TimeSpan.FromSeconds(duration));
                return;
            }

            if (!db.KeyExists(key))
            {
                db.StringSet(key, JsonSerializer.Serialize(data));
                db.KeyExpire(key, TimeSpan.FromSeconds(duration));
            }
        }
    }
}
