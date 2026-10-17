# Mud.Wechat.OfficialAccount.Demo（DM-04 公众号 / 服务号）

一条工程覆盖公众号线的四个高频入口：**OAuth 网页授权**、**JS-SDK 签名**、**模板消息**、**消息与事件回调 + 被动回复**。
对应 Senparc Samples 里 `OAuth2Controller` / `JSSDKController` / `TemplateApi` / `WeixinController` 的合并面。

依赖的 SDK 公开面（Demo 不触碰任何 internal 类型）：

| 能力 | 公开面 |
|---|---|
| 应用底座 | `AddMpApp(Action<MpAppConfig>)`、`AddMpServices(b => b.AddSnsApi().AddUserApi().AddTemplateApi())` |
| 网页授权 | `IMpSnsService.GetAccessTokenAsync` / `GetUserInfoAsync`、`MpSnsGrantTypes` |
| JS-SDK | `AddMpJsApiSignature()`、`IMpJsApiSignatureService.SignAsync` |
| 模板消息 | `IMpTemplateService.SendTemplateMessageAsync`、`MpSendTemplateMessageRequest` |
| 回调 | `AddMpCallback(...)` + `.AddHandler<T>()` / `.AddReplyHandler<T>()`、`UseMpCallback()`、`MpCallbackPayloadHandler<TPayload>`、`IMpCallbackReplyHandler`、`MpCallbackReply.Text` |

## 1 路由

| 方法 | 路径 | 说明 |
|---|---|---|
| GET | `/healthz` | 非机密摘要（应用键、安全模式、回调路由、已注册处理器、写闸状态） |
| GET | `/`、`/jssdk-demo.html` | 静态页（`wwwroot/`，随构建落位） |
| GET | `/oauth/start?scope=` | 302 到微信授权页；`scope` 只接受 `snsapi_base` / `snsapi_userinfo`，其余 400 |
| GET | `/oauth/callback?code=&state=` | 授权码换用户身份；`state` 承载 scope 回传（微信回调只带 `code` + `state`） |
| GET | `/jssdk/config?url=` | `wx.config` 五字段（`appId` / `url` / `nonceStr` / `timestamp` / `signature`），**不含 `jsapi_ticket`** |
| POST | `/api/templates/send` | 模板消息（写操作，默认**不映射** ⇒ 404；`MUDWECHAT_DEMO_ALLOW_WRITE=1` 后启用） |
| GET / POST | `/mp/default` | 回调入口：GET 为 URL 验证（回显 `echostr`），POST 为消息与事件推送 |

## 2 前置条件（后台配置，缺一即「接口通、页面白」）

1. **网页授权域名**：公众号后台「设置与开发 → 公众号设置 → 功能设置 → 网页授权域名」填本 Demo 对外域名（不含协议与路径），且域名根目录需可访问微信下载的验证文件。否则 `/oauth/start` 直接报 `redirect_uri` 参数错误。
2. **JS 接口安全域名**：同页设置，`/jssdk-demo.html` 所在域名必须在列表内，否则 `wx.config` 报 `invalid url`。
3. **服务器配置（回调）**：「设置与开发 → 服务器配置」的 URL 填 `https://<域名>/mp/default`，Token 与 EncodingAESKey 与本 Demo 的环境变量一致；**消息加解密方式选「安全模式」**（SDK 默认模式，明文推送会被 403 拒收）。该 URL 必须外网 HTTPS 可达，本机调试需内网穿透。
4. **模板消息**：需**已认证的服务号**，并在「模板库」选取模板后把模板 ID 放进 `MUDWECHAT_DEMO_MP_TEMPLATE_ID`；`data` 的键名须与所选模板字段逐项一致（`first` / `keyword1`… / `remark`）。

## 3 环境变量

| 变量 | 必填 | 用途 |
|---|---|---|
| `MUDWECHAT_DEMO_MP_APP_ID` | 是 | 应用 AppId；同时是回调解密后 `appid` 校验基准（receiveid） |
| `MUDWECHAT_DEMO_MP_APP_SECRET` | 是 | 应用令牌与 sns 换码凭据 |
| `MUDWECHAT_DEMO_MP_CALLBACK_TOKEN` | 是 | 回调验签 Token（与后台「服务器配置」一致） |
| `MUDWECHAT_DEMO_MP_CALLBACK_AES_KEY` | 是 | 回调 EncodingAESKey，**43 位** Base64 |
| `MUDWECHAT_DEMO_MP_TEMPLATE_ID` | 否 | 模板消息模板 ID；未设置时发送端点直接返回配置提示 |
| `MUDWECHAT_DEMO_ALLOW_WRITE` | 否 | `=1` 时才映射写端点 |

缺失时启动即失败，并一次性列出**全部**缺失项（不会「改一个、跑一次、再报下一个」）；值里含 `<` / `>` / `your-` 视为占位符，等同于未配置。

## 4 运行

```bash
# 1) 构建
dotnet build Demos/Mud.Wechat.OfficialAccount.Demo -c Release

# 2) 无凭据：应立刻失败并列出缺失变量（预期行为，用于验证 fail-fast）
dotnet run --project Demos/Mud.Wechat.OfficialAccount.Demo -c Release --no-build

# 3) 带凭据启动（默认 http://localhost:56554）
export MUDWECHAT_DEMO_MP_APP_ID=wx……           # 公众号 AppId
export MUDWECHAT_DEMO_MP_APP_SECRET=……
export MUDWECHAT_DEMO_MP_CALLBACK_TOKEN=……
export MUDWECHAT_DEMO_MP_CALLBACK_AES_KEY=……   # 43 位
dotnet run --project Demos/Mud.Wechat.OfficialAccount.Demo -c Release --no-build
```

## 5 离线可验证部分（无需真实公众号）

回调链路的四条闸（验签 → 时效 → 解密 + appid → 一次性指纹）可用 SDK 自身的密码学内核构造报文离线验证：

```bash
# 终端 A：带上表里的四个变量启动 Demo
# 终端 B：
dotnet run --project Demos/Mud.Wechat.Demo.SelfTest -c Release -- --channel mp
```

五条断言全绿即证明「Demo 侧配置 + SDK 接收链」两端一致：

| 断言 | 期望 | 锁定的契约 |
|---|---|---|
| GET URL 验证回显 `echostr` 原文 | 200 且响应体 = `echostr` **密文原文** | GET 用**三参** `signature`（不含 echostr）、回显不解密 |
| POST 文本消息触发被动回复 | 200 且回包为加密 XML，解密后 `receiveid = AppId`、明文含回复文案 | 四参 `msg_signature` + 安全模式回包必须加密 |
| 同一报文重放 | 403 | 一次性指纹闸 fail-closed |
| 篡改 `msg_signature` | 403 | 验签失败不消耗指纹、不回 `success`（防探测） |
| 安全模式拒收明文报文 | 403 | POST 密文分支**不得**回落三参 `signature`（CB-MP-2） |

真实凭据下才有意义的部分：`/oauth/*`（需微信内置浏览器 + 已配授权域名）、`/jssdk/config`（需能取到 `jsapi_ticket`）、`/api/templates/send`（需服务号 + 模板）。
用假凭据调用时，SDK 会把官方 `errcode` 原样抛出（如 `40013 invalid appid`），`UseWechatApiErrorGate()` 把它映射为
**502 + `{errorCode, errorMessage, requestUri}`**（`requestUri` 已在异常构造期脱敏）——
这说明请求确实走到了微信 API，属预期结果而非缺陷。

## 6 与企微线（DM-03）的边界差异

| 面 | 公众号（本工程） | 企业微信（`Mud.Wechat.Work.WebDemo`） |
|---|---|---|
| JS-SDK 签名 | **SDK 内置**：`AddMpJsApiSignature()` + `IMpJsApiSignatureService.SignAsync` | SDK 只提供票据，签名算法由宿主自算（`sha1(jsapi_ticket=…&noncestr=…&timestamp=…&url=…)`） |
| 票据缓存 | SDK 内的 `IMpJsApiTicketManager`（7200s + 持久层 + 刷新去重） | 宿主自建缓存 |
| 网页授权 scope | `snsapi_base` / `snsapi_userinfo` | `snsapi_base` / `snsapi_privateinfo`（其余值直接 400） |
| 换码得到的令牌 | **用户级** `access_token` + `refresh_token`（30 天生命周期归宿主） | 用户身份 `userid`（应用令牌由 SDK 管理） |
| 回调 GET | 三参 `signature`，回显 `echostr` 密文原文 | 四参 `msg_signature`，回显解密明文 |
| 回调 POST | 四参 `msg_signature`，`receiveid = AppId` | 四参 `msg_signature`，`receiveid = CorpId` / `SuiteId` |
| 分发软超时出口 | 回 `success` + **200**（公众号重发整封消息，503 只会放大重复） | **503** 触发官方重推 |

两条线的边界不同，**不要互相套用**：`IMpJsApiSignatureService` 在企微线不存在，`IWechatWorkJsSdkService` 也不产出公众号签名。

## 7 实现要点（踩过的坑，改 Demo 前先读）

1. **sns 整域免应用令牌**：授权码换的是用户级令牌，请求参数须显式带 `appid` + `secret`，与应用级 `access_token`（业务接口用）是两套凭据，不可混用。
2. **`code` 一次性、5 分钟过期**：不要在 `/oauth/callback` 里重试换码；重复使用报 `40163 code been used`。
3. **用户级令牌不得回显**：`/oauth/callback` 只返回 `hasUserAccessToken` / `hasRefreshToken` 两个布尔。令牌进响应 = 把它交给浏览器；30 天生命周期与续期是宿主的活。
4. **`jsapi_ticket` 绝不下发前端**：`SignAsync` 的结果刻意只有五字段。页面 URL 必须是**当前页面完整 URL（含 query、不含 `#`）**——SDK 会去除 `#` 片段，但去不掉「用错了 URL」这件事。
5. **被动回复 5 秒契约**：`IMpCallbackReplyHandler` 内**禁止**远程调用、发消息、写慢存储；需要重活就回 `success` 后走客服/模板消息。被动回复不受「48 小时客服会话窗口」限制，这是它与客服消息的本质区别。
6. **回复处理器只看信封**：需要消息正文时经 `IMpPayloadReader` 读同一份解密明文（与分发器共用读取器，不存在两套解析逻辑）。
7. **排重是宿主职责**：`MpReplyDeduper` 只挡「同 `MsgId` 重复进」，进程内实现且容量到限即清空；官方的 5 秒 × 3 次重推会换新 `nonce`/`timestamp`，**穿过 SDK 指纹闸**，因此业务侧唯一键（生产建议落库）不可省。
8. **两道 fail-closed 闸不得为跑通而削弱**：时效窗 ±300s、一次性 SHA1 指纹（不落盘密文本身）。要「跑通」请改后台配置，不要改 SDK。
9. **静态页 404 的根因**：Demo 常被直接执行 `bin` 下的 dll（门禁脚本 / CI 冒烟），此时内容根是 bin 目录 ⇒ `wwwroot/**` 必须 `CopyToOutputDirectory` 且内容根钉在 `AppContext.BaseDirectory`。

## 8 本 Demo 暴露并修复的 SDK 装配缺陷

首次以 `ValidateOnBuild=true`（ASPNETCORE_ENVIRONMENT=Development 默认开启）组装「`AddMpApp` + `AddMpJsApiSignature`」时，建容器阶段即失败：

- `IMpJsApiTicketManager` 按应用装配在 `MpAppContext` 内，根容器没有该项 ⇒ 签名服务无法从根容器解析；
- 未命名 `IOptions<MpAppConfig>` 从未登记（`AddMpApp` 只登记 `List<MpAppConfig>` 与逐应用命名客户端）⇒ 签名服务取 `AppId` 时被校验器打回「AppKey 不能为空」。

两者均已在 `AddMpApp` 内以**默认应用**为桥补齐（`TryAdd`，宿主可预注册覆盖），回归用例为
`Tests/Mud.Wechat.OfficialAccount.Tests/MultiApp/MpAppManagerTests.cs` 的 **A17**。
此前所有签名用例用 Mock 直接构造服务，装配面一直无人看守 —— 这正是 Demo 体系存在的理由。
多公众号宿主不要依赖未命名快照，应按当前上下文取 `IMpAppContext.Config` 与票据管理器。

## 9 官方文档

- 网页授权 <https://developers.weixin.qq.com/doc/service/api/webdev/access/api_snsauth.html>
- 换取用户级令牌 <https://developers.weixin.qq.com/doc/service/api/webdev/access/api_snsaccesstoken.html>
- 刷新用户级令牌 <https://developers.weixin.qq.com/doc/service/api/webdev/access/api_snsrefreshtoken.html>
- 拉取用户资料 <https://developers.weixin.qq.com/doc/service/api/webdev/access/api_snsuserinfo.html>
- 获取 `jsapi_ticket` <https://developers.weixin.qq.com/doc/subscription/api/webdev/js/api_getticket.html>
- 发送模板消息 <https://developers.weixin.qq.com/doc/service/api/notify/template/api_sendtemplatemessage.html>
- 回调接收链路的逐条契约：`Src/OfficialAccount/Mud.Wechat.OfficialAccount.Callback/README.md`

## 10 安全须知

密钥只走环境变量或本机用户机密，绝不写进配置文件、日志或提交仓库；不要指向生产公众号（模板消息与群发是**真实触达用户**的写操作）；写端点默认不映射。
