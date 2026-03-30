using DesktopKit.Common;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace DesktopKit.Diffchecker
{
    /// <summary>
    /// Diffchecker（ファイル差分レポーター）のメインフォーム。
    /// </summary>
    public class MainForm : BaseForm
    {
        /// <summary>
        /// OpenFileDialogで使用するファイルフィルタ文字列。
        /// </summary>
        private const string FileFilter =
            "テキスト系ファイル|*.txt;*.csv;*.json;*.xml;*.html;*.htm;*.css;*.js;*.ts;*.jsx;*.tsx;*.ini;*.log;*.md;*.ps1;*.bat;*.cmd;*.sh;*.py;*.cs;*.csproj;*.yaml;*.yml;*.toml;*.env;*.sql;*.php;*.scss;*.sass;*.less;*.conf;*.cfg;*.vbs;*.reg;*.properties|すべてのファイル|*.*";

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
        private Label lblTabOption = null!;

        /// <summary>最後の差分検出結果（レポート出力で使用）</summary>
        private List<DiffLine> _lastDiffResult = new();

        /// <summary>アプリケーション設定（保存先記憶等）</summary>
        private readonly AppSettings _settings = new("Diffchecker");

        /// <summary>
        /// MainFormのコンストラクタ。
        /// </summary>
        public MainForm()
        {
            ComponentName = "Diffchecker";
            InitializeControls();
            Load += MainForm_Load;
        }

        /// <summary>
        /// フォームのLoadイベント。SplitContainerを均等分割に設定する。
        /// </summary>
        private void MainForm_Load(object? sender, EventArgs e)
        {
            splitDiff.SplitterDistance = ClientSize.Width / 2;
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
                Text = "ファイル①:",
                Location = new Point(10, 12),
                AutoSize = true
            };

            btnBrowse1 = new Button
            {
                Text = "参照",
                Size = new Size(60, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnBrowse1.Location = new Point(topPanel.ClientSize.Width - topPanel.Padding.Right - btnBrowse1.Width, 8);
            btnBrowse1.Click += BtnBrowse1_Click;

            txtFile1Path = new TextBox
            {
                ReadOnly = true,
                AllowDrop = true,
                Location = new Point(90, 10),
                Size = new Size(btnBrowse1.Left - 90 - 6, 23),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            txtFile1Path.DragEnter += TextBox_DragEnter;
            txtFile1Path.DragDrop += TxtFile1Path_DragDrop;

            // ファイル2
            lblFile2 = new Label
            {
                Text = "ファイル②:",
                Location = new Point(10, 45),
                AutoSize = true
            };

            btnBrowse2 = new Button
            {
                Text = "参照",
                Size = new Size(60, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnBrowse2.Location = new Point(topPanel.ClientSize.Width - topPanel.Padding.Right - btnBrowse2.Width, 41);
            btnBrowse2.Click += BtnBrowse2_Click;

            txtFile2Path = new TextBox
            {
                ReadOnly = true,
                AllowDrop = true,
                Location = new Point(90, 43),
                Size = new Size(btnBrowse2.Left - 90 - 6, 23),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            txtFile2Path.DragEnter += TextBox_DragEnter;
            txtFile2Path.DragDrop += TxtFile2Path_DragDrop;

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
                Text = "ファイル①",
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
                Text = "ファイル②",
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
                Size = new Size(120, 28),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Enabled = false
            };
            btnExport.Location = new Point(bottomPanel.ClientSize.Width - bottomPanel.Padding.Right - btnExport.Width, 8);
            btnExport.Click += BtnExport_Click;

            lblTabOption = new Label
            {
                Text = "フォーマットオプション",
                AutoSize = true,
                Font = new Font("Meiryo", 7.5f),
                ForeColor = Color.Blue,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            lblTabOption.Location = new Point(btnExport.Left - lblTabOption.PreferredWidth - 20, 15);
            lblTabOption.Click += LblTabOption_Click;
            lblTabOption.MouseEnter += (s, e) => lblTabOption.Font = new Font(lblTabOption.Font, FontStyle.Underline);
            lblTabOption.MouseLeave += (s, e) => lblTabOption.Font = new Font(lblTabOption.Font, FontStyle.Regular);

            bottomPanel.Controls.Add(lblTabOption);
            bottomPanel.Controls.Add(btnExport);

            // --- フォームに追加 ---
            Controls.Add(splitDiff);
            Controls.Add(topPanel);
            Controls.Add(bottomPanel);

            // 左右スクロール連動
            DiffRenderer.SetupScrollSync(rtbFile1, rtbFile2);

            // StatusBar（リサイズグリップ）をウィンドウ最下層に配置
            StatusBar.SendToBack();
        }

        /// <summary>
        /// 参照ボタン1のClickイベントハンドラ。ファイル選択ダイアログを表示する。
        /// </summary>
        private void BtnBrowse1_Click(object? sender, EventArgs e)
        {
            var path = FileDialogHelper.SelectFile("ファイル①を選択してください", FileFilter);
            if (path != null)
            {
                txtFile1Path.Text = path;
                UpdateCompareButtonState();
            }
        }

        /// <summary>
        /// 参照ボタン2のClickイベントハンドラ。ファイル選択ダイアログを表示する。
        /// </summary>
        private void BtnBrowse2_Click(object? sender, EventArgs e)
        {
            var path = FileDialogHelper.SelectFile("ファイル②を選択してください", FileFilter);
            if (path != null)
            {
                txtFile2Path.Text = path;
                UpdateCompareButtonState();
            }
        }

        /// <summary>
        /// 比較ボタンのClickイベントハンドラ。拡張子チェック後に差分検出・色分け表示を行う。
        /// </summary>
        private void BtnCompare_Click(object? sender, EventArgs e)
        {
            var ext1 = Path.GetExtension(txtFile1Path.Text);
            var ext2 = Path.GetExtension(txtFile2Path.Text);

            if (!string.Equals(ext1, ext2, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    "拡張子が異なるファイルは比較できません。同じ形式のファイルを選択してください。",
                    "拡張子エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                StatusHelper.ShowError(StatusLabel, "エラー：拡張子が一致しません");
                return;
            }

            // ファイル読み込み
            var lines1 = File.ReadAllLines(txtFile1Path.Text, DetectEncoding(txtFile1Path.Text));
            var lines2 = File.ReadAllLines(txtFile2Path.Text, DetectEncoding(txtFile2Path.Text));

            // 差分検出
            _lastDiffResult = DiffEngine.Compare(lines1, lines2);

            // 色分け表示
            DiffRenderer.Render(rtbFile1, rtbFile2, _lastDiffResult);

            // ステータスバー更新
            int modified = _lastDiffResult.Count(d => d.Status == DiffStatus.Modified);
            int added = _lastDiffResult.Count(d => d.Status == DiffStatus.Added);
            int deleted = _lastDiffResult.Count(d => d.Status == DiffStatus.Deleted);

            if (modified + added + deleted == 0)
            {
                StatusHelper.ShowSuccess(StatusLabel, "比較完了 — 差分はありません");
            }
            else
            {
                StatusHelper.ShowSuccess(StatusLabel,
                    $"比較完了 — 変更: {modified}件 / 追加: {added}件 / 削除: {deleted}件");
            }

            btnExport.Enabled = true;
        }

        /// <summary>
        /// ファイルのエンコーディングを検出する。UTF-8を試行し、失敗時はShift_JIS(932)にフォールバックする。
        /// </summary>
        private static Encoding DetectEncoding(string filePath)
        {
            var bytes = File.ReadAllBytes(filePath);

            // BOM付きUTF-8チェック
            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            {
                return new UTF8Encoding(true);
            }

            // BOMなしUTF-8として解読を試行
            try
            {
                var utf8 = new UTF8Encoding(false, true);
                utf8.GetString(bytes);
                return new UTF8Encoding(false);
            }
            catch (DecoderFallbackException)
            {
                // UTF-8でデコードできなければShift_JISにフォールバック
                return Encoding.GetEncoding(932);
            }
        }

        /// <summary>
        /// タブ指定オプションラベルのClickイベントハンドラ。カラム表示設定ダイアログを表示する。
        /// </summary>
        private void LblTabOption_Click(object? sender, EventArgs e)
        {
            bool useColumn = _settings.Get("Diffchecker.UseColumnMode", "false") == "true";
            int maxWidth = int.TryParse(_settings.Get("Diffchecker.MaxColumnWidth", "120"), out var w) ? w : 120;
            int tabWidth = int.TryParse(_settings.Get("Diffchecker.TabWidth", "8"), out var t) ? t : 8;

            using var form = new TabOptionForm(useColumn, maxWidth, tabWidth);
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                _settings.Set("Diffchecker.UseColumnMode", form.UseColumnMode ? "true" : "false");
                _settings.Set("Diffchecker.MaxColumnWidth", form.MaxColumnWidth.ToString());
                _settings.Set("Diffchecker.TabWidth", form.TabWidth.ToString());
            }
        }

        /// <summary>
        /// レポート出力ボタンのClickイベントハンドラ。差分レポートをテキストファイルとして保存する。
        /// </summary>
        private void BtnExport_Click(object? sender, EventArgs e)
        {
            // デフォルトファイル名の生成
            var name1 = Path.GetFileNameWithoutExtension(txtFile1Path.Text);
            var name2 = Path.GetFileNameWithoutExtension(txtFile2Path.Text);
            var defaultFileName = $"diff_{name1}_vs_{name2}_{DateTime.Now:yyyyMMdd}.txt";

            // 前回の保存先を取得
            var lastDir = _settings.Get("Diffchecker.LastSavePath");

            // SaveFileDialog表示
            var savePath = FileDialogHelper.SaveFile(
                "レポートの保存先を選択してください",
                "テキストファイル|*.txt",
                defaultFileName,
                string.IsNullOrEmpty(lastDir) ? null : lastDir);

            if (savePath == null) return;

            // カラム表示モード設定の読み出し
            bool useColumn = _settings.Get("Diffchecker.UseColumnMode", "false") == "true";
            int maxWidth = int.TryParse(_settings.Get("Diffchecker.MaxColumnWidth", "120"), out var w) ? w : 120;
            int tabWidth = int.TryParse(_settings.Get("Diffchecker.TabWidth", "8"), out var t) ? t : 8;

            // レポート保存
            DiffReporter.Export(savePath, txtFile1Path.Text, txtFile2Path.Text, _lastDiffResult, useColumn, maxWidth, tabWidth);

            // 保存先ディレクトリを記憶
            var saveDir = Path.GetDirectoryName(savePath);
            if (!string.IsNullOrEmpty(saveDir))
            {
                _settings.Set("Diffchecker.LastSavePath", saveDir);
            }

            StatusHelper.ShowSuccess(StatusLabel, $"レポートを保存しました：{savePath}");
        }

        /// <summary>
        /// 両方のファイルパスが入力されているかに応じて比較ボタンの有効状態を更新する。
        /// </summary>
        private void UpdateCompareButtonState()
        {
            btnCompare.Enabled = !string.IsNullOrEmpty(txtFile1Path.Text)
                              && !string.IsNullOrEmpty(txtFile2Path.Text);
        }

        /// <summary>
        /// テキストボックスへのドラッグ進入時にファイルドロップを許可する。
        /// </summary>
        private void TextBox_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// ファイル1テキストボックスへのドロップ処理。
        /// </summary>
        private void TxtFile1Path_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
            {
                txtFile1Path.Text = files[0];
                UpdateCompareButtonState();
            }
        }

        /// <summary>
        /// ファイル2テキストボックスへのドロップ処理。
        /// </summary>
        private void TxtFile2Path_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetData(DataFormats.FileDrop) is string[] files && files.Length > 0)
            {
                txtFile2Path.Text = files[0];
                UpdateCompareButtonState();
            }
        }
    }
}
