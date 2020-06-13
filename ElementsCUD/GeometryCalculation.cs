using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementsCUD
{
    [Transaction(TransactionMode.Manual)]
    class GeometryCalculation : IExternalCommand,IExternalCommandAvailability
    {
        private Application _app;
        private static Guid _schemaGuid = new Guid("DAFA029F-2115-478B-B56B-9C30F481CBFD");
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var tol = commandData.Application.Application.ShortCurveTolerance;
            var doc = commandData.Application.ActiveUIDocument.Document;
            _app = commandData.Application.Application;
            _app.DocumentChanged += App_DocumentChanged;
            //var point1 = new XYZ(2, 0, 0);
            //var point2 = new XYZ(0, 2, 0);
            //var point3 = new XYZ(3, 3, 0);
            //var line1 = Line.CreateBound(point1, point2);
            //var line2 = Line.CreateBound(XYZ.Zero, point3);
            //IntersectionResultArray results;
            //var result = line1.Intersect(line2, out results);
            //if(result==SetComparisonResult.Overlap)
            //{
            //    var point = results.get_Item(0).XYZPoint;
            //    TaskDialog.Show("BIMBOX", point.ToString());
            //}
            var point1 = new XYZ(0, 0, 0);
            var point2 = new XYZ(5, 0, 0);
            var point3 = new XYZ(5, 8, 0);
            var point4 = new XYZ(0, 8, 0);
            var line1 = Line.CreateBound(point1, point2);
            var line2 = Line.CreateBound(point2, point3);
            var line3 = Line.CreateBound(point3, point4);
            var line4 = Line.CreateBound(point4, point1);
            var curveLoop = new CurveLoop();
            curveLoop.Append(line1);
            curveLoop.Append(line2);
            curveLoop.Append(line3);
            curveLoop.Append(line4);

            var transform = Transform.CreateTranslation(new XYZ(5, 5, 0));
            curveLoop.Transform(transform);
            var solid = GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop> { curveLoop }, XYZ.BasisZ, 10);
            var transaction = new Transaction(doc, "GeometryCreation");
            transaction.Start();
            var shape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));
            shape.SetShape(new GeometryObject[] { solid });

            var schema = Schema.Lookup(_schemaGuid);
            if (schema == null)
            {
                var schemaBuilder = new SchemaBuilder(_schemaGuid);
                schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
                schemaBuilder.SetWriteAccessLevel(AccessLevel.Public);
                schemaBuilder.SetSchemaName("BIMBOX");
                schemaBuilder.SetDocumentation("UniqueFlag");
                var filedBuidler = schemaBuilder.AddSimpleField("Name", typeof(string));
                schema = schemaBuilder.Finish();
            }


            var entity = new Entity(schema);
            var name = schema.GetField("Name");
            entity.Set(name, "Kevin");
            shape.SetEntity(entity);

            var dataStorageList = from element in new FilteredElementCollector(doc).OfClass(typeof(DataStorage)) let storage = element as DataStorage where storage.GetEntitySchemaGuids().Contains(_schemaGuid) select storage;
            var dataStorage = dataStorageList.FirstOrDefault();
            if (dataStorage == null)
                dataStorage = DataStorage.Create(doc);
            dataStorage.SetEntity(entity);

            var dataEntity = dataStorage.GetEntity(schema);
            var field = dataEntity.Schema.GetField("Name");
            var result = dataEntity.Get<string>(field);
            TaskDialog.Show("BIMBOX", "名字叫：" + result);





            transaction.Commit();




            return Result.Succeeded;
        }

        private void App_DocumentChanged(object sender, Autodesk.Revit.DB.Events.DocumentChangedEventArgs e)
        {
            TaskDialog.Show("BIMBOX", "文档已经被修改");
            _app.DocumentChanged -= App_DocumentChanged;
        }

        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            return false;
        }
    }
}
