using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

public class PostgresDatabaseService
{
    public readonly string ConnectionString = "Host=localhost;Username=admin;Password=password;Database=admin";

    public PostgresDatabaseService() { }

    public async Task CreateTagTable(List<StackOverflowTag> tags)
    {
        try
        {
            using (var conn = NpgsqlDataSource.Create(ConnectionString))
            {
                await conn.OpenConnectionAsync();

                var cmd = conn.CreateCommand("TRUNCATE TABLE tag;");
                await cmd.ExecuteNonQueryAsync();

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
        catch (Exception ex)
        {
            throw new Exception($"Failed to retrieve tags {ex.Message}");
        }
    }

    public List<PercentageOfTags> GetPercentageOfTags(int page, int pageSize)
    {
        List<PercentageOfTags> tags = [];

        try
        {
            using (var conn = NpgsqlDataSource.Create(ConnectionString))
            {
                conn.OpenConnectionAsync();

                var min = (page - 1) * pageSize;
                var max = min + pageSize;

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
        catch (Exception ex)
        {
            throw new Exception($"Failed to retrieve tags {ex.Message}");
        }

        return tags;
    }

    public List<StackOverflowTag> GetTags(int page, int pageSize, TagColumn sortByColumn = TagColumn.id, SortingType sortingType = SortingType.asc)
    {
        List<StackOverflowTag> tags = [];
        try
        {
            using (var conn = NpgsqlDataSource.Create(ConnectionString))
            {
                conn.OpenConnectionAsync();

                var min = (page - 1) * pageSize;
                var max = min + pageSize;

                var command = conn.CreateCommand("select * from tag where id >= @minId and id < @maxId order by " + sortByColumn.ToString() + " " + sortingType.ToString());
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
        catch (Exception ex)
        {
            throw new Exception($"Failed to retrieve tags {ex.Message}");
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