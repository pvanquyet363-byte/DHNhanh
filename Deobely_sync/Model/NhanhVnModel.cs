using Deobely_sync.Service.Impliment;
using System.ComponentModel.DataAnnotations;
namespace API_Adaption.Model
{
    public class OrderNhanhResModel
    {
        public int? OrderId { get; set; }
        public string? ShopOrderId { get; set; }
        public int? SaleChannel { get; set; }
        public string? Type { get; set; }
        public int? TypeId { get; set; }
        public decimal? MoneyDiscount { get; set; }
        public decimal? MoneyDeposit { get; set; }
        public decimal? MoneyTransfer { get; set; }
        public int? UsedPoints { get; set; }
        public decimal? MoneyUsedPoints { get; set; }
        public decimal? ShipFee { get; set; }
        public decimal? CodFee { get; set; }
        public decimal? CustomerShipFee { get; set; }
        public decimal? CalcTotalMoney { get; set; }
        public string? Description { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerMobile { get; set; }
        public string? CustomerAddress { get; set; }
        public int? ShipToCityLocationId { get; set; }
        public int? ShipToDistrictLocationId { get; set; }
        public int? ShipToWardLocationId { get; set; }
        public string? CustomerCity { get; set; }
        public string? CustomerDistrict { get; set; }
        public string? CustomerWard { get; set; }
        public int? CreatedById { get; set; }
        public string? CouponCode { get; set; }
        public string? Status { get; set; }
        public string? StatusDescription { get; set; }
        public int? DepotId { get; set; }
        public List<NhanhResProductModel> Products { get; set; }
    }
    public class NhanhResProductModel
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int Weight { get; set; }
    }
}
