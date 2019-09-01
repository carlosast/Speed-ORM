using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Speed.Data.MetaData;

namespace Speed.Data.Generation
{

#if !DEBUG
    [System.Diagnostics.DebuggerStepThrough]
#endif
    /// <summary>
    /// Resultado da geração de código
    /// </summary>
    public class GenTableResult
    {

        public DbTableInfo Table { get; set; }
        public GenTable GenTable { get; set; }
        public string EntityName { get; set; }
        public string EntityClass { get; set; }
        public string EntityClassExt { get; set; }

        public string EntityFileName { get; set; }
        public EnumFileChanged EntityFileNameChanged { get; set; }

        public string EntityFileNameExt { get; set; }
        public EnumFileChanged EntityFileNameExtChanged { get; set; }

        public string BusinnesClass { get; set; }
        public string BusinnesClassExt { get; set; }
        public string BusinnesName { get; set; }
        public string BusinnesFileName { get; set; }
        public EnumFileChanged BusinnesFileNameChanged { get; set; }

        public string BusinnesFileNameExt { get; set; }
        public EnumFileChanged BusinnesFileNameExtChanged { get; set; }

        public string Enum { get; set; }
        public string EnumName { get; set; }
        public string EnumFileName { get; set; }
        public EnumFileChanged EnumFileNameChanged { get; set; }
        public List<string> ParentRelations = new List<string>();
        public List<string> ChildRelations = new List<string>();
        public EnumFileChanged FileChanged { get; set; }

        public override string ToString()
        {
            return Table.TableName;
        }

    }

}
