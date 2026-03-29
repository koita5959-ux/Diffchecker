# A3 Diffchecker 制作指示書 v1.0

## 作成：C#開発プロジェクト
## 実装：ClaudeCode
## 品質確認：ユーザー（西村）
## 作成日：2026年3月29日

---

## 0. この文書について

本書はDesktopKitのA3コンポーネント「Diffchecker（ファイル差分レポーター）」の制作指示書である。
Phase 1.5（UI骨格）からPhase 4-3（レポート出力）まで、4つのフェーズに分けて指示する。

### このツールの価値

このツールの差別化ポイントは**差分レポートをテキストファイルとして書き出せる**ことにある。
画面での色分け表示（UI）はベストを尽くすが、レポート出力が「画面で見えたものを文字で再現する」品質を持つことで、UI表示の細かな不完全さを補完できる設計とする。
UIとレポートの両輪で「差分を確認する体験」を支える。

### 作業場所

| 項目 | Windows側パス | Linux側パス |
|---|---|---|
| ソース | `C:\Users\PC\Desktop\app\A3_Diffchecker_app\Diffchecker\` | `/home/webdev/app/A3_Diffchecker_app/Diffchecker/` |
| 共通ライブラリ | `同上\Common\` | `同上/Common/` |
| ビルド出力 | `C:\Users\PC\Desktop\app\A3_Diffchecker_app\publish\` | `/home/webdev/app/A3_Diffchecker_app/publish/` |
| ビルドコマンド | `C:\Users\PC\Desktop\app\A3_Diffchecker_app\build.bat` | — |
| 作業メモ等 | `C:\Users\PC\Desktop\app\A3_Diffchecker_app\_works\` | `/home/webdev/app/A3_Diffchecker_app/_works/` |

### 既存ファイル構成（Phase 1完了状態）

```
A3_Diffchecker_app/
├── .git/
├── _works/
│   └── README.md
├── .gitignore
├── build.bat
├── publish/
│   ├── D3DCompiler_47_cor3.dll
│   ├── DesktopKit.Common.pdb
│   ├── DesktopKit.Diffchecker.exe
│   ├── DesktopKit.Diffchecker.pdb
│   ├── PenImc_cor3.dll
│   ├── PresentationNative_cor3.dll
│   ├── vcruntime140_cor3.dll
│   └── wpfgfx_cor3.dll
└── Diffchecker/
    ├── Common/
    │   ├── DesktopKit.Common.csproj
    │   ├── BaseForm.cs
    │   ├── AppSettings.cs
    │   ├── FileDialogHelper.cs
    │   └── StatusHelper.cs
    ├── bin/
    ├── obj/
    ├── DesktopKit.Diffchecker.csproj
    ├── Directory.Build.props
    ├── MainForm.cs（空のBaseForm継承フォーム）
    └── Program.cs
```

### 共通ルール（全フェーズ共通）

1. BaseFormを継承する既存のMainFormに対して追加・変更する
2. UIの文字列は**すべて日本語**
3. フォントは**メイリオ 9pt**（BaseFormで設定済み）
4. 各フェーズ完了時にビルドエラー0であること
5. installer/フォルダ、setup.issは**作成しない**（統合インストーラーはA3完了後に別途作成する）
6. コードにはクラス・メソッドレベルのXMLコメントを付与すること

### 進め方

- 各フェーズの開始前に把握レポートを出力すること
- 「把握しました」で進めない。何を作るか、どのクラスを作るか、具体的に説明すること
- 指示書と異なる実装をした場合、その理由を必ず報告すること
- 各フェーズ完了時に報告すること：作成・変更ファイル一覧、ビルド結果、差異と理由、補足事項

---

## Phase 1.5：UI骨格の配置

### 目的

MainFormに、「ファイル差分レポーター」が何をするツールか一目でわかるUIコントロールを配置する。
**機能の実装は不要**。ボタンを押しても何も起きなくてよい。

### 画面レイアウト

```
┌─────────────────────────────────────────┐
│ DesktopKit — Diffchecker                │
├─────────────────────────────────────────┤
│                                         │
│  ファイル1: [________________] [参照]    │
│  ファイル2: [________________] [参照]    │
│                                         │
│  [比較]                                  │
│                                         │
│  ┌─ SplitContainer ──────────────────┐  │
│  │  ファイル1        │  ファイル2     │  │
│  │  (RichTextBox)    │  (RichTextBox) │  │
│  │                   │               │  │
│  │                   │               │  │
│  │                   │               │  │
│  └───────────────────────────────────┘  │
│                                         │
│  [レポート出力]                          │
│                                         │
├─────────────────────────────────────────┤
│ 準備完了                                │
└─────────────────────────────────────────┘
```

### 配置するコントロール（上から順に）

**ファイル選択エリア（上部・固定高さ）**

| コントロール | 名前 | プロパティ |
|---|---|---|
| Label | lblFile1 | Text = "ファイル1:" |
| TextBox | txtFile1Path | ReadOnly = true, Anchor = Top,Left,Right |
| Button | btnBrowse1 | Text = "参照", Anchor = Top,Right |
| Label | lblFile2 | Text = "ファイル2:" |
| TextBox | txtFile2Path | ReadOnly = true, Anchor = Top,Left,Right |
| Button | btnBrowse2 | Text = "参照", Anchor = Top,Right |

**操作エリア（中段上・固定高さ）**

| コントロール | 名前 | プロパティ |
|---|---|---|
| Button | btnCompare | Text = "比較", Enabled = false（初期状態） |

**差分表示エリア（中央・画面の大部分を占める・可変高さ）**

| コントロール | 名前 | プロパティ |
|---|---|---|
| SplitContainer | splitDiff | Orientation = Vertical, Dock = なし, Anchor = Top,Bottom,Left,Right |
| Label | lblPanel1Title | Text = "ファイル1"（SplitContainer.Panel1内、上部） |
| RichTextBox | rtbFile1 | ReadOnly = true, Dock = Fill（Panel1内、Labelの下） |
| Label | lblPanel2Title | Text = "ファイル2"（SplitContainer.Panel2内、上部） |
| RichTextBox | rtbFile2 | ReadOnly = true, Dock = Fill（Panel2内、Labelの下） |

**出力エリア（下部・固定高さ）**

| コントロール | 名前 | プロパティ |
|---|---|---|
| Button | btnExport | Text = "レポート出力", Enabled = false（初期状態） |

### レイアウト補足

- SplitContainerが画面中央の大部分を占めるようにすること
- ウィンドウリサイズ時にSplitContainerがサイズに追従すること
- ファイル選択エリアと出力エリアは固定高さ、SplitContainerのみ可変
- btnCompareは初期状態でEnabled = false（Phase 1.5ではそのまま）
- btnExportは初期状態でEnabled = false（Phase 1.5ではそのまま）
- RichTextBoxにはWordWrap = false を設定する（横スクロール可能にする）

### イベントハンドラ

- すべてのボタンにClickイベントハンドラを**空で**作成する
- クリックしても何も起きないが、例外も発生しないこと

### 完了条件

1. ビルドエラー0で通ること
2. exeを起動したとき、上記コントロールが画面に表示されていること
3. ウィンドウをリサイズしたとき、SplitContainerがサイズに追従すること
4. ボタンのクリックでエラーが発生しないこと
5. SplitContainerの分割バーをドラッグして左右パネルの幅を変更できること

### 完了時の報告

- 追加・変更したファイルの一覧
- ビルド結果
- 制作指示との差異がある場合はその理由

---

## Phase 4-1：ファイル選択＋バリデーション

### 目的

参照ボタンが動作し、ファイルパスがテキストボックスに表示される。
2ファイル選択後に「比較」ボタンが有効になる。
拡張子の不一致を検出してNGメッセージを出す。

### 対応ファイル形式

テキスト系ファイル全般を対象とする。OpenFileDialogのフィルタで主要な拡張子を提示し、「すべてのファイル」でそれ以外も選択可能にする。

**OpenFileDialogのフィルタ構成：**

```
テキスト系ファイル|*.txt;*.csv;*.json;*.xml;*.html;*.htm;*.css;*.js;*.ts;*.jsx;*.tsx;*.ini;*.log;*.md;*.ps1;*.bat;*.cmd;*.sh;*.py;*.cs;*.csproj;*.yaml;*.yml;*.toml;*.env;*.sql;*.php;*.scss;*.sass;*.less;*.conf;*.cfg;*.vbs;*.reg;*.properties|すべてのファイル|*.*
```

**拡張子チェックのルール：**
- ファイル1とファイル2の拡張子が同一であること（大文字小文字は区別しない）
- 拡張子が異なる場合はNGとする（フィルタに含まれるかどうかは問わない）
- 「すべてのファイル」で選んだ場合でも、同一拡張子であれば比較可能とする

### 実装内容

**参照ボタン（btnBrowse1, btnBrowse2）**

- クリックでOpenFileDialogを表示する
- 上記フィルタを設定する
- 選択されたパスをtxtFile1Path / txtFile2Pathに表示する
- 共通ライブラリのFileDialogHelperを使用する（SelectFile相当の機能）

**比較ボタンの有効化**

- txtFile1PathとtxtFile2Pathの両方にパスが入った時点でbtnCompareのEnabled = trueにする
- どちらかが空になったらEnabled = falseに戻す

**拡張子チェック（btnCompareのClickイベント内）**

- ファイル1とファイル2の拡張子を比較する（大文字小文字を区別しない）
- 拡張子が異なる場合：
  - MessageBox「拡張子が異なるファイルは比較できません。同じ形式のファイルを選択してください。」を表示（アイコン：Warning）
  - 比較処理には進まない
  - ステータスバーに「エラー：拡張子が一致しません」を表示（StatusHelper.ShowError使用）
- 拡張子が同一の場合：
  - ステータスバーに「比較準備完了」を表示
  - ※この時点では差分検出は未実装。ステータスバーのメッセージ表示のみでよい

### 追加・変更ファイル

- MainForm.cs（イベントハンドラの実装）

### 完了条件

1. 参照ボタンでファイル選択ダイアログが開くこと
2. フィルタに「テキスト系ファイル」と「すべてのファイル」が表示されること
3. 選択したファイルパスがテキストボックスに表示されること
4. 2ファイル選択後に「比較」ボタンが有効になること
5. 拡張子が異なる場合にNGメッセージ（MessageBox）が出ること
6. ステータスバーにエラーまたは準備完了のメッセージが表示されること
7. ビルドエラー0であること

---

## Phase 4-2：差分検出エンジン＋色分け表示

### 目的

2つのテキストファイルを行単位で比較し、差分を色分けでSplitContainer内のRichTextBoxに表示する。
**このフェーズがA3の心臓部である。**

### 新規作成クラス

#### DiffLine.cs（差分データクラス＋列挙型）

**DiffStatus列挙型**

| 値 | 意味 | 説明 |
|---|---|---|
| Equal | 同一 | 両ファイルで同じ行 |
| Added | 追加 | ファイル2にのみ存在する行 |
| Deleted | 削除 | ファイル1にのみ存在する行 |
| Modified | 変更 | 両ファイルに存在するが内容が異なる行 |

**DiffLineクラス（またはrecord）**

| プロパティ | 型 | 説明 |
|---|---|---|
| LineNumber1 | int? | ファイル1での行番号（追加行はnull） |
| LineNumber2 | int? | ファイル2での行番号（削除行はnull） |
| Content1 | string | ファイル1の行内容（追加行は空文字列） |
| Content2 | string | ファイル2の行内容（削除行は空文字列） |
| Status | DiffStatus | 差分の種類 |

配置：Diffcheckerプロジェクト直下。名前空間：DesktopKit.Diffchecker

#### DiffEngine.cs（差分検出エンジン）

| 項目 | 仕様 |
|---|---|
| 配置 | Diffcheckerプロジェクト直下 |
| 名前空間 | DesktopKit.Diffchecker |
| アルゴリズム | 行単位のLCS（Longest Common Subsequence）ベースの差分検出 |
| 入力 | string[] lines1, string[] lines2（各ファイルの行配列） |
| 出力 | List\<DiffLine\>（差分結果のリスト） |

**アルゴリズムの指針：**

1. まずLCSで両ファイルの共通部分（Equal行）を特定する
2. LCSに含まれない行をAdded（ファイル2側）またはDeleted（ファイル1側）に分類する
3. Modifiedの判定：AddedとDeletedが連続して出現する場合、対応する行同士をModifiedとして扱う
   - 例：ファイル1の行10が「timeout=30」、ファイル2の行10が「timeout=60」→ Modified
4. 高度な最適化は不要。数千行程度のファイルで実用的に動作すれば十分

#### DiffRenderer.cs（色分け表示）

| 項目 | 仕様 |
|---|---|
| 配置 | Diffcheckerプロジェクト直下 |
| 名前空間 | DesktopKit.Diffchecker |
| 責務 | DiffEngineの出力をRichTextBoxに色分け描画する |

**色分けルール：**

| DiffStatus | 背景色 | 色コード | 説明 |
|---|---|---|---|
| Equal | なし（デフォルト背景） | — | 変更なしの行 |
| Added | 薄緑 | #E6FFE6 | ファイル2にのみ存在 |
| Deleted | 薄赤 | #FFE6E6 | ファイル1にのみ存在 |
| Modified | 薄黄 | #FFFDE6 | 内容が変更された行 |

**表示仕様：**

- 左パネル（rtbFile1）にファイル1の内容を表示する
  - Equal行：そのまま表示
  - Deleted行：薄赤背景で表示
  - Modified行：薄黄背景で表示
  - Added行：**空行を挿入して位置を合わせる**（薄緑背景で「---」と表示）
- 右パネル（rtbFile2）にファイル2の内容を表示する
  - Equal行：そのまま表示
  - Added行：薄緑背景で表示
  - Modified行：薄黄背景で表示
  - Deleted行：**空行を挿入して位置を合わせる**（薄赤背景で「---」と表示）
- 行番号を先頭に付与する。書式：4桁右揃え＋コロン＋スペース（例：「  12: 」）
  - 位置合わせのための空行には行番号を付けない（スペースで埋める）
- **左右のスクロールを連動させる**（片方をスクロールすると他方も追従）

### MainForm.csの変更

**btnCompareのClickイベント：**

1. ファイル1・ファイル2をFile.ReadAllLines()で読み込む
   - エンコーディング検出：UTF-8（BOM付き/なし）を試行、失敗時はShift_JIS（code page 932）にフォールバック
2. DiffEngine.Compare(lines1, lines2) を呼び出す
3. DiffRenderer.Render(rtbFile1, rtbFile2, diffResult) を呼び出す
4. ステータスバーに「比較完了 — 変更: X件 / 追加: Y件 / 削除: Z件」を表示
5. btnExportのEnabled = trueにする
6. 差分が0件の場合：ステータスバーに「比較完了 — 差分はありません」を表示

**Program.csの変更：**

- `System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);` を追加する
- Shift_JIS（code page 932）を使用可能にするため

**NuGetパッケージの追加：**

- `System.Text.Encoding.CodePages` パッケージをDesktopKit.Diffchecker.csprojに追加する

### 追加ファイル

- DiffLine.cs
- DiffEngine.cs
- DiffRenderer.cs

### 変更ファイル

- MainForm.cs（btnCompareイベント実装）
- Program.cs（エンコーディングプロバイダー登録）
- DesktopKit.Diffchecker.csproj（NuGetパッケージ追加）

### テスト用ファイルの作成

以下のテスト用ファイルペアを `_works/testdata/` に作成すること。
テスト用ファイルのエンコーディングはUTF-8（BOM付き）とする。

**テスト1：同一ファイル**
- test1_file1.txt と test1_file2.txt：同じ内容（5行程度の日本語テキスト）
- 期待結果：全行Equal、差分0件

**テスト2：追加のみ**
- test2_file1.txt：3行
- test2_file2.txt：file1の内容＋末尾に2行追加
- 期待結果：追加2件

**テスト3：削除のみ**
- test3_file1.txt：5行
- test3_file2.txt：file1から2行を削除した内容
- 期待結果：削除2件

**テスト4：変更あり**
- test4_file1.txt：5行（設定ファイル風 key=value形式）
- test4_file2.txt：file1の2行目と4行目のvalueを変更
- 期待結果：変更2件

**テスト5：複合（追加＋削除＋変更）**
- test5_file1.txt：10行
- test5_file2.txt：追加2行、削除1行、変更1行
- 期待結果：追加2件、削除1件、変更1件

### 完了条件

1. テスト1：同一内容の2ファイルを比較したとき、全行がEqual（色なし）で表示されること
2. テスト2：追加行が右パネルで緑背景、左パネルで位置合わせの空行が表示されること
3. テスト3：削除行が左パネルで赤背景、右パネルで位置合わせの空行が表示されること
4. テスト4：変更行が両パネルで黄背景で表示されること
5. テスト5：追加・削除・変更が混在した場合に正しく表示されること
6. 左右パネルの行位置が揃っていること（空行挿入による位置合わせ）
7. 左右のスクロールが連動すること
8. ステータスバーに差分件数が表示されること
9. 差分0件のとき「差分はありません」と表示されること
10. ビルドエラー0であること

---

## Phase 4-3：レポート出力＋保存先記憶

### 目的

差分結果をテキストファイルとして書き出す。保存先を記憶し、次回起動時に同じ場所がデフォルトで開く。
**レポートは「画面で見えたものを文字で再現する」品質を目指す。**

### 新規作成クラス

#### DiffReporter.cs（レポート出力）

| 項目 | 仕様 |
|---|---|
| 配置 | Diffcheckerプロジェクト直下 |
| 名前空間 | DesktopKit.Diffchecker |
| 責務 | DiffEngineの出力をテキストレポートとして整形・出力する |

### レポートフォーマット

レポートは**2つのセクション**で構成する。

**セクション1：全行マーク付き表示**
画面の色分け表示をテキストで再現する。全行を出力し、差分のある行にマークを付ける。

```
差分レポート
============================================================
比較元: C:\work\config_v1.txt
比較先: C:\work\config_v2.txt
作成日: 2026-03-29
============================================================

--- 全行比較 ---

     1:   server=web01
~    2:   timeout=30  →  timeout=60
-    3:   debug=true
+    4:   max_retry=3
     5:   port=8080
```

**マーク凡例：**

| マーク | 意味 | 画面での色 |
|---|---|---|
| （空白） | 同一行 | 色なし |
| ~ | 変更行 | 黄背景 |
| - | 削除行（ファイル1にのみ存在） | 赤背景 |
| + | 追加行（ファイル2にのみ存在） | 緑背景 |

**変更行の表記：**
- 変更行は「元の内容  →  新しい内容」の形式で1行にまとめる
- 矢印の前後にスペース2つを入れる

**削除行・追加行の表記：**
- 削除行はファイル1の内容をそのまま表示する
- 追加行はファイル2の内容をそのまま表示する

**セクション2：差分サマリー**
差分のある行だけを抜粋して、変更の種類ごとに整理する。

```
--- 差分サマリー ---

[変更] 行 2:
  元: timeout=30
  先: timeout=60

[削除] 行 3:
  元: debug=true

[追加] 行 4:
  先: max_retry=3

--- 集計 ---
変更: 1件 / 追加: 1件 / 削除: 1件 / 合計: 3件
============================================================
```

**差分が0件の場合：**

```
--- 全行比較 ---

     1:   server=web01
     2:   timeout=30
     3:   port=8080

--- 差分サマリー ---

差分はありません。

--- 集計 ---
変更: 0件 / 追加: 0件 / 削除: 0件 / 合計: 0件
============================================================
```

### 出力仕様

| 項目 | 仕様 |
|---|---|
| エンコーディング | UTF-8（BOM付き） |
| 改行コード | CRLF |
| 行番号書式 | 4桁右揃え＋コロン＋スペース（画面表示と同じ） |

### MainForm.csの変更

**btnExportのClickイベント：**

1. SaveFileDialogを表示する
2. フィルタ：「テキストファイル|*.txt」
3. デフォルトファイル名の生成ルール：
   - `diff_[ファイル1名]_vs_[ファイル2名]_[yyyyMMdd].txt`
   - ファイル名は拡張子を除いた名前部分を使用する
   - 例：config_v1.txt と config_v2.txt → `diff_config_v1_vs_config_v2_20260329.txt`
4. 前回の保存先をAppSettingsから読み出し、InitialDirectoryに設定する
5. DiffReporter.Export() を呼び出してレポートを保存する
6. 保存実行後、保存先ディレクトリをAppSettingsに記憶する
7. ステータスバーに「レポートを保存しました：[ファイルパス]」を表示（StatusHelper.ShowSuccess使用）

**AppSettingsのキー：**
- `Diffchecker.LastSavePath`：前回のレポート保存先ディレクトリ

### 追加ファイル

- DiffReporter.cs

### 変更ファイル

- MainForm.cs（btnExportイベント実装）

### 完了条件

1. 「レポート出力」ボタンでSaveFileDialogが開くこと
2. デフォルトファイル名が正しく生成されること（diff_xxx_vs_yyy_yyyyMMdd.txt）
3. レポートにセクション1（全行マーク付き表示）が含まれていること
4. レポートにセクション2（差分サマリー）が含まれていること
5. マーク（~, -, +）が正しい行に付いていること
6. 変更行の「元 → 先」表記が正しいこと
7. 集計（変更X件/追加Y件/削除Z件）が画面表示の件数と一致すること
8. 差分0件の場合に「差分はありません。」と出力されること
9. 保存先が記憶され、次回起動時に同じ場所が開くこと
10. レポートのエンコーディングがUTF-8（BOM付き）であること
11. ビルドエラー0であること

---

## 全フェーズ完了後の成果物

### ファイル構成（完成時）

```
A3_Diffchecker_app/
├── .git/
├── _works/
│   ├── README.md
│   └── testdata/（テスト用ファイルペア 5セット）
│       ├── test1_file1.txt
│       ├── test1_file2.txt
│       ├── test2_file1.txt
│       ├── test2_file2.txt
│       ├── test3_file1.txt
│       ├── test3_file2.txt
│       ├── test4_file1.txt
│       ├── test4_file2.txt
│       ├── test5_file1.txt
│       └── test5_file2.txt
├── .gitignore
├── build.bat
├── publish/
└── Diffchecker/
    ├── Common/（既存・変更なし）
    │   ├── DesktopKit.Common.csproj
    │   ├── BaseForm.cs
    │   ├── AppSettings.cs
    │   ├── FileDialogHelper.cs
    │   └── StatusHelper.cs
    ├── bin/
    ├── obj/
    ├── DesktopKit.Diffchecker.csproj（NuGetパッケージ追加あり）
    ├── Directory.Build.props
    ├── MainForm.cs（UI＋全イベントハンドラ）
    ├── Program.cs（エンコーディングプロバイダー追加）
    ├── DiffLine.cs（差分データクラス＋DiffStatus列挙型）
    ├── DiffEngine.cs（差分検出エンジン）
    ├── DiffRenderer.cs（色分け表示）
    └── DiffReporter.cs（レポート出力）
```

### 品質チェックリスト

- [ ] ビルドエラー0
- [ ] exeを起動してUIが正しく表示される
- [ ] ウィンドウリサイズでSplitContainerが追従する
- [ ] 参照ボタンでファイル選択ができる（フィルタにテキスト系＋すべてのファイル）
- [ ] 拡張子不一致でNGメッセージが出る
- [ ] 同一ファイルの比較で全行Equalになる
- [ ] 追加・削除・変更が正しい色で表示される
- [ ] 左右パネルの行位置が揃っている
- [ ] 左右のスクロールが連動する
- [ ] レポート出力：セクション1（全行マーク付き）が正しい
- [ ] レポート出力：セクション2（差分サマリー）が正しい
- [ ] レポート出力：集計が画面表示と一致する
- [ ] 保存先が記憶される

---

## 改訂履歴

| 日付 | バージョン | 内容 |
|---|---|---|
| 2026-03-29 | v1.0 | 初版作成。企画相談に基づきC#開発が作成 |
