using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetrieveInformation
{
    public static class GeometryObjectHelper
    {
        /// <summary>
        /// 获取元素所有的几何对象
        /// </summary>
        /// <param name="element">选取的元素</param>
        /// <param name="options">限制条件</param>
        /// <returns></returns>
        public static List<GeometryObject> GetGeometryObjects(this Element element, Options options = default(Options))
        {
            var results = new List<GeometryObject>();
            options = options ?? new Options();
            var geometry = element.get_Geometry(options);
            RecurseObject(geometry, ref results);
            return results;
        }

        /// <summary>
        /// 递归遍历制有的几何对象
        /// </summary>
        /// <param name="geometryElement">初始的几何元素</param>
        /// <param name="geometryObjects">递归的结果</param>
        private static void RecurseObject(this GeometryElement geometryElement, ref List<GeometryObject> geometryObjects)
        {
            if (geometryElement == null) return;
            var enumerator = geometryElement.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                switch (current)
                {
                    case GeometryInstance instance:
                        instance.SymbolGeometry.RecurseObject(ref geometryObjects);
                        break;
                    case GeometryElement element:
                        element.RecurseObject(ref geometryObjects);
                        break;
                    case Solid solid:
                        if (solid.Faces.Size == 0 || solid.Edges.Size == 0)
                            continue;
                        geometryObjects.Add(solid);
                        break;
                    default:
                        geometryObjects.Add(current);
                        break;
                }
            }
        }
    }
}
