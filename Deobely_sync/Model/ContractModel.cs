using Microsoft.AspNetCore.Mvc;

namespace Deobely_sync.Model
{
    public partial class ContractModel
    {
        public string nh_hd1 { get { return nh_hd1; } set { this.nh_hd1 = value.Replace("'", "''"); } }
        public string ma_hd { get; set;  }
        public string ten_hd { get; set; }
        public string loai_hd { get; set; }
        public string ngay_hd { get; set; }
        public string tien_dc { get; set; }
        public string sum_hh_tr_vat { get; set; }
        public string sum_vat { get; set; }
        public string sum_sau_vat { get; set; }
        public string sum_psinh { get; set; }
        public string sum_cp_khac { get; set; }
        public string sum_kmai { get; set; }
        public string sum_phi_dkxe { get; set; }
        public CustomerModel customer { get; set; }
        //public string ma_hd { get; set; }
        //public string ten_hd { get; set; }
        //public string? ten_hd2 { get; set; }
        //public string? ma_kh { get; set; }
        //public string? ma_nvbh { get; set; }
        //public string? ma_bp { get; set; }
        //public string? ma_hd_me { get; set; }
        //public int? bac_hd { get; set; } = 0;
        //public int? loai_hd { get; set; }    =0;
        //public string? nh_hd1 { get; set; }
        //public string? nh_hd2 { get; set; }
        //public string? nh_hd3 { get; set; }
        //public string? ngay_hd { get; set; } = DateTime.Now.ToString("yyyy/MM/dd");
        //public string? so_hd { get; set; }
        //public string? ngay_hd1 { get; set; }
        //public string? ngay_hd2 { get; set; }
        //public int? hd_sd_pslk { get; set; }
        //public string? ma_nt { get; set; } = "VND";
        //public long? tien_nt { get; set; } = 0;
        //public long? tien { get; set; }          =0;
        //public long? tien_gt_nt { get; set; }    =0;
        //public long? tien_gt { get; set; }       =0;
        //public int? tinh_trang { get; set; }    =0;
        //public long? kl_kh { get; set; }         =0;
        //public long? kl_th { get; set; }         =0;
        //public string? ghi_chu { get; set; }
        //public string? status { get; set; }
        //public string? datetime0 { get; set; }= DateTime.Now.ToString("yyyy/MM/dd");
        //public string? datetime2 { get; set; }
        //public int? user_id0 { get; set; }
        //public int? user_id2 { get; set; }
        //public string? ngay_th { get; set; }
        //public string? ma_td1 { get; set; }
        //public string? ma_td2 { get; set; }
        //public string? ma_td3 { get; set; }
        //public long? sl_td1 { get; set; } = 0;
        //public long? sl_td2 { get; set; } = 0;
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
        //public long? s4 { get; set; }           = 0 ;
        //public long? s5 { get; set; }           = 0 ;
        //public long? s6 { get; set; } = 0;
        //public string? s7 { get; set; }
        //public string? s8 { get; set; }
        //public string? s9 { get; set; }
        //public string? ds_dvcs { get; set; }
        //public string? stt_rec_dh { get; set; }

    }
}
