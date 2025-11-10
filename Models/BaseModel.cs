using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomMiddleWare.Models
{
    public class BaseModel
    {
        public string Id { get; set; }
        protected DateTime CreateDate { get; } = DateTime.Now;

        protected DateTime UpdatedDate { get; } = DateTime.Now;
    }
}
