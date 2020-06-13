using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ElementsCUD
{
    class RibbonCreation : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            var tabName = "BIMBOX";
            application.CreateRibbonTab(tabName);
            var panel = application.CreateRibbonPanel(tabName, "小工具");
            var assemblyType = new Face2Face().GetType();
            var location = assemblyType.Assembly.Location;
            var className = assemblyType.FullName;
            var pushButtonData = new PushButtonData("tool", "面生面", location, className);
            var imageSource = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)+@"\Images\tools.png";
            pushButtonData.LargeImage = new BitmapImage(new Uri(imageSource));
            var pushButton = panel.AddItem(pushButtonData) as PushButton;

            panel.AddSeparator();

            var assemblyType1 = new GeometryCalculation().GetType();
            var location1 = assemblyType1.Assembly.Location;
            var className1 = assemblyType1.FullName;
            var pushButtonData1 = new PushButtonData("tool1", "创建几何体", location1, className1);
            var imageSource1 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Images\3D.png";
            pushButtonData1.LargeImage = new BitmapImage(new Uri(imageSource1));
            var pushButton1 = panel.AddItem(pushButtonData1) as PushButton;
            pushButton1.AvailabilityClassName = className1;
            var comboBoxData = new ComboBoxData("选项");
            var comboBox = panel.AddItem(comboBoxData) as ComboBox;
            comboBox.ItemText = "选择操作";
            comboBox.ToolTip = "请选择想要进行的操作";
            var comboBox1 = new ComboBoxMemberData("A", "关闭");
            var comboBox2 = new ComboBoxMemberData("B", "关闭并修改");
            comboBox1.GroupName = "编辑操作";
            comboBox.AddItem(comboBox1);
            comboBox.AddItem(comboBox2);
            comboBox.CurrentChanged += change;
            comboBox.DropDownClosed += closed;

            return Result.Succeeded;
        }

        private void closed(object sender, ComboBoxDropDownClosedEventArgs e)
        {
            TaskDialog.Show("关闭", "已关闭");
        }

        private void change(object sender, ComboBoxCurrentChangedEventArgs e)
        {
            TaskDialog.Show("修改", "已修改");
        }
    }
}
