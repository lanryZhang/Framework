using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
namespace Ifeng.Utility.Helper
{
    public static class RetryHelper
    {
        // Methods
        public static void Retry(int retryCount, TimeSpan interval, bool throwIfFail, 
            Action func)
        {
            if (func != null)
            {
                for (int i = 0; i < retryCount; i++)
                {
                    try
                    {
                        func();
                        break;
                    }
                    catch (Exception)
                    {
                        if (i == (retryCount - 1))
                        {
                            if (throwIfFail)
                            {
                                throw;
                            }
                            break;
                        }
                        if (interval > TimeSpan.Zero)
                        {
                            Thread.Sleep(interval);
                        }
                    }
                }
            }
        }
    }

 

}
