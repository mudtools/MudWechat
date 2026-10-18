# Mud.Wechat.Work.WebDemo（DM-03）

企业微信**浏览器侧**场景 Demo：网页授权（OAuth）、JS-SDK 签名、发送应用消息。
与 [DM-01 通讯录](../Mud.Wechat.Work.ContactDemo/README.md)（服务端 API）、
[DM-02 通讯录回调](../Mud.Wechat.Work.ContactCallbackDemo/README.md) 构成 Work 线三件套。

## 覆盖能力

| 路由 | 场景 | 关键 SDK 面 |
|---|---|---|
| `GET /` | 入口页（静态） | `UseDefaultFiles` / `UseStaticFiles` |
| `GET /jssdk-demo.html` | `wx.config` + `wx.agentConfig` 自检页 | 前端调下面两个签名端点 |
| `GET /oauth/start?scope=` | 拼接网页授权 URL 并 302 | **SDK 不提供 URL 拼接**（宿主职责） |
| `GET /oauth/callback` | `code` 换用户身份 | `IWechatWorkInternalIdentityService.GetUserInfoAsync` |
| `GET /jssdk/config?url=` | `wx.config` 签名四元组 | `IWechatWorkInternalJsSdkService.GetCorpJsapiTicketAsync` + 宿主 SHA-1 |
| `GET /jssdk/agent-config?url=` | `wx.agentConfig` 签名 | `GetAgentJsapiTicketAsync` + 宿主 SHA-1 |
| `POST /api/messages/text` | 发送应用文本消息（**写闸**） | `IWechatWorkInternalMessageService.SendTextMessageAsync` |

模块注册：`AddWechatWorkServices(b => b.AddIdentityApi().AddJsSdkApi().AddMessageApi())`。

## 前置条件

1. 一个企业微信**自建应用**，且已配置：
   - 「企业可信域名」= 本 Demo 的访问域名（否则官方 50001）；
   - 「Web 网页授权」域名 = 同上（OAuth 回调域名不在这里配就换不到身份）；
   - 「JS 接口安全域名」= 同上（否则 `wx.config` 报 invalid signature 之外的域名错误）。
2. 生产语义下这些域名必须是**备案 HTTPS 域名**，本机 `127.0.0.1` 只能验证到「链接构造 + 端点连通 + 官方返回明确错误码」这一层；
   真实授权与 `wx.config` 成功需要企业微信客户端内打开。
3. 反向代理（frp / nginx）访问时须接 `UseForwardedHeaders`，否则 `redirect_uri` 会拼成内网地址
   （本 Demo 按当前请求的 `Scheme`/`Host` 构造回调地址，见 `Program.cs` 的 `BuildCallbackUrl`）。

## 环境变量

| 变量 | 必需 | 说明 |
|---|---|---|
| `MUDWECHAT_DEMO_WORK_CORP_ID` | 是 | 企业 ID，同时作为授权 URL 的 `appid` |
| `MUDWECHAT_DEMO_WORK_AGENT_ID` | 是 | 应用 AgentId：消息归属 + `agentConfig` 下发字段 |
| `MUDWECHAT_DEMO_WORK_AGENT_SECRET` | 是 | 应用 secret，换 `access_token` |
| `MUDWECHAT_DEMO_ALLOW_WRITE` | 否 | `=1` 才映射发消息端点 |

## 运行

```bash
dotnet build Demos/Mud.Wechat.Work.WebDemo -c Release
dotnet run --project Demos/Mud.Wechat.Work.WebDemo -c Release --no-build
# 监听 https://localhost:56551 / http://localhost:56552（launchSettings）
```

## 验证

无凭据即可验证的三项：

```bash
# ① 缺凭据 fail-fast：一次性点名全部缺失变量，进程不起
dotnet run --project Demos/Mud.Wechat.Work.WebDemo -c Release --no-build
# ② 授权链接构造正确（appid=CorpId、redirect_uri 已 encode、结尾 #wechat_redirect）
curl -i "http://127.0.0.1:56552/oauth/start"          # 期望 302 + Location
curl -i "http://127.0.0.1:56552/oauth/start?scope=snsapi_userinfo"   # 期望 400（企微不支持该 scope）
# ③ 写闸默认关闭 ⇒ 发消息 404；缺 url 参数 ⇒ 签名端点 400
curl -o /dev/null -w "%{http_code}\n" -X POST http://127.0.0.1:56552/api/messages/text \
  -H 'Content-Type: application/json' -d '{"toUser":"zhangsan","content":"hi"}'
curl "http://127.0.0.1:56552/jssdk/config"            # 期望 400 + 提示须传 url
```

真实凭据下：

```bash
curl -X POST http://127.0.0.1:56552/api/messages/text \
  -H 'Content-Type: application/json' \
  -d '{"toUser":"<你的测试成员userid>","content":"来自 WebDemo 的测试消息"}'
# 期望 {"msgId":"...","invalidUser":null,...}；invalidUser 非空即该成员不在应用可见范围
```

## 实现要点（容易踩的坑）

- **签名算法在宿主侧**：`jsapi_ticket=...&noncestr=...&timestamp=...&url=...` 取 SHA-1 小写十六进制，
  顺序不可变、不做 URL encode（`JsSdkSignature.cs`）。SDK 只给 ticket，**没有**签名方法 —— 别去找。
- **`url` 必须是待签名页面的真实地址**（去掉 `#` 及之后），所以签名端点要求前端显式传 `url`；
  端点自身的地址不是待签名地址，猜不得。
- **ticket 必须缓存**：官方限「一小时单应用 ≤100 次」，不缓存的 Demo 会先跑通、随后一直失败，
  症状像签名写错、实为配额打光（`JsApiTicketCache.cs`，提前 300s 续期 + 单飞闸，
  且企业 ticket 与应用 ticket **分槽**）。
- **ticket 不下发前端**：响应只回签名四元组；`user_ticket` 同理只回「是否存在」不回值。
- `scope` 与公众号不同：企微只有 `snsapi_base` / `snsapi_privateinfo`（`snsapi_userinfo` 是公众号的，
  本 Demo 显式 400 而不静默改写）。
- 授权 `code` 单次有效、5 分钟过期：刷新回调页必然失败，属预期，不是 bug。

## 安全注意

- 只用测试企业与测试成员；`ALLOW_WRITE` 别对生产开。
- 密钥只走环境变量 / 本机用户机密，不写进 `appsettings*.json`、不进日志、不进提交。
- 本 Demo 不改 `Src/` 与 `Tests/` 的任何契约面。

## 官方文档

- 91023 网页授权登录 · 98176 企业微信 Web 登录 · 95833 获取访问用户敏感信息
- 90506 JS-SDK 签名算法（自建应用）· 90236 发送应用消息
