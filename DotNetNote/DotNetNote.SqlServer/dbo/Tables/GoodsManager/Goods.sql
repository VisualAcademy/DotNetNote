--[0] 테이블: 제품 테이블 설계: 제품에 대한 소개(Goods, Products) 
Create Table dbo.Goods
(
	GoodsId				Int Identity(1, 1) Primary Key Not Null,	-- 일련번호
	GoodsName			NVarChar(100) Null,							-- 제품 이름
	GoodsDescription	NVarChar(Max) Null,							-- 제품 설명
	CreatedDate			DateTimeOffset Default(GetDate())			-- 제품 등록일
)
Go

----[1] 입력: 제품 등록
--Insert Into Goods (GoodsName, GoodsDescription) Values (N'좋은책', N'좋은책입니다.')
--Go
--Insert Into Goods (GoodsName, GoodsDescription) Values (N'좋은강의', N'좋은강의입니다.')
--Go
--Insert Into Goods (GoodsName, GoodsDescription) Values (N'좋은컴퓨터', N'좋은컴퓨터입니다.')
--Go

----[2] 출력: 제품 전체 조회
--Select * From Goods
--Go

----[3] 상세: 제품 상세 조회
--Select * From Goods Where GoodsId = 1 
--Go

----[4] 수정: 제품 수정
--Update Goods Set GoodsName = N'좋은북', GoodsDescription = N'좋은북입니다.' Where GoodsId = 1
--Go

----[5] 삭제: 제품 삭제
--Delete Goods Where GoodsId = 1
--Go

----[6] 검색: 제품 검색
--Select * From Goods Where GoodsName Like N'%좋은%'
--Go
