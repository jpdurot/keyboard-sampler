#if !DOTNETCORE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace Sampler
{
    public class KeyDownEventArgs
    {
        public byte KeyCode
        {
            get; private set;
        }

        public bool IsShiftPressed
        {
            get; private set;
        }

        public KeyDownEventArgs(byte[] buffer)
        {
            KeyCode = buffer[2];
            IsShiftPressed = ((buffer[0] & 2) == 2);
        }
    }
}
#endif
