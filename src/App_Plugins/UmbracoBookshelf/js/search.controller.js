angular.module('umbraco').controller('umbracoBookshelfSearchController', function($scope, umbracoBookshelfResource) {

    $scope.model = {};
    $scope.model.keywords = "";

    $scope.model.books = [];

    $scope.isLoading = false;

    $scope.search = function() {

        if ($scope.model.keywords && $scope.model.keywords.length > 2) {

            $scope.isLoading = true;

            umbracoBookshelfResource.searchFiles($scope.model.keywords).then(function(data) {
                $scope.model.books = data;
                $scope.isLoading = false;
            });
        }
    }

    $scope.clear = function() {
        $scope.model.keywords = "";
    }
});