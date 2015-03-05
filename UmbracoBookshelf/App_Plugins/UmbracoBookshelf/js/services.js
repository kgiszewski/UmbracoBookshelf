angular.module('umbraco.services').factory('umbracoBookshelfService', function ($q, $http, umbRequestHelper) {
    return {
        saveFile: function (filePath, content) {
            return umbRequestHelper.resourcePromise(
                $http.post("/umbraco/backoffice/umbracobookshelfapi/umbracobookshelf/savefile/", {filePath: filePath, content: content}), 'Failed to save file contents'
            );
        }
    }
});