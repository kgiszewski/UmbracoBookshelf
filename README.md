# UmbracoBookshelf
![Bookshelf](https://github.com/kgiszewski/UmbracoBookshelf/blob/master/assets/logo.png)

## Demo Video

Click the image below to see it in action.

[![Bookshelf](http://img.youtube.com/vi/zunbKOPPf8U/0.jpg)](http://www.youtube.com/watch?v=zunbKOPPf8U)

Bookshelf will allow users to create, display and share learning resources quickly through the Umbraco backoffice.

Bookshelf works by reading markdown files from the `UmbracoBookshelf` folder of your website. 

![Sample](https://our.umbraco.org/media/wiki/144508/635650696507955335_Screen-Shot-2015-04-19-at-15038-PMpng.png)

## Install this Package
Use [NuGet](https://www.nuget.org/packages/umbracobookshelf/) or [Our Umbraco](https://our.umbraco.org/projects/backoffice-extensions/bookshelf) to download and install like you would any other Umbraco package.

## Install a Book
Books are available on the Bookshelf dashboard or you can install any zipped markdown book.

## Editing a Book
To edit a book, simply click `Edit` on an `.md` file page.  If the book is hosted on GitHub, please consider sending revisions and additional content.

## Creating a Book
Books are just `.md` files that live in the `UmbracoBookshelf` folder of your webroot.  So to create a book, just create a sub-folder next to your other books (like this one).  By default when a user clicks the folder in the tree to the left, the `README.md` file is loaded for the user.

Want to add images?  Just add a folder with images and use relative links.

Links beginning with `http` auto open in a new tab.

Books should be written in Markdown and Bookshelf recognizes Github Flavored Markdown.  Please use the following resources when learning Markdown:

* https://help.github.com/articles/markdown-basics/
* https://help.github.com/articles/github-flavored-markdown/

There is now an editor to help you create folders, files and markdown.

*Please refrain from using `HTML` directly when possible.  Some things like embedding videos are unavoidable.*

## Consider Contributing Your Time
If you like Bookshelf, please consider helping by adding features or creating books for others to use.

## License
MIT licensed, use it and abuse it.

## Author
Umbraco Bookshelf is maintained by Kevin Giszewski.

http://twitter.com/kevingiszewski

## Donate
If you find Bookshelf useful and would like to see it continue to be supported, please donate :)

[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=V3AUTEFU8GDV4)

## Uninstall

To uninstall, you will need to make sure you remove these files either manually or via Nuget `uninstall-package umbracobookshelf`

~/App_Plugins/UmbracoBookshelf
~/bin/UmbracoBookshelf.dll

And edit these files:

```
~/config/dashboard.config

Remove this element:
  <section alias="UmbracoBookshelfSection">
    <areas>
      <area>UmbracoBookshelf</area>
    </areas>
    <tab caption="Library">
      <control>/app_plugins/umbracobookshelf/backoffice/dashboards/library.html</control>
    </tab>
  </section>

```

```
~/config/ExamineIndex.config

Remove this element:

  <IndexSet SetName="BookshelfIndexSet" IndexPath="~/App_Data/TEMP/ExamineIndexes/Bookshelf">
    <IndexUserFields>
      <add Name="id" />
      <add Name="book" />
      <add Name="path" />
      <add Name="title" />
      <add Name="text" />
      <add Name="url" />
    </IndexUserFields>
  </IndexSet>
```

```
~/config/ExamineSettings.config

Remove these two elements:

<add name="BookshelfIndexer" type="Examine.LuceneEngine.Providers.SimpleDataIndexer, Examine" dataService="UmbracoBookshelf.Examine.BookshelfExamineDataService,UmbracoBookshelf" indexTypes="Bookshelf" />

<add name="BookshelfSearcher" type="Examine.LuceneEngine.Providers.LuceneSearcher, Examine" analyzer="Lucene.Net.Analysis.Standard.StandardAnalyzer, Lucene.Net" />

```
