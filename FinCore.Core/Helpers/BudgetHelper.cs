using System.Globalization;

namespace FinCore.BLL.Helpers;

public class BudgetHelper
{
    public static DateTime ParseAndValidateSearchDate(string? searchDate)
    {
        searchDate ??= DateTime.Now.ToString("dd.MM.yyyy");

        if (!DateTime.TryParseExact(searchDate, "dd.MM.yyyy", null, DateTimeStyles.None, out var date))
            throw new Exception("Aranacak tarih geçerli bir tarih olmalıdır.");

        if (date > DateTime.Today)
            throw new Exception("Aranacak tarih geçerli bir tarih olmalıdır.");

        return date;
    }
}