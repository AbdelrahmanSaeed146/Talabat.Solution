using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Specifications
{
    public class ProductSpecParams
    {

        public string? Sort { get; set; }

        public int? brandId { get; set; }
        public int? cateId { get; set; }
        private int pageSize = 5 ;

        public int PageSize 
        {
            get { return pageSize = 5 ; }
            set { pageSize  = value > 10 ? 10 :value; }
        }

        public int PageIndex { get; set; } = 1;

    }
}
