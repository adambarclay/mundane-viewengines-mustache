# Mundane.ViewEngines.Mustache

[![License: MIT](https://img.shields.io/github/license/adambarclay/mundane-viewengines-mustache?color=blue)](https://github.com/adambarclay/mundane-viewengines-mustache/blob/main/LICENSE) [![build](https://img.shields.io/github/workflow/status/adambarclay/mundane-viewengines-mustache/Build/main)](https://github.com/adambarclay/mundane-viewengines-mustache/actions?query=workflow%3ABuild+branch%3Amain) [![coverage](https://img.shields.io/codecov/c/github/adambarclay/mundane-viewengines-mustache/main)](https://codecov.io/gh/adambarclay/mundane-viewengines-mustache/branch/main)

Mundane is a lightweight "no magic" web framework for .NET.

This package provides Mundane with a view engine using a [Mustache](https://mustache.github.io/mustache.5.html)-like syntax.

See the [Mundane documentation](https://github.com/adambarclay/mundane) for more information.

## Getting Started

Install the [Mundane.ViewEngines.Mustache](https://www.nuget.org/packages/Mundane.ViewEngines.Mustache/) nuget package, then in the Mundane startup code add a dependency to `MustacheViews`, passing in an `IFileProvider` pointing to the view templates.

For example in ASP.NET:
```c#
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var dependencies = new Dependencies(
            new Dependency<Configuration>(new Configuration(env)),
            new Dependency<DataRepository>(request => new DataRepositorySqlServer(
                request.Dependency<Configuration>().ConnectionString)),
            new MustacheViews(new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Views")));

        var routing = new Routing(
            routeConfiguration =>
            {
                routeConfiguration.Get("/", HomeController.HomePage);
                routeConfiguration.Get("/data/{id}", DataController.GetData);
                routeConfiguration.Post("/data/{id}", DataController.UpdateData);
            });

        app.UseMundane(dependencies, routing);
    }
```

Then in your controller call `MustacheView()` on the `ResponseStream`, passing the name of the template file and optionally a view model.

```c#
    internal static class DataController
    {
        internal static async ValueTask<Response> GetData(Request request)
        {
            var dataRepository = request.Dependency<DataRepository>();

            var data = await dataRepository.GetData(request.Route("id"));
			
            return Response.Ok(responseStream => responseStream.MustacheView("DataPage.html", data));
        }
    }
```

## Method Signatures

These methods all belong to:

```c#
    public static class MustacheViewEngine
```

The following are the standard signatures where the `MustacheViews` object will be retrieved from the `Request` dependencies, and the output will be written to the response stream. An exception will be thrown if the `MustacheViews` object has not been registered as a dependency.

```c#
    public static async ValueTask MustacheView(
        this ResponseStream responseStream,
        string templatePath);

    public static async ValueTask MustacheView(
        this ResponseStream responseStream,
        string templatePath,
        object viewModel);
```

The following also write the output to the response stream, but expect the `MustacheViews` object to be supplied explicitly.

```c#
    public static async ValueTask MustacheView(
        this ResponseStream responseStream,
        MustacheViews mustacheViews,
        string templatePath);

    public static async ValueTask MustacheView(
        this ResponseStream responseStream,
        MustacheViews mustacheViews,
        string templatePath,
        object viewModel);
```

The following expect the `MustacheViews` object to be supplied explicitly, and write the output to the supplied `Stream`.

```c#
    public static async ValueTask MustacheView(
        Stream outputStream,
        MustacheViews mustacheViews,
        string templatePath);

    public static async ValueTask MustacheView(
        Stream outputStream,
        MustacheViews mustacheViews,
        string templatePath,
        object viewModel);
```

The following expect the `MustacheViews` object to be supplied explicitly, and return the output as a `string`.

```c#
    public static async ValueTask<string> MustacheView(
        MustacheViews mustacheViews,
        string templatePath);

    public static async ValueTask<string> MustacheView(
        MustacheViews mustacheViews,
        string templatePath,
        object viewModel);
```

## Mustache

See the [Mustache specification](https://mustache.github.io/mustache.5.html) for more detail. The following is a brief outline of what is supported.

### Variables

To output a value from a view model use `{{name}}` where "name" is a property or dictionary key on the view model.

Variables are HTML escaped e.g. if `name` contained `<script>` it would be output as `&lt;script&gt;`.

To output a value without HTML escaping use `{{&name}}`. Note that triple braces `{{{name}}}` for achieving the same thing is not supported.

### Sections

`{{#name}}` begins a section and `{{/name}}` ends a section.

_Falsy_ values or empty lists for `name` will not output the section contents.

_Truthy_ values will output the section contents once. Non-empty lists will output the section contents once for each item in the list.

### Inverted Sections

An inverted section begins with `{{^name}}` and ends with`{{/name}}`.

_Falsy_ values or empty lists for `name` will output the section contents once.

_Truthy_ values or non-empty lists will not output the section contents.

### Comments

Comments are specified with `{{! This is a comment. }}`. Everything between the braces will be ignored.

### Partials

Partials are specified with `{{> mypartial.html }}`. The contents of `mypartial.html` will be output in place of the tag, and will inherit the current view model context.

### Layout

Layouts (as described [here](https://github.com/mustache/spec/pull/75)) are also supported.

```html
{{! layout.html }}
<html>
    <head>
        <title>{{$Title}}Default Title{{/Title}}</title>
    </head>
    <body>
        {{$Body}}Default Body{{/Body}}
    </body>
</html>
```

```mustache
{{! mypage.html }}
{{< layout.html }}
    {{$Title}}My Title{{/Title}}
    {{$Body}}My Body{{/Body}}
{{/ layout.html }}
```

The output of `mypage.html` would be:
```html
<html>
    <head>
        <title>My Title</title>
    </head>
    <body>
        My Body
    </body>
</html>
```

The layout replacements are optional, in which case the contents of the replacement blocks in the layout will be output.

```mustache
{{! alternatepage.html }}
{{< layout.html }}
{{/ layout.html }}
```

The output of `alternatepage.html` would be:
```html
<html>
    <head>
        <title>Default Title</title>
    </head>
    <body>
        Default Body
    </body>
</html>
```