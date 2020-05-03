# HtmlGenerator
Reusable web components, html includes, templates for static webpages

Html Generator is a standalone command line tool which adds additional features to html. It can merge, include and otherwise postprocess html files. It is a pure html script-less solution alternative to many web component frameworks.

## How to use

Add additional tags into your Html, run HtmlGenerator to build the project. Altered html should be copied to destination directory.

## Features

### Include

Includes source of another Html file into specified place when building.
```
<include src="Views/MyView.html" />
```

### Surround

Surround html by source of another Html. Useful for same layouts to be used in mnultiple pages.
```
<surround-begin src="_Layout.html" />
    <.. sogme website code here ..>
<surround-end src="_Layout.html" />
```
and in \_Layout.html:

```
<body>
     <surround-content />
</body>
```
In resulting file:

`surround-begin` tag will be replaced by top part of Layout file (everything above `surround-content`).

`surround-end` tag will be replaced by bottom part of Layout file (everything below `surround-content`).

### Template

Template tags are used as reusable web components with variables.

Example of template usage in one of page files:

```
<template src="_Templates/BookEntry.html">
    <@title>Bob's Adventures</@title>
    <@year>2012</@year>
    <@picture>some link</@picture>
    <@description>Some description.</@description>
</template>
```
Tags which being with @ define a variable with value. In this example, it would create a variable with name `year` and value of `2012`

`BookEntry.html` would contain of definition of how template layout looks like, for example:

```
<div class="card"">
    <img class="card-img-top" src="@picture">
    <div class="card-body">
        <h5 class="card-title">@title</h5>
        <p class="card-text">@description</p>
        <span class="text-monospace">@year</span>
    </div>
</div>
```

Text entries which start with @ would be replaced by appropriate values defined in variable list inside template tag. In this example, it would generate such html file:

```
<div class="card"">
    <img class="card-img-top" src="some link">
    <div class="card-body">
        <h5 class="card-title">Bob's Adventures</h5>
        <p class="card-text">Some description.</p>
        <span class="text-monospace">2012</span>
    </div>
</div>
```

- Variables can contain other Html tags. 
- Nested templates are not supported. 
- Including or Surrounding file with templates work just fine. 
- Templates can include other files as well.

### Underscore naming

Since it is a static webpage, we would not like users to go to `www.mydomain.com/Templates/Layout.html` and see half baked html website.

Use underscore as prefix for folders or html files to exclude them from the build. Useful for templates or layout files.

Example of paths which will not be included in build, but still used if referenced by other files:

```
Source/_Layout.html
_Templates/SomeTempalte.html
```

## Json Config

Default json config file path is **"config.json"**.

Config supports everything that command line arguments support.

Example config file:
```
{
  "sourcePath": "C:\\Website\\Resources",
  "destinationPath": "C:\\Website\\Publish",
  "librariesPath": "C:\\Website\\wwwroot"
}
```

### (Pro tip) Interactive Mode

Start tool in interactive mode to see realtime changes to the website. Files are rebuild whenever anything changes in the source directory. So just save your Html file, go to browser and refresh, files should be updated with instant feedback.

## Command Line Arguments

HtmlGenerator.dll Supports these command line arguments

- `--currentDirectory=<path>` Environment path. By default will be set to be the same as Source Path.
- `--sourcePath=<path>` Source path with HTML files which need to be built.
- `--destinationPath=<path>` Destination path where all built HTML are placed
- `--librariesPath=<path>` Libraries folder path where libs, css, js files can be stored. Will be copied to destination path after build.
-  `-i`, `-interactive` Start interactive continious build. Builds website everytime a file in source directory is modified.
- `-c`, `-clean` Clean destination directory
- `-r`, `-rebuild` Rebuild website and copy libraries
- `-b`, `-build` Build complete website, will not copy libraries if present
- `-h`, `-help` Show this help screen

Command line arguments override settings read from Json file.

Example: `dotnet HtmlGenerator.dll -b --sourcePath=dir/wwwroot`
Or if using non portable build: `HtmlGenerator.exe -rebuild`
