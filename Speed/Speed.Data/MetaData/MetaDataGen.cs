using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Speed.Data.MetaData;

namespace Speed.Data
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public static class MetaDataGen
    {

        #region Templates

        // ------ templateClass ------
        static string templateClass =
@"using System;
using System.Collections.Generic;
[NameSpaces]using System.ComponentModel.DataAnnotations;
using System.ComponentModel;


namespace [NameSpace]
{

    [MetadataType(typeof([MdClassName]))]
    public partial class [ClassName]
    {
    }

    public class [MdClassName]
    {

[Columns]
    }

}";

        #endregion Templates

        public static string Generate(DbTableInfo info, string nameSpace, string className, string mdClassName, string addNameSpaces)
        {
            StringBuilder b = new StringBuilder();

            foreach (var pair in info.Columns)
            {
                var col = pair.Value;
                List<string> atts = new List<string>();

                string type = col.DataType;
                string colName = col.ColumnName;

                bool isNotNull = !col.IsNullable && !col.IsIdentity;
                if (isNotNull)
                    atts.Add("[Required]");

                if (type.In("int", "money", "float", "real"))
                {
                }
                else if (type.In("varchar", "nvarchar", "char", "nchar", "text", "ntext"))
                {
                    if (col.CharacterMaximumLength != -1)
                        atts.Add(string.Format("[StringLength({0})]", col.CharacterMaximumLength));

                    if (colName.ToLower().Contains("mail"))
                        atts.Add("[Email]");
                }

                if (atts.Count > 0)
                    foreach (var att in atts)
                        b.AppendLine("\t\t" + att);

                b.AppendLine("\t\t" + "public object " + colName + " { get; set; }");
                b.AppendLine();
            }

            string result = templateClass;
            result = result.Replace("[NameSpace]", nameSpace);
            if (Conv.HasData(addNameSpaces))
                result = result.Replace("[NameSpaces]", addNameSpaces + "\r\n");
            else
                result = result.Replace("[NameSpaces]", "");
            result = result.Replace("[ClassName]", className);
            result = result.Replace("[MdClassName]", mdClassName);
            result = result.Replace("[Columns]", b.ToString());
            return result.ToString();
        }

    }

}
