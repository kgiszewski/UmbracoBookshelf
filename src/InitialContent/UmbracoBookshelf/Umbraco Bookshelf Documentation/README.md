#Welcome!#

Thanks for downloading Umbraco Bookshelf!  Bookshelf will allow users to read, create and share Umbraco learning resources quickly.

Bookshelf works by reading markdown files from the `~/UmbracoBookshelf` folder of your website. 

##Install a Book##
The fastest way to download books is to use the [Dashboard](/umbraco/#/UmbracoBookshelf).  Get there by clicking the top of the tree to the left.

It is recommended you start with the official Umbraco documentation and then further your reading.

Books are also available eventually be available via http://our.umbraco.org in the packages section.  Install like you would any other Umbraco Package.

##Editing a Book##
To edit a book, simply click `Edit` on an `.md` file page.  If the book is hosted on GitHub, please consider sending revisions and additional content.

Advanced users can modify their `web.config` file and modify the appSetting `<add key="UmbracoBookshelf:customFolder" value="~/UmbracoBookshelf" />` to point their bookshelf at a different directory.  You can setup a virtual directory inside IIS to point to a folder of forked books to keep source control nice and clean.

##Creating a Book#
Books are just `.md` files that live in the `UmbracoBookshelf` folder of your webroot.  So to create a book, just create a sub-folder next to your other books (like this one).  By default when a user clicks the folder in the tree to the left, the `README.md` file is loaded for the user.

Want to add images?  Just add a folder with images and use relative links.  You can perform many of these operations by right-clicking on an item in the tree, or by using the markdown toolbar while editing.

Links should be relative to the current folder (`MyChapter/README.md`) or relative to the root (`/BookRoot/README.md`).  At this time support for relative paths like `../../` is not yet available.

Links beginning with `http` auto open in a new tab.

Books should be written in Markdown and Bookshelf recognizes Github Flavored Markdown.  Please use the following resources when learning Markdown:

* https://help.github.com/articles/markdown-basics/
* https://help.github.com/articles/github-flavored-markdown/

*Please refrain from using `HTML` directly it at all possible.  Some things like embedding videos are unavoidable.*

##Consider Contributing Your Time##
If you like Bookshelf, please consider helping by adding features or creating books for others to use.

##License##
MIT licensed, use it and abuse it.

##Author##
Umbraco Bookshelf is maintained by Kevin Giszewski.

http://twitter.com/kevingiszewski