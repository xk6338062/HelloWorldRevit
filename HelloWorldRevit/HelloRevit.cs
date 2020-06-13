using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWorldRevit
{
    [Transaction(TransactionMode.Manual)]
    public class HelloRevit : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //TaskDialog.Show("BIMBOX", "HelloRevit!");  
            LoginWin loginWindow = new LoginWin();
            loginWindow.ShowDialog();
            return Result.Succeeded;
        }
    }
}
