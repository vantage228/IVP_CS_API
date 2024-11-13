using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_Console.Model
{
    public class EditEquityModel
    {
            public int? securityId { get; set; }
            public string? securityName { get; set; }
            public string? description { get; set; }
            public bool? isActive { get; set; }
            public string? pricingCurrency { get; set; }
            public decimal? totalSharesOutstanding { get; set; }
            public decimal? openPrice { get; set; }
            public decimal? closePrice { get; set; }
            public DateTime? dividendDeclaredDate { get; set; }
            public string? pfCreditRating { get; set; }
    }
}
