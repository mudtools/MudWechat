# Mud.Wechat.Pay.V2.DataModels

微信支付 **APIv2 线**的官方 DTO 包：`PayV2/` 下按功能族分文件的请求/响应 DTO
（统一下单、付款码、查单关单、退款、红包、转账、企业付款、海关、分账、风控、代扣、行业、
车辆、账单评论等 **27 个功能族文件**），仅被 `Mud.Wechat.Pay.V2` 主包消费。

## 序列化形态（改前先读）

- 业务报文是 **XML**（`WechatPayV2XmlCodec` 扁平字段表 ⇄ `<xml>`；报文体内 `sign` 由
  `WechatPayV2SigningHandler` 出站补签 / 入站验签），**不是** JSON——DTO 层面没有
  `JsonPropertyName` 之类的 JSON 映射诉求。
- 但 DTO 仍统一标注 `[HttpJsonSerializable(SerializerClassName = "PayV2")]` 并生成
  `Generated/PayV2JsonContext.g.cs`：这是 **AOT 净零的统一管线**（`Mud.HttpUtils` 的
  `[HttpClientApi]` 内容序列化基建要求所有 DTO 面走源生成上下文，`IL2026`/`IL3050` 不因
  线的报文编码不同而豁免）。生成物**提交进版本控制、勿手改**；加 DTO 后重跑
  `AddHttpJsonSerializable.ps1` + `GenerateJsonContext.ps1`。
- 命名空间恒为 `Mud.Wechat.Pay.V2.DataModels`（`PayV2/` 子目录仅作组织）。

## 与 APIv3（`Mud.Wechat.Pay.DataModels`）的隔离理由（勿「顺手合并」）

- 两线**不共用** DTO、序列化器、命名客户端与契约守卫，也**不共用密钥**（`ApiV2KeySecretName`
  与 `ApiKeySecretName` 是两个字段、两把值）——合并即让两条传输线的 DTO 面互相进入彼此的
  JsonContext 登记与审计面（`audit-config-keys.ps1` / AB-G6 按 DTO 登记表双向不变式）。
- V2 大量响应是「`return_code` + 业务结果混合体」，字段命名与 V3 的 JSON 信封完全不同族；
  共享包会制造大量同名异义类型（如 `RefundQuery` 在两线字段集不同）。
- 依赖方向：本包只引 Core 叶层（`Mud.Wechat.Abstractions`），V2 主包引本包 + V2 Abstractions，
  与全仓 `主包 → {Abstractions, DataModels}` 家族形态一致。

## 守卫索引

支付线契约守卫见 `Tests/Mud.Wechat.Pay.Tests/`（DTO 上下文登记断言随域守卫同批维护）。
