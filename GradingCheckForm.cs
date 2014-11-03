using FISCA.Data;
using FISCA.Presentation.Controls;
using K12.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

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
            if (!ContinueQueryCheck())
                return;

            BeginLoading();
            Task task = Task.Factory.StartNew(() =>
            {
                LoadCourses(); // Output => CoursesGradingStatus
                LoadCoursesExamStatus(); // Output => CoursesGradingStatus
            });

            task.ContinueWith(x =>
            {
                if (x.IsFaulted)
                    MessageBox.Show("讀取資料錯誤。");
                else
                    dgStatus.DataSource = GetFilteredData();

                EndLoading();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// 依畫面設定過慮資料後回傳。
        /// </summary>
        /// <returns></returns>
        private IEnumerable<CourseGradingStatus> GetFilteredData()
        {
            return new SortableBindingList<CourseGradingStatus>(CoursesGradingStatus);
        }

        /// <summary>
        /// 載入課程、授課教師資訊，不含輸入狀態。
        /// </summary>
        private void LoadCourses()
        {
            //查詢課程、授課老師資訊。
            string queryCourses = this.GetRCString("SQL.LoadCourses.sql");

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
                string stuCount = row["stu_count"] + "";
                int sequence = int.Parse(row["sequence"] + "");

                CourseGradingStatus cgs = new CourseGradingStatus();
                if (!courses.ContainsKey(cid))
                {
                    cgs.CourseID = cid;
                    cgs.CourseName = courseName;

                    int attendCount;
                    if (int.TryParse(stuCount, out attendCount))
                        cgs.AttendCount = attendCount;
                    else
                        cgs.AttendCount = 0;

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
        /// 載入課程的輸入狀況。
        /// </summary>
        private void LoadCoursesExamStatus()
        {
            //參數：@@SchoolYear、@@Semester、@@ExamID
            string queryCourses = this.GetRCString("SQL.LoadCoursesExamStatus.sql");

            queryCourses = queryCourses.Replace("@@SchoolYear", GetSchoolYear());
            queryCourses = queryCourses.Replace("@@Semester", GetSemester());
            queryCourses = queryCourses.Replace("@@ExamID", GetExamID());
            queryCourses = queryCourses.Replace("@@CourseIDs", GetCourseIDs());

            QueryHelper helper = new QueryHelper();
            DataTable dt = helper.Select(queryCourses);

            Dictionary<string, CourseGradingStatus> courses = CoursesGradingStatus.ToDictionary(x => x.CourseID);
            foreach (DataRow row in dt.Rows)
            {
                string cid = row["courseid"] + ""; //課程編號。
                /*
            <Extension>
	            <Score Sequence="1">89</Score>
	            <Score Sequence="2">87</Score>
            </Extension>
                 * */
                string extxml = row["extension"] + "";

                if (!courses.ContainsKey(cid))
                    continue; //不處理，理論上不會到這兒。

                CourseGradingStatus cgs = courses[cid];

                if (string.IsNullOrWhiteSpace(extxml))
                    continue; //空的資料就跳過。

                XElement extelm = XElement.Parse(string.Format("<Extension>{0}</Extension>", extxml));

                foreach (XElement score in extelm.Elements("Score"))
                {
                    if (!score.HasAttributes) //如果 Score 沒有屬性，則非協同教學資料。
                        continue;

                    int sequence;
                    int.TryParse(score.Attribute("Sequence").Value, out sequence);

                    if (cgs.TeachersStatus.ContainsKey(sequence))
                    {
                        TeacherStatus ts = cgs.TeachersStatus[sequence];

                        if (!string.IsNullOrWhiteSpace(score.Value)) //有成績就 ++
                            ts.Current++;
                    }
                }
            }
        }

        private string GetCourseIDs()
        {
            List<string> courseIDs = CoursesGradingStatus.ConvertAll(x => x.CourseID);
            return string.Join(",", courseIDs);
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
            if (InvokeRequired)
                return Invoke(new Func<string>(GetSchoolYear)) + "";

            return cboSchoolYear.Text;
        }

        private string GetSemester()
        {
            if (InvokeRequired)
                return Invoke(new Func<string>(GetSemester)) + "";

            return cboSemester.Text;
        }

        private string GetExamID()
        {
            if (InvokeRequired)
                return Invoke(new Func<string>(GetExamID)) + "";

            ExamRecord er = cboExam.SelectedItem as ExamRecord;
            if (er != null)
                return er.ID;
            else
                return string.Empty;
        }
    }
}