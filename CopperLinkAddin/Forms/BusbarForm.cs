using System;
using System.Windows.Forms;

namespace CopperLinkAddin.Forms
{
    public partial class BusbarForm : Form
    {
        private ComboBox cmbType;
        private TextBox txtThickness;
        private TextBox txtBusbarWidth;
        private TextBox txtBendRadius;
        private TextBox txtD1;
        private TextBox txtD2;
        private TextBox txtD3;
        private Button btnCreate;
        private Label lblBendRadius;
        private Label lblD1;
        private Label lblD2;
        private Label lblD3;

        public BusbarForm()
        {
            InitializeComponent();
            BuildForm();
        }

        private void BuildForm()
        {
            this.Text = "CopperLink - Create Busbar";
            this.Width = 350;
            this.Height = 450;
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

            btnCreate = new Button();
            btnCreate.Text = "Create Busbar";
            btnCreate.Left = 100; btnCreate.Top = y;
            btnCreate.Width = 140; btnCreate.Height = 35;
            btnCreate.Click += OnCreate;
            this.Controls.Add(btnCreate);

            UpdateVisibility();
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
            MessageBox.Show("Creating Busbar...");
            this.Close();
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