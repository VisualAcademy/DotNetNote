(function () {
    'use strict';

    angular
        .module('WebCampApp')
        .controller('WebCampController', WebCampController)
        .controller('WebCampListController', WebCampListController)
        .controller('WebCampDetailsController', WebCampDetailsController);

    WebCampController.$inject = ['$scope'];
    WebCampListController.$inject = ['$scope', '$http'];
    WebCampDetailsController.$inject = ['$scope', '$routeParams', '$http'];

    // 기본 컨트롤러: 제목만 표시
    function WebCampController($scope) {
        $scope.title = 'WebCamp 2019';

        activate();

        function activate() { }
    }

    // 리스트 컨트롤러: 스피커 리스트 출력
    function WebCampListController($scope, $http) {
        $scope.title = 'Speaker List';

        $http.get('/api/WebCampService').success(function (data) {
            $scope.speakers = data; 
        });

        activate();

        function activate() { }
    }

    // 상세보기 컨트롤러: 스피커 상세보기 출력
    function WebCampDetailsController($scope, $routeParams, $http) {
        $scope.title = 'Speaker Details';

        $http.get('/api/WebCampService').success(function (data) {
            $scope.speakers = data;

            // 모듈(리스트)에서 상세보기로 #/SpeaekrDetails/0 식으로 전달
            $scope.id = $routeParams.id;

            // 페이저 이전/다음 구현
            if ($routeParams.id > 0) {
                $scope.prev = Number($routeParams.id) - 1;
            }
            else {
                $scope.prev = $scope.speakers.length - 1; // 마지막으로 이동
            }

            if ($routeParams.id < $scope.speakers.length - 1) {
                $scope.next = Number($routeParams.id) + 1;
            }
            else {
                $scope.next = 0; // 처음으로 이동
            }
        });
        
        activate();

        function activate() { }
    }
})();
