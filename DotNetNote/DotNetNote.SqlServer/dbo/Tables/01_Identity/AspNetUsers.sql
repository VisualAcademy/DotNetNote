-- ASP.NET Core Identity의 사용자 테이블인 AspNetUsers의 스키마입니다.
-- 다음은 테이블에 추가된 필드인 Address를 확인할 수 있습니다.

-- ASP.NET Core Identity의 사용자 인증과 권한을 관리하는 테이블입니다.
CREATE TABLE [dbo].[AspNetUsers] (
    [Id]                   NVARCHAR (450)     NOT NULL,                      -- 사용자의 고유 식별자
    [UserName]             NVARCHAR (256)     NULL,                          -- 사용자 이름
    [NormalizedUserName]   NVARCHAR (256)     NULL,                          -- 정규화된 사용자 이름
    [Email]                NVARCHAR (256)     NULL,                          -- 이메일 주소
    [NormalizedEmail]      NVARCHAR (256)     NULL,                          -- 정규화된 이메일 주소
    [EmailConfirmed]       BIT                NOT NULL,                      -- 이메일 확인 여부
    [PasswordHash]         NVARCHAR (MAX)     NULL,                          -- 비밀번호 해시값
    [SecurityStamp]        NVARCHAR (MAX)     NULL,                          -- 보안 스탬프
    [ConcurrencyStamp]     NVARCHAR (MAX)     NULL,                          -- 동시성 스탬프
    [PhoneNumber]          NVARCHAR (256)     NULL,                          -- 전화번호
    [PhoneNumberConfirmed] BIT                NOT NULL,                      -- 전화번호 확인 여부
    [TwoFactorEnabled]     BIT                NOT NULL,                      -- 이중 인증 활성화 여부
    [LockoutEnd]           DATETIMEOFFSET (7) NULL,                          -- 계정 잠금 해제 일시
    [LockoutEnabled]       BIT                NOT NULL,                      -- 계정 잠금 활성화 여부
    [AccessFailedCount]    INT                NOT NULL,                      -- 계정 접근 실패 횟수
    [Address]              NVARCHAR (MAX)     NULL,                          -- 주소 (추가된 필드)
	[FirstName] [nvarchar](max) NULL,
	[LastName] [nvarchar](max) NULL,
	[Timezone] [nvarchar](max) NULL,

    [TenantName]               NVARCHAR (MAX)     Default('Azunt'),

    RegistrationDate DATETIMEOFFSET  NULL DEFAULT (SYSDATETIMEOFFSET()),

    [ShowInDropdown] BIT NULL DEFAULT 0, 

    RefreshToken NVARCHAR(MAX) NULL,

    RefreshTokenExpiryTime DATETIME NULL, 

    DivisionId BigInt Null Default 0, 

    [TenantId]             BIGINT             DEFAULT (CONVERT([bigint],(0))) NOT NULL,

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

    -- Profile Picture(PFP, Persona Avatar)
    [ProfilePicture]          VARBINARY(MAX)    NULL,

    -- Signature image (user handwritten signature, JPG binary)
    [SignatureImage] VARBINARY(MAX) NULL,

    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)             -- 기본키 설정
);


GO
CREATE NONCLUSTERED INDEX [EmailIndex]
    ON [dbo].[AspNetUsers]([NormalizedEmail] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex]
    ON [dbo].[AspNetUsers]([NormalizedUserName] ASC) WHERE ([NormalizedUserName] IS NOT NULL);
