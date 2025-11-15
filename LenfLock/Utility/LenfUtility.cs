using LenfLock.Communication;
using LenfLock.Utility.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LenfLock.Utility {
    internal class LenfUtility {
        private static LenfUtility instance;
        public static LenfUtility Instance {
            get {
                instance ??= new LenfUtility();
                return instance;
            }
        }

        private LenfAudio LenfAudio;
        private LenfServer LenfServer;
        private LenfMulticast LenfMulticast;
        private LenfUtility() {
            this.LenfAudio = new LenfAudio();
            this.LenfServer = new LenfServer();
            this.LenfMulticast = new LenfMulticast();
        }

        public void Start() {
            this.LenfAudio.StartRecording();
            this.LenfServer.Start();
            this.LenfMulticast.Start();
        }

        public void Stop() {
            this.LenfAudio.StopRecording();
        }
    }
}
