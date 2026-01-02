using ClosedXML.Excel;
using System.Collections.Generic;

public class ExcelExportService
{
    public void ExportCompareResults(
        IEnumerable<CompareResult> data,
        string filePath)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("KetQuaDoiChieu");

        // ===== Header =====
        ws.Cell(1, 1).Value = "Số hóa đơn";
        ws.Cell(1, 2).Value = "Ngày hóa đơn";
        ws.Cell(1, 3).Value = "Tên khách hàng";
        ws.Cell(1, 4).Value = "Tiền Cổng Thuế";
        ws.Cell(1, 5).Value = "Tiền MISA";
        ws.Cell(1, 6).Value = "Loại lỗi";
        ws.Cell(1, 7).Value = "Ghi chú";

        ws.Range(1, 1, 1, 7).Style.Font.Bold = true;
        ws.Range(1, 1, 1, 7).Style.Fill.BackgroundColor = XLColor.LightGray;

        // ===== Data =====
        int row = 2;
        foreach (var r in data)
        {
            ws.Cell(row, 1).Value = r.SoHoaDon;
            ws.Cell(row, 2).Value = r.NgayHoaDonHienThi;
            ws.Cell(row, 3).Value = r.TenKhachHangHienThi;
            ws.Cell(row, 4).Value = r.TongTienCongThue;
            ws.Cell(row, 5).Value = r.DoanhThuMisa;
            ws.Cell(row, 6).Value = r.LoaiLoi;
            ws.Cell(row, 7).Value = r.GhiChu;
            row++;
        }

        // ===== Format =====
        ws.Columns().AdjustToContents();

        wb.SaveAs(filePath);
    }
}
