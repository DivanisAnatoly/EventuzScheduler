using Dapper;
using EventuzScheduler.Application.Dapper;
using EventuzScheduler.Application.Enums;
using EventuzScheduler.Core.Entities;
using System.Data;
using System.Data.SqlClient;

namespace EventuzScheduler.Infrastructure.Database
{
    public class SchedulerTaskInfoRepository : ISchedulerTaskInfoRepository
    {
        private readonly IDbConnection _dbConnection;
        public SchedulerTaskInfoRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public Task<int> AddAsync(TaskInfo entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TaskInfo>> GetAllAsync()
        {
            using var connection = new SqlConnection(_dbConnection.ConnectionString);
            var sql = $"SELECT * FROM TaskInfo WHERE Status <> {(int)SchedulerTaskStatus.Deleted}";

            var result = await connection.QueryAsync<TaskInfo>(sql);

            return result;
        }

        public Task<TaskInfo?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(TaskInfo entity)
        {
            throw new NotImplementedException();
        }
    }
}
