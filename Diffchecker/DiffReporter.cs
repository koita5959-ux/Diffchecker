using System;
using System.IO;
using System.Linq;
using System.Text;

namespace DesktopKit.Diffchecker
{
    /// <summary>
    /// DiffEngineの出力をテキストレポートとして整形・出力するクラス。
    /// </summary>
    public static class DiffReporter
    {
        /// <summary>
        /// 差分結果をテキストレポートファイルとして保存する（カラム表示オプション付き）。
        /// </summary>
        public static void Export(string filePath, string file1Path, string file2Path,
                                  List<DiffLine> diffs, bool useColumnMode, int maxWidth, int tabWidth = 8)
        {
            if (useColumnMode)
            {
                ExportColumnMode(filePath, file1Path, file2Path, diffs, maxWidth, tabWidth);
            }
            else
            {
                Export(filePath, file1Path, file2Path, diffs);
            }
        }

        /// <summary>
        /// 差分結果をテキストレポートファイルとして保存する。
        /// </summary>
        /// <param name="filePath">保存先ファイルパス</param>
        /// <param name="file1Path">比較元ファイルのパス</param>
        /// <param name="file2Path">比較先ファイルのパス</param>
        /// <param name="diffs">差分結果のリスト</param>
        public static void Export(string filePath, string file1Path, string file2Path, List<DiffLine> diffs)
        {
            var sb = new StringBuilder();

            // ヘッダー
            sb.AppendLine("差分レポート");
            sb.AppendLine("============================================================");
            sb.AppendLine($"ファイル①: {file1Path}");
            sb.AppendLine($"ファイル②: {file2Path}");
            sb.AppendLine($"作成日: {DateTime.Now:yyyy-MM-dd}");
            sb.AppendLine("============================================================");
            sb.AppendLine();

            // セクション1：全行マーク付き表示
            sb.AppendLine("--- 全行比較 ---");
            sb.AppendLine();

            foreach (var diff in diffs)
            {
                switch (diff.Status)
                {
                    case DiffStatus.Equal:
                        sb.AppendLine($"  {FormatLineNumber(diff.LineNumber1)}  {diff.Content1}");
                        break;
                    case DiffStatus.Modified:
                        sb.AppendLine($"~ {FormatLineNumber(diff.LineNumber1)}  {diff.Content1}  \u2192  {diff.Content2}");
                        break;
                    case DiffStatus.Deleted:
                        sb.AppendLine($"- {FormatLineNumber(diff.LineNumber1)}  {diff.Content1}");
                        break;
                    case DiffStatus.Added:
                        sb.AppendLine($"+ {FormatLineNumber(diff.LineNumber2)}  {diff.Content2}");
                        break;
                }
            }

            sb.AppendLine();

            // セクション2：差分サマリー
            sb.AppendLine("--- 差分サマリー ---");
            sb.AppendLine();

            var hasDiff = diffs.Any(d => d.Status != DiffStatus.Equal);
            if (!hasDiff)
            {
                sb.AppendLine("差分はありません。");
            }
            else
            {
                foreach (var diff in diffs)
                {
                    switch (diff.Status)
                    {
                        case DiffStatus.Modified:
                            sb.AppendLine($"[変更] 行 {diff.LineNumber1}:");
                            sb.AppendLine($"  ①: {diff.Content1}");
                            sb.AppendLine($"  ②: {diff.Content2}");
                            sb.AppendLine();
                            break;
                        case DiffStatus.Deleted:
                            sb.AppendLine($"[削除] 行 {diff.LineNumber1}:");
                            sb.AppendLine($"  ①: {diff.Content1}");
                            sb.AppendLine();
                            break;
                        case DiffStatus.Added:
                            sb.AppendLine($"[追加] 行 {diff.LineNumber2}:");
                            sb.AppendLine($"  ②: {diff.Content2}");
                            sb.AppendLine();
                            break;
                    }
                }
            }

            // 集計
            int modified = diffs.Count(d => d.Status == DiffStatus.Modified);
            int added = diffs.Count(d => d.Status == DiffStatus.Added);
            int deleted = diffs.Count(d => d.Status == DiffStatus.Deleted);
            int total = modified + added + deleted;

            sb.AppendLine("--- 集計 ---");
            sb.AppendLine($"変更: {modified}件 / 追加: {added}件 / 削除: {deleted}件 / 合計: {total}件");
            sb.AppendLine("============================================================");

            // UTF-8 BOM付き、CRLFで出力
            var content = sb.ToString().Replace("\r\n", "\n").Replace("\n", "\r\n");
            File.WriteAllText(filePath, content, new UTF8Encoding(true));
        }

        /// <summary>
        /// 行番号を「4桁右揃え:」の書式で返す。
        /// </summary>
        private static string FormatLineNumber(int? lineNumber)
        {
            if (lineNumber == null) return "    :";
            return $"{lineNumber,4}:";
        }

        /// <summary>
        /// カラム表示モードでレポートを出力する。
        /// </summary>
        private static void ExportColumnMode(string filePath, string file1Path, string file2Path,
                                             List<DiffLine> diffs, int maxWidth, int tabWidth)
        {
            // カラム幅の計算
            // 左側固定: マーク(1)+空白(1)+行番号(4)+": "(2) = 8
            // 中央区切り: マージン(4)+罫線(1)+空白(1) = 6
            // 右側行番号: 行番号(4)+": "(2) = 6
            int contentTotal = maxWidth - 20;
            int leftWidth = contentTotal / 2;
            int rightWidth = contentTotal - leftWidth;

            var sb = new StringBuilder();

            // ヘッダー
            sb.AppendLine("差分レポート（カラム表示）");
            sb.AppendLine("============================================================");
            sb.AppendLine($"ファイル①: {file1Path}");
            sb.AppendLine($"ファイル②: {file2Path}");
            sb.AppendLine($"作成日: {DateTime.Now:yyyy-MM-dd}");
            sb.AppendLine($"最大横幅: {maxWidth}文字");
            sb.AppendLine("============================================================");
            sb.AppendLine();

            // セクション1：全行比較（カラム表示）
            sb.AppendLine("--- 全行比較 ---");
            sb.AppendLine();

            // カラムヘッダー
            // 左側: 空白8文字 + "ファイル1" + パディング
            // 右側: 空白 + "ファイル2"
            var headerLeft = "  ファイル①";
            int headerLeftDisplayWidth = GetDisplayWidth(headerLeft);
            int headerLeftPad = 8 + leftWidth + 4 - headerLeftDisplayWidth; // 8(左固定) + leftWidth + margin(4) - 表示幅
            sb.AppendLine(headerLeft + new string(' ', Math.Max(0, headerLeftPad)) + "\u2502  ファイル②");

            // 罫線ヘッダー
            int leftRuleWidth = 8 + leftWidth + 4; // 罫線の左側幅（マージン含む）
            int rightRuleWidth = maxWidth - leftRuleWidth - 1; // 罫線の右側幅（罫線文字1を除く）
            sb.AppendLine(new string('\u2500', leftRuleWidth) + "\u253C" + new string('\u2500', rightRuleWidth));

            // 本体行（差分行のみ出力、Equal行はスキップ）
            foreach (var diff in diffs)
            {
                if (diff.Status == DiffStatus.Equal) continue;

                string leftContent;
                string rightContent;
                string mark;
                string leftLineNum;
                string rightLineNum;

                switch (diff.Status)
                {
                    case DiffStatus.Modified:
                        mark = "~";
                        leftContent = diff.Content1;
                        rightContent = diff.Content2;
                        leftLineNum = FormatLineNumber(diff.LineNumber1);
                        rightLineNum = FormatLineNumber(diff.LineNumber2);
                        break;
                    case DiffStatus.Deleted:
                        mark = "-";
                        leftContent = diff.Content1;
                        rightContent = "";
                        leftLineNum = FormatLineNumber(diff.LineNumber1);
                        rightLineNum = "";
                        break;
                    case DiffStatus.Added:
                        mark = " ";
                        leftContent = "";
                        rightContent = diff.Content2;
                        leftLineNum = "";
                        rightLineNum = "";
                        break;
                    default:
                        continue;
                }

                // タブ文字をスペースに展開
                leftContent = ExpandTabs(leftContent, tabWidth);
                rightContent = ExpandTabs(rightContent, tabWidth);

                int leftDisplayWidth = GetDisplayWidth(leftContent);
                int rightDisplayWidth = GetDisplayWidth(rightContent);

                // フォールバック判定
                if (leftDisplayWidth > leftWidth || rightDisplayWidth > rightWidth)
                {
                    // フォールバック: 従来形式（タブ展開済みの内容を使用）
                    switch (diff.Status)
                    {
                        case DiffStatus.Modified:
                            sb.AppendLine($"~ {FormatLineNumber(diff.LineNumber1)}  {leftContent}  \u2192  {rightContent}");
                            break;
                        case DiffStatus.Deleted:
                            sb.AppendLine($"- {FormatLineNumber(diff.LineNumber1)}  {leftContent}");
                            break;
                        case DiffStatus.Added:
                            sb.AppendLine($"+ {FormatLineNumber(diff.LineNumber2)}  {rightContent}");
                            break;
                    }
                    continue;
                }

                // カラム形式で出力（差分行のみ）
                if (diff.Status == DiffStatus.Added)
                {
                    // Added行: 左側は空白、右側に +行番号: 内容
                    int emptyLeftWidth = 8 + leftWidth + 4;
                    sb.AppendLine(new string(' ', emptyLeftWidth) + "\u2502 +" + FormatLineNumber(diff.LineNumber2) + "  " + rightContent);
                }
                else if (diff.Status == DiffStatus.Deleted)
                {
                    // Deleted行: 左側に内容、右側は空
                    int padSpaces = leftWidth - leftDisplayWidth + 4;
                    sb.AppendLine(mark + " " + leftLineNum + "  " + leftContent + new string(' ', padSpaces) + "\u2502");
                }
                else
                {
                    // Modified: 左右両方に内容
                    int padSpaces = leftWidth - leftDisplayWidth + 4;
                    sb.AppendLine(mark + " " + leftLineNum + "  " + leftContent + new string(' ', padSpaces)
                        + "\u2502 " + rightLineNum + "  " + rightContent);
                }
            }

            sb.AppendLine();

            // セクション2：差分サマリー（既存と同一）
            sb.AppendLine("--- 差分サマリー ---");
            sb.AppendLine();

            var hasDiff = diffs.Any(d => d.Status != DiffStatus.Equal);
            if (!hasDiff)
            {
                sb.AppendLine("差分はありません。");
            }
            else
            {
                foreach (var diff in diffs)
                {
                    switch (diff.Status)
                    {
                        case DiffStatus.Modified:
                            sb.AppendLine($"[変更] 行 {diff.LineNumber1}:");
                            sb.AppendLine($"  ①: {diff.Content1}");
                            sb.AppendLine($"  ②: {diff.Content2}");
                            sb.AppendLine();
                            break;
                        case DiffStatus.Deleted:
                            sb.AppendLine($"[削除] 行 {diff.LineNumber1}:");
                            sb.AppendLine($"  ①: {diff.Content1}");
                            sb.AppendLine();
                            break;
                        case DiffStatus.Added:
                            sb.AppendLine($"[追加] 行 {diff.LineNumber2}:");
                            sb.AppendLine($"  ②: {diff.Content2}");
                            sb.AppendLine();
                            break;
                    }
                }
            }

            // 集計
            int modified = diffs.Count(d => d.Status == DiffStatus.Modified);
            int added = diffs.Count(d => d.Status == DiffStatus.Added);
            int deleted = diffs.Count(d => d.Status == DiffStatus.Deleted);
            int total = modified + added + deleted;

            sb.AppendLine("--- 集計 ---");
            sb.AppendLine($"変更: {modified}件 / 追加: {added}件 / 削除: {deleted}件 / 合計: {total}件");
            sb.AppendLine("============================================================");

            // UTF-8 BOM付き、CRLFで出力
            var content = sb.ToString().Replace("\r\n", "\n").Replace("\n", "\r\n");
            File.WriteAllText(filePath, content, new UTF8Encoding(true));
        }

        /// <summary>
        /// タブ文字をスペースに展開する。タブストップ位置に基づいて計算する。
        /// </summary>
        private static string ExpandTabs(string text, int tabWidth)
        {
            if (!text.Contains('\t')) return text;

            var sb = new StringBuilder();
            int column = 0;
            foreach (char c in text)
            {
                if (c == '\t')
                {
                    int spaces = tabWidth - (column % tabWidth);
                    sb.Append(' ', spaces);
                    column += spaces;
                }
                else
                {
                    sb.Append(c);
                    column += (c >= 0x100) ? 2 : 1;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 文字列の表示幅を計算する。全角文字は2、半角文字は1としてカウント。
        /// </summary>
        private static int GetDisplayWidth(string text)
        {
            int width = 0;
            foreach (char c in text)
            {
                width += (c >= 0x100) ? 2 : 1;
            }
            return width;
        }
    }
}
