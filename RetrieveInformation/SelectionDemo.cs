using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetrieveInformation
{
    [Transaction(TransactionMode.ReadOnly)]
    public class SelectionDemo : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiDoc = commandData.Application.ActiveUIDocument;
            var doc = uiDoc.Document;
            try
            {
                var doorFilter = new DoorSelectionFilter();
                //uiDoc.Selection.SetElementIds(new List<ElementId> { new ElementId(428745) });
                var reference = uiDoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element,doorFilter);
                var element = doc.GetElement(reference.ElementId);
                var door = element as FamilyInstance;
                var geometryObjects = door.GetGeometryObjects();

                //var location = wall.Location as LocationPoint;
                //var point = location.Point;
                //var start = curve.GetEndPoint(0);
                //var end = curve.GetEndPoint(1);
                //var length =UnitUtils.ConvertFromInternalUnits(curve.Length,DisplayUnitType.DUT_MILLIMETERS);
                //var length1 = curve.Length * 304.8;

                //TaskDialog.Show("BIMBOX", $"选择墙的起点是{start.ToString()}\n终点是{end.ToString()}\n长度是{length}mm或者是{length1}mm");
            }
            catch(Autodesk.Revit.Exceptions.OperationCanceledException e)
            {

            }
            
            return Result.Succeeded;
        }
    }

    public class DoorSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Doors)
                return true;
            else return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
