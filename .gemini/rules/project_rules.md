# Project Rules — Gimersia 2026

Please read and strictly follow the full guidelines in `Assets/Docs/Agentrule.md`:

1. **NO Scene or Prefab Direct Edits:** Do not modify `.unity` or `.prefab` files directly via text edits. Only the developer modifies them in the Unity Editor.
2. **Preserve `.meta` Files:** Never delete or alter GUIDs of `.meta` files.
3. **No Generative AI Assets:** Strictly NO AI-generated images, 3D models, textures, or audio assets. Coding assistance only.
4. **Unity New Input System:** Use `UnityEngine.InputSystem` only (supporting Keyboard & Gamepad/Joystick co-op). Never use legacy `Input.GetKey`.
5. **No Scope Creep:** Only implement what is requested.
6. **Architecture & Standards:**
   - 1 Feature = 1 Script (SRP).
   - Place scripts in appropriate subfolders under `Assets/Resources/Scripts/`.
   - Add a header comment explaining function and features on the first line.
   - Use `[SerializeField] private Type _fieldName;` for inspector-assigned variables.
   - Follow naming conventions: `PascalCase` for Classes/Methods/Interfaces/Enums, `_camelCase` for private fields, `camelCase` for public fields/private methods.
7. **Zero Compile Errors:** Always ensure code compiles cleanly without errors.
8. **Follow Task Templates:** Use `Assets/Docs/templates/` and provide realtime todo checklists.
