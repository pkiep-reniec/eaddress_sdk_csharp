using System;

namespace eaddress_sdk_csharp.dto
{
    public class ApiResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public int code { get; set; }
    }
}
