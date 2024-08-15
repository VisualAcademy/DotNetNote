-- 샘플 테이블 및 데이터 생성
CREATE TABLE #Employees (
    ID bigint PRIMARY KEY,
    Name nvarchar(100)
);

CREATE TABLE #Attachments (
    ID bigint PRIMARY KEY IDENTITY(1,1),
    DateCreated datetimeoffset(7) NOT NULL,
    FileName nvarchar(max) NULL,
    EmployeeID bigint NULL
);

-- 샘플 데이터 삽입
INSERT INTO #Employees (ID, Name)
VALUES (1, 'John Doe'),
       (2, 'Jane Smith');

INSERT INTO #Attachments (DateCreated, FileName, EmployeeID)
VALUES (SYSDATETIMEOFFSET(), 'document_CHRI.pdf', 1),
       (SYSDATETIMEOFFSET(), 'report_CHRI MOU Compliance.pdf', 1),
       (SYSDATETIMEOFFSET(), 'summary_CHRI.pdf', 2),
       (SYSDATETIMEOFFSET(), 'invoice_CHR.pdf', 2),
       (SYSDATETIMEOFFSET(), 'FinancialReport-CompanyXYZ-EMAIL_2023.pdf', 2),  
       (SYSDATETIMEOFFSET(), 'summary_Alexander.pdf', 1),                      
       (SYSDATETIMEOFFSET(), 'John Doe-Financial-Report-202406171551.pdf', 1);  

-- 트랜잭션 시작
BEGIN TRANSACTION;

-- 데이터 복사
SELECT A.*
INTO #Attachments_Delete_CHRI
FROM #Attachments A
LEFT JOIN #Employees E ON A.EmployeeID = E.ID
WHERE ((A.FileName COLLATE SQL_Latin1_General_CP1_CS_AS LIKE '%CHRI%.pdf'
         OR A.FileName COLLATE SQL_Latin1_General_CP1_CS_AS LIKE '%CHR%.pdf')
        OR A.FileName LIKE '%chri.pdf'
        OR A.FileName LIKE '% chri %.pdf'
        OR A.FileName LIKE '%chri-%.pdf')
    AND A.FileName LIKE '%.pdf'
    AND A.FileName COLLATE SQL_Latin1_General_CP1_CS_AS NOT LIKE '%CHRI MOU Compliance%'
    AND A.FileName COLLATE SQL_Latin1_General_CP1_CS_AS NOT LIKE '%FinancialReport-CompanyXYZ-EMAIL_2023%'
    AND A.FileName COLLATE SQL_Latin1_General_CP1_CS_AS NOT LIKE '%John Doe-Financial-Report-202406171551%'
    AND A.FileName COLLATE SQL_Latin1_General_CP1_CS_AS NOT LIKE '%Alexander%';

-- 데이터 삭제
DELETE A
FROM #Attachments A
LEFT JOIN #Employees E ON A.EmployeeID = E.ID
WHERE ((A.FileName COLLATE SQL_Latin1_General_CP1_CS_AS LIKE '%CHRI%.pdf'
         OR A.FileName COLLATE SQL_Latin1_General_CP1_CS_AS LIKE '%CHR%.pdf')
        OR A.FileName LIKE '%chri.pdf'
        OR A.FileName LIKE '% chri %.pdf'
        OR A.FileName LIKE '%chri-%.pdf')
    AND A.FileName LIKE '%.pdf'
    AND A.FileName COLLATE SQL_Latin1_General_CP1_CS_AS NOT LIKE '%CHRI MOU Compliance%'
    AND A.FileName COLLATE SQL_Latin1_General_CP1_CS_AS NOT LIKE '%FinancialReport-CompanyXYZ-EMAIL_2023%'
    AND A.FileName COLLATE SQL_Latin1_General_CP1_CS_AS NOT LIKE '%John Doe-Financial-Report-202406171551%'
    AND A.FileName COLLATE SQL_Latin1_General_CP1_CS_AS NOT LIKE '%Alexander%';

-- 모든 작업이 정상적으로 완료되면 커밋
COMMIT TRANSACTION;
