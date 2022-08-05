using EraCsvManager.Config;
using EraCsvManager.MVVM;

using EraDataHandler;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;

namespace EraCsvManager.VM
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        public bool BackupOutput { get; set; }
        public bool UpdateErb { get; set; }
        public bool ReverseErb { get; set; }
        public bool ReverseWithRename { get; set; }

        public string[] Prefices = new string[] { "ABL", "EXP", "TALENT", "TRAIN", "MARK", "ITEM", "BASE", "EQUIP", "TEQUIP", "FLAG", "TFLAG", "CFLAG", "PALAM", "JUEL", "TCVAR", "TSTR", "CSTR" };


        protected string outErbExeDir = @"I:\Temp\era\erAV_ver030_Custom22";
        public string OutErbExeDir
        {
            get
            {
                return outErbExeDir;
            }
            set
            {
                if (value != outErbExeDir)
                {
                    outErbExeDir = value;
                    NotifyPropertyChanged();
                }
            }
        }

        protected string erbExeDir = @"I:\Temp\era\erAV_Ho";
        public string ErbExeDir
        {
            get
            {
                return erbExeDir;
            }
            set
            {
                if (value != erbExeDir)
                {
                    erbExeDir = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string ErbDir => Path.Combine(ErbExeDir, "ERB\\");

        public string CsvDir => Path.Combine(ErbExeDir, "CSV\\");

        public string ContentDir => CsvDir;

        public string configPath => Path.Combine(ErbExeDir, "emuera.config");

        public string configdebugPath => Path.Combine(ErbExeDir, "emuera.config");

        public bool AnalysisMode => false;

        protected List<CharaModel> characterList;
        public List<CharaModel> CharacterList => characterList;

        protected DataTable renameDataTable;
        public DataView RenameDataView => renameDataTable?.AsDataView();

        protected DataTable ablDataTable;
        public DataView AblDataView => ablDataTable?.AsDataView();

        protected DataTable expDataTable;
        public DataView ExpDataView => expDataTable?.AsDataView();

        protected DataTable talentDataTable;
        public DataView TalentDataView => talentDataTable?.AsDataView();

        protected DataTable trainDataTable;
        public DataView TrainDataView => trainDataTable?.AsDataView();

        protected DataTable markDataTable;
        public DataView MarkDataView => markDataTable?.AsDataView();

        protected DataTable itemDataTable;
        public DataView ItemDataView => itemDataTable?.AsDataView();

        protected DataTable baseDataTable;
        public DataView BaseDataView => baseDataTable?.AsDataView();

        protected DataTable equipDataTable;
        public DataView EquipDataView => equipDataTable?.AsDataView();

        protected DataTable tequipDataTable;
        public DataView TEquipDataView => tequipDataTable?.AsDataView();

        protected DataTable flagDataTable;
        public DataView FlagDataView => flagDataTable?.AsDataView();

        protected DataTable tflagDataTable;
        public DataView TFlagDataView => tflagDataTable?.AsDataView();

        protected DataTable cflagDataTable;
        public DataView CFlagDataView => cflagDataTable?.AsDataView();

        protected DataTable tcvarDataTable;
        public DataView TCVarDataView => tcvarDataTable?.AsDataView();

        protected DataTable tstrDataTable;
        public DataView TStrDataView => tstrDataTable?.AsDataView();

        protected DataTable cstrDataTable;
        public DataView CStrDataView => cstrDataTable?.AsDataView();

        protected DataTable strNameDataTable;
        public DataView StrNameDataView => strNameDataTable?.AsDataView();

        protected DataTable strDataTable;
        public DataView StrDataView => strDataTable?.AsDataView();

        protected DataTable palamDataTable;
        public DataView PalamDataView => palamDataTable?.AsDataView();

        protected DataTable saveStrDataTable;
        public DataView SaveStrDataView => saveStrDataTable?.AsDataView();

        protected DataTable cdFlag1DataTable;
        public DataView CDFlag1DataView => cdFlag1DataTable?.AsDataView();

        protected DataTable cdFlag2DataTable;
        public DataView CDFlag2DataView => cdFlag2DataTable?.AsDataView();

        public static readonly Regex sWhitespace = new Regex(@"\s+");
        public static readonly Regex sIrregular = new Regex(@"\W+");

        public Dictionary<string, string> 逆引きCSV設定 = new Dictionary<string, string>();
        public Dictionary<string, string> 逆引きキャラID = new Dictionary<string, string>();
        public List<string> Prefixes = new List<string>();

        public bool Loaded = false;

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged

        protected ICommand loadCsvCommand = null;
        public ICommand LoadCsvCommand
        {
            get
            {
                if (loadCsvCommand == null)
                {
                    loadCsvCommand = new RelayCommand((obj) => true, (obj) =>
                    {
                        try
                        {
                            逆引きキャラID = new Dictionary<string, string>();

                            #region Load data

                            var genCsvModel = new GeneralCsvModel(this.CsvDir);

                            #endregion

                            #region Create renames
                            renameDataTable = new DataTable();
                            renameDataTable.CaseSensitive = true;
                            var column = new DataColumn("pre")
                            {
                                DefaultValue = "[[",
                                ReadOnly = true
                            };
                            renameDataTable.Columns.Add(column);
                            column = new DataColumn("置き換えパターン");
                            renameDataTable.Columns.Add(column);
                            renameDataTable.PrimaryKey = new DataColumn[] { renameDataTable.Columns["置き換えパターン"] };
                            column = new DataColumn("post")
                            {
                                DefaultValue = "]]=>",
                                ReadOnly = true
                            };
                            renameDataTable.Columns.Add(column);
                            column = new DataColumn("置き換え値");
                            renameDataTable.Columns.Add(column);
                            column = new DataColumn("コメント");
                            renameDataTable.Columns.Add(column);
                            foreach (var item in genCsvModel.Rename)
                            {
                                if (Prefices.Any(pref => item.Name.Contains(pref)))
                                    continue;
                                var row = renameDataTable.NewRow();
                                row["置き換えパターン"] = item.Name;
                                row["置き換え値"] = item.Value;
                                row["コメント"] = item.Comment;
                                renameDataTable.Rows.Add(row);
                            }
                            #endregion

                            #region Load chara data
                            characterList = genCsvModel.Charas;
                            foreach (var chara in characterList.OrderBy(tmpchara => tmpchara.No))
                            {
                                var pattern = $"キャラ:{/*sWhitespace.Replace(*/chara.Name/*, "")*/}";
                                var value = $"{chara.No}";
                                逆引きキャラID[value] = pattern;
                                if (characterList.Count(tmpchara => tmpchara.Name == chara.Name) > 1)
                                {
                                    var count = characterList.Where(tmpchara => tmpchara.Name == chara.Name).Count(tmpchara => tmpchara.No < chara.No);
                                    if (count > 0)
                                        pattern += $"_重複{count}";
                                }
                                if (renameDataTable.AsEnumerable().Where(row => row["置き換えパターン"].ToString() == pattern).Count() == 0)
                                {
                                    var row = renameDataTable.NewRow();
                                    row["置き換えパターン"] = pattern;
                                    row["置き換え値"] = value;
                                    row["コメント"] = "自動追加";
                                    renameDataTable.Rows.Add(row);
                                }
                                else
                                {
                                    foreach (DataRow row in renameDataTable.AsEnumerable().Where(row => row["置き換えパターン"].ToString() == pattern))
                                    {
                                        row["置き換え値"] = value;
                                    }
                                }
                            }
                            NotifyPropertyChanged("CharacterList");
                            #endregion

                            #region AblDataView
                            var prefix = "ABL";
                            AppendRenameData(genCsvModel.Abl, prefix);
                            ablDataTable = CreateUiTable(genCsvModel.Abl);
                            NotifyPropertyChanged("AblDataView");
                            #endregion

                            #region ExpDataView
                            prefix = "EXP";
                            AppendRenameData(genCsvModel.Exp, prefix);
                            expDataTable = CreateUiTable(genCsvModel.Exp);
                            NotifyPropertyChanged("ExpDataView");
                            #endregion

                            #region TalentDataView
                            prefix = "TALENT";
                            AppendRenameData(genCsvModel.Talent, prefix);
                            talentDataTable = CreateUiTable(genCsvModel.Talent);
                            NotifyPropertyChanged("TalentDataView");
                            #endregion

                            #region TrainDataView
                            prefix = "TRAIN";
                            AppendRenameData(genCsvModel.Train, prefix);
                            trainDataTable = CreateUiTable(genCsvModel.Train);
                            NotifyPropertyChanged("TrainDataView");
                            #endregion

                            #region MarkDataView
                            prefix = "MARK";
                            AppendRenameData(genCsvModel.Mark, prefix);
                            markDataTable = CreateUiTable(genCsvModel.Mark);
                            NotifyPropertyChanged("MarkDataView");
                            #endregion

                            #region ItemDataView
                            prefix = "ITEM";
                            AppendRenameData(genCsvModel.Item.Select(item => item as IdNameValueComment).ToList(), prefix);
                            itemDataTable = CreateUiTable(genCsvModel.Item);
                            NotifyPropertyChanged("ItemDataView");
                            #endregion

                            #region BaseDataView
                            prefix = "BASE";
                            AppendRenameData(genCsvModel.Base, prefix);
                            baseDataTable = CreateUiTable(genCsvModel.Base);
                            NotifyPropertyChanged("BaseDataView");
                            #endregion

                            #region EquipDataView
                            prefix = "EQUIP";
                            AppendRenameData(genCsvModel.Equip, prefix);
                            equipDataTable = CreateUiTable(genCsvModel.Equip);
                            NotifyPropertyChanged("EquipDataView");
                            #endregion

                            #region TEquipDataView
                            prefix = "TEQUIP";
                            AppendRenameData(genCsvModel.TEquip, prefix);
                            tequipDataTable = CreateUiTable(genCsvModel.TEquip);
                            NotifyPropertyChanged("TEquipDataView");
                            #endregion

                            #region FlagDataView
                            prefix = "FLAG";
                            AppendRenameData(genCsvModel.Flag, prefix);
                            flagDataTable = CreateUiTable(genCsvModel.Flag);
                            NotifyPropertyChanged("FlagDataView");
                            #endregion

                            #region TFlagDataView
                            prefix = "TFLAG";
                            AppendRenameData(genCsvModel.TFlag, prefix);
                            tflagDataTable = CreateUiTable(genCsvModel.TFlag);
                            NotifyPropertyChanged("TFlagDataView");
                            #endregion

                            #region CFlagDataView
                            prefix = "CFLAG";
                            AppendRenameData(genCsvModel.CFlag, prefix);
                            cflagDataTable = CreateUiTable(genCsvModel.CFlag);
                            NotifyPropertyChanged("CFlagDataView");
                            #endregion

                            #region PalamDataView
                            prefix = "PALAM";
                            AppendRenameData(genCsvModel.Param, prefix);
                            palamDataTable = CreateUiTable(genCsvModel.Param);
                            NotifyPropertyChanged("PalamDataView");
                            prefix = "JUEL";
                            AppendRenameData(genCsvModel.Param, prefix);
                            #endregion

                            #region TCVarDataView
                            prefix = "TCVAR";
                            AppendRenameData(genCsvModel.TCVar, prefix);
                            tcvarDataTable = CreateUiTable(genCsvModel.TCVar);
                            NotifyPropertyChanged("TCVarDataView");
                            #endregion

                            #region TStrDataView
                            prefix = "TSTR";
                            AppendRenameData(genCsvModel.TStr, prefix);
                            tstrDataTable = CreateUiTable(genCsvModel.TStr);
                            NotifyPropertyChanged("TStrDataView");
                            #endregion

                            #region CStrDataView
                            prefix = "CSTR";
                            AppendRenameData(genCsvModel.CStr, prefix);
                            cstrDataTable = CreateUiTable(genCsvModel.CStr);
                            NotifyPropertyChanged("CStrDataView");
                            #endregion

                            #region SaveStrDataView
                            prefix = "SAVESTR";
                            AppendRenameData(genCsvModel.SaveStr, prefix);
                            saveStrDataTable = CreateUiTable(genCsvModel.SaveStr);
                            NotifyPropertyChanged("SaveStrDataView");
                            #endregion

                            #region CDFlag1DataView
                            prefix = "CSTR";
                            AppendRenameData(genCsvModel.CDFlag1, prefix);
                            cdFlag1DataTable = CreateUiTable(genCsvModel.CDFlag1);
                            NotifyPropertyChanged("CDFlag1DataView");
                            #endregion

                            #region CDFlag2DataView
                            prefix = "CSTR";
                            AppendRenameData(genCsvModel.CDFlag2, prefix);
                            cdFlag2DataTable = CreateUiTable(genCsvModel.CDFlag2);
                            NotifyPropertyChanged("CDFlag2DataView");
                            #endregion

                            NotifyPropertyChanged("RenameDataView");

                            #region StrNameDataView
                            strNameDataTable = new DataTable();
                            column = new DataColumn("ID", typeof(int));
                            column.AutoIncrement = true;
                            column.AutoIncrementSeed = 0;
                            column.AutoIncrementStep = 1;
                            strNameDataTable.Columns.Add(column);
                            strNameDataTable.PrimaryKey = new DataColumn[] { strNameDataTable.Columns["ID"] };
                            column = new DataColumn("名前");
                            strNameDataTable.Columns.Add(column);
                            column = new DataColumn("コメント");
                            strNameDataTable.Columns.Add(column);
                            foreach (var item in genCsvModel.StrName)
                            {
                                var row = strNameDataTable.NewRow();
                                row["ID"] = item.Id;
                                row["名前"] = genCsvModel.StrName.FirstOrDefault(name => name.Id == item.Id)?.Name;
                                row["コメント"] = item.Comment;
                                strNameDataTable.Rows.Add(row);
                            }
                            strNameDataTable.DefaultView.Sort = "ID ASC";
                            NotifyPropertyChanged("StrNameDataView");
                            #endregion

                            #region StrDataView
                            strDataTable = new DataTable();
                            column = new DataColumn("ID", typeof(int));
                            column.AutoIncrement = true;
                            column.AutoIncrementSeed = 0;
                            column.AutoIncrementStep = 1;
                            strDataTable.Columns.Add(column);
                            strDataTable.PrimaryKey = new DataColumn[] { strDataTable.Columns["ID"] };
                            column = new DataColumn("名前");
                            strDataTable.Columns.Add(column);
                            column = new DataColumn("文字列");
                            strDataTable.Columns.Add(column);
                            column = new DataColumn("コメント");
                            strDataTable.Columns.Add(column);
                            foreach (var item in genCsvModel.Str)
                            {
                                var row = strDataTable.NewRow();
                                row["ID"] = item.Id;
                                row["名前"] = genCsvModel.StrName.FirstOrDefault(name => name.Id == item.Id)?.Name;
                                row["文字列"] = item.Name;
                                row["コメント"] = item.Comment;
                                strDataTable.Rows.Add(row);
                            }
                            strDataTable.DefaultView.Sort = "ID ASC";
                            NotifyPropertyChanged("StrDataView");
                            #endregion

                            Loaded = true;
                        }
                        catch (Exception ex)
                        {
                        }
                    });
                }
                return loadCsvCommand;
            }
        }

        private DataTable CreateUiTable(List<IdNameValueComment> nameList)
        {
            var table = new DataTable();
            var column = new DataColumn("ID", typeof(int));
            column.AutoIncrement = true;
            column.AutoIncrementSeed = 0;
            column.AutoIncrementStep = 1;
            table.Columns.Add(column);
            table.PrimaryKey = new DataColumn[] { table.Columns["ID"] };
            column = new DataColumn("名前");
            table.Columns.Add(column);
            column = new DataColumn("コメント");
            table.Columns.Add(column);
            foreach (var item in nameList)
            {
                var row = table.NewRow();
                row["ID"] = item.Id;
                row["名前"] = item.Name;
                row["コメント"] = item.Comment;
                table.Rows.Add(row);
            }
            table.DefaultView.Sort = "ID ASC";
            return table;
        }

        private DataTable CreateUiTable(List<IdNameValuePriceComment> nameList)
        {
            var table = CreateUiTable(nameList.Select(item => item as IdNameValueComment).ToList());
            var column = new DataColumn("値段", typeof(long));
            table.Columns.Add(column);
            foreach (DataRow row in table.AsEnumerable())
            {
                row["値段"] = nameList.First(item => item.Id.ToString() == row["ID"].ToString()).Price;
            }
            table.DefaultView.Sort = "ID ASC";
            return table;
        }

        private void AppendRenameData(List<IdNameValueComment> nameList, string prefix)
        {
            if (!Prefixes.Contains(prefix))
                Prefixes.Add(prefix);
            foreach (var item in nameList)
            {
                var pattern = $"{prefix}:{/*sWhitespace.Replace(*/item.Name/*, "")*/}";
                var value = $"{item.Id}";
                if (renameDataTable.AsEnumerable().Where(row => row["置き換えパターン"].ToString() == pattern).Count() == 0)
                {
                    var row = renameDataTable.NewRow();
                    row["置き換えパターン"] = pattern;
                    row["置き換え値"] = value;
                    row["コメント"] = "自動追加";
                    renameDataTable.Rows.Add(row);
                }
                else
                {
                    foreach (DataRow row in renameDataTable.AsEnumerable().Where(row => row["置き換えパターン"].ToString() == pattern))
                    {
                        row["置き換え値"] = value;
                    }
                }
            }
        }

        protected ICommand generateOutputCommand = null;
        public ICommand GenerateOutputCommand
        {
            get
            {
                if (generateOutputCommand == null)
                {
                    generateOutputCommand = new RelayCommand((obj) => Loaded, (obj) =>
                    {
                        if (BackupOutput && Directory.Exists(OutErbExeDir))
                        {
                            var dirName = Path.GetFileName(OutErbExeDir);
                            var backupName = Path.Combine(OutErbExeDir, $"..\\{dirName}_Backup{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss")}");
                            foreach (string dirPath in Directory.GetDirectories(OutErbExeDir, "*", SearchOption.AllDirectories))
                            {
                                Directory.CreateDirectory(dirPath.Replace(OutErbExeDir, backupName));
                            }

                            //Copy all the files & Replaces any files with the same name
                            foreach (string newPath in Directory.GetFiles(OutErbExeDir, "*.*", SearchOption.AllDirectories))
                            {
                                File.Copy(newPath, newPath.Replace(OutErbExeDir, backupName), true);
                            }
                        }

                        逆引きCSV設定 = new Dictionary<string, string>();

                        WriteCsv(ablDataTable, "ABL.csv");
                        WriteCsv(expDataTable, "EXP.csv");
                        WriteCsv(talentDataTable, "TALENT.csv");
                        WriteCsv(trainDataTable, "TRAIN.csv");
                        WriteCsv(markDataTable, "MARK.csv");
                        WriteCsv(itemDataTable, "ITEM.csv", true);
                        WriteCsv(baseDataTable, "BASE.csv");
                        WriteCsv(equipDataTable, "EQUIP.csv");
                        WriteCsv(tequipDataTable, "TEQUIP.csv");
                        WriteCsv(flagDataTable, "FLAG.csv");
                        WriteCsv(tflagDataTable, "TFLAG.csv");
                        WriteCsv(cflagDataTable, "CFLAG.csv");
                        WriteCsv(tcvarDataTable, "TCVAR.csv");
                        WriteCsv(cstrDataTable, "CSTR.csv");
                        WriteCsv(tstrDataTable, "TSTR.csv");
                        WriteCsv(palamDataTable, "PALAM.csv");
                        WriteCsv(saveStrDataTable, "SAVESTR.csv");
                        WriteCsv(cdFlag1DataTable, "CDFLAG1.csv");
                        WriteCsv(cdFlag2DataTable, "CDFLAG2.csv");

                        string csvFileName;

                        #region STR.csv

                        if (strDataTable.Rows.Count > 0)
                        {

                            var addToReverseDic = Prefixes.Contains("STR");

                            csvFileName = Path.Combine(CsvDir, "STR.csv").Replace(ErbExeDir, OutErbExeDir);
                            var csvNameFileName = Path.Combine(CsvDir, "STRNAME.csv").Replace(ErbExeDir, OutErbExeDir);
                            using (var csvFile = File.Open(csvFileName, FileMode.Create))
                            {
                                using (var sw = new StreamWriter(csvFile, new UTF8Encoding(true)))
                                {
                                    using (var csvNameFile = File.Open(csvNameFileName, FileMode.Create))
                                    {
                                        using (var namesw = new StreamWriter(csvNameFile, new UTF8Encoding(true)))
                                        {
                                            strDataTable.DefaultView.Sort = "ID ASC";
                                            strDataTable = strDataTable.DefaultView.ToTable();
                                            foreach (DataRow row in strDataTable.Rows)
                                            {
                                                var strName = strNameDataTable.AsEnumerable().FirstOrDefault(nameRow => nameRow["ID"] == row["ID"])?["名前"].ToString();
                                                if (!string.IsNullOrWhiteSpace(strName))
                                                {
                                                    var strComment = strNameDataTable.AsEnumerable().FirstOrDefault(nameRow => nameRow["ID"] == row["ID"])?["コメント"].ToString();
                                                    namesw.WriteLine($"{row["ID"]},{strName},{strComment}");
                                                    if (addToReverseDic && !逆引きCSV設定.ContainsKey($"STR:{row["ID"]}"))
                                                    {
                                                        逆引きCSV設定[$"STR:{row["ID"]}"] = strName;
                                                    }
                                                }
                                                if (!string.IsNullOrWhiteSpace(row["文字列"].ToString()))
                                                {
                                                    sw.WriteLine($"{row["ID"]},{row["文字列"]},{row["コメント"]}");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        #endregion STR.csv

                        csvFileName = Path.Combine(CsvDir, "_Rename.csv").Replace(ErbExeDir, OutErbExeDir);
                        using (var csvFile = File.Open(csvFileName, FileMode.Create))
                        {
                            using (var sw = new StreamWriter(csvFile, new UTF8Encoding(true)))
                            {
                                foreach (DataRow row in renameDataTable.Rows)
                                {
                                    sw.WriteLine($"{row["置き換え値"]},{row["置き換えパターン"]},{row["コメント"]}");
                                    if (Prefixes.Contains(row["置き換えパターン"].ToString().Split(':')[0]))
                                    {
                                        var key = $"{row["置き換えパターン"].ToString().Split(':')[0]}:{row["置き換え値"]}";
                                        if (!逆引きCSV設定.ContainsKey(key))
                                            逆引きCSV設定[key] = $"[[{row["置き換えパターン"]}]]";
                                    }
                                }
                            }
                        }

                        var csvNoDigits = CharacterList.Max(chara => chara.EditNo).ToString().Length;
                        foreach (var chara in CharacterList)
                        {
                            csvFileName = Path.Combine(CsvDir, $"Chara{chara.EditNo.ToString($"D{csvNoDigits}")}_{/*sWhitespace.Replace(*/chara.EditName/*, "")*/}.csv").Replace(ErbExeDir, OutErbExeDir);
                            using (var csvFile = File.Open(csvFileName, FileMode.Create))
                            {
                                using (var sw = new StreamWriter(csvFile, new UTF8Encoding(true)))
                                {
                                    sw.WriteLine($"番号,{chara.EditNo}");
                                    sw.WriteLine($"名前,{chara.EditName}");
                                    if (!string.IsNullOrWhiteSpace(chara.EditCallName))
                                        sw.WriteLine($"呼び名,{chara.EditCallName}");
                                    if (!string.IsNullOrWhiteSpace(chara.EditNickName))
                                        sw.WriteLine($"あだ名,{chara.EditNickName}");

                                    WriteTable(chara.BaseDataView.ToTable(), baseDataTable, "基礎", sw);
                                    WriteTable(chara.AblDataView.ToTable(), ablDataTable, "能力", sw);
                                    WriteTable(chara.ExpDataView.ToTable(), expDataTable, "経験", sw);
                                    WriteTable(chara.TalentDataView.ToTable(), talentDataTable, "素質", sw);
                                    WriteTable(chara.MarkDataView.ToTable(), markDataTable, "刻印", sw);
                                    WriteTable(chara.EquipDataView.ToTable(), equipDataTable, "装着物", sw);
                                    WriteTable(chara.JuelDataView.ToTable(), palamDataTable, "珠", sw);
                                    WriteTable(chara.CFlagDataView.ToTable(), cflagDataTable, "フラグ", sw);
                                    WriteTable(chara.CStrDataView.ToTable(), cstrDataTable, "CSTR", sw);

                                    foreach (DataRow row in chara.RelationDataView.ToTable().Rows)
                                    {
                                        sw.WriteLine($"相性,{row["ID"]},{ row["値"]}");
                                    }

                                }
                            }
                        }

                        if (UpdateErb)
                        {
                            var outputEncoding = new UTF8Encoding(true);
                            foreach (var item in Directory.GetFiles(ErbExeDir, "*.erb", SearchOption.AllDirectories))
                            {
                                var erbContent = File.ReadAllText(item);
                                erbContent = ReplaceContent(erbContent);
                                File.WriteAllText(item.Replace(ErbExeDir, OutErbExeDir), erbContent, outputEncoding);
                            }
                        }

                    });
                }
                return generateOutputCommand;
            }
        }

        string intermittentTokens = "([\\w]+)";
        string intermittentParenthesisTokens = "(\\([^(R]*\\))";

        private string ReplaceContent(string erbContent)
        {
            foreach (var renameEntry in 逆引きCSV設定)
            {
                var keyParts = renameEntry.Key.Split(':');
                var value = renameEntry.Value;

                var fromPattern = $"{keyParts[0]}:{(ReverseErb ? value : keyParts[1])}";
                var toPattern = $"{keyParts[0]}:{(ReverseErb ? keyParts[1] : value)}";
                PerformErbReplace(ref erbContent, fromPattern, toPattern);

                fromPattern = $"{keyParts[0]}:{intermittentTokens}:{(ReverseErb ? value : keyParts[1])}";
                toPattern = $"{keyParts[0]}:$1:{(ReverseErb ? keyParts[1] : value)}";
                PerformErbReplace(ref erbContent, fromPattern, toPattern);

                fromPattern = $"{keyParts[0]}:{intermittentParenthesisTokens}:{(ReverseErb ? value : keyParts[1])}";
                toPattern = $"{keyParts[0]}:$1:{(ReverseErb ? keyParts[1] : value)}";
                PerformErbReplace(ref erbContent, fromPattern, toPattern);

                foreach (var convertPattern in Configuration.Get文字列変換設定.ERB変換設定.Where(item => item.種類 == keyParts[0]))
                {
                    if (!ReverseErb)
                    {
                        PerformErbReplace(ref erbContent,
                            convertPattern.変換前パターン
                            .Replace("{Id}", keyParts[1])
                            .Replace("{名前}", value)
                            .Replace("{かっこ以外のトークン}", intermittentTokens)
                            .Replace("{かっこ含むトークン}", intermittentParenthesisTokens),
                            convertPattern.変換後パターン
                            .Replace("{Id}", keyParts[1])
                            .Replace("{名前}", value)
                            .Replace("{かっこ以外のトークン}", "$1")
                            .Replace("{かっこ含むトークン}", "$1"));
                    }
                    else
                    {
                        PerformErbReplace(ref erbContent,
                            convertPattern.変換後パターン
                            .Replace("{Id}", keyParts[1])
                            .Replace("{名前}", value)
                            .Replace("{かっこ以外のトークン}", intermittentTokens)
                            .Replace("{かっこ含むトークン}", intermittentParenthesisTokens),
                            convertPattern.変換前パターン
                            .Replace("{Id}", keyParts[1])
                            .Replace("{名前}", value)
                            .Replace("{かっこ以外のトークン}", "$1")
                            .Replace("{かっこ含むトークン}", "$1"));
                    }
                }

                if (value.StartsWith("[["))
                {
                    fromPattern = $"{keyParts[0]}NAME:{keyParts[1]}";
                    toPattern = $"{keyParts[0]}NAME:{value}";
                    PerformErbReplace(ref erbContent, fromPattern, toPattern);

                    fromPattern = $"{keyParts[0]}NAME:{intermittentTokens}:{keyParts[1]}";
                    toPattern = $"{keyParts[0]}NAME:$1:{value}";
                    PerformErbReplace(ref erbContent, fromPattern, toPattern);

                    fromPattern = $"{keyParts[0]}NAME:{intermittentParenthesisTokens}:{keyParts[1]}";
                    toPattern = $"{keyParts[0]}NAME:$1:{value}";
                    PerformErbReplace(ref erbContent, fromPattern, toPattern);
                }

            }

            if (ReverseErb && ReverseWithRename)
            {
                foreach (DataRow row in renameDataTable.Rows)
                {
                    var fromPattern = $"[[{row["置き換えパターン"]}]]";
                    var toPattern = $"{row["置き換え値"]}";
                    erbContent = erbContent.Replace(fromPattern, toPattern);
                }
            }
            else if (!ReverseErb)
            {
                foreach (var renameEntry in 逆引きキャラID)
                {
                    foreach (var convertPattern in Configuration.Get文字列変換設定.ERB変換設定.Where(item => item.種類 == "NO"))
                    {
                        PerformErbReplace(ref erbContent,
                            convertPattern.変換前パターン
                            .Replace("{Id}", renameEntry.Key)
                            .Replace("{名前}", renameEntry.Value)
                            .Replace("{かっこ以外のトークン}", intermittentTokens)
                            .Replace("{かっこ含むトークン}", intermittentParenthesisTokens),
                            convertPattern.変換後パターン
                            .Replace("{Id}", renameEntry.Key)
                            .Replace("{名前}", renameEntry.Value)
                            .Replace("{かっこ以外のトークン}", "$1")
                            .Replace("{かっこ含むトークン}", "$1"));
                    }
                    //var fromPattern = $"NO:{intermittentTokens}([\\s]*)==([\\s]*){renameEntry.Key}";
                    //var toPattern = $"NO:$1 == {($"[[{renameEntry.Value}]]")}";
                    //PerformErbReplace(ref erbContent, fromPattern, toPattern);
                    //fromPattern = $"NO:{intermittentParenthesisTokens}([\\s]*)!=([\\s]*){renameEntry.Key}";
                    //toPattern = $"NO:$1 != {($"[[{renameEntry.Value}]]")}";
                    //PerformErbReplace(ref erbContent, fromPattern, toPattern);
                    //PerformErbReplace(ref erbContent, fromPattern, toPattern);
                    //fromPattern = $"GETCHARA\\({renameEntry.Key}";
                    //toPattern = $"GETCHARA({($"[[{renameEntry.Value}]]")}";
                    //PerformErbReplace(ref erbContent, fromPattern, toPattern);
                    //fromPattern = $"ADDCHARA([\\s*]){renameEntry.Key}";
                    //toPattern = $"ADDCHARA {($"[[{renameEntry.Value}]]")}";
                    //PerformErbReplace(ref erbContent, fromPattern, toPattern);
                    //fromPattern = $"DELCHARA([\\s*]){renameEntry.Key}";
                    //toPattern = $"DELCHARA {($"[[{renameEntry.Value}]]")}";
                    //PerformErbReplace(ref erbContent, fromPattern, toPattern);
                }
            }
            return erbContent;
        }

        private void PerformErbReplace(ref string erbContent, string fromPattern, string toPattern)
        {
            erbContent = Regex.Replace(erbContent, $"\\b{fromPattern.Replace("[[", "\\[\\[").Replace("]]", "\\]\\]")}\\b", toPattern);
        }

        private void WriteTable(DataTable charaData, DataTable idName, string prefix, StreamWriter sw)
        {
            foreach (DataRow row in charaData.Rows)
            {
                var id = row["ID"].ToString();
                var replacement = idName.AsEnumerable()
                .Where(tmprow => tmprow["ID"].ToString() == id)?
                .Select(tmprow => tmprow["名前"].ToString());
                if (replacement.Count() > 0)
                    id = replacement.First();

                sw.WriteLine($"{prefix},{id},{row["値"]},{row["コメント"]}");
            }
        }

        private void WriteCsv(DataTable table, string fileName, bool hasPrice = false)
        {
            if (table.Rows.Count == 0)
                return;

            var prefix = fileName.Split('.')[0];
            var addToReverseDic = Prefixes.Contains(prefix);

            var csvFileName = Path.Combine(CsvDir, fileName).Replace(ErbExeDir, OutErbExeDir);
            using (var csvFile = File.Open(csvFileName, FileMode.Create))
            {
                using (var sw = new StreamWriter(csvFile, new UTF8Encoding(true)))
                {
                    table.DefaultView.Sort = "ID ASC";
                    table = table.DefaultView.ToTable();
                    foreach (DataRow row in table.Rows)
                    {
                        if (hasPrice)
                        {
                            sw.WriteLine($"{row["ID"]},{row["名前"]},{row["値段"]},{row["コメント"]}");
                        }
                        else
                        {
                            sw.WriteLine($"{row["ID"]},{row["名前"]},{row["コメント"]}");
                        }
                        if (addToReverseDic &&
                            !(sWhitespace.IsMatch(row["名前"].ToString()) || sIrregular.IsMatch(row["名前"].ToString())) &&
                            !逆引きCSV設定.ContainsKey($"{prefix}:{row["ID"]}"))
                        {
                            逆引きCSV設定[$"{prefix}:{row["ID"]}"] = row["名前"].ToString();
                            if (prefix == "PALAM")
                                逆引きCSV設定[$"JUEL:{row["ID"]}"] = row["名前"].ToString();
                        }
                    }
                }
            }
        }
        public string SelectExeDir(string type)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fbd.Description = "フォルダを選択してください";
            fbd.RootFolder = Environment.SpecialFolder.Desktop;
            fbd.SelectedPath = ErbExeDir;
            fbd.ShowNewFolderButton = true;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                switch (type)
                {
                    case "root":
                        ErbExeDir = fbd.SelectedPath;
                        return ErbExeDir;
                    case "out":
                        OutErbExeDir = fbd.SelectedPath;
                        return OutErbExeDir;
                }
            }
            return "";
        }
    }
}
