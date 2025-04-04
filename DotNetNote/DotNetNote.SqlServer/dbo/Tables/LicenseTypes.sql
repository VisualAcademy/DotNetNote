CREATE TABLE [dbo].[LicenseTypes] (
    [ID]                    BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY, -- 고유 ID (자동 증가)
    [Active]                BIT NULL,                                  -- 활성 여부
    [CreatedAt]             DATETIMEOFFSET(7) NULL,                    -- 생성 일시
    [CreatedBy]             NVARCHAR(70) NULL,                         -- 작성자
    [Type]                  NVARCHAR(450) NULL,                        -- 라이선스 유형
    [Description]           NVARCHAR(MAX) NULL,                        -- 설명
    [ApplicantType]         INT NULL,                                  -- 신청자 유형
    [BgRequired]            BIT NULL,                                  -- 백그라운드 체크 필요 여부
    [IsApplicationRequired] BIT NOT NULL DEFAULT 1,                    -- 신청서 필요 여부 (기본값: 체크됨)
    [IsCertificateRequired] BIT NULL DEFAULT 0
);
