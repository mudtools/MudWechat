# Mud.Wechat.Channels.Callback

微信小店 / 视频号线**回调接收包**（P3 已收口）：官方**强制 JSON + 仅安全模式**回调的验签 / 解密 / AppId 校验 / 抗重放 / 事件分发全链路，HTTP 中间件与多应用路由。**42 个官方回调事件键**经 `[ChannelsCallbackContract]` 声明式登记（`ChannelsPayloadContracts.RegisterAll` 由生成器档位发射），net8.0+ 提供类型化处理器通道。

## 公开面

| 面 | 入口 | 说明 |
| --- | --- | --- |
| 配置 | `AddWechatChannelsCallback`（节 `ChannelsCallback`） | `Apps` 字典为凭据唯一来源（PushToken / PushEncodingAESKey 43 位 / AppId），路由 `/{GlobalRoutePrefix}/{AppKey}`（默认前缀 `channels`）；`IConfiguration` 重载惰性绑定、请求期热更 |
| 接入 | `app.UseWechatChannelsWebhook()` | 经典约定式 `RequestDelegate` 中间件（不进 DI 泛型派发） |
| 接收 | `IWechatChannelsCallbackReceiver` | 验签（GET 三参 `signature` / POST 四参 `msg_signature`，双验签与公众号同构）→ 时效 ±300s → AES 解密 → AppId 校验（解密明文尾部 receiveid）→ 一次性指纹 → 明文 JSON 顶层字段解析 |
| 分发 | `WechatChannelsCallbackDispatcher` | 组合根急切注册、无 Freeze；软超时默认 4000ms（< 官方 5s 契约）；匹配序 appKey 专属精确 → 全局精确 → 专属兜底 → 全局兜底；单处理器异常隔离 |
| 处理器 | `IChannelsCallbackEventHandler`（信封级）/ `WechatChannelsCallbackPayloadHandler<TPayload>`（类型级，net8+） | 类型化载荷经事件键 → `ChannelsPayloadContract` → `CallbackJsonContext` 源生成反序列化（AOT 安全，无反射） |
| 拦截器 | `IChannelsCallbackEventInterceptor` | Before（可中断 → 回 success 不触发重推）/ After（观测） |
| 事件键 | `ChannelsCallbackEventTypes`（42 键常量，落 DataModels/Callback） | 与契约登记键集 1:1（CB-CH-7 守卫闭环断言） |

## 报文体系（官方实测口径）

- 官方回调**强制 JSON 报文 + 仅安全模式**（不可选明文/兼容；XML 为设计文档 v1 §3.5 的错误假设，2026-10 逐页核对官方 `doc/store/shop/dev_before/message_push` 修正）——包体 `{"ToUserName":"gh_*","Encrypt":"..."}`，解密明文为 JSON 事件。
- 验签与 AES-256-CBC（PKCS#7 32 字节块）与公众号同算法，复用 Core `WechatCallbackCrypto` 与 `IWechatCallbackReplayGuard`；解密明文尾部 `appid` 必须与回调配置 `AppId` 一致。
- 应答为明文 `success`/空串（无被动回复；ewaybill / 客诉工单 / 会员三事件官方要求结构化 JSON 应答，后续按需扩展回复写回）。
- 官方重试策略：5 秒未响应即失败，0s/0s/10s/1min/5min/30min/60min… 至多 10 次（约 3.6 小时）——软超时出口为 200 + success（指纹已消费 ⇒ 重推同指纹被 403，**处理器须幂等**）。

## 已踩陷阱与边界（改动前必读）

- **事件键常量落 DataModels/Callback（勿「修正」回 Abstractions）**：载荷 DTO 是 JSON 模型必须落 DataModels 承接源生成上下文，而 DataModels 不引用 Abstractions（依赖方向 `Abstractions → DataModels`）⇒ 特性引用的常量类只能同域落位（与载荷契约特性同命名空间）。
- **契约机制在 DataModels、信封与处理器契约在 Abstractions**：`[ChannelsCallbackContract]` / `ChannelsPayloadContract` / 注册表随载荷落 `DataModels.Callback`；`WechatChannelsCallbackEvent` / 处理器 / 拦截器接口落 `Abstractions.Callback`。
- **生成器挂载在 DataModels 工程**（载荷所在地；对齐企微线「生成器只挂载荷工程」先例）：跨程序集发射 partial 实现会产出 CS0759。
- **类型化通道以 `CHANNELS_CALLBACK_TYPED_PAYLOAD` 编译开关启用**（Callback csproj 已定义）：载荷 DTO + `CallbackJsonContext` 已落地；低目标框架（ns2.0/net6）类型化处理器在运行期被跳过（记 Error），信封级处理器不受影响。
- **抗重放两道闸 fail-closed 不得为兼容降级**：时间戳 ±300s（缺失/非数字即拒）+ 一次性 SHA1 指纹（解密 + AppId 校验成功之后、事件返回之前消费；GET echo 只过时效闸不消费指纹）。
- **MUDCB002~005 分析器只覆盖企微档位**（硬编码 metadata name）：小店线契约一致性由 CB-CH 系列测试守卫承担（`Tests/Mud.Wechat.Channels.Callback.Tests/`），与公众号线 CB-MP 系列同款分工。
