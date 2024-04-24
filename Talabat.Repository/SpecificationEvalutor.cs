using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
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

        public static IQueryable<T> GetQuery(IQueryable<T> InputQuery , ISpecifications<T> spec)
        {
            var query = InputQuery;

            if (spec.Criretia is not null)
            {
                query = query.Where(spec.Criretia);
            }

            if (spec.OrderBy is not null  )
            {
                query =  query.OrderBy(spec.OrderBy);
            }

            else if (spec.OrderByDesc is not null )
            {
                query = query.OrderByDescending(spec.OrderByDesc);
            }

            if (spec.IsPaginationEnabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            query = spec.Includes.Aggregate(query, (CurrentQuery, IncludeExpression) => CurrentQuery.Include(IncludeExpression));


            return query;
        }

    }
}
