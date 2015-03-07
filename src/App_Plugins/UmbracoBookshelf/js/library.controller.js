angular.module('umbraco').controller('UmbracoBookshelfLibraryController', function ($scope, $http, $routeParams, umbracoBookshelfResource) {

    $scope.model = {};
    $scope.isDownloading = false;
    $scope.model.downloadUrl = "https://github.com/umbraco/Umbraco4Docs/archive/master.zip";

    $scope.downloadUrl = function () {
        $scope.isDownloading = true;

        umbracoBookshelfResource.downloadUrl($scope.model.downloadUrl).then(function(data) {
            $scope.isDownloading = false;
            window.location.reload();
        });
    }
});