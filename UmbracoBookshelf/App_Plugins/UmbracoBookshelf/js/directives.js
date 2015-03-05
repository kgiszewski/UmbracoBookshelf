angular.module('umbraco.directives').directive('umbracoBookshelfMarkedUp', function ($location) {

    function getRelativePath() {
        var relativePath = "";

        var url = (decodeURIComponent($location.url()));

        var indexOfFile = url.indexOf('/file/');

        if (indexOfFile != -1) {
            relativePath = decodeURIComponent(url.substr(indexOfFile + 6));

            relativePath = relativePath.substring(0, relativePath.lastIndexOf("/")) + "/";
        } else {
            var indexOfFolder = url.indexOf('/folder/');
            relativePath = decodeURIComponent(url.substr(indexOfFolder + 8)) + "/";
        }

        return "/UmbracoBookshelf" + relativePath;
    }

    function isRelative($element, attribute) {
        return (!isExternal($element, attribute) && $element.attr(attribute).indexOf('/') != 0);
    }

    function isExternal($element, attribute) {
        return $element.attr(attribute).indexOf('http') == 0;
    }

    function cleanForwardSlashes(input) {
        return input.replace(/\%2f/gi, '%252F');
    }

    var linker = function (scope, element, attrs) {
        scope.$watch('model.content', function (newValue, oldValue) {
            if (newValue) {
                var markup = element.html(marked(newValue));
                var relativePath = getRelativePath();

                /* adding global rule for external links */
                markup.find('a').each(function () {
                    var $a = $(this);

                    if (isExternal($a, 'href')) {
                        $a.attr('target', '_blank');
                    } else {
                        if (isRelative($a, 'href')) {
                            if ($a.attr('href').indexOf('/') != 0) {
                                $a.attr('href', "/umbraco/#/UmbracoBookshelf/UmbracoBookshelfTree/file/" + cleanForwardSlashes(encodeURIComponent(relativePath.replace(/\/UmbracoBookshelf/g, '') + $a.attr('href'))));
                            }
                        }
                    }
                });

                /* fixup image relative paths */
                markup.find('img').each(function () {
                    var $img = $(this);
                    if (isRelative($img, 'src')) {
                        $img.attr('src', relativePath + $img.attr('src'));
                    }
                });
            }
        }, true);
    }

    return {
        restrict: "A",
        link: linker
    }
})