#nullable enable

using System;

namespace FakturowniaService
{
    public class WarehouseDocument
    {
        public int id { get; set; }
        public string? kind { get; set; }
        public string? supplier { get; set; }
        public string? recipient { get; set; }
        public string? description { get; set; }
        public DateTime issue_date { get; set; }
        public string? number { get; set; }
        public int warehouse_id { get; set; }
        public int updater_id { get; set; }
        public int creator_id { get; set; }
        public bool deleted { get; set; }
        public DateTimeOffset created_at { get; set; }
        public DateTimeOffset updated_at { get; set; }
        public string? pattern { get; set; }
        public int? pattern_nr { get; set; }
        public int? pattern_nr_m { get; set; }
        public int? pattern_nr_d { get; set; }
        public int issue_year { get; set; }
        public string? external_id { get; set; }
        public decimal exchange_rate { get; set; }
        public string? currency { get; set; }
        public string? oid { get; set; }

        public long? client_id { get; set; }

        public long GetClient_id()
        {
            return client_id ?? 0;
        }
        public DateTime? expected_delivery_date { get; set; }
        public string? recipient_ref { get; set; }
        public int? warehouse_document_id { get; set; }
        public string? seller_person { get; set; }
        public string? buyer_person { get; set; }
        public string? gave_person { get; set; }

        public long? department_id { get; set; }

        public long GetDepartment_id()
        {
            return department_id ?? 0;
        }

        public string? lang { get; set; }
        public string? department_name { get; set; }
        public string? department_tax_no_kind { get; set; }
        public string? department_tax_no { get; set; }
        public string? department_bank_account { get; set; }
        public string? department_bank { get; set; }
        public string? department_street { get; set; }
        public string? department_post_code { get; set; }
        public string? department_city { get; set; }
        public string? department_country { get; set; }
        public string? department_email { get; set; }
        public string? department_www { get; set; }
        public string? department_fax { get; set; }
        public string? department_phone { get; set; }
        public bool client_company { get; set; }
        public string? client_title { get; set; }
        public string? client_first_name { get; set; }
        public string? client_last_name { get; set; }
        public string? client_name { get; set; }
        public string? client_tax_no_kind { get; set; }
        public string? client_tax_no { get; set; }
        public string? client_street { get; set; }
        public string? client_post_code { get; set; }
        public string? client_city { get; set; }
        public string? client_bank_account { get; set; }
        public string? client_bank { get; set; }
        public string? client_country { get; set; }
        public string? client_note { get; set; }
        public string? client_email { get; set; }
        public string? client_phone { get; set; }
        public string? client_delivery_address { get; set; }
        public bool client_use_delivery_address { get; set; }
        public string? exchange_currency { get; set; }
        public string? exchange_kind { get; set; }
        public decimal? exchange_currency_rate { get; set; }
        public DateTime? exchange_date { get; set; }
        public string? exchange_note { get; set; }
        public decimal price_net { get; set; }
        public decimal price_gross { get; set; }
        public decimal price_tax { get; set; }
        public string? calculating_strategy_position { get; set; }
        public string? calculating_strategy_sum { get; set; }
        public string? calculating_strategy_invoice_form_price_kind { get; set; }
        public DateTime transaction_date { get; set; }
        public int? template_id { get; set; }
        public decimal purchase_price_net { get; set; }
        public decimal purchase_price_gross { get; set; }
        public decimal purchase_price_tax { get; set; }
        public bool from_form_with_actions { get; set; }
        public bool from_api { get; set; }
        public string? origin { get; set; }
        public string? additional_info { get; set; }
        public string? additional_info_desc { get; set; }
        public string? status { get; set; }
        public decimal quantity { get; set; }
        public decimal exchange_rate_den { get; set; }
        public decimal exchange_currency_rate_den { get; set; }
        public string? fiscal_currency { get; set; }
        public decimal fiscal_exchange_rate { get; set; }
        public decimal fiscal_exchange_rate_den { get; set; }
        public string? search_data { get; set; }
    }
}
