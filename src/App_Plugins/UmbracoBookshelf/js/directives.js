angular.module('umbraco.directives').directive('umbracoBookshelfMarkedUp', function ($location, $routeParams) {

    //not sure why, but the urls are double encoded
    var url = decodeURIComponent($location.url());
    var isViewingFile = (url.indexOf('/file/') != -1);
    var pathToFileUrl = "/umbraco/#/UmbracoBookshelf/UmbracoBookshelfTree/file/";
    var pathToFolderUrl = "/umbraco/#/UmbracoBookshelf/UmbracoBookshelfTree/folder/";
    var pathToReader = (isViewingFile) ? pathToFileUrl : pathToFolderUrl;
    var pathOnFileSystem = decodeURIComponent($routeParams.id);
    var pathOnFileSystemSections = pathOnFileSystem.split('/');

    function getCurrentRelativePath() {
        var relativePathSections = [];

        var size = (isViewingFile) ? 2 : 1;

        for (var i = 1; i <= pathOnFileSystemSections.length - size; i++) {
            relativePathSections.push(pathOnFileSystemSections[i]);
        }

        var relativePath = relativePathSections.join('/');

        return "/" + relativePath + "/";
    }

    function getRootRelativePath() {
        return "/" + pathOnFileSystemSections[1] + "/" + pathOnFileSystemSections[2];
    }

    function isRelative($element, attribute) {
        return (!isExternal($element, attribute) && $element.attr(attribute).indexOf('/') != 0);
    }

    function isExternal($element, attribute) {
        return $element.attr(attribute).indexOf('http') == 0;
    }

    var linker = function (scope, element, attrs) {
        scope.$watch('model.content', function (newValue, oldValue) {
            if (newValue) {
                var markup = element.html(marked(newValue));

                /* adding global rule for external links */
                markup.find('a').each(function () {
                    var $a = $(this);
                    var relativePath = "";

                    if (isExternal($a, 'href')) {
                        $a.attr('target', '_blank');
                    } else {
                        if (isRelative($a, 'href')) {
                            //is relative to current
                            relativePath = getCurrentRelativePath();
                        } else {
                            //is relative to root
                            relativePath = getRootRelativePath();
                        }

                        var pathToLinkedFile = ($a.attr('href').indexOf(".md") != -1) ? pathToFileUrl : pathToFolderUrl;

                        $a.attr('href', pathToLinkedFile + encodeURIComponent(encodeURIComponent(relativePath + $a.attr('href'))));
                    }
                });

                /* fixup image relative paths */
                markup.find('img').each(function () {
                    var $img = $(this);
                    var relativePath = "";

                    if (isRelative($img, 'src')) {
                        //is relative to current
                        relativePath = getCurrentRelativePath();
                    } else {
                        //is relative to root
                        relativePath = getRootRelativePath();
                    }
                    $img.attr('src', relativePath + $img.attr('src'));
                });
            }
        }, true);
    }

    return {
        restrict: "A",
        link: linker
    }
})