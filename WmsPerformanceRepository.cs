using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Contracts.Dtos;
using Vertica.Data.VerticaClient;
using System.IO;

namespace Data
{
    public class WmsPerformanceRepository : IWmsPerformanceRepository
    {
        private readonly string _connectionString =
            ConfigurationManager.ConnectionStrings["HPVerticaDataContext"].ConnectionString;

        private readonly IOADbConnectionManager _dbConnectionManager;

        public WmsPerformanceRepository(IOADbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        public IEnumerable<WmsPerformanceTableDto> GetWmsPerformanceTableInfos(
            IEnumerable<string> ignoredColumnTypes = null)
        {
            var queryResult = new[] { new { TableName = string.Empty, ColumnName = string.Empty, DataType = string.Empty, IsNullable = true, LastUpdateDate = DateTime.MinValue } }.ToList();
            var queryLastDateResult = new[] { new { TableName = string.Empty, LastUpdateDate = DateTime.MinValue } }.ToList();
            const string verticaQuery = "SELECT table_name, column_name, data_type, is_nullable FROM columns WHERE table_schema = 'extr_manual' ORDER BY ordinal_position";

            using (var verticaConnection = new VerticaConnection(_connectionString))
            {
                try
                {
                    verticaConnection.Open();
                    using (var verticaCommand = new VerticaCommand(verticaQuery, verticaConnection))
                    {
                        var reader = verticaCommand.ExecuteReader();

                        while (reader.Read())
                        {
                            var columnType = reader[2].ToString();

                            //Skip the ignored column types.
                            if (ignoredColumnTypes != null && ignoredColumnTypes.Contains(columnType))
                            {
                                continue;
                            }

                            if (!queryLastDateResult.Any(x => x.TableName == reader[0].ToString()))
                            {
                                queryLastDateResult.Add(new
                                {
                                    TableName = reader[0].ToString(),
                                    LastUpdateDate = GetLastUpdateDateOfTable(reader[0].ToString(), verticaConnection)
                                });
                            }

                            queryResult.Add(new
                            {
                                TableName = reader[0].ToString(),
                                ColumnName = reader[1].ToString(),
                                DataType = columnType,
                                IsNullable = Convert.ToBoolean(reader[3]),
                                LastUpdateDate = queryLastDateResult.SingleOrDefault(x => x.TableName == reader[0].ToString()).LastUpdateDate
                            });
                        }
                    }
                }
                finally
                {
                    verticaConnection.Close();
                }
            }

            //Remove the initial dummy item.
            queryResult.RemoveAt(0);

            //Prepare the result by using anonymous query result type.
            var tables = queryResult.GroupBy(e => e.TableName).Select(e => new WmsPerformanceTableDto
            {
                TableName = e.Key,
                LastUpdateDate = e.FirstOrDefault(c => c.TableName == e.Key).LastUpdateDate,
                Columns = e.Select(c => new WmsPerformanceColumnDto
                {
                    ColumnName = c.ColumnName,
                    DataType = c.DataType,
                    IsNullable = c.IsNullable,
                }).ToList()
            }).ToList();

            return tables;
        }

        public void TruncateWmsPerformanceTable(string tableName)
        {
            var verticaQuery = "DELETE FROM extr_manual." + tableName;

            using (var verticaConnection = new VerticaConnection(_connectionString))
            {
                try
                {
                    verticaConnection.Open();
                    using (var verticaTransaction = verticaConnection.BeginTransaction())
                    {
                        try
                        {
                            using (var verticaCommand = new VerticaCommand(verticaQuery, verticaConnection))
                            {
                                verticaCommand.ExecuteNonQuery();
                                verticaTransaction.Commit();
                            }
                        }
                        catch (Exception)
                        {
                            verticaTransaction.Rollback();

                            throw;
                        }
                    }
                }
                finally
                {
                    verticaConnection.Close();
                }
            }
        }

        public IList<long> BulkInsert(string fullFilePath, string tableName)
        {
            IList<long> rejectedRowNumbers = null;
            using (var verticaConnection = new VerticaConnection(_connectionString))
            {
                try
                {
                    verticaConnection.Open();
                    using (var verticaTransaction = verticaConnection.BeginTransaction())
                    {
                        try
                        {
                            FileStream inputFileStream = File.OpenRead(fullFilePath);
                            string copy = @"COPY extr_manual." + tableName + @" FROM STDIN RECORD TERMINATOR E'\r\n' DELIMITER E'|' ENFORCELENGTH NO COMMIT";

                            VerticaCopyStream stream = new VerticaCopyStream(verticaConnection, copy);
                            stream.Start();
                            stream.AddStream(inputFileStream, false);
                            stream.Execute();
                            long insertedRowsCount = stream.Finish();
                            rejectedRowNumbers = stream.Rejects;

                            if (rejectedRowNumbers.Count == 0)
                                verticaTransaction.Commit();
                            else
                                verticaTransaction.Rollback();
                        }
                        catch (Exception ex)
                        {
                            verticaTransaction.Rollback();

                            throw;
                        }
                    }
                }
                finally
                {
                    verticaConnection.Close();
                }
            }
            return rejectedRowNumbers;
        }

        private DateTime GetLastUpdateDateOfTable(string tableName, VerticaConnection verticaConnection)
        {
            DateTime result = DateTime.MinValue;
            string verticaTableLastUpdateQuery =
              string.Format("SELECT MAX(insert_date) AS LastUpdateDate FROM extr_manual.{0}", tableName);

            using (var verticaTableLastUpdateCommand = new VerticaCommand(verticaTableLastUpdateQuery, verticaConnection))
            {
                var verticaTableLastUpdateReader = verticaTableLastUpdateCommand.ExecuteReader();

                while (verticaTableLastUpdateReader.Read())
                { 
                    DateTime.TryParse(verticaTableLastUpdateReader[0].ToString(), out result);
                }
            }

            return result;
        }
    }
}