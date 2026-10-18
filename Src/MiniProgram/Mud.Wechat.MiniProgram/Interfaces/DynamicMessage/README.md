# DynamicMessage 域 · 动态消息

微信小程序「动态消息」域：**3 支端点**（`activityid/create` + `updatablemsg/send` + `chattoolmsg/send`）。
接口 [IWxaDynamicMessageService.cs](./IWxaDynamicMessageService.cs)，注册组 `DynamicMessage`
（`RegistryGroupName = "DynamicMessage"`、`TokenManage = nameof(IMpAppManager)`），
DI 组方法 [`AddDynamicMessageApi()`](../../Extensions/MiniProgramServiceBuilder.cs)（`MiniProgramModule.DynamicMessage`）。
接口 XML 记本域核验日为 **2026-10-10**（依据官方清单）；本 README 的复核日为 **2026-10-18**。

> **事实来源限定**：本文所有断言只来自本仓的接口文件、契约守卫与 DTO 声明，
> 不引用任何外部文档原文。凡本仓核不到的（官方频率数字、错误码全集、私密消息系列本仓的消费面），
> 一律集中列在文末「已知留档缺口」并标 **未逐项核验**，不做推测。

## 端点速览

令牌域**复用** `MpTokenTypes.AccessToken`（值 `"Wechat.Mp.AccessToken"`），注入 **Query 的 `access_token`**
（守卫 MP-X2 / MP-X3）。三支**全部 POST**、全部走请求体。

| # | 方法 | HTTP 动词 | 路由 | 令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `CreateActivityIdAsync` | POST | `/cgi-bin/message/wxopen/activityid/create` | `access_token`(Query) | 体 `WxaCreateActivityIdRequest`；官方**无必填参数**，可为空对象 |
| 2 | `UpdateDynamicMessageAsync` | POST | `/cgi-bin/message/wxopen/updatablemsg/send` | `access_token`(Query) | 体 `WxaUpdateDynamicMessageRequest`；含 `target_state`；应答仅 `errcode`/`errmsg` |
| 3 | `SendChatToolMsgAsync` | POST | `/cgi-bin/message/wxopen/chattoolmsg/send` | `access_token`(Query) | 体 `WxaChatToolMsgRequest`；**不含** `target_state`；应答仅 `errcode`/`errmsg` |

## 路由前缀族：本域是小程序线少见的 `/cgi-bin/` 族

本域 3 支**全部落在 `/cgi-bin/message/wxopen/` 之下**，与小程序线主流的 `/wxa/`、`/datacube/`
前缀族**不同**：

| 前缀族 | 本域支数 | 备注 |
|---|---|---|
| `/cgi-bin/message/wxopen/…` | 3 | `activityid/create` / `updatablemsg/send` / `chattoolmsg/send` |

- **`/cgi-bin/` 不代表属公众号线**：本线与公众号线共用微信公众平台令牌域，
  但**路由不得重复**（守卫 MP-X6 会判红本线与 OfficialAccount 的路由交集）。
- **`activityid/create` 的层级易抄错**：是 `message/wxopen/activityid/create`
  （`activityid` **连写、不带下划线**、也没写成 `activity_id`），
  而另外两支是 `message/wxopen/{updatablemsg,chattoolmsg}/send`（`…msg` **连写**、动词 `send` 在后）。
  ⇒ 「名词/create」与「名词/send」两种排布在同域并存，别按统一模板生成。
- `wxopen` 一段是本域标识，`SubscribeMessage` 域的 `/cgi-bin/message/subscribe/send`
  与之同层但**不同段**，跨域封装**不得**把 `wxopen` 当可配置前缀抽出。

## 三支之间的调用时序与「一次性」语义

1. 先 `activityid/create` 取 `activity_id`（接口 XML：同一 `activity_id` **只能被使用一次**
   ——分享后即失效，**不可复用于多条动态消息**；需再次分享须重新创建）。
   应答 `WxaCreateActivityIdResponse { ActivityId→activity_id string?,
   ExpirationTime→expiration_time long?, IsExpired→is_expired bool? }`。
2. `expiration_time` 是 **Unix 秒级时间戳**（接口 XML 原文），更新须在分享卡片的有效期内进行。
   ⇒ 不要按毫秒解析；`is_expired` 由官方给出，本线不本地推算有效期。
3. 再 `updatablemsg/send` 按 `target_state` 更新已分享卡片内容。

**私密消息入口**：接口 XML 记第 1 支同时是私密消息（官方 `openapi/privatemessage` 系列）的
入口凭证来源；该系列本仓**未声明任何端点**（属未消费面，见缺口）。

## `target_state`：游戏态语义，且只有一支有

- `WxaUpdateDynamicMessageRequest.TargetState`→`target_state`，类型 **`long?`**，
  官方取值 **`0` = 未进入游戏 / `1` = 已进入游戏**（用于区分玩前 / 玩中 / 玩后展示的卡片内容）。
- 第 3 支 `WxaChatToolMsgRequest` **没有** `TargetState` 属性 ⇒ 两请求体不同构，
  不得把动态消息的请求体直接投给聊天工具端点（多余键会被官方忽略、意图丢失）。
- 本仓**未**建 `target_state` 枚举类型（照原文以 `long` 传）。取值集合是否只有 0/1
  **未逐项核验**（见缺口）。

## `parameter_list`：两支同构的模板表达

两支 send 都通过 `TemplateInfo`→`template_info`（`WxaDynamicMessageTemplateInfo`）承载内容，
其唯一属性 `ParameterList`→`parameter_list` 是 `List<WxaDynamicMessageParameter>?`，
每项 `{ Name→name string?, Value→value string? }`；`Value` 即具体展示文本
（一键更新同一卡片在各处的展示）。

- 该嵌套类型被第 2、3 支**共用**（同构 ⇒ 一份，各建即双源）。
- 本仓未核验 `parameter_list` 的长度上限与 `name` 的合法集合（见缺口）。

## 应答信封：两支只有 `errcode`/`errmsg`

第 2、3 支返回 **`WxaResponse`**（`ErrorCode` / `ErrorMessage`，`IsSuccess => ErrorCode == 0`）；
第 1 支返回带三字段的 `WxaCreateActivityIdResponse`。本域**没有**批量结果、逐条码或
`resultcode`/`resultmsg` 那类双层信封。

本域三支都是普通 JSON 通道：
**不返回流、不返回文本**，无 `IDisposable` 应答（本线需要释放的是 `QrCodeLink` 的 `WxaCodeResult`，
文本手工通道在 `Express` 的 `get_bill`）。

## 令牌与代调用面

- 本域属**消息发送**类，令牌仍走小程序应用级 `access_token`（Query，MP-X3）。
- **本线不建 component / authorizer 令牌链**（守卫 P3-R2，
  见 [MiniProgramP3RulingsContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramP3RulingsContractGuards.cs)）。
  本域接口 XML **未**表态第三方代商家调用是否适用 ⇒ **未逐项核验**（见缺口）。
- **本域不消费消息事件**：动态消息是「主动更新已分享卡片」，不是事件回调；
  消息与事件的接收属**公众号 XML 通道**（见文末「改动本域需要同批完成」第 6 条）。

## 快速开始

```csharp
using Mud.Wechat.MiniProgram.DataModels.DynamicMessage;

// 两段式装配：MP 令牌底座先行，再装本域模块
builder.Services.AddMpApp(builder.Configuration, "MpApps")
               .AddMiniProgramServices(b => b.AddDynamicMessageApi());

// 注入 IWxaDynamicMessageService（示例用字段名 dynamicMessage）
// 1) 创建 activity_id：官方无必填参数，可传空对象；unionid 选填
var created = await dynamicMessage.CreateActivityIdAsync(
    new WxaCreateActivityIdRequest(), ct);
WxaException.ThrowIfFailed(created, "/cgi-bin/message/wxopen/activityid/create");
var activityId = created.ActivityId;              // 一次性：分享后即失效
long? expireAt = created.ExpirationTime;           // Unix 秒级时间戳

// 2) 更新动态消息：target_state 0=未进入游戏 / 1=已进入游戏
var updated = await dynamicMessage.UpdateDynamicMessageAsync(
    new WxaUpdateDynamicMessageRequest
    {
        ActivityId = activityId,
        TargetState = 1,
        TemplateInfo = new WxaDynamicMessageTemplateInfo
        {
            ParameterList = [new WxaDynamicMessageParameter { Name = "progress", Value = "已完成 8/10" }],
        },
    }, ct);
WxaException.ThrowIfFailed(updated, "/cgi-bin/message/wxopen/updatablemsg/send");

// 3) 聊天工具动态卡片：同一套 parameter_list，但没有 target_state
var chatTool = await dynamicMessage.SendChatToolMsgAsync(
    new WxaChatToolMsgRequest
    {
        ActivityId = activityId,
        TemplateInfo = new WxaDynamicMessageTemplateInfo
        {
            ParameterList = [new WxaDynamicMessageParameter { Name = "rank", Value = "第 3 名" }],
        },
    }, ct);
WxaException.ThrowIfFailed(chatTool, "/cgi-bin/message/wxopen/chattoolmsg/send");
```

`activity_id` 是**分享凭证标识而非凭据**，但仍不应写入日志（可用于重放更新）；
`access_token` / `appsecret` 只写参数名，示例一律用占位符，禁入日志、遥测或异常消息。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| **本域无独立守卫文件** | 断言落在 [MiniProgramContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramContractGuards.cs)：MP-X5 的 `RoutesOf(…, nameof(IWxaDynamicMessageService))` 3 条逐字数组 + 合计计数注释（`DynamicMessage 3`）、MP-X3 令牌名；路由交集由 MP-X6 兜（`MiniProgramScaffoldContractGuards`） | 若要收拢需新建专属守卫并同批更新本文「事实来源」 |
| 官方逐端点调用频率 / 限流数字 | 接口 XML 未登记 ⇒ **未逐项核验** | 不得写入本文 |
| `activity_id` 有效期的具体时长（秒数） | 只承载 `expiration_time`，官方未在本仓给出常量 | **未逐项核验** |
| `target_state` 是否只有 0 / 1 两值 | 只照录接口 XML 的两值说明 | **未逐项核验**；本仓不建枚举 |
| `parameter_list` 长度上限、`name` 合法集合 | DTO 无约束面，本仓无校验 | **未逐项核验** |
| 私密消息（官方 `openapi/privatemessage` 系列）本仓消费面 | 本域只有凭证入口，该系列**零声明** | 落地须走「加端点 + 同批改计数与守卫」 |
| 第三方代商家调用是否适用本域 | 接口 XML **未表态** | **未逐项核验**；P3-R2 只锁本线不建令牌链 |
| 错误码全集 | 接口 XML 未逐页登记 | **未逐项核验** |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、官方 URL、「一次性」「Unix 秒级」「target_state 游戏态」口径 | [IWxaDynamicMessageService.cs](./IWxaDynamicMessageService.cs) 的 XML 注释 |
| 路由集合（3 条逐字）与全线合计计数 398 | `MiniProgramContractGuards` 的 MP-X5；基线在 `Tests/ContractBaseline.cs` |
| 令牌绑定 / 复用；本线与公众号线路由不得交集 | MP-X3、MP-X2 与 MP-X6（`MiniProgramScaffoldContractGuards`） |
| 本线不建 component/authorizer 令牌链 | P3-R2（`MiniProgramP3RulingsContractGuards`） |
| 请求 / 应答字段名与类型（6 个文件） | [../../../Mud.Wechat.MiniProgram.DataModels/DynamicMessage/](../../../Mud.Wechat.MiniProgram.DataModels/DynamicMessage/)（`SerializerClassName = "DynamicMessage"`） |
| AOT 序列化上下文（`internal`，示例中不得引用） | `../../../Mud.Wechat.MiniProgram.DataModels/Generated/DynamicMessageJsonContext.g.cs` |
| 本线端点总表与装配入口 | [../../README.md](../../README.md) |

## 改动本域需要同批完成

1. **加 / 改端点** → 接口 XML 补齐官方 URL 与约束，**同批**更新 MP-X5 的本域 3 条路由数组与
   合计计数（全线 398 = 392 特性路由 + 6 手工通道，基线 `Tests/ContractBaseline.cs`）。
2. **加 / 改 DTO** → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `DynamicMessage`）→
   跑 `scripts/AddHttpJsonSerializable.ps1` 与 `scripts/GenerateJsonContext.ps1` →
   确认并入 `DynamicMessageJsonContext`；JSON 键**照官方原文**（`activity_id` / `target_state` /
   `parameter_list` 等不得驼峰化）。
3. **改注册面** → `MiniProgramModule.DynamicMessage` + `AddDynamicMessageApi()`
   （[MiniProgramServiceBuilder.cs](../../Extensions/MiniProgramServiceBuilder.cs)）与本域注册委托同步；
   新增接口须仍落 `RegistryGroupName = "DynamicMessage"`。
4. **更新本文**：速览表、时序与一次性语义、两支不同构说明。
5. **不要**把 `activity_id` 复用给多条动态消息；**不要**给 `chattoolmsg/send` 的请求体补
   `target_state`；**不要**把 `activityid` 改成 `activity_id` 或加下划线；
   **不要**引入 component/authorizer 令牌参数（P3-R2）。
6. **本线无回调工程**：消息与事件接收属公众号 XML 通道（`Mud.Wechat.OfficialAccount.Callback`），
   形态由 [MiniProgramScaffoldContractGuards.cs](../../../../../Tests/Mud.Wechat.MiniProgram.Tests/ContractGuards/MiniProgramScaffoldContractGuards.cs)
   （MP-X4：`Mud.Wechat.MiniProgram.Callback.csproj` 不得存在）锁定；
   本域是「主动更新卡片」而非事件消费，**不要**顺手补回调工程。
7. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。
