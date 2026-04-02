-- 예약 종류 관리 테이블
-- NOTE:
-- 'AppointmentsTypes' 테이블명은 문법적으로는 다소 어색할 수 있으나,
-- SQL Server의 테이블 목록에서 'Appointments' 테이블 바로 아래에 정렬되어
-- 관련 테이블임을 쉽게 식별할 수 있도록 의도적으로 사용한 이름입니다.
CREATE TABLE [dbo].[AppointmentsTypes]
(
    [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [AppointmentTypeName] NVARCHAR(50) NOT NULL,
    [IsActive] BIT NOT NULL CONSTRAINT [DF_AppointmentsTypes_IsActive] DEFAULT ((1)),
    [DateCreated] DATETIME NOT NULL CONSTRAINT [DF_AppointmentsTypes_DateCreated] DEFAULT (GETDATE())
);