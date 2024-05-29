using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using CustomersApi.DataAccess;
using CustomersApi.Models;
using Microsoft.Data.SqlClient;
using CustomersApi.IRepository;
using Microsoft.AspNetCore.Cors;

namespace CustomersApi.Repositories
{
    public class BeneficiariesRepository : BaseDataAccess, IBeneficiariesRepository
    {
        public BeneficiariesRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<Beneficiaries> AddBeneficiaryAsync(Beneficiaries beneficiary)
        {
            var parameters = new[]
            {
        new SqlParameter("@BenefName", beneficiary.BenefName),
        new SqlParameter("@BenefAccount", beneficiary.BenefAccount),
        new SqlParameter("@BenefIFSC", beneficiary.IFSC),
        new SqlParameter("@CustomerId", beneficiary.CustomerId),
        new SqlParameter("@BenefAccType", beneficiary.BenefAccType),
        new SqlParameter("@NewBenefId", SqlDbType.Int) { Direction = ParameterDirection.Output }
    };

            try
            {
                OpenConnection(); // Using inherited method to open the connection
                using (var command = _connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "sp_AddBeneficiary";
                    command.Parameters.AddRange(parameters);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                beneficiary.BenefId = reader.GetInt32(reader.GetOrdinal("BenefID"));
                                beneficiary.BenefName = reader.GetString(reader.GetOrdinal("BenefName"));
                                beneficiary.BenefAccount = reader.GetInt64(reader.GetOrdinal("BenefAccount"));
                                beneficiary.IFSC = reader.GetString(reader.GetOrdinal("BenefIFSC"));
                                beneficiary.isActive = true; // Since isActive is always 1
                                beneficiary.CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"));
                                beneficiary.BenefAccType = reader.GetInt32(reader.GetOrdinal("BenefAccType"));
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                return beneficiary;
            }
            catch (SqlException sqlEx)
            {
                // Log the exception (consider using a logging framework)
                // For example: _logger.LogError(sqlEx, "A SQL error occurred while adding a beneficiary.");
                throw new Exception("A database error occurred while adding the beneficiary.", sqlEx);
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                // For example: _logger.LogError(ex, "An error occurred while adding a beneficiary.");
                throw new Exception("An error occurred while adding the beneficiary.", ex);
            }
            finally
            {
                CloseConnection();
            }
        }
        public async Task<Beneficiaries> DeleteBeneficiaryByAccountIdAsync(long benefAccount)
        {
            try
            {
                OpenConnection();
                using (var command = _connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "sp_DeleteBeneficiariesByAccId";
                    command.Parameters.AddWithValue("@BenefAccount", benefAccount);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read() && reader["BenefID"] != DBNull.Value)
                        {
                            return new Beneficiaries
                            {
                                BenefAccount = benefAccount,
                                BenefId = reader.GetInt32(reader.GetOrdinal("BenefID"))
                            };
                        }
                        return null;  // Return null if no beneficiary was found or deactivated
                    }
                }
            }
            finally
            {
                CloseConnection();
            }
        }
        public async Task<IEnumerable<Beneficiaries>> GetBeneficiariesByCustomerIdAsync(int customerId)
        {
            var parameters = new[]
            {
                new SqlParameter("@CustomerId", customerId)
            };

            var beneficiaries = new List<Beneficiaries>();

            try
            {
                OpenConnection(); // Using inherited method to open the connection
                using (var command = _connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "sp_GetBeneficiariesByCustomerId";
                    command.Parameters.AddRange(parameters);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var beneficiary = new Beneficiaries
                            {
                                BenefName = reader["BenefName"].ToString(),
                                BenefAccount = reader["BenefAccount"] as long?,
                                IFSC = reader["BenefIFSC"].ToString(),
                                isActive = (bool)reader["isActive"],
                                CustomerId = (int)reader["CustomerId"],
                                BenefAccType = (int)reader["BenefAccType"]
                            };
                            beneficiaries.Add(beneficiary);
                        }
                    }
                }
                return beneficiaries;
            }
            finally
            {
                CloseConnection();
            }
        }

    }
}