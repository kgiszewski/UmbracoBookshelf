angular.module('umbraco').controller('UmbracoBookshelfCreateController', function ($scope, $routeParams, navigationService, $timeout, treeService, umbracoBookshelfService) {

    $scope.model = {};
    $scope.model.name = "";
    $scope.isCreating = false;
    $scope.model.isFolder = false;

    $scope.create = function () {
        $scope.isCreating = true;

        if (!$scope.model.isFolder) {
            umbracoBookshelfService.createFile("/" + $scope.currentNode.id + "/" + $scope.model.name).then(function(data) {
                $scope.isCreating = false;

                treeService.loadNodeChildren({ node: $scope.currentNode, section: $scope.section });
                navigationService.hideNavigation();
            });
        }
        else {
            umbracoBookshelfService.createFolder("/" + $scope.currentNode.id + "/" + $scope.model.name).then(function (data) {
                $scope.isCreating = false;

                treeService.loadNodeChildren({ node: $scope.currentNode, section: $scope.section });
                navigationService.hideNavigation();
            });
        }
    }
});
