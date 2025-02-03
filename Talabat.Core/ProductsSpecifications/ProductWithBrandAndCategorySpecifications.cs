using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.ProductsSpecifications
{
    public class ProductWithBrandAndCategorySpecifications :BaseSpecification<Product>
    {
        // To Get AllProducts, will chain on the base parameterless constructor
        public ProductWithBrandAndCategorySpecifications()
            :base() 
        {
            Includes.Add(P => P.Brand);
            Includes.Add(P => P.Category);
        }
        // To Get ProductsById, will chain on the base constructor that take the criteria ( p=> p.Id == id ) 
        public ProductWithBrandAndCategorySpecifications(int id)
            :base(P => P.Id == id)
        {
            Includes.Add(P => P.Brand);
            Includes.Add(P => P.Category);
        }
    }
}
