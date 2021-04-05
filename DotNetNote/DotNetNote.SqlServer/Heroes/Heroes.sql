--[!] 영웅 캐릭터 관리 앱

--[1] 테이블 생성
Create Table dbo.Heroes
(
    Id Int Identity(1, 1) Primary Key Not Null,     -- 기본키, 일련번호
    Name NVarChar(50) Not Null,                     -- 캐릭터 이름
    Icon NVarChar(255) Null,                        -- 캐릭터 이미지(아이콘)
    Created DateTimeOffset(7) Default(GetDate())    -- 생성일
)
Go

----[2] 예시문 작성
---- 입력
--Insert Into Heroes (Name, Icon) Values (N'홍길동', N'h.png');
--Insert Into Heroes (Name, Icon) Values (N'백두산', N'b.png');
--Insert Into Heroes (Name, Icon) Values (N'임꺽정', N'i.png');
---- 출력
--Select * From Heroes;
---- 상세 보기
--Select Id, Name, Icon, Created From Heroes Where Id = 1;
---- 수정
--Update Heroes Set Icon = N'p.png' Where Name = N'백두산';
---- 삭제
--Delete From Heroes Where Id = 3; 
---- 검색
--Select Id, Name, Icon From Heroes Where Icon Like N'%p.png%';
