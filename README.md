# 🎮 RPGGame —— 2D 类银河恶魔城游戏

一个基于 **Unity 2022.3** 与 **C#** 开发的 2D 动作冒险游戏项目。项目包含玩家状态机、敌人 AI、Boss 战斗、多种主动技能、装备与背包、材料合成、角色属性、检查点以及本地存档等系统，重点实践**分层有限状态机、系统模块化与数据持久化**。

## 🎮 立即体验

👉 [点击下载 Windows 演示版 (v1.0.0)](https://github.com/XianWang12/RPGGame/releases/tag/1.0.0)

## 📖 项目简介

RPGGame 是一款以横版动作战斗和角色成长为核心的 2D 类银河恶魔城游戏。玩家可以在场景中移动、跳跃、冲刺、墙跳并与敌人战斗，通过技能树、装备、属性成长和物品效果不断增强角色能力。项目包含普通骷髅敌人及拥有独立状态逻辑的 Boss，并围绕物品管理、技能解锁和数据持久化构建了较完整的游戏系统。

项目的主要开发重点包括：**分层有限状态机、技能模块化、背包/装备/仓库管理、属性修饰器、合成流程和 JSON 本地存档**。

## ✨ 核心功能与技术实现

### 1. 玩家状态机与动作控制

- 使用有限状态机（FSM）管理玩家行为，将待机、移动、跳跃、空中、冲刺、墙面滑行、墙跳、普通攻击、反击、瞄准/施放技能、黑洞与死亡等行为拆分为独立状态类。
- 通过统一的 `PlayerStateMachine` 与 `PlayerState` 基类组织状态切换，状态在 `Enter/Update/Exit` 中统一处理动画布尔量、计时与速度设置。
- 普通攻击为**三段连击**：`PlayerPrimaryAttackState` 使用 `comboCounter` 与 0.4 秒的连招窗口控制段数，并按 `attackMovement` 数组为每段配置击退位移。
- 通过 `PlayerAnimationTriggers` 把动画事件回调到状态机（`AnimationFinishTrigger`），让攻击判定、位移等阶段与动画播放对齐。
- 冲刺、跳跃、墙跳等动作分别在各自状态中处理，并在 `SlowEntityBy` 中统一实现减速对移速、跳跃力、冲刺速度与动画播放速度的影响。

### 2. 普通敌人与 Boss AI

- 普通骷髅敌人（`Enemy_Skeleton`）使用独立状态类组织待机、移动、战斗、攻击、受击硬直与死亡行为；进入战斗后通过射线检测持续追踪玩家，超出距离则回到待机。
- 敌人拥有**反击窗口**：`OpenCounterAttackWindow`/`CloseCounterAttackWindow` 控制 `canBeStunned`，玩家在窗口内反击即可将其打入**眩晕**状态（`SkeletonStunnedState`，附带红色闪烁与击退）。
- Boss（`Enemy_Boss`）拥有单独的状态体系：待机、移动、战斗、攻击、施法、传送和死亡。玩家进入一定距离后 `bossFightStarted` 置位并进入战斗。
- Boss 会向玩家所在区域施放法术（`BossSpellCastState` + `BossSpell_Controller`），并按冷却时间与概率决定是否施法或传送。
- 传送（`BossTeleportState`）会在竞技场 `BoxCollider2D` 范围内随机取点，并通过向下射线与重叠检测确保落点在地面上；`Enemy_BossTrigger` 中的 `Relocate`/`MakeInvisible`/`MakeVisible` 用于配合动画完成位移与显隐。
- Boss 生命条仅在玩家与 Boss 距离小于 21 时显示。
- 敌人属性支持**等级成长**：`EnemyStats` 按 `level` 与 `percentageModifier` 逐级放大伤害、生命、护甲、闪避、魔抗与三类魔法伤害。

### 3. 多种技能与技能树

项目包含多个独立技能类，均由 `Skill` 基类统一管理冷却计时与 `CanUseSkill()/UseSkill()` 入口，`SkillManager` 单例集中持有各技能实例：

- **冲刺（`Dash_Skill`）：** 提供位移，并可解锁「冲刺时留下分身」「到达时留下分身」。
- **闪避（`Dodge_Skill`）：** 受击/闪避时生成残影（`CreateMirageOnDodge`），与闪避率属性联动。
- **招架/反击（`Parry_Skill`）：** 反击命中可眩晕敌人，并可生成反击分身（`MakeMirageOnParry`）。
- **剑类技能（`Sword_Skill`）：** 掷出与召回剑，包含瞄准（`PlayerAimSwordState`）与接剑（`PlayerCatchSwordState`）等状态，回收命中造成额外伤害。
- **水晶（`Crystal_Skill`）：** 生成可爆炸的水晶，也可作为分身技能的替代效果。
- **黑洞（`Blackhole_Skill`）：** 生成会成长/收缩的引力区域，可吸引并反复攻击范围内敌人，配合 `PlayerBlackholeState` 使用。
- **分身（`Clone_Skill`）：** 生成限时分身，可配置能否攻击、是否附带命中效果、多分身与分裂概率、以及替换为水晶等。
- **技能树（`UI_SkillTreeSlot`）：** 每个槽位可配置前置解锁/互斥槽位与价格，使用货币解锁，解锁状态写入存档；`Skill` 基类通过 `CheckUnlock` 在启动时同步解锁状态。

### 4. 背包、装备与仓库

- 使用**背包（inventory）**、**仓库（stash）**与**装备栏（equipment）**三类容器分别管理物品，每种容器同时用 `List<InventoryItem>` 保存顺序、用 `Dictionary` 建立「物品数据 → 物品实例」映射，兼顾遍历与查询。
- 支持物品添加、堆叠（`stackSize`）、按堆移除、装备穿戴及同类型装备替换（新装备顶替旧装备并退回背包）。
- 物品数据以 `ScriptableObject` 组织：`ItemData` 为基类，`ItemData_Equipment` 扩展装备类型（武器/护甲/护符/药瓶）、属性加成、物品效果与合成材料。
- 装备通过 `AddModifiers()`/`RemoveModifiers()` 把自身属性写入/移出玩家 `Stat` 的修饰器列表；穿脱装备会同步调整最大生命值并联动当前生命值，随后刷新 UI 与属性面板。
- `UI_ItemSlot` / `UI_EquipmentSlot` / `UI_StatSlot` / `UI_ItemToolTip` 负责槽位显示、装备交互与属性/效果说明；仓库与背包在 UI 中均可通过右键使用物品。

### 5. 材料合成与物品效果

- 提供合成窗口、合成列表与合成槽位（`UI_CraftWindow` / `UI_CraftList` / `UI_CraftSlot`）组织配方展示与制作交互。
- 配方由 `ItemData_Equipment.craftingMaterials` 配置；制作时检查材料是否齐备，扣除材料并产出装备。
- 物品效果以 `ItemEffect : ScriptableObject` 为基类，包含治疗、增益、火焰、冰霜、雷击以及冻结敌人等实现，可在数据资产中自由组合。
- 例如：`FreezeEnemies_Effect` 在角色低血量时冻结周围敌人；`ThunderStrike_Effect` 在目标位置生成落雷；这些效果通过装备的 `Effect(Transform)` 统一触发。

### 6. 角色属性与战斗计算

- 使用 `Stat` 结构统一表示每项属性：以 `baseValue` 为基础值，叠加 `modifiers` 列表得到最终值，从而支持装备与限时效果叠加。
- 属性涵盖四维主属性（力量/敏捷/智力/体质）与伤害、暴击率、暴击伤害、最大生命、护甲、闪避、魔法抗性、火焰/冰霜/雷电伤害等。
- 主属性参与二级计算：**力量**提高伤害与暴击伤害，**敏捷**提高暴击率与闪避，**智力**提高魔法伤害，**体质**提供生命成长（每点 +5 最大生命）。
- 战斗计算包含命中/闪避判定、暴击（默认暴击伤害 150%）、护甲减伤与魔法抗性处理；生命变化通过 `onHealthChanged` 回调驱动血条等 UI 刷新。
- 实现**燃烧、冰缓与感电**异常状态：燃烧按 0.3 秒间隔结算持续伤害，冰缓影响命中/闪避判定，感电有概率触发额外落雷；状态由计时器自动结束，并由 `EntityFX` 以颜色闪烁表现。

### 7. 存档与数据持久化

- 使用 `ISaveManager` 统一存档模块的接口（`LoadData` / `SaveData`），任何系统实现该接口即可参与保存与读取；`SaveManager` 在启动时通过 `FindObjectsOfType<MonoBehaviour>` 自动收集所有实现者。
- 使用 `GameData` 集中保存关键数据：货币、技能树解锁状态、背包物品、已装备物品、检查点状态、最近检查点与音量设置。
- 通过 `JsonUtility` 将数据序列化为 JSON，并在 `Application.persistentDataPath` 下完成本地文件读写与删除。
- 自定义 `SerializableDictionary<TKey, TValue>`（实现 `ISerializationCallbackReceiver`）以键值对列表形式保存字典，解决 Unity 无法直接序列化字典的问题。
- 存档模块提供可选的 `encryptData` 开关，采用逐字符异或处理（密钥为固定字符串）；**该实现属于简单混淆，不应视为安全的加密方案**。
- 保存时机为 `OnApplicationQuit`，载入时机为 `SaveManager.Start`；提供「Delete Saved Data」右键菜单用于清档。

### 8. 检查点、界面与音频

- **检查点（`CheckPoint`）：** 使用 `Guid` 作为唯一 id，玩家触发时激活并播放动画与音效；存档记录每个检查点的激活状态与最近激活的检查点，读档时把玩家移动回最近检查点。
- **界面（`UI`）：** 以 `Tab`/`I`/`U`/`Esc` 切换人物属性、技能树、合成与设置界面；打开菜单时通过 `GameManager.pauseGame` 暂停游戏（`Time.timeScale`），并自动回到游戏内 HUD。
- **游戏内 HUD（`UI_InGame`）：** 显示生命值、货币、技能冷却遮罩（冲刺/招架/水晶/剑/黑洞/药瓶）等。
- **音频（`AudioManager` / `AreaSound`）：** 集中管理 SFX 与 BGM 数组，SFX 播放随机音高并支持按距离衰减；`AreaSound` 在玩家进入区域时切换 BGM，离开时渐隐停止。
- **表现（`EntityFX`）：** 使用 Cinemachine 的 `CinemachineImpulseSource` 实现命中/受击屏幕震动，并通过材质替换与颜色闪烁表现受击与异常状态。
- 使用视差背景脚本（`Parallax Background`）增强场景层次感。

## 🛠️ 技术栈

| 技术 | 用途 |
| --- | --- |
| Unity 2022.3.62f3c1 | 游戏引擎与项目编辑环境 |
| C# | 游戏逻辑与系统开发 |
| 有限状态机（FSM） | 玩家、普通敌人与 Boss 的行为管理 |
| ScriptableObject | 物品数据、装备数据与物品效果的数据驱动配置 |
| 属性修饰器（Modifier） | 装备与限时效果对属性的叠加计算 |
| `List` / `Dictionary` | 背包、装备、仓库与数据组织 |
| `JsonUtility` / JSON | 游戏数据序列化与本地存档 |
| `SerializableDictionary` | 字典的序列化保存 |
| Cinemachine | 镜头震动等相机表现 |
| TextMeshPro / UGUI | 游戏内 HUD 与各项菜单界面 |
| Unity 2D Physics | 碰撞、射线检测与攻击判定 |

## 📁 主要脚本结构

```text
Assets/Scripts/
├── Controllers/          # 技能与技能效果控制器（法术/火焰/冰霜/雷击等）
│   └── Skill_Controller/ # 黑洞、分身、水晶、剑等技能控制器
├── Enemy/                # 普通敌人、骷髅敌人与 Boss 状态机
│   ├── Boss/             # Boss 各状态与触发脚本
│   └── Skeleton/         # 骷髅敌人各状态与动画触发
├── Items and Inventory/  # 物品、背包、装备、掉落与物品效果
│   └── Effects/          # 治疗、增益、火焰、冰霜、雷击、冻结等效果
├── Manager/              # 游戏管理（暂停/检查点）与音频管理
├── Player/               # 玩家控制器、玩家状态机与各状态、动画触发
├── Save and Load/        # 存档接口、数据结构、文件读写与可序列化字典
├── Skills/               # 技能基类、技能管理器与具体技能
├── Stats/                # 属性结构、角色属性、玩家属性与敌人属性
└── UI/                   # 游戏 UI、背包、技能树、属性与合成界面
```

## 🛠️ 开发环境

- **Unity Editor：** `2022.3.62f3c1`
- **编程语言：** C#
- **版本管理：** Git

## 🚀 如何运行

1. 克隆仓库：

   ```bash
   git clone https://github.com/XianWang12/RPGGame.git
   ```

2. 使用 Unity Hub「添加」，选择克隆后的 `RPGGame` 文件夹。
3. 使用 Unity `2022.3.62f3c1` 或兼容版本打开项目，等待资源导入与脚本编译完成。
4. 在 `Assets/Scenes` 中打开游戏入口场景 `SampleScene`；它也是构建场景列表中唯一启用的场景。
5. 点击 Unity 编辑器顶部的播放按钮运行。

> 项目使用 Unity 内置输入（`Horizontal` / `Vertical` 轴与 `KeyCode`），请确保输入设置保持默认。

## 🎮 操作说明

| 按键 | 功能 |
| --- | --- |
| `A` `D` / `←` `→` | 左右移动 |
| `Space` | 跳跃（贴墙时可墙跳） |
| `Left Shift` | 冲刺（需解锁） |
| 鼠标左键 | 普通攻击（三段连击） |
| 鼠标右键 | 掷出 / 瞄准剑（需解锁） |
| `Q` | 招架 / 反击（需解锁） |
| `R` | 黑洞技能（需解锁） |
| `F` | 水晶技能（需解锁） |
| `E` | 使用药瓶回复（需装备药瓶） |
| `Tab` | 人物属性界面 |
| `I` | 技能树界面 |
| `U` | 合成界面 |
| `Esc` | 设置 / 暂停菜单 |

## 🔮 项目中体现的设计思路

- **状态与行为分离：** 通过状态类管理玩家、普通敌人与 Boss 的不同行为，把复杂行为拆分为可独立维护的小状态。
- **技能可扩展：** 使用技能基类统一冷却与调用入口，独立技能继承实现自身效果，并可由技能树逐项解锁。
- **数据结构分工：** 使用集合维护物品顺序，并通过字典辅助建立物品数据到实例的映射。
- **属性效果可组合：** 以基础值 + 修饰器列表的方式计算最终属性，装备与限时效果均可叠加。
- **数据与表现解耦：** 属性变化通过回调通知 UI，敌人等级成长与掉落通过属性/组件协作完成。
- **存档模块解耦：** 通过 `ISaveManager` 接口与统一数据对象组织多个系统的数据持久化，新增可存档系统只需实现接口。

## 👤 作者

**XianWang12**  
GitHub：[https://github.com/XianWang12](https://github.com/XianWang12)

项目地址：[https://github.com/XianWang12/RPGGame](https://github.com/XianWang12/RPGGame)

## 📄 许可证

本仓库目前未附带许可证文件。如需转载、修改或分发项目代码，请先确认相关授权情况；在未提供许可证的情况下，使用者不应默认拥有再发布或修改项目的授权。
