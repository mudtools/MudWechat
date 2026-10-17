# Work.ContactCallbackDemo — 企业微信通讯录变更回调

## 目的

演示回调接收链路的完整闭环：**URL 验证 → 验签 → AES 解密 → receiveid 校验 → 抗重放 → 事件解析 → 类型化处理器分发**，
覆盖成员（`create_user`/`update_user`/`delete_user`）、部门（`create_party`/`update_party`/`delete_party`）、
标签（`update_tag`）七类通讯录变更事件。

## 覆盖的 SDK 能力

| 能力 | 类型 | 说明 |
|---|---|---|
| `AddWechatApp` | DI 装配 | 令牌/仓储底座；`AddWechatCallback` 的 `TryAdd` 仓储依赖此项，**必须先注册** |
| `AddWechatCallback(Action<WechatCallbackOptions>)` | 回调装配 | 回调凭据（唯一来源是 `Apps` 字典）+ 路由前缀 |
| `AddHandler<T>` | 处理器注册 | 类型化载荷处理器（组合根期急切注册） |
| `UseWechatWebhook()` | 中间件 | GET = URL 验证，POST = 验签 + 解密 + 分发 |
| `WechatCallbackPayloadHandler<TPayload>` | 处理器基座 | 强类型载荷，编译期映射、无反射 |
| `ContactUserChangedPayload` 等 | 载荷契约 | `[PayloadContract]` + `[WechatCallbackContract]` |
| `WechatCallbackEventTypes.CreateUser` 等 | 事件键常量 | `SupportedEventType` 精确匹配（写常量由 MUDCB004 校验） |
| `DemoEnv` / `MapDemoHealth` | Demo 基座 | 环境变量校验、健康检查 |

## 前置条件

- .NET 8 SDK；
- 测试企业的**自建应用**或「通讯录同步」应用，已在后台配置接收消息 URL 与 Token / EncodingAESKey；
- 若没有公网 URL，用 [`Demos/Mud.Wechat.Demo.SelfTest`](../Mud.Wechat.Demo.SelfTest/README.md) 在本机离线验证同一链路。

## 环境变量

| 变量 | 必填 | 说明 |
|---|---|---|
| `MUDWECHAT_DEMO_WORK_CORP_ID` | 是 | 企业 CorpId；同时作为自建应用回调的 `ReceiveId` |
| `MUDWECHAT_DEMO_WORK_AGENT_SECRET` | 是 | 自建应用 secret（令牌底座用） |
| `MUDWECHAT_DEMO_WORK_CALLBACK_TOKEN` | 是 | 后台「接收消息」配置的 Token |
| `MUDWECHAT_DEMO_WORK_CALLBACK_AES_KEY` | 是 | 43 位 EncodingAESKey |

## 运行

```bash
dotnet run --project Demos/Mud.Wechat.Work.ContactCallbackDemo
```

企业微信后台填写回调 URL：`https://<你的域名>/wechat/default`（`default` = AppKey，与 `GlobalRoutePrefix` 组成路由），
Token / AESKey 必须与上面环境变量一致。通讯录变更事件在「通讯录同步」或自建应用的接收消息设置中勾选。

## 验证

| 步骤 | 期望 |
|---|---|
| `GET /healthz` | 回显路由 / 通道 / 处理器数量，**不含** Token 与 AESKey |
| GET URL 验证（后台保存配置或 SelfTest） | 返回解密后的明文 `echostr`，200 |
| POST 一条 `create_user` 报文 | 日志出现 `新增成员：UserId=...`，响应体 `success` |
| 同一条报文 5 分钟内再 POST 一次 | **403**（一次性指纹闸，fail-closed 属预期行为，不是 Demo 故障） |
| 篡改 `msg_signature` | 403（验签失败） |
| `timestamp` 偏离当前 >300s | 403（时效闸） |

SelfTest 一键跑完上面三条回调断言：

```bash
dotnet run --project Demos/Mud.Wechat.Demo.SelfTest -- --channel work --base-url http://localhost:56534
```

## 行为要点（勿改）

- **加解密**：官方 32 字节块 PKCS7 手工补位/剥离；SDK 已实现，Demo 不得替换为 .NET 内置 16 块填充（会误拒官方 pad∈[17..32] 的报文）。
- **指纹闸位置**：在「解密 + receiveid 校验成功」之后、分发之前。因此同一条报文重推被 403，
  而官方超时重推（新报文）仍会再次进入处理器 ⇒ **处理器必须幂等**。
  本 Demo 用 `Services/DemoIdempotencyTracker`（进程内字典，仅作教学）演示幂等键；
  生产须换成数据库唯一键。
- **分发软超时** 默认 4500ms：处理器内禁止长耗时同步（DB 大批量、下游 RPC 阻塞），否则 503 触发官方重推。
- **未知 AppKey**（`POST /wechat/unknown`）不劫持管线，落到路由末端 404。
- 若自建应用误配 `Channel = Suite`，`WechatAppCallbackOptions.Validate` 在请求期即拒。

## 安全注意

- 日志禁止出现 `PushToken` / `PushEncodingAESKey` / 信封 `DecryptedXml` / `SuiteTicket` / `AuthCode`（SDK 侧已脱敏，处理器不得自行打印明文）。
- 密钥只走环境变量或本机用户机密，不写进 `appsettings.json`，不提交仓库。
- 通讯录变更事件的 `FromUserName` 固定为 `sys`，SDK 不会用它伪造 `AuthCorpId`（见决策 D10）。

## 官方文档

- 回调配置与加解密方案：<https://developer.work.weixin.qq.com/document/path/90968>
- 通讯录变更事件：<https://developer.work.weixin.qq.com/document/path/90967>
- 接收消息/事件服务器配置：<https://developer.work.weixin.qq.com/document/path/90930>
