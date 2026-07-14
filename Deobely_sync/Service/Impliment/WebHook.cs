using API_Adaption.Model;
using Dapper;
using Deobely_sync.DAL;
using Deobely_sync.Infrastructure;
using Deobely_sync.Model;
using Deobely_sync.Service.Interface;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Immutable;
using System.Data;
using System.Text.Json.Nodes;
using static Deobely_sync.Service.Impliment.Webhook;

namespace Deobely_sync.Service.Impliment
{
    public class Webhook : IWebHook
    {
        private readonly ILogger<Webhook> _logger;
        private IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public Webhook(
            ILogger<Webhook> logger,
            IConfiguration config,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        public void Test(object t)
        {
            APILog.log("webhook_nhanh_test", "", t, "send ok", "");
        }

        public async Task<int> AddOrder(OrderNhanhResModel order)
        {
            string sql = $"";
            try
            {
                int table = 81;
                int i = 0;
                string time = DateTime.Now.ToString("yyyyMM");
                string Address = "";
                Address = (order.CustomerAddress ?? "") + " " +
          (order.CustomerWard ?? "") + " " +
          (order.CustomerDistrict ?? "") + " " +
          (order.CustomerCity ?? "");
                sql += Environment.NewLine;
                sql += $"declare @wsID varchar(1)";
                sql += Environment.NewLine;
                sql += $"declare @ma_ct nvarchar(10) = (select top 1 ma_ct from dmct9 where c$ = 'c{table}$')";
                sql += Environment.NewLine;
                sql += $"if exists (select 1 from log_webhook_nhanh where rtrim(ltrim(OrderId))='{order.OrderId.ToString().Trim()}') return";
                sql += Environment.NewLine;

                sql += "declare @voucherNumber varchar(32)";
                sql += Environment.NewLine;
                sql += "SELECT @voucherNumber = ltrim(rtrim(cast(so_ct AS varchar(32)))) from dmct where ma_ct_me in (select ma_ct_me from dmct where ma_ct =  @ma_ct)";
                sql += Environment.NewLine;
                sql += "SELECT @voucherNumber = convert(char(11), convert(numeric, @voucherNumber) + 1)";
                sql += Environment.NewLine + "IF OBJECT_ID('TempDb..#master') IS NOT NULL DROP TABLE #master;";
                sql += Environment.NewLine + "IF OBJECT_ID('TempDb..#detail') IS NOT NULL DROP TABLE #detail;";
                sql += Environment.NewLine + "IF OBJECT_ID('TempDb..#e81') IS NOT NULL DROP TABLE #e81;";
                sql += Environment.NewLine + "IF OBJECT_ID('TempDb..#dmkh') IS NOT NULL DROP TABLE #dmkh;";
                sql += Environment.NewLine + $"select top 0 * into #detail from d{table}$000000";
                sql += Environment.NewLine + $"select top 0 * into #master from m{table}$000000";
                sql += Environment.NewLine + $"select top 0 * into #e81 from e81$000000";
                sql += Environment.NewLine + $"select top 0 * into #dmkh from dmkh";
                sql += Environment.NewLine + "select @wsID = rtrim(val) from options where upper(name) = 'M_WS_ID'";
                sql += Environment.NewLine;
                sql += "create table #idNumber (stt_rec varchar(32)) ";
                sql += Environment.NewLine;
                sql += $"insert into #idNumber exec fs_GetIdentityNumber @wsID,  @ma_ct, c{table}$000000 ";
                sql += Environment.NewLine;
                sql += "declare @rec nvarchar(200)";
                sql += Environment.NewLine;
                sql += "select @rec = stt_rec from #idNumber";
                sql += Environment.NewLine;
                sql += "declare @sct nvarchar(200)";
                sql += Environment.NewLine;
                sql += $"set @sct =(select max(so_ct)+1 from m{table}$000000)";
                sql += Environment.NewLine;
                sql += " drop table #idNumber";
                sql += Environment.NewLine;
                sql += " declare @ngay_ct smalldatetime, @ma_kh nvarchar(32), @SaleChannel nvarchar(16)";
                sql += Environment.NewLine;
                sql += " select @ngay_ct = convert(varchar, getdate(), 111)";
                sql += Environment.NewLine;
                sql += $" select @SaleChannel = '{order.SaleChannel}'";
                sql += Environment.NewLine;
                sql += $" select @ma_kh = ma_kh from dmkh where dien_thoai = '{order.CustomerMobile}'";
                sql += Environment.NewLine;
                sql += $"insert into #dmkh(ma_kh, ten_kh, ten_kh2, dien_thoai, dia_chi, loai_hinh, kh_yn,datetime0,datetime2)";
                sql += Environment.NewLine;
                sql += $"select '{order.CustomerMobile}', N'{order.CustomerName}', N'{order.CustomerName}', '{order.CustomerMobile}', N'{Address.Replace("'", " ")}', '3', '1',getdate(),getdate()";

                sql += Environment.NewLine;
                sql += $"insert into #master (status,stt_rec,ma_ct, so_ct," +
                "ma_dvcs,ngay_ct,ma_nt,ty_gia,ma_nx,ma_kh,t_tien2,t_tien_nt2,t_ck,t_ck_nt,t_thue,t_thue_nt,t_tt,t_tt_nt,thue_suat,tk_thue_no,tk_thue_co,ma_nvbh,fcode1,dien_giai,so_seri,ma_gd,tk,ma_thue,so_dh,loai_ct, ong_ba, dia_chi,so_ct_dms,job_id)  ";
                sql += Environment.NewLine + $"values('0', @rec,@ma_ct," +
                           $" '{order.OrderId}'," +
                           $"'CTY',@ngay_ct,'VND','1','','{order.CustomerMobile}',0,0,0,0,0,0,0,0,0,0,0,'','',N'{order.ShopOrderId}', '','2','','','{order.OrderId}','2', '{order.CustomerName}',N'{order.(order.CustomerAddress ?? "").Replace("'", " ")}','{order.OrderId}','{order.SaleChannel}'" +
                   ")";

                int line_id = 0;

                foreach (NhanhResProductModel product in order.Products)
                {
                    string ma_vt = await NhanhRateLimiter.RunAsync(() =>
                        GetCodeProductByIdNhanh(product.Id, product.Quantity));
                    double gia = 0;
                    double Discount = 0;
                    double ck = 0;
                    double tien = 0;

                    JArray jsonArray = JArray.Parse(ma_vt);

                    foreach (JObject obj in jsonArray)
                    {
                        string code = obj["Code"].ToString();
                        int quantity = (int)obj["Quantity"];
                        string typeId = obj["typeId"].ToString();

                        if (typeId != "9")
                        {
                            gia = (double)product.Price;
                            Discount = (double)product.Discount;
                            ck = Discount * quantity;
                            tien = quantity * gia;
                        }
                        sql += Environment.NewLine + " insert into #detail ( stt_rec, stt_rec0, ma_ct, ngay_ct, so_ct, line_nbr, ma_vt, ma_kho, ma_vv, ma_sp, so_luong,gia_nt, gia_nt2, tien_nt2, thue_nt,ck_nt,ma_ck)" + Environment.NewLine +
                        $" select '', '{(line_id + 1).ToString().PadLeft(3, '0')}', 'HDA', @ngay_ct, '{order.OrderId}', {line_id + 1}, '{code}', '', '', '', {quantity},{gia},{gia}, {tien},0,{ck},'' ";

                        line_id++;
                    }
                }
                i = 0;

                sql += $"exec Insert_InvoiceFromNhanhVN 'm{table}${time}', 'd{table}${time}', 'e{table}${time}'";
                sql += Environment.NewLine;

                DAL_SQL.ExecuteNonquery(sql);
                return 1;
            }
            catch (Exception e)
            {
                var ex = e.Message;
                APILog.ApiLog.logval("err_WebHook_send_sql", ex);
                APILog.ApiLog.logval("err_WebHook_send_sql2", sql);
                return 0;
            }
        }

        public async Task<int> AddOrderUpdate(NhanhVNUpdateModel order)
        {
            string sql = $"";
            try
            {
                int table = 81;
                int i = 0;
                string time = DateTime.Now.ToString("yyyyMM");
                string Address = "";
                Address = (order.CustomerAddress ?? "") + " " +
          (order.CustomerWard ?? "") + " " +
          (order.CustomerDistrict ?? "") + " " +
          (order.CustomerCity ?? "");
                sql += Environment.NewLine;
                sql += $"declare @wsID varchar(1)";
                sql += Environment.NewLine;
                sql += $"declare @ma_ct nvarchar(10) = (select top 1 ma_ct from dmct9 where c$ = 'c{table}$')";
                sql += Environment.NewLine;

                sql += "declare @voucherNumber varchar(32)";
                sql += Environment.NewLine;
                sql += "SELECT @voucherNumber = ltrim(rtrim(cast(so_ct AS varchar(32)))) from dmct where ma_ct_me in (select ma_ct_me from dmct where ma_ct =  @ma_ct)";
                sql += Environment.NewLine;
                sql += "SELECT @voucherNumber = convert(char(11), convert(numeric, @voucherNumber) + 1)";
                sql += Environment.NewLine + "IF OBJECT_ID('TempDb..#master') IS NOT NULL DROP TABLE #master;";
                sql += Environment.NewLine + "IF OBJECT_ID('TempDb..#detail') IS NOT NULL DROP TABLE #detail;";
                sql += Environment.NewLine + "IF OBJECT_ID('TempDb..#e81') IS NOT NULL DROP TABLE #e81;";
                sql += Environment.NewLine + "IF OBJECT_ID('TempDb..#dmkh') IS NOT NULL DROP TABLE #dmkh;";
                sql += Environment.NewLine + $"select top 0 * into #detail from d{table}$000000";
                sql += Environment.NewLine + $"select top 0 * into #master from m{table}$000000";
                sql += Environment.NewLine + $"select top 0 * into #e81 from e81$000000";
                sql += Environment.NewLine + $"select top 0 * into #dmkh from dmkh";
                sql += Environment.NewLine + "select @wsID = rtrim(val) from options where upper(name) = 'M_WS_ID'";
                sql += Environment.NewLine;
                sql += "create table #idNumber (stt_rec varchar(32)) ";
                sql += Environment.NewLine;
                sql += $"insert into #idNumber exec fs_GetIdentityNumber @wsID,  @ma_ct, c{table}$000000 ";
                sql += Environment.NewLine;
                sql += "declare @rec nvarchar(200)";
                sql += Environment.NewLine;
                sql += "select @rec = stt_rec from #idNumber";
                sql += Environment.NewLine;
                sql += "declare @sct nvarchar(200)";
                sql += Environment.NewLine;
                sql += $"set @sct =(select max(so_ct)+1 from m{table}$000000)";
                sql += Environment.NewLine;
                sql += " drop table #idNumber";
                sql += Environment.NewLine;
                sql += " declare @ngay_ct smalldatetime, @ma_kh nvarchar(32), @SaleChannel nvarchar(16)";
                sql += Environment.NewLine;
                sql += " select @ngay_ct = convert(varchar, getdate(), 111)";
                sql += Environment.NewLine;
                sql += $" select @SaleChannel = '{order.channel}'";
                sql += Environment.NewLine;
                sql += $" select @ma_kh = ma_kh from dmkh where dien_thoai = '{order.CustomerMobile}'";
                sql += Environment.NewLine;
                sql += $"insert into #dmkh(ma_kh, ten_kh, ten_kh2, dien_thoai, dia_chi, loai_hinh, kh_yn,datetime0,datetime2)";
                sql += Environment.NewLine;
                sql += $"select '{order.CustomerMobile}', N'{order.CustomerName}', N'{order.CustomerName}', '{order.CustomerMobile}', N'{Address.Replace("'", " ")}', '3', '1',getdate(),getdate()";

                sql += Environment.NewLine;
                sql += $"insert into #master (status,stt_rec,ma_ct, so_ct," +
                "ma_dvcs,ngay_ct,ma_nt,ty_gia,ma_nx,ma_kh,t_tien2,t_tien_nt2,t_ck,t_ck_nt,t_thue,t_thue_nt,t_tt,t_tt_nt,thue_suat,tk_thue_no,tk_thue_co,ma_nvbh,fcode1,dien_giai,so_seri,ma_gd,tk,ma_thue,so_dh,loai_ct, ong_ba, dia_chi,so_ct_dms,job_id)  ";
                sql += Environment.NewLine + $"values('0', @rec,@ma_ct," +
                           $" '{order.id}'," +
                           $"'CTY',@ngay_ct,'VND','1','',@ma_kh,0,0,0,0,0,0,0,0,0,0,0,'','',N'{order.ShopOrderId}', '','2','','','{order.id}','2', '{order.CustomerName}',N'{(order.CustomerAddress ?? "").Replace("'", " ")}','{order.id}','{order.saleChannel}'" +
                   ")";

                int line_id = 0;
                foreach (NhanhResUpdateProductModel product in order.Products)
                {
                    string ma_vt = await NhanhRateLimiter.RunAsync(() =>
                        GetCodeProductByIdNhanh(product.productId ?? 0, product.Quantity ?? 0));
                    double gia = 0;
                    double Discount = 0;
                    double ck = 0;
                    double tien = 0;

                    JArray jsonArray = JArray.Parse(ma_vt);

                    foreach (JObject obj in jsonArray)
                    {
                        string code = obj["Code"].ToString();
                        int quantity = (int)obj["Quantity"];
                        string typeId = obj["typeId"].ToString();

                        if (typeId != "9")
                        {
                            gia = (double)(product.Price ?? 0);
                            Discount = double.Parse(product.Discount ?? "0");
                            ck = Discount * quantity;
                            tien = quantity * gia;
                        }
                        sql += Environment.NewLine + " insert into #detail ( stt_rec, stt_rec0, ma_ct, ngay_ct, so_ct, line_nbr, ma_vt, ma_kho, ma_vv, ma_sp, so_luong,gia_nt, gia_nt2, tien_nt2, thue_nt,ck_nt,ma_ck)" + Environment.NewLine +
                        $" select '', '{(line_id + 1).ToString().PadLeft(3, '0')}', 'HDA', @ngay_ct, '{order.id}', {line_id + 1}, '{code}', '', '', '', {quantity},{gia},{gia}, {tien},0,{ck},'' ";

                        line_id++;
                    }
                }
                i = 0;

                sql += $"exec Insert_InvoiceFromNhanhVN 'm{table}${time}', 'd{table}${time}', 'e{table}${time}'";
                sql += Environment.NewLine;

                DAL_SQL.ExecuteNonquery(sql);
                return 1;
            }
            catch (Exception e)
            {
                var ex = e.Message;
                APILog.ApiLog.logval("err_WebHook_send_sql_update", ex);
                APILog.ApiLog.logval("err_WebHook_send_sql_update2", sql);
                return 0;
            }
        }

        public class Product
        {
            public string Code { get; set; }
            public int Quantity { get; set; }
            public string typeId { get; set; }
        }

        /// <summary>
        /// Gọi Nhanh lấy chi tiết đơn - qua IHttpClientFactory + Polly retry.
        /// Lưu ý: nếu Nhanh trả 429, Polly sẽ tự retry với backoff 2/4/8s.
        /// </summary>
        public async Task<NhanhVNUpdateModel?> GetOrderDetailAsync(string orderId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("nhanh");
                string token = _config.GetValue<string>("NhanhVn:accesToken");
                string version = _config.GetValue<string>("NhanhVn:version");
                string appId = _config.GetValue<string>("NhanhVn:appId");
                string businessId = _config.GetValue<string>("NhanhVn:idNhanh");

                var content = new MultipartFormDataContent
                {
                    { new StringContent(version), "version" },
                    { new StringContent(appId), "appId" },
                    { new StringContent(businessId), "businessId" },
                    { new StringContent(token), "accessToken" },
                    { new StringContent("{\"id\":" + orderId + "}"), "data" }
                };

                var response = await client.PostAsync("api/order/index", content);
                content.Dispose();

                if (!response.IsSuccessStatusCode)
                {
                    APILog.ApiLog.logval("err_nhanh_order_detail",
                        $"orderId={orderId}, status={(int)response.StatusCode}");
                    return null;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                JObject responseData = JObject.Parse(responseContent);
                JObject dataObj = JObject.Parse(responseData["data"].ToString());
                JObject ordersObj = JObject.Parse(dataObj["orders"].ToString());

                JToken? orderToken;
                if (!ordersObj.TryGetValue(orderId, out orderToken)) return null;

                return JsonConvert.DeserializeObject<NhanhVNUpdateModel>(orderToken.ToString());
            }
            catch (Exception ex)
            {
                APILog.ApiLog.logval("err_WebHook_send", ex.Message);
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<string> GetCodeProductByIdNhanh(int id, int quantity)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("nhanh");
                string token = _config.GetValue<string>("NhanhVn:accesToken");
                string version = _config.GetValue<string>("NhanhVn:version");
                string appId = _config.GetValue<string>("NhanhVn:appId");
                string businessId = _config.GetValue<string>("NhanhVn:idNhanh");

                var content = new MultipartFormDataContent
                {
                    { new StringContent(version), "version" },
                    { new StringContent(appId), "appId" },
                    { new StringContent(token), "accessToken" },
                    { new StringContent(businessId), "businessId" },
                    { new StringContent(id.ToString()), "data" }
                };

                var response = await client.PostAsync("api/product/detail", content);
                content.Dispose();

                if (!response.IsSuccessStatusCode)
                {
                    APILog.ApiLog.logval("err_nhanh_product_detail",
                        $"id={id}, status={(int)response.StatusCode}");
                    return "";
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                JObject obj = JObject.Parse(responseContent);
                List<Product> products = new List<Product>();
                if (obj["data"][id.ToString()]["typeId"].ToString() == "9")
                {
                    JObject combosData = JObject.Parse(obj["data"][id.ToString()]["combos"].ToString());
                    foreach (var item in combosData)
                    {
                        Product product = new Product
                        {
                            Code = (string)item.Value["code"],
                            Quantity = quantity * (int)item.Value["quantity"],
                            typeId = obj["data"][id.ToString()]["typeId"].ToString()
                        };
                        products.Add(product);
                    }
                }
                else
                {
                    Product product = new Product
                    {
                        Code = obj["data"][id.ToString()]["code"].ToString(),
                        Quantity = (int)quantity,
                        typeId = obj["data"][id.ToString()]["typeId"].ToString()
                    };
                    products.Add(product);
                }
                string jsonString = JsonConvert.SerializeObject(products);
                return jsonString;
            }
            catch (Exception ex)
            {
                APILog.ApiLog.logval("err_WebHook_send", ex.Message);
                _logger.LogError(ex.Message);
                return "";
            }
        }

        public async Task<string> ResetAcessToken()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("nhanh");
                string accessCode = _config.GetValue<string>("NhanhVn:accessCode");
                string version = _config.GetValue<string>("NhanhVn:version");
                string appId = _config.GetValue<string>("NhanhVn:appId");
                string businessId = _config.GetValue<string>("NhanhVn:idNhanh");

                var content = new MultipartFormDataContent
                {
                    { new StringContent(version), "version" },
                    { new StringContent(appId), "appId" },
                    { new StringContent(accessCode), "accessCode" },
                    { new StringContent(businessId), "businessId" }
                };

                var response = await client.PostAsync("api/oauth/access_token", content);
                content.Dispose();

                var responseContent = await response.Content.ReadAsStringAsync();
                JObject obj = JObject.Parse(responseContent);
                string newAccessToken = obj["accessToken"].ToString();
                return newAccessToken;
            }
            catch (Exception ex)
            {
                APILog.ApiLog.logval("err_WebHook_send", ex.Message);
                _logger.LogError(ex.Message);
                return "";
            }
        }

        public async void GetCity()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("nhanh");
                string url = _config.GetValue<string>("NhanhVn:api_url") + "/api/shipping/location";
                string token = _config.GetValue<string>("NhanhVn:accesToken");
                string version = _config.GetValue<string>("NhanhVn:version");
                string appId = _config.GetValue<string>("NhanhVn:appId");
                string businessId = _config.GetValue<string>("NhanhVn:idNhanh");

                var content = new MultipartFormDataContent
                {
                    { new StringContent(version), "version" },
                    { new StringContent(appId), "appId" },
                    { new StringContent(token), "accessToken" },
                    { new StringContent(businessId), "businessId" }
                };

                var response = await client.PostAsync("api/shipping/location", content);
                content.Dispose();

                var responseContent = await response.Content.ReadAsStringAsync();
                JObject obj = JObject.Parse(responseContent);
                JArray data = JArray.Parse(obj["data"].ToString());
                string sql = $"";
                foreach (var item in data)
                {
                    sql += Environment.NewLine;
                    sql += $"insert into nhanh_city(name,ma_tinh,id_nhanh)  values ";
                    sql += $"(N'{item["name"].ToString()}',N'',{item["id"].ToString()})";
                }
                DAL_SQL.ExecuteNonquery(sql);
            }
            catch (Exception ex)
            {
                APILog.ApiLog.logval("err_WebHook_send", ex.Message);
                _logger.LogError(ex.Message);
            }
        }

        public async Task<string> GetDistrict()
        {
            try
            {
                DataSet ds = DAL.DAL_SQL.ExecuteGetlistDataset("select * from nhanh_city where id>31");
                var client = _httpClientFactory.CreateClient("nhanh");
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    string param = "{\"type\":\"DISTRICT\",\"parentId\":" + r["id_nhanh"].ToString() + "}";
                    string token = _config.GetValue<string>("NhanhVn:accesToken");
                    string version = _config.GetValue<string>("NhanhVn:version");
                    string appId = _config.GetValue<string>("NhanhVn:appId");
                    string businessId = _config.GetValue<string>("NhanhVn:idNhanh");

                    var content = new MultipartFormDataContent
                    {
                        { new StringContent(version), "version" },
                        { new StringContent(appId), "appId" },
                        { new StringContent(token), "accessToken" },
                        { new StringContent(businessId), "businessId" },
                        { new StringContent(param), "data" }
                    };

                    var response = await client.PostAsync("api/shipping/location", content);
                    content.Dispose();

                    var responseContent = await response.Content.ReadAsStringAsync();
                    JObject obj = JObject.Parse(responseContent);
                    JArray data = JArray.Parse(obj["data"].ToString());
                    string sql = $"";
                    foreach (var item in data)
                    {
                        sql += Environment.NewLine;
                        sql += $"insert into nhanh_district(name,ma_quan,id_nhanh,id_tinh)  values ";
                        sql += $"(N'{item["name"].ToString().Replace("'", "''")}',N'',{item["id"].ToString()},{r["id_nhanh"].ToString()})";
                    }
                    DAL_SQL.ExecuteNonquery(sql);

                    await Task.Delay(2500);
                }
                return "OK";
            }
            catch (Exception ex)
            {
                APILog.ApiLog.logval("err_WebHook_send", ex.Message);
                _logger.LogError(ex.Message);
                return "";
            }
        }

        public async Task<string> GetWard()
        {
            try
            {
                DataSet ds = DAL.DAL_SQL.ExecuteGetlistDataset("select * from nhanh_district");
                var client = _httpClientFactory.CreateClient("nhanh");
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    string param = "{\"type\":\"WARD\",\"parentId\":" + r["id_nhanh"].ToString() + "}";
                    string token = _config.GetValue<string>("NhanhVn:accesToken");
                    string version = _config.GetValue<string>("NhanhVn:version");
                    string appId = _config.GetValue<string>("NhanhVn:appId");
                    string businessId = _config.GetValue<string>("NhanhVn:idNhanh");

                    var content = new MultipartFormDataContent
                    {
                        { new StringContent(version), "version" },
                        { new StringContent(appId), "appId" },
                        { new StringContent(token), "accessToken" },
                        { new StringContent(businessId), "businessId" },
                        { new StringContent(param), "data" }
                    };

                    var response = await client.PostAsync("api/shipping/location", content);
                    content.Dispose();

                    var responseContent = await response.Content.ReadAsStringAsync();
                    JObject obj = JObject.Parse(responseContent);
                    JArray data = JArray.Parse(obj["data"].ToString());
                    string sql = $"";
                    foreach (var item in data)
                    {
                        sql += Environment.NewLine;
                        sql += $"insert into nhanh_ward(name,ma_phuong,id_nhanh,id_quan)  values ";
                        sql += $"(N'{item["name"].ToString().Replace("'", "''")}',N'',{item["id"].ToString()},{r["id_nhanh"].ToString()})";
                    }
                    if (sql != "") DAL_SQL.ExecuteNonquery(sql);

                    await Task.Delay(2500);
                }
                return "OK";
            }
            catch (Exception ex)
            {
                APILog.ApiLog.logval("err_WebHook_send", ex.Message);
                _logger.LogError(ex.Message);
                return "";
            }
        }

        public void AutoInsertLocation()
        {
            DataSet ds = DAL.DAL_SQL.ExecuteGetlistDataset("select * from nhanh_district");
            int stt = 0;
            string sql = $"";
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                stt++;
                sql += Environment.NewLine;
                sql += $"INSERT INTO dmquan(ma_quan ,ten_quan,ten_quan2,ma_tinh,ma_qg,status)  values ";
                sql += $"('d_{stt.ToString()}',N'{r["name"].ToString()}',N'{r["name"].ToString()}','{r["ma_tinh"].ToString()}','c_VN',1)";
            }
            if (sql != "") DAL_SQL.ExecuteNonquery(sql);
        }
    }
}
