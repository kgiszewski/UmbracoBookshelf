angular.module('umbraco').service('umbracoBookshelfInterceptor', function ($location, $templateCache) {
    var service = this;

    service.request = function (request) {
        if (request.url == "views/directives/umb-navigation.html") {
            request.url = "/App_Plugins/UmbracoBookshelf/Views/navigation.router.html";
        }

        return request;
    };
});

angular.module('umbraco').config(function ($httpProvider) {
    $httpProvider.interceptors.push('umbracoBookshelfInterceptor');
});
