using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Ifeng.Utility
{
    public class ApplicationArgument
    {
        private StringBuilder _builder = new StringBuilder();

        public void Add(string key)
        {
            if (this._builder.Length == 0)
            {
                this._builder.AppendFormat("--{0}", key);
            }
            else
            {
                this._builder.AppendFormat(" --{0}", key);
            }
        }

        public void Add(string key, string value)
        {
            if (this._builder.Length == 0)
            {
                this._builder.AppendFormat("/{0}:{1}", key, value);
            }
            else
            {
                this._builder.AppendFormat(" /{0}:{1}", key, value);
            }
        }

        public void AddWithDoubleQuote(string key, string value, char quoteChar)
        {
            this.AddWithQuote(key, value, '"');
        }

        private void AddWithQuote(string key, string value, char quoteChar)
        {
            if (this._builder.Length == 0)
            {
                this._builder.AppendFormat("/{0}:{1}{2}{1}", key, quoteChar, value);
            }
            else
            {
                this._builder.AppendFormat(" /{0}:{1}{2}{1}", key, quoteChar, value);
            }
        }

        public void AddWithSingleQuote(string key, string value, char quoteChar)
        {
            this.AddWithQuote(key, value, '\'');
        }

        public override int GetHashCode()
        {
            return this._builder.GetHashCode();
        }

        public static Dictionary<string, string> Parse(string[] args)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            Regex spliterRegex = new Regex("^-{1,2}|^/|=|:", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Regex removerRegex = new Regex("^['\"]?(.*?)['\"]?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            string parameter = null;
            foreach (string text in args)
            {
                string[] parts = spliterRegex.Split(text, 3);
                switch (parts.Length)
                {
                    case 1:
                        if (parameter != null)
                        {
                            if (!dict.ContainsKey(parameter))
                            {
                                parts[0] = removerRegex.Replace(parts[0], "$1");
                                dict.Add(parameter, parts[0]);
                            }
                            parameter = null;
                        }
                        break;

                    case 2:
                        if ((parameter != null) && !dict.ContainsKey(parameter))
                        {
                            dict.Add(parameter, "true");
                        }
                        parameter = parts[1];
                        break;

                    case 3:
                        if ((parameter != null) && !dict.ContainsKey(parameter))
                        {
                            dict.Add(parameter, "true");
                        }
                        parameter = parts[1];
                        if (!dict.ContainsKey(parameter))
                        {
                            parts[2] = removerRegex.Replace(parts[2], "$1");
                            dict.Add(parameter, parts[2]);
                        }
                        parameter = null;
                        break;
                }
            }
            if ((parameter != null) && !dict.ContainsKey(parameter))
            {
                dict.Add(parameter, "true");
            }
            return dict;
        }

        public override string ToString()
        {
            return this._builder.ToString();
        }

    }
}
