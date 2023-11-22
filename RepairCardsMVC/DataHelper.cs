using RepairCardsMVC.ViewModels;
using System.Linq.Dynamic.Core;

namespace RepairCardsMVC
{
    public static class DataHelper
    {
        public static DataHelperViewModel<T> GetDataForJQueryDataTable<T>(this HttpRequest request, IQueryable<T> query, params string[] filterColumns)
        {
            int totalCount = 0;
            int filteredCount = 0;
            var draw = request.Form["draw"].FirstOrDefault();
            var sortColumn = request.Form["columns[" + request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(request.Form["start"].FirstOrDefault() ?? "0");

            totalCount = query.Count();

            if (!string.IsNullOrEmpty(searchValue))
            {
                string expression = string.Join(" || ", filterColumns.Select(x => x + $".ToLower().Contains(\"{searchValue.ToLower()}\")"));
                query = query.Where(expression);
            }

            filteredCount = query.Count();

            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                query = query.OrderBy(sortColumn + " " + sortColumnDirection);
            }

            List<T> items;

            if (pageSize > -1)
                items = query.Skip(skip).Take(pageSize).ToList();
            else
                items = query.ToList();

            var result = new DataHelperViewModel<T>
            {
                Draw = draw,
                RecordsTotal = totalCount,
                RecordsFiltered = filteredCount,
                Data = items
            };

            return result;
        }
    }
}
