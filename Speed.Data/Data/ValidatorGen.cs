using System.Text;
using Speed.Data.MetaData;
using Speed.Common;

namespace Speed.Data
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    public static class ValidatorGen
    {

        #region Templates

        // ------ templateClass ------
        static string templateClass =
@"using System;
using System.Collections.Generic;
[NameSpaces]using FluentValidation;

namespace [NameSpace]
{
    public partial class [ClassName] : AbstractValidator<[TypeName]>
    {

        public [ClassName]()
        {

[Columns]
            Config();
        }

    }

}";

        // ------ templateClassExt ------
        static string templateClassExt =
@"using System;
using System.Collections.Generic;
[NameSpaces]using FluentValidation;

namespace [NameSpace]
{
    public partial class [ClassName]
    {

        public void Config()
        {
        }

    }

}";

        // ------ fieldTemplate ------
        static string fieldTemplate = @"            RuleFor({0} => {0}.{1})";

        #endregion Templates

        public static GenVal Generate(DbTableInfo info, string nameSpace, string className, string typeName, string addNameSpaces)
        {
            StringBuilder b = new StringBuilder();
            StringBuilder dicNames = new StringBuilder();

            foreach (var pair in info.Columns)
            {
                var col = pair.Value;

                string type = col.DataType;
                string colName = col.ColumnName;
                string template = string.Format(fieldTemplate, "p", colName);
                string colText = template;

                dicNames.AppendFormat(string.Format("            DicNames.Add(\"{0}\", \"{1}\");\r\n", typeName, colName));
                //colText += RuleWithName(colName);

                bool isNotNull = !col.IsNullable && !col.IsIdentity;

                if (isNotNull)
                    colText += RuleNotNull();

                if (type.In("int", "money", "float", "real"))
                {
                }
                else if (type.In("varchar", "nvarchar", "char", "nchar", "text", "ntext"))
                {
                    if (col.CharacterMaximumLength != -1)
                        colText += RuleLength(col.CharacterMaximumLength, isNotNull);

                    if (colName.ToLower().Contains("mail"))
                        colText += RuleEmailAddress();
                }
                else if (type.In("date", "datetime", "time"))
                {
                }

                // name
                if (colText != template)
                    colText += RuleWithName(colName);
                // colText += RuleWithName(typeName + "." + colName);

                colText += ";";
                b.AppendLine(colText);
            }

            string result = templateClass;
            result = result.Replace("[NameSpace]", nameSpace);
            if (Conv.HasData(addNameSpaces))
                result = result.Replace("[NameSpaces]", addNameSpaces + "\r\n");
            else
                result = result.Replace("[NameSpaces]", "");
            result = result.Replace("[ClassName]", className);
            result = result.Replace("[TypeName]", typeName);
            result = result.Replace("[Columns]", b.ToString());
            result = result.Replace("[DicNames]", dicNames.ToString());
            return new GenVal{ Validator = result.ToString(), ValidatorExt = GenerateExt(info, nameSpace, className, typeName, addNameSpaces) };
        }

        public static string GenerateExt(DbTableInfo info, string nameSpace, string className, string typeName, string addNameSpaces)
        {
            StringBuilder b = new StringBuilder();
            StringBuilder dicNames = new StringBuilder();

            foreach (var pair in info.Columns)
            {
                var col = pair.Value;

                string type = col.DataType;
                string colName = col.ColumnName;
                string template = string.Format(fieldTemplate, "p", colName);
                string colText = template;

                colText += RuleWithName(colName);
                colText += ";";
                b.AppendLine(colText);
            }

            string result = templateClassExt;
            result = result.Replace("[NameSpace]", nameSpace);
            if (Conv.HasData(addNameSpaces))
                result = result.Replace("[NameSpaces]", addNameSpaces + "\r\n");
            else
                result = result.Replace("[NameSpaces]", "");
            result = result.Replace("[ClassName]", className);
            result = result.Replace("[TypeName]", typeName);
            result = result.Replace("[Columns]", b.ToString());
            result = result.Replace("[DicNames]", dicNames.ToString());
            return result.ToString();
        }

        public static string RuleNotNull()
        {
            return ".NotNull()";
        }

        public static string RuleEmailAddress()
        {
            return ".EmailAddress()";
        }

        public static string RuleLength(long length, bool required)
        {
            return string.Format(".Length({0}, {1})", required ? 0 : 0, length);
        }

        public static string RuleWithName(string columnName)
        {
            return string.Format(".WithName(\"{0}\")", columnName);
        }

    }

    public class GenVal
    {
        public string Validator { get; set; }
        public string ValidatorExt { get; set; }

        public GenVal()
        {
        }

    }

}
