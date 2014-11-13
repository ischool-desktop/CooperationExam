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
using System.Windows.Forms;
using System.Xml;

namespace CooperationExam
{
    public partial class GradingForm : BaseForm
    {
        private CourseRecord _course;
        Dictionary<string, CEScoreObj> _CEScoreObjDic;
        List<DataGridViewCell> _dirtyCellList;

        QueryHelper _Q;
        UpdateHelper _U;
        BackgroundWorker _BW;
        object _runningExam;

        const string access_control_code = "OneAdmin.CooperationExam";

        public GradingForm()
        {
            InitializeComponent();

            _course = K12.Data.Course.SelectByID(K12.Presentation.NLDPanels.Course.SelectedSource[0]);

            _Q = new QueryHelper();
            _U = new UpdateHelper();
            _CEScoreObjDic = new Dictionary<string, CEScoreObj>();
            _dirtyCellList = new List<DataGridViewCell>();

            _BW = new BackgroundWorker();
            _BW.DoWork += new DoWorkEventHandler(_BW_DoWork);
            _BW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BW_Completed);
        }

        //Form Load
        private void GradingForm_Load(object sender, EventArgs e)
        {
            //判斷課程是否為協同教學
            string sql_hasTag = "select tag.access_control_code from tag_course ";
            sql_hasTag += "join tag on tag.id=tag_course.ref_tag_id ";
            sql_hasTag += "where ref_course_id=" + _course.ID + " and tag.access_control_code='" + access_control_code + "'";
            DataTable dt = _Q.Select(sql_hasTag);

            if (dt.Rows.Count > 0)
            {
                #region 老師名稱列印
                List<string> teachers = new List<string>();
                foreach (CourseTeacherRecord ctr in _course.Teachers)
                {
                    teachers.Add(ctr.TeacherName);
                }

                string teacher_str = string.Join(",", teachers);

                lblCourseName.Text = _course.Name + (!string.IsNullOrEmpty(teacher_str) ? " (" + teacher_str + ")" : "");

                //未設定老師1的欄位無法輸入成績
                if (teachers.Count > 0)
                    colTeacher1.HeaderText = teachers[0];
                else
                    colTeacher1.ReadOnly = true;

                //未設定老師2的欄位無法輸入成績
                if (teachers.Count > 1)
                    colTeacher2.HeaderText = teachers[1];
                else
                    colTeacher2.ReadOnly = true;

                #endregion

                //取得該課程設定的試別
                string sql_getExamList = "select te_include.ref_exam_id,exam.exam_name from course ";
                sql_getExamList += "join te_include on te_include.ref_exam_template_id=course.ref_exam_template_id ";
                sql_getExamList += "join exam on exam.id=te_include.ref_exam_id ";
                sql_getExamList += "where course.id=" + _course.ID + " order by exam.display_order";

                dt = _Q.Select(sql_getExamList);
                foreach (DataRow row in dt.Rows)
                {
                    cboExamList.Items.Add(new ExamItem(row["exam_name"] + "", row["ref_exam_id"] + ""));
                }

                //將學生填入DataGridView
                FillStudentsToDataGridView();

                //選擇第一個試別
                if (cboExamList.Items.Count > 0)
                    cboExamList.SelectedItem = cboExamList.Items[0];
                else
                {
                    MessageBox.Show("該課程無任何試別可以輸入");
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("該課程未設定成協同教學");
                this.Close();
            }
        }

        /// <summary>
        /// 將學生填入DataGridView。
        /// </summary>
        private void FillStudentsToDataGridView()
        {
            dgv.SuspendLayout();
            dgv.Rows.Clear();

            List<SCAttendRecord> _scAttendRecordList = SCAttend.SelectByCourseIDs(new string[] { _course.ID });

            //休課紀錄排序
            _scAttendRecordList.Sort(delegate(SCAttendRecord x, SCAttendRecord y)
            {
                StudentRecord aStudent = x.Student;
                ClassRecord aClass = x.Student.Class;
                StudentRecord bStudent = y.Student;
                ClassRecord bClass = y.Student.Class;

                string aa = aClass == null ? (string.Empty).PadLeft(10, '0') : (aClass.Name).PadLeft(10, '0');
                aa += aStudent == null ? (string.Empty).PadLeft(3, '0') : (aStudent.SeatNo + "").PadLeft(3, '0');
                aa += aStudent == null ? (string.Empty).PadLeft(10, '0') : (aStudent.StudentNumber).PadLeft(10, '0');

                string bb = bClass == null ? (string.Empty).PadLeft(10, '0') : (bClass.Name).PadLeft(10, '0');
                bb += bStudent == null ? (string.Empty).PadLeft(3, '0') : (bStudent.SeatNo + "").PadLeft(3, '0');
                bb += bStudent == null ? (string.Empty).PadLeft(10, '0') : (bStudent.StudentNumber).PadLeft(10, '0');

                return aa.CompareTo(bb);
            });


            foreach (SCAttendRecord record in _scAttendRecordList)
            {
                StudentRecord student = record.Student;
                ClassRecord cr = student.Class;
                if (student.StatusStr != "一般") continue;

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgv,
                    (cr != null) ? cr.Name : "",
                    student.SeatNo,
                    student.Name,
                    student.StudentNumber
                );
                row.Tag = record;
                dgv.Rows.Add(row);
            }

            dgv.ResumeLayout();
        }

        private void _BW_DoWork(object sender, DoWorkEventArgs e)
        {
            Save();
        }

        private void _BW_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            LoadData();
        }

        private void Save()
        {
            List<string> updates = new List<string>();
            StringBuilder sb = new StringBuilder();
            bool mustWirteLog = false;

            ExamItem exam = _runningExam as ExamItem;
            string exam_id = exam.Value;

            sb.AppendLine(string.Format("課程:{0} 試別:{1}", _course.Name, exam.DisplayText));

            foreach (DataGridViewRow row in dgv.Rows)
            {
                SCAttendRecord scr = row.Tag as SCAttendRecord;
                string student_id = scr.RefStudentID;
                string scattend_id = scr.ID;
                
                string class_name = row.Cells[colClassName.Index].Value + "";
                string seat_no = row.Cells[colSeatNo.Index].Value + "";
                string student_name = row.Cells[colName.Index].Value + "";

                string old_score1 = row.Cells[colTeacher1.Index].Tag + "";
                string new_score1 = row.Cells[colTeacher1.Index].Value + "";
                string old_score2 = row.Cells[colTeacher2.Index].Tag + "";
                string new_score2 = row.Cells[colTeacher2.Index].Value + "";

                if (old_score1 != new_score1 || old_score2 != new_score2)
                {
                    mustWirteLog = true;
                    CEScoreObj ceso = _CEScoreObjDic.ContainsKey(student_id) ? _CEScoreObjDic[student_id] : null;

                    if (ceso != null)
                    {
                        string sce_take_id = ceso.SCETakeID;
                        ceso.CEScore1 = null;
                        ceso.CEScore2 = null;

                        decimal d;
                        if (decimal.TryParse(row.Cells[colTeacher1.Index].Value + "", out d))
                            ceso.CEScore1 = d;
                        if (decimal.TryParse(row.Cells[colTeacher2.Index].Value + "", out d))
                            ceso.CEScore2 = d;

                        string extension = ceso.GetExtension();

                        updates.Add(string.Format("update sce_take set extension='{0}' where id={1}", extension, sce_take_id));

                        if (old_score1 != new_score1)
                            sb.AppendLine(string.Format("班級:{0} 座號:{1} 姓名:{2} 原始成績一:{3} 修改成績一:{4}", class_name, seat_no, student_name, old_score1, new_score1));
                        if (old_score2 != new_score2)
                            sb.AppendLine(string.Format("班級:{0} 座號:{1} 姓名:{2} 原始成績二:{3} 修改成績二:{4}", class_name, seat_no, student_name, old_score2, new_score2));
                    }
                    else
                    {
                        //if (!string.IsNullOrWhiteSpace(row.Cells[colTeacher1.Index].Value + "") || !string.IsNullOrWhiteSpace(row.Cells[colTeacher2.Index].Value + ""))
                        //{
                            //mustWirteLog = true;
                            ceso = new CEScoreObj(string.Empty);

                            decimal d;
                            if (decimal.TryParse(row.Cells[colTeacher1.Index].Value + "", out d))
                                ceso.CEScore1 = d;
                            if (decimal.TryParse(row.Cells[colTeacher2.Index].Value + "", out d))
                                ceso.CEScore2 = d;

                            string extension = ceso.GetExtension();

                            updates.Add(string.Format("insert into sce_take (ref_sc_attend_id, ref_exam_id, extension) values ({0},{1},'{2}')", scattend_id, exam_id, extension));

                            if (old_score1 != new_score1)
                                sb.AppendLine(string.Format("班級:{0} 座號:{1} 姓名:{2} 原始成績一:{3}  修改成績一:{4}", class_name, seat_no, student_name, old_score1, new_score1));
                            if (old_score2 != new_score2)
                                sb.AppendLine(string.Format("班級:{0} 座號:{1} 姓名:{2} 原始成績二:{3} 修改成績二:{4}", class_name, seat_no, student_name, old_score2, new_score2));
                        //}
                    }
                }
            }

            if (updates.Count > 0)
                _U.Execute(updates);

            if (mustWirteLog)
                FISCA.LogAgent.ApplicationLog.Log("協同教學成績輸入", "修改成績", sb.ToString());
        }

        private void LoadData()
        {
            _dirtyCellList.Clear();
            lblSave.Visible = false;
            _CEScoreObjDic.Clear();

            pictureBox1.Visible = false;
            cboExamList.Enabled = true;
            dgv.Enabled = true;

            ExamItem ei = _runningExam as ExamItem;

            string sql_getScTake = "select sce_take.id as take_id,sc_attend.id as attend_id,student.id as student_id,student.name as student_name,sce_take.extension,course.course_name from sce_take ";
            sql_getScTake += "join sc_attend on sc_attend.id = sce_take.ref_sc_attend_id ";
            sql_getScTake += "join student on sc_attend.ref_student_id=student.id ";
            sql_getScTake += "join course on course.id = sc_attend.ref_course_id ";
            sql_getScTake += "join exam on sce_take.ref_exam_id=exam.id ";
            sql_getScTake += "where course.school_year=" + _course.SchoolYear + " and course.semester=" + _course.Semester + " and course.id=" + _course.ID + " and sce_take.ref_exam_id=" + ei.Value;

            XmlDocument doc = new XmlDocument();

            DataTable dt = _Q.Select(sql_getScTake);
            foreach (DataRow row in dt.Rows)
            {
                string student_id = row["student_id"] + "";
                string take_id = row["take_id"] + "";
                string extension = row["extension"] + "";

                if (!_CEScoreObjDic.ContainsKey(student_id))
                    _CEScoreObjDic.Add(student_id, new CEScoreObj(extension));

                _CEScoreObjDic[student_id].SCETakeID = take_id;

                doc.LoadXml("<root>" + extension + "</root>");

                foreach (XmlElement elem in doc.SelectNodes("root/Extension/Score[@Sequence]"))
                {
                    string sequence = elem.GetAttribute("Sequence");
                    decimal d;
                    if (decimal.TryParse(elem.InnerText, out d))
                    {
                        if (sequence == "1")
                            _CEScoreObjDic[student_id].CEScore1 = d;
                        else if (sequence == "2")
                            _CEScoreObjDic[student_id].CEScore2 = d;
                    }
                }
            }

            foreach (DataGridViewRow row in dgv.Rows)
            {
                SCAttendRecord scr = row.Tag as SCAttendRecord;
                string studentID = scr.RefStudentID;

                row.Cells[colTeacher1.Index].Value = "";
                row.Cells[colTeacher2.Index].Value = "";

                if (_CEScoreObjDic.ContainsKey(studentID))
                {
                    if (_CEScoreObjDic[studentID].CEScore1.HasValue)
                        row.Cells[colTeacher1.Index].Value = _CEScoreObjDic[studentID].CEScore1.Value;

                    if (_CEScoreObjDic[studentID].CEScore2.HasValue)
                        row.Cells[colTeacher2.Index].Value = _CEScoreObjDic[studentID].CEScore2.Value;
                }

                //記憶原始成績
                row.Cells[colTeacher1.Index].Tag = row.Cells[colTeacher1.Index].Value;
                row.Cells[colTeacher2.Index].Tag = row.Cells[colTeacher2.Index].Value;

                //文字顏色修正
                FixTextColor(row.Cells[colTeacher1.Index]);
                FixTextColor(row.Cells[colTeacher2.Index]);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            dgv.EndEdit();

            if (!IsValid())
            {
                MessageBox.Show("儲存資料有誤,無法儲存");
                return;
            }

            if (HasGreenText())
            {
                if (MessageBox.Show("資料存在非0~100分的成績,確定要儲存?", "ischool", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            }

            if (_BW.IsBusy)
            {
                MessageBox.Show("系統忙碌中,請稍後再試");
                return;
            }
            else
            {
                pictureBox1.Visible = true;
                cboExamList.Enabled = false;
                dgv.Enabled = false;
                _BW.RunWorkerAsync();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cboExamList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_runningExam == null)
                _runningExam = new object();

            //選擇與上次相同Item則不做畫面ReLoad
            if (!_runningExam.Equals(cboExamList.SelectedItem))
            {
                if (_dirtyCellList.Count > 0)
                {
                    if (MessageBox.Show("資料尚未儲存,確定切換?", "ischool", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        //不切換則選回上次的Item
                        cboExamList.SelectedItem = _runningExam;
                        return;
                    }
                }

                _runningExam = cboExamList.SelectedItem;
                LoadData();
            }
        }

        /// <summary>
        /// 驗證每個欄位是否正確。
        /// 有錯誤訊息表示不正確。
        /// </summary>
        /// <returns></returns>
        private bool IsValid()
        {
            bool valid = true;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (!string.IsNullOrEmpty(row.Cells[colTeacher1.Index].ErrorText))
                    valid = false;

                if (!string.IsNullOrEmpty(row.Cells[colTeacher2.Index].ErrorText))
                    valid = false;
            }

            return valid;
        }

        /// <summary>
        /// 驗證分數是否超出正常範圍(0~100)。
        /// </summary>
        private bool HasGreenText()
        {
            bool retVal = false;
            //只驗證dirtyCell,找到一個就不繼續驗證了
            foreach (DataGridViewCell cell in _dirtyCellList)
            {
                if (cell.Style.ForeColor == Color.Green)
                {
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }

        /// <summary>
        /// 修正數字顏色
        /// </summary>
        private void FixTextColor(DataGridViewCell cell)
        {
            cell.Style.ForeColor = Color.Black;

            cell.ErrorText = "";
            if (!string.IsNullOrEmpty("" + cell.Value))
            {
                decimal d;
                if (decimal.TryParse("" + cell.Value, out d))
                {
                    if (d < 60)
                        cell.Style.ForeColor = Color.Red;
                    if (d > 100 || d < 0)
                        cell.Style.ForeColor = Color.Green;
                }
                else
                {
                    cell.ErrorText = "分數必須是數字";
                }
            }
        }

        /// <summary>
        /// 當欄位結束編輯，進行驗證。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != colTeacher1.Index && e.ColumnIndex != colTeacher2.Index) return;

            DataGridViewCell cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];

            //驗證分數及分數顏色修正
            FixTextColor(cell);

            //Cell變更檢查
            if ("" + cell.Tag != "" + cell.Value)
            {
                if (!_dirtyCellList.Contains(cell)) _dirtyCellList.Add(cell);
            }
            else
            {
                if (_dirtyCellList.Contains(cell)) _dirtyCellList.Remove(cell);
            }

            lblSave.Visible = _dirtyCellList.Count > 0;
        }

        /// <summary>
        /// 點欄位立即進入編輯。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == colTeacher1.Index || e.ColumnIndex == colTeacher2.Index)
                dgv.BeginEdit(true);
        }
    }

    class CEScoreObj
    {
        public string SCETakeID;
        public decimal? CEScore1;
        public decimal? CEScore2;
        private string _extension;

        public CEScoreObj(string extension)
        {
            _extension = extension;
        }

        public string GetExtension()
        {
            XmlDocument doc = new XmlDocument();

            doc.LoadXml("<root>" + _extension + "</root>");

            XmlElement extension = doc.SelectSingleNode("root/Extension") as XmlElement;

            if (extension == null)
            {
                extension = doc.CreateElement("Extension");

                if (CEScore1.HasValue)
                {
                    XmlElement first = extension.OwnerDocument.CreateElement("Score");
                    first.SetAttribute("Sequence", "1");
                    first.InnerText = CEScore1.Value + "";

                    extension.AppendChild(first);
                }


                if (CEScore2.HasValue)
                {
                    XmlElement second = extension.OwnerDocument.CreateElement("Score");
                    second.SetAttribute("Sequence", "2");
                    second.InnerText = CEScore2.Value + "";

                    extension.AppendChild(second);
                }

                return extension.OuterXml;
            }
            else
            {
                XmlElement first = extension.SelectSingleNode("Score[@Sequence='1']") as XmlElement;
                XmlElement second = extension.SelectSingleNode("Score[@Sequence='2']") as XmlElement;

                if (CEScore1.HasValue)
                {
                    if (first == null)
                    {
                        first = extension.OwnerDocument.CreateElement("Score");
                        extension.AppendChild(first);
                    }

                    first.SetAttribute("Sequence", "1");
                    first.InnerText = CEScore1.Value + "";
                }
                else
                {
                    if (first != null)
                        extension.RemoveChild(first);
                }

                if (CEScore2.HasValue)
                {
                    if (second == null)
                    {
                        second = extension.OwnerDocument.CreateElement("Score");
                        extension.AppendChild(second);
                    }

                    second.SetAttribute("Sequence", "2");
                    second.InnerText = CEScore2.Value + "";
                }
                else
                {
                    if (second != null)
                        extension.RemoveChild(second);
                }

                return extension.OuterXml;
            }
        }
    }

    class ExamItem
    {
        private string _key;
        private string _value;

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
