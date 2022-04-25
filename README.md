# TextTemplate Loupedeck Plugin
[![License](http://img.shields.io/:license-MIT-blue.svg?style=flat)](LICENSE)
![forks](https://img.shields.io/github/forks/Steinerd/Loupedeck.TextTemplatePlugin?style=flat)
![stars](https://img.shields.io/github/stars/Steinerd/Loupedeck.TextTemplatePlugin?style=flat)
![issues](https://img.shields.io/github/issues/Steinerd/Loupedeck.TextTemplatePlugin?style=flat)
[![downloads](https://img.shields.io/github/downloads/Steinerd/Loupedeck.TextTemplatePlugin/latest/total?style=flat)](https://github.com/Steinerd/Loupedeck.TextTemplatePlugin/compare)

This is a product of a question posed on a Loupedeck FB Group. 

In short, it allows for kinda clever text send macro templates that dynamically read other files for replacements. 

--------

# Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [Support](#support)
- [Contributing](#contributing)
- [License](#license)

# Installation

<details><summary><b>Loupedeck Installation</b></summary>
  

  1. Go to [latest release](https://github.com/Steinerd/Loupedeck.TextTemplatePlugin/releases/latest), and download the `lplug4` file to you computer
  1. Open (normally double-click) to install, the Loupedeck software should take care of the rest
  1. Restart Loupedeck (if not handled by the installer)
  1. In the Loupedeck interface, enable **TextTemplate** by clicking <ins>Manage plugins</ins>
  1. Check the TextTemplate box on to enable
  1. Drag the desired control onto your layout

Once click it will bring you to a dynamic playback device selection page. 
</details>

<details><summary><b>IDE Installation</b></summary>
  Made with Visual Studio 2022, C# will likely only compile in VS2019 or greater. 

  Assuming Loupedeck is already installed on your machine, make sure you've stopped it before you debug the project. 

  Debugging _should_ build the solution, which will then output the DLL, config, and pdb into your `%LocalAppData%\Loupedeck\Plugins` directory.

  If all goes well, Loupedeck will then open and you can then debug. 

</details>

# Usage

Follow the __Loupedeck Installation__ instructions above. 

Check out the [Example Template File](example-template.tt).

The important pieces are, the `Template` section at the top, which can be multiple lines. The text send will emulate a return/enter key press for every line. 

I realize that the same-file replacement seems pointless, but it might be useful to an automation that writes to the template file... idk. 

The the `fileRef#` examples are a little complex. 

Firstly, any file-ref secion will have to start with `file` or it will be treated the same as an in-file replacement. 

I'm using Newtonsoft's `SelectToken` via `JSONPath` ([click here to learn more](https://www.newtonsoft.com/json/help/html/QueryJsonSelectTokenJsonPath.htm)).

I'm also using it to convert XML to JSON so the same method can be used for those file types. 

*Important*; if your XML document has a `<?xml version ... ?>` header, you will have to traverse from `$.root`; otherwise `$` is effectively root. 

As for all other file types, the fileRef is using Regular Expression (.net's flavor of it anyways...)

You *MUST* denote the `replacement` group name or the script/plugin won't pick it up. That is done by simply going to the beginning of the group and writing `?<replacement>...`. 

So a search for a random setting without a group would look like `random-setting=(.*)`. But with the group name it will look like `random-setting=(?<replacement>.*)`

# Support

[Submit an issue](https://github.com/Steinerd/Loupedeck.TextTemplatePlugin/issues/new)

Fill out the template to the best of your abilities and send it through. 

# Contribute

Easily done. Just [open a pull request](https://github.com/Steinerd/Loupedeck.TextTemplatePlugin/compare). 

Don't worry about specifics, I'll handle the minutia. 

# License
The MIT-License for this plugin can be reviewed at [LICENSE](LICENSE) attached to this repo.
