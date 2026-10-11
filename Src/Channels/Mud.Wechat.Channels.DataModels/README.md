# Mud.Wechat.Channels.DataModels

微信小店 / 视频号线**官方 DTO 包**：`[HttpJsonSerializable]` 标注的请求 / 响应 / 嵌套对象模型 + 生成物 `Generated/*JsonContext.g.cs`（AOT JSON 上下文）。零业务逻辑、零 HTTP 语义——接口声明在主包，序列化语义在生成上下文。

## 公开面

| 目录 | 域 | 状态 |
| --- | --- | --- |
| `Basic/` | 基础接口 | ✅ P1 |
| `Funds/` | 资金结算 | ✅ P1 |
| `Product/` | 商品管理（商品 / 库存 / 赠品 / 买赠 / 限时抢购） | ✅ P1 |
| `Order/` | 订单管理 | ✅ P1 |
| `Aftersale/` | 售后管理（售后单 / 纠纷单 / 保障单） | ✅ P1 |
| `Logistics/` | 物流发货（地址 / 运费模板 / 电子面单 / 发货） | ✅ P1 |
| `ChannelsResponse.cs` | 基线响应（`errcode`/`errmsg`，实现 `IWechatApiResponse`） | ✅ |
| `Generated/` | 7 个域 JsonContext（Common + 6 业务域，`internal`，经 `InternalsVisibleTo` 供主包与测试直读） | 随域增长 |

P2 各域 DTO 随接口落地同批进入本包（命名空间 `Mud.Wechat.Channels.DataModels.{域}`）。

## 已踩陷阱与边界（改动前必读）

- **生成物勿手改**：`Generated/*JsonContext.g.cs` 由 `scripts/GenerateJsonContext.ps1` 生成并提交版本控制；新增 / 修改 DTO 后必须重跑 `AddHttpJsonSerializable.ps1` + `GenerateJsonContext.ps1`，并同批更新逐域守卫的登记计数断言。
- **官方参数名照抄**：含官方历史拼写（如售后 `USER_CANCELD`、保障 `STATUS_WAIT_OP_COMFIRM`），**勿「顺手纠正」**——逐域守卫以字面量锁定，改动即变红。
- **`SerializerClassName` = 命名空间域段**（如 `Aftersale`）；常量类（`static class` + `public const`）**不得**标 `[HttpJsonSerializable]`（无序列化语义，逐域守卫锁定）。
- **共用 DTO 裁决**：字段集一致的端点共用请求 / 响应模型（如 `ChannelsAftersaleReshipRequest` 三个发货端点共用），严禁同字段重复建模；两个独立官方端点参数表可独立演化时按端点分建（如纠纷留言 / 举证两请求）并在守卫记录裁决理由。
- **官方反直觉点**：多数查询接口官方即 POST；`product/auditstrategy/get`、`getauditquota`、`aftersale/reason/get`、`rejectreason/get` 等为「POST + 空请求体」形态，不得人为造请求体 DTO。
