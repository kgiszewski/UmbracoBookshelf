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
        }
    }
});