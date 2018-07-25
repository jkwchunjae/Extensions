using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    public class ExcelWorkbook : IDisposable
    {
        ExcelPackage _excelPackage;

        public ExcelWorkbook(string path)
        {
            _excelPackage = new ExcelPackage(new FileInfo(path));
        }

        public ExcelWorksheet GetWorksheet(string sheetName)
        {
            if (_excelPackage.Workbook.Worksheets.Any(x => x.Name == sheetName))
            {
                return _excelPackage.Workbook.Worksheets.First(x => x.Name == sheetName);
            }

            throw new Exception($"Can't find sheet. SheetName: {sheetName}");
        }

        public List<T> GetTable<T>(string sheetName, int startRowBase1 = 1, int startColumnBase1 = 1) where T : new()
        {
            var sheet = GetWorksheet(sheetName);

            // var availableColumnsCount = sheet.UsedRange.Columns.Count + sheet.UsedRange.Column - startColumnBase1;
            // var availableRowsCount = sheet.UsedRange.Rows.Count + sheet.UsedRange.Column - startRowBase1;

            var titleInfo = Enumerable.Range(startColumnBase1, 100)
                .TakeWhile(column => (sheet.Cells[startRowBase1, column]).Value != null)
                .Select(column => new { Column = column, FieldName = (string)(sheet.Cells[startRowBase1, column]).Value })
                .ToDictionary(x => x.Column, x => x.FieldName);

            return Enumerable.Range(startRowBase1 + 1, 100000)
                .TakeWhile(row => ((sheet.Cells[row, titleInfo.Min(x => x.Key)]).Value != null))
                .AsParallel()
                .Select(row => new
                {
                    Row = row,
                    ValueDic = titleInfo
                        .Select(x => new { FieldName = x.Value, Value = (sheet.Cells[row, x.Key]).Value })
                        .ToDictionary(x => x.FieldName, x => x.Value)
                })
                .OrderBy(x => x.Row)
                .Select(x => ExcelHelper.Create<T>(x.ValueDic))
                .ToList();
        }

        public void Dispose()
        {
            _excelPackage.Dispose();
        }
    }
}
