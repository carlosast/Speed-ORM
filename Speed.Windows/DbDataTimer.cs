using System;
using Speed.Data;

namespace Speed.Windows
{

    public class DbDataTimer<T> : Speed.DataTimer<T>
    {


        public DbDataTimer(int milliseconds, Func<Database, T> funcReturnValue) :
            base(milliseconds, () =>
         {
             using (var db = ProgramBase.NewDb())
                 return funcReturnValue(db);
         })
        {
        }

    }

    //ConcurrentDictionary<short, Data.VwRepository> t = new ConcurrentDictionary<short, VwRepository>();
    //repositories = new Speed.DataTimer<ConcurrentDictionary<short, VwRepository>>(1000 * 60, 
    //    () => t);
    //repositories = new Speed.DataTimer<ConcurrentDictionary<short, VwRepository>>(1000 * 60, 
    //    () => ProgramBase.RunInDb((db) => new ConcurrentDictionary<short, Data.VwRepository>( BL.VwRepositories.Select(db).ToDictionary(p => p.RepositoryId))));


}
