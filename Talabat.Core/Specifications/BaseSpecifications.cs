﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public class BaseSpecifications<T> : ISpecifications<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>> Criretia { get; set; } = null;
        public List<Expression<Func<T, object>>> Includes { get ; set ; } = new List<Expression<Func<T, object>>>();
        public List<Expression<Func<T, object>>> OrderBy { get; set; } = new List<Expression<Func<T, object>>>();
        public List<Expression<Func<T, object>>> OrderByDesc { get ; set; } = new List<Expression<Func<T, object>>>();

        public BaseSpecifications()
        {
            
        }

        public BaseSpecifications(Expression<Func<T,bool>> criteriaExp)
        {
            Criretia = criteriaExp;
        }


        public void AddOrderBy(Expression<Func<T , object>> OrderByExpression)
        {
            OrderBy.Add(OrderByExpression);
        }   
        
        public void AddOrderByDesc(Expression<Func<T , object>> OrderByDescExpression)
        {
            OrderByDesc.Add( OrderByDescExpression);
        }

    }
}
