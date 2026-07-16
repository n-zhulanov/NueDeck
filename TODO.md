# NueDeck prototype TODO

## Current objective

Turn the existing **NueDeck card roguelike template** into a small, reliable,
presentable game prototype.

The template must remain the foundation of the game. First run and understand
the stock game, then make a small number of focused improvements based on what
is actually visible in Play Mode.

## Explicit non-goals

- Do **not** create a separate top-down game or a new gameplay framework.
- Do **not** integrate an existing Xsolla SDK.
- Do **not** implement Xsolla Balance Studio, telemetry, simulations, reports,
  suggested diffs, or SDK-facing abstractions yet.
- Do **not** rewrite NueDeck's singleton/ScriptableObject/action architecture
  merely to modernize it.
- Do **not** discard or overwrite unrelated user changes in the dirty worktree.
- Do **not** bypass git hooks with `--no-verify`.

`xsolla_balance_studio.md` is product context for a later phase only. The
current deliverable is a lightly polished playable game that can eventually be
used to demonstrate that future SDK.

## Known project state

- Project root: `/Users/n.zhulanov/NueDeck`
- Unity Editor: `6000.5.4f1`, Apple Silicon.
- NueDeck documentation identifies the imported template as v2.1.0; it was
  originally designed around Unity `2020.3.34f1`, so Unity 6 upgrade regressions
  are possible and must be tested rather than assumed away.
- Render pipeline: URP assets are present.
- Active input handler: legacy/old Input Manager (`activeInputHandler: 0`).
- TextMeshPro essentials and the required scenes appear to be imported.
- Current Build Settings order is correct:
  1. `Assets/NueGames/NueDeck/Scenes/0- Main Menu.unity`
  2. `Assets/NueGames/NueDeck/Scenes/1- Map.unity`
  3. `Assets/NueGames/NueDeck/Scenes/2- Combat Scene.unity`
  4. `Assets/NueGames/NueDeck/Scenes/NueCore.unity`
- `CoreLoader` loads `NueCore` additively from any gameplay scene when the
  persistent managers do not exist.
- Expected game flow: Main Menu -> Map -> Combat -> Reward/Map -> next encounter.
- No NueDeck C# compiler errors were found in the inspected `Logs/Editor.log`.
- Repeated `NoSubscription` messages come from Unity AI Generators and are not
  currently evidence of a NueDeck failure.
- The repository already contains many user modifications and untracked files.
  Treat all of them as user-owned and preserve them.

## Official NueDeck setup/run instructions

Extracted from `Assets/NueGames/NueDeck/NueDeck - Documentation.pdf`:

1. Import NueDeck.
2. Import TextMeshPro Essentials.
3. Put the four scenes into Build Settings in the order listed above, with
   `NueCore` last.
4. Run `NueDeck -> SetLayers` to create required layers.
5. For Built-in RP, replace FX shaders; this project currently uses URP.
6. Open any scene except `NueCore` and press Play. For the full intended flow,
   start with `0- Main Menu.unity`.
7. When adding cards/allies, update
   `Assets/NueGames/NueDeck/Data/Settings/Gameplay Data.asset` accordingly.

The template targets a 1920x1080 presentation and a fast prototype/game-jam
scope. It is designed primarily for one ally versus up to three enemies.

## Unity MCP prerequisites

Unity MCP is already installed and configured:

```toml
[mcp_servers.unityMCP]
url = "http://127.0.0.1:8080/mcp"
```

Before starting a new Codex session:

1. Keep Unity open on this project.
2. Open `Window -> MCP for Unity`.
3. Ensure HTTP Local server is `Running` at `http://127.0.0.1:8080`.
4. Ensure the Unity bridge/session says `Connected`.
5. Start/restart the Codex session **after** the server is ready. MCP tools are
   fixed at session startup; an older session will not gain them dynamically.
6. Verify locally if needed:

   ```bash
   lsof -nP -iTCP:8080 -sTCP:LISTEN
   ```

7. In the new session, load the installed Unity skill at
   `/Users/n.zhulanov/.codex/skills/unity-mcp-skill/SKILL.md` and follow its
   resource-first workflow.

## Unity MCP resources to inspect first

- `mcpforunity://instances` - available Unity Editor instances.
- `mcpforunity://editor/state` - compilation, domain reload, Play Mode, active
  scene, readiness, and blocking reasons.
- `mcpforunity://project/info` - Unity version, render pipeline, input handler,
  and installed UI packages.
- `mcpforunity://scene/gameobject-api` - scene object access conventions.
- Active scene hierarchy and individual GameObject/component resources.
- `mcpforunity://scene/cameras` where camera inspection is needed.
- Rendering resources only if visuals are broken:
  `mcpforunity://scene/volumes`, `mcpforunity://rendering/stats`, and
  `mcpforunity://pipeline/renderer-features`.

## Available Unity MCP tool inventory

Use only the subset needed for the task, but this is the installed tool set:

### Connection and batching

- `set_active_instance` - route calls to the NueDeck Editor instance.
- `batch_execute` - group independent discovery or edit operations; maximum 25
  commands by default.
- `refresh_unity` - manual AssetDatabase refresh/compile request. Do not call it
  after script tools that already trigger compilation.

### Scenes and GameObjects

- `manage_scene` - active scene, hierarchy, load/save/create, screenshots, and
  scene view framing.
- `find_gameobjects` - find by name, tag, layer, component, path, or ID.
- `manage_gameobject` - create, modify, duplicate, move, or delete objects and
  instantiate prefabs.
- `manage_components` - add/remove components and inspect/set serialized fields.

### Scripts

- `create_script`
- `script_apply_edits`
- `apply_text_edits`
- `validate_script`
- `get_sha`
- `find_in_file`
- `delete_script`

After every script change: poll `mcpforunity://editor/state` until
`is_compiling == false` and `ready_for_tools == true`, then run `read_console`
for errors. Use SHA-aware edits and retry on `stale_file` rather than overwriting
newer user changes.

### Assets, prefabs, visuals, and UI

- `manage_asset`
- `manage_prefabs` (prefab editing/info; instantiate with `manage_gameobject`)
- `manage_material`
- `manage_texture`
- `manage_ui` (UI Toolkit; NueDeck itself primarily uses uGUI/TMP)
- `manage_camera`
- `manage_graphics`
- `manage_physics`
- `manage_probuilder`

For this task, prefer modifying existing NueDeck prefabs/scenes and
ScriptableObjects over generating replacements.

### Editor, validation, tests, and diagnostics

- `manage_editor` - Play/Pause/Stop and other editor control.
- `execute_menu_item` - including `NueDeck/SetLayers` if layer validation shows
  it is required.
- `read_console` - clear/read filtered errors, warnings, and stack traces.
- `run_tests` and `get_test_job` - EditMode/PlayMode Unity tests.
- `execute_custom_tool` - only for already registered project tools.
- `manage_profiler` - targeted runtime performance inspection if a real issue is
  observed; not part of baseline polish.
- `unity_reflect` - inspect live Unity/C# API signatures.
- `unity_docs` - official Unity/package documentation lookup.
- `manage_packages` - inspect packages; do not add/remove packages unless a
  confirmed project defect requires it.

### Screenshot verification

Use `manage_camera(action="screenshot", include_image=true)` or the equivalent
`manage_scene` screenshot action after each visual change. Capture at least:

- Main Menu before/after.
- Map before/after.
- Combat at the start of the player turn.
- Combat with a targeted card/highlight active.
- Victory, reward, and defeat states if reachable.

Prefer 512px inline captures for review and use the scene-view capture only for
hierarchy/layout debugging. Do not leave accidental screenshot assets in the
repository unless they are intentionally part of the handoff.

## Fallback tools

- Project inspection/editing: `rg`, `rtk`, `apply_patch`, git diff/status.
- Unity log: `/Users/n.zhulanov/NueDeck/Logs/Editor.log`.
- Global fallback log: `/Users/n.zhulanov/Library/Logs/Unity/Editor.log` (this
  project currently redirects subsequent output to the project log).
- Computer Use for clicking the live Game view is optional. It was unavailable
  in the previous session because its Node runtime failed to start. Do not block
  the task on it: use Unity MCP plus PlayMode tests, or ask the user for a short
  manual interaction only when unavoidable.

## Execution plan

### Phase 1 - Preserve and baseline

- [ ] Run `rtk --version` and use `rtk` where applicable.
- [ ] Inspect `git status --short`; record the dirty paths and do not revert them.
- [ ] Read `mcpforunity://instances`, select NueDeck, then read editor state and
  project info.
- [ ] Wait for any import/compilation/domain reload to finish.
- [ ] Read and then clear relevant Console messages so new Play Mode failures
  can be distinguished from old Unity AI/service noise.
- [ ] Confirm required layers exist; run `NueDeck -> SetLayers` only if missing.
- [ ] Confirm the Build Settings scene order through Unity, not only YAML.

### Phase 2 - Test the stock template before changing it

- [ ] Load and save-check `0- Main Menu.unity` without modifying scene content.
- [ ] Enter Play Mode from Main Menu.
- [ ] Capture the initial Main Menu screenshot and Console.
- [ ] Exercise the intended flow:
  - [ ] Start/new game.
  - [ ] Open Map.
  - [ ] Select the active encounter.
  - [ ] Enter Combat.
  - [ ] Inspect initial hand, mana, draw/discard/exhaust counters, player health,
    enemy health, and enemy intention.
  - [ ] Play cards with and without targets.
  - [ ] End turn and observe enemy actions/status effects.
  - [ ] Reach victory and verify reward selection/return to Map.
  - [ ] Verify defeat/restart or return-to-menu behavior.
- [ ] After each scene transition, inspect Console for null references, missing
  shaders, missing sprites/materials, serialization upgrade errors, or broken
  button events.
- [ ] Record actual usability problems and visual defects before choosing edits.

If MCP cannot click runtime uGUI buttons, create focused PlayMode tests or ask
the user to perform only the minimum click sequence while the agent observes
state/Console. Do not invent a second game to avoid testing the template.

### Phase 3 - Choose minimal polish from evidence

Keep the change set small enough for a reliable demo. Prioritize in this order:

1. **Blocking correctness:** broken scene transitions, missing references,
   Unity 6 API regressions, invalid materials/shaders, unusable buttons.
2. **First-run clarity:** obvious Start Game action and a concise How to Play
   explanation for card targeting, mana, enemy intentions, and End Turn.
3. **Complete short loop:** a new user can reach one encounter, win or lose, and
   understand what happens next within roughly 3-5 minutes.
4. **Feedback polish:** card hover/selection clarity, target highlight, hit/block
   feedback, turn-state label, and readable win/lose/reward actions.
5. **Presentation consistency:** only small title/copy/layout/color fixes using
   existing NueDeck assets and prefabs.

Do not implement every item automatically. Select only changes justified by the
baseline run and keep a before/after record.

### Phase 4 - Implement safely

- [ ] Prefer existing ScriptableObjects, prefabs, scenes, and editor tooling.
- [ ] Keep gameplay tuning in the existing card/character/encounter data.
- [ ] Preserve public APIs and serialized fields where possible.
- [ ] Use `batch_execute` for independent scene discovery/edits.
- [ ] After every script edit, wait for compilation and inspect Console errors.
- [ ] After every scene/prefab edit, save explicitly and take a screenshot.
- [ ] Add tests only where they provide stable coverage of the repaired flow.
- [ ] Do not modify `xsolla_balance_studio.md` as part of this task.

### Phase 5 - Verification and handoff

- [ ] Re-run Main Menu -> Map -> Combat -> Reward/Map.
- [ ] Re-run or directly verify defeat behavior.
- [ ] Confirm zero new C# errors and zero new runtime exceptions.
- [ ] Validate at 1920x1080 Game view, the template's documented target.
- [ ] Review `git diff --check` and a scoped diff of only the files changed for
  this task.
- [ ] Report exact launch instructions to the user:
  open `0- Main Menu.unity`, press Play, then follow the visible Start action.
- [ ] Summarize what was changed, what was deliberately left untouched, tests
  performed, and any remaining Unity 6/template limitations.

## Definition of done

- NueDeck, not a replacement game, is the playable prototype.
- A first-time user can launch it from `0- Main Menu.unity` and understand the
  basic card-combat loop without reading source code.
- At least one complete encounter path works in Play Mode.
- No Balance Studio/Xsolla SDK code is present.
- No unrelated user changes were reverted or reformatted.
- Unity Console contains no new compile errors or runtime exceptions caused by
  the implementation.
- The final handoff includes concise launch instructions and evidence of the
  tested flow.
