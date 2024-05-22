﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories;

namespace Talabat.Core
{
    public interface IUnitOfWork :IAsyncDisposable
    {

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity :BaseEntity;

        Task<int> CompleteAsync();
    }
}
