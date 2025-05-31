
using System;

namespace FakturowniaService.entity
{
    public class Warehouse
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int account_id { get; set; }
        public int? updater_id { get; set; }
        public int? creator_id { get; set; }
        public string kind { get; set; }
        public DateTimeOffset created_at { get; set; }
        public DateTimeOffset updated_at { get; set; }
        public bool deleted { get; set; }
        public bool active { get; set; }
        public bool use_product_warehouse_cache { get; set; }
        public int? siteor_shop_id { get; set; }
    }

}
