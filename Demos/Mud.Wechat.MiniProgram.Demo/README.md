# Mud.Wechat.MiniProgram.Demo（DM-05 微信小程序）

一条工程覆盖小程序服务端的三件高频事：**登录换会话**、**生成小程序码**、**发送订阅消息**。
对应 Senparc Samples 里 `WxOpenController`（`Code2Session` / `CreateQRCode`）与订阅消息示例的合并面。

依赖的 SDK 公开面（Demo 不触碰任何 internal 类型）：

| 能力 | 公开面 |
|---|---|
| 令牌底座 | `AddMpApp(Action<MpAppConfig>)`（与公众号同令牌域，本线不自建令牌类型） |
| 模块装配 | `AddMiniProgramServices(b => b.AddAuthApi().AddQrCodeLinkApi().AddSubscribeMessageApi())` |
| 登录 | `IWxaAuthService.Code2SessionAsync`、`WxaGrantTypes.AuthorizationCode`、`WxaCode2SessionResponse` |
| 小程序码 | `IWxaCodeService.GetUnlimitedCodeAsync`、`WxaCodeUnlimitRequest`、`WxaCodeResult`（手工二进制通道） |
| 订阅消息 | `IWxaSubscribeMessageService.SendSubscribeMessageAsync`、`WxaSubscribeMessageSendRequest`、`WxaSubscribeMessageDataValue` |

## 1 路由

| 方法 | 路径 | 说明 |
|---|---|---|
| GET | `/healthz` | 非机密摘要（应用键、令牌底座、已装模块、模板是否已配、写闸状态） |
| POST | `/api/session` | `js_code` 换 `openid` / `unionid`；`session_key` **只回显是否存在** |
| GET | `/api/qrcode?scene=&page=` | 不限量小程序码，成功回图片流（`Content-Type` 取官方回传值） |
| POST | `/api/subscribe/send` | 订阅消息（写操作，默认**不映射** ⇒ 404；`MUDWECHAT_DEMO_ALLOW_WRITE=1` 后启用） |

本线**没有回调工程**：小程序的消息与事件接收由公众号 XML 通道（`Mud.Wechat.OfficialAccount.Callback`，见 DM-04）承载，
故本工程不设 `/healthz` 之外的推送入口。

## 2 前置条件

1. **登录**：小程序端 `wx.login` 现场产生的 `code` 才能换会话，一次性的、5 分钟过期 —— 刷新页面复用同一 `code` 必然报 `40029`。
2. **小程序码**：`page` 须是**已发布**（或开发版存在）的页面路径；本 Demo 把 `check_path` 置 `false` 以便未发布页面也能出码，**正式发布前应回默认 `true`**。
3. **订阅消息**：需两段准备 —— 小程序后台「订阅消息」选用模板并把模板 ID 放进 `MUDWECHAT_DEMO_WXA_TEMPLATE_ID`；端上先用 `wx.requestSubscribeMessage` 取得用户授权。**一次授权只能发一条**，未授权直接报 `43101`。

## 3 环境变量

| 变量 | 必填 | 用途 |
|---|---|---|
| `MUDWECHAT_DEMO_WXA_APP_ID` | 是 | 小程序 AppId；同时是 `code2Session` 的显式入参 |
| `MUDWECHAT_DEMO_WXA_APP_SECRET` | 是 | 应用令牌（小程序码 / 订阅消息用）与换会话凭据 |
| `MUDWECHAT_DEMO_WXA_TEMPLATE_ID` | 否 | 订阅消息模板 ID；未设置时发送端点直接返回配置提示 |
| `MUDWECHAT_DEMO_ALLOW_WRITE` | 否 | `=1` 时才映射写端点 |

缺失时启动即失败并一次性列出全部缺失项；值里含 `<` / `>` / `your-` 视为占位符，等同于未配置。

## 4 运行

```bash
# 1) 构建
dotnet build Demos/Mud.Wechat.MiniProgram.Demo -c Release

# 2) 无凭据：应立刻失败并列出缺失变量（预期行为，用于验证 fail-fast）
dotnet run --project Demos/Mud.Wechat.MiniProgram.Demo -c Release --no-build

# 3) 带凭据启动（默认 https://localhost:56555 / http://localhost:56556）
export MUDWECHAT_DEMO_WXA_APP_ID=wx……
export MUDWECHAT_DEMO_WXA_APP_SECRET=……
dotnet run --project Demos/Mud.Wechat.MiniProgram.Demo -c Release --no-build
```

```bash
# 登录（假 code 也应有明确响应，见 §5）
curl -X POST http://localhost:56556/api/session -H "Content-Type: application/json" \
     -d '{"jsCode":"<wx.login 现场产生的 code>"}'

# 小程序码（存成文件再看）
curl -o /tmp/code.jpg "http://localhost:56556/api/qrcode?scene=abc&page=pages/index/index"
file /tmp/code.jpg     # 成功=image data；失败=JSON（Demo 已把官方错误码原样回传）
```

## 5 离线可验证部分（无需真实小程序）

用假凭据即可跑通的四条，全部是「装配 + 契约」层面的断言：

| 断言 | 期望 | 锁定的契约 |
|---|---|---|
| 缺环境变量启动 | 进程立刻失败，一次性列出 `WXA_APP_ID` / `WXA_APP_SECRET` | Demo 基座 fail-fast，不带默认值静默起服务 |
| `GET /healthz` | 200，且响应里**没有任何** secret / token | 摘要面只含配置键与非机密状态 |
| `GET /api/qrcode` 不带 `scene` / `scene` 超 32 字符 | 400 且说明原因 | `scene` 官方必填、最长 32 个可见字符 |
| 写闸关闭时 `POST /api/subscribe/send` | 404（路由根本没映射） | 写操作默认不暴露，而非「暴露了再判」 |
| 带假凭据调用三条业务路由 | 502 + 官方 `errcode`（如 `40013 invalid appid`） | 请求确实走到了 `api.weixin.qq.com`，属预期结果而非缺陷 |

真实凭据下才有意义的部分：`/api/session`（需端上现场 `wx.login`）、`/api/qrcode`（需有效应用令牌）、`/api/subscribe/send`（需用户订阅）。

## 6 实现要点（踩过的坑，改 Demo 前先读）

1. **两段装配顺序不可颠倒**：`AddMiniProgramServices(...).Build()` 会校验 `IMpAppManager` 已注册 ⇒ 漏了 `AddMpApp` 是**注册期 fail-fast**，不会静默少装模块。
2. **声明式客户端不替调用方判错**：官方 `errcode != 0` 时 HTTP 仍是 **200**，响应照样反序列化成功。判错必须由调用方消费 `IWechatApiResponse.IsSuccess`（或不判就把 `invalid code` 当成「拿到 openid」）。生成路径与手写路径的语义在这里分道：令牌链与二进制通道**抛异常**，由 `UseWechatApiErrorGate()` 统一映射 502。
3. **`session_key` 是会话密钥**（红线 MP-X7）：不下发前端、不入日志、不回显值，只回 `hasSessionKey`。生产要按用户持久化，用于解密端上加密数据与校验登录态。
4. **小程序码「成功是流、失败才是 JSON」**：SDK 已按 `Content-Type` 分支判错，宿主只拿到流；`WxaCodeResult` 是 `IDisposable`，且 **`IResult` 在委托返回之后才写响应** —— 直接 `Results.Stream(result.Content)` 会交给已关闭的流，故先缓冲再一次交回（小程序码只有几十 KB）。
5. **图片格式由官方决定**：非透明底出 JPEG、`is_hyaline=true` 出透明 PNG ⇒ 一律以 `result.ContentType` 为准，不要硬编码 `image/png`。
6. **订阅消息的 `data` 是扁平「关键词编号 → 值」**：键名须与所选模板逐项一致（`thing1` / `character_string2`…），否则报 `47003`；值还须符合该类型的长度与字符集（`thing` ≤20 字符等）。Demo 的入参刻意用 `Dictionary<string,string>`，不把 SDK 字典类型推给调用方。
7. **一次授权一条消息**：`43101` 不是「发送失败可重试」，是「用户没订阅」——重试无意义，得回端上重新申请。
8. **本线无 `/wxa/vip/`、无 `Security` 域以外的跨线端点**：小程序与公众号共用令牌域与部分路由前缀，跨线归属由 MP-X1 / MP-X6 守卫锁定，新增端点前先看守卫。

## 7 本 Demo 暴露并修复的 SDK 缺陷

**手工二进制通道不认默认应用**。`/api/qrcode` 在「只配一个默认应用、从不进入作用域」这一最常见宿主形态下抛
`InvalidOperationException：无法找到当前小程序（公众号）应用上下文（IMpAppContext）。请先进入应用作用域或配置默认应用。`
—— 提示语写着「或配置默认应用」，代码却只看 `IAppContextHolder.Current`，与生成客户端的
`Current ?? GetDefaultApp()` 口径不一致。

已同批修正六条手工通道（小程序码 / 反馈图片 / 客服临时素材 / 物流对账 + 公众号素材下载 / 非税账单）：
构造尾参增加**可选** `IMpAppManager? appManager = null`（DI 自动注入，既有手工构造与测试不受影响），
`ResolveAppContext()` 补默认应用回退。契约锁定：守卫 **MP-X10**（`MiniProgramContractGuards`）、
**MD11**（`MpMediaContractGuards`，覆盖公众号两条），行为回归 `MpMediaDownloadServiceTests.D9b`。

Demo 侧同时补齐两类「看起来成功」的缺陷：DM-03 / DM-04 / DM-05 所有响应汇总处改判 `IsSuccess`（票据缓存工厂另加
`WechatWorkException.ThrowIfFailed`，空 ticket 会被浏览器报 `invalid signature` 且绝不可缓存 2 小时），
并在 `Mud.Wechat.Demo.Shared` 新增 `UseWechatApiErrorGate()`（异常 → 502 + 错误码，不回堆栈）。

## 8 官方文档

- 登录凭证校验 `code2Session` <https://developers.weixin.qq.com/miniprogram/dev/server/API/user-login/api_code2session>
- 获取不限量小程序码 <https://developers.weixin.qq.com/miniprogram/dev/server/API/qrcode-link/qr-code/api_getunlimitedqrcode>
- 发送订阅消息 <https://developers.weixin.qq.com/miniprogram/dev/server/API/mp-message-management/subscribe-message/api_sendmessage>
- 订阅消息授权与一次性约束：端上 `wx.requestSubscribeMessage`（小程序开发指南「订阅消息」章）
- 本线公开面与逐域守卫：`Src/MiniProgram/*/README.md` + `Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/`

## 9 安全须知

密钥只走环境变量或本机用户机密，绝不写进配置文件、日志或提交仓库；不要指向生产小程序（订阅消息是**真实触达用户**的写操作）；
写端点默认不映射；`session_key` 与 `access_token` 一律不出响应体。
