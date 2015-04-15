angular.module('umbraco').controller('UmbracoBookshelfImageSelectorController', function ($scope, $routeParams, umbracoBookshelfResource) {

    $scope.model = {};

    $scope.model.currentPath = decodeURIComponent($routeParams.id);

    umbracoBookshelfResource.getImages($scope.model.currentPath).then(function(data) {
        $scope.model.images = data;
    });

    $scope.select = function(index) {
        $scope.submit($scope.model.images[index]);
    }
});