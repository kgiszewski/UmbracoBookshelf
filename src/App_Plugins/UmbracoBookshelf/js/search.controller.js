angular.module('umbraco').controller('umbracoBookshelfSearchController', function ($scope, umbracoBookshelfResource) {

    $scope.model = {};
    $scope.model.keywords = "";

    $scope.model.books = [
        {
            name: "Learn Umbraco 7",
            results: [
                {
                    name: "04 - Surface, RenderMVC and API Controllers",
                    url: "#"
                },
                {
                    name: "06 - Blah Chapter",
                    url: "#"
                }
            ]
        },
        {
            name: "Official Umbraco Docs",
            results: [
                {
                    name: "Surface Controllers",
                    url: "#"
                },
                {
                    name: "API Controllers",
                    url: "#"
                }
            ]
        }
    ];
});