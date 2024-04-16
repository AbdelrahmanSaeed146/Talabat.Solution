using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repository
{
    public static class SpecificationEvalutor<T> where T : BaseEntity
    {

        //public SpecificationEvalutor(AppContext )
        //{
            
        //}

        public static IQueryable<T> GetQuery(IQueryable<T> InputQuery , ISpecifications<T> Spec)
        {
            var Query = InputQuery;

            if (Spec.Criretia is not null)
            {
                Query = Query.Where(Spec.Criretia);
            }

            Query = Spec.Includes.Aggregate(Query, (CurrentQuery, IncludeExpression) => CurrentQuery.Include(IncludeExpression));


            return Query;
        }

    }
}
