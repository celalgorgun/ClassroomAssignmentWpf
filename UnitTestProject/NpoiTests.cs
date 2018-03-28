using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ClassroomAssignment.Utils;
using System.Collections.Generic;

namespace UnitTestProject
{
    [TestClass]
    public class NpoiTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var docPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filePath = Path.Combine(docPath, "Classroom Grid.xls");

            // XSSFWorkbook constructor close filestream so need to open filestream again to read
            HSSFWorkbook dest = new HSSFWorkbook();
            HSSFWorkbook src; 
            using (var fileStream = File.OpenRead(filePath))
            {

                src = new HSSFWorkbook(fileStream);
            }

            ISheet srcSheet = src.GetSheetAt(0);
            ISheet destSheet = dest.CreateSheet();
            NpoiUtil.copySheets(destSheet, srcSheet, true);

            var outPath = Path.Combine(docPath, "test.xls");
            using (var fileStream = File.Open(outPath, FileMode.Create, FileAccess.Write))
            {
                dest.Write(fileStream);
            }

            src.Close();
            dest.Close();
        }

    }
}
