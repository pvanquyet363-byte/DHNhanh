namespace API_Adaption.Model
{
    public class InvoiceMatBaoModel
    {
        public int? No { get; set; }
        public string? Pattern { get; set; }
        public string? Serial { get; set; }
        public string? Fkey { get; set; }
        public string? CusCode { get; set; }
        public string? CusName { get; set; }
        public string? CusTaxCode { get; set; }
        public string? CusAddress { get; set; }
        public string? CusEmail { get; set; }
        public string? SO { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime? ArisingDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public decimal? Total { get; set; }
        public decimal? Amount { get; set; }
        public string? MaThongDiep { get; set; }
        public string? MLTDiep { get; set; }
        public string? MCCQT { get; set; }
        public string? LoaiHoaDon { get; set; }
        public string? TrangThaiHD { get; set; }
        public string? Extra1 { get; set; }
        public List<ProductMatBaoModel> Products { get; set; }
    }

    public class ProductMatBaoModel
    {
        public int? STT { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }
        public double? Quantity { get; set; }
        public double? TotalChuaCK { get; set; }
        public double? Discount { get; set; }
        public double? DiscountAmount { get; set; }
        public double? VATRate { get; set; }
        public string? VATRateName { get; set; }
        public double? VATAmount { get; set; }
        public double? Amount { get; set; }
        public int? TinhChat { get; set; }
        public string? TinhChatName { get; set; }
    }
    public class ReqParamModel
    {
        public string ApiInvPattern {  get; set; }
        public string ApiInvSerial {  get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
    }
}
