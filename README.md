# 井字棋游戏

基于 Unity 开发的井字棋游戏，支持多种玩法模式和 AI 对战。

## 功能特性

- **三种游戏模式**
  - 标准模式：经典 3×3 井字棋
  - 重力模式：棋子会因为重力下落到底部
  - 终极模式：9 个小棋盘组成的大棋盘
 
- 可选是否与 AI 对战，可选玩家先后手

- **三种AI难度**
  - Random：随机落子
  - Basic：基础攻防策略
  - Minmax：MinMax 算法实现

## 技术实现

### 架构设计

采用 MVC 模式和面向接口编程，实现逻辑与显示的完全分离：

- **Model层**：纯 C# 实现的游戏逻辑，不依赖 Unity 引擎
  - `IGameMode` 接口定义游戏规则契约
  - 每种玩法（Standard/Gravity/Ultimate）独立实现该接口
  - 包含落子验证、胜负判定、状态管理等核心逻辑

- **View层**：负责游戏的视觉呈现
  - `IBoardView` 接口定义显示契约
  - 监听 GameManager 事件，更新棋盘显示
  - 处理用户点击输入，转发给 Controller

- **Controller层**：协调 Model 和 View
  - `GameManager` 作为中央控制器
  - 管理游戏流程、处理用户输入、触发 AI 思考

### AI实现

- **策略模式设计**
  - `IAIStrategy` 接口定义AI行为：`GetBestMove(IGameMode)`
  - 不同难度 AI 实现该接口，可随时切换
  - AI 通过 Clone 游戏状态进行模拟，不影响实际游戏

- **Minimax 算法（困难 AI）**
  - 递归搜索所有可能的游戏树分支
  - 使用极大极小值原理选择最优落子
  - Alpha-Beta 剪枝减少不必要的分支搜索
  - 针对 Ultimate 模式单独优化
 
### 游戏模式

- Standard
  - 最标准的游戏模式
  - 使用 Standard Move 和 Standard View
 
- Gravity
  - 继承自 Standard
  - 使用 Standard Move 和 Standard View
  - 棋子会下落到最下面一行。
 
- Ultimate
  - 本质上是 10 个 Standard Gamemode 加入额外的处理逻辑
  - 9 个为基础游戏版图，来自玩家输入； 1 个叠加在上面，是基于 9 个基础版图生成的
  - 使用 Ultimate Move 和专门的 Ultimate View 来显示

## 项目结构

```
Assets
│   
├───Prefab
│       Cell.prefab  // used to generate the board
│       
├───Scenes
│       MainGame.unity  // main game scene
│       Menu.unity  // menu scene, start from here
│       
├───Scripts
│   │   GameManager.cs  // manage the game, connect the view and gamemode
│   │   GameSettings.cs  // store game settings, edit in menu scene
│   │   MenuManager.cs  // used in menu scene to manage the menu
│   │   
│   ├───AI
│   │       BasicAI.cs  // have little logic
│   │       IAIStrategy.cs  // ai interface
│   │       MinMaxAI.cs  // search for best solution
│   │       RandomAI.cs  // just random
│   │       
│   ├───Core
│   │   │   GameEnums.cs  // store every enums used in game
│   │   │   GameResult.cs  // stone game result
│   │   │   IGameMode.cs  // gamemode interface
│   │   │   MoveData.cs  // move data for different gamemode
│   │   │   
│   │   └───GameModes
│   │           GravityGameMode.cs  // standard but have gravity
│   │           StandardGameMode.cs  // standard
│   │           UltimateGameMode.cs  // try this by your self
│   │           
│   └───View
│       │   CameraAutoFit.cs  // so no need to worry about camera
│       │   CellView.cs  // for single cell
│       │   IBoardView.cs  // view interface
│       │   
│       └───BoardViews
│               StandardBoardView.cs  // basic 3x3
│               UltimateBoardView.cs  // 9x9
│               
└───Sprites  // for display pieces
        Piece_O.png
        Piece_X.png
```

## 如何运行

1. Unity 2022.3.51f1 打开项目
2. 打开 Scenes/Menu 场景
3. 运行游戏
