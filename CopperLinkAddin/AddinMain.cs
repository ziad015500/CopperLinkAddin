using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using CopperLinkAddin.Forms;

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
        private Timer initTimer;
        private UserControl taskPaneControl;
        private Button btnCreate;

        public bool ConnectToSW(object ThisSW, int Cookie)
        {
            try
            {
                iSwApp = (ISldWorks)ThisSW;
                addinID = Cookie;
                iCmdMgr = iSwApp.GetCommandManager(Cookie);

                initTimer = new Timer();
                initTimer.Interval = 3000;
                initTimer.Tick += OnTimerTick;
                initTimer.Start();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            initTimer.Stop();
            initTimer.Dispose();

            try
            {
                string iconPath = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(
                        System.Reflection.Assembly.GetExecutingAssembly().Location),
                    "Icons\\busbar.bmp");

                taskPane = iSwApp.CreateTaskpaneView2(iconPath, "CopperLink");

                taskPaneControl = new UserControl();
                taskPaneControl.Width = 200;
                taskPaneControl.Height = 400;

                btnCreate = new Button();
                btnCreate.Text = "Create Busbar";
                btnCreate.Dock = DockStyle.Top;
                btnCreate.Height = 35;
                btnCreate.Click += (s, ev) => OnCreateBusbar();
                taskPaneControl.Controls.Add(btnCreate);

                taskPaneControl.Resize += (s, ev) =>
                {
                    btnCreate.Width = taskPaneControl.Width;
                };

                taskPane.DisplayWindowFromHandle(taskPaneControl.Handle.ToInt32());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading UI: " + ex.Message);
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
            BusbarForm form = new BusbarForm(iSwApp);
            form.ShowDialog();
        }
    }
}