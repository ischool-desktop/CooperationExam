using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CooperationExam
{
    /// <summary>
    /// 代表課程輸入狀態
    /// </summary>
    class CourseGradingStatus
    {
        public CourseGradingStatus()
        {
            TeachersStatus = new Dictionary<int, TeacherStatus>();
        }

        public string CourseID { get; set; }

        public string CourseName { get; set; }

        /// <summary>
        /// 應輸入總數。
        /// </summary>
        public int AttendCount { get; set; }

        public Dictionary<int, TeacherStatus> TeachersStatus { get; private set; }

        public string Teacher1Name
        {
            get { return GetStatusString(1); }
        }

        public string Teacher2Name
        {
            get { return GetStatusString(2); }
        }

        public string Teacher3Name
        {
            get { return GetStatusString(3); }
        }

        /// <summary>
        /// 取得輸入狀態的字串，例：呂韻如 (45/45)
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        private string GetStatusString(int sequence)
        {
            if (TeachersStatus.ContainsKey(sequence))
            {
                TeacherStatus ts = TeachersStatus[sequence];

                return string.Format("{0} ({1}/{2})", ts.TeacherName, ts.Current, AttendCount);
            }
            else
                return string.Empty;
        }
    }

    class TeacherStatus
    {
        public TeacherStatus()
        {
            Current = 0;
        }

        public string TeacherName { get; set; }

        /// <summary>
        /// 目前輸入數。
        /// </summary>
        public int Current { get; set; }
    }
}
