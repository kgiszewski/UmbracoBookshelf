angular.module('umbraco.directives').directive('umbracoBookshelfMarkedUp', function($location, $routeParams) {

    function getCurrentRelativePath(isViewingFile, pathOnFileSystemSections) {
        var relativePathSections = [];

        var size = (isViewingFile) ? 2 : 1;

        for (var i = 1; i <= pathOnFileSystemSections.length - size; i++) {
            relativePathSections.push(pathOnFileSystemSections[i]);
        }

        var relativePath = relativePathSections.join('/');

        return "/" + relativePath + "/";
    }

    function getRootRelativePath(pathOnFileSystemSections) {
        return "/" + pathOnFileSystemSections[1] + "/" + pathOnFileSystemSections[2];
    }

    function inArray(array, item) {
        var foundItem = _.find(array, function(arrayItem) { return arrayItem == item; });
        return (foundItem != undefined);
    }

    function isRelative($element, attribute) {
        return (!isExternal($element, attribute) && $element.attr(attribute).indexOf('/') != 0);
    }

    function isExternal($element, attribute) {
        return $element.attr(attribute).indexOf('http') == 0;
    }

    function removeQueryString(input) {
        var qMarkIndex = input.indexOf('?');

        if (qMarkIndex == -1) {
            return input;
        }

        return input.substring(0, qMarkIndex);
    }

    var linker = function (scope, element, attrs) {
        marked.setOptions({
            highlight: function(code) {
                hljs.initHighlightingOnLoad();
                return hljs.highlightAuto(code).value;
            }
        });

        scope.$watch('model.content', function (newValue, oldValue) {
            //not sure why, but the urls are double encoded
            var url = decodeURIComponent($location.url());
            var isViewingFile = (url.indexOf('/file/') != -1);
            var pathToFileUrl = "/umbraco/#/UmbracoBookshelf/UmbracoBookshelfTree/file/";
            var pathToFolderUrl = "/umbraco/#/UmbracoBookshelf/UmbracoBookshelfTree/folder/";
            var pathToMediaUrl = "/umbraco/umbracobookshelfapi/umbracobookshelfmedia/getmedia/?filePath=";
            var pathOnFileSystem = decodeURIComponent($routeParams.id);
            var pathOnFileSystemSections = pathOnFileSystem.split('/');

            if (newValue) {
                var markup = element.html(marked(newValue));

                /* handle links */
                markup.find('a').each(function () {
                    var $a = $(this);
                    var href = $a.attr('href');
                    var relativePath = "";

                    if (isExternal($a, 'href')) {
                        $a.attr('target', '_blank');
                    } else if (href.indexOf("/umbraco/#/UmbracoBookshelf") == 0) {
                        //allow as-is
                    } else {
                        if (isRelative($a, 'href')) {
                            //is relative to current
                            relativePath = getCurrentRelativePath(isViewingFile, pathOnFileSystemSections);
                        } else {
                            //is relative to root
                            relativePath = getRootRelativePath(pathOnFileSystemSections);
                        }

                        var extension = href.split('.').pop();

                        //test for media downloads
                        if (extension != 'md' && inArray(scope.config.fileExtensions, "." + extension)) {
                            $a.attr('href', relativePath + href);
                            $a.attr('target', '_blank');
                        } else {
                            var pathToLinkedFileOrFolder = (extension.indexOf("/") == -1) ? pathToFileUrl : pathToFolderUrl;

                            $a.attr('href', pathToLinkedFileOrFolder + encodeURIComponent(encodeURIComponent(relativePath + href)));
                        }
                    }
                });

                /* fixup image relative paths */
                markup.find('img').each(function () {
                    var $img = $(this);
                    var relativePath = "";

                    if (isRelative($img, 'src')) {
                        //is relative to current
                        relativePath = getCurrentRelativePath(isViewingFile, pathOnFileSystemSections);
                    } else {
                        //is relative to root
                        relativePath = getRootRelativePath(pathOnFileSystemSections);
                    }
                    $img.attr('src', pathToMediaUrl + relativePath + removeQueryString($img.attr('src')));
                });
            }
        }, true);
    }

    return {
        restrict: "A",
        link: linker
    }
}).directive('autoGrow', function ($timeout) {
    var linker = function (scope, element, attrs) {
        scope.$watch('isEditing', function (newValue, oldValue) {
            if (newValue) {
                $timeout(function() {
                    element.autogrow();
                }, 100);
            }
        });
    }

    return {
        restrict: "A",
        link: linker
    }
}).directive('umbracoBookshelfNavigation', function ($http, $compile) {

    function linker(scope, element, attrs) {
        function getView(view) {
            $http.get(view, { cache: false }).then(function (data) {

                element.html(data.data).show();

                $compile(element.contents())(scope);
            });
        }

        scope.$on("$routeChangeSuccess", function(event, current, previous) {

            var isComingFromBookshelf = (previous && previous.pathParams.section.toLowerCase().indexOf("umbracobookshelf") != -1);
            var isOnBookshelf = (current.pathParams.section.toLowerCase().indexOf("umbracobookshelf") != -1);

            //if on bookshelf but not coming from bookshelf
            if (isOnBookshelf) {
                if (!isComingFromBookshelf) {
                    getView("/App_Plugins/UmbracoBookshelf/Views/navigation.html");
                }
            } else {
                //if first time viewed
                if (!previous) {
                    getView("views/directives/umb-navigation.html?intercepted=1");
                } else {
                    //if coming from bookshelf
                    if (isComingFromBookshelf) {
                        getView("views/directives/umb-navigation.html?intercepted=1");
                    }
                }
            }
        });
    }

    return {
        restrict: "E",
        link: linker,
        replace: true
    }
}).directive('insertMd', function() {
    var insertAtCaret = function (element, text) {
        text = text || '';
        if (document.selection) {
            // IE
            element.focus();
            var sel = document.selection.createRange();
            sel.text = text;
        } else if (element.selectionStart || element.selectionStart === 0) {
            // Others
            var startPos = element.selectionStart;
            var endPos = element.selectionEnd;
            element.value = element.value.substring(0, startPos) +
              text +
              element.value.substring(endPos, element.value.length);
            element.selectionStart = startPos + text.length;
            element.selectionEnd = startPos + text.length;
        } else {
            element.value += text;
        }
    };

    function linker(scope, element, attrs) {
        scope.$on('insertMd', function (ev, args) {
            insertAtCaret(element.get(0), "\n" + args.md + "\n");
            scope.makeDirty();

            scope.$emit('insertMdComplete');
        });
    }

    return {
        restrict: "A",
        link: linker
    }
});

