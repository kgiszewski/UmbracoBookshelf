angular.module('umbraco').controller('umbracoBookshelfSearchController', function($scope, umbracoBookshelfResource) {

    $scope.model = {};
    $scope.model.keywords = "";

    $scope.model.books = [];

    $scope.search = _.debounce(function() {
        if ($scope.model.keywords) {
            console.log('Calling server...' + $scope.model.keywords);
            umbracoBookshelfResource.searchFiles($scope.model.keywords).then(function(data) {
                console.log(data);
                $scope.model.books = data;
            });
        }
    },
    500);
});