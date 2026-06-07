using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using CopperLinkAddin.Models;
using CopperLinkAddin.Services;
using SolidWorks.Interop.sldworks;

namespace CopperLinkAddin.Forms
{
    public partial class BusbarForm : Form
    {
        private ISldWorks iSwApp;

        private ComboBox cmbType;
        private ComboBox cmbMaterial;
        private TextBox txtThickness;
        private TextBox txtBusbarWidth;
        private TextBox txtBendRadius;
        private TextBox txtD1;
        private TextBox txtD2;
        private TextBox txtD3;
        private TextBox txtSavePath;
        private Button btnCreate;
        private Label lblBendRadius;
        private Label lblD1;
        private Label lblD2;
        private Label lblD3;
        private PictureBox previewBox;

        public BusbarForm(ISldWorks swApp)
        {
            iSwApp = swApp;
            InitializeComponent();
            BuildForm();
        }

        private void BuildForm()
        {
            this.Text = "CopperLink - Create Busbar";
            this.Width = 350;
            this.Height = 700;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;

            int y = 20;

            AddLabel("Busbar Type:", y);
            cmbType = new ComboBox();
            cmbType.Items.AddRange(new string[] {
                "1 - Straight Bar (I)",
                "2 - L Shape - Flat Bend",
                "3 - Z Shape - Flat Bend",
                "4 - L Shape - Edge",
                "5 - Z Shape - Edge"
            });
            cmbType.SelectedIndex = 0;
            cmbType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbType.Left = 150; cmbType.Top = y; cmbType.Width = 160;
            cmbType.SelectedIndexChanged += OnTypeChanged;
            this.Controls.Add(cmbType);
            y += 40;

            AddLabel("Material:", y);
            cmbMaterial = new ComboBox();
            cmbMaterial.Items.AddRange(new string[] {
                "Copper (CU)",
                "Aluminium (AL)"
            });
            cmbMaterial.SelectedIndex = 0;
            cmbMaterial.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMaterial.Left = 150; cmbMaterial.Top = y; cmbMaterial.Width = 160;
            cmbMaterial.SelectedIndexChanged += OnTypeChanged;
            this.Controls.Add(cmbMaterial);
            y += 40;

            AddLabel("Thickness (mm):", y);
            txtThickness = AddTextBox(y); y += 40;

            AddLabel("Width (mm):", y);
            txtBusbarWidth = AddTextBox(y); y += 40;

            lblBendRadius = AddLabel("Bend Radius (mm):", y);
            txtBendRadius = AddTextBox(y); y += 40;

            lblD1 = AddLabel("Length (mm):", y);
            txtD1 = AddTextBox(y); y += 40;

            lblD2 = AddLabel("Second Length (mm):", y);
            txtD2 = AddTextBox(y); y += 40;

            lblD3 = AddLabel("Third Length (mm):", y);
            txtD3 = AddTextBox(y); y += 40;

            // Save Path
            AddLabel("Save Folder:", y);
            txtSavePath = AddTextBox(y);
            txtSavePath.Width = 110;

            Button btnBrowse = new Button();
            btnBrowse.Text = "...";
            btnBrowse.Left = 265; btnBrowse.Top = y;
            btnBrowse.Width = 45; btnBrowse.Height = 21;
            btnBrowse.Click += OnBrowse;
            this.Controls.Add(btnBrowse);
            y += 40;

            // Preview Box
            previewBox = new PictureBox();
            previewBox.Left = 20;
            previewBox.Top = y;
            previewBox.Width = 290;
            previewBox.Height = 120;
            previewBox.BorderStyle = BorderStyle.FixedSingle;
            previewBox.BackColor = Color.White;
            previewBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.Controls.Add(previewBox);
            y += 130;

            btnCreate = new Button();
            btnCreate.Text = "Create Busbar";
            btnCreate.Left = 100; btnCreate.Top = y;
            btnCreate.Width = 140; btnCreate.Height = 35;
            btnCreate.Click += OnCreate;
            this.Controls.Add(btnCreate);

            UpdateVisibility();
            UpdatePreview();
        }

        private string GenerateFileName()
        {
            int type = cmbType.SelectedIndex + 1;
            string mat = cmbMaterial.SelectedIndex == 0 ? "CU" : "AL";

            string shapeName = "";
            switch (type)
            {
                case 1: shapeName = "Straight_Bar"; break;
                case 2: shapeName = "L_Shape_Flat_Bend"; break;
                case 3: shapeName = "Z_Shape_Flat_Bend"; break;
                case 4: shapeName = "L_Shape_Edge"; break;
                case 5: shapeName = "Z_Shape_Edge"; break;
            }

            string dims = "";
            double d1 = double.TryParse(txtD1.Text, out double v1) ? v1 : 0;
            double d2 = double.TryParse(txtD2.Text, out double v2) ? v2 : 0;
            double d3 = double.TryParse(txtD3.Text, out double v3) ? v3 : 0;

            if (type == 1)
                dims = $"{d1}";
            else if (type == 2 || type == 4)
                dims = $"{d1}x{d2}";
            else if (type == 3 || type == 5)
                dims = $"{d1}x{d2}x{d3}";

            return $"{shapeName}_{dims}_{mat}";
        }

        private void OnBrowse(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "Select Save Folder";

            if (dlg.ShowDialog() == DialogResult.OK)
                txtSavePath.Text = dlg.SelectedPath;
        }

        private string GetImageName()
        {
            int type = cmbType.SelectedIndex + 1;
            string mat = cmbMaterial.SelectedIndex == 0 ? "CU" : "AL";

            switch (type)
            {
                case 1: return $"Straight_Bar_{mat}";
                case 2: return $"L_Shape_Flat_Bend_{mat}";
                case 3: return $"Z_Shape_Flat_Bend_{mat}";
                case 4: return $"L_Shape_Edge_{mat}";
                case 5: return $"Z_Shape_Edge_{mat}";
                default: return $"Straight_Bar_{mat}";
            }
        }

        private void UpdatePreview()
        {
            string imageName = GetImageName();
            string resourceName = $"CopperLinkAddin.Images.{imageName}.PNG";

            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                        previewBox.Image = Image.FromStream(stream);
                    else
                        previewBox.Image = null;
                }
            }
            catch
            {
                previewBox.Image = null;
            }
        }

        private Label AddLabel(string text, int y)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Left = 20; lbl.Top = y + 3;
            lbl.Width = 130;
            this.Controls.Add(lbl);
            return lbl;
        }

        private TextBox AddTextBox(int y)
        {
            TextBox txt = new TextBox();
            txt.Left = 150; txt.Top = y;
            txt.Width = 160;
            this.Controls.Add(txt);
            return txt;
        }

        private void OnTypeChanged(object sender, EventArgs e)
        {
            UpdateVisibility();
            UpdatePreview();
        }

        private void UpdateVisibility()
        {
            int type = cmbType.SelectedIndex + 1;

            lblBendRadius.Visible = (type == 2 || type == 3);
            txtBendRadius.Visible = (type == 2 || type == 3);

            lblD2.Visible = (type != 1);
            txtD2.Visible = (type != 1);

            lblD3.Visible = (type == 3 || type == 5);
            txtD3.Visible = (type == 3 || type == 5);

            lblD1.Text = (type == 1) ? "Length (mm):" : "First Length (mm):";
            lblD2.Text = (type == 3 || type == 5) ? "Offset Length (mm):" : "Second Length (mm):";
        }

        private void OnCreate(object sender, EventArgs e)
        {
            if (!double.TryParse(txtThickness.Text, out double thickness) || thickness <= 0)
            {
                MessageBox.Show("Please enter a valid Thickness.", "Validation Error");
                txtThickness.Focus();
                return;
            }

            if (!double.TryParse(txtBusbarWidth.Text, out double width) || width <= 0)
            {
                MessageBox.Show("Please enter a valid Width.", "Validation Error");
                txtBusbarWidth.Focus();
                return;
            }

            if (txtBendRadius.Visible)
            {
                if (!double.TryParse(txtBendRadius.Text, out double br) || br <= 0)
                {
                    MessageBox.Show("Please enter a valid Bend Radius.", "Validation Error");
                    txtBendRadius.Focus();
                    return;
                }
            }

            if (!double.TryParse(txtD1.Text, out double d1) || d1 <= 0)
            {
                MessageBox.Show("Please enter a valid Length.", "Validation Error");
                txtD1.Focus();
                return;
            }

            if (txtD2.Visible)
            {
                if (!double.TryParse(txtD2.Text, out double d2) || d2 <= 0)
                {
                    MessageBox.Show("Please enter a valid Second Length.", "Validation Error");
                    txtD2.Focus();
                    return;
                }
            }

            if (txtD3.Visible)
            {
                if (!double.TryParse(txtD3.Text, out double d3) || d3 <= 0)
                {
                    MessageBox.Show("Please enter a valid Third Length.", "Validation Error");
                    txtD3.Focus();
                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(txtSavePath.Text))
            {
                MessageBox.Show("Please select a Save Folder.", "Validation Error");
                return;
            }

            try
            {
                string fileName = GenerateFileName();

                BusbarModel model = new BusbarModel
                {
                    ShapeType = ShapeType,
                    Thickness = BusbarThickness,
                    Width = BusbarWidth,
                    BendRadius = BendRadius,
                    D1 = D1,
                    D2 = D2,
                    D3 = D3,
                    Material = cmbMaterial.SelectedIndex == 0 ? "CU" : "AL",
                    SavePath = txtSavePath.Text,
                    FileName = fileName
                };

                BusbarService service = new BusbarService(iSwApp);
                service.CreateBusbar(model);

                MessageBox.Show($"Saved as:\n{fileName}.SLDPRT", "Success");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public int ShapeType => cmbType.SelectedIndex + 1;
        public double BusbarThickness => double.Parse(txtThickness.Text);
        public double BusbarWidth => double.Parse(txtBusbarWidth.Text);
        public double BendRadius => txtBendRadius.Visible ? double.Parse(txtBendRadius.Text) : 0;
        public double D1 => double.Parse(txtD1.Text);
        public double D2 => txtD2.Visible ? double.Parse(txtD2.Text) : 0;
        public double D3 => txtD3.Visible ? double.Parse(txtD3.Text) : 0;
    }
}