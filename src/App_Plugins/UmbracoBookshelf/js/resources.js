angular.module('umbraco.resources').factory('umbracoBookshelfResource', function ($q, $http, umbRequestHelper) {
    return {
        getFileContents: function(filePath) {
            return umbRequestHelper.resourcePromise(
                $http.get("/umbraco/backoffice/umbracobookshelfapi/umbracobookshelf/getfilecontents/?filepath=" + filePath), 'Failed to retrieve file contents'
            );
        },
        getFolderContents: function (dirPath) {
            return umbRequestHelper.resourcePromise(
                $http.get("/umbraco/backoffice/umbracobookshelfapi/umbracobookshelf/getfoldercontents/?dirpath=" + dirPath), 'Failed to retrieve folder contents'
            );
        },
        downloadUrl: function(downloadUrl) {
            return umbRequestHelper.resourcePromise(
                $http.get("/umbraco/backoffice/umbracobookshelfapi/umbracobookshelf/downloadurl/?url=" + encodeURIComponent(downloadUrl)), 'Failed to download url'
            );
        },
        getBookFeed: function() {
            return umbRequestHelper.resourcePromise(
                $http.get("/umbraco/backoffice/umbracobookshelfapi/umbracobookshelf/getbookfeed/"), 'Failed to get book feed'
            );
        },
        getContributors: function (feedItem) {
            return umbRequestHelper.resourcePromise(
                $http.get("https://api.github.com/repos/" + feedItem.user + "/" + feedItem.repo + "/contributors"), 'Failed to get contributors'
            );
        },
        getRepoDetails: function (feedItem) {
            return umbRequestHelper.resourcePromise(
                $http.get("https://api.github.com/repos/" + feedItem.user + "/" + feedItem.repo ), 'Failed to get repo details'
            );
        },
        getConfig: function () {
            return umbRequestHelper.resourcePromise(
                $http.get("/umbraco/backoffice/umbracobookshelfapi/umbracobookshelf/getconfig/"), 'Failed to get config'
            );
        },
        getImages: function(currentPath) {
            return umbRequestHelper.resourcePromise(
                $http.get("/umbraco/backoffice/umbracobookshelfapi/umbracobookshelf/getimages/?currentPath=" + currentPath), 'Failed to get images'
            );
        }
    }
});