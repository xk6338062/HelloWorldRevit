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
    public class TransactionDemo : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            var group = new TransactionGroup(doc);
            group.Start("创建很多墙");
            var transaction = new Transaction(doc); 
            try
            {
                
                transaction.Start("创建一片墙");
                //创建墙体
                var subTran = new SubTransaction(doc);

                transaction.Commit();
                group.Assimilate();
            }
           catch(Exception e)
            {
                if (transaction.GetStatus() == TransactionStatus.Started)
                    transaction.RollBack();

            }
            
            return Result.Succeeded;

        }
    }
}
