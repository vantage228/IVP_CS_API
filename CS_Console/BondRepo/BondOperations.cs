using BondConsoleApp.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondConsoleApp.Repository
{
    public class BondOperations : IBond
    {
        private string _connectionString;

        public BondOperations(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void ImportDataFromCsv(Stream csvStream)
        {
            var records = ReadCsvFile(csvStream);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (var record in records)
                        {
                            InsertFullBondData(record, connection, transaction);
                        }

                        transaction.Commit();
                        Console.WriteLine("Data imported successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error during import: " + ex.Message);
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private List<BondModel> ReadCsvFile(Stream csvStream)
        {
            using (var reader = new StreamReader(csvStream))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<BondModel>().ToList();
            }
        }

        private void InsertFullBondData(BondModel data, SqlConnection connection, SqlTransaction transaction)
        {
            using (SqlCommand command = new SqlCommand("Corporate_Bond.sp_InsertFullBondData", connection, transaction))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Security Summary Params
                command.Parameters.AddWithValue("@security_name", data.SecurityName);
                command.Parameters.AddWithValue("@security_description", data.SecurityDescription ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@asset_type", data.AssetType);
                command.Parameters.AddWithValue("@investment_type", data.InvestmentType ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@trading_factor", data.TradingFactor);
                command.Parameters.AddWithValue("@pricing_factor", data.PricingFactor);
                command.Parameters.AddWithValue("@is_active", true);
                command.Parameters.AddWithValue("@cusip", data.CUSIP ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@isin", data.ISIN ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@sedol", data.Sedol ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@bloomberg_ticker", data.BloombergTicker ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@bloomberg_unique_id", data.BloombergUniqueID ?? (object)DBNull.Value);

                // Security Identifier Params
                command.Parameters.AddWithValue("@first_coupon_date", data.FirstCouponDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@coupon_cap", data.CouponCap ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@coupon_floor", data.CouponFloor ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@coupon_frequency", data.CouponFrequency);
                command.Parameters.AddWithValue("@coupon", data.Coupon);
                command.Parameters.AddWithValue("@coupon_type", data.CouponType);
                command.Parameters.AddWithValue("@spread", data.Spread);

                // Security Details Params
                command.Parameters.AddWithValue("@is_callable", data.IsCallable);
                command.Parameters.AddWithValue("@is_fix_to_float", data.IsFixToFloat);
                command.Parameters.AddWithValue("@is_putable", data.IsPutable);
                command.Parameters.AddWithValue("@issue_date", data.IssueDate);
                command.Parameters.AddWithValue("@last_reset_date", data.LastResetDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@maturity_date", data.MaturityDate);
                command.Parameters.AddWithValue("@maximum_call_notice_days", data.MaximumCallNoticeDays ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@maximum_put_notice_days", data.MaximumPutNoticeDays ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@penultimate_coupon_date", data.PenultimateCouponDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@reset_frequency", data.ResetFrequency);
                command.Parameters.AddWithValue("@has_position", data.HasPosition);

                // Risk Params
                command.Parameters.AddWithValue("@average_volume_30d", data.AverageVolume30D ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@duration", data.Duration ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@convexity", data.Convexity ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@volatility_90d", data.Volatility90D ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@volatility_30d", data.Volatility30D ?? (object)DBNull.Value);

                // Regulatory Details Params
                command.Parameters.AddWithValue("@pf_asset_class", data.FormPFAssetClass);
                command.Parameters.AddWithValue("@pf_country", data.FormPFCountry);
                command.Parameters.AddWithValue("@pf_credit_rating", data.FormPFCreditRating ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@pf_currency", data.FormPFCurrency);
                command.Parameters.AddWithValue("@pf_instrument", data.FormPFInstrument);
                command.Parameters.AddWithValue("@pf_liquidity_profile", data.FormPFLiquidityProfile);
                command.Parameters.AddWithValue("@pf_maturity", data.FormPFMaturity ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@pf_naics_code", data.FormPFNAICSCode ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@pf_region", data.FormPFRegion ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@pf_sector", data.FormPFSector ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@pf_sub_asset_class", data.FormPFSubAssetClass);

                // Reference Data Params
                command.Parameters.AddWithValue("@country_of_issuance", data.IssueCountry ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@issuer", data.Issuer ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@issue_currency", data.IssueCurrency);
                command.Parameters.AddWithValue("@bbg_industry_sub_group", data.BloombergIndustrySubGroup);
                command.Parameters.AddWithValue("@bloomberg_industry_group", data.BloombergIndustryGroup);
                command.Parameters.AddWithValue("@bloomberg_sector", data.BloombergSector);
                command.Parameters.AddWithValue("@risk_currency", data.RiskCurrency);

                // Pricing Details Params
                command.Parameters.AddWithValue("@high_price", data.HighPrice ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@low_price", data.LowPrice ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@open_price", data.OpenPrice ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@volume", data.Volume ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@last_price", data.LastPrice ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ask_price", data.AskPrice);
                command.Parameters.AddWithValue("@bid_price", data.BidPrice);

                // Dividend History Params
                command.Parameters.AddWithValue("@put_date", data.PutDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@put_price", data.PutPrice ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@call_date", data.CallDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@call_price", data.CallPrice ?? (object)DBNull.Value);

                command.ExecuteNonQuery();
            }
        }



        public void UpdateBondData(EditBondModel ebm)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("Corporate_Bond.sp_UpdateBondData", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Set up parameters with values
                    command.Parameters.AddWithValue("@security_id", ebm.SecurityID);
                    command.Parameters.AddWithValue("@security_description", ebm.SecurityDescription ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@coupon", ebm.Coupon);
                    command.Parameters.AddWithValue("@is_callable", ebm.IsCallable);
                    command.Parameters.AddWithValue("@penultimate_coupon_date", ebm.PenultimateCouponDate);
                    command.Parameters.AddWithValue("@pf_credit_rating", ebm.FormPFCreditRating ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ask_price", ebm.AskPrice);
                    command.Parameters.AddWithValue("@bid_price", ebm.BidPrice);

                    try
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Bond data updated successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred while updating bond data: " + ex.Message);
                    }
                }
            }

        }

        public void DeleteBondData(int SecurityID)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Corporate_Bond.DeleteSecurity", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@security_id", SecurityID);

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

        public List<EditBondModel> GetBondsData()
        {
            var bonds = new List<EditBondModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Corporate_Bond.sp_GetEditBondData", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var bond = new EditBondModel
                                {
                                    SecurityID = Convert.ToInt32(reader["SecurityID"]),
                                    SecurityDescription = reader["SecurityDescription"].ToString(),
                                    SecurityName = reader["SecurityName"].ToString(),
                                    MaturityDate = reader["MaturityDate"] as DateTime?,
                                    AskPrice = Convert.ToDecimal(reader["AskPrice"]),
                                    IsActive = Convert.ToBoolean(reader["IsActive"])
                                };

                                bonds.Add(bond);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            }

            return bonds;
        }

    }

}