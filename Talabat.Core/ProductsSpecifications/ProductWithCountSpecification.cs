﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.ProductsSpecifications
{
    public class ProductWithCountSpecification :BaseSpecification<Product>
    {
        public ProductWithCountSpecification(ProductSpecParams specParams)
            :base(
                    P =>
                    (string.IsNullOrEmpty(specParams.Search) || P.Name.ToLower().Contains(specParams.Search)) &&
                    (!specParams.BrandId.HasValue || P.BrandId == specParams.BrandId) &&
                    (!specParams.CategoryId.HasValue || P.CategoryId == specParams.CategoryId)
                 )
        {
            
        }
    }
}
