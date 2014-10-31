using FISCA;
using System;
using System.Collections.Generic;
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
            new GradingCheckForm().ShowDialog();
        }
    }
}
