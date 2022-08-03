using eraRenamer.Config;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace eraRenamer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        const int LOG = -1;
        const int FILE_PROCESSED = 1;
        BackgroundWorker bgWorker = new BackgroundWorker();
        BackgroundWorker fileCountWorker = null;
        string rootPath = string.Empty;
        int fileCount = 0;
        AutoResetEvent _resetEvent = new AutoResetEvent(false);

        Dictionary<string, CSV設定CSV種類> 逆引きCSV設定 = new Dictionary<string, CSV設定CSV種類>();

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            foreach (var item in Configuration.GetCSV設定.基本機能変換)
            {
                if (item.追加変換トークン != null && item.追加変換トークン.Length > 0)
                    逆引きCSV設定[item.トークン] = item;
            }
        }

        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        private void BgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == FILE_PROCESSED)
            {
                ProgressBar.Value += e.ProgressPercentage;
                ProgressText.Text = e.UserState.ToString();
            }
            else if (e.ProgressPercentage == LOG)
            {
                var text = new System.Windows.Controls.TextBox();
                text.Text = e.UserState.ToString();
                text.IsReadOnly = true;
                Log.Children.Add(text);
                File.AppendAllText("Log.txt", $"{ e.UserState.ToString()}{Environment.NewLine}");
            }
        }

        private void BgWorker_DoCreateRename(object sender, DoWorkEventArgs e)
        {
            try
            {
                var root = new DirectoryInfo(rootPath);
                var csvDir = root.GetDirectories("CSV").FirstOrDefault();
                var 基本機能変換Dictionary = new Dictionary<string, Dictionary<string, string>>();

                var renameCsvFileName = System.IO.Path.Combine(csvDir.FullName, "_Rename.csv");
                if (File.Exists(renameCsvFileName))
                {
                    var backupRenameCsvFileName = System.IO.Path.Combine(csvDir.FullName, $"_Rename{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}.csv");
                    File.Copy(renameCsvFileName, backupRenameCsvFileName);
                }
                using (var renameCsvFile = File.Open(renameCsvFileName, FileMode.Create))
                {
                    using (var sw = new StreamWriter(renameCsvFile, new UTF8Encoding(true)))
                    {
                        foreach (var CSV種類 in Configuration.GetCSV設定.基本機能変換)
                        {
                            基本機能変換Dictionary[CSV種類.種類] = GetDictionary(csvDir, string.IsNullOrWhiteSpace(CSV種類.ファイル名) ? $"{CSV種類.トークン}.csv" : CSV種類.ファイル名);
                            if (基本機能変換Dictionary[CSV種類.種類].Count > 0)
                            {
                                sw.WriteLine($";-----------------------------------------------------------------");
                                sw.WriteLine($"{CSV種類.種類}");
                                sw.WriteLine($";-----------------------------------------------------------------");
                                foreach (var item in 基本機能変換Dictionary[CSV種類.種類])
                                {
                                    sw.WriteLine($"{CSV種類.トークン}:{item.Key},{item.Value}");
                                }
                            }
                        }
                        foreach (var CSV種類 in Configuration.GetCSV設定.文字列の変換)
                        {
                            try
                            {
                                var tmpResultDict = new Dictionary<string, string>();
                                foreach (var item in System.IO.Directory.GetFiles(csvDir.FullName, CSV種類.ファイル名パターン, SearchOption.AllDirectories))
                                {
                                    var tmpDict = GetCsvDictionary(item);
                                    if (tmpDict.ContainsKey(CSV種類.変換前パラ) && tmpDict.ContainsKey(CSV種類.変換後パラ))
                                        tmpResultDict[tmpDict[CSV種類.変換前パラ]] = tmpDict[CSV種類.変換後パラ];
                                }
                                if (tmpResultDict.Count > 0)
                                {
                                    sw.WriteLine($";-----------------------------------------------------------------");
                                    sw.WriteLine($"{CSV種類.種類}");
                                    sw.WriteLine($";-----------------------------------------------------------------");

                                    foreach (var item in tmpResultDict.OrderBy(kvpair => int.Parse(kvpair.Key)))
                                    {
                                        sw.WriteLine($"{item.Key},{CSV種類.変換後前置}:{item.Value}");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                bgWorker.ReportProgress(LOG, $"Error:{ex.Message}:{ex.StackTrace}");
                            }
                        }
                    }
                }
                bgWorker.ReportProgress(FILE_PROCESSED, "終了");
            }
            catch (Exception ex)
            {
                bgWorker.ReportProgress(LOG, $"Error:{ex.Message}:{ex.StackTrace}");
            }

        }

        private void BgWorker_DoRename(object sender, DoWorkEventArgs e)
        {
            try
            {
                var outputEncoding = new UTF8Encoding(true);

                var root = new DirectoryInfo(rootPath);
                var csvDir = root.GetDirectories("CSV").FirstOrDefault();
                var combinedDict = GetDictionary(csvDir, "_Rename.csv");

                foreach (var item in System.IO.Directory.GetFiles(rootPath, "*.erb", SearchOption.AllDirectories))
                {
                    var erbContent = File.ReadAllText(item);
                    erbContent = ReplaceContent(combinedDict, erbContent);
                    File.WriteAllText(item, erbContent, outputEncoding);
                    bgWorker.ReportProgress(FILE_PROCESSED, item);
                }
                bgWorker.ReportProgress(FILE_PROCESSED, "終了");
            }
            catch (Exception ex)
            {
                bgWorker.ReportProgress(LOG, $"Error:{ex.Message}:{ex.StackTrace}");
            }

        }

        private string ReplaceContent(Dictionary<string, string> combinedDict, string erbContent)
        {
            foreach (var renameEntry in combinedDict)
            {
                var keyParts = renameEntry.Key.Split(':');
                if (keyParts.Length > 1)
                {
                    var value = renameEntry.Value;
                    erbContent = Regex.Replace(erbContent, $"{keyParts[0]}:{keyParts[1]}(?![0-9])", $"{keyParts[0]}:{value}");
                    erbContent = Regex.Replace(erbContent, $"{keyParts[0]}:([0-9a-zA-Z]+):{keyParts[1]}(?![0-9])", $"{keyParts[0]}:$1:{value}");
                    if (逆引きCSV設定.ContainsKey(keyParts[0]))
                    {
                        foreach (var item in 逆引きCSV設定[keyParts[0]].追加変換トークン)
                        {
                            erbContent = Regex.Replace(erbContent, $"{item.トークン}:{keyParts[1]}(?![0-9])", $"{item.トークン}:{value}");
                            erbContent = Regex.Replace(erbContent, $"{item.トークン}:([0-9a-zA-Z]+):{keyParts[1]}(?![0-9])", $"{item.トークン}:$1:{value}");
                        }
                    }
                }

            }

            return erbContent;
        }

        private Dictionary<string, string> GetCsvDictionary(string csvFullFileName)
        {
            var result = new Dictionary<string, string>();
            if (File.Exists(csvFullFileName))
            {
                var entries = File.ReadAllLines(csvFullFileName);
                foreach (var item in entries)
                {
                    var values = item.Split(new string[] { "," }, StringSplitOptions.None);
                    if (values.Length < 2 ||
                        string.IsNullOrWhiteSpace(values[0]) ||
                        string.IsNullOrWhiteSpace(values[1]) ||
                        values[0].Trim().StartsWith(";") ||
                        values[1].Trim().StartsWith(";"))
                        continue;
                    result[values[0]] = values[1].Replace(' ', '_');
                }
            }
            return result;
        }

        private Dictionary<string, string> GetDictionary(DirectoryInfo csvDir, string csvName)
        {
            var csvFullFileName = System.IO.Path.Combine(csvDir.FullName, csvName);
            var result = new Dictionary<string, string>();
            if (File.Exists(csvFullFileName))
            {
                var entries = File.ReadAllLines(csvFullFileName);
                foreach (var item in entries)
                {
                    var values = item.Split(new string[] { "," }, StringSplitOptions.None);
                    if (values.Length < 2 ||
                        string.IsNullOrWhiteSpace(values[0]) ||
                        string.IsNullOrWhiteSpace(values[1]) ||
                        values[0].Trim().StartsWith(";") ||
                        values[1].Trim().StartsWith(";") ||
                        (Regex.IsMatch(values[1].Trim(), "([0-9a-zA-Z]+)")) && !values[1].Contains(":"))
                        continue;
                    //if (values[1].Contains(" "))
                    //    continue;
                    result[values[0]] = values[1].Replace(' ', '_');
                }
            }
            return result;
        }

        private void RenameWithRenameFile_Click(object sender, RoutedEventArgs e)
        {
            RunBackgroundRename();
        }

        private void CreateNewRenameFile_Click(object sender, RoutedEventArgs e)
        {
            RunBackgroundCreateRename();
        }

        private void RunBackgroundRename()
        {
            if (bgWorker.IsBusy != true)
            {
                GC.Collect();

                ProgressText.Text = "ERBファイル数計算中...";
                ProgressBar.Value = 0;
                Log.Children.Clear();
                rootPath = FolderPath.Text.Trim();

                fileCountWorker = new BackgroundWorker();
                fileCountWorker.DoWork += (object doSender, DoWorkEventArgs arg) =>
                {
                    fileCount = System.IO.Directory.GetFiles(rootPath, "*.erb", SearchOption.AllDirectories).Count();
                    _resetEvent.Set();
                };

                fileCountWorker.RunWorkerAsync();
                _resetEvent.WaitOne();
                ProgressBar.Maximum = fileCount;

                GC.Collect();

                // Start the asynchronous operation.
                bgWorker = new BackgroundWorker();
                bgWorker.WorkerReportsProgress = true;
                bgWorker.DoWork += BgWorker_DoRename;
                bgWorker.ProgressChanged += BgWorker_ProgressChanged;
                bgWorker.RunWorkerCompleted += BgWorker_RunWorkerCompleted;
                bgWorker.RunWorkerAsync();
            }
        }

        private void RunBackgroundCreateRename()
        {
            if (bgWorker.IsBusy != true)
            {
                GC.Collect();

                ProgressBar.Value = 0;
                rootPath = FolderPath.Text.Trim();

                ProgressBar.Maximum = 1;

                GC.Collect();

                // Start the asynchronous operation.
                bgWorker = new BackgroundWorker();
                bgWorker.WorkerReportsProgress = true;
                bgWorker.DoWork += BgWorker_DoCreateRename;
                bgWorker.ProgressChanged += BgWorker_ProgressChanged;
                bgWorker.RunWorkerCompleted += BgWorker_RunWorkerCompleted;
                bgWorker.RunWorkerAsync();
            }
        }
    }
}
