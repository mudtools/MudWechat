# Mud.Wechat.Channels.Callback

微信小店 / 视频号线**回调接收包**——**当前为 P3 收口前的脚手架占位包**（零运行时类型，仅装配工程形态：依赖方向、生成器挂接、分析器随包下发、ASP.NET 传输层按 TFM 分流）。本文件同时记录 P3 的**规划契约**，落地时以此为准、不得弱化。

## 当前形态（P0~P2 期，占位有意为之）

- 程序集**零手写类型**：`Tests/Mud.Wechat.Channels.Callback.Tests/ChannelsCallbackScaffoldGuards` 显式按名加载本程序集（防 slnx 漏配 / 空库不装载导致测试假红），并锁定「不引用既有线任何回调包」。
- 已挂接 `Mud.Wechat.Callback.Generator`（发射 `ChannelsPayloadContracts.RegisterAll`）与 `Mud.Wechat.Callback.Analyzers`（MUDCB002~005，随包 `analyzers/dotnet/cs` 下发）——P3 落 `[WechatCallbackContract]` / `[PayloadContract]` 时即刻生效，无需再改工程。
- 依赖方向：`→ {Channels.Abstractions, Channels.DataModels}`，**不引用主包**（硬边界，CH-X 守卫锁定）。

## P3 规划契约（落地时同批补齐守卫，缺一即半成品）

- **报文体系**：官方微信小店回调为 XML（`msg_signature` + `EncodingAESKey` + `receiveid`），与公众号回调同构——复用 Core `WechatCallbackCrypto`（32 字节块 PKCS7 手工补位，禁 .NET 内置 16 块 PKCS7）与 `IWechatCallbackReplayGuard`。
- **抗重放两道闸 fail-closed**：时间戳 ±300s + 一次性 SHA1 指纹（指纹闸在「解密 + receiveid 校验成功」之后、事件返回之前；GET echo 只过时效闸不消费指纹）；分布式守卫异常必须上抛（→ 5xx → 官方重试），禁止吞异常放行。
- **事件键与载荷族**：按官方报文结构族建 `Events/Payloads/`（订单 / 售后 / 物流 / 纠纷 / 优惠券 / 库存 / 会员等），声明式登记进生成器档位；**不得**新增「逐事件 DTO + 手写 ParseXxx」。
- **分发语义**：组合根急切注册、无 Freeze；同步分发 + 软超时默认 4500ms（< 5s 契约）；指纹在分发前消费 ⇒ 处理器须幂等；单处理器异常隔离。
- **配置面**：`ChannelsCallbackOptions`（节 `ChannelsCallback`），`Apps` 字典为凭据唯一来源，路由 `/{Prefix}/{AppKey}`；与建文件同批登记 `audit-config-keys.ps1`。

## 已踩陷阱与边界（改动前必读）

- **包元数据如实**：csproj `Description` 已注明占位状态——P3 落地时同步改回能力描述，勿在占位期宣称未实现的功能面。
- **不改工具本体**：本线回调登记走生成器「档位」加挂（发射 `ChannelsPayloadContracts.RegisterAll`），`Mud.Wechat.Callback.Generator` / `Mud.Wechat.Callback.Analyzers` 为中立工具，不得为小店线改其源码。
- **回调凭据不入主配置**：`WechatChannelsCallbackOptions`（P3 落地时）与 `ChannelsAppConfig` 是两个面——回调 AppKey / Token / EncodingAESKey 只在回调配置声明。
