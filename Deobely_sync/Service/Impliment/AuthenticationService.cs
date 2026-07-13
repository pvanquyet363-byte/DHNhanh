using Dapper;
using Deobely_sync.Model;
using Deobely_sync.Service.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Deobely_sync.Service.Impliment
{
    public class AuthenticationService : IAuthentication
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        public AuthenticationService(IConfiguration configuration, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        public async Task<TokenResponse> Authenticate(UserModel request)
        {
             
           string token = CreateToken(request, DateTime.Now.AddDays(7));
            var response = new TokenResponse()
            {
                Token = token,
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpiryTime = DateTime.Now.AddDays(7),
            };
            var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
        .SetPriority(CacheItemPriority.NeverRemove);
            InsertToken(response.RefreshToken);
            return response;
            
        }
        public async Task<bool> CheckToken(string refresh)
        {
            var table = new List<RF_Token>();
            var param = new DynamicParameters();
            var connect = _configuration.GetConnectionString("DbCnn");
            IDbConnection conn = new SqlConnection(connect);
            string querry = $"Select * from refresh_token where Date_create > '{DateTime.Now.AddDays(-1)}'";
            var result = await conn.QueryMultipleAsync(querry, param,
                   commandType: CommandType.Text);
            table = (await result.ReadAsync<RF_Token>()).ToList();
            foreach (RF_Token token in table)
            {
                if (token.value == refresh) return true;
            }
            return false;
        }

        public async void InsertToken(string refresh)
        {
            var param = new DynamicParameters();
            var connect = _configuration.GetConnectionString("DbCnn");
            IDbConnection conn = new SqlConnection(connect);
            string querry = $"Insert into refresh_token( Date_create, value) values('{DateTime.Now}','{refresh}') ";
            querry += Environment.NewLine + $" Delete from refresh_token where Date_create < '{DateTime.Now.AddDays(-8)}'";
            var result = await conn.ExecuteAsync(querry, param,
                   commandType: CommandType.Text);
        }



        public async Task<List<ErrorLog>> GetLog()
        {
            var table = new List<ErrorLog>();
            var param = new DynamicParameters();
            var connect = _configuration.GetConnectionString("DbCnn");
            IDbConnection conn = new SqlConnection(connect);
            string querry = $"Select * from apilog where Date_create > '{DateTime.Now.AddDays(-7)}'";
            var result = await conn.QueryMultipleAsync(querry, param,
                   commandType: CommandType.Text);
            table = (await result.ReadAsync<ErrorLog>()).ToList();
            return table;
        }


        public async Task<TokenResponse> GetRefreshTokenAsync(UserRefreshTokenModel model)
        {
            var userPrincipal = GetPrincipalFromExpiredToken(model.Token);
            if (userPrincipal.FindFirstValue(ClaimTypes.Name) != "admin")
                return null;
            var check = await CheckToken(model.RefreshToken);
            if (!check)
            {
                return null;
            }
            else
            {
                var request = new UserModel()
                {
                    UserName = "admin",
                    Password = "Abc@123456"
                };
                string token = CreateToken(request, DateTime.Now.AddDays(7));
                var response = new TokenResponse()
                {
                    Token = token,
                    RefreshToken = GenerateRefreshToken(),
                    RefreshTokenExpiryTime = DateTime.Now.AddDays(7),
                };
                
                return response;
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidIssuer = _configuration["Jwt:Issuer"]
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

      

            return principal;
        }



        private string CreateToken(UserModel user, DateTime expiresTime)
        {
            List<Claim> claims = new List<Claim>();

            claims = new List<Claim>
            {  
                new Claim(ClaimTypes.Name,user.UserName)
            };
               
          
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
             _configuration["Jwt:Issuer"],
             claims,
             expires: expiresTime,
             signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt.ToString();
        }
        public string GenerateRefreshToken()
        {
            byte[] array = new byte[32];
            using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(array);
            return Convert.ToBase64String(array);
        }

    }
}
