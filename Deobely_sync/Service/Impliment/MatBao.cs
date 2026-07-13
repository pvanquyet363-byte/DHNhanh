using API_Adaption.Model;
using Azure.Core;
using Dapper;
using Deobely_sync.DAL;
using Deobely_sync.Model;
using Deobely_sync.Service.Interface;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
using Google.Type;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog.Fluent;
using System.Data;
using System.Globalization;
using System.Net.Http;
using System.Reflection;
using System.Text;
using static Google.Protobuf.WellKnownTypes.Field.Types;

namespace Deobely_sync.Service.Impliment
{
    public class MatBao: IMatBao
    {
        private readonly ILogger<MatBao> _logger;
        private IConfiguration _config;

        public MatBao(ILogger<MatBao> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<ResponseModel> GetInvoice(ReqParamModel param)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string url = _config.GetValue<string>("MatBao:url")+ "/api/v2/invoice/SearchInvDetailByDate";
                string username = _config.GetValue<string>("MatBao:ApiUserName");
                string pass = _config.GetValue<string>("MatBao:ApiPassword");

                //  dd//MM//yyyy
                string fromDate = ConvertDateString(param.fromDate);
                string toDate = ConvertDateString(param.toDate);

                Object body = new
                {
                    ApiUserName = username,
                    ApiPassword = pass,
                    ApiInvPattern = param.ApiInvPattern,
                    ApiInvSerial = param.ApiInvSerial,
                    ArisingDateFrom = fromDate,
                    ArisingDateTo = toDate,
                    No = 0,
                    TypeInv = 0,
                    StatusInv =  0,
                    InfoCus = ""

                };
                var httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage.Method = HttpMethod.Post;
                httpRequestMessage.RequestUri = new Uri(url);

                var httpContent = new StringContent(JObject.FromObject(body).ToString(), Encoding.UTF8, "application/json");
                httpRequestMessage.Content = httpContent;

                var response = await httpClient.SendAsync(httpRequestMessage);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode) return new ResponseModel(200, response.StatusCode.ToString());
                JObject obj = JObject.Parse(responseContent);

                if (!(obj["data"] is JArray dataArray))
                {
                    return new ResponseModel(500, obj["messages"].ToString());
                }

                JArray lstInvoice = (JArray)obj["data"];

                Object mapMasterField = new
                {
                    No= "so_ct",
                    Pattern= "contract_id",
                    Serial= "bcontract_id",
                    Fkey= "fee_id",
                    CusCode= "cus_code",
                    CusName= "dien_giai",
                    CusTaxCode= "prd_id",
                    CusAddress= "cus_add",
                    CusEmail= "cus_email",
                    SO= "s1",
                    PaymentMethod= "fnote1",
                    ArisingDate= "ngay_ct",
                    CreateDate= "ngay_lct",
                    Total= "t_tt",
                    Amount="t_tien",
                    MaThongDiep= "ma_thongdiep",
                    MLTDiep= "fcode1",
                    MCCQT= "mccqt",
                    LoaiHoaDon= "loai_hd",
                    TrangThaiHD= "tt_hd",
                    Extra1= "extral",
                    Extra = "fcode2",

                };
                Object mapDetailField = new
                {
                    Code= "ma_vt",
                    Name= "ten_vt",
                    Price= "gia",
                    Quantity= "s4",
                    TotalChuaCK= "tien0",
                    Discount= "ck",
                    DiscountAmount= "tl_ck_i",
                    VATRate= "thue",
                    VATRateName= "s1",
                    VATAmount= "thue_nt",
                    Amount= "tt",
                    TinhChat= "tc",
                    TinhChatName= "ghi_chu"
                };
                Object mapOptionsField = new
                {
                    OptExtra1 = "OptExtra1",
                    OptExtra2 = "OptExtra2",
                    OptExtra3 = "OptExtra3",
                    OptExtra4 = "OptExtra4",
                    OptExtra5 = "OptExtra5",
                    OptExtra6 = "OptExtra6",
                    OptExtra7 = "OptExtra7",
                    OptExtra8 = "OptExtra8",
                    OptExtra9 = "OptExtra9",
                    OptExtra10 = "OptExtra10",
                    OptExtra11 = "OptExtra11",
                    OptExtra12 = "OptExtra12",
                    OptExtra13 = "OptExtra13",
                    OptExtra14 = "OptExtra14",
                    OptExtra15 = "OptExtra15",
                    OptExtra16 = "OptExtra16",
                    OptExtra17 = "OptExtra17",
                    OptExtra18 = "OptExtra18",
                    OptExtra19 = "OptExtra19",
                    OptExtra20 = "OptExtra20"
                };

                foreach (JObject inv in lstInvoice)
                {
                    InvoiceMatBaoModel m = inv.ToObject<InvoiceMatBaoModel>();
                    string str = $"";
                    str += Environment.NewLine;

                    string createDate = m.CreateDate?.ToString();

                    // m.CreateDate 8 / 13 / 2024 8:39:29 AM
                    if (m.CreateDate == null)
                    {
                        continue;
                    }
                    string table_sufix = m.CreateDate?.ToString("yyyyMM");
                    
                    string Fkey = inv["Fkey"].ToString();
                    str += Environment.NewLine + $" if exists (select 1 from mOpv${table_sufix} where fee_id = N'{Fkey}') return ";

                    string temp1 = $" insert into mOpv${table_sufix} ( stt_rec,ma_dvcs,ma_ct,";
                    string temp2 = "select '','','',";

                    foreach (PropertyInfo propertyInfo in mapMasterField.GetType().GetProperties())
                    {
                        var propertyName = propertyInfo.Name;
                        var propertyValue = propertyInfo.GetValue(mapMasterField);
                        var value = inv[propertyName].ToString();

                        if (propertyName == "ArisingDate" || propertyName == "CreateDate")
                        {
                            // Định dạng của chuỗi đầu vào
                            value = value.Split(" ")[0];
                            if (value == null || value == "")
                            {
                                continue;
                            }
                            value = ConvertDateStringSql(value);
                        }
                        if (propertyName == "Total" || propertyName == "Amount")
                        {
                            value = value.Replace(",", ".");
                        }

                        temp1 += propertyValue + ",";
                        temp2 += "N'"+ value + "',";
                    }
                    str += Environment.NewLine + temp1.Remove(temp1.Length - 1) + ")";
                    str += Environment.NewLine + temp2.Remove(temp2.Length - 1) + "";
                    int stt_rec0 = 0;
                    foreach(ProductMatBaoModel d in m.Products)
                    {
                        JObject product = JObject.FromObject(d);
                        string d1 = $" insert into dOpv${table_sufix} ( stt_rec, stt_rec0,ma_ct,ngay_ct,so_ct,fee_id,";
                        string d2 = $"select '',N'{stt_rec0.ToString()}','','','',N'{Fkey}',";
                        foreach (PropertyInfo propertyInfo in mapDetailField.GetType().GetProperties())
                        {
                            var propertyName = propertyInfo.Name;
                            var propertyValue = propertyInfo.GetValue(mapDetailField);
                            var detailValue = product[propertyName].ToString();
                            if (propertyValue == "Price" || propertyValue == "Quantity" || propertyValue == "TotalChuaCK" || propertyValue == "Discount" || propertyValue == "VATRate" || propertyValue == "VATAmount" || propertyValue == "Amount")
                            {
                                detailValue = detailValue.Replace(",", ".");
                            }

                            d1 += propertyValue + ",";
                            d2 += "N'" + detailValue + "',";
                        }
                        str += Environment.NewLine + d1.Remove(d1.Length - 1) + ")";
                        str += Environment.NewLine + d2.Remove(d2.Length - 1) + "";
                        stt_rec0++;
                    }

                    DAL_SQL_Thue.ExecuteNonquery(str);
                }
              
                return new ResponseModel(200, "") ;
            }
            catch (Exception ex)
            {
                APILog.ApiLog.logval("err_WebHook_send", ex.Message);
                _logger.LogError(ex.Message);
                return new ResponseModel(500,ex.Message);
            }
        }
        public string GetValue(object obj, string key)
        {
            PropertyInfo property = obj.GetType().GetProperty(key);
            if (property != null)
            {
                object value = property.GetValue(obj);
                return value.ToString();
            }

            return "";
        }
        string ConvertDateString(string date)
        {
            string day = date.Split("/")[0].PadLeft(2, '0');
            string month = date.Split("/")[1].PadLeft(2, '0');
            string year = date.Split("/")[2];
            return $"{year}-{month}-{day}";
        }

        public string ConvertDateStringSql(string value)
        {
            // Tách ngày, tháng, năm từ chuỗi
            string month = value.Split("/")[0].PadLeft(2, '0');
            string day = value.Split("/")[1].PadLeft(2, '0');
            string year = value.Split("/")[2];

            // Ghép lại chuỗi với định dạng "dd/MM/yyyy"
            string formattedValue = day + "/" + month + "/" + year;

            // Định dạng và chuyển đổi chuỗi ngày thành kiểu DateTime của SQL
            string format = "dd/MM/yyyy";
            CultureInfo provider = new CultureInfo("vi-VN");
            return System.DateTime.ParseExact(formattedValue, format, provider).ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
