# How to: Navigate the Modding Whitelist

← [Back to scripting index](README.md)

Space Engineers compiles mod scripts at runtime inside a secure sandbox. This sandbox is governed by a **whitelist** that restricts mods from accessing harmful APIs (like raw file I/O, threads, or reflection) to protect clients and dedicated servers in multiplayer.

This guide covers why the whitelist exists, how to detect blocked APIs before loading the game, common traps (such as matrix decomposition), and how to find allowed alternatives.

---

## 1. What is the Whitelist?

The whitelist is a set of rules evaluated by the game's compiler. It restricts assembly loading and enforces type/member access control. 
If your script references an unlisted namespace, type, or specific method, the game will **fail to compile the mod** and write a compiler warning or error to the game's log.

### Why it exists
Without a whitelist, a downloaded mod could execute arbitrary code, modify system files, access network ports, or inject malware. The whitelist limits mod scripts to the game's specific API, mathematical operations, and basic .NET containers.

---

## 2. How to Detect Whitelist Violations

You do not need to boot Space Engineers to verify if your code complies with the whitelist.

### The Realtime IDE Analyzer (Recommended)
By using an IDE (such as Visual Studio or Rider) equipped with the **MDK2 (Mod Development Kit 2)** packages, you get real-time feedback:
* **Nuget Package:** `Mal.Mdk2.ModAnalyzers` runs a custom Roslyn analyzer in the background.
* **Behavior:** It highlights non-whitelisted types or calls with red/compiler-error squiggles immediately inside your code editor, preventing "compile-test-crash" cycles.

### Silent / Ambiguous Compiler Errors
Sometimes, a blocked method will throw a confusing compile error:
`'Matrix' does not contain a definition for 'Decompose'`
Even though the method *does* exist in standard .NET, the sandbox compiler actively hides blocked members. If a known standard method throws a "does not exist" or "missing definition" error, **it is blocked by the whitelist.**

---

## 3. Common Blocked APIs & The Traps

Modders frequently hit the whitelist wall when attempting to port standard C# code. Here are the most common blocked categories and known traps:

| Blocked Concept | Blocked API / Type | Whitelisted Alternative |
|---|---|---|
| **Direct File I/O** | `System.IO.File`, `System.IO.Directory` | `MyAPIGateway.Utilities.WriteFileInLocalStorage` / `WorldStorage` |
| **Multithreading** | `System.Threading.Thread`, tasks | Game-managed task processing (or limit logic to throttled updates) |
| **Reflection** | `Type.GetMethods()`, `FieldInfo.SetValue` | None (compile-time reflection / `nameof` only) |
| **Process execution** | `System.Diagnostics.Process` | None (completely blacklisted) |
| **Matrix Decomposition** | `Matrix.Decompose` & `MatrixD.Decompose` | Cache the rest state matrix, and pre-multiply rotations |

### The `Matrix.Decompose` Trap (Crucial)
A recurring pitfall when writing animation scripts (e.g., spinning rotors or custom weapon models) is attempting to extract scale, rotation, and translation vectors from an entity's local matrix.

**Both `Matrix.Decompose` (float) and `MatrixD.Decompose` (double) are completely blocked.**

#### The Whitelisted Alternative
Instead of decomposing the matrix each frame to inject rotation, **cache the original resting local matrix** on the first frame, and pre-multiply a rotation matrix:

```csharp
// 1. On first frame, store rest pose
_restMatrix = subpart.PositionComp.LocalMatrixRef;

// 2. Each frame, apply rotation IN local space
Matrix rotation = Matrix.CreateRotationY(angle);
Matrix finalMatrix = rotation * _restMatrix; // Rotates local coordinate space

// 3. Apply matrix safely
subpart.PositionComp.SetLocalMatrix(ref finalMatrix, null, true);
```
Pre-multiplying ensures the model's baked-in non-uniform scaling, translation, and tilt remain completely intact, removing any need to decompose.

---

## 4. How to Find Whitelisted Alternatives

When standard .NET types or helper methods are blocked, follow these strategies to find safe workarounds:

1. **Verify against the Whitelist Reference:** Use the [community ModAPI whitelist index](https://spaceengineers.wiki.gg/wiki/Modding/Reference/Programming/Whitelist) to inspect allowed types grouped by their assembly files (`mscorlib.dll`, `System.dll`, `VRage.Math.dll`, etc.).
2. **Decompile Game Code:** Launch a decompiler (such as ILSpy or dnSpy) and search for how vanilla blocks perform the desired task. If a vanilla block uses a specific API inside the game assembly, that API's containing namespace is almost certainly whitelisted. See [finding-things-in-the-world.md](finding-things-in-the-world.md) for decompiled searching tips.
3. **Use the SDK Analyzers:** Type the proposed code in your MDK2 project. If the Roslyn analyzer does not report a violation, it is safe to use.

Before releasing a mod that uses newly discovered APIs, make sure they are checked against the [crash-safety-checklist.md](crash-safety-checklist.md).

---

## Gotchas & TL;DR

* **"Method Missing" Lies:** If the IDE says a standard .NET math or file method "does not exist," the whitelist compiler is hiding it.
* **Double variants fail too:** If a `float` math method is blocked (e.g., `Matrix.Decompose`), switching to the `double` variant (`MatrixD.Decompose`) will still fail the compilation.
* **No Raw Reflection:** You cannot use reflection to access private variables or cheat around block boundaries.
* **Secure File I/O Only:** Never use `System.IO.File`. Always write to world/local storage using the designated `MyAPIGateway.Utilities` hooks.

---

**Source:**
* https://spaceengineers.wiki.gg/wiki/Modding/Reference/Programming/Whitelist
