using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Rentify.Core.Entities;
using Rentify.DataAccess.Core.Repositories;
using Rentify.DataAccess.SqlServer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.DataAccess.SqlServer.Repositories
{
    // Using ADO.NET to bypass DbContext (EF core) setup as this method will be used by middleware before user context / data scope is setup
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private const string GET_BY_REFERENCE = "SELECT Id, ReferenceId, OwnerUserId, CreatedDate, ModifiedDate FROM Subscriptions WHERE ReferenceId = @ReferenceId";
        private const string GET_BY_OWNER_USER_ID = "SELECT Id, ReferenceId, OwnerUserId, CreatedDate, ModifiedDate FROM Subscriptions WHERE OwnerUserId = @OwnerUserId";


        private readonly string _connString;

        public SubscriptionRepository(string connectionString)
        {
            _connString = connectionString;
        }

        public async Task<Subscription?> GetByReferenceId(Guid referenceId)
        {
            using(var conn = new SqlConnection(_connString))
            {
                using(var cmd = new SqlCommand(GET_BY_REFERENCE, conn))
                {
                    cmd.Parameters.AddWithValue("@ReferenceId", referenceId);

                    conn.Open();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return Fill(reader);
                        } else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public async Task<Subscription?> GetByUserId(int userId)
        {
            using (var conn = new SqlConnection(_connString))
            {
                using (var cmd = new SqlCommand(GET_BY_OWNER_USER_ID, conn))
                {
                    cmd.Parameters.AddWithValue("@OwnerUserId", userId);

                    conn.Open();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return Fill(reader);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        private Subscription Fill(SqlDataReader reader)
        {
            int idOrdinal = reader.GetOrdinal("Id");
            int referenceIdOrdinal = reader.GetOrdinal("ReferenceId");
            int ownerUserIdOrdinal = reader.GetOrdinal("OwnerUserId");
            int createdDateOrdinal = reader.GetOrdinal("CreatedDate");
            int modifiedDateOrdinal = reader.GetOrdinal("ModifiedDate");

            return new Subscription(
                reader.IsDBNull(idOrdinal) ? 0 : reader.GetInt32(idOrdinal),
                reader.IsDBNull(referenceIdOrdinal) ? Guid.Empty : reader.GetGuid(referenceIdOrdinal),
                reader.IsDBNull(ownerUserIdOrdinal) ? 0 : reader.GetInt32(ownerUserIdOrdinal),
                reader.IsDBNull(createdDateOrdinal) ? DateTime.MinValue : reader.GetDateTime(createdDateOrdinal),
                reader.IsDBNull(modifiedDateOrdinal) ? null : reader.GetDateTime(modifiedDateOrdinal)
            );
        }
    }
}
