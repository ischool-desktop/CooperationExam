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

        public static string 協同成績輸入狀況 { get { return "CooperationExam.A1CF8F99-E80C-4484-9785-E09B5329B166"; } }
        public static bool 協同成績輸入狀況權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[協同成績輸入狀況].Executable;
            }
        }

    }
}
