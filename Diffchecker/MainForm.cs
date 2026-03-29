using DesktopKit.Common;
using System.Windows.Forms;
using System.Drawing;

namespace DesktopKit.Diffchecker
{
    /// <summary>
    /// Diffchecker（ファイル差分レポーター）のメインフォーム。
    /// </summary>
    public class MainForm : BaseForm
    {
        private Label lblFile1 = null!;
        private TextBox txtFile1Path = null!;
        private Button btnBrowse1 = null!;
        private Label lblFile2 = null!;
        private TextBox txtFile2Path = null!;
        private Button btnBrowse2 = null!;
        private Button btnCompare = null!;
        private SplitContainer splitDiff = null!;
        private Label lblPanel1Title = null!;
        private RichTextBox rtbFile1 = null!;
        private Label lblPanel2Title = null!;
        private RichTextBox rtbFile2 = null!;
        private Button btnExport = null!;

        /// <summary>
        /// MainFormのコンストラクタ。
        /// </summary>
        public MainForm()
        {
            ComponentName = "Diffchecker";
            InitializeControls();
        }

        /// <summary>
        /// UIコントロールを初期化・配置する。
        /// </summary>
        private void InitializeControls()
        {
            // --- 上部パネル: ファイル選択 + 比較ボタン ---
            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 105,
                Padding = new Padding(10, 10, 10, 5)
            };

            // ファイル1
            lblFile1 = new Label
            {
                Text = "ファイル1:",
                Location = new Point(10, 12),
                AutoSize = true
            };

            txtFile1Path = new TextBox
            {
                ReadOnly = true,
                Location = new Point(90, 10),
                Size = new Size(600, 23),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            btnBrowse1 = new Button
            {
                Text = "参照",
                Location = new Point(700, 8),
                Size = new Size(60, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnBrowse1.Click += BtnBrowse1_Click;

            // ファイル2
            lblFile2 = new Label
            {
                Text = "ファイル2:",
                Location = new Point(10, 45),
                AutoSize = true
            };

            txtFile2Path = new TextBox
            {
                ReadOnly = true,
                Location = new Point(90, 43),
                Size = new Size(600, 23),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            btnBrowse2 = new Button
            {
                Text = "参照",
                Location = new Point(700, 41),
                Size = new Size(60, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnBrowse2.Click += BtnBrowse2_Click;

            // 比較ボタン
            btnCompare = new Button
            {
                Text = "比較",
                Location = new Point(10, 73),
                Size = new Size(100, 28),
                Enabled = false
            };
            btnCompare.Click += BtnCompare_Click;

            topPanel.Controls.AddRange(new Control[]
            {
                lblFile1, txtFile1Path, btnBrowse1,
                lblFile2, txtFile2Path, btnBrowse2,
                btnCompare
            });

            // --- 中央: SplitContainer ---
            splitDiff = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 380
            };

            // Panel1: ファイル1
            lblPanel1Title = new Label
            {
                Text = "ファイル1",
                Dock = DockStyle.Top,
                Height = 22
            };

            rtbFile1 = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                WordWrap = false
            };

            splitDiff.Panel1.Controls.Add(rtbFile1);
            splitDiff.Panel1.Controls.Add(lblPanel1Title);

            // Panel2: ファイル2
            lblPanel2Title = new Label
            {
                Text = "ファイル2",
                Dock = DockStyle.Top,
                Height = 22
            };

            rtbFile2 = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                WordWrap = false
            };

            splitDiff.Panel2.Controls.Add(rtbFile2);
            splitDiff.Panel2.Controls.Add(lblPanel2Title);

            // --- 下部パネル: レポート出力ボタン ---
            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 45,
                Padding = new Padding(10, 5, 10, 5)
            };

            btnExport = new Button
            {
                Text = "レポート出力",
                Location = new Point(10, 8),
                Size = new Size(120, 28),
                Enabled = false
            };
            btnExport.Click += BtnExport_Click;

            bottomPanel.Controls.Add(btnExport);

            // --- フォームに追加 ---
            Controls.Add(splitDiff);
            Controls.Add(topPanel);
            Controls.Add(bottomPanel);
        }

        /// <summary>
        /// 参照ボタン1のClickイベントハンドラ。
        /// </summary>
        private void BtnBrowse1_Click(object? sender, System.EventArgs e)
        {
        }

        /// <summary>
        /// 参照ボタン2のClickイベントハンドラ。
        /// </summary>
        private void BtnBrowse2_Click(object? sender, System.EventArgs e)
        {
        }

        /// <summary>
        /// 比較ボタンのClickイベントハンドラ。
        /// </summary>
        private void BtnCompare_Click(object? sender, System.EventArgs e)
        {
        }

        /// <summary>
        /// レポート出力ボタンのClickイベントハンドラ。
        /// </summary>
        private void BtnExport_Click(object? sender, System.EventArgs e)
        {
        }
    }
}
