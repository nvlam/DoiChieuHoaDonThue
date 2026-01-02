using System.Data;

public class ExcelCompareService
{
    public List<CompareResult> Compare(
        DataTable dtCongThue,
        DataTable dtMisa,
        ExcelMappingRoot map)
    {
        var results = new List<CompareResult>();

        var ctMap = map.CongThue;
        var msMap = map.Misa;

        var congThueDict = dtCongThue.AsEnumerable()
            .Where(r => r[ctMap.SoHoaDon] != null &&
                        !string.IsNullOrWhiteSpace(r[ctMap.SoHoaDon].ToString()))
            .ToDictionary(
                r => ExcelValueHelper.NormalizeInvoiceNumber(r[ctMap.SoHoaDon]),
                r => r
            );

        var misaDict = dtMisa.AsEnumerable()
            .Where(r => r[msMap.SoHoaDon] != null &&
                        !string.IsNullOrWhiteSpace(r[msMap.SoHoaDon].ToString()))
            .ToDictionary(
                r => ExcelValueHelper.NormalizeInvoiceNumber(r[msMap.SoHoaDon]),
                r => r
            );

        // 1️⃣ Công Thuế → Missing MISA
        foreach (var ct in congThueDict)
        {
            if (!misaDict.ContainsKey(ct.Key))
            {
                results.Add(new CompareResult
                {
                    SoHoaDon = ct.Key,
                    NgayHoaDonCongThue =
                        ExcelValueHelper.ToDateStringSafe(ct.Value[ctMap.NgayHoaDon]),
                    TenKhachHangCongThue =
                        ct.Value[ctMap.TenKhachHang]?.ToString(),
                    TongTienCongThue =
                        ExcelValueHelper.ToDecimalSafe(ct.Value[ctMap.TongTien]),
                    LoaiLoi = "MissingMISA",
                    GhiChu = "Có ở Công Thuế nhưng không có trong MISA"
                });
            }
        }

        // 2️⃣ MISA → Missing Công Thuế
        foreach (var ms in misaDict)
        {
            if (!congThueDict.ContainsKey(ms.Key))
            {
                results.Add(new CompareResult
                {
                    SoHoaDon = ms.Key,
                    NgayHoaDonMisa =
                        ExcelValueHelper.ToDateStringSafe(ms.Value[msMap.NgayHoaDon]),
                    TenKhachHangMisa =
                        ms.Value[msMap.TenKhachHang]?.ToString(),
                    DoanhThuMisa =
                        ExcelValueHelper.ToDecimalSafe(ms.Value[msMap.DoanhThu]),
                    LoaiLoi = "MissingCT",
                    GhiChu = "Có ở MISA nhưng không có trong Công Thuế"
                });
            }
        }

        // 3️⃣ Amount mismatch
        foreach (var soHoaDon in congThueDict.Keys.Intersect(misaDict.Keys))
        {
            decimal ctAmount =
                ExcelValueHelper.ToDecimalSafe(congThueDict[soHoaDon][ctMap.TongTien]);
            decimal msAmount =
                ExcelValueHelper.ToDecimalSafe(misaDict[soHoaDon][msMap.DoanhThu]);

            if (ctAmount != msAmount)
            {
                results.Add(new CompareResult
                {
                    SoHoaDon = soHoaDon,
                    NgayHoaDonCongThue =
                        ExcelValueHelper.ToDateStringSafe(congThueDict[soHoaDon][ctMap.NgayHoaDon]),
                    TenKhachHangCongThue =
                        congThueDict[soHoaDon][ctMap.TenKhachHang]?.ToString(),

                    NgayHoaDonMisa =
                        ExcelValueHelper.ToDateStringSafe(misaDict[soHoaDon][msMap.NgayHoaDon]),
                    TenKhachHangMisa =
                        misaDict[soHoaDon][msMap.TenKhachHang]?.ToString(),

                    TongTienCongThue = ctAmount,
                    DoanhThuMisa = msAmount,
                    LoaiLoi = "AmountDiff",
                    GhiChu = "Chênh lệch tiền giữa Công Thuế và MISA"
                });
            }
        }

        return results;
    }
}
