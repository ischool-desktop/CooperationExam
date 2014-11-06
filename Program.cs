using Customization.Tagging;
using FISCA;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CooperationExam
{
    public class Program
    {
        [MainMethod]
        public static void Main()
        {
            SystemTag.Define("Course", "協同教學", Color.Red, "OneAdmin.CooperationExam", "設定協同教學課程", "課程");
            new GradingCheckForm().ShowDialog();
        }
    }
}
