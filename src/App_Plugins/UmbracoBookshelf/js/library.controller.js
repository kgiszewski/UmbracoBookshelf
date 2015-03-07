angular.module('umbraco').controller('UmbracoBookshelfLibraryController', function ($scope, $http, $routeParams, umbracoBookshelfResource) {

    $scope.model = {};
    $scope.isDownloading = false;
    $scope.model.downloadUrl = "";
    $scope.model.feed = {};

    $scope.downloadUrl = function () {
        $scope.isDownloading = true;

        umbracoBookshelfResource.downloadUrl($scope.model.downloadUrl).then(function(data) {
            $scope.isDownloading = false;
            window.location.reload();
        });
    }

    $scope.populateDownloader = function (url) {
        $scope.model.downloadUrl = url;
    }

    function init() {
        umbracoBookshelfResource.getBookFeed().then(function(data) {
            $scope.model.feed = data;
        });
    }

    init();
});