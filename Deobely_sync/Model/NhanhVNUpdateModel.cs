namespace API_Adaption.Model
{
    public class NhanhVNUpdateModel
    {
        public int? id { get; set; }
        public string? privateId { get; set; }
        public string? ShopOrderId { get; set; }
        public int? channel { get; set; }
        public string? ecomShopId { get; set; }
        public int? saleChannel { get; set; }
        public string? merchantTrackingNumber { get; set; }
        public int? depotId { get; set; }
        public int? handoverId { get; set; }
        public string? depotName { get; set; }
        public int? TypeId { get; set; }
        public string? type { get; set; }
        public decimal? MoneyDiscount { get; set; }
        public decimal? MoneyDeposit { get; set; }
        public decimal? MoneyTransfer { get; set; }
        public string? depositAccount { get; set; }
        public string? transferAccount { get; set; }
        public int? usedPoints { get; set; }
        public decimal? moneyUsedPoints { get; set; }
        public int? carrierId { get; set; }
        public string? carrierName { get; set; }
        public int? serviceId { get; set; }
        public string? serviceName { get; set; }
        public string? carrierCode { get; set; }
        public decimal? ShipFee { get; set; }
        public decimal? CodFee { get; set; }
        public decimal? CustomerShipFee { get; set; }
        public decimal? returnFee { get; set; }
        public decimal? overWeightShipFee { get; set; }
        public decimal? declaredFee { get; set; }
        public string? Description { get; set; }
        public string? privateDescription { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerMobile { get; set; }
        public string? CustomerAddress { get; set; }
        public int? shipToCityLocationId { get; set; }
        public string? shipToDistrictLocationId { get; set; }
        public int? customerCityId { get; set; }
        public string? CustomerCity { get; set; }
        public int? customerDistrictId { get; set; }
        public string? CustomerDistrict { get; set; }
        public int? shipToWardLocationId { get; set; }
        public string? CustomerWard { get; set; }
        public int? createdById { get; set; }
        public string? createdByUserName { get; set; }
        public string? createdByName { get; set; }
        public int? saleId { get; set; }
        public string? saleName { get; set; }
        public string? createdDateTime { get; set; }
        public string? deliveryDate { get; set; }
        public string? sendCarrierDate { get; set; }
        public string? statusName { get; set; }
        public string? status { get; set; }
        public string? statusCode { get; set; }
        public decimal? CalcTotalMoney { get; set; }
        public int? trafficSourceId { get; set; }
        public string? trafficSourceName { get; set; }
        public string? affiliateCode { get; set; }
        public decimal? affiliateBonusCash { get; set; }
        public decimal? affiliateBonusPercent { get; set; }
        public List<NhanhResUpdateProductModel> Products { get; set; }
    }
    public class NhanhResUpdateProductModel
    {
        public int? productId { get; set; }
        public string? productName { get; set; }
        public string? productCode { get; set; }
        public string? productBarcode { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public string? Discount { get; set; }
        public int? Weight { get; set; }
    }

}
