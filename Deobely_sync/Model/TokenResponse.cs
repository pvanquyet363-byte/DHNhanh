using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace Deobely_sync.Model
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
    public class UserRefreshTokenModel
    {
        [Required(ErrorMessage = "required_field")]
        public string Token { get; set; }
        [Required(ErrorMessage = "required_field")]
        public string RefreshToken { get; set; }
    }


    public class ErrorLog
    {

        [DisplayName("Date")]
        [Description("{Width: 10}")]
        [Column(Order = 1)]
        [XmlElement("Date")]
        public DateTime Date_create { get; set; }

        [DisplayName("Chức Năng")]
        [Description("{Width: 10}")]
        [Column(Order = 2)]
        [XmlElement("Controller")]
        public string Controller { get; set; }
        [DisplayName("Chi tiết lỗi")]
        [Description("{Width: 10}")]
        [Column(Order = 3)]
        [XmlElement("Error")]
        public string Error { get; set; }

    }
    public class RF_Token
    {
        public DateTime Date_create { get; set; }   
        public string value { get; set; }

    }
}
