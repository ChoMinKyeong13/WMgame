<div align="center">

# 🍉 WMgame

**C# WinForms로 만든 수박게임 클론**

![C#](https://img.shields.io/badge/C%23-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![.NET 6](https://img.shields.io/badge/.NET%206-WinForms-512BD4?style=flat-square)
![GDI+](https://img.shields.io/badge/GDI+-2D%20그래픽-grey?style=flat-square)
![Personal](https://img.shields.io/badge/개인_프로젝트-blue?style=flat-square)

<img src="WMgame/Resources/_1.png" height="36"> → <img src="WMgame/Resources/_2.png" height="40"> → <img src="WMgame/Resources/_3.png" height="44"> → <img src="WMgame/Resources/_4.png" height="48"> → <img src="WMgame/Resources/_5.png" height="52"> → <img src="WMgame/Resources/_6.png" height="56"> → <img src="WMgame/Resources/_7.png" height="60"> → <img src="WMgame/Resources/_8.png" height="64"> → <img src="WMgame/Resources/_9.png" height="68"> → <img src="WMgame/Resources/_10.png" height="76">

</div>

## 📌 소개
같은 과일끼리 부딪히면 한 단계 큰 과일로 합쳐지는 '수박게임'입니다.
물리 엔진 없이 **중력 · 충돌 · 합체 로직을 직접 구현**했습니다.

## ✨ 구현 내용
- 🖱 마우스 클릭 위치에서 과일 낙하
- 🌍 중력 · 원형 충돌 · 벽/바닥 경계 처리
- 🔀 같은 단계 과일 충돌 시 **합체 → 다음 단계 생성**, 단계별 점수 가산
- 🎲 다음 과일은 낮은 단계 중 랜덤 생성
- ⚠️ 위험선 초과 상태가 일정 시간 지속되면 게임 오버, **최고 점수 기록**

## ▶️ 실행 방법
Visual Studio 2022에서 `WMgame.sln` 열고 `F5`
