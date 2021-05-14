# DB-mini-Project with WinForm
KOSTA Weeks 5 mini Project
# 
HTML File , Csv File을 솔루션파일 > bin > 디버그에 넣어야 실행이 가능합니다.

* Mission: 공공데이터 포털의 데이터(csv)를 이용해 검색 필터를 만들고, 검색 결과를 리스트와 지도를 통해 확인하는 프로그램
* 결과 화면
![image](https://user-images.githubusercontent.com/78033158/113406314-3f60c600-93e6-11eb-9189-e3dd3d23e3ab.png)
* 기능
  * Data Processing 
    * Data preprocessing: 파일 내의 데이터가 정확하지 않은 것, 쉼표가 있는 주소 필드 맨뒤로
    * split(“,”)로 파일 읽기
  * Create Filter
    * 검색 조건이 될 컬럼에 대해 조건 값 리스트 추출 및 버튼 생성
    → “구”컬럼 값에 따라 “동”컬럼의 조건 값 리스트가 달라짐 (ex)분당구 → 구미동, 정자동 등
    ⇒ “구”조건 선택 시, “동”조건 값 동적으로 나타나도록
  * Search
    * 선택된 조건에 맞는 데이터 찾기
  * Plot Map
    * 결과 데이터를 List Box에 출력 및 지도에 표시
    * List Box의 데이터 선택 시, 해당 데이터(사용처)의 위치로 지도 이동
* Flow Chart
![image](https://user-images.githubusercontent.com/78033158/113406628-c31ab280-93e6-11eb-82d5-31fefa8a8048.png)

* 진행 과정에서 이슈 사항
    * 공공데이터 포털에서 주어진 데이터가 정확하지 않아 별도의 전처리과정 필요했음
    → 데이터에 대한 자세한 분석과정 중요
    * csv파일을 쉼표(“,”)로 split할 때, 데이터값으로 쉼표가 있어, 인덱스를 통해 컬럼값 특정짓기 어려웠음
    ⇒ 쉼표 있는 컬럼은 1개여서 해당 컬럼을 가장 마지막으로 바꿈
    → 이것도 데이터 분석 간과한 것, 어떤 칼럼이 있고, 각각마다 어떤 값의 형태인지
    * 사용한 카카오의 로컬 API에서 제공하는 기능들을 보고, 더 부합하는 기능이 있음을 간과?
