using ExcelDataReader;
using System.Data;
using System.IO;
using System;


public class ExcelFileReaderService
{
    public DataTable ReadExcelToDataTable(string filePath)
    {
        System.Text.Encoding.RegisterProvider(
            System.Text.CodePagesEncodingProvider.Instance);

        try
        {
            using var stream = File.Open(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite   // cho phép đọc khi Excel đang mở (chưa lock)
            );

            using var reader = ExcelReaderFactory.CreateReader(stream);

            var result = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = true
                }
            });

            return result.Tables[0];
        }
        catch (IOException)
        {
            throw new Exception(
                "File Excel đang được mở.\nVui lòng đóng file Excel trước khi đối chiếu.");
        }
        catch (UnauthorizedAccessException)
        {
            throw new Exception(
                "Không có quyền truy cập file Excel.\nVui lòng kiểm tra quyền truy cập.");
        }
        catch (Exception ex)
        {
            throw new Exception(
                $"Không thể đọc file Excel.\nChi tiết lỗi:\n{ex.Message}");
        }
    }
}
