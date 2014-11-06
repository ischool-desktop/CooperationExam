-- 參數：@@SchoolYear、@@Semester、@@ExamID、@@CourseIDs
select course.id "courseid",sce_take.extension
from course join sc_attend on course.id=sc_attend.ref_course_id
	join sce_take on sc_attend.id = sce_take.ref_sc_attend_id
where course.school_year=@@SchoolYear 
	and course.semester=@@Semester 
	and sce_take.ref_exam_id=@@ExamID
	and course.id in (@@CourseIDs)
order by course.id,sce_take.ref_exam_id
