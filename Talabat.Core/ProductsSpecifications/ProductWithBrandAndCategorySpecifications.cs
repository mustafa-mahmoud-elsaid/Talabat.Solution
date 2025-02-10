using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.ProductsSpecifications
{
    public class ProductWithBrandAndCategorySpecifications :BaseSpecification<Product>
    {
        // To Get AllProducts, will chain on the base parameterless constructor
        public ProductWithBrandAndCategorySpecifications(ProductSpecParams specParams)
            :base(
                    P => 
                    (string.IsNullOrEmpty(specParams.Search) || P.Name.ToLower().Contains(specParams.Search)) &&
                    (!specParams.BrandId.HasValue || P.BrandId == specParams.BrandId) &&
                    (!specParams.CategoryId.HasValue || P.CategoryId == specParams.CategoryId)
                 ) 
        {
            Includes.Add(P => P.Brand);
            Includes.Add(P => P.Category);
            if(!string.IsNullOrEmpty(specParams.Sort))
            {
                switch (specParams.Sort)
                {
                    case "priceAsc":
                        OrderBy = P => P.Price;
                        break;
                    case "priceDesc":
                        OrderByDesc = P => P.Price;
                        break;
                    case "nameDesc":
                        OrderByDesc = P => P.Price;
                        break;
                    default:
                        OrderBy = P => P.Name;
                        break;
                }
            }
            else
                OrderBy = P => P.Name;
            
            ApplyPagination((specParams.PageIndex - 1) * specParams.PageSize, specParams.PageSize);
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
