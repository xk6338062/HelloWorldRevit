using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RetrieveInformation
{
    [Transaction(TransactionMode.ReadOnly)]
    public class GetRoomInfo : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var collector = new FilteredElementCollector(doc).OfClass(typeof(SpatialElement)).ToElements();
            var roomInfoList = new List<List<string>>();
            foreach (Room item in collector)
            {
                var name = item.Name;
                var area = item.Area;
                var levelName = item.Level.Name;
                var parameter = item.get_Parameter(BuiltInParameter.ROOM_HEIGHT);
                var roomHeight = parameter.AsValueString();
                var roomInfo = new List<string> { name, area.ToString(), levelName, roomHeight };
                roomInfoList.Add(roomInfo);
            }

            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet("房间信息");
            var headers = new string[] { "房间名称", "房间面积", "房间所在标高", "房间标示高度" };
            var row0 = sheet.CreateRow(0);
            for (int i = 0; i < headers.Count(); i++)
            {
                var cell = row0.CreateCell(i);
                cell.SetCellValue(headers[i]);
            }
            for (int i = 0; i < roomInfoList.Count; i++)
            {
                var row = sheet.CreateRow(i + 1);
                for (int j = 0; j < roomInfoList[i].Count; j++)
                {
                    var cell = row.CreateCell(j);
                    cell.SetCellValue(roomInfoList[i][j]);
                }
            }

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "(Excel文件)|*.xls";
            fileDialog.FileName = "房间信息统计";
            bool isFileOk = false;
            fileDialog.FileOk += (s, e) => { isFileOk = true; };
            fileDialog.ShowDialog();
            if(isFileOk)
            {
                var path = fileDialog.FileName;
                using (var fs = File.OpenWrite(path))
                {
                    workbook.Write(fs);
                    MessageBox.Show($"文件成功保存至{fileDialog.FileName}", "BIMBOX");
                }
            }
            return Result.Succeeded;
        }
    }
}
