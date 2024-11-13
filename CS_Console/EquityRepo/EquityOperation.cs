using CS_Console.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Transactions;
using Microsoft.Extensions.Configuration;

namespace CS_Console.EquityRepo
{
    public class EquityOperation : IEquity
    {
        private string _connectionString;
        
        public EquityOperation(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public EquityOperation() { }

        public void ImportDataFromCsv(string filePath)
        {
            var records = ReadCsvFile(filePath);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var record in records)
                        {
                            InsertFullSecurityData(record, connection, transaction);
                        }

                        // Commit the transaction if all inserts are successful
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if any insert fails
                        Console.WriteLine("Error during import: " + ex.Message);
                        transaction.Rollback();
                    }
                }
            }
        }

        private List<EquityDataModel> ReadCsvFile(string filePath)
        {
            var records = new List<EquityDataModel>();

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                records = csv.GetRecords<EquityDataModel>().ToList();
            }
            return records;
        }

        private void InsertFullSecurityData(EquityDataModel data, SqlConnection connection, SqlTransaction transaction)
        {
            using (SqlCommand command = new SqlCommand("Corporate_Equity.sp_InsertFullSecurityData", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaction;

                //Security Summary Params
                command.Parameters.AddWithValue("@security_name", data.SecurityName);
                command.Parameters.AddWithValue("@security_description", data.SecurityDescription);
                command.Parameters.AddWithValue("@has_position", data.HasPosition);
                command.Parameters.AddWithValue("@is_active_security", data.IsActiveSecurity);
                command.Parameters.AddWithValue("@lot_size", data.LotSize);
                command.Parameters.AddWithValue("@bbg_unique_name", data.BbgUniqueName);

                //Security Identifier Params
                command.Parameters.AddWithValue("@cusip", data.Cusip);
                command.Parameters.AddWithValue("@isin", data.Isin);
                command.Parameters.AddWithValue("@sedol", data.Sedol);
                command.Parameters.AddWithValue("@bloomberg_ticker", data.BloombergTicker);
                command.Parameters.AddWithValue("@bloomberg_unique_id", data.BloombergUniqueId);
                command.Parameters.AddWithValue("@bbg_global_id", data.BbgGlobalId);
                command.Parameters.AddWithValue("@bbg_ticker_exchange", data.BbgTickerExchange);

                //Security Details Params
                command.Parameters.AddWithValue("@is_adr", data.IsAdr);
                command.Parameters.AddWithValue("@adr_underlying_ticker", data.AdrUnderlyingTicker);
                command.Parameters.AddWithValue("@adr_underlying_currency", data.AdrUnderlyingCurrency);
                command.Parameters.AddWithValue("@shares_per_adr", data.SharesPerAdr ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ipo_date", data.IpoDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@pricing_currency", data.PricingCurrency);
                command.Parameters.AddWithValue("@settle_days", data.SettleDays);
                command.Parameters.AddWithValue("@shares_outstanding", data.SharesOutstanding ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@voting_rights_per_share", data.VotingRightsPerShare);

                //Risk Params
                command.Parameters.AddWithValue("@average_volume_20d", data.AverageVolume20D);
                command.Parameters.AddWithValue("@beta", data.Beta);
                command.Parameters.AddWithValue("@short_interest", data.ShortInterest);
                command.Parameters.AddWithValue("@return_ytd", data.ReturnYtd);
                command.Parameters.AddWithValue("@volatility_90D", data.Volatility90D ?? (object)DBNull.Value);

                //Regulatory Details Params
                command.Parameters.AddWithValue("@pf_asset_class", data.PfAssetClass);
                command.Parameters.AddWithValue("@pf_country", data.PfCountry);
                command.Parameters.AddWithValue("@pf_credit_rating", data.PfCreditRating);
                command.Parameters.AddWithValue("@pf_currency", data.PfCurrency);
                command.Parameters.AddWithValue("@pf_instrument", data.PfInstrument);
                command.Parameters.AddWithValue("@pf_liquidity_profile", data.PfLiquidityProfile);
                command.Parameters.AddWithValue("@pf_maturity", data.PfMaturity ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@pf_naics_code", data.PfNaicsCode ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@pf_region", data.PfRegion);
                command.Parameters.AddWithValue("@pf_sector", data.PfSector ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@pf_sub_asset_class", data.PfSubAssetClass);

                //Reference Data Params
                command.Parameters.AddWithValue("@country_of_issuance", data.CountryOfIssuance);
                command.Parameters.AddWithValue("@exchange", data.Exchange);
                command.Parameters.AddWithValue("@issuer", data.Issuer);
                command.Parameters.AddWithValue("@issue_currency", data.IssueCurrency);
                command.Parameters.AddWithValue("@trading_currency", data.TradingCurrency);
                command.Parameters.AddWithValue("@bbg_industry_sub_group", data.BbgIndustrySubGroup);
                command.Parameters.AddWithValue("@bloomberg_industry_group", data.BloombergIndustryGroup);
                command.Parameters.AddWithValue("@bloomberg_sector", data.BloombergSector);
                command.Parameters.AddWithValue("@country_of_incorporation", data.CountryOfIncorporation);
                command.Parameters.AddWithValue("@risk_currency", data.RiskCurrency);

                //Pricing Details Params
                command.Parameters.AddWithValue("@open_price", data.OpenPrice);
                command.Parameters.AddWithValue("@close_price", data.ClosePrice);
                command.Parameters.AddWithValue("@volume", data.Volume);
                command.Parameters.AddWithValue("@last_price", data.LastPrice);
                command.Parameters.AddWithValue("@ask_price", data.AskPrice);
                command.Parameters.AddWithValue("@bid_price", data.BidPrice);
                command.Parameters.AddWithValue("@pe_ratio", data.PeRatio);

                //Dividend History Params
                command.Parameters.AddWithValue("@dividend_declared_date", data.DividendDeclaredDate);
                command.Parameters.AddWithValue("@dividend_ex_date", data.DividendExDate);
                command.Parameters.AddWithValue("@dividend_record_date", data.DividendRecordDate);
                command.Parameters.AddWithValue("@dividend_pay_date", data.DividendPayDate);
                command.Parameters.AddWithValue("@dividend_amount", data.DividendAmount);
                command.Parameters.AddWithValue("@frequency", data.Frequency ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@dividend_type", data.DividendType);

                command.ExecuteNonQuery();

            }   
        }

        public string UpdateSecurityData(EditEquityModel esm)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("Corporate_Equity.sp_UpdateSecurityData", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Set up parameters with values
                    command.Parameters.AddWithValue("@security_id", esm.securityId);
                    command.Parameters.AddWithValue("@description", esm.description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@pricing_currency", esm.pricingCurrency ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@total_shares_outstanding", esm.totalSharesOutstanding);
                    command.Parameters.AddWithValue("@open_price", esm.openPrice);
                    command.Parameters.AddWithValue("@close_price", esm.closePrice);
                    command.Parameters.AddWithValue("@dividend_declared_date", (object)esm.dividendDeclaredDate ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Pf_credit_rating", esm.pfCreditRating ?? (object)DBNull.Value);

                    try
                    {
                        command.ExecuteNonQuery();
                        return "Updated Data";
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                }
            }
        }

        public void DeleteSecurityData(int securityId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Corporate_Equity.sp_DeleteSecurityData", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@security_id", securityId);

                    try
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Record successfully marked as inactive.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            }
        }

        public List<EditEquityModel> GetSecurityData()
        {
            var securityList = new List<EditEquityModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Corporate_Equity.sp_SelectSecurityData", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        foreach (DataRow row in dataTable.Rows)
                        {
                            var security = new EditEquityModel
                            {
                                securityId = row["security_id"] == DBNull.Value ? 0 : Convert.ToInt32(row["security_id"]),
                                securityName = row["security_name"] == DBNull.Value ? string.Empty : row["security_name"].ToString(),
                                description = row["security_description"] == DBNull.Value ? string.Empty : row["security_description"].ToString(),
                                isActive = row["is_active_security"] == DBNull.Value ? false : Convert.ToBoolean(row["is_active_security"]),
                                pricingCurrency = row["pricing_currency"] == DBNull.Value ? string.Empty : row["pricing_currency"].ToString(),
                                totalSharesOutstanding = row["shares_outstanding"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["shares_outstanding"]),
                                openPrice = row["open_price"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["open_price"]),
                                closePrice = row["close_price"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["close_price"]),
                                dividendDeclaredDate = row["dividend_declared_date"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["dividend_declared_date"]),
                                pfCreditRating = row["pf_credit_rating"] == DBNull.Value ? string.Empty : row["pf_credit_rating"].ToString()
                            };
                            securityList.Add(security);
                        }
                    }
                }
            }

            return securityList;
        }
    }
}
