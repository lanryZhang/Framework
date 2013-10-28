using System;
using System.Collections.Generic;
using System.Text;

namespace Ifeng.Utility.Helper
{
    public static class EnumHelper
    {
        public static int GetValue(Type type, string name)
        {
            var names = Enum.GetNames(type);
            var res = 0;

            foreach (var n in names)
            {
                if (n == name)
                {
                    res = (int)Enum.Parse(type, name);
                    break;
                }
            }

            return res;
        }
    }
}
