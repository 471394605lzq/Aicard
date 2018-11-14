using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCard.Common
{
    interface IErrorReusult
    {
        bool IsSuccess { get; set; }

        string Code { get; set; }

        string Message { get; set; }
    }

    public class BassErrorResult : IErrorReusult
    {
        public string Code { get; set; }

        public bool IsSuccess { get; set; }

        public string Message { get; set; }
    }
}
