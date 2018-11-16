using AiCard.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AiCard.ResultNotify
{
    public partial class PayResultNotifyPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PayResultNotify notify = new Bll.PayResultNotify(this.Page);
            notify.ProcessNotify();

        }
    }
}