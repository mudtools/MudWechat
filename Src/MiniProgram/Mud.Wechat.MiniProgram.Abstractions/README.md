# Mud.Wechat.MiniProgram.Abstractions

微信小程序 SDK **抽象包**：官方错误码常量与线级异常类型。体量刻意极小——小程序与公众号同属微信公众平台，**令牌、多应用基座与配置面全部复用 `Mud.Wechat.OfficialAccount.Abstractions`**，本包不重复实现任何一处。

## 内容

- `WxaErrorCodes`——官方错误码常量 **13 个**（含 `Success = 0`）。字段名与数值照官方原文，判定器与本集合同源，避免「散落的魔法数字」。
- `WxaException : WechatApiException`——小程序线统一异常（`ErrorCode` = 官方 `errcode`；构造期经基座剥离 Query 与 userinfo，不泄露令牌）。含静态入口 `ThrowIfFailed<TResponse>(TResponse?, string? requestUri)`，约束 `where TResponse : class, IWechatApiResponse` ⇒ 与公众号 / 企业微信线共用同一叶层判错契约。

**本包不提供配置类型**：小程序应用凭据即公众号凭据（`MpAppConfig`：`AppId` / `AppSecret` / `UseStableToken`，默认节 `MpApps`、`UseStableToken` 默认 `true`）。新增「小程序专属配置面」会造成同一 `AppId` 两处注册、两套令牌缓存，属被否决形态。

## 为什么令牌不落在本包（关键裁决）

公众号侧 `MpTokenManagerRegistry` 是 `internal sealed`、按 `MpTokenTypes.AccessToken` **单槽键控**。若小程序线新增一个令牌类型（如 `Wechat.Wxa.AccessToken`），注册表读不到该槽 ⇒ errcode 令牌恢复返回 `null`、**静默空转且不抛异常**——这是最危险的一类漂移（编译通过、运行期只表现为「令牌过期后不自愈」）。因此 MP-X2 把「复用令牌类型」锁成守卫，MP-X3 进一步锁死 `[Token].Name` 恒为官方 `access_token`。

同理，本线接口为**平铺命名空间、无 `IsAbstract` 父接口**（MP-X4）：三模式（自建 / 第三方 / 代开发）差异在小程序侧不存在，父/子分面只会凭空造出一层无用抽象。

## 依赖

`Mud.Wechat.Abstractions`（叶层）+ `Mud.Wechat.MiniProgram.DataModels` + `Mud.Wechat.OfficialAccount.Abstractions`（跨线唯一允许的边）+ `Mud.HttpUtils` 3.0.3（`.Generator` 3.0.3 仅分析器，`PrivateAssets=all`）。

**禁止**引用公众号主包 / DataModels / Callback（MP-X1 / MP-X5 守卫锁定）。`Mud.HttpUtils` 与 `.Generator` 版本须全仓单一（通用守卫 G1，`NU1605` 视为错误）。

目标框架继承根 `Directory.Build.props`（`netstandard2.0;net6.0;net8.0;net10.0`、Version `1.0.3`），可打包。

## 已踩陷阱

- `netstandard2.0` 下无 `IsExternalInit` polyfill ⇒ 本线公共类型不用 `init` / `record` / `with`。
- 免令牌端点（`sns/jscode2session`）**不得**声明 `[Token]`：生成器仍要求显式 `TokenManage = nameof(IMpAppManager)`（`HTTPCLIENT018`），但 `[Token]` 一旦声明就会把应用级令牌注入该请求，语义即错。
