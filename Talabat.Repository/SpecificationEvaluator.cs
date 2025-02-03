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
    internal static class SpecificationEvaluator <TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery , ISpecification<TEntity> specification)
        {
            var query = inputQuery;

            if(specification.Criteria is not null)
                query = query.Where(specification.Criteria);

            query = specification.Includes.Aggregate(query, (currentQuery, includeExp) => currentQuery.Include(includeExp));

            return query;
        }
    }
}
