using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetrieveInformation
{
    [Transaction(TransactionMode.ReadOnly)]
    public class FilterElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var collector = new FilteredElementCollector(doc);
            //collector.OfCategory(BuiltInCategory.OST_Doors).OfClass(typeof(FamilyInstance));
            var instanceFilter = new ElementClassFilter(typeof(FamilyInstance));
            var doorCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_Doors);
            var doorInstanceFilter = new LogicalAndFilter(instanceFilter, doorCategoryFilter);
            collector.WherePasses(doorInstanceFilter);
            //var elementList = new List<Element>();
            //foreach (Element item in collector)
            //{
            //    elementList.Add(item);
            //}
            var selectedDoors = from elem in collector where elem.Name == "Entrance door" select elem;
            TaskDialog.Show("BIMBOX", $"名字叫作“Entrance door”的门的族实例有{selectedDoors.Count()}个");
            return Result.Succeeded;
        }
    }
}
