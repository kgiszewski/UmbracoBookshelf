angular.module('umbraco').controller('UmbracoBookshelfFolderController', function ($scope, $http, $routeParams, umbracoBookshelfResource) {

    $scope.model = {};
    $scope.model.dirPath = decodeURIComponent($routeParams.id);
    $scope.model.content = "";
    $scope.config = {};

    umbracoBookshelfResource.getConfig().then(function(data) {
        $scope.config = data;
    }).then(function() {
        umbracoBookshelfResource.getFolderContents($scope.model.dirPath).then(function (data) {
            $scope.model.content = data.Content;
        });
    });
});