select course.id,course.course_name,teacher.teacher_name,tc_instruct.sequence, count(*) "stu_count"
from course join tc_instruct on course.id=tc_instruct.ref_course_id --需要老師姓名
	join teacher on teacher.id=tc_instruct.ref_teacher_id --需要老師姓名
	join sc_attend on sc_attend.ref_course_id=course.id --需要學生數
	join student on student.id=sc_attend.ref_student_id --需要「一般狀態」的學生
	join exam_template on course.ref_exam_template_id=exam_template.id --需要特定試別
	join te_include on exam_template.id=te_include.ref_exam_template_id --需要特定試別
	join tag_course on course.id=tag_course.ref_course_id --需要過慮「非協同教學」的課程
	join tag on tag.id = tag_course.ref_tag_id --需要過慮「非協同教學」的課程
where course.school_year='@@SchoolYear'
	and course.semester='@@Semester' 
	and student.status in (1,2)
	and te_include.ref_exam_id='@@ExamID'
	and tag.access_control_code='OneAdmin.CooperationExam'
group by course.id,course.course_name,teacher.teacher_name,tc_instruct.sequence
order by course.id,sequence