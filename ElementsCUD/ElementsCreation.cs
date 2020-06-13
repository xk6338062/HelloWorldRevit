using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ElementsCUD
{
    [Transaction(TransactionMode.Manual)]
    public class ElementsCreation : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uiDoc = commandData.Application.ActiveUIDocument;
            var doc = commandData.Application.ActiveUIDocument.Document;
            
            var levelList = from element in new FilteredElementCollector(doc).OfClass(typeof(Level)) where element.Name == "标高 1" select element;
            var level = levelList.FirstOrDefault() as Level;

            //var doorId = new ElementId(94654);
            //var doorSymbol = doc.GetElement(doorId) as FamilySymbol;
            //if (!doorSymbol.IsActive) doorSymbol.Activate();
            //try
            //{
            //    var count = 30;
            //    var r1 = 10;
            //    var r2 = 25;
            //    var aphla = 2 * Math.PI / count;
            //    var transCreate = new Transaction(doc, "创建墙体");
            //    transCreate.Start();

            //    for (int i = 0; i < count; i++)
            //    {
            //        var line = Line.CreateBound(new XYZ(r1*Math.Cos(aphla*i), r1*Math.Sin(aphla*i), 0), new XYZ(r2 * Math.Cos(aphla * i), r2 * Math.Sin(aphla * i), 0));
            //        var doorLocation = line.Evaluate(0.5, true);

            //        var wall = Wall.Create(doc, line, level.Id, false);
            //        doc.Create.NewFamilyInstance(doorLocation, doorSymbol, wall, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
            //        doc.Regenerate();
            //        uiDoc.RefreshActiveView();
            //        Thread.Sleep(100);
            //    }

            //    transCreate.Commit();
            //}

            try
            {
                var line = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(10, 0, 0.02));
                var familySymbol = doc.GetElement(new ElementId(265299)) as FamilySymbol;
               
                var trans = new Transaction(doc, "创建梁");
                var options = trans.GetFailureHandlingOptions();
                var processor = new InaccurateFailureProcessor();
                options.SetFailuresPreprocessor(processor);
                trans.SetFailureHandlingOptions(options);
                trans.Start();
                if (!familySymbol.IsActive) familySymbol.Activate();
                doc.Create.NewFamilyInstance(line, familySymbol, level, Autodesk.Revit.DB.Structure.StructuralType.Beam);
                trans.Commit();
            }
            catch(Exception e)
            {
                TaskDialog.Show("BIMBOX", e.Message);
                return Result.Failed;
            }
            return Result.Succeeded;

        }
    }

    public class InaccurateFailureProcessor : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            var failList = failuresAccessor.GetFailureMessages();
            foreach (var item in failList)
            {
                var failureId = item.GetFailureDefinitionId();
                if (failureId == BuiltInFailures.InaccurateFailures.InaccurateBeamOrBrace)
                    failuresAccessor.DeleteWarning(item);
            }
            return FailureProcessingResult.Continue;
        }
    }
}
