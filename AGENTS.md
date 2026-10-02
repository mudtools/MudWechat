# AGENTS.md

Guidance for AI coding agents working on the **Mud.Wechat** codebase.

## Project Overview

Mud.Wechat 是**企业微信（WeCom）**的 .NET SDK，架构对齐 `D:/Repos/MudFeishu/FeishuV3`（包家族、令牌基座、契约守卫、门禁脚本模式）。覆盖三类应用：

| 应用类型 | `WechatAppType` | 令牌链 |
|---|---|---|
| 企业内部自建应用 | `Internal` | `access_token`（`corpid` + `corpsecret`） |
| 第三方应用（Suite） | `ThirdParty` | `provider_access_token` + `suite_access_token` + 每授权企业 `access_token`（scope） |
| 服务商代开发 | `Provider` | 同 `ThirdParty`，但企业令牌走 `gettoken(corpsecret = permanent_code)` |

## Build / Test / Gate Commands

```bash
dotnet build Mud.Wechat.slnx -c Release                     # 全量构建
dotnet build Mud.Wechat.Work/Mud.Wechat.Work.csproj         # 单项目
dotnet test  Mud.Wechat.slnx                                # 全部测试（4 个测试工程）
dotnet test  Tests/Mud.Wechat.Work.Tests -c Release -f net8.0
dotnet test  Tests/Mud.Wechat.Work.Tests -c Release --filter "FullyQualifiedName~ContractGuards"  # 守卫子集
pwsh ./scripts/verify-build.ps1                             # 质量门禁（构建 + AOT + 测试）
pwsh ./scripts/verify-build.ps1 -SkipTests                  # 仅构建 + AOT
pwsh ./scripts/AddHttpJsonSerializable.ps1                  # DataModels 新增 DTO 批量标注（幂等）
pwsh ./scripts/GenerateJsonContext.ps1                      # 重新生成 DataModels Generated/ 域 JsonContext
pwsh ./scripts/audit-config-keys.ps1                        # 配置属性消费点审计（CI 同款判据）
dotnet format Mud.Wechat.slnx                               # 格式化（未纳入门禁）
```

**本机未安装 pwsh 7**：上述 `pwsh` 命令在此环境会 `CommandNotFoundException`，改用
`powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/verify-build.ps1`（5.1）。

## Quality Gate (`scripts/verify-build.ps1`)

三步，**必须全绿**：

1. **Release 全量构建** — 断言 `: error ` 计数 = 0、`NU1603`（依赖降级）计数 = 0。
2. **AOT strict 冒烟** — `Get-ChildItem -Recurse` 定位全部源项目（排除 `Tests`/`Demos`/`obj`/`bin`），逐项目以
   `-c Release -f net8.0 -p:AotStrictMode=true --no-incremental` 构建，断言每项目的
   **编译错误 = 0**、`IL\d{4}` 诊断 = 0、`IL3050` = 0。
   - `-Recurse` 必须保留：去掉后 `Get-ChildItem` 找不到嵌套的 `.csproj`，AOT 步骤会静默空跑（假绿）。
   - `--no-incremental` 必须保留：MSBuild 的 `CoreCompile` 只比对输入/输出时间戳、**不比对 csc 命令行**，
     紧接在步骤 1 之后发起的 strict 构建会被判定为「已最新」而整体跳过编译，报告 0 诊断（假绿）。
3. **单元测试** — 逐测试工程以单 TFM `net8.0` 运行（`Tests/Directory.Build.props` 遮蔽根 props），
   断言 TRX 存在、`total > 0`、`failed = 0`。产物落 `test-reports/`（已 gitignore）。

**已知假绿陷阱（勿"优化"掉）**：只断言诊断计数、不断言编译错误 —— 构建本身失败时诊断计数仍为 0。
本脚本对步骤 2 **同时**断言「编译错误」与「IL 诊断」。

**门禁脚本可直接运行（Windows PowerShell 5.1 与 pwsh 7 均可）**：`scripts/*.ps1` 为 UTF-8 **含 BOM** + 中文注释
（BOM 于 2026-09-30 补齐）。历史坑：**无 BOM** 的 UTF-8 中文脚本在 5.1 下被按 ANSI 解码，报
`Missing ')' in method call` / `The string is missing the terminator`（解析失败，非脚本逻辑问题）——
改写脚本后请确认编辑器没有把 BOM 剥掉。
`audit-config-keys.ps1` **无 `-Strict` 开关**：任何「无消费点」的配置属性都会使其 `exit 1`（与 CI 判据一致）。

## CI（`.github/workflows/dotnet-publish.yml`）

步骤顺序：Build → **诊断白名单断言** → `verify-build.ps1 -SkipTests` → `audit-config-keys.ps1` → `dotnet test --no-build`（TRX 解析摘要）。

- Release 构建日志**必须为 0** 的桶：`NU1603`、`CS1750`、`HTTPCLIENT0\d\d`、`FORM0\d\d`、`EHSG0\d\d`、
  `MUD00[1-4]`、`IL\d{4}`、`AOT00[1-5]|AOT007`。
- **已接受风险、断言为 0 即假红**的两桶：`AOT006`（未登记的 `[HttpJsonSerializable]` DTO，Warning；
  strict 步骤才升为 Error）、`MUD005`（Query/Path 令牌注入，企业微信官方契约要求）。CI 只 INFO 打印其计数。
- 发布链：`workflow_dispatch` → semantic-release 改写 `Directory.Build.props` 的 `<Version>`（**版本唯一来源**）→
  tag `v*` → `dotnet pack` 后断言恰 **4 个 nupkg**（Tests 均 `IsPackable=false`）→ OIDC 临时 Key 推 nuget.org。
- `DOTNET_VERSIONS` 必须**多行**（每行一个 SDK 版本）；写成空格分隔会静默归一为单版本，随后以
  「testhost 无法启动」形式炸开（见 workflow 内注释）。
- workflow 头部注释称「nuget.config 只声明 nuget.org」**已过时**（见 Dependency Version Policy）——以 nuget.config 实际内容为准。

**AOT006（P0-5）已修复**：`Abstractions` 的 **8 个** `[HttpJsonSerializable]` 领域模型由
`Authentication/Models/AuthenticationJsonContext.cs`（手写）覆盖；`DataModels` 的全部传输 DTO 由
`Generated/` 目录的 **20 个域上下文**覆盖（`scripts/GenerateJsonContext.ps1` 按 `[HttpJsonSerializable]`
标注生成，勿手改）。主包 `WechatJsonResolverExtensions` 将 20 个上下文（+1 手写 `AuthenticationJsonContext`）一并合并进组件序列化管线。
**新增 `[HttpJsonSerializable]` 类型必须同批重跑 `scripts/AddHttpJsonSerializable.ps1`（标注）+
`scripts/GenerateJsonContext.ps1`（登记；Abstractions 域则手写登记到 `AuthenticationJsonContext`）**，
否则 `AotStrictMode=true` 下 `AOT006`（severity = error）会让门禁步骤 2 直接失败 —— 该诊断本身就是漂移守卫。
脚手架核对（组件官方工具）：`dotnet tool install -g Mud.HttpUtils.JsonContextScaffolder` →
`mud-jsonctx --project Mud.Wechat.Work.DataModels\Mud.Wechat.Work.DataModels.csproj --dry-run`。

## Target Frameworks & Language Constraints

`Directory.Build.props`：`TargetFrameworks = netstandard2.0;net6.0;net8.0;net10.0`、
`LangVersion = 13.0`、`Nullable = enable`、`ImplicitUsings = enable`、`TreatWarningsAsErrors = false`、`Version = 1.0.3`。

**`netstandard2.0` 无 `IsExternalInit` polyfill**，因此在该 TFM 下：

- **禁止 `init` 访问器、禁止 `record` 类型 / `with` 表达式**（会报 `CS0518`/`CS0656`）。用 `get; set;` 或构造器。
- `ArgumentNullException.ThrowIfNull` 不可用；用 `x ?? throw new ArgumentNullException(nameof(x))`。
- `string.IsNullOrEmpty` / `IsNullOrWhiteSpace` **无 `[NotNullWhen]` 标注**，流分析不会收窄可空引用。
  需显式 `x == null` 判断或 `is { Length: > 0 }` 模式。
- 条件编译用 `#if NET6_0_OR_GREATER` / `#if NET8_0_OR_GREATER`；**禁用** `Math.Clamp`（用 `Math.Min/Max`）。

`netstandard2.0` 存在**大量既有 CS86xx 可空警告**（Abstractions 的令牌/仓储/回调相关文件）。属既存形态，
门禁只统计「编译错误」与 AOT IL 诊断，**不因 CS 警告失败** —— 不要为消警告做大范围重构。

**`Tests/Directory.Build.props` 遮蔽根 props**：测试项目单 TFM（`net8.0`）。在此新增任何治理属性
（如 `WarningsAsErrors`）必须同时写入 `Tests/Directory.Build.props`，否则测试工程成为门禁盲区。

## AOT / Trim 合规

- `net8.0` / `net10.0` 默认启用 AOT/裁剪分析；`AotStrictMode=true` 时把 `IL2026;IL2046;IL2050;IL2057;IL2067;IL2070;IL2072;IL2075;IL2080;IL3050` 提升为错误。
- `WarningsAsErrors` 常驻含 `AOT001;AOT002;AOT003;AOT004;AOT007`。
- **禁止**调用反射版 `JsonSerializer.Serialize<T>(T, JsonSerializerOptions)` / `Deserialize<T>`。
  统一走 `JsonTypeInfo`（DataModels 域 `JsonContext`，`Generated/` 源生成）或
  `WechatJsonResolverExtensions` 合并解析器。
- **DataModels 的 JSON 上下文为生成物**（对齐 `Mud.Feishu.DataModels` 模式）：DTO 标注
  `[HttpJsonSerializable]`（`SerializerClassName` = 命名空间域段，根命名空间直属文件归 `Common`），
  `scripts/GenerateJsonContext.ps1` 生成 `Generated/*JsonContext.g.cs`（**提交进版本控制**，勿手改）。
  生成上下文为 `internal`，经 `InternalsVisibleTo` 供主包与测试直读。
  已知边界：开放泛型 `WechatChatbotResponse<>`（STJ 源生成器不生成其元数据，SYSLIB1030）不登记；
  关闭 `--auto-derived-types`（派生根对本仓库全是冗余且会拖入该开放泛型），工具运行期的
  `AOT003` 警告（多态缺 `[JsonDerivedType]`）在本仓库为已知误报——反序列化目标恒为具体 DTO 类型。
- 配置绑定为**源生成**（`EnableConfigurationBindingGenerator=true`）。后果：
  - 配置 DTO **禁止 `required`**（生成器以 `new T()` 构造 → `CS9035`）；改在 `Validate()` 里校验（见 `WechatAppConfig`）。
  - 绑定必须走 `Configure<T>(o => section.Bind(o))`，**不要**用 `Configure<T>(IConfiguration)` 重载
    —— 其反射绑定调用点无法被源生成器拦截，会破坏 `IL2026`/`IL3050` 净零。
- `UnconditionalSuppressMessageAttribute` 在 `net10.0` 为 `internal`，用户代码不可引用；必要时用
  `#pragma warning disable IL2026, IL3050` 并附理由注释。

## Project Structure

```
Mud.Wechat/
├── Mud.Wechat.Work/             # 主包：接口声明、服务、DI、模块注册
│   └── Interfaces/              # 按功能族分目录：Authentication/、Contacts/（通讯录六域，Export 独立子目录）、
│                                # ExternalContact/（客户联系六域：FollowUser + Customer + Tag + JobInheritance + ResignedInheritance + GroupChat）、CorpGroup/（上下游三接口族：CorpGroup + ChainContacts + Rules）、
│                                # Security/（安全管理三接口族：Security + SecurityVip + SecurityOperLog，官方仅自建开放）、
│                                # Message/（消息推送四接口族：Message + AppChat + SchoolMessage + SmartSheetGroupChat；template_msg 仅第三方差异端点，群聊会话/学校通知/智能表格群聊官方仅自建开放）
├── Mud.Wechat.Work.Abstractions/# 抽象：令牌基座、多应用、配置、仓储、枚举、异常
├── Mud.Wechat.Work.DataModels/  # 官方 DTO（[HttpJsonSerializable] 标注）+ Generated/ 域 JsonContext（生成）
│   ├── Contacts/                # 通讯录域 DTO 分组：Users / Department / Tags / ContactRules / Batch（RequestModel/ 仅作目录组织）
│   ├── ExternalContact/         # 客户联系域 DTO 分组：FollowUser / Customer / Tag / JobInheritance / ResignedInheritance / GroupChat（RequestModel/ 仅作目录组织）
│   ├── CorpGroup/               # 上下游域 DTO 分组：基础 + ChainContacts + Rules（RequestModel/ 仅作目录组织）
│   ├── Security/                # 安全管理域 DTO 分组：38 型单一命名空间（RequestModel/ 仅作目录组织；含文件防泄漏/设备管理/截屏录屏/域名IP/高级功能账号/操作日志）
│   └── Message/                 # 消息推送域 DTO 分组：86 型单一命名空间（RequestModel/ 仅作目录组织；发送应用消息/更新模版卡片/撤回/群聊会话/学校通知/智能表格自动化群聊 + 模板卡片家族）
├── Mud.Wechat.Work.Callback/    # 回调接收（AES 解密、事件解析、分发）+ HTTP 中间件（多应用路由、URL 验证、
│                                # 类型化处理器/拦截器分发，Events/ 强类型事件 DTO；ASP.NET 引用条件化，见回调域）
├── Mud.Wechat.Redis/            # Redis 分布式存储扩展（四存储端口 Redis 实现 + 连接基座 + DI 编排，RD11 单依赖 Abstractions）
├── Tests/                       # 测试工程（镜像源结构，单 TFM net8.0）
├── scripts/                     # verify-build.ps1 / audit-config-keys.ps1
├── .docs/                       # 方案与设计文档（中文）
├── Directory.Build.props        # 全局 MSBuild 属性
└── Mud.Wechat.slnx              # 解决方案
```

命名空间与包名一致：`Mud.Wechat.Work` / `Mud.Wechat.Work.Abstractions` / `Mud.Wechat.Work.DataModels` / `Mud.Wechat.Work.Callback` / `Mud.Wechat.Redis`。
依赖方向单向：`Work → {Abstractions, DataModels}`、`Callback → {Abstractions, DataModels}`、`Abstractions → DataModels`、`Redis → Abstractions`。
**`Callback` 不得引用主包 `Work`**（授权自动化解耦即为此，见下）；**`Redis` 不得引用主包 / Callback**
（重放守卫接口上移 Abstractions 后单依赖，RD11；顺序守卫对回调注册的探测经 InMemory 实现全名字符串
匹配，全名漂移由 RD-G5 锁定）。

**目录与命名空间现状（2026-10-01 重构后）**：通讯录六域接口落 `Interfaces/Contacts/`（Export 有独立子目录
`Contacts/Export/`；接口命名空间一律为 `Mud.Wechat.Work`）；DTO 迁入 `DataModels/Contacts/{域}/`，
命名空间为 `Mud.Wechat.Work.DataModels.Contacts.{域}`（Department/Tags/ContactRules/Batch/ContactRules/Export），
**例外：Users 域命名空间为 `Mud.Wechat.Work.DataModels.Contracts.Users`**（与目录名不一致，属既存形态；
新增 Users DTO 请沿用该命名空间直至统一决策）。
`RequestModel/`、`ResponseModel/` 仅作目录组织，**命名空间不含该目录段**。

**客户联系域（2026-10-01 新增）**：接口落 `Interfaces/ExternalContact/`，命名空间 `Mud.Wechat.Work`；
DTO 落 `DataModels/ExternalContact/{FollowUser,Customer,Tag,JobInheritance,ResignedInheritance,GroupChat}/`，命名空间
`Mud.Wechat.Work.DataModels.ExternalContact.{FollowUser,Customer,Tag,JobInheritance,ResignedInheritance,GroupChat}`。
模块枚举为独立的 `WechatModule.ExternalContact`（`AddExternalContactApi()` → 生成器
`AddExternalContactWebApiHttpClient()`，注册组 `ExternalContact`），与通讯录 `Contact` 模块平行。
2026-10-01 增补：客户标签管理域（Tag，9 端点）与在职继承域（JobInheritance，3 端点）——官方对三类应用
开放面完全一致，端点全部收敛父接口，三个应用类型子接口均为空标记（同通讯录标签域形态）。
2026-10-02 增补：离职继承域（ResignedInheritance，4 端点）与客户群管理域（GroupChat，3 端点）——同为
三类应用开放面完全一致的形态，端点全部收敛父接口，三个应用类型子接口均为空标记。
**离职继承 ≠ 在职继承**：两者都含「分配客户 + 查询接替状态 + 分配客户群」三段式，但路由不同——
离职客户的接替为 `resigned/transfer_customer` / `resigned/transfer_result`、离职群接替为
`groupchat/transfer`（在职为 `transfer_customer` / `transfer_result` / `groupchat/onjob_transfer`，
"onjob" 拼写属官方契约）；另有「获取待分配的离职成员列表」（`get_unassigned_list`）为离职继承独有。

**消息推送域（2026-10-02 新增）**：接口落 `Interfaces/Message/`，命名空间 `Mud.Wechat.Work`；DTO 落
`DataModels/Message/`，命名空间 `Mud.Wechat.Work.DataModels.Message`（`RequestModel/` 仅作目录组织）。
模块枚举 `WechatModule.Message`（`AddMessageApi()` → 生成器 `AddMessageWebApiHttpClient()`，注册组 `Message`）。
三接口族：**发送应用消息族**（Message：`/cgi-bin/message/send` 12 种 msgtype 每型一端点方法 +
`update_template_card` + `recall`）官方对三类应用开放完全一致 ⇒ 公共端点收敛父接口，自建/代开发子接口
空标记、第三方子接口另持 `template_msg` 差异端点（94515，同路由 `/cgi-bin/message/send`，官方页面未单独
标注路由，以正文表述为准）；**群聊会话族**（AppChat：appchat/create + update + get + send）官方仅自建开放
（可见范围须根部门、第三方明示不可调用）⇒ 父接口零端点 + 仅自建子接口承载端点；**家校学校通知族**
（SchoolMessage：`/cgi-bin/externalcontact/message/send` 8 种 msgtype）同仅自建形态；
**智能表格自动化创建的群聊族**（SmartSheetGroupChat：`wedoc/smartsheet/groupchat/{list,get,update}` 全 POST，
需配置到文档「可调用应用」且可见范围含根部门；list/get 并发限制 20、update 并发限制 10 且**同一群聊修改必须串行**）
亦仅自建（100989/101028/101029）。
**多态落位决策**：msgtype/card_type 不做运行时多态（AOT 源生成按声明类型序列化），每个 msgtype 一个
端点方法 + 请求 DTO（同路由多方法），官方各 msgtype 参数表差异（safe 有无、id 转译支持面、
mentioned_list 仅群聊）在 DTO 层面精确表达；模板卡片 `TemplateCardBody` 为发送/更新两端点共用的扁平结构
（card_type 判别 + 全可选嵌套，replace_text/disable 仅更新接口支持）。
**响应形态陷阱**：`message/send` 的 invaliduser 为竖线分隔字符串，`update_template_card` 的 invaliduser
为字符串数组——两类响应 DTO 不共用；学校通知响应为 invalid_parent_userid / invalid_student_userid /
invalid_party 三个数组。**限频契约**：发送应用消息每应用「账号上限数 × 200」人次/天、同一成员
30 次/分 + 1000 次/时（超限丢弃）；appchat/send 每企业 2 万人次/分 + 规模分档小时额度、成员级
200 条/分 + 1 万条/天（超限静默丢弃且不报错，推送成功 ≠ 全员送达）；创建群 1000 个/天、修改群 1000 次/小时。
**「接收消息与事件」不在本域**：消息接收为企业微信回调推送（XML），由 `Mud.Wechat.Work.Callback`
中间件承载，本域全部为发送侧 HTTP API。

## Dependency Version Policy (Mud.HttpUtils)

- 全仓库锁定 `Mud.HttpUtils` / `Mud.HttpUtils.Generator` **同一版本**（当前 **3.0.0**），由
  `WechatContractGuards.MudHttpUtils_PackageReference_ShouldBeSingleVersion` 守卫「全仓库单一版本」
  （防混版 `TypeLoadException`）。守卫只断言「版本集合大小 = 1」，不硬编码版本号，升级时无需改守卫；
  运行时版本由 `MudHttpUtilsUpgradeSmokeTests` 断言（≥2.0.9 且 Major ≥3）。
- **`nuget.config` 当前含本机源 `mudhttputils-local` → `D:\Repos\MudHttpUtils\artifacts`**（3.0.0 尚未上架
  nuget.org，见 nuget.config 内注释）。restore **只在存在该路径的机器上可用**；既定收尾动作是官方包上架后
  删除该源、把 `PackageReference` 指回 nuget.org 版本——勿在上架前擅自删除（会断 restore），也勿以为这是长期形态。
- 本地组件开发时的缓存陷阱（组件仓库 `D:/Repos/MudHttpUtils`）：NuGet 全局包缓存按 `id + version` 计价，
  **同版本重新打包不会失效下游缓存**；须 `dotnet nuget locals global-packages --clear` → `dotnet clean` → `dotnet build`，
  `clean` 步骤**不可省**（增量构建会把新旧组件程序集并置，运行时爆 `TypeLoadException`）。

## 企业微信领域契约（不可违反）

- **K1**：代开发 `permanent_code` 语义是「应用 secret」→ 走 `gettoken`（`corpsecret = permanent_code`）；
  第三方应用 `permanent_code` 是「授权码」→ 走 `get_corp_token`。二者都在 `CorpTokenManager` 内按 `AppType` 分流，**不新建管理器**。
- **K2**：代开发 `template_id` 即 `suite_id`（`dk` 开头）⇒ **`WechatAppConfig` 不提供独立 `TemplateId` 配置项**
  （2026-09-30 删除：原属性仅被 `Validate()` 用于与非空 `SuiteId` 比对 = 死配置，且使 `audit-config-keys.ps1` 判红）。
  删除后「模板 id ≠ suite_id」这一非法状态**在类型层面不可表达**（强于"启动期校验一致性"）；
  需要模板 id 的**接口参数**（如 `get_customized_auth_url` 的 `templateid_list`）由宿主显式传入，与配置面无关。
- 授权安装链接前缀：`https://open.work.weixin.qq.com/3rdapp/install`。
- 令牌注入统一走 **Query**（企业微信契约，非 Header），触发组件 `MUD005` 已知接受风险；注入白名单由
  `WechatContractGuards.QueryTokenInjection_ShouldBeLimitedToWechatOfficialContractInterfaces` 锁定为
  **授权流接口 + 通讯录六域 + 客户联系六域（企业服务人员管理/客户管理/客户标签管理/在职继承/离职继承/客户群管理）+ 上下游三接口族（CorpGroup/ChainContacts/Rules）+ 安全管理三接口族（Security/SecurityVip/SecurityOperLog）+ 消息推送四接口族（Message/AppChat/SchoolMessage/SmartSheetGroupChat）父/子接口共 71 个**（见 Contract Guards 表 G5），新增 Query 注入接口须评估后显式扩展守卫。
- v2 端点：`/cgi-bin/service/v2/get_permanent_code`、`/cgi-bin/service/v2/get_auth_info`；
  `get_customized_auth_url` 以**显式 Query 参数** `provider_access_token` 传令牌（**不带 `[Token]`**，不放宽白名单）。
- `TokenKey` 布局：**三段式 `{tokenType}:{appKey}:{scopeKey}`**（如 `Wechat.AccessToken:default:default`；
  由 `WechatAppTokenManagerBase.BuildCache` 的 `storeKeyMapper` 构造）；`WechatTokenTypes` 一律 `"Wechat."` 前缀，与组件通用 `TokenTypes` 隔离。
- **`AppKey` 形状受约束**：`[A-Za-z0-9]` 开头 + 仅 `[A-Za-z0-9._-]` + ≤128（`WechatAppKeyValidator`，经
  `WechatAppConfig.Validate()` 单点收敛）。理由：AppKey 参与持久化键与命名 HttpClient 名，含 `:` 会造成**键别名**（跨应用令牌串号）。
- 企业级令牌**一企一份**由 `TokenManagerBase` 的 scope 机制承担（`scopeKey = authCorpId`），不依赖 `IWechatTokenStore`。
  企业级令牌**不经声明式 `[Token]`**（由宿主/编排服务显式 `GetTokenAsync(new[]{ authCorpId })` 获取）⇒
  errcode 恢复**必须显式传 scope**：`InvalidateTokenAsync(appKey, AccessToken, new[]{ authCorpId })`；
  以默认作用域失效对已缓存的企业令牌是**空转**（能力边界，由 `CorpTokenManagerScopeIsolationTests` 锁定）。
  `CorpTokenManager` 读取环境 `SetCorp` 上下文须通过**两级校验**：① 上下文 `AppKey` 归属一致；② 上下文 `authCorpId` 与 scope 一致。
  `WechatCorpContext.SetCorp` 的 `authCorpId` **必填**（null/空白即抛；M7）——它是企业令牌 scope 的唯一来源，
  与 `appKey=null`「未声明归属」语义显式区分；`InvalidateTokenAsync` 的 `scopes` 非空但全为空/空白串判为编程错误 fail-fast
  （`scopes=[]`/null 保持「全部」既有语义）。
- **退役清库批量能力（W1/M10）**：`IWechatTokenStoreBatchRemove : IWechatTokenStore`（可选实现）——管理器清库
  先 `is` 探测，命中走一次 `RemoveRangeAsync`，未实现回退逐键（宿主自定义 store 零破坏）；
  形态为「调用方算键 + 实现方批删」（键按中间段 appKey 匹配，前缀扫描不适用）。
- `IWechatCorpAuthStore` 为**复合键 `(AppKey, AuthCorpId)`**；`IWechatSuiteTicketStore` **按 `suiteId` 分槽**（多套件/多代开发模板互不覆盖）。
  默认实现仅进程内，多实例部署**引用 `Mud.Wechat.Redis`**（`AddWechatRedis`，须先于 `AddWechatApp`/`AddWechatCallback`
  调用，颠倒顺序注册期 fail-fast）或由宿主提供分布式实现（`TryAdd` 前置注册覆盖；宿主预注册的自定义实现按契约胜出）。
  四个存储端口（含 `IWechatCallbackReplayGuard`，**接口本体落 Abstractions.TokenManager 域，RD11**；InMemory 实现留 Callback 包）
  由 Redis 包单依赖 Abstractions 实现并共享连接基座。**SE.Redis 3.3.0 两个陷阱**（改 Redis 包代码必读）：
  ① `RedisTimeoutException` 直接继承 `TimeoutException` 而非 `RedisException`——存储层捕获须用
  `WechatRedisErrors.ShouldWrap`（`is RedisException or RedisTimeoutException`），裸 `catch (RedisException)` 会漏超时；
  ② `StringSetAsync` 存在经典 4/5 参（无默认值、隐藏）、keepTtl 6 参、`Expiration`/`ValueCondition` 全默认四套重载，
  裸 2/3 位置参调用的重载决胜不确定——生产调用一律以命名参数显式钉住 keepTtl 形态（`keepTtl: false` / `when: ...`）。
- **DI 桥接不变量**：`IAppContextHolder`、`IAppContextSwitcher`、`IWechatAppContextSwitcher` **必须是同一实例**
  （组件 `AddMudHttpClient` 内部会 `TryAdd IAppContextHolder`，故 SDK 的注册必须在 `AddMudHttpClient` **之前**）。
  破坏该不变量 ⇒ 声明式 `[Token]` 客户端读到的环境上下文恒为 `null`，多套件静默回退默认应用令牌。
- **`WechatAppManager` 直接实现 `IAppManager<IWechatAppContext>`（不继承组件 `DefaultAppManager<T>`）**：
  基类另有影子注册表（`_apps`）与非 virtual 写入口，会让 `RegisterApp`/`UpdateApp`/`TrySetDefaultApp` 静默写影子表
  而 `GetApp`/`DefaultAppKey` 读不到。故：注册表**单一来源**（本类 `_configs` + `_lazyContexts`）；
  `RegisterApp`/`UpdateApp`/`RegisterSwitcherFactory` 显式 `NotSupportedException`。
  配置读取一律走 `ConfiguredConfigs` / `TryGetConfig`（**非物化**）；`TryGetApp` 会构造命名 HttpClient/DI scope/Timer，
  仅用于「确实需要上下文」的场景（回调的 `SuiteId → appKey` 匹配不得使用它）。
  **默认应用链（M2）**：全部应用经 `RemoveApp` 移除后 `DefaultAppKey` 置 `null`（对齐组件契约），
  `GetDefaultApp` 抛明确错误；重新 `AddApp` 时兜底提升为当前应用（`??=`）。
  **重建白名单（M4）**：`InvalidOperationException` 不属瞬时装配故障（DI 解析失败为确定性错误，直抛不重建）；
  瞬时白名单限于 IO 型异常组（HttpRequestException/TimeoutException/IOException/SocketException）。
  **退役队列（M3/M5）**：清库任务脱泵 fire-and-forget（飞行登记）、停机限时（默认 5s）排空；
  停机后到达的退役入队立即 Dispose、下线清库任务丢弃（令牌随 TTL 过期）；`RemoveApp` 删除顺序恒为
  `TryRemove` 先于配置删除（「返回 false ⇒ 零突变」，MA1）。
  **装配孤儿回收（M1）**：`CreateAppContext` 经 `WechatOwnedResourceTracker` 登记 scope 之外产物
  （令牌管理器持组件基类维护 Timer），装配中途失败逆序确定性回收后原样重抛。
- 授权编排：`IWechatWorkAuthorizationService`（换码/刷新/撤销/枚举）；换码**单飞门 + 结果记忆**以
  **`(appKey, authCode)` 复合键**为粒度（含长度前缀拼接）；共享任务用 `CancellationToken.None` 承载，
  各调用者经 `AwaitSharedAsync` 独立取消；飞行条目**仅创建者移除**。`RevokeAuthorizationAsync` 顺序为
  **先失效令牌、后删库**，且**仅失效本 `appKey`** 令牌。
  策略统一落 `WechatAuthorizationOptions`（`WechatAuthorization` 节），**不得**把编排字段塞进 `WechatAppConfig`。
- 回调自动化：Abstractions 定义 `IWechatAuthorizationCoordinator`，**主包实现**，`Callback` 经
  `IServiceProvider.GetService<IWechatAuthorizationCoordinator>()` **惰性可选解析**；未安装主包授权模块时
  首次 `Warning` 后**降级不抛**。`change_auth` 本地无记录时仅告警、**不发 `get_auth_info`**；
  `cancel_auth` **清理范围恒为 `SuiteId` 命中集**：未命中（含 `SuiteId` 缺失）**只告警不删库**
  ——`(AppKey, authCorpId)` 是**每套件独立**的授权记录，回退「全部应用清理」会删除其它套件的有效授权。
  协调器三个逐 appKey 循环（换码/刷新/撤销）**单应用失败必须记 Error 后继续**（P3-1 逐应用隔离，
  与处理器直连分支的捕获面对齐）；`OperationCanceledException` **不在捕获面**（取消必须穿透）。
- **回调抗重放不变量**：验签通过后必须过两道 fail-closed 闸——① 时间戳时效窗口 ±300s（缺失/非数字即拒）；
  ② 一次性指纹去重（SHA1 指纹，不得落盘密文本身）。**指纹闸位于「解密 + receiveid 校验成功」之后、
  事件返回之前**（P1-1/D2 后移：解密成功即证明报文经仅企微与我方共知的 AESKey 验证可信；解密失败不消耗指纹，
  官方重试可重新进入管线）——修改回调入口时**不得**把指纹闸移回解密之前，也不得绕过（CB9 锁定）。
  **GET URL 验证（echo）只过时效闸、不消费指纹**（幂等验证，同 echostr 二次保存配置必须成功，D8/D5；CB6 锁定）。
  分布式实现（`Mud.Wechat.Redis`）故障时守卫异常**必须上抛**（RD3 fail-closed：上抛 → 回调 5xx → 官方 96238
  重试；**禁止**吞异常返回 `true` 放行重放、或返回 `false` 静默丢事件），空键返回 `false` 且不触达存储（与 InMemory 对齐）。
- **回调接收面（Callback 包）**：**多应用凭据 = `WechatCallbackOptions.Apps` 字典，路由 = 路径
  `/{GlobalRoutePrefix}/{AppKey}`**（v1.2 D3）——`AddWechatCallback` 注册（返回建造者链式注册处理器/拦截器），
  `IConfiguration` 重载为**注册期急切绑定**；接收失败统一抛 `WechatCallbackException : InvalidOperationException`
  （`Kind` 映射失败类别，P2-2）；加解密按官方 **32 字节块 PKCS7** 手工补位/剥离（P0-1，
  **禁用 .NET 内置 16 块 `PaddingMode.PKCS7`**，CB8 锁定）。
  **形态取舍（2026-10-02 合并定案）**：远端曾按「接收方 ID（外层 XML ToUserName）路由的统一注册表 +
  `IWechatCallbackUrlVerifier`」实现多套件，能力已被 v1 多应用路径路由覆盖（每套件一个 `Apps` 条目，
  各自独立 Token/AESKey/接收方 ID）；`WechatCallbackOptionsRegistry`/`WechatCallbackReceiverGroup`/
  `IWechatCallbackUrlVerifier` 不并入主干，勿再以「注册表形态」描述回调接收面。
- **`WechatAppCallbackOptions.ReceiveId` 语义是「接收方 ID」**（v1.2 起由 `CorpId` 重命名，语义泛化）：企业自建回调为企业 `CorpId`，**套件回调为 `SuiteId`**。
  非空时校验解密明文的 `receiveid`，不一致即拒（该属性是唯一消费点，不得改为死配置）；留空（通讯录同步助手）
  或明文未携带 `receiveid` 时跳过校验并一次性告警（官方「个人主体第三方为空串」兼容，90968）。
  v1.2 起该属性迁入 `WechatCallbackOptions.Apps[appKey].ReceiveId`（`WechatAppCallbackOptions`，**与主配置同类文件**
  纳入 audit 扫描）；顶层单体字段（PushToken/PushEncodingAESKey/CorpId）已删除，`Apps` 为回调凭据唯一来源。
  **应用类型 × 回调通道区分（CB10~CB13）**：`WechatAppCallbackOptions` 另暴露 `AppType`（`WechatAppType`，默认 `Internal`）
  + `Channel`（`WechatCallbackChannel`，`App=1` 应用数据通道 / `Suite=2` 套件指令通道，默认 `App`）；`Validate()` 校验
  「自建应用不得占用套件通道」「套件通道仅第三方/代开发」两条非法组合。`ValidateReceiveId`（CB11）按「AppType × Channel」
  三元分流 receiveid 语义：自建 App / 第三方·代开发 Suite = 静态 `ReceiveId`；第三方·代开发 App = 动态授权企业 CorpId
  （比对外层 `ToUserName`）。`IsEventFamilyAllowed`（CB12）实现开放面矩阵：授权族→Suite+第三方/代开发；上下游→App+自建；
  通讯录/异步→App；Unknown→不拦截。分发器合法性闸（CB13）先于拦截器 `BeforeHandleAsync`，不适用族返回 `Rejected`（→200 不重推）。
- **回调域（2026-10-02 新增，对齐 `.docs/MudWechatWork-回调解决方案-v1.md` v1.2）**：
  - **包形态**：`Callback` 引入 ASP.NET（`netstandard2.0` 用 `Microsoft.AspNetCore.Http 2.3.9` 包、`net6+` 用
    `FrameworkReference`，对齐飞书）；中间件为**经典约定式**（RequestDelegate，不进 DI），宿主 `UseWechatWebhook()` 一行接入。
  - **事件信封上移**：`WechatCallbackEvent`/`IWechatCallbackEventHandler`/`IWechatCallbackEventInterceptor`/
    `WechatCallbackEventTypes` 落 `Abstractions.Callback` 命名空间（CB7 锁定：该目录不得出现 XML 类型）；
    XML→信封解析留在 `Callback.WechatCallbackReceiver`。**AuthCorpId ← FromUserName 兜底仅限授权族**
    （D10：change_contact 的 FromUserName 固定 sys，无差别兜底会伪造授权企业）。
  - **事件类型键**：`EventTypeKey` = `InfoType`（非空）→ `ChangeType` → `Event`（D4）；处理器
    `SupportedEventType` 空串 = 兜底（内置 `WechatCallbackHandler` 即此形态，文件**留包根目录**——G9 按路径断言）。
  - **注册表**：组合根期急切注册、**无 Freeze**；通配键 `"*"`（`WechatCallbackOptions.WildcardAppKey`）双重语义 =
    通讯录同步助手路由 + 全局处理器/拦截器桶（D11）；匹配顺序 appKey 专属精确 → 全局精确 → 专属兜底 → 全局兜底。
  - **分发与超时**：同步分发 + 软超时（默认 4500ms，**必须 < 企业微信 5s 契约**）；超时/拦截器中断 → 503
    触发重推；指纹在分发前消费，重推同指纹将被 403（fail-closed 优先于 at-least-once，处理器须幂等）；
    单处理器异常隔离（LogError 后继续，结果仍 Handled）。
  - **回调事件 DTO**（`Callback/Events/`）不复用 DataModels 的 JSON DTO（XML vs JSON、逗号/竖线串 vs List、
    权限降权语义三重差异，见方案 §5.5）；`MaxConcurrentEvents` 信号量容量为构造期快照（热更不改容量）。
  - **上下游事件族（2026-10-02 增补，官方 95796/95797）**：`Event=change_chain` + 9 个 ChangeType
    （空间/分组/企业三族），信封带 `ChainId`、`IsChangeChain`；DTO 按**官方报文结构族**建 3 型
    （`ChainChangedEvent`/`ChainGroupChangedEvent`/`ChainCorpChangedEvent`，同族字段完全一致，
    ChangeType 经信封判别，非逐事件 DTO）；`batch_job_result` 官方存在**双报文布局**
    （通讯录 90973 顶层节点 vs 上下游 95797 `BatchJob` 包装节点），解析器同批兼容、宿主按 `JobType`
    （`import_chain_contact`）分流。开放面仅自建应用（配置到「上下游-可调用接口的应用」），
    第三方/代开发暂不支持；上下游系统应用自身触发的变更不回调。

## Code Style

### 文件头（所有源文件必须以此开头）

```csharp
// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------
```

### 命名与风格

- **文件域命名空间**（`namespace X.Y;`）；公共导入集中 `GlobalUsings.cs`。
- 文件头版权块 ≠ `<auto-generated>`；生成文件（`Generated/*JsonContext.g.cs` 等）不要手写（重跑 scripts/GenerateJsonContext.ps1）。
- 命名：类型/方法/属性 PascalCase，接口 `I` + PascalCase，私有字段 `_camelCase`，参数 camelCase，异步方法 `...Async` + `CancellationToken cancellationToken = default`。
- 常量类用 `static class` + `public const string`（如 `WechatTokenTypes`、`WechatErrorCodes`）。
- 错误：参数用 `ArgumentNullException`，状态用 `InvalidOperationException`；统一异常类型 `WechatWorkException`（含 `ThrowIfFailed`）。
- DI：构造注入、字段 `readonly`；多应用基座相关 Singleton 必须经 `IServiceScopeFactory` 建 scope（防 Captive Dependency），退役上下文入退役队列（默认 300s 宽限）后再 Dispose。
- 公共 API 必须写 XML 文档（`<summary>` / `<param>` / `<returns>` / `<exception>`）；门禁开启 `GenerateDocumentationFile`。
  注意 cref 必须可解析（`<paramref name="x"/>` 要求参数存在，否则 `CS1734`；跨包 cref 写全限定或用 `<c>` 兜底）。

## Contract Guards（`Tests/**/ContractGuards/`）

新增/修改契约面时**必须同批**更新守卫。现有 9 条通用守卫（`WechatContractGuards.cs`，G1~G9；
G8 的运行期同实例断言在 `WechatServiceCollectionExtensionsTests`）+ 域守卫组
（`WechatUsersContractGuards.cs` U1~U4、`WechatDepartmentsContractGuards.cs` D1~D4、
`WechatTagsContractGuards.cs` T1~T4、`WechatContactRulesContractGuards.cs` CR1~CR4、
`WechatBatchContractGuards.cs` B1~B4、`WechatExportContractGuards.cs` E1~E4、
`WechatExternalContactFollowUserContractGuards.cs` FU1~FU4、`WechatExternalContactCustomerContractGuards.cs` CU1~CU4、
`WechatExternalContactTagContractGuards.cs` CT1~CT4、`WechatExternalContactJobInheritanceContractGuards.cs` JI1~JI4、
`WechatCorpGroupContractGuards.cs` CG1~CG11）：

| 编号 | 守卫 | 约束 |
|---|---|---|
| G1 | `MudHttpUtils_PackageReference_ShouldBeSingleVersion` | 全仓库 `Mud.HttpUtils*` 单一版本 |
| G2 | `ConfigDtos_ShouldNotUseRequired` | 配置 DTO 禁用 `required` |
| G3 | `WechatTokenTypes_ShouldUseWechatPrefixedNamespace` | `"Wechat."` 前缀隔离 |
| G4 | `WechatErrorCodes_ShouldAlignWithDetectorCollection` | 失效码 `{40014,42001,42007,42009,42011}` 与判定器同源 |
| G5 | `QueryTokenInjection_ShouldBeLimitedToWechatOfficialContractInterfaces` | Query 注入白名单未放宽（现为授权接口 + 通讯录六域 + 客户联系六域 + 上下游三接口族 + 安全管理三接口族共 61 接口；**应用类型子接口仅覆盖官方实际开放的应用类型**，官方无对应 API 的应用类型不设子接口） |
| G6 | `AuthorizationEndpoints_ShouldMatchOfficialRoutes` | 授权端点路由 + `get_customized_auth_url` 不带 `[Token]` |
| G7 | `QueryCredentialParams_ShouldBeRedactionRegisteredOrExplicitlyExempted` | Query 承载凭据的参数名 ⊆ 组件脱敏词表 **∪ 显式豁免清单**（豁免项须附追踪号；清单已清空——组件 3.0.0 含 C-01，`access_token` 亦在词表内）。新增 Query 凭据参数必须做「补齐词表 / 登记豁免」二选一决策。**豁免自过期**：豁免项一旦被组件词表覆盖即失败，不得静默遗留 |
| G8 | `AppContextHolder_ShouldBeSameInstanceAsSwitcher_InRegistrationSource`（源码顺序断言，`WechatContractGuards.cs`）；运行期同实例断言（2 例）在 `WechatServiceCollectionExtensionsTests` | DI 桥接不变量（见「企业微信领域契约」）。**原计划中的 G8-B（`IAppManager<T>` 反射对齐守卫）已撤回**——`WechatAppManager` 直连实现后不存在影子注册表可能，改由行为用例锁定 |
| G9 | `CancelAuthCleanup_ShouldBeScopedToMatchedAppKeys`（源码文本） | `cancel_auth` 不得再引入「未命中回退全部应用」的越权删除（行为用例在 `WechatCallbackAuthorizationDispatchTests`） |
| U1~U4 | `WechatUsersContractGuards`（成员管理域） | U1 全域路由表 23 条逐一断言（子接口重复声明以 `DeclaredOnly` 限定）；U2 父接口 `IsAbstract` + 三子挂 `Contact` 组并 `InheritedFrom` 父实现类 + **代开发子接口零端点**；U3 四接口统一 `Wechat.AccessToken` + Query 注入 `access_token`；U4 经 `JsonSerializerContext.GetTypeInfo` 断言 DTO 已登记 JSON 上下文。新增成员管理端点/DTO 必须同批更新；**新增 Query 注入接口同样须评估后扩展 G5** |
| D1~D4 | `WechatDepartmentsContractGuards`（部门管理域） | 与 U1~U4 同构：D1 路由表 9 条；D2 层级 + `Contact` 组 + 代开发零端点；D3 令牌绑定；D4 JSON 上下文登记（8 型）。部门域与成员域共挂 `Contact` 注册组，共用 `AddContactApi()` 注册入口 |
| T1~T4 | `WechatTagsContractGuards`（标签管理域） | 与 U1~U4 同构，但**形态特殊**：官方对三类应用开放完全一致的 7 个端点 ⇒ 全部端点收敛父接口，T1 路由表 7 条（全在父接口、互不重复）；T2 层级 + `Contact` 组 + **三个子接口全部零端点**（空标记；任何子接口新增端点 = 能力漂移，先核对官方文档再落位）；T3 令牌绑定；T4 JSON 上下文登记（10 型）。同挂 `Contact` 注册组共用 `AddContactApi()` |
| CR1~CR4 | `WechatContactRulesContractGuards`（通讯录查看权限管理域） | **形态为父接口零端点 + 端点全落自建子接口**（官方仅向自建/通讯录同步应用开放，第三方/代开发无文档 ⇒ **不设第三方/代开发子接口**）：CR1 路由表 4 条（全在 Internal，POST `/cgi-bin/contactrule/*`，其中 list 亦为 POST 无请求体）；CR2 父接口 IsAbstract + **父接口零端点** + Internal 恰好 4 端点；CR3 令牌绑定（父/自建两接口）；CR4 JSON 上下文登记（8 型）。同挂 `Contact` 注册组共用 `AddContactApi()` |
| B1~B4 | `WechatBatchContractGuards`（异步导入接口域） | 形态与标签域同构：官方对自建与第三方开放一致的 4 个端点（`/cgi-bin/batch/syncuser`、`replaceuser`、`replaceparty`、`getresult`；代开发无文档 ⇒ **不设代开发子接口**）⇒ 全部端点收敛父接口，B1 路由表 4 条（3 POST + 1 GET）；B2 层级 + `Contact` 组 + **自建/第三方子接口零端点**；B3 令牌绑定（父/自建/第三方三接口）；B4 JSON 上下文登记（6 型）。同挂 `Contact` 注册组共用 `AddContactApi()`。**危险操作警示**：全量覆盖成员会删除文件外成员（官方对删除比例有熔断），接口注释必须保留该警示 |
| E1~E4 | `WechatExportContractGuards`（异步导出接口域） | 形态与标签域同构：官方对三类应用开放一致的 5 个端点（`/cgi-bin/export/simple_user`、`user`、`department`、`taguser`、`get_result`）⇒ 全部端点收敛父接口，E1 路由表 5 条（4 POST + 1 GET，注释警示 `export/get_result` 与 `batch/getresult` 拼写差异）；E2 层级 + `Contact` 组 + **三个子接口零端点**；E3 令牌绑定；E4 JSON 上下文登记（5 型）。同挂 `Contact` 注册组共用 `AddContactApi()`。「导出任务完成通知」为回调事件（`batch_job_result`）非 HTTP 端点，不落接口契约；`encoding_aeskey` 为 POST 体敏感凭据不得记日志，解密由调用方完成 |
| FU1~FU4 | `WechatExternalContactFollowUserContractGuards`（客户联系·企业服务人员管理域） | 形态为**父接口公共端点 + 第三方/代开发各 1 条差异端点**：FU1 路由表 3 条（父接口 GET `externalcontact/get_follow_user_list`；第三方 GET `externalcontact/customer_acquisition_app/get_permit`——**仅营销获客类应用可调用**；代开发 POST `externalcontact/check_follow_user`）；FU2 父接口 IsAbstract + 三子挂 `ExternalContact` 组并 `InheritedFrom` 父实现类 + **自建子接口零端点**、第三方/代开发各恰 1 差异端点；FU3 令牌绑定（四接口 `Wechat.AccessToken` + Query 注入）；FU4 JSON 上下文登记（5 型）。同挂 `ExternalContact` 注册组共用 `AddExternalContactApi()` |
| CU1~CU4 | `WechatExternalContactCustomerContractGuards`（客户联系·客户管理域） | 形态为**父接口 10 条公共端点 + 第三方 3 条身份转换差异端点**：CU1 路由表 13 条（父接口 GET `externalcontact/list`、GET `externalcontact/get`（跟进人 >500 时 cursor 分页）、POST `externalcontact/batch/get_by_user`、POST `externalcontact/remark`、POST `customer_strategy/{list,get,get_range,create,edit,del}`；第三方 POST `idconvert/unionid_to_external_userid`、POST `idconvert/batch/external_userid_to_pending_id`、POST `externalcontact/to_service_external_userid`；CU1 注释警示 `batch/get_by_user` 与 `batch/getresult`、`export/get_result` 拼写差异）；CU2 层级 + `ExternalContact` 组 + **自建/代开发子接口零端点**、第三方恰 3；CU3 令牌绑定；CU4 JSON 上下文登记（38 型，CustomerJsonContext）。同挂 `ExternalContact` 注册组共用 `AddExternalContactApi()`。**危险操作警示**：规则组 create/edit 仅支持串行调用（勿并发），接口注释必须保留该警示；规则组端点要求「管理客户联系规则组」权限且仅能管理本应用创建的规则组 |
| CT1~CT4 | `WechatExternalContactTagContractGuards`（客户联系·客户标签管理域） | 形态与通讯录标签域同构：官方对三类应用开放完全一致的 9 个端点 ⇒ 全部端点收敛父接口，CT1 路由表 9 条（全 POST：`externalcontact/get_corp_tag_list`、`add/edit/del_corp_tag`、`mark_tag`、`get/add/edit/del_strategy_tag`；代开发文档树 96320/96322/99544 与自建/第三方同路由）；CT2 层级 + `ExternalContact` 组 + **三个子接口零端点**；CT3 令牌绑定；CT4 JSON 上下文登记（17 型，TagJsonContext）。同挂 `ExternalContact` 注册组共用 `AddExternalContactApi()`。**权限分层**：标签库读取须「客户基础信息」权限，企业客户标签管理须「管理企业客户标签」权限，规则组标签管理须「管理客户联系规则组」权限；应用仅能编辑/删除本应用创建的标签，仅能获取和管理由本应用创建的规则组标签 |
| JI1~JI4 | `WechatExternalContactJobInheritanceContractGuards`（客户联系·在职继承域） | 形态与通讯录标签域同构：官方对三类应用开放完全一致的 3 个端点 ⇒ 全部端点收敛父接口，JI1 路由表 3 条（全 POST：`externalcontact/transfer_customer`、`transfer_result`、`groupchat/onjob_transfer`——注释警示 "onjob" 拼写属官方契约，离职继承群接替为 `groupchat/transfer` 勿混淆）；JI2 层级 + `ExternalContact` 组 + **三个子接口零端点**；JI3 令牌绑定；JI4 JSON 上下文登记（9 型，JobInheritanceJsonContext）。同挂 `ExternalContact` 注册组共用 `AddExternalContactApi()`。**限频契约**：90 自然日内每位客户/每个客户群仅可被转接 2 次，客户每次最多 100 个、客户群每次 1~100 个、每人每天客户群最多分配 300 个，接口注释必须保留该警示 |
| RI1~RI4 | `WechatExternalContactResignedInheritanceContractGuards`（客户联系·离职继承域） | 形态与在职继承域同构：官方对三类应用开放完全一致的 4 个端点 ⇒ 全部端点收敛父接口，RI1 路由表 4 条（全 POST：`externalcontact/get_unassigned_list`、`resigned/transfer_customer`、`resigned/transfer_result`、`groupchat/transfer`——**`groupchat/transfer` 为离职群接替，勿与在职 `groupchat/onjob_transfer` 混淆**）；RI2 层级 + `ExternalContact` 组 + **三个子接口零端点**；RI3 令牌绑定；RI4 JSON 上下文登记（12 型，ResignedInheritanceJsonContext）。同挂 `ExternalContact` 注册组共用 `AddExternalContactApi()`。**约束契约**：原跟进成员须已离职且离职时间不超过 1 年（离职前一年内至少登录过一次企业微信）；接替成员/新群主最近一年内至少登录过一次企业微信，新群主另须配置客户联系功能 + 实名 + 已激活；客户每次最多 100 个、客户群每次 1~100 个、每人每天客户群最多分配 300 个；`transfer_customer` 返回 errcode 0 仅表示开始分配流程（待 24 小时自动接替），最终状态须查询客户接替状态 |
| GC1~GC4 | `WechatExternalContactGroupChatContractGuards`（客户联系·客户群管理域） | 形态与标签域同构：官方对三类应用开放完全一致的 3 个端点 ⇒ 全部端点收敛父接口，GC1 路由表 3 条（全 POST：`externalcontact/groupchat/list`、`groupchat/get`、`opengid_to_chatid`；注释警示 `groupchat/list`/`groupchat/get` 与在职/离职继承的群接替路由互不重叠）；GC2 层级 + `ExternalContact` 组 + **三个子接口零端点**；GC3 令牌绑定；GC4 JSON 上下文登记（12 型，GroupChatJsonContext）。同挂 `ExternalContact` 注册组共用 `AddExternalContactApi()`。**分页/范围契约**：`groupchat/list` 必须指定 `owner_filter`（不指定则拉取应用可见范围内全部群主，可见范围超 1000 人报错 81017；群主为离职成员时必须指定）；旧版 `offset + limit` 分页将废弃，须用 `cursor + limit` |
| CG1~CG4 | `WechatCorpGroupContractGuards`（上下游域） | 开放面为**自建 + 代开发**（6 端点）+ **第三方（仅获取应用共享信息 95324，同路由同契约，随父接口继承）**：6 个端点全落父接口（`/cgi-bin/corpgroup/corp/list_app_share_info`、`corp/gettoken`、`miniprogram/transfer_session`、`unionid_to_external_userid`、`unionid_to_pending_id`、`batch/external_userid_to_pending_id`；97357/98040 一篇覆盖 2 端点）。CG1 路由表 6 条（全 POST）；CG2 层级 + `CorpGroup` 注册组（`WechatModule.CorpGroup`，经 `AddCorpGroupApi()` 独立注册）+ **三个子接口零端点** + **反射断言继承链上恰好只有自建/第三方/代开发三个子接口**；CG3 令牌绑定（四接口）；CG4 JSON 上下文登记（15 型）。**令牌语义警示**：transfer_session 必须用下级/下游企业凭证（经 corpgroup/corp/gettoken 获取，SDK 不自动缓存，由宿主写入令牌存储后切换上下文调用）；`session_key`/下游 access_token 为敏感凭据不得记日志 |
| CG5~CG8 | `WechatCorpGroupContractGuards`（上下游通讯录管理域，与 CG1~CG4 同文件） | 公共读取面父接口（4 端点：`corpgroup/corp/get_chain_list`、`get_chain_group`、`get_chain_corpinfo_list`、`get_chain_corpinfo`）+ 自建子接口（5 端点：`import_chain_contact`、`corpgroup/getresult`、`corp/remove_corp`、`corp/get_chain_user_custom_id`、`get_corp_shared_chain_list`）+ 代开发空标记（官方代开发树仅镜像获取上下游信息）。CG5 路由表 9 条（`DeclaredOnly` 限定声明位置，注释警示 `corpgroup/getresult` 与 `batch/getresult`、`export/get_result` 拼写差异）；CG6 父接口恰 4 端点 + 自建恰 5 端点 + 代开发零端点；CG7 令牌绑定；CG8 JSON 上下文登记（23 型）。同挂 `CorpGroup` 注册组共用 `AddCorpGroupApi()`。**导入强串行**：同时仅一个导入任务、只允许串行调用，接口注释必须保留该警示 |
| CG9~CG11 | `WechatCorpGroupContractGuards`（上下游规则域，与 CG1~CG8 同文件） | **形态为父接口零端点 + 端点全落自建子接口**（官方仅向自建开放，且仅上下游创建空间的主企业可调用）：5 个端点（`corpgroup/rule/list_ids`、`delete_rule`、`get_rule_info`、`add_rule`、`modify_rule`）。CG9 路由表 5 条（全 POST）；CG10 父接口零端点 + Internal 恰 5 端点、无其它子接口；CG11 令牌绑定（2 接口）+ JSON 上下文登记（11 型）。同挂 `CorpGroup` 注册组共用 `AddCorpGroupApi()`。**频率警示**：新增/更新规则共用每天 1000 次额度，接口注释必须保留 |
| SEC1~SEC4 | `WechatSecurityContractGuards`（安全管理域，`WechatModule.Security` / `Security` 注册组，经 `AddSecurityApi()` 独立注册） | **形态为三接口族「父接口零端点 + 端点全落自建子接口」**（官方安全管理目录 16 端点对第三方/代开发均无文档 ⇒ 不设第三方/代开发子接口；官方文档 URL：文件防泄漏 98079、设备管理 98920、截屏/录屏管理 100128、域名 IP 100079、高级功能账号 99503/99505/99506、操作日志 100178/100179）。SEC1 路由表 16 条（Security 族 9：文件防泄漏 1 + trustdevice 设备 6 + 截屏录屏 1 + 域名 IP GET 1；Vip 族 5 全 POST；OperLog 族 2 全 POST）；SEC2 层级（三父接口 IsAbstract 零端点 + 自建子接口恰 9/5/2 端点 + 继承链上仅 Internal 一个子接口的漂移守卫）；SEC3 令牌绑定（6 接口 `Wechat.AccessToken` + Query 注入）；SEC4 JSON 上下文登记（46 型，SecurityJsonContext）。**官方拼写陷阱**：域名 IP 响应字段 `universal_domian`（原文如此）、admin_oper_log 参数表游标拼作 `cusor`（SDK 以官方 JSON 示例为准用 `cursor`）；**权限分层**：文件防泄漏/设备管理/截屏录屏/域名 IP/高级功能账号/操作日志各自独立配置「可调用接口的应用」，可见范围外用户数据被过滤；**限频**：操作日志 600 次/分钟、跨度 ≤7 天；高级功能分配/取消为异步任务（jobid 查询） |
| MSG1~MSG4 | `WechatMessageContractGuards`（消息推送域，`WechatModule.Message` / `Message` 注册组，经 `AddMessageApi()` 独立注册） | 四接口族：发送应用消息族为**父接口公共端点 + 第三方 template_msg 差异端点**（自建/代开发空标记；官方文档 URL：发送应用消息 90236/90372/96458、更新模版卡片 94888/94945/96459、撤回 94867/94947/96460、模板消息 94515 仅第三方），群聊会话族（90245/98913/98914/90248）、家校学校通知族（91609）与智能表格自动化创建的群聊族（100989/101028/101029）为**父接口零端点 + 端点全落自建子接口**（官方仅自建开放，群聊会话明示第三方不可调用，应用可见范围须根部门）。MSG1 路由表 37 条（message/send 12 方法同路由：11 公共 msgtype + 第三方 template_msg；update_template_card / recall 各 1；appchat create/update/get/send 共 12 方法，其中 get 为 GET + `[Query("chatid")]`；学校通知 externalcontact/message/send 8 方法同路由；智能表格群聊 wedoc/smartsheet/groupchat/{list,get,update} 3 条）；MSG2 层级（Message 父接口 IsAbstract 恰 13 端点 + 自建/代开发零端点 + 第三方恰 1 端点 + 四族「继承链上恰好只有既定子接口」漂移守卫；AppChat/SchoolMessage/SmartSheetGroupChat 父接口零端点 + 仅 Internal 子接口承载端点）；MSG3 令牌绑定（10 接口 `Wechat.AccessToken` + Query 注入）；MSG4 JSON 上下文登记（86 型，MessageJsonContext）。**多态落位决策**：每 msgtype 一端点方法 + 请求 DTO（同路由多方法；不做运行时多态——AOT 源生成按声明类型序列化），msgtype 参数表差异（safe 有无、id 转译支持面、mentioned_list 仅群聊）在 DTO 层面表达；`TemplateCardBody` 为发送/更新两端点共用扁平结构（card_type 判别，replace_text/disable 仅更新接口支持）。**响应形态陷阱**：message/send 的 invaliduser 为竖线分隔字符串，update_template_card 的 invaliduser 为字符串数组，两响应 DTO 不共用。**限频**：应用消息每应用「账号上限数 × 200」人次/天、同一成员 30 次/分 + 1000 次/时（超限丢弃）；appchat/send 每企业 2 万人次/分 + 规模分档小时额度、成员级 200 条/分 + 1 万条/天（超限静默丢弃不报错）；创建群 1000 个/天、修改群 1000 次/小时；智能表格群聊 update 同一群聊修改串行、并发限制 10。**「接收消息与事件」为回调推送（XML），由 Callback 包承载，不落本域接口** |
| CB1~CB13 | `WechatCallbackContractGuards`（回调域，对齐《回调解决方案 v1》§7 + §13 上下游增补） | CB1 `Callback` 包 csproj 不得引用主包 `Work`（K-callback）；CB2 `WechatCallbackEventTypes` 常量覆盖官方事件键 24 个（授权 6 + 通讯录 7 + 异步 1 + 上下游 Event 1 与 ChangeType 9，95796）+ `EventTypeKey` 优先级（InfoType→ChangeType→Event）；CB3 内置授权族处理器为**单类兜底**（`SupportedEventType => string.Empty`，文件留 `Callback` 包根目录——G9 按路径断言）+ 信封含授权族 6 判别；CB4 事件 DTO 官方字段反射断言（成员/部门/标签/异步/上下游 11 型）；CB5 回调凭据唯一来源 = `WechatCallbackOptions.Apps`（`WechatAppConfig` 无 Push 属性、应用级配置无 `AppKey` 属性）；CB6 echo 复用 `VerifySignature`+`Decrypt`、验签先于解密且**不消费指纹**（协议文本）+ 被动应答 `Encrypt`/`ComputeSignature` 基座；CB7 `Abstractions/Callback/` 不得出现 XML 类型（信封上移边界）；CB8 `WechatCallbackCrypto` **不得出现 `PaddingMode.PKCS7`**（.NET 内置 16 块校验会误拒官方 pad∈[17..32] 报文），必须 `PaddingMode.None` + 手工 32 块填充剥离（P0-1）；CB9 接收器源码中 `TryMarkAsync` 必须位于 `WechatCallbackCrypto.Decrypt` **之后**（P1-1/D2 指纹闸后移防回归）；CB10 `WechatCallbackChannel` 枚举（App=1/Suite=2）存在 + `WechatAppCallbackOptions` 暴露 `AppType`/`Channel`/`ReceiveId` 配置面；CB11 `ValidateReceiveId` 按「应用类型 × 回调通道」三元分流（自建 App / 第三方·代开发 Suite = 静态 ReceiveId；第三方·代开发 App = 动态 CorpId 比对 ToUserName）；CB12 `IsEventFamilyAllowed` 开放面矩阵（授权族→Suite+第三方/代开发；上下游→App+自建；通讯录/异步→App；Unknown→不拦截）；CB13 分发器合法性闸先于拦截器 `BeforeHandleAsync` + 不适用族返回 `Rejected`（→200 不重推）。守卫文件位于 `Tests/Mud.Wechat.Work.Tests/ContractGuards/`（该工程对 `Callback` 有测试专用 ProjectReference） |
| MA1~MA4 | `WechatMultiAppContractGuards`（多应用管理域，`Tests/Mud.Wechat.Work.Abstractions.Tests/ContractGuards/`） | MA1 `RemoveApp` 方法体内 `_lazyContexts.TryRemove` 必须先于 `_configs.Remove`（M8「返回 false ⇒ 零突变」防回归）；MA2 `IsTransientInitFailure` 方法体**不得包含 IOE 白名单判定**且须保留 IO 型异常组 + OCE 显式排除（M4）；MA3 `WechatAppContextRetirement.Enqueue` 方法体必须含 `_disposed` 闸且落闸即 `Dispose` 上下文、先于宽限期判定（M3 停机竞态闸）；MA4 `WechatCorpContext.SetCorp` 必须含 null（`ArgumentNullException`）与空白（`IsNullOrWhiteSpace`）校验（M7）。守卫为**方法体提取**（花括号配平）的源码文本断言，签名漂移须同步更新守卫 |
| RD-G1~RD-G6 | `WechatRedisContractGuards`（Redis 分布式存储域，`Tests/Mud.Wechat.Redis.Tests/ContractGuards/`） | RD-G1 SCAN 模式仅经 `WechatRedisKeyBuilder.Pattern` 产出（含 glob 字面量转义 + `:*` 段级精确结尾，KeysAsync 调用点文件必须引用 Pattern 单一出口——飞书 D10 静默失效防回归）；RD-G2 `WechatRedisOptions`/`WechatRedisConnectionOptions` 无 `required`（G2 同源）；RD-G3 重放守卫 `TryMarkAsync` 方法体必须含 `WechatRedisErrors.Map` 上抛且无「catch 吞异常返回 true/false」形态（RD3 fail-closed）；RD-G4 `Password`/`PermanentCode`/票据值不进日志调用点（连接失败消息只携带脱敏后的 `options.ToString()`）；RD-G5 顺序守卫常量 `InMemoryReplayGuardTypeName` 与 Callback 包 `InMemoryWechatCallbackReplayGuard` 的 FullName 反射一致（R-1 后 Redis 不引用 Callback，全名探测防漂移）；RD-G6 Redis 包 csproj 单依赖 Abstractions（不得引用 Callback/主包，RD11） |

## Test Guidelines

- xUnit + FluentAssertions + Moq；测试镜像源结构；类名 `{ClassName}Tests`，方法名 `{Method}_Should{Behavior}_When{Condition}`。
- 已知陷阱（本项目已踩过，勿重复）：
  - **FluentAssertions `Equal` 重载误绑定**：`Equal("a","b","because 文本")` 会绑到 `params object[]` 重载。
    集合断言须写 `Equal(new[]{ "a","b" }, "because 文本")`。
  - **Moq 的 `out` 参数**：不能直接用局部变量，须 `It.Ref<T>.IsAny` + 自定义 delegate 回调。
  - **DI 解析守卫**：新增 Singleton 服务时补「DI 可解析 + 跨 scope 同一实例」用例（`ValidateScopes = true` 变体）。
  - **可选依赖**：走 `IServiceProvider.GetService<T>()`（返回 null 降级），**不**依赖带默认值的可选构造参数。
  - `Moq` 的 `Setup` 与真实 DI 装配顺序：先 `AddWechatWorkServices(...)` 完成真实装配，再用 `AddSingleton(mock.Object)` 覆盖外部边界。

## Configuration Surface

- 配置面唯一公共 API：`WechatAppConfig`（数组节 `WechatApps`）+ `Validate()`；编排策略 `WechatAuthorizationOptions`（节 `WechatAuthorization`）。
- **禁止新增「日志开关」类配置属性**（历史死配置反模式）；日志级别统一由 `Logging:LogLevel:{Category}` 控制。
- 每个公开配置属性必须有真实消费点（`Validate`/`ToString` 不算）。**`scripts/audit-config-keys.ps1` 的口径是「消费点扫描」，并无 `$strictPatterns` 白名单** —— 删除配置键后若脚本报「无消费点」，正确处置是补消费点或删除该属性，而不是加模式。**现状：全绿**（`WechatAppConfig` 12 + `WechatCallbackOptions.cs` 7（含应用级 `WechatAppCallbackOptions` 的 PushToken/PushEncodingAESKey/ReceiveId/AppType/Channel——**应用级配置类必须与主配置同类文件**才会纳入扫描）+ `WechatRedisOptions`/`WechatRedisConnectionOptions` 8，权威计数以脚本输出为准）。
  - `WechatAppCallbackOptions.ReceiveId` 的消费点是 `receiveid` 校验（不是日志开关）；接收方 ID 语义：企业自建填 `CorpId`，**套件回调填 `SuiteId`**，通讯录同步助手（通配键）可留空（留空或明文未携带 receiveid 时跳过校验并一次性告警）。`AppType`/`Channel` 的消费点是 `Validate()`（非法组合校验）+ `ValidateReceiveId`（三元分流）+ `IsEventFamilyAllowed`（开放面矩阵）。
  - `WechatAppConfig.TemplateId` 曾因「只被 `Validate()` 使用」被判红 → 已删除（见 K2），不是加白名单绕开；回调域同款教训：`WechatAppCallbackOptions` 不得有 `AppKey` 属性（字典键唯一权威，CB5 锁定）。
- 安全默认不得削弱：`BaseUrl` 必须 HTTPS + 白名单（`AllowCustomBaseUrl=false` 为默认 SSRF 防线）。
  `AllowCustomBaseUrl=true` 的应用主机在注册期登记到 `WechatCustomBaseUrlRegistry`，供 errcode 判定器的同步预过滤放行（否则私有化部署静默失去令牌恢复能力）。

## Security

- 绝不记录或暴露 `AgentSecret` / `SuiteSecret` / `ProviderSecret` / `permanent_code` / `auth_code` / `suite_ticket`；日志脱敏。
  回调域同款警示：`WechatCallbackEvent` 的 `DecryptedXml` / `SuiteTicket` / `AuthCode` 为敏感凭据，
  不得写入日志、遥测或异常消息（P3-3 契约文档化）。
- **Query 承载凭据的脱敏登记**：`corpsecret` / `suite_access_token` / `provider_access_token` 由企业微信契约强制放在 **Query**，
  而组件 `SensitiveUrlRedactor` 是**精确匹配**词表——在组件 3.0.0（C-01）之前不覆盖这三者 ⇒ 随 `ApiException.RequestUri` /
  遥测 URL 明文外泄，故 G7 要求逐参数「补齐词表 / 登记豁免（附追踪号）」。**新增任何 Query 凭据参数仍必须做该二选一决策**。
  SDK 侧不得抢占组件 `IExceptionRedactor`（会丢掉 `Content`/`RequestContent` 的词表擦除 = 削弱安全默认）。
- `WechatWorkException.RequestUri` 在构造期剥离 query 与 userinfo；不得把原始 URI 直接传出。
- 授权链接参数校验：`get_customized_auth_url` 的 `state` ≤ 32 字节且仅 `[a-zA-Z0-9]`（`ValidateCustomizedState`）；
  安装链接 `state` ≤ 128 字节（`WechatAuthorizationUrlRequest.State`）。两条规则**不同**，勿互相套用。
- 回调入口必须保留抗重放两道闸（时效窗口 + 一次性标记），不得为兼容而降级为 fail-open。
- 不要绕过契约守卫与门禁（禁 `--no-verify`、禁删除断言）。

## Docs

方案与设计文档在 `.docs/`（中文）：`MudWechatWork-授权功能方案-v1.md`（规格 + 评审记录 R1~R15）、
`MudWechatWork-详细设计文档-v1.md`、`MudWechatWork-产品规划方案-v1.md`；
`MudWechatWork-审查缺陷修复与完善方案-v1.md`（P0/P1/P2 修复方案 + **评审记录 R1~R26** + 附录 D 实施记录）。
**`.docs/` 被 `.gitignore` 忽略（本地文档，fresh clone 无此目录）**。
**代码变更若触及契约面，须同批同步对应章节。**