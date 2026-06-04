using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;

namespace CopperLinkAddin
{
    [Guid("A1B2C3D4-E5F6-7890-ABCD-EF1234567890")]
    [ComVisible(true)]
    public class AddinMain : ISwAddin
    {
        private ISldWorks iSwApp;
        private int addinID;
        private ICommandManager iCmdMgr;
        private ITaskpaneView taskPane;

        public bool ConnectToSW(object ThisSW, int Cookie)
        {
            try
            {
                iSwApp = (ISldWorks)ThisSW;
                addinID = Cookie;
                iCmdMgr = iSwApp.GetCommandManager(Cookie);

                string iconPath = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(
                        System.Reflection.Assembly.GetExecutingAssembly().Location),
                    "Icons\\busbar.bmp");

                taskPane = iSwApp.CreateTaskpaneView2(iconPath, "CopperLink");

                UserControl uc = new UserControl();
                Button btn = new Button();
                btn.Text = "Create Busbar";
                btn.Dock = DockStyle.Top;
                btn.Click += (s, e) => OnCreateBusbar();
                uc.Controls.Add(btn);

                taskPane.DisplayWindowFromHandle(uc.Handle.ToInt32());

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

        public bool DisconnectFromSW()
        {
            try
            {
                taskPane.DeleteView();
                Marshal.ReleaseComObject(iCmdMgr);
                Marshal.ReleaseComObject(iSwApp);
            }
            catch { }
            return true;
        }

        public void OnCreateBusbar()
        {
            MessageBox.Show("Busbar Form هيتفتح هنا!");
        }
    }
}