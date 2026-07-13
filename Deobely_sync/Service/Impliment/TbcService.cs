using Dapper;
using Deobely_sync.DAL;
using Deobely_sync.Model;
using Deobely_sync.Service.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Deobely_sync.Service.Impliment
{
    public class TbcRepository: ITbc
    {
        private readonly ILogger<TbcRepository> _logger;
        private IConfiguration _config;

        public TbcRepository(ILogger<TbcRepository> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<List<ProductModel>> GetProduct()
        {
            var product = new ProductModel();

            var connect = _config.GetConnectionString("DbCnn");
            IDbConnection conn = new SqlConnection(connect);
            
                var result = await conn.QueryMultipleAsync($"select * from dmvt ",
                       commandType: CommandType.Text);
            List<ProductModel> table1 = (await result.ReadAsync<ProductModel>()).ToList();

            return table1;
        }

        public async Task<string> CreateTbc(TbcModel model)
        {
            try
            {
               
                int table = 81;
                HttpClientApp client = new HttpClientApp();
                string time = DateTime.Now.ToString("yyyyMM");
                string host = _config.GetValue<string>("WP:url");
                string sql = $"";
                sql += Environment.NewLine;
                sql += $"declare @wsID varchar(1)                                          ";
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
                sql += Environment.NewLine + $"select top 0 * into #detail from d{table}$000000";
                sql += Environment.NewLine + $"select top 0 * into #master from m{table}$000000";
                sql += Environment.NewLine + $"select top 0 * into #e81 from e81$000000";
                sql += Environment.NewLine + "select @wsID = rtrim(val) from options where upper(name) = 'M_WS_ID'      ";
                sql += Environment.NewLine;
                sql += "create table #idNumber (stt_rec varchar(32)) ";
                sql += Environment.NewLine;
                sql += $"insert into #idNumber exec fs_GetIdentityNumber @wsID,  @ma_ct, c{table}$000000  ";
                sql += Environment.NewLine;
                sql += "declare @rec nvarchar(200)                                                ";
                sql += Environment.NewLine;
                sql += "select @rec = stt_rec from #idNumber                                      ";
                sql += Environment.NewLine;
                sql += "declare @sct nvarchar(200)                                                ";
                sql += Environment.NewLine;
                sql += $"set @sct =(select max(so_ct)+1 from m{table}$000000)";
                sql += Environment.NewLine;
                sql += " drop table #idNumber";
                sql += Environment.NewLine;
                sql += $"insert into #master (ma_ck,diem_tl,t_diem_so,t_diem_dh, so_ct_dms," +
                "status,stt_rec,ma_ct, so_ct," +
                "ma_dvcs,ngay_ct,ma_nt,ty_gia,ma_nx,ma_kh,t_tien2,t_tien_nt2,t_ck,t_ck_nt,t_thue,t_thue_nt,t_tt,t_tt_nt,thue_suat,tk_thue_no,tk_thue_co,ma_nvbh,fcode1,fcode2,dien_giai,so_seri,ma_gd,tk,ma_thue,so_dh,loai_ct)  ";
                sql += Environment.NewLine + $"values(N'{model.ma_ck}',N'{model.diem_tl}','{model.t_diem_so}','{model.t_diem_dh}','{model.so_ct_dms}','{model.status}', @rec,@ma_ct," +
                           $" @voucherNumber," +
                           $"'{model.ma_dvcs}','{model.ngay_ct}','{model.ma_nt}','{model.ty_gia}','{model.ma_nx}','{model.ma_kh}','{model.t_tien2}','{model.t_tien_nt2}','{model.t_ck}','{model.t_ck_nt}','{model.t_thue}','{model.t_thue_nt}','{model.t_tt}','{model.t_tt_nt}','{model.thue_suat}','{model.tk_thue_no}','{model.tk_thue_co}','{model.ma_nvbh}','{model.fcode1}',N'{model.kenhbanhang}',N'{model.dien_giai.Replace("'", "''")}', @voucherNumber,'{model.ma_gd}','{model.tk}','{model.ma_thue}','{model.so_dh}','{model.loai_ct}'" + 
                   ")";
                sql += Environment.NewLine;
                sql += $"insert into #e81 (stt_rec,so_hd,ngay_hd,ht_tt,dd_gh,dd_nh, so_vd,so_con,dv_vc)";
                sql += Environment.NewLine + $"values(@rec,N'{model.so_ct_dms}','{model.ngay_ct}','',N'{model.dd_gh}','','','',''" +")";
                sql += Environment.NewLine;
                int a = 0;
                foreach (var detail in model.detail)
                {
                    //if (detail.ck != 0) detail.ma_vt = "CK001";
                    a++;
                    sql += $"insert into #detail (ma_ck,diem,dien_giai_km," +
                    "stt_rec," +
                    "stt_rec0," +
                    "ma_ct,so_ct," +
                     "ma_vt,dvt,ma_kho,ma_lo,so_luong,gia2,gia_nt2,tien2,tien_nt2,tl_ck,ck,ck_nt,tt,tt_nt,gc_td1,ma_td3,ma_bp,ma_vv,ma_phi," +
                    "ngay_ct,km_yn,ma_sp,so_lsx,he_so,ma_vi_tri,tk_vt,gia_nt,gia,tien_nt,tien,thue,thue_nt,xstatus,xaction,cp_bh,cp_bh_nt,cp_vc,cp_vc_nt,cp_khac,cp_khac_nt,cp,cp_nt,stt_rec_ct,stt_rec0ct,gia_ban0,gia_ban_nt0,gia_ban,gia_ban_nt,gia_ck,gia_ck_nt,ck_tt,ck_tt_nt,sl_dh,sl_xuat,sl_giao,stt_rec_dh,stt_rec0dh,dh_so,dh_ln,stt_rec_px,stt_rec0px,px_so,px_ln,stt_rec_gh,stt_rec0gh,stt_rec_pn,stt_rec0pn,tk_gv,tk_dt,tk_ck,tk_cpbh,px_gia_dd,ma_nvbh_i,line_nbr,ma_hd,ma_ku,so_dh_i,ma_td1,ma_td2,sl_td1,sl_td2,sl_td3,ngay_td1,ngay_td2,ngay_td3,gc_td2,gc_td3,s1,s2,s3,s4,s5,s6,s7,s8,s9,gt_nt,gt)" +
                    $"values( N'{detail.ma_ck}','{detail.diem}',N'{detail.dien_giai_km}'," +
                    $"@rec," +
                    $"'{a.ToString("D3")}'," +
                    "@ma_ct," +
                    $" @voucherNumber," +
                    $"'{detail.ma_vt}',N'{detail.dvt}','{model.ma_dvcs}','{detail.ma_lo}',{detail.so_luong},{detail.gia2},{detail.gia_nt2},{detail.tien2},{detail.tien_nt2},{detail.tl_ck},{detail.ck},{detail.ck_nt},{detail.tt},{detail.tt_nt},'{detail.gc_td1.Replace("'", "''")}','{detail.ma_td3}','{detail.ma_bp}','{detail.ma_vv}','{detail.ma_phi}'," +
                   $"'{model.ngay_ct}','{detail.km_yn}','','',1,'','{detail.tk_vt}',{detail.tien2},{detail.tien2},{detail.tien2},0,0,0,'','',0,0,0,0,0,0,0,0,'','',0,0,0,0,0,0,0,0,0,0,0,'','','',0,'','','',0,'','','','','{detail.tk_gv}','{detail.tk_dt}','','',0,'',{a},'','','','','',0,0,0,'','','','','','','','',0,0,0,'','','',0,0)";
                    sql += Environment.NewLine;
                    sql += $"update #detail set tk_ck = '{detail.tk_ck}', tk_cpbh = '{detail.tk_cpbh}' where  stt_rec0 =  '{a.ToString("D3")}' ";
                }
                sql += Environment.NewLine;
                sql += $"Insert into c{table}$000000 (stt_rec,ngay_ct,so_ct,ma_dvcs,status,user_id0) values (@rec,'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}',@voucherNumber,'CTY','{model.status}','10061')";

                sql += Environment.NewLine;
                sql += $"Insert into i{table}${time} (stt_rec,ngay_ct,so_ct,ma_dvcs,status,user_id0,c$,m$,d$) values (@rec,'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}',@voucherNumber,'CTY','{model.status}','10061','c{table}${time}','m{table}${time}','d{table}${time}')";
                sql += Environment.NewLine;
                sql += $"exec Insert_Invoice 'm{table}${time}', 'd{table}${time}', 'e{table}${time}'";
                sql+= Environment.NewLine;
                
                DAL_SQL.ExecuteNonquery(sql);
                return "ok";
            }
            catch (Exception e)
            {
                var ex = e.Message;
                Console.WriteLine(ex);
                return ex;
            }
        }


        public async Task<string> CreateTbc2(TbcModel model)
        {
            try
            {
                int table = 76;
                HttpClientApp client = new HttpClientApp();
                string time = DateTime.Now.ToString("yyyyMM");
                string host = _config.GetValue<string>("WP:url");
                string sql = $"";
                sql += Environment.NewLine;
                sql += $"declare @wsID varchar(1)                                          ";
                sql += Environment.NewLine;
                sql += $"declare @ma_ct nvarchar(10) = (select top 1 ma_ct from dmct9 where c$ = 'c{table}$')";
                sql += Environment.NewLine;
                sql += "declare @voucherNumber varchar(32)\r\n\t\tSELECT @voucherNumber =   ltrim(rtrim(cast(so_ct AS varchar(32)))) from dmct where ma_ct_me in (select ma_ct_me from dmct where ma_ct =  @ma_ct)\r\n\t\t-- Tăng số chứng từ\r\n\t\tSELECT @voucherNumber = convert(char(11),convert(numeric,@voucherNumber)+1)\t\r\n\t\texec SSELIB$App$IncreaseVoucherNumber '',  @ma_ct, @voucherNumber";

                sql += Environment.NewLine;
                sql += "SELECT @voucherNumber = convert(char(11), convert(numeric, @voucherNumber) + 1)";
                sql += Environment.NewLine + "IF OBJECT_ID('TempDb..#master') IS NOT NULL DROP TABLE #master;";
                sql += Environment.NewLine + "IF OBJECT_ID('TempDb..#detail') IS NOT NULL DROP TABLE #detail;";
                sql += Environment.NewLine + $"select top 0 * into #detail from d{table}$000000";
                sql += Environment.NewLine + $"select top 0 * into #master from m{table}$000000";
                sql += Environment.NewLine + "select @wsID = rtrim(val) from options where upper(name) = 'M_WS_ID'      ";
                sql += Environment.NewLine;
                sql += "create table #idNumber (stt_rec varchar(32))                              ";
                sql += Environment.NewLine;
                sql += $"insert into #idNumber exec fs_GetIdentityNumber @wsID,  @ma_ct, c{table}$000000  ";
                sql += Environment.NewLine;
                sql += "declare @rec nvarchar(200)                                                ";
                sql += Environment.NewLine;
                sql += "select @rec = stt_rec from #idNumber                                      ";
                sql += Environment.NewLine;
                sql += "declare @sct nvarchar(200)                                                ";
                sql += Environment.NewLine;
                sql += $"set @sct =(select max(so_ct)+1 from m{table}$000000)";
                sql += Environment.NewLine;
                sql += " drop table #idNumber";
                sql += Environment.NewLine;
                sql += $"insert into #master (" +
                    $"ma_ck,diem_tl,t_diem_so,t_diem_dh," +
                    $" so_ct_dms," +
                "status,stt_rec,ma_ct, so_ct," +
                "ma_dvcs,ngay_ct,ma_nt,ty_gia,ma_nx,ma_kh,t_tien2,t_tien_nt2,t_ck,t_ck_nt,t_thue,t_thue_nt,t_tt,t_tt_nt,thue_suat,tk_thue_no,tk_thue_co,ma_nvbh,fcode1,fcode2,dien_giai,so_seri,ma_gd,tk,ma_thue,so_dh)  ";
                sql += Environment.NewLine + $"values(" +
                 $"'{model.ma_ck}','{model.diem_tl}','{model.t_diem_so}','{model.t_diem_dh}', " +
                    $"'{model.so_ct_dms}','{model.status}', @rec,@ma_ct," +
                           $"@voucherNumber," +
                           $"'{model.ma_dvcs}','{DateTime.Now.ToString("yyyy/MM/dd")}','{model.ma_nt}','{model.ty_gia}','{model.ma_nx}','{model.ma_kh}','{model.t_tien2}','{model.t_tien_nt2}','{model.t_ck}','{model.t_ck_nt}','{model.t_thue}','{model.t_thue_nt}','{model.t_tt}','{model.t_tt_nt}','{model.thue_suat}','{model.tk_thue_no}','{model.tk_thue_co}','{model.ma_nvbh}','{model.ma_bp}',N'{model.kenhbanhang}',N'{model.dien_giai}',@voucherNumber,'{model.ma_gd}','{model.tk}','{model.ma_thue}','{model.so_dh}'" +
                   ")";
                sql += Environment.NewLine;
                sql += $"update #master set so_ct0 = '{model.so_ct0}', ngay_ct0 = '{model.ngay_ct0}'";
                sql += Environment.NewLine;
                int a = 0;
                foreach (var detail in model.detail)
                {
                    a++;
                    sql += $"insert into #detail (ma_ck," +
                      $"diem,dien_giai_km," +
                    "stt_rec," +
                    "stt_rec0," +
                    "ma_ct,so_ct," +
                     "ma_vt,dvt,ma_kho,ma_lo,so_luong,gia2,gia_nt2,tien2,tien_nt2,tl_ck,ck,ck_nt,tt,tt_nt,gc_td1,ma_td3,ma_bp,ma_vv,ma_phi," +
                    "ngay_ct,km_yn,ma_sp,so_lsx,he_so,ma_vi_tri,tk_vt,gia_nt,gia,tien_nt,tien,thue,thue_nt,xstatus,xaction,cp_bh,cp_bh_nt,cp_vc,cp_vc_nt,cp_khac,cp_khac_nt,cp,cp_nt,stt_rec_ct,stt_rec0ct,gia_ban0,gia_ban_nt0,gia_ban,gia_ban_nt,gia_ck,gia_ck_nt,ck_tt,ck_tt_nt,sl_dh,sl_xuat,sl_giao,stt_rec_dh,stt_rec0dh,dh_so,dh_ln,stt_rec_px,stt_rec0px,px_so,px_ln,stt_rec_gh,stt_rec0gh,stt_rec_pn,stt_rec0pn,tk_gv,tk_dt,tk_ck,tk_cpbh,px_gia_dd,ma_nvbh_i,line_nbr,ma_hd,ma_ku,so_dh_i,ma_td1,ma_td2,sl_td1,sl_td2,sl_td3,ngay_td1,ngay_td2,ngay_td3,gc_td2,gc_td3,s1,s2,s3,s4,s5,s6,s7,s8,s9,gt_nt,gt)" +
                    $"values( '{detail.ma_ck}'," +
             $"'{detail.diem}',N'{detail.dien_giai_km}'," +
                    $"@rec," +
                    $"'{a.ToString("D3")}'," +
                    "@ma_ct," +
                    $"@voucherNumber," +
                    $"'{detail.ma_vt}',N'{detail.dvt}','{model.ma_dvcs}','{detail.ma_lo}',{detail.so_luong},{detail.gia2},{detail.gia_nt2},{detail.tien2},{detail.tien_nt2},{detail.tl_ck},{detail.ck},{detail.ck_nt},{detail.tt},{detail.tt_nt},'{detail.gc_td1}','{detail.ma_td3}','{detail.ma_bp}','{detail.ma_vv}','{detail.ma_phi}'," +
                   $"'{DateTime.Now.ToString("yyyy/MM/dd")}','{detail.km_yn}','','',1,'','{detail.tk_vt}',{detail.tien2},{detail.tien2},{detail.tien2},0,0,0,'','',0,0,0,0,0,0,0,0,'','',0,0,0,0,0,0,0,0,0,0,0,'','','',0,'','','',0,'','','','','{detail.tk_gv}','{detail.tk_dt}','','',0,'',{a},'','','','','',0,0,0,'','','','','','','','',0,0,0,'','','',0,0)";
                    sql += Environment.NewLine;
                    sql += $"update #detail set tk_ck = '{detail.tk_ck}', tk_cpbh = '{detail.tk_cpbh}' where  stt_rec0 =  '{a.ToString("D3")}' ";
                }
                sql += Environment.NewLine;
                sql += $"Insert into c{table}$000000 (stt_rec,ngay_ct,so_ct,ma_dvcs,status,user_id0) values (@rec,'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}',@voucherNumber,'CTY','{model.status}','10061')";

               sql += Environment.NewLine;
               sql += $"Insert into i{table}${time} (stt_rec,ngay_ct,so_ct,ma_dvcs,status,user_id0,c$,m$,d$) values (@rec,'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}',@voucherNumber,'CTY','{model.status}','10061','c{table}${time}','m{table}${time}','d{table}${time}')";
                sql += Environment.NewLine;
                sql += $"exec  [dbo].[Insert_PhieuNhap]  'm{table}${time}', 'd{table}${time}' ";
                sql += Environment.NewLine;
                DAL_SQL.ExecuteNonquery(sql);
                return "ok";
            }
            catch (Exception e)
            {
                var ex = e.Message;
                return "failed";
            }
        }


        public async Task<string> CreateInvoiceCar(InvoiceCarMode model)
        {
            try
            {
                int table = 81;
                HttpClientApp client = new HttpClientApp();
                string time = DateTime.Now.ToString("yyyyMM");
                string host = _config.GetValue<string>("WP:url");
                string sql = $"";
                sql += $"declare @ma_ct nvarchar(10) = (select top 1 ma_ct from dmct9 where c$ = 'c{table}$')";
                sql += Environment.NewLine;
                sql += $"declare @wsID varchar(1)                                          " +
                "select @wsID = rtrim(val) from options where upper(name) = 'M_WS_ID'      " +
                "create table #idNumber (stt_rec varchar(32))                              " +
                $"insert into #idNumber exec fs_GetIdentityNumber @wsID, @ma_ct, c{table}$000000" +
                "declare @rec nvarchar(200)                                                " +
                "select @rec = stt_rec from #idNumber                                      " +
                "declare @sct nvarchar(200)                                                " +
                $"set @sct =(select max(so_ct)+1 from m{table}$000000)";
                sql += Environment.NewLine;
                sql += "SELECT @voucherNumber = convert(char(11),convert(numeric,@voucherNumber)+1)";
                sql += Environment.NewLine;
                sql += "SELECT @voucherNumber =   ltrim(rtrim(cast(so_ct AS varchar(32)))) from dmct where ma_ct_me in(select ma_ct_me from dmct where ma_ct = @ma_ct)";
                sql += Environment.NewLine;
                sql += "SELECT @voucherNumber = convert(char(11),convert(numeric,@voucherNumber)+1)";
                sql += Environment.NewLine;
                sql += $"insert into m{table}${time} (" +
                "stt_rec,so_ct,ngay_lct,ngay_ct,so_seri,ma_nvbh,status,ma_kh,loai_gd,ma_dvcs) values  ";
                sql += Environment.NewLine + $"values( @rec" +
                           $"@sct,@voucherNumber,N'{model.ngay_lct}',N'{model.ngay_ct}',N'{model.so_seri}',N'{model.ma_nvbh}',N'{model.status}',{model.customer.ma_kh},'{model.loai_gd}','CTY'" +
                   ")";
                sql += Environment.NewLine;
                int a = 0;
                foreach (var detail in model.detail)
                {
                    a++;
                    sql += $"insert into d{table}${time} (" +
                    "stt_rec," +
                    "stt_rec0," +
                    "ma_ct," +
                     "ma_vt,nh_vt1,nh_vt2,nh_vt3,So_VIN,ma_kho,so_luong,gia_nt2,tien_nt2,tl_ck,ck_nt,ma_hd" +
                    ")" +
                    "values(" +
                    "@rec," +
                    $"'{a.ToString("D3")}'," +
                    "BC1," +
                    $"@sct,N'{detail.ma_vt}',N'{detail.nh_vt1}',N'{detail.nh_vt2}',N'{detail.nh_vt3}',N'{detail.So_VIN}',N'{detail.ma_kho}'," +
                    $"N'{detail.so_luong}',N'{detail.gia_nt2}',N'{detail.tien_nt2}',N'{detail.tl_ck}',N'{detail.ck_nt}',N'{detail.ma_hd}'" +
                   ")";


                    sql += Environment.NewLine;
                    sql += $"Insert into c81$000000 (stt_rec,ngay_ct,so_ct,ma_dvcs,status,user_id0) values (@rec,'{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}',@voucherNumber,'CTY','{model.status}','10061')";
                }
                DAL_SQL.ExecuteNonquery(sql);
                return "ok";
            }
            catch (Exception e)
            {
                return "failed";
            }
        }

        public Task<List<ProductModel>> CreateTbc()
        {
            throw new NotImplementedException();
        }
    }
}
