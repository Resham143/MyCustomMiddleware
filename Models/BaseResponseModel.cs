using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomMiddleWare.Models
{
    public class BaseResponseModel<T>
    {
        public string Message { get; set; }
        public T? Data { get; set; }

        public bool Status { get; set; }

        public int Code { get; set; }
    }
}
