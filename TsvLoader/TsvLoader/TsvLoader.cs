using Microsoft.VisualBasic.FileIO;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace TsvLoader
{
    public class TsvLoader : IDisposable
    {
        private LoadParam param = null;

        public TsvLoader()
        {
            this.param = this.GetParamFromAppSetting();
        }

        public void Dispose()
        {

        }

        /// <summary>
        /// タブ区切りのテキストファイル（ヘッダ行あり）のロード処理。
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>ロード結果</returns>
        public DataTable Load()
        {
            if (!File.Exists(param.FilePath))
            {
                throw new ArgumentException("指定されたTSVファイルが見つかりません。");
            }

            using (TextFieldParser parser = new TextFieldParser(param.FilePath, Encoding.GetEncoding(param.FileEncode)))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters("\t");                    // タブ区切り(TSVファイルの場合)

                DataTable dataTable = null;
                if (param.HasTableName)
                {
                    dataTable = new DataTable(param.TableName);
                }
                else
                {
                    dataTable = new DataTable();
                }

                bool isHeader = true;
                while (parser.EndOfData == false)
                {
                    var columns = parser.ReadFields();
                    if (columns == null) continue;
                    if (isHeader)
                    {
                        //ヘッダ行
                        foreach (var column in columns)
                        {
                            dataTable.Columns.Add(new DataColumn(column));
                        }
                        isHeader = false;
                    }
                    else
                    {
                        var row = dataTable.NewRow();
                        row.ItemArray = columns.ToArray();
                        dataTable.Rows.Add(row);
                    }
                }
                dataTable.AcceptChanges();

                return dataTable;
            }
        }

        private LoadParam GetParamFromAppSetting()
        {
            LoadParam param = new LoadParam();

            param.FilePath = ConfigurationManager.AppSettings["filePath"].ToString();

            param.FileEncode = ConfigurationManager.AppSettings["fileEncode"].ToString();

            param.HasTableName = !ConfigurationManager.AppSettings["hasTableName"].ToString().Equals("0");

            param.TableName = ConfigurationManager.AppSettings["tableName"].ToString();

            return param;
        }

        public class LoadParam
        {
            public string FilePath = string.Empty;

            public string FileEncode = string.Empty;

            public bool HasTableName = false;

            public string TableName = string.Empty;
        }
    }
}
