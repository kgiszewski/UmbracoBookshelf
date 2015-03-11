angular.module('umbraco.services').factory('umbracoBookshelfService', function ($q, $http, umbRequestHelper) {
    return {
        saveFile: function (filePath, content) {
            return umbRequestHelper.resourcePromise(
                $http.post("/umbraco/backoffice/umbracobookshelfapi/umbracobookshelf/savefile/", {filePath: filePath, content: content}), 'Failed to save file contents'
            );
        },
        delete: function (path) {
            return umbRequestHelper.resourcePromise(
                $http.post("/umbraco/backoffice/umbracobookshelfapi/umbracobookshelf/delete/", { path: path }), 'Failed to delete path'
            );
        },
        createFile: function (path) {
            return umbRequestHelper.resourcePromise(
                $http.post("/umbraco/backoffice/umbracobookshelfapi/umbracobookshelf/createFile/", { filePath: path }), 'Failed to create file'
            );
        },
        createFolder: function (path) {
            return umbRequestHelper.resourcePromise(
                $http.post("/umbraco/backoffice/umbracobookshelfapi/umbracobookshelf/createFolder/", { path: path }), 'Failed to create file'
            );
        },
        rename: function (sourcePath, newName, isFolder) {
            return umbRequestHelper.resourcePromise(
                $http.post("/umbraco/backoffice/umbracobookshelfapi/umbracobookshelf/rename/", { sourcePath: sourcePath, newName: newName, isFolder: isFolder }), 'Failed to create file'
            );
        }
    }
});