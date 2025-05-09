The `Cosmos.Asm.Build` component is part of the Cosmos OS SDK. Its purpose is to compile assembly files into object files, which can then be consumed by other `Cosmos.*.Build` components or custom user targets.

---

## Input Parameters
| Name       | Description                                                         | Default Value                                      |
|------------|---------------------------------------------------------------------|---------------------------------------------------|
| `YasmPath` | Full file path to Yasm, the tool used to compile the object files.  | Unix: `/usr/bin/yasm`, Windows: To be decided.    |
| `AsmSearch`| One or more directory paths that contain assembly files.            | None                                              |
| `AsmFiles` | One or more paths to specific assembly files.                       | None                                              |

---

## Output
All compiled object files are placed in the intermediate output directory of the project, specifically at `$(IntermediateOutput)\cosmos\asm`.

---

## Tasks
| Name                    | Description                                                                                       | Depends On             |
|-------------------------|---------------------------------------------------------------------------------------------------|------------------------|
| `FindSourceFilesForYasm`| Filters all `AsmSearch` and `AsmFiles` paths specified by the user.                               | `Build`                |
| `BuildYasm`             | Compiles all specified assembly files and places them in the `Cosmos.Asm.Build` output directory. | `FindSourceFilesForYasm` |
| `CleanYasm`             | Removes all files generated by `Cosmos.Asm.Build`.                                               | `Clean`                |

---

## Understanding the `AsmSearch` Parameter
The `AsmSearch` parameter defines a collection of directories to scan for `.asm` files. TThe scan is restricted to the root of each specified directory, with no recursive scanning of subdirectories.

### Operations
The following operations are performed on the `AsmSearch` parameter:

1. **Remove Non-Existent Directories**
   Any directory or assembly file that does not exist is removed from the list. A warning with the code `TODO: Pick an error code` is generated for each missing entry.

2. **Scan for Architecture-Specific Subfolders**
   If a subdirectory matches the current target architecture, it replaces its parent directory as an entry for scanning.

3. **Search for Assembly Files**
   The remaining entries in the `AsmSearch` parameter are scanned for `.asm` files. These files are then added to the `AsmFiles` parameter as input for the `Yasm` task.
