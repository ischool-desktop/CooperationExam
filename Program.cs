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

            //診斷模式不要執行 UDM 更新。
            if (!RTContext.IsDiagMode)
                ServerModule.AutoManaged("http://module.ischool.com.tw/module/148/oneadmin.CooperationExam/udm.xml");

            RibbonBarItem item = FISCA.Presentation.MotherForm.RibbonBarItems["教務作業", "協同教學"];

            Catalog group = RoleAclSource.Instance["教務作業"]["協同教學"];
            group.Add(new RibbonFeature(Permissions.協同成績結算, "協同教學成績結算"));
            item["成績結算"].Enable = Permissions.協同成績結算權限;
            item["成績結算"].Click += delegate
            {
                new ScoreCalculate().ShowDialog();
            };

            group.Add(new RibbonFeature(Permissions.協同成績輸入狀況, "協同教學輸入狀況"));
            item["輸入狀況"].Enable = Permissions.協同成績輸入狀況權限;
            item["輸入狀況"].Click += delegate
            {
                new GradingCheckForm().ShowDialog();
            };

            RibbonBarItem coursePane = FISCA.Presentation.MotherForm.RibbonBarItems["課程", "協同教學"];
            group.Add(new RibbonFeature(Permissions.協同成績輸入, "協同教學成績輸入"));
            coursePane["成績輸入"].Enable = false; ;
            K12.Presentation.NLDPanels.Course.SelectedSourceChanged += delegate
            {
                coursePane["成績輸入"].Enable = (Permissions.協同成績輸入狀況權限 && K12.Presentation.NLDPanels.Course.SelectedSource.Count == 1);
            };
            coursePane["成績輸入"].Click += delegate
            {
                new GradingForm().ShowDialog();
            };
        }
    }
}