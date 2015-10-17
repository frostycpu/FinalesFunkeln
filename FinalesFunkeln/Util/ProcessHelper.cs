using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalesFunkeln.Util
{
    class ProcessHelper
    {
        public static List<string> SplitCommandLineArgs(string commandLine)
        {
            var sb = new StringBuilder();
            var args = new List<string>();
            var inQuotes = false;
            foreach (var c in commandLine)
            {
                switch (c)
                {
                    case '"':
                        if (inQuotes)
                        {
                            args.Add(sb.ToString());
                            sb.Length = 0;
                            inQuotes = false;
                        }
                        else
                        {
                            inQuotes = true;
                        }
                        break;
                    case ' ':
                        if (inQuotes)
                        {
                            sb.Append(c);
                        }
                        else
                        {
                            if(sb.Length>0)
                                args.Add(sb.ToString());
                            sb.Length = 0;
                        }
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return args;
        }
    }
}
