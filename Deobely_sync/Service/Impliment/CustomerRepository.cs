using Dapper;
using Deobely_sync.DAL;
using Deobely_sync.Model;
using Deobely_sync.Service.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Deobely_sync.Service.Impliment
{
    public class CustomerRepository:ICustomer
    {
        private readonly ILogger<CustomerRepository> _logger;
        private IConfiguration _config;

        public CustomerRepository(ILogger<CustomerRepository> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public int CovertBool(bool x)
        {
            if(x) return 1;
            else return 0;
        }

        public async Task<bool> CheckCustomer(string customer)
        {
            try
            {
                var connect = _config.GetConnectionString("DbCnn");
                IDbConnection conn = new SqlConnection(connect);

                var result = await conn.QueryMultipleAsync($"select ma_kh from dmkh where ma_kh = '{customer.Trim()}' ",
                       commandType: CommandType.Text);
                var check = await result.ReadFirstOrDefaultAsync<string>();
                if (check != null) return false;
                else return true;
            }
            catch(Exception e)
            {
                return false;
            }

        }

        public async Task<int> CreateCustomer(CustomerModel model)
            {
            try
            {
                if (await CheckCustomer(model.ma_kh))
                {
                    HttpClientApp client = new HttpClientApp();
                    string host = _config.GetValue<string>("WP:url");
                    string sql = $"";
                    //int a = model.kh_yn ? 1 : 0;
                    int a = 1;
                    sql += Environment.NewLine;
                    sql += $"insert into dmkh (ma_kh,ten_kh,ten_kh2,kh_yn,ma_so_thue,dia_chi,dien_thoai,e_mail,doi_tac,nh_kh1,nh_kh2,nh_kh3,tk,ghi_chu,ds_dvcs,status,nh_kh4, nh_kh5, nh_kh6, nh_kh7, nh_kh8, nh_kh9,gc_td2,tinh_thanh,quan_huyen,loai_hinh) values";
                    sql += $"(N'{model.ma_kh.Replace("'", "''")}', N'{model.ten_kh.Replace("'", "''")}', N'{model.ten_kh2.Replace("'", "''")}', '{a}', '{model.ma_so_thue}', N'{model.dia_chi.Replace("'", "''")}', '{model.dien_thoai}', '{model.e_mail}', N'{model.doi_tac.Replace("'", "''")}', N'{model.nh_kh1}', '{model.nh_kh2}', '{model.nh_kh3}', '{model.tk}', N'{model.ghi_chu.Replace("'", "''")}', '{model.ds_dvcs}', '{model.status}','{model.nh_kh4}', '{model.nh_kh5}', '{model.nh_kh6}', '{model.nh_kh7}', '{model.nh_kh8}', '{model.nh_kh9}',N'{model.gc_td2.Replace("'", "''")}',N'{model.tinh_thanh.Replace("'", "''")}',N'{model.quan_huyen.Replace("'", "''")}',N'{model.loai_hinh.Replace("'", "''")}' )";
                    DAL_SQL.ExecuteNonquery(sql);
                    return 1;
                }
                else
                {
                    HttpClientApp client = new HttpClientApp();
                    string host = _config.GetValue<string>("WP:url");
                    string sql = $"";
                    //int a = model.kh_yn ? 1 : 0;
                    int a = 1;
                    sql += Environment.NewLine;
                    sql += $"update dmkh set ten_kh = N'{model.ten_kh.Replace("'","''")}',ten_kh2 =  N'{model.ten_kh.Replace("'", "''")}',kh_yn = {a},ma_so_thue =  '{model.ma_so_thue}',dia_chi = N'{model.dia_chi.Replace("'", "''")}',dien_thoai =  '{model.dien_thoai.Replace("'", "''")}',e_mail =  '{model.e_mail}',doi_tac = N'{model.doi_tac.Replace("'", "''")}',nh_kh1 = N'{model.nh_kh1}',nh_kh2 ='{model.nh_kh2}',nh_kh3='{model.nh_kh3}'" +
                        $",tk = '{model.tk}',ghi_chu =  N'{model.ghi_chu.Replace("'", "''")}',ds_dvcs = '{model.ds_dvcs}',status = '{model.status}',nh_kh4 = '{model.nh_kh4}', nh_kh5 =  '{model.nh_kh5}', nh_kh6 = '{model.nh_kh6}', nh_kh7  ='{model.nh_kh7}', nh_kh8 = '{model.nh_kh8}', nh_kh9 = '{model.nh_kh9}',gc_td2 = N'{model.gc_td2}',tinh_thanh = N'{model.tinh_thanh.Replace("'", "''")}',quan_huyen='{model.quan_huyen.Replace("'", "''")}',loai_hinh='{model.loai_hinh.Replace("'", "''")}' where ma_kh = '{model.ma_kh}' ";
                    DAL_SQL.ExecuteNonquery(sql);
                    return 1;
                }
            }
            catch(Exception e)
            {
                return 0;
            }
        }
    }
}
