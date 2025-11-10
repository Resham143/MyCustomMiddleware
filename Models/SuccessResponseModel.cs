using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomMiddleWare.Models
{
    public sealed class SuccessResponseModel<T>:BaseResponseModel<T>
    {
        public SuccessResponseModel()
        {
            Message = "Success";
            Status = true;
            Code = 200;
        }
    }
}
