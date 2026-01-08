CREATE TABLE [dbo].[Employees]
(
    [Id] INT NOT NULL PRIMARY KEY,


    -- Change of Information (정보 변경 사유 및 내역)
    [MaritalStatus] NVARCHAR(50) NULL, -- 혼인 상태
    [NewEmail] NVARCHAR(254) NULL, -- 변경 요청된 새 이메일
    [BadgeName] NVARCHAR(255) NULL, -- 배지에 표시할 이름
    [ReasonForChange] NVARCHAR(MAX) NULL, -- 정보 변경 사유
    [SpousesName] NVARCHAR(255) NULL, -- 배우자 이름
    [RoommateName1] NVARCHAR(255) NULL, -- 동거인 이름 1
    [RoommateName2] NVARCHAR(255) NULL, -- 동거인 이름 2
    [RelationshipDisclosureName] NVARCHAR(255) NULL, -- 관계 공개 대상 이름
    [RelationshipDisclosurePosition] NVARCHAR(255) NULL, -- 관계 공개 대상 직위
    [RelationshipDisclosure] NVARCHAR(MAX) NULL, -- 관계 공개 내용
    [AdditionalEmploymentBusinessName] NVARCHAR(255) NULL, -- 추가 근무 사업장명
    [AdditionalEmploymentStartDate] DATETIME2(7) NULL, -- 추가 근무 시작일
    [AdditionalEmploymentEndDate] DATETIME2(7) NULL, -- 추가 근무 종료일
    [AdditionalEmploymentLocation] NVARCHAR(255) NULL, -- 추가 근무 장소

)
