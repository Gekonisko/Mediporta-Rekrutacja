using Npgsql;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

public class PostgresDatabaseService : IDatabaseService
{
    private readonly ILogger<PostgresDatabaseService> _logger;
    private readonly DatabaseSettings _databaseSettings = new DatabaseSettings();
    private readonly IConfiguration _configuration;

    public PostgresDatabaseService(IConfiguration configuration, ILogger<PostgresDatabaseService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        InitDatabaseSettings(_configuration);
    }

    public async Task<int> GetSizeOfTagTable()
    {
        _logger.Log(LogLevel.Information, "GetSizeOfTagTable");

        try
        {
            using (var conn = NpgsqlDataSource.Create(GetConnectionUrl()))
            {
                await conn.OpenConnectionAsync();
                var cmd = conn.CreateCommand("select count(id) from tag");
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32(0);
                    }
                }
            }
        }catch
        {
            _logger.LogError("Failed to get size of table Tag");
            throw new Exception("Failed to get size of table Tag");
        }
        return 0;
    }

    public async Task FillTagTable(List<StackOverflowTag> tags, int startId)
    {
        _logger.Log(LogLevel.Information, "FillTagTable");
        try
        {
            using (var conn = NpgsqlDataSource.Create(GetConnectionUrl()))
            {
                await conn.OpenConnectionAsync();
                _logger.Log(LogLevel.Information, "Connected to database");

                var task = new List<Task>();
                foreach (var tag in tags)
                {
                    var cmd = conn.CreateCommand("INSERT INTO tag (id, name, count) VALUES (@id, @name, @count)");
                    cmd.Parameters.AddWithValue("id", startId++);
                    cmd.Parameters.AddWithValue("name", tag.Name);
                    cmd.Parameters.AddWithValue("count", tag.Count);
                    task.Add(cmd.ExecuteNonQueryAsync());
                }
    
                await Task.WhenAll(task);
            }
        }
        catch
        {
            _logger.LogError("Failed to create table Tag");
            throw new Exception("Failed to create table Tag");
        }
    }

    public List<PercentageOfTags> GetPercentageOfTags(int page, int size)
    {
        _logger.Log(LogLevel.Information, $"GetPercentageOfTags page={page} size={size}");

        List<PercentageOfTags> tags = [];
        try
        {
            using (var conn = NpgsqlDataSource.Create(GetConnectionUrl()))
            {
                conn.OpenConnectionAsync();
                _logger.Log(LogLevel.Information, "Connected to database");

                var min = (page - 1) * size;
                var max = min + size;

                var command = conn.CreateCommand("select name, count, ROUND((count * 100.0 / (select sum(count) from tag)), 2) from tag where id >= @minId and id < @maxId;");
                command.Parameters.AddWithValue("minId", min);
                command.Parameters.AddWithValue("maxId", max);

                Console.WriteLine(command.CommandText);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tags.Add(new PercentageOfTags() { Name = reader.GetString(0), Count = reader.GetInt32(1), Percentage = reader.GetDouble(2) });
                    }
                }
            }
        }
        catch
        {
            _logger.LogError("Failed to retrieve tags from database");
            throw new Exception("Failed to retrieve tags from database");
        }

        return tags;
    }

    public List<StackOverflowTag> GetTags(int page, int size, TagColumn sort = TagColumn.id, SortingType direction = SortingType.asc)
    {
        _logger.Log(LogLevel.Information, $"GetTags page={page} size={size} sort={sort} direction={direction}");

        List<StackOverflowTag> tags = [];
        try
        {
            using (var conn = NpgsqlDataSource.Create(GetConnectionUrl()))
            {
                conn.OpenConnectionAsync();

                var min = (page - 1) * size;
                var max = min + size;

                var command = conn.CreateCommand("select * from tag where id >= @minId and id < @maxId order by " + sort.ToString() + " " + direction.ToString());
                command.Parameters.AddWithValue("minId", min);
                command.Parameters.AddWithValue("maxId", max);

                Console.WriteLine(command.CommandText);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tags.Add(new StackOverflowTag() { Name = reader.GetString(1), Count = reader.GetInt32(2) });
                    }
                }
            }
        }
        catch
        {
            _logger.LogError("Failed to retrieve tags from database");
            throw new Exception("Failed to retrieve tags from database");
        }

        return tags;
    }

    private string GetConnectionUrl()
    {
        return $"Host={_databaseSettings.Server};Username={_databaseSettings.Username};Password={_databaseSettings.Password};Database={_databaseSettings.Database}";
    }
    private void InitDatabaseSettings(IConfiguration configuration)
    {
        _databaseSettings.Server = configuration["POSTGRES_HOST"];
        _databaseSettings.Database = configuration["POSTGRES_DATABASE"];
        _databaseSettings.Username = configuration["POSTGRES_USERNAME"];
        _databaseSettings.Password = configuration["POSTGRES_PASSWORD"];
    }
}

public enum TagColumn
{
    id, name, count
}

public enum SortingType
{
    asc, desc
}