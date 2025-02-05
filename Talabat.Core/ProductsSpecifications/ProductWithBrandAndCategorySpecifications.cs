using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.ProductsSpecifications
{
    public class ProductWithBrandAndCategorySpecifications :BaseSpecification<Product>
    {
        // To Get AllProducts, will chain on the base parameterless constructor
        public ProductWithBrandAndCategorySpecifications(string? sort)
            :base() 
        {
            Includes.Add(P => P.Brand);
            Includes.Add(P => P.Category);
            if(!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "priceAsc":
                        OrderBy = P => P.Price;
                        break;
                    case "priceDesc":
                        OrderByDesc = P => P.Price;
                        break;
                    default:
                        OrderBy = P => P.Name;
                        break;
                }
            }
            else
                OrderBy = P => P.Name;
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
