using System;
using System.Drawing;
using System.Windows.Forms;

namespace DesktopKit.Diffchecker
{
    /// <summary>
    /// カラム表示設定ダイアログ。
    /// </summary>
    public class TabOptionForm : Form
    {
        private CheckBox chkUseColumnMode = null!;
        private Label lblMaxWidth = null!;
        private NumericUpDown nudMaxWidth = null!;
        private Label lblTabWidth = null!;
        private NumericUpDown nudTabWidth = null!;
        private Button btnOK = null!;
        private Button btnCancel = null!;

        /// <summary>カラム表示モードを使用するかどうか。</summary>
        public bool UseColumnMode => chkUseColumnMode.Checked;

        /// <summary>最大横幅（文字数）。</summary>
        public int MaxColumnWidth => (int)nudMaxWidth.Value;

        /// <summary>タブ幅（文字数）。</summary>
        public int TabWidth => (int)nudTabWidth.Value;

        /// <summary>
        /// TabOptionFormのコンストラクタ。
        /// </summary>
        /// <param name="useColumnMode">現在のカラム表示モード設定</param>
        /// <param name="maxWidth">現在の最大横幅設定</param>
        /// <param name="tabWidth">現在のタブ幅設定</param>
        public TabOptionForm(bool useColumnMode, int maxWidth, int tabWidth)
        {
            Text = "フォーマットオプション";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(320, 185);
            Font = new Font("Meiryo", 9f);

            chkUseColumnMode = new CheckBox
            {
                Text = "カラム表示を使用する",
                Location = new Point(20, 20),
                AutoSize = true,
                Checked = useColumnMode
            };
            chkUseColumnMode.CheckedChanged += (s, e) =>
            {
                nudMaxWidth.Enabled = chkUseColumnMode.Checked;
                nudTabWidth.Enabled = chkUseColumnMode.Checked;
            };

            lblMaxWidth = new Label
            {
                Text = "最大横幅（文字数）:",
                Location = new Point(20, 58),
                AutoSize = true
            };

            nudMaxWidth = new NumericUpDown
            {
                Minimum = 80,
                Maximum = 200,
                Value = maxWidth,
                Increment = 10,
                Location = new Point(190, 55),
                Size = new Size(80, 25),
                Enabled = useColumnMode
            };

            lblTabWidth = new Label
            {
                Text = "送り文字数（倍数）:",
                Location = new Point(20, 93),
                AutoSize = true
            };

            nudTabWidth = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 16,
                Value = tabWidth,
                Increment = 1,
                Location = new Point(190, 90),
                Size = new Size(80, 25),
                Enabled = useColumnMode
            };

            btnOK = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Size = new Size(90, 30),
                Location = new Point(60, 135)
            };

            btnCancel = new Button
            {
                Text = "キャンセル",
                DialogResult = DialogResult.Cancel,
                Size = new Size(90, 30),
                Location = new Point(170, 135)
            };

            AcceptButton = btnOK;
            CancelButton = btnCancel;

            Controls.AddRange(new Control[] { chkUseColumnMode, lblMaxWidth, nudMaxWidth, lblTabWidth, nudTabWidth, btnOK, btnCancel });
        }
    }
}
