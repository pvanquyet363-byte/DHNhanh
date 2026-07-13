using System.ComponentModel.DataAnnotations;

namespace Deobely_sync.Model
{
    public class TbcModel
    {

        /// <summary>
        /// /////////////
        /// 
        /// </summary>

        public float? diem_tl { get; set; } = 0;
        public float? t_diem_dh { get; set; }       = 0;
        public string? ma_ck { get; set; } = "";
        public float? t_diem_so { get; set; } = 0;

        ///////////////
        ///
        [DataType(DataType.Text)]
        [MaxLength(256, ErrorMessage = "{0} is maximal 14 characters ")]
        public string ma_thue { get; set; } = "";
       
        [DataType(DataType.Text)]
        [MaxLength(8, ErrorMessage = "{0} is maximal 6 characters ")]
        public string tk { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(2, ErrorMessage = "{0} is maximal 2 characters ")]
        public string ma_gd { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(1, ErrorMessage = "{0} is maximal 6 characters ")]
        public string status { get; set; } = "";

        [Required(ErrorMessage = "{0} is required ")]
        [DataType(DataType.Text)]
        [MaxLength(8, ErrorMessage = "{0} is maximal 8 characters ")]
        public string ma_dvcs { get; set; } = "";

        [Required(ErrorMessage = "{0} is required ")]
        public string? ngay_ct { get; set; } = DateTime.Now.ToString("yyyy/MM/dd");


        [DataType(DataType.Text)]
        [MaxLength(3, ErrorMessage = "{0} is maximal 3 characters ")]
        public string ma_nt { get; set; } = "";

        [RegularExpression(@"^-?\d+$.?\d{0,2}$", ErrorMessage = "Invalid number; Maximum Two Decimal Points.")]
        public float? ty_gia { get; set; } =0;

        [DataType(DataType.Text)]
        [MaxLength(16, ErrorMessage = "{0} is maximal 16 characters ")]
        public string ma_nx { get; set; } = "";

        [Required(ErrorMessage = "{0} is required ")]
        [DataType(DataType.Text)]
        [MaxLength(16, ErrorMessage = "{0} is maximal 16 characters ")]
        public string ma_kh { get; set; } = "";

        [RegularExpression(@"^-?\d+$.?\d{0,2}$", ErrorMessage = "Invalid number; Maximum Two Decimal Points.")]
        public float? t_tien2 { get; set; } = 0;

        public float? t_tien_nt2 { get; set; } = 0;

        [RegularExpression(@"^-?\d+$.?\d{0,2}$", ErrorMessage = "Invalid number; Maximum Two Decimal Points.")]
        public float? t_ck { get; set; } = 0;

        [RegularExpression(@"^-?\d+$.?\d{0,2}$", ErrorMessage = "Invalid number; Maximum Two Decimal Points.")]
        public float? t_ck_nt { get; set; } = 0;


        [RegularExpression(@"^-?\d+$.?\d{0,2}$", ErrorMessage = "Invalid number; Maximum Two Decimal Points.")]
        public float? t_thue { get; set; } = 0;

        [RegularExpression(@"^-?\d+$.?\d{0,2}$", ErrorMessage = "Invalid number; Maximum Two Decimal Points.")]
        public float? t_thue_nt { get; set; } = 0;

        [RegularExpression(@"^-?\d+$.?\d{0,2}$", ErrorMessage = "Invalid number; Maximum Two Decimal Points.")]
        public float? t_tt { get; set; } = 0;

        [RegularExpression(@"^-?\d+$.?\d{0,2}$", ErrorMessage = "Invalid number; Maximum Two Decimal Points.")]
        public float t_tt_nt { get; set; } = 0;


        [Required(ErrorMessage = "{0} is required ")]
        [RegularExpression(@"^-?\d+$.?\d{0,2}$", ErrorMessage = "Invalid number; Maximum Two Decimal Points.")]
        public float? thue_suat { get; set; } = 0;

        [DataType(DataType.Text)]
        [MaxLength(16, ErrorMessage = "{0} is maximal 16 characters ")]
        public string tk_thue_no { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(16, ErrorMessage = "{0} is maximal 16 characters ")]
        public string tk_thue_co { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(8, ErrorMessage = "{0} is maximal 8 characters ")]
        public string ma_nvbh { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(16, ErrorMessage = "{0} is maximal 16 characters ")]
        public string ma_bp { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(16, ErrorMessage = "{0} is maximal 16 characters ")]
        public string so_dh { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(1, ErrorMessage = "{0} is maximal 1 characters ")]
        public string loai_ct { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(225, ErrorMessage = "{0} is maximal 300 characters ")]
        public string dd_gh { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(16, ErrorMessage = "{0} is maximal 16 characters ")]
        public string fcode1 { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(16, ErrorMessage = "{0} is maximal 16 characters ")]
        public string fcode2 { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(50, ErrorMessage = "{0} is maximal 50 characters ")]
        public string kenhbanhang { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(512, ErrorMessage = "{0} is maximal 512 characters ")]
        public string dien_giai { get; set; } = "";
        public string? so_ct_dms { get; set; } = "";
        public List<TbcDetailModel> detail { get; set; }
        public string? so_ct0 { get; set; } 
        public string? ngay_ct0 { get; set; }

    }


    public class PhieuNhapModel
    {
        public string? so_ct { get; set; }
        public string ngay_lct { get; set; }
        public string ngay_ct { get; set; }
        public string ong_ba { get; set; }
        public string status { get; set; }
        public string dien_giai { get; set; }
        public List<TbcDetailModel> detail { get; set; }
        public CustomerModel customer { get; set; }
        public bool phieu_thu { get; set; }

    }
    public class TbcDetailModel
    {
        /// <summary>
        /// /
        /// 
        /// </summary>
        public float? diem { get; set; } = 0;
        public string? dien_giai_km { get; set; } = "";

        //////////
        [DataType(DataType.Text)]
        [MaxLength(16, ErrorMessage = "{0} is maximal 16 characters ")]
        public string tk_gv { get; set; } = "";
        [DataType(DataType.Text)]
        [MaxLength(16, ErrorMessage = "{0} is maximal 16 characters ")]
        public string? tk_cpbh { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(16, ErrorMessage = "{0} is maximal 16 characters ")]
        public string? tk_ck { get; set; } = "";
        [DataType(DataType.Text)]
        [MaxLength(16, ErrorMessage = "{0} is maximal 16 characters ")]
        public string tk_dt { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(16, ErrorMessage = "{0} is maximal 16 characters ")]
        public string? tk_cpkm { get; set; }
        public string? ma_ck { get; set; } = "";
        [DataType(DataType.Text)]
        [MaxLength(16, ErrorMessage = "{0} is maximal 16 characters ")]
        public string tk_vt { get; set; } = "";

    
        [DataType(DataType.Text)]
        [MaxLength(16, ErrorMessage = "{0} is maximal 16 characters ")]
        public string? ma_vt { get; set; } = "CK001";

        [DataType(DataType.Text)]
        [MaxLength(10, ErrorMessage = "{0} is maximal 10 characters ")]
        public string dvt { get; set; } = "";



        [DataType(DataType.Text)]
        [MaxLength(16, ErrorMessage = "{0} is maximal 16 characters ")]
        public string ma_lo { get; set; } = "";

        [Required(ErrorMessage = "{0} is required ")]
        public float? so_luong { get; set; } = 0;

        public float? gia2 { get; set; } = 0;

        [Required(ErrorMessage = "{0} is required ")]
        [RegularExpression(@"^-?\d+$.?\d{0,2}$", ErrorMessage = "Invalid number; Maximum Two Decimal Points.")]
        public float? gia_nt2 { get; set; } = 0;

        [RegularExpression(@"^-?\d+$.?\d{0,2}$", ErrorMessage = "Invalid number; Maximum Two Decimal Points.")]
        public float? tien2 { get; set; } = 0;

        [Required(ErrorMessage = "{0} is required ")]
        public float? tien_nt2 { get; set; } = 0;

        [Required(ErrorMessage = "{0} is required ")]
        [RegularExpression(@"^-?\d+$.?\d{0,2}$", ErrorMessage = "Invalid number; Maximum Two Decimal Points.")]
        public float? tl_ck { get; set; } = 0;

        [Required(ErrorMessage = "{0} is required ")]
        [RegularExpression(@"^-?\d+$.?\d{0,2}$", ErrorMessage = "Invalid number; Maximum Two Decimal Points.")]
        public float? ck { get; set; } = 0;

        [RegularExpression(@"^-?\d+$.?\d{0,2}$", ErrorMessage = "Invalid number; Maximum Two Decimal Points.")]
        public float? ck_nt { get; set; } = 0;

        public float? tt { get; set; } = 0;

        public float tt_nt { get; set; } = 0; 
       
        [DataType(DataType.Text)]
        [MaxLength(24, ErrorMessage = "{0} is maximal 24 characters ")]
        public string gc_td1 { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(16, ErrorMessage = "{0} is maximal 16 characters ")]
        public string ma_td3 { get; set; } = "";
        public string ma_bp { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(16, ErrorMessage = "{0} is maximal 16 characters ")]
        public string ma_vv { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(16, ErrorMessage = "{0} is maximal 16 characters ")]
        public string ma_phi { get; set; } = "";

        [DataType(DataType.Text)]
        [MaxLength(16, ErrorMessage = "{0} is maximal 16 characters ")]
        public string km_yn { get; set; } = "";

    }

    public class InvoiceCarMode
    {
     
        public string loai_gd = "1";
        public string so_ct { get; set; }
        public string ngay_lct { get; set; }
        public string ngay_ct { get; set; }
        public string so_seri { get; set; }
        public string ma_nvbh { get; set; }
        public int status { get; set; }
        public CustomerModel customer { get; set; }
        public List<InvoiceCarDetail> detail{get;set;}

    }
    public class InvoiceCarDetail
    {
        public string? ma_vt { get; set; } = "CK001";
        public string nh_vt1 { get; set; }
        public string nh_vt2 { get; set; }
        public string nh_vt3 { get; set; }
        public string So_VIN { get; set; }
        public string ma_kho { get; set; }
        public int so_luong { get; set; }
        public float? gia_nt2 { get; set; }
        public float? tien_nt2 { get; set; }
        public float? tl_ck { get; set; }
        public float? ck_nt { get; set; }
        public string ma_hd { get; set; }

    }

    public class ServiceModel
    {
        public string so_ct { get; set; }
        public string ngay_lct { get; set; }
        public string ngay_ct { get; set; }

    }

}
