public class CompareResult
{
    public string SoHoaDon { get; set; }

    public string NgayHoaDonCongThue { get; set; }
    public string TenKhachHangCongThue { get; set; }

    public string NgayHoaDonMisa { get; set; }
    public string TenKhachHangMisa { get; set; }

    public decimal? TongTienCongThue { get; set; }
    public decimal? DoanhThuMisa { get; set; }

    public string GhiChu { get; set; }
    public string LoaiLoi { get; set; }

    // =========================
    // DISPLAY PROPERTIES (NEW)
    // =========================

    public string NgayHoaDonHienThi =>
        !string.IsNullOrWhiteSpace(NgayHoaDonCongThue)
            ? NgayHoaDonCongThue
            : NgayHoaDonMisa;

    public string TenKhachHangHienThi =>
        !string.IsNullOrWhiteSpace(TenKhachHangCongThue)
            ? TenKhachHangCongThue
            : TenKhachHangMisa;

    public decimal? TongTienHienThi =>
        TongTienCongThue.HasValue && TongTienCongThue > 0
            ? TongTienCongThue
            : DoanhThuMisa;
}
