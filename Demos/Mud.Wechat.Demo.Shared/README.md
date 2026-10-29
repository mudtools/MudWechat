# Mud.Wechat.Demo.Shared（L0 基座）

`Demos/` 里所有工程共用的**四个部件**：环境变量校验、写操作闸、`/healthz` 装配、业务异常映射闸。
类库（`OutputType=Library`、`IsPackable=false`），不进解决方案的产品线扫描面（`/src/` 之外），
也**不含任何 SDK 业务逻辑** —— 它只规定「一个 Demo 应该长成什么样」。

索引与逐 Demo 凭据矩阵在 `Demos/README.md`；本文件只讲基座自身的契约。

## 1 四个部件

| 类型 | 成员 | 承担什么 |
|---|---|---|
| `DemoEnv` | `Require` / `RequireAll` / `Optional` / `Flag` / `FullName`；`RequireAll(IConfiguration, (env, key)[])` / `Optional(env, IConfiguration, key)` | 统一 `MUDWECHAT_DEMO_` 前缀读取；`RequireAll` **一次性列出全部缺失项**再抛；含 `<` `>` `your-` 的值按占位符等同于缺失。后两个重载把「变量名 + 配置键」当**成对来源**（变量优先，两处共用同一套占位符判定），错误消息同时给出两种写法 |
| `DemoWriteGate` | `Enabled` / `Announce(logger)` | 只认 `MUDWECHAT_DEMO_ALLOW_WRITE=1` 这一个开关；启用时启动日志打 **Warning** |
| `DemoHealthEndpoint` | `MapDemoHealth(demo, config)` / `MapDemoHealth(demo, ctx => Task<object>)` | 固定形状 `{status, demo, utcNow, writeGate, config}`；`config` 由各 Demo 自己给，**只放非敏感摘要**。第二个重载供带本地存储的 Demo **按请求现算**（台账行数在启动期快照里没有意义）；两者都走 `WriteAsJsonAsync` —— `async (HttpContext) => Results.Json(…)` 会被重载解析绑成 `RequestDelegate`，返回值丢弃后是**不报错的空 200**（DM-14 实测踩过） |
| `DemoApiErrorGate` | `UseWechatApiErrorGate(app)` | 把 `WechatApiException` 族映射成 502 + `{errorCode, errorMessage, requestUri}`；另拆**组件负缓存包装**（内层是 `WechatApiException` 才映射，普通 `InvalidOperationException` 原样上抛） |
| `DemoConfigSources` | `BuildSettingsChain(environmentName, args)` | 为需要文件化配置的 Demo **独立**构造设置链：`appsettings.json`（模板）< `appsettings.{Environment}.json` < `appsettings.local.json`（本机真值，被 `.gitignore` 忽略）< 命令行；环境变量再由 `DemoEnv` 压在最上层。刻意不碰宿主配置链 |

## 2 使用纪律（新增 Demo 时照 §4 清单，别改本库）

1. **`DemoEnv.RequireAll` 必须在任何 DI 装配之前**：`Add*App` 在注册期就构造应用管理器并 `Validate()`，
   凭据缺失若走到那时才炸，报错形态是「配置非法」而不是「少配了哪个变量」，读者无法自助。
   唯一例外是 `Mud.Wechat.AllInOne.Demo`（它按凭据束整线降级，见该工程 README）。
   需要读 JSON 配置的 Demo（DM-14）把校验放在 `CreateBuilder` **之后**、任何 `builder.Services.*` 之前 ——
   「DI 之前」这条纪律仍然成立，`CreateBuilder` 本身不注册服务。
2. **写端点包在 `if (DemoWriteGate.Enabled)` 里，不是「映射了再判 403」**：默认**根本不映射**路由 ⇒ 404。
   「暴露了再判」会让读者以为写操作只是被权限挡住，而实际防线是路由不存在。
3. **错误闸的注册顺序**：通用闸 `UseWechatApiErrorGate()` 先注册、线专属闸后注册
   （`app.Use` 越晚注册越靠内、越先捕获）。写反**不报错**，只是把专属语义（支付的字符串 `payErrorCode`、
   开放平台的凭证链措辞、广告线的 401 需重新授权）退化成一段通用 502。
   七线里需要专属闸的只有支付与开放平台（异常族不派生自 `WechatApiException`）；
   广告线专属闸是**分流**（401 vs 502）而非补漏。
4. **`/healthz` 不回显任何凭据**：可以回「是否已配置」布尔、长度、sha256 前 4 字节指纹，
   绝不回原文 —— 指纹足以证明「换到了新值」，又不把凭据摊在屏幕上。
5. **闸的实现刻意只有一句 `DemoEnv.Flag("ALLOW_WRITE")`**（只认 `"1"`）：保持可被 grep 到，
   任何 Demo 里出现绕过它的写端点都属体系缺陷。不要给它加「默认开启」「按环境推导」之类的智能。
   —— 写闸**没有配置文件等价键**：DM-14 支持 JSON 配置时特意不开这条口，
   否则「默认关闭」会变成「换台机器（带一份 appsettings）就默认打开」。
6. **本机真值走独立设置链，不要动宿主的 `builder.Configuration.Sources`**：`WebApplicationBuilder` 在 `Build()` 时
   把宿主链**接在自己 sources 尾部**，事后插入的源会被 `appsettings.json` 反向压过 ⇒「本机文件存在却读不到、
   读到的是模板占位符」且静默（DM-14 实测：`appIdResolved` 的长度等于占位符串长）。正确做法是
   `DemoConfigSources.BuildSettingsChain(...)` 独立建链（模板 < 环境文件 < `appsettings.local.json` < 命令行），
   再由 `DemoEnv` 把环境变量压在最上层；入库的 `appsettings.json` 只放占位符（含 `<` `>` `your-` 等同未配置）。

   `appsettings.json` 只放模板与非机密项。

## 3 为什么不把这几件事并进 SDK

`DemoEnv` 的占位符判定、写闸的默认关闭、`/healthz` 的形状都属**示例工程的教学策略**，不属运行时契约：
SDK 侧的配置校验由 `WechatAppConfigBase.Validate()` 承担（真实值非法即抛），
不存在「SDK 里藏一个只服务于 Demo 的开关」的空间。把 Demo 纪律做进 `Src/` 会让示例的默认值变成产品的默认值。

## 4 新增 Demo 的接入动作

1. csproj 引 `Mud.Wechat.Demo.Shared` + 所需产品线包（回调 Demo 另内嵌 `Mud.Wechat.Callback.Analyzers`，
   例外见 `Demos/README.md` §6.2）；
2. `DemoEnv.RequireAll(...)` → `Add*App` → `Add*Api` → `builder.Build()` → `app.UseWechatApiErrorGate()`
   （末步的判据是「本宿主会不会主动出网调接口」；只收不发的回调宿主按 `Demos/README.md` §6.4 有意不挂）；
3. `app.MapDemoHealth("<线名>", new { 非敏感摘要 })` + `DemoWriteGate.Announce(app.Logger)`；
4. 写端点一律 `if (DemoWriteGate.Enabled)`；
5. 工程进 `Mud.Wechat.slnx` 的 `/Demos/` 组（否则全量 Release 编译覆盖不到，等于不设防）；
6. 写本工程 `README.md`，并把凭据登记进 `Demos/README.md` §2。

逐条判据与实跑命令以 `Demos/README.md` §4/§6 为准，本文件不复述。

## 5 安全须知

本库自身不读写任何凭据文件、不落日志（`DemoWriteGate.Announce` 只打「写操作已启用/关闭」这一句状态）。
`UseWechatApiErrorGate` 回显的 `requestUri` 来自异常基类**构造期已脱敏**的属性（query 与 userinfo 已剥离）⇒
若某线把凭据放在**路径**里（七线目前无此形态），该形态不属本闸的脱敏假设，须先改 SDK 的脱敏落点再说。
