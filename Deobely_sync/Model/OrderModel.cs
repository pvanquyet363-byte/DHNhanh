namespace Deobely_sync.Model
{
    public class OrderModel
    {
        public string CreateDate { get; set; } = DateTime.Now.ToString();//
        public double? Shipping_Fee { get; set; } = 0;//
        public string PaymentMethod { get; set; }//
       public double TotalPrice { get; set; }//
        public double TotalDiscountPrice { get; set; }//
        public string? Coupon { get; set; }//
        public Billing? Billing { get; set; }//
        public List<Item> Items { get; set; }//
        public string OrderNumber { get; set; }//
        public VAT? VAT { get; set; }//
        public string? PickUp { get; set; }//
        public string? Note { get; set; }//
        public TranferInfo? TranferInfo { get; set; }//

    }

    public class TranferInfo
    {
        public string? bank_name { get; set; }
        public string? bank_account_name { get; set; }
        public string? bank_account_number { get; set; }
        public long? order_total { get; set; }
        public long? discount_value { get; set; }
        public string? payment_status { get; set; }
    }



    public class VAT
    {
        public long? vat_id { get; set; }
        public string? vat_address { get; set; }
        public string? vat_name { get; set; }
        public string? vat_email { get; set; }   
    }
    public class Billing
    {
        public string? phone { get; set; } 
        public string? name { get; set; } 
        public string? address { get; set; } 
        public string? quan { get; set; } 
        public string? tinh { get; set; } 
        public string? phuong { get; set; } 
        public string? email { get; set; }
    }

    public class Item
    {
        public string ma_vt { get; set; }
        public double quantity { get; set; }
        public double price { get; set; }
        public double total { get; set; }
        public double subtotal { get; set; }

    }

}
