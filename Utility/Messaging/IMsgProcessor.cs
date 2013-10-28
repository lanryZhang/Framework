using System;
using System.Collections.Generic;
using System.Text;

namespace Ifeng.Utility.Messaging
{
    public interface IMsgProcessor
    {
        // Methods
        void Process(IReadableMessage msg);
    }
}
