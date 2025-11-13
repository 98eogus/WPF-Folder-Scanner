📁 WPF Folder Scanner

WPF 기반 폴더 스캔 및 파일 확장자 통계 분석 도구

📌 개요 (Overview)

WPF Folder Scanner는 선택한 폴더 전체를 재귀적으로 스캔하여,
확장자별로 다음 정보를 분석해 주는 데스크톱 애플리케이션입니다:

📄 파일 수

📦 총 용량(bytes)

🔍 사람이 읽기 쉬운 형태의 용량(KB, MB, GB)

⚙ 실시간 진행률 표시 (ProgressBar)

⏹ 취소 기능(CancellationToken 기반)

MVVM 패턴을 적용하여 View, ViewModel, Service 계층을 분리하였으며
비동기 프로그래밍(async/await)을 통해 UI 멈춤 현상을 방지합니다.

✨ 주요 기능 (Features)
✔ 폴더 전체 스캔 (재귀 탐색)

Directory.EnumerateFiles() + EnumerationOptions 를 이용하여
하위 폴더까지 빠르고 안정적으로 검색합니다.

✔ 확장자별 파일 통계

Service 계층에서 확장자 기준으로 다음 정보를 계산합니다:

파일 개수

전체 파일 크기(TotalBytes)

자동 변환된 용량(KB, MB, GB)

✔ 실시간 진행률 표시

IProgress<long>를 사용하여 Service에서 ViewModel로
진행된 파일 수를 실시간 전달합니다.

✔ 취소 기능

스캔 도중 언제든지 "취소" 버튼을 누르면
CancellationTokenSource.Cancel()로 즉시 작업을 중단합니다.

✔ MVVM 아키텍처

View (XAML) : UI

ViewModel : Command + Binding + 상태 관리

Service : 파일 스캔/통계 로직

Model : 확장자별 데이터 구조

🏗 기술 스택 (Tech Stack)

WPF (.NET 8)

C# / XAML

MVVM 패턴 적용

async / await 비동기 프로그래밍

IProgress<T> 기반 UI 업데이트

CancellationToken 기반 취소 처리

📂 프로젝트 구조 (Project Structure)
WpfFolderScanner/
│
├── Model/
│   └── FileModel.cs          # 확장자별 데이터 모델
│
├── Service/
│   └── FileService.cs        # 폴더 스캔 + 통계 계산
│
├── ViewModel/
│   └── MainViewModel.cs      # UI 상태/커맨드/진행률
│
├── View/
│   └── MainWindow.xaml       # 사용자 인터페이스
│   └── MainWindow.xaml.cs
│
├── RelayCommand.cs           # MVVM 커맨드 구현
├── WpfApp4.csproj
└── README.md

🚀 실행 방법 (How to Run)
1) 프로젝트 클론
git clone https://github.com/98eogus/WPF-Folder-Scanner.git
cd WPF-Folder-Scanner

2) Visual Studio로 열기

WpfApp4.sln 더블 클릭
또는

.csproj 파일 직접 열기

3) NuGet 패키지 복원

Visual Studio가 자동 복원합니다.

4) 실행 (F5)
🖼 스크린샷 (Screenshots)

(원하면 이미지 추가하면 됨)

[ 폴더 선택 ] [ 스캔 ] [ 취소 ]
┌───────────────────────────────┐
│ ProgressBar 진행률 표시       │
└───────────────────────────────┘

확장자별 통계가 DataGrid로 표시됨

📜 라이선스 (License)

MIT License
