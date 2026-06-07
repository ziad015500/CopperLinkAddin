using System;
using System.Windows.Forms;
using CopperLinkAddin.Models;

namespace CopperLinkAddin.Forms
{
    public partial class HoleForm : Form
    {
        private BusbarModel busbarModel;

        // Tab controls
        private TabControl tabControl;
        private TabPage tabFace1;
        private TabPage tabFace2;

        // Face 1 controls
        private ComboBox cmbHoleType1;
        private TextBox txtDiameter1;
        private TextBox txtSlotWidth1;
        private TextBox txtSlotHeight1;
        private Label lblDiameter1;
        private Label lblSlotWidth1;
        private Label lblSlotHeight1;
        private TextBox txtColumns1;
        private TextBox txtColumnSpacing1;
        private TextBox txtRows1;
        private TextBox txtRowSpacing1;
        private TextBox txtEdgeX1;
        private TextBox txtEdgeY1;
        private CheckBox chkCenter1;
        private CheckBox chkMirror1;

        // Face 2 controls
        private ComboBox cmbHoleType2;
        private TextBox txtDiameter2;
        private TextBox txtSlotWidth2;
        private TextBox txtSlotHeight2;
        private Label lblDiameter2;
        private Label lblSlotWidth2;
        private Label lblSlotHeight2;
        private TextBox txtColumns2;
        private TextBox txtColumnSpacing2;
        private TextBox txtRows2;
        private TextBox txtRowSpacing2;
        private TextBox txtEdgeX2;
        private TextBox txtEdgeY2;
        private CheckBox chkCenter2;

        private Panel mirrorPanel2;

        private Button btnCreate;
        private Button btnSkip;

        public HoleForm(BusbarModel model)
        {
            busbarModel = model;
            BuildForm();
        }

        private void BuildForm()
        {
            this.Text = "CopperLink - Hole Pattern";
            this.Width = 420;
            this.Height = 620;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Tab names حسب النوع
            string face1Name = "";
            string face2Name = "";

            if (busbarModel.ShapeType == 1)
            {
                face1Name = "Top Face";
                face2Name = "Bottom Face";
            }
            else if (busbarModel.ShapeType == 2)
            {
                face1Name = $"First Leg ({busbarModel.D1}mm)";
                face2Name = $"Second Leg ({busbarModel.D2}mm)";
            }
            else if (busbarModel.ShapeType == 3)
            {
                face1Name = $"First Leg ({busbarModel.D1}mm)";
                face2Name = $"Third Leg ({busbarModel.D3}mm)";
            }
            else if (busbarModel.ShapeType == 4)
            {
                face1Name = $"First Leg ({busbarModel.D1}mm)";
                face2Name = $"Second Leg ({busbarModel.D2}mm)";
            }
            else if (busbarModel.ShapeType == 5)
            {
                face1Name = $"First Leg ({busbarModel.D1}mm)";
                face2Name = $"Third Leg ({busbarModel.D3}mm)";
            }

            tabControl = new TabControl();
            tabControl.Left = 10; tabControl.Top = 10;
            tabControl.Width = 385; tabControl.Height = 510;
            this.Controls.Add(tabControl);

            tabFace1 = new TabPage(face1Name);
            tabFace2 = new TabPage(face2Name);
            tabControl.TabPages.Add(tabFace1);
            tabControl.TabPages.Add(tabFace2);

            BuildFace1Panel();
            BuildFace2Panel();

            // Buttons
            btnSkip = new Button();
            btnSkip.Text = "Skip (no holes)";
            btnSkip.Left = 10; btnSkip.Top = 530;
            btnSkip.Width = 130; btnSkip.Height = 30;
            btnSkip.Click += (s, e) => { busbarModel.Face1Holes = null; busbarModel.Face2Holes = null; this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnSkip);

            btnCreate = new Button();
            btnCreate.Text = "Create Busbar";
            btnCreate.Left = 270; btnCreate.Top = 530;
            btnCreate.Width = 120; btnCreate.Height = 30;
            btnCreate.Click += OnCreate;
            this.Controls.Add(btnCreate);
        }

        private void BuildFace1Panel()
        {
            int y = 15;
            Panel p = new Panel();
            p.Dock = DockStyle.Fill;
            p.AutoScroll = true;
            tabFace1.Controls.Add(p);

            // Hole Type
            AddLabelTo(p, "Hole type:", y);
            cmbHoleType1 = new ComboBox();
            cmbHoleType1.Items.AddRange(new string[] { "Circle", "Vertical Slot", "Horizontal Slot" });
            cmbHoleType1.SelectedIndex = 0;
            cmbHoleType1.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbHoleType1.Left = 160; cmbHoleType1.Top = y; cmbHoleType1.Width = 180;
            cmbHoleType1.SelectedIndexChanged += (s, e) => UpdateHoleType1();
            p.Controls.Add(cmbHoleType1);
            y += 35;

            // Circle
            lblDiameter1 = AddLabelTo(p, "Diameter (mm):", y);
            txtDiameter1 = AddTextBoxTo(p, y); y += 35;

            // Slot
            lblSlotWidth1 = AddLabelTo(p, "Width (mm):", y);
            txtSlotWidth1 = AddTextBoxTo(p, y); y += 35;
            lblSlotHeight1 = AddLabelTo(p, "Height (mm):", y);
            txtSlotHeight1 = AddTextBoxTo(p, y); y += 35;

            // Pattern
            AddLabelTo(p, "── PATTERN ──", y); y += 25;
            AddLabelTo(p, "Columns:", y);
            txtColumns1 = AddTextBoxTo(p, y); y += 35;
            AddLabelTo(p, "Column spacing (mm):", y);
            txtColumnSpacing1 = AddTextBoxTo(p, y); y += 35;
            AddLabelTo(p, "Rows:", y);
            txtRows1 = AddTextBoxTo(p, y); y += 35;
            AddLabelTo(p, "Row spacing (mm):", y);
            txtRowSpacing1 = AddTextBoxTo(p, y); y += 35;

            // Position
            AddLabelTo(p, "── POSITION ──", y); y += 25;
            AddLabelTo(p, "Edge distance X (mm):", y);
            txtEdgeX1 = AddTextBoxTo(p, y); y += 35;
            AddLabelTo(p, "Edge distance Y (mm):", y);
            txtEdgeY1 = AddTextBoxTo(p, y); y += 35;

            chkCenter1 = new CheckBox();
            chkCenter1.Text = "Center holes across width";
            chkCenter1.Left = 15; chkCenter1.Top = y;
            chkCenter1.Width = 220;
            chkCenter1.CheckedChanged += (s, e) => {
                txtEdgeY1.Enabled = !chkCenter1.Checked;
                txtEdgeY1.Text = chkCenter1.Checked ? "Auto" : "";
            };
            p.Controls.Add(chkCenter1);
            y += 35;

            // Mirror
            AddLabelTo(p, "── ──", y); y += 20;
            chkMirror1 = new CheckBox();
            chkMirror1.Text = "Mirror to " + tabFace2?.Text;
            chkMirror1.Left = 15; chkMirror1.Top = y;
            chkMirror1.Width = 280;
            chkMirror1.CheckedChanged += OnMirrorChanged;
            p.Controls.Add(chkMirror1);

            UpdateHoleType1();
        }

        private void BuildFace2Panel()
        {
            Panel pMirror = new Panel();
            pMirror.Dock = DockStyle.Fill;
            tabFace2.Controls.Add(pMirror);

            mirrorPanel2 = new Panel();
            mirrorPanel2.Dock = DockStyle.Fill;
            mirrorPanel2.Visible = false;

            Label lblMirror = new Label();
            lblMirror.Text = "Mirror is enabled — this face will use the same pattern as Face 1.";
            lblMirror.Left = 15; lblMirror.Top = 20;
            lblMirror.Width = 340; lblMirror.Height = 40;
            lblMirror.ForeColor = System.Drawing.Color.Gray;
            mirrorPanel2.Controls.Add(lblMirror);
            pMirror.Controls.Add(mirrorPanel2);

            Panel contentPanel2 = new Panel();
            contentPanel2.Name = "contentPanel2";
            contentPanel2.Dock = DockStyle.Fill;
            contentPanel2.AutoScroll = true;
            pMirror.Controls.Add(contentPanel2);

            int y = 15;

            AddLabelTo(contentPanel2, "Hole type:", y);
            cmbHoleType2 = new ComboBox();
            cmbHoleType2.Items.AddRange(new string[] { "Circle", "Vertical Slot", "Horizontal Slot" });
            cmbHoleType2.SelectedIndex = 0;
            cmbHoleType2.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbHoleType2.Left = 160; cmbHoleType2.Top = y; cmbHoleType2.Width = 180;
            cmbHoleType2.SelectedIndexChanged += (s, e) => UpdateHoleType2();
            contentPanel2.Controls.Add(cmbHoleType2);
            y += 35;

            lblDiameter2 = AddLabelTo(contentPanel2, "Diameter (mm):", y);
            txtDiameter2 = AddTextBoxTo(contentPanel2, y); y += 35;

            lblSlotWidth2 = AddLabelTo(contentPanel2, "Width (mm):", y);
            txtSlotWidth2 = AddTextBoxTo(contentPanel2, y); y += 35;
            lblSlotHeight2 = AddLabelTo(contentPanel2, "Height (mm):", y);
            txtSlotHeight2 = AddTextBoxTo(contentPanel2, y); y += 35;

            AddLabelTo(contentPanel2, "── PATTERN ──", y); y += 25;
            AddLabelTo(contentPanel2, "Columns:", y);
            txtColumns2 = AddTextBoxTo(contentPanel2, y); y += 35;
            AddLabelTo(contentPanel2, "Column spacing (mm):", y);
            txtColumnSpacing2 = AddTextBoxTo(contentPanel2, y); y += 35;
            AddLabelTo(contentPanel2, "Rows:", y);
            txtRows2 = AddTextBoxTo(contentPanel2, y); y += 35;
            AddLabelTo(contentPanel2, "Row spacing (mm):", y);
            txtRowSpacing2 = AddTextBoxTo(contentPanel2, y); y += 35;

            AddLabelTo(contentPanel2, "── POSITION ──", y); y += 25;
            AddLabelTo(contentPanel2, "Edge distance X (mm):", y);
            txtEdgeX2 = AddTextBoxTo(contentPanel2, y); y += 35;
            AddLabelTo(contentPanel2, "Edge distance Y (mm):", y);
            txtEdgeY2 = AddTextBoxTo(contentPanel2, y); y += 35;

            chkCenter2 = new CheckBox();
            chkCenter2.Text = "Center holes across width";
            chkCenter2.Left = 15; chkCenter2.Top = y;
            chkCenter2.Width = 220;
            chkCenter2.CheckedChanged += (s, e) => {
                txtEdgeY2.Enabled = !chkCenter2.Checked;
                txtEdgeY2.Text = chkCenter2.Checked ? "Auto" : "";
            };
            contentPanel2.Controls.Add(chkCenter2);

            UpdateHoleType2();
        }

        private void OnMirrorChanged(object sender, EventArgs e)
        {
            bool mirrored = chkMirror1.Checked;
            mirrorPanel2.Visible = mirrored;

            Panel contentPanel2 = (Panel)tabFace2.Controls[0].Controls["contentPanel2"];
            contentPanel2.Visible = !mirrored;
        }

        private void UpdateHoleType1()
        {
            bool isSlot = cmbHoleType1.SelectedIndex > 0;
            lblDiameter1.Visible = txtDiameter1.Visible = !isSlot;
            lblSlotWidth1.Visible = txtSlotWidth1.Visible = isSlot;
            lblSlotHeight1.Visible = txtSlotHeight1.Visible = isSlot;
        }

        private void UpdateHoleType2()
        {
            bool isSlot = cmbHoleType2.SelectedIndex > 0;
            lblDiameter2.Visible = txtDiameter2.Visible = !isSlot;
            lblSlotWidth2.Visible = txtSlotWidth2.Visible = isSlot;
            lblSlotHeight2.Visible = txtSlotHeight2.Visible = isSlot;
        }

        private Label AddLabelTo(Control parent, string text, int y)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Left = 15; lbl.Top = y + 3;
            lbl.Width = 145;
            parent.Controls.Add(lbl);
            return lbl;
        }

        private TextBox AddTextBoxTo(Control parent, int y)
        {
            TextBox txt = new TextBox();
            txt.Left = 160; txt.Top = y;
            txt.Width = 180;
            parent.Controls.Add(txt);
            return txt;
        }

        private HoleModel BuildHoleModel(
            ComboBox cmbType, TextBox txtDiam,
            TextBox txtSW, TextBox txtSH,
            TextBox txtCols, TextBox txtColSp,
            TextBox txtRows, TextBox txtRowSp,
            TextBox txtEX, TextBox txtEY,
            CheckBox chkCen, bool mirror = false)
        {
            string holeType = cmbType.SelectedIndex == 0 ? "Circle"
                            : cmbType.SelectedIndex == 1 ? "VSlot"
                            : "HSlot";

            return new HoleModel
            {
                HoleType = holeType,
                Diameter = ParseOrZero(txtDiam.Text),
                SlotWidth = ParseOrZero(txtSW.Text),
                SlotHeight = ParseOrZero(txtSH.Text),
                Columns = (int)ParseOrZero(txtCols.Text),
                ColumnSpacing = ParseOrZero(txtColSp.Text),
                Rows = (int)ParseOrZero(txtRows.Text),
                RowSpacing = ParseOrZero(txtRowSp.Text),
                EdgeDistanceX = ParseOrZero(txtEX.Text),
                EdgeDistanceY = chkCen.Checked ? -1 : ParseOrZero(txtEY.Text),
                CenterAcrossWidth = chkCen.Checked,
                Mirror = mirror
            };
        }

        private double ParseOrZero(string s)
        {
            return double.TryParse(s, out double v) ? v : 0;
        }

        private void OnCreate(object sender, EventArgs e)
        {
            bool mirrored = chkMirror1.Checked;

            busbarModel.Face1Holes = BuildHoleModel(
                cmbHoleType1, txtDiameter1,
                txtSlotWidth1, txtSlotHeight1,
                txtColumns1, txtColumnSpacing1,
                txtRows1, txtRowSpacing1,
                txtEdgeX1, txtEdgeY1,
                chkCenter1, mirrored);

            if (mirrored)
                busbarModel.Face2Holes = busbarModel.Face1Holes;
            else
                busbarModel.Face2Holes = BuildHoleModel(
                    cmbHoleType2, txtDiameter2,
                    txtSlotWidth2, txtSlotHeight2,
                    txtColumns2, txtColumnSpacing2,
                    txtRows2, txtRowSpacing2,
                    txtEdgeX2, txtEdgeY2,
                    chkCenter2, false);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}