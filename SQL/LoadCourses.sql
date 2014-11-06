select course.id,course.course_name,teacher.teacher_name,tc_instruct.sequence, count(*) "stu_count"
from course join tc_instruct on course.id=tc_instruct.ref_course_id
	join teacher on teacher.id=tc_instruct.ref_teacher_id
	join sc_attend on sc_attend.ref_course_id=course.id
	join student on student.id=sc_attend.ref_student_id
where course.school_year=@@SchoolYear 
	and course.semester=@@Semester 
	and student.status in (1,2)
group by course.id,course.course_name,teacher.teacher_name,tc_instruct.sequence
order by course.id,sequence