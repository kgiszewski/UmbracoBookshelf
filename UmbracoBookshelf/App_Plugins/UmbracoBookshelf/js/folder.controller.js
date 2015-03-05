angular.module('umbraco').controller('UmbracoBookshelfFolderController', function ($scope, $http, $routeParams, umbracoBookshelfResource) {

    $scope.model = {};
    $scope.model.dirPath = decodeURIComponent($routeParams.id);
    $scope.model.content = "";

    umbracoBookshelfResource.getFolderContents($scope.model.dirPath).then(function (data) {
        $scope.model.content = data.Content;
    });
});