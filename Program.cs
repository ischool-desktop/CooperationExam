using Customization.Tagging;
using FISCA;
using FISCA.Permission;
using FISCA.Presentation;
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
            //new GradingCheckForm().ShowDialog();

            RibbonBarItem item = FISCA.Presentation.MotherForm.RibbonBarItems["教務作業", "測試區"];
            item["協同教學成績結算"].Enable = Permissions.協同成績結算權限;
            item["協同教學成績結算"].Click += delegate
            {
                new ScoreCalculate().ShowDialog();
            };

            Catalog detail1 = RoleAclSource.Instance["教務作業"]["協同教學成績結算"];
            detail1.Add(new RibbonFeature(Permissions.協同成績結算, "協同教學成績結算"));
        }
    }
}
