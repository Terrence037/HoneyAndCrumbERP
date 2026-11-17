using System.Drawing;
using System.Windows.Forms;

namespace HoneyAndCrumb.Client
{
    public static class Theme
    {
        // Brand Palette (from Honey & Crumb Color Tables)
        public static readonly Color Espresso = Color.FromArgb(25, 21, 12);         // #19150C
        public static readonly Color CoffeeBrown = Color.FromArgb(76, 64, 36);     // #4C4024
        public static readonly Color WalnutBrown = Color.FromArgb(101, 85, 48);    // #655530
        public static readonly Color MustardGold = Color.FromArgb(203, 170, 96);   // #CBAA60
        public static readonly Color Camel = Color.FromArgb(198, 178, 123);        // #C6B27B
        public static readonly Color LightHoney = Color.FromArgb(253, 217, 138);   // #FDD98A
        public static readonly Color Buttercream = Color.FromArgb(254, 234, 188);  // #FEEABC
        public static readonly Color Cream = Color.FromArgb(255, 243, 228);        // #FFF3E4
        public static readonly Color Eggshell = Color.FromArgb(255, 247, 238);     // #FFF7EE
        public static readonly Color PearlWhite = Color.FromArgb(255, 251, 247);   // #FFFBF7
        public static readonly Color PureWhite = Color.FromArgb(255, 255, 255);    // #FFFFFF
        public static readonly Color ErrorCrimson = Color.FromArgb(165, 42, 42);   // Semantic error

        // HCI Typography
        public static readonly Font TitleFont = new Font("Segoe UI", 16F, FontStyle.Bold);
        public static readonly Font SectionFont = new Font("Segoe UI", 11F, FontStyle.Bold);
        public static readonly Font BodyFont = new Font("Segoe UI", 10F, FontStyle.Regular);
        public static readonly Font LabelFont = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        public static readonly Font SmallFont = new Font("Segoe UI", 9F, FontStyle.Regular);

        public static void StylePrimaryButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = WalnutBrown;
            btn.FlatAppearance.MouseOverBackColor = MustardGold;
            btn.BackColor = LightHoney;
            btn.ForeColor = Espresso;
            btn.Font = LabelFont;
            btn.Cursor = Cursors.Hand;
        }

        public static void StyleDarkButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = CoffeeBrown;
            btn.BackColor = Espresso;
            btn.ForeColor = Cream;
            btn.Font = LabelFont;
            btn.Cursor = Cursors.Hand;
        }

        public static void StyleSecondaryButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Camel;
            btn.FlatAppearance.MouseOverBackColor = Buttercream;
            btn.BackColor = PearlWhite;
            btn.ForeColor = Espresso;
            btn.Font = BodyFont;
            btn.Cursor = Cursors.Hand;
        }

        public static void StyleDataGrid(DataGridView grid)
        {
            grid.BackgroundColor = PearlWhite;
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.GridColor = Camel;
            grid.EnableHeadersVisualStyles = false;
            grid.RowHeadersVisible = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.RowTemplate.Height = 32;

            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.ColumnHeadersHeight = 36;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Espresso;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Cream;
            grid.ColumnHeadersDefaultCellStyle.Font = LabelFont;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Espresso;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Cream;

            grid.DefaultCellStyle.BackColor = PearlWhite;
            grid.DefaultCellStyle.ForeColor = Espresso;
            grid.DefaultCellStyle.Font = BodyFont;
            grid.DefaultCellStyle.SelectionBackColor = LightHoney;
            grid.DefaultCellStyle.SelectionForeColor = Espresso;
            grid.DefaultCellStyle.Padding = new Padding(6, 2, 6, 2);

            grid.AlternatingRowsDefaultCellStyle.BackColor = Eggshell;
            grid.AlternatingRowsDefaultCellStyle.ForeColor = Espresso;
        }

        public static ToolStripRenderer CreateMenuRenderer()
        {
            return new ToolStripProfessionalRenderer(new HoneyCrumbColorTable());
        }

        private class HoneyCrumbColorTable : ProfessionalColorTable
        {
            public override Color MenuStripGradientBegin => Espresso;
            public override Color MenuStripGradientEnd => Espresso;
            public override Color MenuItemSelected => CoffeeBrown;
            public override Color MenuItemSelectedGradientBegin => CoffeeBrown;
            public override Color MenuItemSelectedGradientEnd => CoffeeBrown;
            public override Color MenuItemPressedGradientBegin => WalnutBrown;
            public override Color MenuItemPressedGradientEnd => WalnutBrown;
            public override Color MenuItemBorder => MustardGold;
            public override Color MenuBorder => WalnutBrown;
            public override Color ToolStripDropDownBackground => Espresso;
            public override Color StatusStripGradientBegin => Espresso;
            public override Color StatusStripGradientEnd => Espresso;
        }
    }
}