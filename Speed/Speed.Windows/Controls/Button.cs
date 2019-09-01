using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Speed.Windows.Controls
{
    
    /// <summary>
    /// Button com try catch no evento click
    /// </summary>
    public class Button : System.Windows.Forms.Button
    {

        public Button()
        {
        }

        protected override void OnClick(EventArgs e)
        {
            ProgramBase.RunSafe(this, () => base.OnClick(e));
        }

    }

}
