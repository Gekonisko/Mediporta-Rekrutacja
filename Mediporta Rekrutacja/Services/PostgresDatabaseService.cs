using Npgsql;

public class PostgresDatabaseService
{
    public readonly string ConnectionString = "Host=localhost;Username=admin;Password=password;Database=admin";
    private readonly ILogger<PostgresDatabaseService> _logger;


    public PostgresDatabaseService(ILogger<PostgresDatabaseService> logger)
    {
        _logger = logger;
    }

    public async Task CreateTagTable(List<StackOverflowTag> tags)
    {
        _logger.Log(LogLevel.Information, "CreateTagTable");
        try
        {
            using (var conn = NpgsqlDataSource.Create(ConnectionString))
            {
                await conn.OpenConnectionAsync();
                _logger.Log(LogLevel.Information, "Connected to database");

                var cmd = conn.CreateCommand("TRUNCATE TABLE tag;");
                await cmd.ExecuteNonQueryAsync();

                _logger.Log(LogLevel.Information, "TRUNCATE table tag");


                var id = 0;

                var task = new List<Task>();
                foreach (var tag in tags)
                {
                    cmd = conn.CreateCommand("INSERT INTO tag (id, name, count) VALUES (@id, @name, @count)");
                    cmd.Parameters.AddWithValue("id", id++);
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
            using (var conn = NpgsqlDataSource.Create(ConnectionString))
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
            using (var conn = NpgsqlDataSource.Create(ConnectionString))
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
}

public enum TagColumn
{
    id, name, count
}

public enum SortingType
{
    asc, desc
}