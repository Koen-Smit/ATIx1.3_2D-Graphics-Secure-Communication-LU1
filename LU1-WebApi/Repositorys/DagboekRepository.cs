using Dapper;
using Microsoft.Data.SqlClient;

public class DagboekRepository : IDagboekRepository
{
    private readonly string _connectionString;

    public DagboekRepository(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<Result?> CreateDagboek(DagboekDTO dagboek, Guid userId)
    {
        if (userId == Guid.Empty)
            return Result.Failure("No authenticated user found.");

        var dagboekId = Guid.NewGuid();

        const string insertDagboekQuery = @"
        INSERT INTO DiaryEntries (Id, UserId, Note, Timestamp)
        VALUES (@Id, @UserId, @Note, @Timestamp)";
        using var connection = new SqlConnection(_connectionString);

        await connection.ExecuteAsync(insertDagboekQuery, new
        {
            Id = dagboekId,
            UserId = userId,
            Note = dagboek.Note,
            Timestamp = DateTime.UtcNow
        });

        return Result.Success("Diary entry successfully created!");
    }


    public async Task<IEnumerable<DagboekDTO>?> GetDagboekenByUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            return null;

        const string selectDagboekenQuery = @"
        SELECT CAST(UserId AS UNIQUEIDENTIFIER) AS UserId, Note, Timestamp
        FROM DiaryEntries
        WHERE UserId = @UserId
        ORDER BY Timestamp DESC";

        using var connection = new SqlConnection(_connectionString);
        var dagboeken = await connection.QueryAsync<DagboekDTO>(selectDagboekenQuery, new { UserId = userId });

        return dagboeken;
    }

}

