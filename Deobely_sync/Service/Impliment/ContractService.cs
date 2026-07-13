using Dapper;
using Deobely_sync.DAL;
using Deobely_sync.Model;
using Deobely_sync.Service.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Deobely_sync.Service.Impliment
{
    public class ContractRepository:IContract
    {
        private readonly ILogger<ContractRepository> _logger;
        private IConfiguration _config;

        public ContractRepository(ILogger<ContractRepository> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }
        public async Task<bool> Check(string ma_hd)
        {
            try
            {
                var connect = _config.GetConnectionString("DbCnn");
                IDbConnection conn = new SqlConnection(connect);

                var result = await conn.QueryMultipleAsync($"select ma_hd from dmhd where ma_kh = '{ma_hd.Trim()}' ",
                       commandType: CommandType.Text);
                var check = await result.ReadFirstOrDefaultAsync<string>();
                if (check != null) return false;
                else return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<int> CreateContract(ContractModel model)
        {
            try
            {
                if(await Check(model.ma_hd)) return 2;
                HttpClientApp client = new HttpClientApp();
                string host = _config.GetValue<string>("WP:url");
                string sql = $"";
                sql += Environment.NewLine;
                sql += $"insert into dmhd (status,ma_nt,nh_hd1,ma_hd,ten_hd,ten_hd2, loai_hd,ngay_hd,tien_dc,ma_kh,sum_hh_tr_vat,sum_vat,sum_sau_vat,sum_psinh,sum_cp_khac,sum_kmai,sum_phi_dkxe) values";
                sql += $"('1','VND','{model.nh_hd1}','{model.ma_hd}',N'{model.ten_hd}',N'{model.ten_hd}','{model.loai_hd}',N'{model.ngay_hd}','{model.tien_dc}',N'{model.customer.ma_kh}',{model.sum_hh_tr_vat},{model.sum_vat},{model.sum_sau_vat},{model.sum_psinh},{model.sum_cp_khac},{model.sum_kmai},{model.sum_phi_dkxe})";
                DAL_SQL.ExecuteNonquery(sql);
                return 1;
            }
            catch(Exception e)
            {
                return 0;
            }
        }
    }
}
