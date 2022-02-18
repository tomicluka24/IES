using FTN.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMSTestClientWPF
{
    class Channel
    {
        private static Channel instance = null;

        private NetworkModelGDAProxy gdaQueryProxy = null;
        public NetworkModelGDAProxy GdaQueryProxy
        {
            get
            {
                if (gdaQueryProxy != null)
                {
                    gdaQueryProxy.Abort();
                    gdaQueryProxy = null;
                }
                gdaQueryProxy = new NetworkModelGDAProxy("NetworkModelGDAEndpoint");
                gdaQueryProxy.Open();

                return gdaQueryProxy;
            }
        }
        private Channel() { }
        public static Channel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Channel();
                }
                return instance;
            }
        }
    }
}
