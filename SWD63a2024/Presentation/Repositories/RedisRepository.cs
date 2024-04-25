using StackExchange.Redis;

namespace Presentation.Repositories
{
    public class RedisRepository
    {
        private IDatabase myDb;

        public RedisRepository(string password) 
        {
            //127.0.0.1:6379 - for local connections
            ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect($"redis-12879.c329.us-east4-1.gce.redns.redis-cloud.com:12879,password={password}");

            myDb = connectionMultiplexer.GetDatabase();
        }

        public void IncrementCounterInfo()
        {
            // If variable is a list of menus
            // First you serialize it JsonSerializer

            // Menu >> id, title, url, order
            int num = GetCounterInfo();
            num++;

            myDb.StringSet("counter", num);
        }

        public int GetCounterInfo()
        {
            // In home assignment, if you are retrieving a list of menus. it will be given as a json string
            // hence you must JsonSerializer.Deserialize<List<Menu>>("menus");
            var counter = myDb.StringGet("counter");

            if (string.IsNullOrEmpty(counter) == true)
            {
                return 0;
            }
            else
            {
                return (int)counter;
            }
        }

    }
}
