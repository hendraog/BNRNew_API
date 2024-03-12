using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XWPF.UserModel;

namespace BNRNew_API.utils
{
    public class NPoiUtils
    {        
        public static IRow createRow(IWorkbook wb, ISheet ws, int rowNum,IFont font,  params object[] value)
        {
            var row = ws.CreateRow(rowNum);
            IDataFormat dataFormatCustom = wb.CreateDataFormat();


            var cellX = 0;
            foreach( var item in value ) {
                var style = wb.CreateCellStyle();
                style.DataFormat = wb.CreateDataFormat().GetFormat("text");
                if(font!=null)
                    style.SetFont(font);

                var cell = row.CreateCell(cellX);

                if (item is double? || item is double) { 
                    cell.SetCellValue((double)item);
                    cell.CellStyle = style;
                    cell.CellStyle.DataFormat = dataFormatCustom.GetFormat("[>=10000000]##\\,##\\,##\\,##0;[>=100000] ##\\,##\\,##0;##,##0.00");
                }
                else if (item is int || item is int?)
                {
                    cell.SetCellValue((int)item);
                    cell.CellStyle = style;
                    cell.CellStyle.DataFormat = dataFormatCustom.GetFormat("##");
                }
                else if (item is long || item is long?) { 
                    cell.SetCellValue((long)item);
                    cell.CellStyle = style;
                    cell.CellStyle.DataFormat = dataFormatCustom.GetFormat("[>=10000000]##\\,##\\,##\\,##0;[>=100000] ##\\,##\\,##0;##,##0.00");
                }else if (item is string)
                {

                    cell.CellStyle = style;
                    cell.SetCellValue((string)item);
                }
                else if (item is bool || item is bool?)
                {
                    cell.CellStyle = style;
                    cell.SetCellValue((bool)item);
                }
                else if (item is DateTime || item is DateTime?)
                {
                    cell.SetCellValue((DateTime)item);
                    cell.CellStyle.DataFormat = dataFormatCustom.GetFormat("yyyy/mm/dd");
                }
                cellX++;
            }
            return row;
        }
    }

}
