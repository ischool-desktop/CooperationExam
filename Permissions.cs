using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CooperationExam
{
    class Permissions
    {
        public static string 協同成績結算 { get { return "CooperationExam.D81732A8-F55C-4B18-9E9D-A8D23373A8AA"; } }
        public static bool 協同成績結算權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[協同成績結算].Executable;
            }
        }
    }
}
