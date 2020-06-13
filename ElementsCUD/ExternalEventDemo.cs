using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementsCUD
{
    [Transaction(TransactionMode.Manual)]
    class ExternalEventDemo : IExternalCommand
    {
        private ExternalEvent _wallCreation;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            _wallCreation = ExternalEvent.Create(new ExternalCommand());
            var doc = commandData.Application.ActiveUIDocument.Document;
            var window = new Window1();
            window.Show();
            window.CreationExternal += create;
            return Result.Succeeded;
        }

        private void create(object sender, EventArgs e)
        {
            _wallCreation?.Raise();
        }
    }

    class ExternalCommand : IExternalEventHandler
    {
        public void Execute(UIApplication app)
        {
            var doc = app.ActiveUIDocument.Document;
            var levelId = new ElementId(311);
            var curve = Line.CreateBound(XYZ.Zero, new XYZ(10, 0, 0));
            var tran = new Transaction(doc, "Create");
            tran.Start();
            Wall.Create(doc, curve, levelId, false);
            tran.Commit();
        }

        public string GetName()
        {
            return "WallCreation";
        }
    }
}
