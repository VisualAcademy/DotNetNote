-- Courses.sql
-- 과목 테이블
Create Table dbo.Courses
(
	CourseId Int Identity(1, 1) Not Null Primary Key,	-- 일련번호
	CourseName NVarChar(25) Not Null					-- 과목이름
)
Go

---- 입력
--Insert Into Courses (CourseName) Values (N'C#')
--Go

---- 출력
--Select * From Courses Order By CourseId Asc
--Go
