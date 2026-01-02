public static class ExcelValueHelper
{
    public static decimal ToDecimalSafe(object value)
    {
        if (value == null) return 0;

        return decimal.TryParse(
            value.ToString().Replace(",", "").Trim(),
            out decimal result
        ) ? result : 0;
    }

    public static string NormalizeInvoiceNumber(object value)
    {
        if (value == null) return null;

        string s = value.ToString().Trim();
        s = s.TrimStart('0');
        return string.IsNullOrEmpty(s) ? "0" : s;
    }

    public static string ToDateStringSafe(object value)
    {
        if (value == null) return "";

        if (DateTime.TryParse(value.ToString(), out var d))
            return d.ToString("dd/MM/yyyy");

        if (double.TryParse(value.ToString(), out var oa))
            return DateTime.FromOADate(oa).ToString("dd/MM/yyyy");

        return value.ToString();
    }
}
