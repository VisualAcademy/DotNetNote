(function () {
    'use strict';

    //[!] 모듈 생성: 'WebCampApp' 이름으로 모듈 생성
    var WebCampApp = angular.module('WebCampApp', [
        // Angular modules
        'ngAnimate',
        'ngRoute'

        // Custom modules

        // 3rd Party Modules
        
    ]);

    //[!] Angular 라우트 설정
    WebCampApp.config(['$routeProvider', function ($routeProvider) {
        $routeProvider.
            when('/SpeakerList', {
                templateUrl: '/app/WebCamp/Templates/SpeakerList.html',
                controller: 'WebCampListController'
            }).
            when('/SpeakerDetails/:id', {
                templateUrl: '/app/WebCamp/Templates/SpeakerDetails.html',
                controller: 'WebCampDetailsController'
            }).
            otherwise({
                redirectTo: '/SpeakerList'
            });
    }]);
})();
