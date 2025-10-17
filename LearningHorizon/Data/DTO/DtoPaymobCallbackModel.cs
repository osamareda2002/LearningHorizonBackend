namespace LearningHorizon.Data.DTO
{
    public class DtoPaymobCallbackModel
    {
        public int id { get; set; }

        public bool pending { get; set; }

        public int amount_cents { get; set; }

        public bool success { get; set; }

        public bool is_auth { get; set; }

        public bool is_capture { get; set; }

        public bool is_standalone_payment { get; set; }

        public bool is_voided { get; set; }

        public bool is_refunded { get; set; }

        public bool is_3d_secure { get; set; }

        public int integration_id { get; set; }

        public int profile_id { get; set; }

        public bool has_parent_transaction { get; set; }

        public int order { get; set; }

        public string created_at { get; set; }

        public string currency { get; set; }

        public int merchant_commission { get; set; }

        public string discount_details { get; set; }

        public bool is_void { get; set; }

        public bool is_refund { get; set; }

        public bool error_occured { get; set; }

        public int refunded_amount_cents { get; set; }

        public int captured_amount { get; set; }

        public string updated_at { get; set; }

        public bool is_settled { get; set; }

        public bool bill_balanced { get; set; }

        public bool is_bill { get; set; }

        public int owner { get; set; }

        public Data data { get; set; }

        public SourceData source_data { get; set; }

        public string acq_response_code { get; set; }

        public string txn_response_code { get; set; }

        public string hmac { get; set; }
    }

    public class Data
    {
        public string message { get; set; }
    }

    public class SourceData
    {
        public string type { get; set; }

        public string pan { get; set; }

        public string sub_type { get; set; }
    }
}
