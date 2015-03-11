angular.module('umbraco').controller('UmbracoBookshelfMoveController', function ($scope, $routeParams, navigationService, $timeout, treeService, umbracoBookshelfService) {

    var path = $scope.currentNode.id;

    $scope.model = {};
    $scope.isRenaming = false;
    $scope.model.isFolder = (path.indexOf('.md') == -1);
    $scope.model.name = path.split('/').pop();

    if (!$scope.model.isFolder) {
        $scope.model.name = $scope.model.name.split('.').shift();
    }

    $scope.rename = function () {
        $scope.isRenaming = true;

        umbracoBookshelfService.rename(path, $scope.model.name, $scope.model.isFolder).then(function (data) {
            $scope.isRenaming = false;

            treeService.loadNodeChildren({ node: $scope.currentNode.parent(), section: $scope.section });
            navigationService.hideNavigation();
        });
    }
});
