using System;
using System.Collections.Generic;
using System.Text;

namespace Ifeng.Utility.Ioc
{
    class ServiceInitException : ApplicationException
    {
        private string _ServiceName;

        public ServiceInitException(string serviceName)
            : this(serviceName, string.Empty)
        {
        }

        public ServiceInitException(string serviceName, string msg)
            : base(msg)
        {
            _ServiceName = serviceName;
        }

        public string ServiceName
        {
            get
            {
                return _ServiceName;
            }
        }

    }
}
