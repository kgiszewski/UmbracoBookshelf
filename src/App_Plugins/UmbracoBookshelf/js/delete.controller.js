angular.module('umbraco').controller('UmbracoBookshelfDeleteController', function ($scope, navigationService, umbracoBookshelfService) {

    $scope.performDelete = function () {

        //mark it for deletion (used in the UI)
        $scope.currentNode.loading = true;

        umbracoBookshelfService.delete($scope.currentNode.id).then(function (data) {
            $scope.currentNode.loading = false;

            window.location.reload();
        });
    };

    $scope.cancel = function () {
        navigationService.hideDialog();
    };
});
