angular.module('umbraco').controller('UmbracoBookshelfFileController', function ($scope, $http, $routeParams, umbracoBookshelfResource, umbracoBookshelfService, notificationsService) {

    $scope.model = {};
    $scope.model.filePath = decodeURIComponent($routeParams.id);
    $scope.isEditing = false;
    $scope.isSaving = false;
    $scope.model.content = "";
    $scope.config = {};

    umbracoBookshelfResource.getConfig().then(function(data) {
        $scope.config = data;
    }).then(function() {
        umbracoBookshelfResource.getFileContents($scope.model.filePath).then(function(data) {
            $scope.model.content = data.Content;
        });
    });

    $scope.toggleEdit = function() {
        $scope.isEditing = !$scope.isEditing;
    }

    $scope.save = function () {
        $scope.isSaving = true;

        umbracoBookshelfService.saveFile($scope.model.filePath, $scope.model.content).then(function (data) {
            $scope.isSaving = false;

            notificationsService.success("Success", "The file has been saved.");
        });
    }
});
