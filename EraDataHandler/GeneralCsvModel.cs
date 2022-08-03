using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EraDataHandler
{
    public class IdNameValueComment
    {
        public Int32 Id;
        public string Name;
        public string Value;
        public string Comment;
    }

    public class IdNameValuePriceComment : IdNameValueComment
    {
        public Int32 Price;
    }

    public class GeneralCsvModel
    {
        #region Const

        public const string ColumnNameId = "Id";
        public const string ColumnNameComment = "Comment";
        public const string ColumnNameName = "Name";
        public const string ColumnNameValue = "Value";
        public const string ColumnNamePrice = "Price";

        #endregion

        #region DataConverter

        protected List<IdNameValueComment> ToListIdNameComment(DataTable source)
        {
            var id = 0;
            var result = new List<IdNameValueComment>();
            if (source != null && source.Rows.Count > 0)
            {
                result.AddRange(source.AsEnumerable().Select(row => new IdNameValueComment()
                {
                    Id = Int32.TryParse(row[ColumnNameId].ToString(), out id) ? id : 0,
                    Name = row[ColumnNameName].ToString(),
                    Value = row[ColumnNameValue]?.ToString(),
                    Comment = row[ColumnNameComment]?.ToString()
                }));
            }
            return result;
        }

        protected List<IdNameValuePriceComment> ToListIdNamePriceComment(DataTable source)
        {
            var result = new List<IdNameValuePriceComment>();
            if (source != null && source.Rows.Count > 0)
            {
                result.AddRange(source.AsEnumerable().Select(row => new IdNameValuePriceComment()
                {
                    Id = Int32.Parse(row[ColumnNameId].ToString()),
                    Name = row[ColumnNameName].ToString(),
                    Value = row[ColumnNameValue]?.ToString(),
                    Price = Int32.Parse(row[ColumnNamePrice]?.ToString()),
                    Comment = row[ColumnNameComment]?.ToString()
                }));
            }
            return result;
        }

        #endregion

        #region Data Structure

        protected DataTable rename = null;
        public List<IdNameValueComment> Rename => ToListIdNameComment(rename);

        protected DataTable abl = null;
        public List<IdNameValueComment> Abl => ToListIdNameComment(abl);

        protected DataTable exp = null;
        public List<IdNameValueComment> Exp => ToListIdNameComment(exp);

        protected DataTable ex = null;
        public List<IdNameValueComment> Ex => ToListIdNameComment(ex);

        protected DataTable talent = null;
        public List<IdNameValueComment> Talent => ToListIdNameComment(talent);

        protected DataTable train = null;
        public List<IdNameValueComment> Train => ToListIdNameComment(train);

        protected DataTable mark = null;
        public List<IdNameValueComment> Mark => ToListIdNameComment(mark);

        protected DataTable item = null;
        public List<IdNameValuePriceComment> Item => ToListIdNamePriceComment(item);

        protected DataTable bases = null;
        public List<IdNameValueComment> Base => ToListIdNameComment(bases);

        protected DataTable str = null;
        public List<IdNameValueComment> Str => ToListIdNameComment(str);

        protected DataTable tStr = null;
        public List<IdNameValueComment> TStr => ToListIdNameComment(tStr);

        protected DataTable strName = null;
        public List<IdNameValueComment> StrName => ToListIdNameComment(strName);

        protected DataTable cStr = null;
        public List<IdNameValueComment> CStr => ToListIdNameComment(cStr);

        protected DataTable equip = null;
        public List<IdNameValueComment> Equip => ToListIdNameComment(equip);

        protected DataTable tEquip = null;
        public List<IdNameValueComment> TEquip => ToListIdNameComment(tEquip);

        protected DataTable param = null;
        public List<IdNameValueComment> Param => ToListIdNameComment(param);

        protected DataTable source = null;
        public List<IdNameValueComment> Source => ToListIdNameComment(source);

        protected DataTable stain = null;
        public List<IdNameValueComment> Stain => ToListIdNameComment(stain);

        protected DataTable flag = null;
        public List<IdNameValueComment> Flag => ToListIdNameComment(flag);

        protected DataTable tFlag = null;
        public List<IdNameValueComment> TFlag => ToListIdNameComment(tFlag);

        protected DataTable cFlag = null;
        public List<IdNameValueComment> CFlag => ToListIdNameComment(cFlag);

        protected DataTable cdFlag1 = null;
        public List<IdNameValueComment> CDFlag1 => ToListIdNameComment(cdFlag1);

        protected DataTable cdFlag2 = null;
        public List<IdNameValueComment> CDFlag2 => ToListIdNameComment(cdFlag2);

        protected DataTable tCVar = null;
        public List<IdNameValueComment> TCVar => ToListIdNameComment(tCVar);

        protected DataTable saveStr = null;
        public List<IdNameValueComment> SaveStr => ToListIdNameComment(saveStr);

        protected DataTable global = null;
        public List<IdNameValueComment> Global => ToListIdNameComment(global);

        protected DataTable globals = null;
        public List<IdNameValueComment> Globals => ToListIdNameComment(globals);

        public List<CharaModel> Charas = null;

        #endregion

        public GeneralCsvModel(string eraCsvPath)
        {
            rename = LoadNameValueCommentFile(eraCsvPath + "_Rename.csv");
            abl = LoadIdNameCommentFile(eraCsvPath + "ABL.CSV");
            exp = LoadIdNameCommentFile(eraCsvPath + "EXP.CSV");
            talent = LoadIdNameCommentFile(eraCsvPath + "TALENT.CSV");
            param = LoadIdNameCommentFile(eraCsvPath + "PALAM.CSV");
            train = LoadIdNameCommentFile(eraCsvPath + "TRAIN.CSV");
            mark = LoadIdNameCommentFile(eraCsvPath + "MARK.CSV");
            item = LoadIdNamePriceCommentFile(eraCsvPath + "ITEM.CSV");
            bases = LoadIdNameCommentFile(eraCsvPath + "BASE.CSV");
            source = LoadIdNameCommentFile(eraCsvPath + "SOURCE.CSV");
            ex = LoadIdNameCommentFile(eraCsvPath + "EX.CSV");
            str = LoadIdNameCommentFile(eraCsvPath + "STR.CSV");
            equip = LoadIdNameCommentFile(eraCsvPath + "EQUIP.CSV");
            tEquip = LoadIdNameCommentFile(eraCsvPath + "TEQUIP.CSV");
            flag = LoadIdNameCommentFile(eraCsvPath + "FLAG.CSV");
            tFlag = LoadIdNameCommentFile(eraCsvPath + "TFLAG.CSV");
            cFlag = LoadIdNameCommentFile(eraCsvPath + "CFLAG.CSV");
            tCVar = LoadIdNameCommentFile(eraCsvPath + "TCVAR.CSV");
            cStr = LoadIdNameCommentFile(eraCsvPath + "CSTR.CSV");
            stain = LoadIdNameCommentFile(eraCsvPath + "STAIN.CSV");
            cdFlag1 = LoadIdNameCommentFile(eraCsvPath + "CDFLAG1.CSV");
            cdFlag2 = LoadIdNameCommentFile(eraCsvPath + "CDFLAG2.CSV");

            strName = LoadIdNameCommentFile(eraCsvPath + "STRNAME.CSV");
            tStr = LoadIdNameCommentFile(eraCsvPath + "TSTR.CSV");
            saveStr = LoadIdNameCommentFile(eraCsvPath + "SAVESTR.CSV");
            global = LoadIdNameCommentFile(eraCsvPath + "GLOBAL.CSV");
            globals = LoadIdNameCommentFile(eraCsvPath + "GLOBALS.CSV");

            Charas = LoadCharacterData(eraCsvPath);

        }

        protected DataTable LoadNameValueCommentFile(string path)
        {
            var result = new DataTable();
            result.Columns.Add(new DataColumn(ColumnNameId));
            result.Columns.Add(new DataColumn(ColumnNameName));
            result.Columns.Add(new DataColumn(ColumnNameValue));
            result.Columns.Add(new DataColumn(ColumnNameComment));

            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var reader = new StreamReader(stream, Encoding.GetEncoding("SHIFT-JIS")))
                    {
                        string line = null;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (line.Length == 0)
                                continue;
                            if (line.StartsWith(";"))
                                continue;
                            string[] tokens = line.Split(',');

                            var newRow = result.NewRow();
                            newRow[ColumnNameValue] = tokens[0];
                            newRow[ColumnNameName] = tokens[1];
                            if (tokens.Length > 2)
                                newRow[ColumnNameComment] = tokens[2];
                            result.Rows.Add(newRow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        protected DataTable LoadIdNameCommentFile(string path)
        {
            var result = new DataTable();
            result.Columns.Add(new DataColumn(ColumnNameId));
            result.Columns.Add(new DataColumn(ColumnNameName));
            result.Columns.Add(new DataColumn(ColumnNameValue));
            result.Columns.Add(new DataColumn(ColumnNameComment));

            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var reader = new StreamReader(stream, Encoding.GetEncoding("SHIFT-JIS")))
                    {
                        string line = null;
                        while ((line = reader.ReadLine()) != null)
                        {
                            line = line.Trim();
                            if (line.Length == 0)
                                continue;
                            if (line.StartsWith(";"))
                                continue;
                            string[] tokens = line.Split(',');

                            var newRow = result.NewRow();
                            newRow[ColumnNameId] = tokens[0];
                            newRow[ColumnNameName] = tokens[1];
                            if (tokens.Length > 2)
                                newRow[ColumnNameComment] = tokens[2];
                            result.Rows.Add(newRow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        protected DataTable LoadIdNamePriceCommentFile(string path)
        {
            var result = new DataTable();
            result.Columns.Add(new DataColumn(ColumnNameId));
            result.Columns.Add(new DataColumn(ColumnNameName));
            result.Columns.Add(new DataColumn(ColumnNameValue));
            result.Columns.Add(new DataColumn(ColumnNamePrice));
            result.Columns.Add(new DataColumn(ColumnNameComment));

            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var reader = new StreamReader(stream, Encoding.GetEncoding("SHIFT-JIS")))
                    {
                        string line = null;
                        while ((line = reader.ReadLine()) != null)
                        {
                            line = line.Trim();
                            if (line.Length == 0)
                                continue;
                            if (line.StartsWith(";"))
                                continue;
                            string[] tokens = line.Split(',');

                            var newRow = result.NewRow();
                            newRow[ColumnNameId] = tokens[0];
                            newRow[ColumnNameName] = tokens[1];
                            if (tokens.Length > 2)
                                newRow[ColumnNamePrice] = tokens[2];
                            else
                                newRow[ColumnNamePrice] = 0;
                            if (tokens.Length > 3)
                                newRow[ColumnNameComment] = tokens[3];
                            result.Rows.Add(newRow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        protected List<CharaModel> LoadCharacterData(string csvDir)
        {
            var result = new List<CharaModel>();
            if (!Directory.Exists(csvDir))
                return result;
            var charFiles = Directory.GetFiles(csvDir, "CHARA*.CSV", SearchOption.AllDirectories);
            foreach (var charFile in charFiles)
            {
                var chara = new CharaModel(this, charFile);
                try
                {
                    using (var stream = new FileStream(charFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (var reader = new StreamReader(stream, Encoding.GetEncoding("SHIFT-JIS")))
                        {
                            string line = null;
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (line.Length == 0)
                                    continue;
                                if (line.StartsWith(";"))
                                    continue;
                                string[] tokens = line.Split(',');
                                if (tokens.Length < 2 || tokens[0].Length == 0)
                                {
                                    continue;
                                }
                                try
                                {
                                    chara.ParseLine(line);
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                        }
                    }
                    result.Add(chara);
                }
                catch (Exception ex)
                {

                }

            }

            return result;
        }

    }
}
