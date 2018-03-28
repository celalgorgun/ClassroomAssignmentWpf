using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassroomAssignment.Utils
{
    public class NpoiUtil
    {
        public static void copySheets(ISheet newSheet, ISheet sheet)
        {
            copySheets(newSheet, sheet, true);
        }
        public static void copySheets(ISheet newSheet, ISheet sheet, bool copyStyle)
        {
            int maxColumnNum = 0;
            Dictionary<int, ICellStyle> styleMap = (copyStyle)
                    ? new Dictionary<int, ICellStyle>() : null;

            for (int i = sheet.FirstRowNum; i <= sheet.LastRowNum; i++)
            {
                IRow srcRow = sheet.GetRow(i);
                IRow destRow = newSheet.CreateRow(i);
                if (srcRow != null)
                {
                    copyRow(sheet, newSheet, srcRow, destRow, styleMap);
                    if (srcRow.LastCellNum > maxColumnNum)
                    {
                        maxColumnNum = srcRow.LastCellNum;
                    }
                }
            }
            for (int i = 0; i <= maxColumnNum; i++)
            {
                newSheet.SetColumnWidth(i, sheet.GetColumnWidth(i));
            }
        }

        public static void copyRow(ISheet srcSheet, ISheet destSheet, IRow srcRow, IRow destRow, Dictionary<int, ICellStyle> styleMap)
        {
            HashSet<CellRangeAddress> mergedRegions = new HashSet<CellRangeAddress>();
            destRow.Height = srcRow.Height;
            for (int j = srcRow.FirstCellNum; j <= srcRow.LastCellNum; j++)
            {
                ICell oldCell = srcRow.GetCell(j);
                ICell newCell = destRow.GetCell(j);
                if (oldCell != null)
                {
                    if (newCell == null)
                    {
                        newCell = destRow.CreateCell(j);
                    }
                    copyCell(oldCell, newCell, styleMap);
                    CellRangeAddress mergedRegion = getMergedRegion(srcSheet, srcRow.RowNum, oldCell.ColumnIndex);
                    if (mergedRegion != null)
                    {
                        //                    Region newMergedRegion = new Region( destRow.getRowNum(), mergedRegion.getColumnFrom(),
                        //                            destRow.getRowNum() + mergedRegion.getRowTo() - mergedRegion.getRowFrom(), mergedRegion.getColumnTo() );
                        CellRangeAddress newMergedRegion = new CellRangeAddress(mergedRegion.FirstRow, mergedRegion.LastRow,
                                mergedRegion.FirstColumn, mergedRegion.LastColumn);
                        if (isNewMergedRegion(newMergedRegion, mergedRegions))
                        {
                            mergedRegions.Add(newMergedRegion);
                            destSheet.AddMergedRegion(newMergedRegion);
                        }
                    }
                }
            }

        }
        public static void copyCell(ICell oldCell, ICell newCell, Dictionary<int, ICellStyle> styleMap)
        {
            if (styleMap != null)
            {
                if (oldCell.Sheet.Workbook == newCell.Sheet.Workbook)
                {
                    newCell.CellStyle = oldCell.CellStyle;
                }
                else
                {
                    int stHashCode = oldCell.CellStyle.GetHashCode();
                    ICellStyle newCellStyle;
                    if (!styleMap.ContainsKey(stHashCode))
                    {
                        newCellStyle = newCell.Sheet.Workbook.CreateCellStyle();
                        newCellStyle.CloneStyleFrom(oldCell.CellStyle);
                        styleMap.Add(stHashCode, newCellStyle);
                    }
                    newCellStyle = styleMap[stHashCode];
                    newCell.CellStyle = newCellStyle;
                }
            }
            switch (oldCell.CellType)
            {
                case CellType.String:
                    newCell.SetCellValue(oldCell.StringCellValue);
                    break;
                case CellType.Numeric:
                    newCell.SetCellValue(oldCell.NumericCellValue);
                    break;
                case CellType.Blank:
                    newCell.SetCellType(CellType.Blank);
                    break;
                case CellType.Boolean:
                    newCell.SetCellValue(oldCell.BooleanCellValue);
                    break;
                case CellType.Error:
                    newCell.SetCellErrorValue(oldCell.ErrorCellValue);
                    break;
                case CellType.Formula:
                    newCell.SetCellFormula(oldCell.CellFormula);
                    break;
                default:
                    break;
            }

        }
        public static CellRangeAddress getMergedRegion(ISheet sheet, int rowNum, int cellNum)
        {
            for (int i = 0; i < sheet.NumMergedRegions; i++)
            {
                CellRangeAddress merged = sheet.GetMergedRegion(i);
                if (merged.IsInRange(rowNum, cellNum))
                {
                    return merged;
                }
            }
            return null;
        }

        private static bool isNewMergedRegion(CellRangeAddress region, ICollection<CellRangeAddress> mergedRegions)
        {
            return !mergedRegions.Contains(region);

        }
    }
}
