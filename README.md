# ror2

> Monolithic Risk of Rain 2 modding repository.

---

This is a collection of various projects I've developed for Risk of Rain 2
modding.

## Void.Build

Void.Build is a collection of MSBuild target files designed to facilitate a
convenient and consistent way to build mods.

### Features

- Automatic dependency resolution.
  - Void.Build can automatically detect game installations and reference the
    applicable assemblies as necessary.
  - This handles boilerplate referencing, and does not pull from the BepinEx
    NuGet repository. Unfortunately, these assemblies do not come with
    publicized members. This is a TODO.
  - MonoMod HookGen assemblies are not generated either. This is a TODO.
- Built-in detection for building and running mods in-game.
  - The output directory is automatically inferred, meaning you can build your
    mod and have it output to the proper plugin/patcher directory in BepinEx.
  - Comes with built-in support for mod managers (currently only r2modman), not
    all platforms are supported yet, contribute paths!
  - Support for specifying custom profiles for aforementioned mod managers.
  - Your preferred mod manager and/or profile can be set with an environment
    variable outside of the `.csproj` for maximum compatibility.
  - Convenient support for running a Mono debug server that your IDE can listen
    to.
  - Auto-detected game paths allow for quickly setting this up.
- Easily include NuGet packages, project references, and assembly references.
- Error detection for invalid paths and `.csproj` properties.
  - Mostly just convenience...
