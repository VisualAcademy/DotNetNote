---
title: DocFX 빌드 및 배포
description: DotNetNote Markdown 문서의 자동 HTML 생성 및 ASP.NET Core 호스팅 방법
---

# DocFX 빌드 및 배포

DotNetNote 웹 프로젝트의 `DocFx.targets`는 AzuntSiteStarter의 DocFX 빌드 패턴을 적용합니다. 문서의 원본은 리포지토리 루트의 `docs/DotNetNote.Docs`에 보관합니다.

## 문서 수정 및 실행

1. 솔루션 탐색기의 `DotNetNote > Docs` 링크 폴더 또는 실제 `docs/DotNetNote.Docs` 폴더에서 Markdown 파일과 필요한 이미지를 수정합니다.
2. Visual Studio에서 `DotNetNote` 웹 프로젝트를 빌드하거나 F5 또는 Ctrl+F5로 실행합니다(실행 시 빌드 설정 사용).
3. `/docs/`를 열어 결과를 확인합니다.

명령줄에서 리포지토리 루트를 기준으로 실행할 수도 있습니다.

```powershell
dotnet build .\DotNetNote\DotNetNote\DotNetNote.csproj
dotnet run --project .\DotNetNote\DotNetNote\DotNetNote.csproj
```

DocFX 2.78.5는 .NET SDK의 `dotnet tool exec`로 자동 확보 및 실행합니다. 처음 빌드할 때는 NuGet 패키지 소스에 접속할 수 있어야 합니다. 생성된 파일은 `DotNetNote/DotNetNote/obj/DocFxSite`에 저장되며 Git에 포함하지 않습니다.

## 새 문서 작성

`dotnetnote/my-topic/index.md`를 작성하고 `navigation/toc.yml`에 `../dotnetnote/my-topic/index.md`를 등록합니다. 이미지는 해당 문서 폴더의 `images/`에 저장합니다. 그러면 `/docs/dotnetnote/my-topic/`으로 접근할 수 있습니다. DocFX에서 생성한 `index.html` 링크는 디렉터리 URL로 리디렉션됩니다.

## 게시

```powershell
dotnet publish .\DotNetNote\DotNetNote\DotNetNote.csproj -c Release
```

게시 결과의 `wwwroot/docs`에 HTML, 검색 인덱스, CSS, JavaScript 및 이미지가 함께 포함됩니다. 배포 서버에 DocFX를 따로 설치할 필요가 없습니다. 이전에 게시한 문서를 삭제하는 경우에는 깨끗한 게시 폴더로 배포합니다.

## 문서 빌드 생략

```powershell
dotnet build .\DotNetNote\DotNetNote\DotNetNote.csproj -p:BuildDocsOnBuild=false
```

빌드를 생략하면 새 문서는 생성되지 않습니다. 기존 로컬 생성 결과가 남아 있다면 이전 문서가 표시될 수 있습니다. `--no-build` 실행 및 실행 중 Markdown 변경에는 자동 빌드가 수행되지 않으므로 다시 빌드합니다.
