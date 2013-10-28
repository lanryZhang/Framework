using System;
using System.Collections.Generic;
using System.Text;

namespace Ifeng.Utility.Messaging
{
    public interface IReadableMessage
    {
        // Methods
        string GetContent();
        string GetName();
        string GetSource();
    }
}
