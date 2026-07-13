using Dapper;
using Deobely_sync.Model;
using Deobely_sync.Service.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Deobely_sync.Service.Impliment
{
    public class CreateOrderService: ICreateOrder
    {
        private readonly ILogger<Product> _logger;
        private IConfiguration _config;

        public CreateOrderService(ILogger<Product> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<int> CheckCustomer(OrderModel model)
        {
            string str = $"select top 1 ma_kh from dmkh where dien_thoai = '{model.Billing.phone}'";

            var connect = _config.GetConnectionString("DbCnn");
            IDbConnection conn = new SqlConnection(connect);
            var result = await conn.QueryMultipleAsync(str, commandType: CommandType.Text);
            var table2 = await result.ReadAsync<String>();
            var x = table2.ToList();
            if (x.Count == 0)
            {
                str = $"insert into dmkh (ma_kh,ten_kh,kh_yn,cc_yn,nv_yn,dia_chi,dien_thoai,datetime0,datetime2,ten_kh2 )";
                str += $" values ('{model.Billing.phone}','{model.Billing.name}',1,1,1,N'{model.Billing.address}','{model.Billing.phone}','{DateTime.Now}','{DateTime.Now}','' )";
                await conn.ExecuteAsync(str, commandType: CommandType.Text);
                return 1;
            }
            return 0;
        }



            public async Task<string> CreateOrder(OrderModel model)
        {
            try {
                var a = await  CheckCustomer(model);
                var myDate = DateTime.Now.AddHours(7);
                string table_sufix = myDate.ToString("yyyyMM", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                int i = 0;
                string ma_dvcs = "ON01";
                string so_ct = DateTime.Now.ToString("yy") + $"{model.OrderNumber}";
                string loai_ct = "CT";
                string ma_gd = "GD";
                string ma_nt = "VND";
                string ngay_ct = myDate.ToString("yyyy/MM/dd", System.Globalization.CultureInfo.GetCultureInfo("en-US"));

                double t_tien2 = model.TotalPrice;
                double t_ck_tt_nt = model.TotalDiscountPrice;

                //ma-voucher
                string ma_voucher = model.Coupon;//hỏi


                string status = "1";



                string str = "declare @q nvarchar(4000) = ''";

                str += Environment.NewLine + "IF OBJECT_ID('TempDb..#master') IS NOT NULL DROP TABLE #master;";
                str += Environment.NewLine + "IF OBJECT_ID('TempDb..#v03') IS NOT NULL DROP TABLE #v03;";
                str += Environment.NewLine + "IF OBJECT_ID('TempDb..#v04') IS NOT NULL DROP TABLE #v04;";
                str += Environment.NewLine + "IF OBJECT_ID('TempDb..#detail') IS NOT NULL DROP TABLE #detail;";
                str += Environment.NewLine + "IF OBJECT_ID('TempDb..#return') IS NOT NULL DROP TABLE #return;"; 
                str += Environment.NewLine + "IF OBJECT_ID('TempDb..#ctgt20') IS NOT NULL DROP TABLE #ctgt20;";

                str += Environment.NewLine + "select top 0 * into #master from m81$000000";
                str += Environment.NewLine + "select top 0 * into #detail from d81$000000";
                str += Environment.NewLine + "select top 0 * into #return from d816$000000";
                str += Environment.NewLine + "select top 0 * into #v03 from v03$000000";
                str += Environment.NewLine + "select top 0 * into #v04 from v04$000000";
                str += Environment.NewLine + "select top 0 * into #ctgt20 from ctgt20";
                str += Environment.NewLine;

                str += Environment.NewLine + "declare @tinh nvarchar(50)";
                str += Environment.NewLine + "declare @matinh nvarchar(50)";



                str += Environment.NewLine + "declare @quan nvarchar(50)";
                str += Environment.NewLine + "declare @maquan nvarchar(50)";


                str += Environment.NewLine + "declare @phuong nvarchar(50)";
                str += Environment.NewLine + "declare @maphuong nvarchar(50)";

                if(model.Billing != null)
                {
                    str += Environment.NewLine + $"set @tinh = N'%{model.Billing.tinh}%'";
                    str += Environment.NewLine + $"set @quan = N'%{model.Billing.quan.Replace("Quận", "").Replace("Thị xã", "").Replace("Huyện", "").Replace("Thành phố", "").Trim()}%'";
                    str += Environment.NewLine + $"set @phuong = N'%{model.Billing.phuong}%'";

                }


                str += Environment.NewLine + "set @matinh= (select top 1 ma_tinh from hrdmtinh where ten_tinh like @tinh)";
                str += Environment.NewLine + "set @maquan= (select top 1 ma_quan from hrdmquan where ma_tinh = @matinh and ten_quan like @quan)";
                str += Environment.NewLine + "set @maphuong= (select top 1 ma_phuong from hrdmphuong where ma_quan = @maquan and ten_phuong like @phuong)";

                str += Environment.NewLine + " insert into #v04 ( stt_rec," +
                      " ma_tinh,ma_quan,ma_phuong)";
                str += Environment.NewLine + $"select" +
                 $"'0', ma_tinh,ma_quan,ma_phuong from hrdmphuong where ma_phuong = @maphuong";


                str += Environment.NewLine + " insert into #v03 (" +
                      " stt_rec,tt_cod,tt_cod_nt)";


                str += Environment.NewLine + $"select" +
                   $" '',0,0";


                str += Environment.NewLine;
                str += $"DECLARE @status VARCHAR(50) set @status = '{status}'";
                str += Environment.NewLine;
                List<Item> listitem = model.Items.ToList();

                str += Environment.NewLine + "Declare @ma_vt varchar(50) ";
          
                for (i = 0; i < listitem.Count; i++)
                {

                    var item = listitem[i];
                    string ma_vt = item.ma_vt;
                    string ma_kho = "";
                    double so_luong = item.quantity;
                    double gia_nt2 = item.price;
                    double gia_nt = item.price;
                    double total = item.total;
                    double subtotal = item.subtotal;
                    double ck_nt = 0;
                    double thue_nt = 0;
                    string ds_ma_ck = "";
                    double gia_trc_ck = subtotal / so_luong;
                    double tl_ck = (gia_trc_ck - gia_nt) / gia_nt * 100<0?0: (gia_trc_ck - gia_nt) / gia_nt * 100;

                    str += Environment.NewLine + $"set @ma_vt = '{ma_vt}' ";

                    str += Environment.NewLine + " insert into #detail ( " +
                        "stt_rec," +
                        " stt_rec0, " +
                        "ma_ct," +
                        " ngay_ct," +
                        " so_ct, " +
                        "line_nbr," +
                        " ma_vt," +
                        " ma_kho," +
                        " ma_vv, " +
                        "ma_sp," +
                        " so_luong," +
                        "gia2, " +
                        "gia_nt2," +
                        " thue_nt," +
                        "ck_nt," +
                        "ck_tt_nt," +
                        "tien_nt2," +
                        "tien2," +
                        "gia_ban," +
                        "gia_ban_nt," +
                        "ma_bp," +
                        "tl_ck)" +
                        Environment.NewLine + $" select" +
                        $"  '', " +
                                $"'{(i + 1).ToString().PadLeft(3, '0')}'" +
                                $", 'HDO'," +
                                $" '{ngay_ct}', " +
                                $"'{so_ct}'," +
                                $" {i + 1}," +
                                $" @ma_vt, " +
                                $"'{ma_kho}', " +
                                $"''," +
                                $" '', " +
                                $"{so_luong}," +
                                $"{gia_trc_ck}," +
                                $"{gia_trc_ck}, " +
                                $"{thue_nt}," +
                                $"{ck_nt}," +
                                $"{ck_nt}," +
                                $"{subtotal}," +
                                $"{subtotal}," +
                                $"{gia_trc_ck}," +
                                $"{gia_trc_ck}," +
                                $"'ON01'," +
                                $"{tl_ck} ";
                }
                i = 0;

                str += Environment.NewLine + " insert into #master (" +
                    "t_cp_vc," +
                    " stt_rec," +
                    "ma_dvcs," +
                    "ma_ct," +
                    "loai_ct," +
                    "ma_gd," +
                    "ngay_lct," +
                    "ngay_ct," +
                    "so_ct," +
                    "ma_nt," +
                    "ty_gia," +
                    "ma_kh," +
                    "status," +
                    "dept_id," +
                    "s1," +
                    "so_voucher," +
                    "so_dt," +
                    "dia_chi)";




                str += Environment.NewLine + $"select" +
                    $"'{model.Shipping_Fee}', ''," +
                    $"'{ma_dvcs}'" +
                    $",'HDO'" +
                    $",'2'" +
                    $",'2'" +
                    $",'{ngay_ct}'" +
                    $",'{ngay_ct}'" +
                    $",'{so_ct}'," +
                    $"'VND'" +
                    $",'1'," +
                    $"'{model.Billing.phone}'" +
                    $",'1'" +
                    $",'ON01'" +
                    $",'{model.Billing.phone}'," +
                    $"N'{ma_voucher}'," +
                    $"'{model.Billing.phone}'," +
                    $"N'{model.Billing.address}, {model.Billing.phuong},{model.Billing.quan},{model.Billing.tinh}'";


                str += Environment.NewLine + $"declare @table_sufix varchar(6) = '{table_sufix}'";

                if (model.PaymentMethod == "COD")
                {
                    str += Environment.NewLine + $"update #v03 set tt_cod_nt = '{model.TotalDiscountPrice}',";
                    str += Environment.NewLine + $"tt_cod = {model.TotalDiscountPrice}";

                }
                else
                {
                    str += Environment.NewLine + $"update #v03 set  tt_chyen_khoan_nt ={model.TotalDiscountPrice},";
                    str += Environment.NewLine + $"tt_chyen_khoan ={model.TotalDiscountPrice}";
               
                    str += Environment.NewLine + $"Declare @adder nvarchar(500)= N'Phương Thức Thanh Toán: {model.PaymentMethod} '+char(10)+";
                    str += Environment.NewLine + $"N'Ngân Hàng:{model.TranferInfo.bank_name} '+char(10)+N' Chủ tài Khoản:{model.TranferInfo.bank_account_name}'+char(10)+ ";
                    str += Environment.NewLine + $"N' Số tài khoản:{model.TranferInfo.bank_account_number}'+char(10)+ N' Tổng giá trị:{model.TranferInfo.order_total}'+char(10)+ ";
                    str += Environment.NewLine + $"N' Số tài khoản:{model.TranferInfo.bank_account_number}'+char(10)+N' Tổng giá trị:{model.TranferInfo.order_total}'+char(10)+ ";
                    str += Environment.NewLine + $"N' Số tiền được chiết khấu:{model.TranferInfo.discount_value}'+char(10)+ N' Trạng thái thanh toán:{model.TranferInfo.payment_status}' ";


                    str += Environment.NewLine + $"update  #master set dien_giai = @adder ";

                }
                if (model.PickUp!= null )
                {
                    if(model.PickUp != "")
                    {
                        str += Environment.NewLine + $"update  #master set fnote2 = 'Pick Up At: {model.PickUp}'";
                        if (model.PickUp.Trim() != "") str += Environment.NewLine + $"update  #master set fnote2 = 'Pick Up At: {model.PickUp}'";

                    }

                }    
                if(model.Note !=null)
                {
                    if(model.Note !="")
                    {
                        str += Environment.NewLine + $"update  #master set fnote1 = N'{model.Note}'";
                    }    
                }
                if (model.VAT != null|| model.VAT != new VAT())
                {
                    str += Environment.NewLine + $"declare @ten_vt nvarchar(50) = (select ten_vt from dmvt where ma_vt = '{model.Items[0].ma_vt}')";
                   str += Environment.NewLine + $"Insert into #ctgt20 (stt_rec,loai_ct,ma_ct,ngay_ct,ngay_lct,so_ct,so_seri,ten_kh,dia_chi,ten_vt,t_tien2,t_tien_nt2,ty_gia,ma_thue,ma_kh,ma_dvcs, ma_nt,thue_suat,t_thue,t_thue_nt,tk_thue_co,tk_du,ma_mau_ct)";
                    str += Environment.NewLine + $"values('',2,'HDL','{model.CreateDate}','{model.CreateDate}','','{model.VAT.vat_id}','{model.VAT.vat_name}','{model.VAT.vat_address}',@ten_vt,{model.TotalDiscountPrice},0,0,'KT','{model.Billing.phone}','DEOHD','',0,0,0,'','','')";
                }    
                str += Environment.NewLine + $" exec create_CreateOrderDL @table_sufix ";

                var connect = _config.GetConnectionString("DbCnn");
                IDbConnection conn = new SqlConnection(connect);
                var result = await conn.ExecuteAsync(str, commandType: CommandType.Text);

                //str += Environment.NewLine + $" exec haravan_UpdateManv @stt_rec,'{ngay_ct}' ";
                return str;

            }
            catch( Exception e )
            {
                return e.Message;
            }



        }
    }
}
