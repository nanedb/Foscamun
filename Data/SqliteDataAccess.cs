using Dapper;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foscamun2026.Models;

namespace Foscamun2026.Data
{
    public class SqliteDataAccess
    {
        private readonly string _connectionString = "Data Source=foscamun.db";

        public async Task<List<Committee>> GetCommitteesAsync()
        {
            using var connection = new SqliteConnection(_connectionString);

            var sql = @"SELECT CommID, Name, TopicA, TopicB, President, VicePresident, Moderator 
                        FROM Committees";

            var result = await connection.QueryAsync<Committee>(sql);
            return result.AsList();
        }

        public async Task AddCommitteeAsync(Committee committee)
        {
            using var connection = new SqliteConnection(_connectionString);

            var sql = @"INSERT INTO Committees 
                        (Name, TopicA, TopicB, President, VicePresident, Moderator) 
                        VALUES (@Name, @TopicA, @TopicB, @President, @VicePresident, @Moderator)";

            await connection.ExecuteAsync(sql, committee);
        }

        public async Task<List<VoteResult>> GetResultsAsync()
        {
            using var connection = new SqliteConnection(_connectionString);

            var sql = @"SELECT Id, CommitteeName, Topic, VotesFor, VotesAgainst, Abstentions 
                FROM Results";

            var result = await connection.QueryAsync<VoteResult>(sql);
            return result.AsList();
        }

    }
}