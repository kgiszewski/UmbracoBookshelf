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

    var linker = function (scope, element, attrs) {
        scope.$watch('model.content', function (newValue, oldValue) {
            if (newValue) {
                var markup = element.html(marked(newValue));
                var relativePath = getRelativePath();

                /* adding global rule for external links */
                markup.find('a').each(function() {
                    var $a = $(this);
                    if ($a.attr('href').indexOf('http') == 0) {
                        $a.attr('target', '_blank');
                    }
                });

                /* fixup image relative paths */
                markup.find('img').each(function () {
                    var $img = $(this);
                    if ($img.attr('src').indexOf('http') != 0 && $img.attr('src').indexOf('/') != 0) {
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