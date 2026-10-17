# Mud.Wechat.Demo.Shared（L0 基座）

`Demos/` 里所有工程共用的**四个部件**：环境变量校验、写操作闸、`/healthz` 装配、业务异常映射闸。
类库（`OutputType=Library`、`IsPackable=false`），不进解决方案的产品线扫描面（`/src/` 之外），
也**不含任何 SDK 业务逻辑** —— 它只规定「一个 Demo 应该长成什么样」。

索引与逐 Demo 凭据矩阵在 `Demos/README.md`；本文件只讲基座自身的契约。

## 1 四个部件

| 类型 | 成员 | 承担什么 |
|---|---|---|
| `DemoEnv` | `Require` / `RequireAll` / `Optional` / `Flag` / `FullName` | 统一 `MUDWECHAT_DEMO_` 前缀读取；`RequireAll` **一次性列出全部缺失项**再抛；含 `<` `>` `your-` 的值按占位符等同于缺失 |
| `DemoWriteGate` | `Enabled` / `Announce(logger)` | 只认 `MUDWECHAT_DEMO_ALLOW_WRITE=1` 这一个开关；启用时启动日志打 **Warning** |
| `DemoHealthEndpoint` | `MapDemoHealth(demo, config)` | 固定形状 `{status, demo, utcNow, writeGate, config}`；`config` 由各 Demo 自己给，**只放非敏感摘要** |
| `DemoApiErrorGate` | `UseWechatApiErrorGate(app)` | 把 `WechatApiException` 族映射成 502 + `{errorCode, errorMessage, requestUri}`；另拆**组件负缓存包装**（内层是 `WechatApiException` 才映射，普通 `InvalidOperationException` 原样上抛） |

## 2 使用纪律（新增 Demo 时照 §4 清单，别改本库）

1. **`DemoEnv.RequireAll` 必须在任何 DI 装配之前**：`Add*App` 在注册期就构造应用管理器并 `Validate()`，
   凭据缺失若走到那时才炸，报错形态是「配置非法」而不是「少配了哪个变量」，读者无法自助。
   唯一例外是 `Mud.Wechat.AllInOne.Demo`（它按凭据束整线降级，见该工程 README）。
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

## 3 为什么不把这几件事并进 SDK

`DemoEnv` 的占位符判定、写闸的默认关闭、`/healthz` 的形状都属**示例工程的教学策略**，不属运行时契约：
SDK 侧的配置校验由 `WechatAppConfigBase.Validate()` 承担（真实值非法即抛），
不存在「SDK 里藏一个只服务于 Demo 的开关」的空间。把 Demo 纪律做进 `Src/` 会让示例的默认值变成产品的默认值。

## 4 新增 Demo 的接入动作

1. csproj 引 `Mud.Wechat.Demo.Shared` + 所需产品线包（回调 Demo 另内嵌 `Mud.Wechat.Callback.Analyzers`，
   例外见 `Demos/README.md` §6.2）；
2. `DemoEnv.RequireAll(...)` → `Add*App` → `Add*Api` → `builder.Build()` → `app.UseWechatApiErrorGate()`；
3. `app.MapDemoHealth("<线名>", new { 非敏感摘要 })` + `DemoWriteGate.Announce(app.Logger)`；
4. 写端点一律 `if (DemoWriteGate.Enabled)`；
5. 工程进 `Mud.Wechat.slnx` 的 `/Demos/` 组（否则全量 Release 编译覆盖不到，等于不设防）；
6. 写本工程 `README.md`，并把凭据登记进 `Demos/README.md` §2。

逐条判据与实跑命令以 `Demos/README.md` §4/§6 为准，本文件不复述。

## 5 安全须知

本库自身不读写任何凭据文件、不落日志（`DemoWriteGate.Announce` 只打「写操作已启用/关闭」这一句状态）。
`UseWechatApiErrorGate` 回显的 `requestUri` 来自异常基类**构造期已脱敏**的属性（query 与 userinfo 已剥离）⇒
若某线把凭据放在**路径**里（七线目前无此形态），该形态不属本闸的脱敏假设，须先改 SDK 的脱敏落点再说。
