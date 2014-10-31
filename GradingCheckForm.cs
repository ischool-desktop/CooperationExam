using FISCA.Data;
using FISCA.Presentation.Controls;
using K12.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CooperationExam
{
    public partial class GradingCheckForm : BaseForm
    {
        private List<CourseGradingStatus> CoursesGradingStatus { get; set; }

        public GradingCheckForm()
        {
            InitializeComponent();

            CoursesGradingStatus = new List<CourseGradingStatus>();
        }

        /// <summary>
        /// 僅載入清單類型的資料，不應進行條件式的資料載入。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GradingCheckForm_Load(object sender, EventArgs e)
        {
            dgStatus.AutoGenerateColumns = false;

            try
            {
                BeginLoading();

                cboSchoolYear.Text = K12.Data.School.DefaultSchoolYear;
                cboSemester.Text = K12.Data.School.DefaultSemester;

                LoadExamRecord();
            }
            catch (Exception ex)
            {
                MessageBox.Show("載入資料錯誤：" + ex.Message);
            }
            finally
            {
                EndLoading();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ContinueQueryCheck();
            LoadCoursePart1();

            dgStatus.DataSource = CoursesGradingStatus;
        }

        /// <summary>
        /// 載入課程、授課教師資訊，不含輸入狀態。
        /// </summary>
        private void LoadCoursePart1()
        {
            //查詢課程、授課老師資訊。
            string queryCourses = @"select course.id,course.course_name,teacher.teacher_name,tc_instruct.sequence
from course join tc_instruct on course.id=tc_instruct.ref_course_id
	join teacher on teacher.id=tc_instruct.ref_teacher_id
where course.school_year=@@SchoolYear and course.semester=@@Semester
order by course.id,sequence";

            queryCourses = queryCourses.Replace("@@SchoolYear", GetSchoolYear());
            queryCourses = queryCourses.Replace("@@Semester", GetSemester());

            QueryHelper helper = new QueryHelper();
            DataTable dt = helper.Select(queryCourses);

            Dictionary<string, CourseGradingStatus> courses = new Dictionary<string, CourseGradingStatus>();
            foreach (DataRow row in dt.Rows)
            {
                string cid = row["id"] + "";
                string courseName = row["course_name"] + "";
                string teacherName = row["teacher_name"] + "";
                int sequence = int.Parse(row["sequence"] + "");

                CourseGradingStatus cgs = new CourseGradingStatus();
                if (!courses.ContainsKey(cid))
                {
                    cgs.CourseID = cid;
                    cgs.CourseName = courseName;
                    courses.Add(cid, cgs);
                }
                else
                    cgs = courses[cid];

                if (!cgs.TeachersStatus.ContainsKey(sequence))
                {
                    cgs.TeachersStatus.Add(sequence, new TeacherStatus() { TeacherName = teacherName });
                }

                courses[cid] = cgs;
            }

            CoursesGradingStatus = courses.Values.ToList();
        }

        /// <summary>
        /// 如果檢查通過，就可執行查詢。
        /// </summary>
        /// <returns></returns>
        private bool ContinueQueryCheck()
        {
            ep1.SetError(cboExam, "");
            if (cboExam.SelectedItem == null)
            {
                ep1.SetError(cboExam, "請選擇正確的試別。");
                return false;
            }

            int val;

            ep1.SetError(cboSchoolYear, "");
            if (!int.TryParse(cboSchoolYear.Text, out val))
            {
                ep1.SetError(cboSchoolYear, "必須是數字。");
                return false;
            }

            ep1.SetError(cboSemester, "");
            if (!int.TryParse(cboSemester.Text, out val))
            {
                ep1.SetError(cboSemester, "必須是數字。");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 載入試別清單到畫面上，並選擇第一筆資料。
        /// </summary>
        private void LoadExamRecord()
        {
            List<ExamRecord> exams = Exam.SelectAll();

            //按 DisplayOrder 排序
            exams.Sort((x, y) =>
            {
                int xv = x.DisplayOrder.HasValue ? x.DisplayOrder.Value : int.MaxValue;
                int xy = y.DisplayOrder.HasValue ? y.DisplayOrder.Value : int.MaxValue;
                return xv.CompareTo(xy);
            });

            //顯示資料。
            foreach (ExamRecord exam in exams)
                cboExam.Items.Add(exam);

            cboExam.ValueMember = "ID";
            cboExam.DisplayMember = "Name";

            if (cboExam.Items.Count > 0)
                cboExam.SelectedIndex = 0;
        }

        private void BeginLoading()
        {
            if (InvokeRequired)
                Invoke(new Action(BeginLoading));

            cp1.IsRunning = true;
            btnRefresh.Enabled = false;
        }

        private void EndLoading()
        {
            if (InvokeRequired)
                Invoke(new Action(EndLoading));

            cp1.IsRunning = false;
            btnRefresh.Enabled = true;
        }

        private string GetSchoolYear()
        {
            return cboSchoolYear.Text;
        }

        private string GetSemester()
        {
            return cboSemester.Text;
        }

        class CourseBindingSource : BindingSource
        {
            public override bool SupportsSorting
            {
                get
                {
                    return true;
                }
            }
        }
    }
}
