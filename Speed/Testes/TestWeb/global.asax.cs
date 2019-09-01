using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Speed.Data;

namespace TestWeb
{
    public class global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            //Sys.ConnectionString = Database.BuildConnectionString(EnumDbProviderType.SqlServer, ".", "TesteRaizen", null, null, true).ConnectionString;
            Sys.ProviderType = EnumDbProviderType.SqlServer;
            Sys.ConnectionString = Database.BuildConnectionString(EnumDbProviderType.SqlServer, ".", "SpeedTests", null, null, true).ConnectionString;
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}