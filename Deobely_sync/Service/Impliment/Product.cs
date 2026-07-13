using Dapper;
using Deobely_sync.DAL;
using Deobely_sync.Model;
using Deobely_sync.Service.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Deobely_sync.Service.Impliment
{
    public class Product:IProduct
    {
        private readonly ILogger<Product> _logger;
        private IConfiguration _config;

        public Product(ILogger<Product> logger, IConfiguration config)
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

        public async Task<bool> Check(string ma_vt)
        {
          
            var connect = _config.GetConnectionString("DbCnn");
            IDbConnection conn = new SqlConnection(connect);

            var result = await conn.QueryFirstOrDefault($"select ma_vt from dmvt where ma_vt = '{ma_vt}' ",
                   commandType: CommandType.Text);
            List<ProductModel> table1 = (await result.ReadAsync<ProductModel>()).ToList();
            string check = await result.ReadAsync<string>();
            if (check != null) return false;
            else return true;
        }


        public async Task<int> CreateProduct(ProductInsertModel model)
        {
            try
            {
                if (await Check(model.ma_vt))
                {
                    HttpClientApp client = new HttpClientApp();
                    string host = _config.GetValue<string>("WP:url");
                    string sql = $"";
                    sql += Environment.NewLine;
                    sql += $"insert into dmvt (ma_vt,ten_vt,dvt,loai_vt,nh_vt1,nh_vt2,nh_vt3,nuoc_sx,xcolor) values ";
                    sql += $"(N'{model.ma_vt}',N'{model.ten_vt}',N'{model.dvt}',N'{model.loai_vt}',N'{model.nh_vt1}',N'{model.nh_vt2}',N'{model.nh_vt3}',N'{model.nuoc_sx}',N'{model.xcolor}')";
                    DAL_SQL.ExecuteNonquery(sql);
                    return 1;
                }
                else return 2;
            }
            catch (Exception e)
            {
                return 0;
            }
        }


    }
}
