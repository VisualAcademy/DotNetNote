-- ===================================================================
-- DotNetSale 쇼핑몰 프로젝트
-- (http://www.VisualAcademy.com/)
-- ===================================================================

-- ===================================================================
-- Categories 테이블 생성
-- ===================================================================

--[1] 카테고리: 상품분류
Create Table dbo.Categories
(
    CategoryId Int Identity(1, 1) Not Null Primary Key,		-- 카테고리번호
    CategoryName NVarChar(50),								-- 카테고리명
    --
    SuperCategory Int Null									-- 부모카테고리번호(확장용) : ParentId, ParentCategoryId로 이름 변경해도 무관
        References Categories(CategoryId),
    Align SmallInt Default(0)								-- 카테고리보여지는순서(확장용)
)
Go

--[!] 예시 데이터 입력
-- 아래 카테고리를 표현하고자 한다면???
--	-책
--	-강의
--	-컴퓨터
--		-노트북
--			-삼성
--			-LG
--		-데스크톱

-- 대분류만 사용한다면... SuperCategory가 Null이면, 최상위 분류, 그렇지 않으면 하위 분류
-- -- 또 다른 방법은 SuperCategory에 -1값을 넣고 구분(프로그램 작성시 훨씬 편함)
--Insert Into Categories Values('책', Null, 1)
--Go
--Insert Into Categories Values('강의', Null, 2)
--Go
--Insert Into Categories Values('컴퓨터', Null, 3)
--Go

--Select * From Categories
--Go

 --대중소 분류로 확장한다면...
--Insert Into Categories Values('노트북', 3, 1)
--Go
--Insert Into Categories Values('데스크톱', 3, 2)
--Go
--Insert Into Categories Values('삼성', 4, 1)
--Go
--Insert Into Categories Values('LG', 4, 2)
--Go

---- 전체 출력 예시
--Select * From Categories Order By CategoryId Desc
--Go

---- 대분류만 출력 예시
--Select CategoryId, CategoryName From Categories Where SuperCategory Is Null 
--Order By Align Asc
--Go

---- 현재 카테고리의 하위 카테고리 목록을 출력하는 구문
--Declare @SuperCategory Int
--Set @SuperCategory = 3 -- 3번 카테고리의 자식 카테고리 리스트
--Select CategoryId, CategoryName From Categories 
--Where SuperCategory = @SuperCategory
--Go

