using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

public class PostgresDatabaseService
{
    public readonly string ConnectionString = "Host=localhost;Username=admin;Password=password;Database=admin";

    public PostgresDatabaseService() { }

    public List<StackOverflowTag> Tags { get; set; }

    public void AddTags(IEnumerable<StackOverflowTag> tags)
    {
        Tags?.Clear();
        Tags = tags.ToList();
    }

    public async Task SaveChangesAsync()
    {
        using (var conn = NpgsqlDataSource.Create(ConnectionString))
        {
            await conn.OpenConnectionAsync();

            var truncateCommand = conn.CreateCommand("TRUNCATE TABLE tag;");
            await truncateCommand.ExecuteNonQueryAsync();

            var id = 0;

            var insertTasks = new List<Task>();
            foreach (var tag in Tags)
            {
                var insertCommand = conn.CreateCommand("INSERT INTO tag (id, name, count) VALUES (@id, @name, @count)");
                insertCommand.Parameters.AddWithValue("id", id++);
                insertCommand.Parameters.AddWithValue("name", tag.Name);
                insertCommand.Parameters.AddWithValue("count", tag.Count);
                insertTasks.Add(insertCommand.ExecuteNonQueryAsync());
            }

            await Task.WhenAll(insertTasks);

            await conn.DisposeAsync();
        }
    }

    public List<PercentageOfTags> GetPercentageOfTags(int page, int pageSize)
    {
        List<PercentageOfTags> tags = [];

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
                    tags.Add(new PercentageOfTags() { Name = reader.GetString(0), Count = reader.GetInt32(1), Percentage = reader.GetDouble(2)});
                }
            }
            conn.DisposeAsync();
        }

        return tags;
    }

    public List<StackOverflowTag> GetTags(int page, int pageSize, TagColumn sortByColumn, SortingType sortingType)
    {
        List<StackOverflowTag> tags = [];

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
            conn.DisposeAsync();
        }

        return tags;
    }

    public enum TagColumn
    {
        id, name, count
    }

    public enum SortingType
    {
        asc, desc
    }
}
