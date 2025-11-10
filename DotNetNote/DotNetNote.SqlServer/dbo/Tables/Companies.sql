-- ==========================================
-- Table: Companies
-- 회사 정보 저장용 테이블
-- ==========================================
CREATE TABLE [dbo].[Companies]
(
	[Id] BIGINT NOT NULL PRIMARY KEY Identity(1, 1), -- 고유 ID (자동 증가)
	[Name] NVarChar(255) Not Null                    -- 회사 이름
)
GO
