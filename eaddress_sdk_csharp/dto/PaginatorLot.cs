using System;
using System.Collections.Generic;

namespace eaddress_sdk_csharp.dto
{
    public class PaginatorLot
    {
        public int recordsTotal { get; set; }
        public List<LotData> data { get; set; }
    }
}
