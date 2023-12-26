# dotnet UI Mock

Dotnet UI Mock's is framework that allows developers to dynamically set mock responses for interfaces.
This allows to quickly validate UI behavior for different responses and response times.

It allows for rapid UI development and validation. Next to this it makes reproducing UI issues locally a breeze as there is no dependency on a backend system (api, database etc.)

The video below gives a quick introduction to the library.
[[create and insert video]]

In the sample an example can be found for using the library with Blazor.
The path to open UiMock is /uimock

### uimock-selection.txt

The uimock-selection.txt is added to the wwwroot of your project source files by the ui mock library. 
This folder is watched by dotnet watch. So the moment you change your mock settings in the /uimock page, it will automatically trigger a page refresh.
You can add this file to your gitignore as is done in the example


### todo
- improve usage instructions and add video
- add tests for library

### wish list
- mock authentication/authorisation
- support playwright (parallel) ui testing