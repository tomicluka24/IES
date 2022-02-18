using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMSTestClientWPF.Model
{
    public class Item
    {
        public ModelCode Code { get; set; }
        public bool Checked { get; set; }

        public Item(ModelCode code, bool check = false)
        {
            Code = code;
            Checked = check;
        }
    }
}
