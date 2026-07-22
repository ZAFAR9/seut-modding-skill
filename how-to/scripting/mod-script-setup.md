# How to: Set Up a Mod Script

← [Back to scripting index](README.md)

Writing C# scripts for Space Engineers requires a specific directory layout and compile-time considerations. This guide walks you through setting up a clean scripting environment, managing namespaces, and converting older projects.

---

## 1. Directory Structure: The One-Folder Rule

Space Engineers compiles mod scripts at runtime on a **folder-by-folder basis**. Every file or folder directly inside the `Data/Scripts/` folder is compiled as a completely separate assembly. If your files are split across multiple root folders under `Scripts/`, they will not be able to reference each other, causing compilation errors or broken logic.

### Correct Layout
Place all your script files inside a single subdirectory named after your mod. You can use as many nested subfolders as you like inside that directory:

```text
MyMod/
├── Data/
│   └── Scripts/
│       └── MyModName/               <-- ALL scripts must live inside this single folder
│           ├── Session/
│           │   └── SessionCore.cs
│           ├── Blocks/
│           │   └── CustomBlockLogic.cs
│           └── MyModName.csproj     <-- Project files (excluded from compilation in-game)
```

**Benefits of naming your folder after your mod:**
* Prevents assembly name collisions.
* Sets a recognizable storage folder name. World and mod-specific data is saved in `%appdata%\SpaceEngineers\Storage\<WorkshopId>.sbm_<MyModName>\`. Using your actual mod name makes debugging and user support significantly easier.

---

## 2. In-Game Compilation Rules

* **`.cs` Files Only:** The game's compiler only loads and compiles `.cs` source files. Compiled binaries like `.dll` or `.exe` are completely ignored for security.
* **Namespace Ambiguity (The Ingame Trap):** Never use `using Sandbox.ModAPI.Ingame;` as a normal using declaration in a mod script. These namespaces are meant for the Programmable Block (PB). Because PB interfaces share identical names with full ModAPI interfaces (e.g., `IMyShipConnector`), importing both will cause silent compiler ambiguity errors in-game, even if your local IDE doesn't show them.
  * **Solution:** If you must use a PB-specific type, always use an alias:
    ```csharp
    using MyShipConnectorStatus = Sandbox.ModAPI.Ingame.MyShipConnectorStatus;
    ```

---

## 3. Setting Up Your IDE (MDK2)

The most efficient way to write mod scripts is using [MDK2 (Mod Development Kit 2)](https://github.com/malforge/mdk2). It automatically sets up the correct project structures and registers local analyzers.

### Manual SDK Conversion (NuGet)
If you have an older project and want to quickly convert it to the modern SDK structure:
1. Close your IDE (Visual Studio, Rider, etc.).
2. Move your `.csproj` and `.sln` files to your mod's root directory (never leave them inside the `Scripts/` folder).
3. Delete the `bin/`, `obj/`, `.vs/` folders, and any `.ruleset` or `.user` files.
4. Replace the contents of your `.csproj` with this clean SDK-style project:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net48</TargetFramework>
    <Platforms>x64</Platforms>
    <LangVersion>6</LangVersion>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
    <GenerateNeutralResourcesLanguageAttribute>false</GenerateNeutralResourcesLanguageAttribute>
    <GenerateTargetFrameworkAttribute>false</GenerateTargetFrameworkAttribute> 
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Mal.Mdk2.ModAnalyzers" Version="*" />
    <PackageReference Include="Mal.Mdk2.References" Version="*" />
  </ItemGroup>
</Project>
```

5. Open your `.sln` in a text editor and update the global target platforms from `Any CPU` to `x64` to match Space Engineers' 64-bit runtime:

```text
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|x64 = Debug|x64
		Release|x64 = Release|x64
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{YOUR_PROJECT_GUID}.Debug|x64.ActiveCfg = Debug|x64
		{YOUR_PROJECT_GUID}.Debug|x64.Build.0 = Debug|x64
		{YOUR_PROJECT_GUID}.Release|x64.ActiveCfg = Release|x64
		{YOUR_PROJECT_GUID}.Release|x64.Build.0 = Release|x64
	EndGlobalSection
EndGlobal
```
*(Replace `YOUR_PROJECT_GUID` with your project's actual GUID found at the end of the `Project()` declaration line).*

---

## 4. API Documentation References

When writing code, refer to these documentation sources:
1. **Official ModAPI Docs:** [Keen ModAPI Index](https://keensoftwarehouse.github.io/SpaceEngineersModAPI/api/index.html) (Incomplete; omits certain whitelisted types like `MyGunBase` and .NET types).
2. **Community ModAPI Docs:** [Malforge ModAPI Directory](https://malforge.github.io/spaceengineers/modapi/) (More complete, but does not list whitelisted standard .NET types).
3. **Local Decompiler:** To compensate for missing docs, decompile the game files directly. See [finding-things-in-the-world.md](finding-things-in-the-world.md) for decompiling steps.

Before distributing your scripts, always cross-reference your implementations with the [crash-safety-checklist.md](crash-safety-checklist.md).

---

## Gotchas & TL;DR

* **The One-Folder Trap:** Placing multiple script folders or loose files directly in `Data/Scripts/` will build them into separate, isolated assemblies that cannot see each other.
* **Never use `using Ingame`:** This will break compiler execution due to interface type collisions with full `ModAPI`. Always alias PB-only types instead.
* **Keep Code Local:** Only `.cs` files are built. Do not attempt to link external `.dll` files; they are completely blacklisted.

---

**Source:**
* https://spaceengineers.wiki.gg/wiki/Modding/Reference/Programming/Intro
* https://spaceengineers.wiki.gg/wiki/Modding/Tutorials/Programming/Convert_old_projects_to_new_SDK
