using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HPlusSportAPI.Models
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        public int UserID { get; set; }

        public virtual List<Product> Products { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }

    }
}
