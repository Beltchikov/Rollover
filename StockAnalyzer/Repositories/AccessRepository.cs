using StockAnalyzer.DataProviders;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace StockAnalyzer.Repositories
{
    public class AccessRepository : IRepository
    {
        private readonly string _connectionString;

        public AccessRepository()
        {
            string databasePath = "C:\\r\\Rollover\\StockAnalyzer\\Repositories\\Bel.accdb";
            _connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={databasePath};Persist Security Info=False;";
        }

        //public List<string> Get(List<RepoDataDescriptor> repoDataDescriptors)
        //{
        //    var results = new List<string>();

        //    using (var connection = new OleDbConnection(_connectionString))
        //    {
        //        connection.Open();
        //        foreach (var descriptor in repoDataDescriptors)
        //        {
        //            //string query = $"SELECT * FROM EdgarMissingData WHERE Symbol = '{descriptor.Symbol}' AND Year = {descriptor.Year} AND AccountingAttribute = '{descriptor.AccountingAttribute}'";
        //            //string query = $"SELECT * FROM EdgarMissingData WHERE Symbol = '{descriptor.Symbol}' AND Year = {descriptor.Year} AND AccountingAttribute = 'FreeCashFlow'";
        //            string query = "SELECT Symbol, Filed, Value FROM EdgarMissingData WHERE Symbol = @Symbol AND Year = @Year AND AccountingAttribute = @AccountingAttribute";


        //            using (var command = new OleDbCommand(query, connection))
        //            {
        //                command.Parameters.AddWithValue("@Symbol", descriptor.Symbol);
        //                command.Parameters.AddWithValue("@Year", descriptor.Year);
        //                command.Parameters.AddWithValue("@AccountingAttribute", "FreeCashFlow");

        //                using (OleDbDataReader reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        // Convert each row into a string representation and add it to the results
        //                        //results.Add(reader["ColumnName"].ToString()); // Replace ColumnName with the appropriate column

        //                        // Create a StringBuilder to build a string representing the entire row
        //                        StringBuilder row = new StringBuilder();

        //                        // Iterate through all columns in the current row
        //                        for (int i = 0; i < reader.FieldCount; i++)
        //                        {
        //                            // Append each column's value to the string, separated by a delimiter (like a comma)
        //                            row.Append(reader.GetValue(i).ToString());

        //                            if (i < reader.FieldCount - 1)
        //                            {
        //                                row.Append(", "); // Add a delimiter between columns
        //                            }
        //                        }

        //                        // Add the entire row as a string to the results list
        //                        results.Add(row.ToString());
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return results;
        //}

        public List<string> Get(List<SymbolAndAccountingAttribute> symbolAndAccountingAttributeList)
        {
            var results = new List<string>();
            List<string> symbols = symbolAndAccountingAttributeList.Select(a=> a.Symbol).ToList(); 

            using (var connection = new OleDbConnection(_connectionString))
            {
                connection.Open();
                foreach (var symbolAndAccountingAttribute in symbolAndAccountingAttributeList)
                {
                    //string query = $"SELECT * FROM EdgarMissingData WHERE Symbol = '{descriptor.Symbol}' AND Year = {descriptor.Year} AND AccountingAttribute = '{descriptor.AccountingAttribute}'";
                    //string query = $"SELECT * FROM EdgarMissingData WHERE Symbol = '{descriptor.Symbol}' AND Year = {descriptor.Year} AND AccountingAttribute = 'FreeCashFlow'";
                    string query = "SELECT Symbol, End, Value FROM EdgarMissingData WHERE Symbol = @Symbol AND AccountingAttribute = @AccountingAttribute";

                    using (var command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Symbol", symbolAndAccountingAttribute.Symbol);
                        command.Parameters.AddWithValue("@AccountingAttribute", symbolAndAccountingAttribute.AccountingAttribute);

                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Convert each row into a string representation and add it to the results
                                //results.Add(reader["ColumnName"].ToString()); // Replace ColumnName with the appropriate column

                                // Create a StringBuilder to build a string representing the entire row
                                StringBuilder row = new StringBuilder();

                                // Iterate through all columns in the current row
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    // Append each column's value to the string, separated by a delimiter (like a comma)
                                    var cellValue = reader.GetValue(i);
                                    row.Append(cellValue.ToString());

                                    if (i < reader.FieldCount - 1)
                                    {
                                        row.Append(", "); // Add a delimiter between columns
                                    }
                                }

                                // Add the entire row as a string to the results list
                                results.Add(row.ToString());
                            }
                        }
                    }
                }
            }

            return results;
        }
    }
}
