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
