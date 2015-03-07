angular.module('umbraco').controller('UmbracoBookshelfLibraryController', function ($scope, $http, $routeParams, umbracoBookshelfResource) {

    $scope.model = {};
    $scope.isDownloading = false;
    $scope.model.downloadUrl = "";
    $scope.model.feed = {};

    $scope.downloadUrl = function () {
        $scope.isDownloading = true;

        umbracoBookshelfResource.downloadUrl($scope.model.downloadUrl).then(function(data) {
            $scope.isDownloading = false;
            window.location.reload();
        });
    }

    $scope.populateDownloader = function (feedItem) {
        $scope.model.downloadUrl = "https://github.com/" + feedItem.user + "/" + feedItem.repo + "/archive/" + feedItem.branch + ".zip";
    }

    $scope.getBookItemAuthors = function (feedItem) {
        var authors = [];

        angular.forEach(feedItem.authors, function (author) {
            authors.push(author.login);
        });

        return authors.join(', ');
    }

    $scope.getProjectUrl = function (feedItem) {
        return "https://github.com/" + feedItem.user + "/" + feedItem.repo;
    }

    function init() {
        umbracoBookshelfResource.getBookFeed().then(function(data) {
            $scope.model.feed = data;
        }).then(function () {
            angular.forEach($scope.model.feed.gitHub, function(feedItem) {
                umbracoBookshelfResource.getContributors(feedItem).then(function(contributors) {
                    feedItem.authors = contributors;
                });
            });
        });
    }

    init();
});