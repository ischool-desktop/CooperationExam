using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CooperationExam
{
    /// <summary>
    /// 提供功能取得組件中的「內嵌資源」。
    /// </summary>
    internal static class RCHelper
    {
        /// <summary>
        /// 取得內嵌資源，並轉換成字串形式。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name">資源名稱。</param>
        /// <returns></returns>
        public static string GetRCString(this object obj, string name)
        {
            if (obj == null)
                return string.Empty;

            Assembly asm = obj.GetType().Assembly;
            string fullname = string.Format("{0}.{1}", asm.GetName().Name, name);

            using (Stream sm = asm.GetManifestResourceStream(fullname))
            {
                if (sm != null)
                {
                    sm.Seek(0, SeekOrigin.Begin);
                    return new StreamReader(sm).ReadToEnd();
                }
                else
                    return string.Empty;
            }
        }
    }
}
