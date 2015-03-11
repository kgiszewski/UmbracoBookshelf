angular.module('umbraco').controller('UmbracoBookshelfDeleteController', function ($scope, navigationService, treeService, umbracoBookshelfService) {

    $scope.performDelete = function () {

        //mark it for deletion (used in the UI)
        $scope.currentNode.loading = true;

        umbracoBookshelfService.delete($scope.currentNode.id).then(function (data) {
            $scope.currentNode.loading = false;

            treeService.removeNode($scope.currentNode);
            navigationService.hideDialog();
        });
    };

    $scope.cancel = function () {
        navigationService.hideDialog();
    };
});
