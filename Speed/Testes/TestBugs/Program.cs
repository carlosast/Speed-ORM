using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speed.Data;

namespace TestBugs
{

    class Program
    {

        static void Main(string[] args)
        {
            using (var db = new Database(EnumDbProviderType.SqlServer, ".", "TesteRaizen", "sa", "manager"))
            {
                db.Open();

                db.GenerateTables("TesteRaizen.Data", "TesteRaizen.BL", "../../Data",  "../../BL", false, Speed.Data.Generation.EnumNameCase.None);
            }
        }

    }

}
