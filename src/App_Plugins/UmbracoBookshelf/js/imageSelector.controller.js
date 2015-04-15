angular.module('umbraco').controller('UmbracoBookshelfImageSelectorController', function ($scope, $routeParams) {

    $scope.model = {};

    $scope.model.filePath = decodeURIComponent($routeParams.id);

    console.log($scope.model.filePath);

    $scope.model.images = [{ filePath: "/Books/BizmagManual/assets/codegarden.png", alt: "foo" }, { filePath: "/Books/BizmagManual/assets/codegarden.png", alt: "foo" }, { filePath: "/Books/BizmagManual/assets/codegarden.png", alt: "foo" }, { filePath: "/Books/BizmagManual/assets/codegarden.png", alt: "foo" }];

    $scope.select = function(index) {
        $scope.submit($scope.model.images[index]);
    }
});