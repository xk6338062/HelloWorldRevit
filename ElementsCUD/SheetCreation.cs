using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RetrieveInformation;

namespace ElementsCUD
{
    [Transaction(TransactionMode.Manual)]
    class SheetCreation : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            try
            {
                var transaction = new Transaction(doc, "创建图纸");
                transaction.Start();

                #region 创建楼层平面
                var level = Level.Create(doc, 1);
                var viewTypeList = from element in new FilteredElementCollector(doc).
                                   OfClass(typeof(ViewFamilyType))
                                   let type = element as ViewFamilyType
                                   where type.ViewFamily == ViewFamily.FloorPlan
                                   select type;
                var viewTypeId = viewTypeList?.FirstOrDefault()?.Id;
                if (viewTypeId == null) throw new Exception("没有找到楼层平面");
                var viewPlan = ViewPlan.Create(doc, viewTypeId, level.Id);
                #endregion

                #region 添加文字

                #region 创建文字类型
                TextNoteType newTextNoteType;
                var textFamilyName = "3.5mm 宋体";
                var textNoteTypeList = from element in new FilteredElementCollector(doc).
                                       OfClass(typeof(TextNoteType))
                                       let type = element as TextNoteType
                                       where type.FamilyName == "文字" && type.Name == textFamilyName
                                       select type;
                if (textNoteTypeList.Count() > 0)
                    newTextNoteType = textNoteTypeList.FirstOrDefault();
                else
                {
                    textNoteTypeList = from element in new FilteredElementCollector(doc).
                                         OfClass(typeof(TextNoteType))
                                       let type = element as TextNoteType
                                       where type.FamilyName == "文字"
                                       select type;
                    var textNoteType = textNoteTypeList.FirstOrDefault();
                    newTextNoteType = textNoteType.Duplicate(textFamilyName) as TextNoteType;
                    newTextNoteType.get_Parameter(BuiltInParameter.TEXT_SIZE).Set(3.5 / 304.8);//文字大小
                    newTextNoteType.get_Parameter(BuiltInParameter.TEXT_FONT).Set("宋体");
                    newTextNoteType.get_Parameter(BuiltInParameter.TEXT_BACKGROUND).Set(1);
                }
                #endregion

                #region 创建文字
                var option = new TextNoteOptions();
                option.HorizontalAlignment = HorizontalTextAlignment.Center;
                option.TypeId = newTextNoteType.Id;
                var textNote = TextNote.Create(doc, viewPlan.Id, new XYZ(0, 0, 0), viewPlan.Name, option);


                #endregion


                #endregion

                #region 应用视图样板
                var viewTemplateList = from element in new FilteredElementCollector(doc).
                                       OfClass(typeof(ViewPlan))
                                       let view = element as ViewPlan
                                       where view.IsTemplate && view.Name == "Architectural Plan"
                                       select view;
                var viewTemplate = viewTemplateList?.FirstOrDefault();
                if (viewTemplate == null) throw new Exception("没有找到视图样板");
                viewPlan.ViewTemplateId = viewTemplate.Id;

                #endregion

                #region 添加标注
                var dimTypeList = from element in new FilteredElementCollector(doc).
                                         OfClass(typeof(DimensionType))
                                  let type = element as DimensionType
                                  where type.Name == "Feet & Inches"
                                  select type;
                var targetDimensionType = dimTypeList?.FirstOrDefault();

                var wall = doc.GetElement(new ElementId(1101836)) as Wall;
                var wallLocationCurve = (wall.Location as LocationCurve).Curve;
                var wallDirection = (wallLocationCurve as Line).Direction;
                var options = new Options();
                options.ComputeReferences = true;
                var wallSolid = wall.GetGeometryObjects(options).FirstOrDefault() as Solid;
                var references = new ReferenceArray();
                foreach (Face face in wallSolid.Faces)
                {
                    if (face is PlanarFace pFace &&
                        pFace.FaceNormal.CrossProduct(wallDirection).IsAlmostEqualTo(XYZ.Zero)
                            )
                    {
                        references.Append(face.Reference);
                    }
                }
                var offset = 1000 / 304.8;
                var line = Line.CreateBound(wallLocationCurve.GetEndPoint(0) + XYZ.BasisY * offset,
                    wallLocationCurve.GetEndPoint(1) + XYZ.BasisY * offset);
                var dimension = doc.Create.NewDimension(viewPlan, line, references);
                dimension.DimensionType = targetDimensionType;


                #endregion
                #region 创建图纸
                var sheet = ViewSheet.Create(doc, new ElementId(382615));


                #endregion

                #region 添加视图到图纸
                if (Viewport.CanAddViewToSheet(doc, sheet.Id, viewPlan.Id))
                {
                    var uv = new XYZ(698 / 304.8 / 2, 522 / 304.8 / 2, 0);
                    var viewPort = Viewport.Create(doc, sheet.Id, viewPlan.Id, uv);
                }

                #endregion

                transaction.Commit();

            }
            catch (Exception e)
            {
                return Result.Failed;
            }

            return Result.Succeeded;

        }


    }
}
