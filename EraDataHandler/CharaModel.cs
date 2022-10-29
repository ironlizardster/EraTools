using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Created with Emuera Wiki as reference for parsing
/// https://osdn.net/projects/emuera/wiki/FrontPage
/// </summary>
namespace EraDataHandler
{
    public class CharaModel
    {
        public string Name { get; set; }

        private string editName;
        public string EditName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(editName))
                    editName = Name;
                return editName;
            }
            set
            {
                editName = value;
            }
        }

        public string Callname { get; set; }

        private string editCallName;
        public string EditCallName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(editCallName))
                    editCallName = Callname;
                return editCallName;
            }
            set
            {
                editCallName = value;
            }
        }

        public string Nickname { get; set; }

        private string editNickName;
        public string EditNickName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(editNickName))
                    editNickName = Nickname;
                return editNickName;
            }
            set
            {
                editNickName = value;
            }
        }

        public string Mastername { get; set; }

        private string editMasterName;
        public string EditMasterName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(editMasterName))
                    editMasterName = Mastername;
                return editMasterName;
            }
            set
            {
                editMasterName = value;
            }
        }

        public Int64 No { get; set; }

        private Int64 editNo = -1;
        public Int64 EditNo
        {
            get
            {
                if (editNo < 0)
                    editNo = No;
                return editNo;
            }
            set
            {
                editNo = value;
            }
        }

        public Int64 csvNo { get; set; }

        private Int64 editCsvNo = -1;
        public Int64 EditCsvNo
        {
            get
            {
                if (editCsvNo < 0)
                    editCsvNo = csvNo;
                return editCsvNo;
            }
            set
            {
                editCsvNo = value;
            }
        }

        public List<IdNameValueComment> Base;
        private DataTable baseDataTable;
        public DataView BaseDataView
        {
            get
            {
                if (baseDataTable == null)
                {
                    baseDataTable = CreateUiTable(Base, generalCsvModel.Base);
                }
                return baseDataTable.AsDataView();
            }
        }

        public List<IdNameValueComment> Mark;
        private DataTable markDataTable;
        public DataView MarkDataView
        {
            get
            {
                if (markDataTable == null)
                {
                    markDataTable = CreateUiTable(Mark, generalCsvModel.Mark);
                }
                return markDataTable.AsDataView();
            }
        }

        public List<IdNameValueComment> Exp;
        private DataTable expDataTable;
        public DataView ExpDataView
        {
            get
            {
                if (expDataTable == null)
                {
                    expDataTable = CreateUiTable(Exp, generalCsvModel.Exp);
                }
                return expDataTable.AsDataView();
            }
        }

        public List<IdNameValueComment> Abl;
        private DataTable ablDataTable;
        public DataView AblDataView
        {
            get
            {
                if (ablDataTable == null)
                {
                    ablDataTable = CreateUiTable(Abl, generalCsvModel.Abl);
                }
                return ablDataTable.AsDataView();
            }
        }

        public List<IdNameValueComment> Talent;
        private DataTable talentDataTable;
        public DataView TalentDataView
        {
            get
            {
                if (talentDataTable == null)
                {
                    talentDataTable = CreateUiTable(Talent, generalCsvModel.Talent);
                }
                return talentDataTable.AsDataView();
            }
        }

        public List<IdNameValueComment> Relation;
        private DataTable relationDataTable;
        public DataView RelationDataView
        {
            get
            {
                if (relationDataTable == null)
                {
                    relationDataTable = new DataTable();
                    var column = new DataColumn("ID", typeof(Int32));
                    column.AutoIncrement = true;
                    column.AutoIncrementSeed = 1;
                    column.AutoIncrementStep = 1;
                    relationDataTable.Columns.Add(column);
                    relationDataTable.PrimaryKey = new DataColumn[] { relationDataTable.Columns["ID"] };
                    column = new DataColumn("名前");
                    column.ReadOnly = true;
                    relationDataTable.Columns.Add(column);
                    column = new DataColumn("値", typeof(Int64));
                    relationDataTable.Columns.Add(column);
                    column = new DataColumn("コメント");
                    relationDataTable.Columns.Add(column);
                    foreach (var item in Relation)
                    {
                        var row = relationDataTable.NewRow();
                        var relationChar = generalCsvModel.Charas.FirstOrDefault(chara => chara.No == item.Id);

                        row["ID"] = item.Id;
                        row["名前"] = relationChar != null ? relationChar.Name : item.Id.ToString();
                        row["値"] = item.Value;
                        row["コメント"] = item.Comment;
                        relationDataTable.Rows.Add(row);
                    }
                    relationDataTable.RowChanged += (obj, arg) =>
                    {
                        var row = arg.Row;
                        if ((Int32)row["ID"] > -1)
                        {
                            try
                            {
                                var relationChar = generalCsvModel.Charas.First(chara => chara.No == (Int32)row["ID"]);

                                if (relationChar != null && row["名前"].ToString() != relationChar.Name)
                                {
                                    row.Table.Columns["名前"].ReadOnly = false;
                                    row["名前"] = relationChar.Name;
                                    row.Table.Columns["名前"].ReadOnly = true;
                                    NotifyAllPropertyChanged();
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    };
                }
                relationDataTable.DefaultView.Sort = "ID ASC";
                return relationDataTable.AsDataView();
            }
        }

        public List<IdNameValueComment> CFlag;
        private DataTable cFlagDataTable;
        public DataView CFlagDataView
        {
            get
            {
                if (cFlagDataTable == null)
                {
                    cFlagDataTable = CreateUiTable(CFlag, generalCsvModel.CFlag);
                }
                return cFlagDataTable.AsDataView();
            }
        }

        public List<IdNameValueComment> Equip;
        private DataTable equipDataTable;
        public DataView EquipDataView
        {
            get
            {
                if (equipDataTable == null)
                {
                    equipDataTable = CreateUiTable(Equip, generalCsvModel.Equip);
                }
                return equipDataTable.AsDataView();
            }
        }

        private List<IdNameValueComment> Juel;
        private DataTable juelDataTable;
        public DataView JuelDataView
        {
            get
            {
                if (juelDataTable == null)
                {
                    juelDataTable = CreateUiTable(Juel, generalCsvModel.Param);
                }
                return juelDataTable.AsDataView();
            }
        }

        private List<IdNameValueComment> CStr;
        private DataTable cStrDataTable;
        public DataView CStrDataView
        {
            get
            {
                if (cStrDataTable == null)
                {
                    cStrDataTable = CreateUiTable(CStr, generalCsvModel.CStr);
                }
                return cStrDataTable.AsDataView();
            }
        }

        private GeneralCsvModel generalCsvModel;
        private string fileName;

        public CharaModel(GeneralCsvModel generalCsvModel, string fileName)
        {
            this.generalCsvModel = generalCsvModel;
            this.fileName = fileName;
            Base = new List<IdNameValueComment>();
            Mark = new List<IdNameValueComment>();
            Exp = new List<IdNameValueComment>();
            Abl = new List<IdNameValueComment>();
            Talent = new List<IdNameValueComment>();
            Relation = new List<IdNameValueComment>();
            CFlag = new List<IdNameValueComment>();
            Equip = new List<IdNameValueComment>();
            Juel = new List<IdNameValueComment>();
            CStr = new List<IdNameValueComment>();
        }

        public void ParseLine(string line)
        {
            Int64 index = -1;
            Int32 paramIndex = -1;
            //string[] tokens = line.Split(';');
            //var comment = tokens.Length > 1 ? tokens[1] : String.Empty;
            //tokens = tokens[0].Split(',');
            string[] tokens = line.Split(',');
            // Return if comment line
            if (tokens[0].StartsWith(";"))
                return;
            var comment = string.Empty;
            if ((tokens[0].Equals("NO", StringComparison.OrdinalIgnoreCase)) || (tokens[0].Equals("番号", StringComparison.OrdinalIgnoreCase)))
            {

                if (!Int64.TryParse(tokens[1].TrimEnd(), out index))
                {
                    return;
                }
                No = index;
                var no = fileName.ToLowerInvariant().Replace(".csv", "").Replace("chara", "");
                if (Int64.TryParse(no, out index))
                {
                    csvNo = index;
                }
                else
                {
                    csvNo = 0;
                }
                return;
            }
            else
            {
                string varname = tokens[0].ToUpper();
                var value = tokens.Length > 2 && !tokens[2].StartsWith(";") ? tokens[2] : String.Empty;
                if (string.IsNullOrWhiteSpace(comment))
                {
                    comment = tokens.Length > 2 && tokens[2].StartsWith(";")
                        || tokens.Length > 3 && tokens[3].StartsWith(";")
                        ? (tokens.Length > 2 && tokens[2].StartsWith(";") ? tokens[2] : tokens[3])
                        : String.Empty;
                }
                switch (varname)
                {
                    case "NAME":
                    case "名前":
                        Name = tokens[1];
                        return;
                    case "CALLNAME":
                    case "呼び名":
                        Callname = tokens[1];
                        return;
                    case "NICKNAME":
                    case "あだ名":
                        Nickname = tokens[1];
                        return;
                    case "MASTERNAME":
                    case "主人の呼び方":
                        Mastername = tokens[1];
                        return;
                    case "MARK":
                    case "刻印":
                        Mark.Add(new IdNameValueComment()
                        {
                            Id = Int32.TryParse(tokens[1], out paramIndex) ? paramIndex :
                            generalCsvModel.Mark.First(idNameValue => idNameValue.Id.ToString() == tokens[1] || idNameValue.Name == tokens[1]).Id,
                            Value = value,
                            Comment = comment
                        });
                        break;
                    case "EXP":
                    case "経験":
                        Exp.Add(new IdNameValueComment()
                        {
                            Id = Int32.TryParse(tokens[1], out paramIndex) ? paramIndex :
                            generalCsvModel.Exp.First(idNameValue => idNameValue.Id.ToString() == tokens[1] || idNameValue.Name == tokens[1]).Id,
                            Value = value,
                            Comment = comment
                        });
                        break;
                    case "ABL":
                    case "能力":
                        Abl.Add(new IdNameValueComment()
                        {
                            Id = Int32.TryParse(tokens[1], out paramIndex) ? paramIndex :
                            generalCsvModel.Abl.First(idNameValue => idNameValue.Id.ToString() == tokens[1] || idNameValue.Name == tokens[1]).Id,
                            Value = value,
                            Comment = comment
                        });
                        break;
                    case "BASE":
                    case "基礎":
                        Base.Add(new IdNameValueComment()
                        {
                            Id = Int32.TryParse(tokens[1], out paramIndex) ? paramIndex :
                            generalCsvModel.Base.First(idNameValue => idNameValue.Id.ToString() == tokens[1] || idNameValue.Name == tokens[1]).Id,
                            Value = value,
                            Comment = comment
                        });
                        break;
                    case "TALENT":
                    case "素質":
                        Talent.Add(new IdNameValueComment()
                        {
                            Id = Int32.TryParse(tokens[1], out paramIndex) ? paramIndex :
                            generalCsvModel.Talent.First(idNameValue => idNameValue.Id.ToString() == tokens[1] || idNameValue.Name == tokens[1]).Id,
                            Value = value,
                            Comment = comment
                        });
                        break;
                    case "RELATION":
                    case "相性":
                        Relation.Add(new IdNameValueComment()
                        {
                            Id = Int32.TryParse(tokens[1], out paramIndex) ? paramIndex : -1,
                            Value = value,
                            Comment = comment
                        });
                        break;
                    case "CFLAG":
                    case "フラグ":
                        CFlag.Add(new IdNameValueComment()
                        {
                            Id = Int32.TryParse(tokens[1], out paramIndex) ? paramIndex :
                            generalCsvModel.CFlag.First(idNameValue => idNameValue.Id.ToString() == tokens[1] || idNameValue.Name == tokens[1]).Id,
                            Value = value,
                            Comment = comment
                        });
                        break;
                    case "EQUIP":
                    case "装着物":
                        Equip.Add(new IdNameValueComment()
                        {
                            Id = Int32.TryParse(tokens[1], out paramIndex) ? paramIndex :
                            generalCsvModel.Equip.First(idNameValue => idNameValue.Id.ToString() == tokens[1] || idNameValue.Name == tokens[1]).Id,
                            Value = value,
                            Comment = comment
                        });
                        break;
                    case "JUEL":
                    case "珠":
                        Juel.Add(new IdNameValueComment()
                        {
                            Id = Int32.TryParse(tokens[1], out paramIndex) ? paramIndex :
                            generalCsvModel.Param.First(idNameValue => idNameValue.Id.ToString() == tokens[1] || idNameValue.Name == tokens[1]).Id,
                            Value = value,
                            Comment = comment
                        });
                        break;
                    case "CSTR":
                        CStr.Add(new IdNameValueComment()
                        {
                            Id = Int32.TryParse(tokens[1], out paramIndex) ? paramIndex :
                            generalCsvModel.CStr.First(idNameValue => idNameValue.Id.ToString() == tokens[1] || idNameValue.Name == tokens[1]).Id,
                            Value = value,
                            Comment = comment
                        });
                        break;
                    default:
                        return;
                }
            }
        }

        public void Reload()
        {
            editNo = editCsvNo = -1;
            editName = editCallName = editNickName = editMasterName = string.Empty;
            baseDataTable = markDataTable = expDataTable = ablDataTable =
                talentDataTable = relationDataTable = cFlagDataTable =
                equipDataTable = juelDataTable = cStrDataTable = null;
            NotifyAllPropertyChanged();
        }

        protected DataTable CreateUiTable(List<IdNameValueComment> valueList, List<IdNameValueComment> nameList)
        {
            var table = new DataTable();
            var column = new DataColumn("ID", typeof(Int32));
            table.Columns.Add(column);
            table.PrimaryKey = new DataColumn[] { table.Columns["ID"] };
            column = new DataColumn("名前");
            table.Columns.Add(column);
            column = new DataColumn("値");
            table.Columns.Add(column);
            column = new DataColumn("コメント");
            table.Columns.Add(column);
            foreach (var item in valueList)
            {
                var row = table.NewRow();
                row["ID"] = item.Id;
                row["名前"] = nameList.FirstOrDefault(idNameValue => idNameValue.Id == item.Id)?.Name;
                row["値"] = item.Value;
                row["コメント"] = item.Comment;
                table.Rows.Add(row);
            }
            table.RowChanged += (obj, arg) =>
            {
                var row = arg.Row;
                try
                {
                    if ((Int32)row["ID"] > -1 && row["名前"].ToString() != nameList.First(idNameValue => idNameValue.Id == (Int32)row["ID"]).Name)
                    {
                        row["名前"] = nameList.First(idNameValue => idNameValue.Id == (Int32)row["ID"]).Name;
                        NotifyAllPropertyChanged();
                    }
                }
                catch (Exception ex)
                {

                }
            };

            return table;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void NotifyAllPropertyChanged()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
            }
        }

        #endregion INotifyPropertyChanged

    }
}
