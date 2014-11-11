using FISCA.Data;
using FISCA.Presentation.Controls;
using K12.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace CooperationExam
{
    public partial class ScoreCalculate : BaseForm
    {
        private BackgroundWorker _BW;
        private QueryHelper _Q;
        private UpdateHelper _U;
        private string _schoolYear, _semester,_examid;

        public ScoreCalculate()
        {
            InitializeComponent();
            _Q = new QueryHelper();
            _U = new UpdateHelper();
            _BW = new BackgroundWorker();
            _BW.DoWork += new DoWorkEventHandler(_BW_DoWork);
            _BW.ProgressChanged += new ProgressChangedEventHandler(_BW_ProgressChanged);
            _BW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BW_RunWorkerCompleted);
            _BW.WorkerReportsProgress = true;

            string schoolYear = K12.Data.School.DefaultSchoolYear;
            string semester = K12.Data.School.DefaultSemester;

            int sy;
            if (int.TryParse(schoolYear, out sy))
            {
                for (int i = -2; i <= 2; i++)
                    cboSchoolYear.Items.Add(sy + i);
            }

            cboSemester.Items.Add("1");
            cboSemester.Items.Add("2");

            cboSchoolYear.Text = schoolYear;
            cboSemester.Text = semester;

            foreach (ExamRecord exam in K12.Data.Exam.SelectAll())
            {
                    cboExam.Items.Add(new ExamItem(exam.Name, exam.ID));
            }

            if (cboExam.Items.Count > 0)
                cboExam.SelectedItem = cboExam.Items[0];
        }

        private void _BW_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FISCA.Presentation.MotherForm.SetStatusBarMessage(e.UserState + "", e.ProgressPercentage);
        }

        private void _BW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetForm(true);
            MessageBox.Show("協同成績計算完成");
            this.Close();
        }

        private void _BW_DoWork(object sender, DoWorkEventArgs e)
        {
            Dictionary<string, string> oldScores = new Dictionary<string, string>();
            Dictionary<string, string> studentInfo = new Dictionary<string, string>();

            _BW.ReportProgress(0, "協同成績計算中...");
            //PropertyInfo pi = r[0].GetType().GetProperty("Score", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic);
            //decimal score = (decimal)pi.GetValue(r[0], null);

            string sql = "select exam.exam_name,sce_take.id as take_id,student.name,class.class_name,course.course_name,course.school_year,course.semester,tag.access_control_code,sce_take.score,sce_take.extension from sce_take";
            sql += " join sc_attend on sc_attend.id = sce_take.ref_sc_attend_id";
            sql += " join course on course.id = sc_attend.ref_course_id";
            sql += " left join tag_course on tag_course.ref_course_id=course.id";
            sql += " left join tag on tag_course.ref_tag_id=tag.id";
            sql += " join student on sc_attend.ref_student_id=student.id";
            sql += " left join class on student.ref_class_id=class.id";
            sql += " left join exam on sce_take.ref_exam_id=exam.id";
            sql += " where course.school_year=" + _schoolYear + " and course.semester=" + _semester + " and tag.access_control_code='OneAdmin.CooperationExam' and sce_take.ref_exam_id=" + _examid;

            DataTable dt = _Q.Select(sql);
            XmlDocument doc = new XmlDocument();

            if (dt.Rows.Count > 0)
            {
                decimal per = 70 / dt.Rows.Count;
                int index = 0;

                Dictionary<string, decimal> tmp_score = new Dictionary<string, decimal>();
                List<string> sce_take_ids = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    string take_id = row["take_id"] + "";
                    string extension = row["extension"] + "";
                    string score = row["score"] + "";
                    string name = row["name"] + "";
                    string class_name = row["class_name"] + "";
                    string course_name = row["course_name"] + "";
                    string exam_name = row["exam_name"] + "";

                    if (!oldScores.ContainsKey(take_id))
                        oldScores.Add(take_id, score);

                    if (!studentInfo.ContainsKey(take_id))
                        studentInfo.Add(take_id, string.Format("班級:{0} 姓名:{1} 課程:{2} 試別:{3}", class_name, name, course_name, exam_name));

                    sce_take_ids.Add(take_id);

                    doc.LoadXml("<root>" + extension + "</root>");

                    int count = 0;
                    decimal sum = 0;
                    decimal avg = 0;
                    foreach (XmlElement elem in doc.SelectNodes("root/Extension/Score[@Sequence]"))
                    {
                        count++;

                        decimal d;
                        if (decimal.TryParse(elem.InnerText, out d))
                        {
                            sum += d;
                        }
                    }

                    if (count > 0)
                    {
                        avg = Math.Round(sum / count, 0, MidpointRounding.AwayFromZero);

                        if (!tmp_score.ContainsKey(take_id))
                            tmp_score.Add(take_id, avg);
                    }

                    index++;
                    int p = (int)(index * per);
                    _BW.ReportProgress(p, "協同成績計算中...");
                }

                _BW.ReportProgress(80, "準備更新資料中...");
                List<string> cmd = new List<string>();
                foreach (string id in tmp_score.Keys)
                {
                    cmd.Add(string.Format("update sce_take set score={0} where id={1}", tmp_score[id], id));
                }

                _BW.ReportProgress(90, "更新資料中...");
                _U.Execute(cmd);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("學年度:{0}\t學期:{1}", _schoolYear, _semester));
                foreach (string id in oldScores.Keys)
                {
                    string old_score = oldScores[id];
                    string new_score = tmp_score.ContainsKey(id) ? tmp_score[id] + "" : "";
                    string info = studentInfo[id];

                    sb.AppendLine(info);
                    sb.AppendLine(string.Format("原始成績:{0} 新成績:{1}", old_score, new_score));
                }

                FISCA.LogAgent.ApplicationLog.Log("協同成績計算", "計算成績", sb.ToString());
            }

            _BW.ReportProgress(100, "協同成績計算完成");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            _schoolYear = cboSchoolYear.Text;
            _semester = cboSemester.Text;
            ExamItem exam = cboExam.SelectedItem as ExamItem;

            int sy;
            if (!int.TryParse(_schoolYear, out sy))
            {
                MessageBox.Show("學年度必須為整數");
                return;
            }

            if (cboSemester.Text != "1" && cboSemester.Text != "2")
            {
                MessageBox.Show("學期必須為1或2");
                return;
            }

            if (exam == null)
            {
                MessageBox.Show("請選擇考試評量");
                return;
            }
            else
            {
                _examid = exam.Value;
            }

            if (_BW.IsBusy)
            {
                MessageBox.Show("系統忙碌請稍後再試...");
            }
            else
            {
                SetForm(false);
                _BW.RunWorkerAsync();
            }
        }

        private void SetForm(bool b)
        {
            cboSchoolYear.Enabled = b;
            cboSemester.Enabled = b;
            btnOk.Enabled = b;
        }

        class ExamItem
        {
            string _key, _value;
            public string DisplayText
            {
                get
                {
                    return _key;
                }
            }

            public string Value
            {
                get
                {
                    return _value;
                }
            }

            public ExamItem(string key, string value)
            {
                _key = key;
                _value = value;
            }
        }
    }
}
