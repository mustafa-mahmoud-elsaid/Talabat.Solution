using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.Order
{
    public class ProductItemOrdered
    {
        public ProductItemOrdered()
        {
            
        }
        public ProductItemOrdered(int productId, string productName, string pictureUrl)
        {
            Id = productId;
            Name = productName;
            PictureUrl = pictureUrl;
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string PictureUrl { get; set; } = null!;
    }
}
