using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Deobely_sync.Model
{
    public partial class CustomerModel
    {
        
        [DataType(DataType.Text)]
        [MaxLength(16, ErrorMessage = "{0} is maximal 16 characters ")]
        public string? ma_kh { get; set; }


        public string? tinh_thanh { get; set; } = "";
        public string? quan_huyen { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(128, ErrorMessage = "{0} is maximal 128 characters ")]
        public string? ten_kh { get; set; }
         
        [DataType(DataType.Text)]
        [MaxLength(128, ErrorMessage = "{0} is maximal 128 characters ")]
        public string? ten_kh2 { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(8, ErrorMessage = "{0} is maximal 8 characters ")]
        public string? nh_kh1 { get; set; } = "01";

        [DataType(DataType.Text)]
        [MaxLength(8, ErrorMessage = "{0} is maximal 8 characters ")]
        public string? nh_kh2 { get; set; } = "";
         
        [DataType(DataType.Text)]
        [MaxLength(8, ErrorMessage = "{0} is maximal 8 characters ")]
        public string? nh_kh3 { get; set; } = "";
        [DataType(DataType.Text)]
        [MaxLength(8, ErrorMessage = "{0} is maximal 8 characters ")]
        public string? nh_kh4 { get; set; } ="";
        [DataType(DataType.Text)]
        [MaxLength(8, ErrorMessage = "{0} is maximal 8 characters ")]
        public string? nh_kh5 { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(8, ErrorMessage = "{0} is maximal 8 characters ")]
        public string? nh_kh6 { get; set; } = "";
        [DataType(DataType.Text)]
        [MaxLength(8, ErrorMessage = "{0} is maximal 8 characters ")]
        public string? nh_kh7 { get; set; } = "";
        [DataType(DataType.Text)]
        [MaxLength(8, ErrorMessage = "{0} is maximal 8 characters ")]
        public string? nh_kh8 { get; set; } = "";
        [DataType(DataType.Text)]
        [MaxLength(8, ErrorMessage = "{0} is maximal 8 characters ")]
        public string? nh_kh9 { get; set; } = "";
        [DataType(DataType.Text)]

        public string? gc_td2 { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(18, ErrorMessage = "{0} is maximal 18 characters ")]
        public string? ma_so_thue { get; set; } = "";
         
        [DataType(DataType.Text)]
        [MaxLength(32, ErrorMessage = "{0} is maximal 32 characters ")]
        public string? dien_thoai { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(48, ErrorMessage = "{0} is maximal 48 characters ")]
        public string? e_mail { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(128, ErrorMessage = "{0} is maximal 128 characters ")]
        public string? doi_tac { get; set; } = "";
        public bool kh_yn { get; set; } = true;
        public bool ncc_yn { get; set; } = false;
        
        [DataType(DataType.Text)]
        [MaxLength(16, ErrorMessage = "{0} is maximal 16 characters ")]
        public string? tk { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(64, ErrorMessage = "{0} is maximal 64 characters ")]
        public string? ngan_hang { get; set; } = "";
         
        [DataType(DataType.Text)]
        [MaxLength(500, ErrorMessage = "{0} is maximal 500 characters ")]
        public string? ghi_chu { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(512, ErrorMessage = "{0} is maximal 512 characters ")]
        public string? ds_dvcs { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(512, ErrorMessage = "{0} is maximal 512 characters ")]
        public string? dia_chi { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(1, ErrorMessage = "{0} is maximal 1 characters ")]
        public string? status { get; set; } = "1";

        [DataType(DataType.Text)]
        [MaxLength(1, ErrorMessage = "{0} is maximal 1 characters ")]
        public string? loai_hinh { get; set; } = "";

        //public string? ma_kh { get; set; }
        //public string? ten_kh { get; set; }
        //public string? ten_kh2 { get; set; } = "";
        //public bool kh_yn { get; set; } = true;
        //public bool cc_yn { get; set; } = false;
        //public bool nv_yn { get; set; } = false;
        //public string? ma_so_thue { get; set; } = "";
        //public string? dia_chi { get; set; }
        //public string? dien_thoai { get; set; }
        //public string? fax { get; set; }
        //public string? e_mail { get; set; }
        //public string? ngay_sinh { get; set; }
        //public string? home_page { get; set; }
        //public string? doi_tac { get; set; }
        //public string? ong_ba { get; set; }
        //public string? ma_nvbh { get; set; }
        //public string? ngan_hang { get; set; }
        //public string? tk_nh { get; set; }
        //public string? tinh_thanh { get; set; }
        //public string? ghi_chu { get; set; }
        //public string? ma_tt { get; set; }
        //public string? tk { get; set; }
        //public string? nh_kh1 { get; set; }
        //public string? nh_kh2 { get; set; }
        //public string? nh_kh3 { get; set; }
        //public string? nh_kh9 { get; set; }
        //public long? du_nt13 { get; set; } = 0;
        //public long? du13 { get; set; } = 0;
        //public long? t_tien_cn { get; set; } = 0;
        //public long? t_tien_hd { get; set; } = 0;
        //public string? ngay_gh { get; set; }
        //public string? status { get; set; } = "1";
        //public string? datetime0 { get; set; }
        //public string? datetime2 { get; set; }
        //public int? user_id0 { get; set; }
        //public int? user_id2 { get; set; }
        //public string? ma_td1 { get; set; }
        //public string? ma_td2 { get; set; }
        //public string? ma_td3 { get; set; }
        //public long? sl_td1 { get; set; }= 0;
        //public long? sl_td2 { get; set; }= 0;
        //public long? sl_td3 { get; set; } = 0;
        //public string? ngay_td1 { get; set; }
        //public string? ngay_td2 { get; set; }
        //public string? ngay_td3 { get; set; }
        //public string? gc_td1 { get; set; }
        //public string? gc_td2 { get; set; }
        //public string? gc_td3 { get; set; }
        //public string? s1 { get; set; }
        //public string? s2 { get; set; }
        //public string? s3 { get; set; }
        //public long? s4 { get; set; }  =0;
        //public long? s5 { get; set; }  =0;
        //public long? s6 { get; set; } = 0;
        //public string? s7 { get; set; }
        //public string? s8 { get; set; }
        //public string? s9 { get; set; }
        //public string? ds_dvcs { get; set; }
        //public string? gioi_tinh { get; set; }
        //public long? diem { get; set; } = 0;
        //public string? loai_gttt { get; set; }
        //public string? so_gttt { get; set; }
        //public string? ngay_cap { get; set; } = null;
        //public string? noi_cap { get; set; }
        //public string? gplx { get; set; }
        //public string? ngay_het_han { get; set; } = null;
        //public string? ds_user { get; set; }



    }
}
