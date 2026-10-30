# 微信客服 · 会话分配与消息收发

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `KF` 域 · 会话分配与消息收发族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkKfSessionService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfSessionService.cs)（父，承载全部 15 支方法）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfSessionService_Internal.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfSessionService_ThirdParty.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/IWechatWorkKfSessionService_Provider.cs)（均空标记） |
| DTO 源文件 | [DataModels/Kf/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Kf/) 与 [DataModels/Kf/RequestModel/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Kf/RequestModel/)（命名空间 `Mud.Wechat.Work.DataModels.Kf`；消息体集中在 `KfMsgBodies.cs` / `KfMenuMsgBodies.cs`，同步侧在 `KfSync*.cs`） |
| 契约守卫 | [WechatKfContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatKfContractGuards.cs)（**KF1c** 15 支方法 / 5 条路由 / KF2 层级 / KF3 令牌绑定 / **KF1j** 字段名与 `has_more` 数字形态 / **KF1k** 必填面双向锁定（追踪号 P2-QY-B8-KF-REQUIREDFACE-10）/ KF4 DTO 登记）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 状态查询/变更 94669 / 94698 / 96425 · 拉消息 94670 / 94699 / 96426 · 发消息 94677 / 94700 / 96427 · 事件响应消息 95122 / 94910 / 96428 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 14 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 14 段、全仓累计 125 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：KF1c / KF1j / KF1k / KF2 / KF3 / KF4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/README.md)（§5 sync_msg、§6 状态机、§7 发送语义、§8 场景值）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

一条客户会话从**状态流转 → 拉取消息 → 回复消息**的全部 HTTP 面，**15 支方法落在 5 条路由上**：

| 路由 | 方法 | 用途 |
|---|---|---|
| `/cgi-bin/kf/service_state/get` | E1 `GetServiceStateAsync` | 查会话当前接待状态 |
| `/cgi-bin/kf/service_state/trans` | E2 `TransitServiceStateAsync` | 变更状态（可设 0/1/2/3，**状态 4 不可经 API 变更**） |
| `/cgi-bin/kf/sync_msg` | E3 `SyncMessageAsync` | 游标续拉消息与事件 |
| `/cgi-bin/kf/send_msg` | E4–E13（**10 支，每 msgtype 一支**） | 主动发送：text / image / voice / video / file / link / miniprogram / **msgmenu** / location / **ca_link** |
| `/cgi-bin/kf/send_msg_on_event` | E14 `SendTextMsgOnEventAsync` · E15 `SendMenuMsgOnEventAsync` | 用事件 code 回欢迎语 / 结束语（text / msgmenu） |

**「一个 msgtype 一个方法 + 一个请求 DTO、同路由多方法」是本仓的有意形态**（对齐消息推送域先例，因 AOT 按声明类型序列化、不做运行时多态）⇒ 不要把 10 支折叠成一支 `SendMsgAsync(msgtype)`。

形态：官方对**三类应用开放完全一致** ⇒ 收敛父接口 + 三个零差异空标记子接口（KF2）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider`（三类等宽） | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token`（KF3） | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp` | 运行期归属校验 |

> **两个令牌不是一回事**：Query 上的 `access_token` 由 `[Token]` 注入；`sync_msg` 请求体里的 `token` 字段是**回调事件下发的同步令牌**，以请求体字段承载、**不是** `[Token]` 声明（KF1c 锁定）。

第三方 / 代开发须先 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（命名空间 `Mud.Wechat.Work.Abstractions.Authentication`）。

### 2.2 权限与业务前置

- 自建应用须配置到「**微信客服-可调用接口的应用**」中；第三方 / 代开发须具有「**微信客服 → 管理账号、分配会话和收发消息**」权限。
- **只能通过 API 管理企业指定的客服账号**；接待人员应在应用可见范围内；**自 2023-12-01 起不再支持系统应用 secret 调用**（存量企业暂不受影响）。
- **会话定位**：`open_kfid` 定位「哪个客服账号」，`external_userid` 定位「哪个客户」，**两者共同锁定一条会话**；第三方 / 代开发侧 `servicer_userid` / `external_userid` **一律为密文**（`open_userid` / 客服侧 external_userid）。
- **发送条件**：仅当客户处于「**新接入待处理**」或「**由智能助手接待**」状态时才能调 `send_msg`；**客户主动发消息后 48 小时内企业最多下发 5 条**（客户再次发消息可重新计时）。
- **成功 ≠ 送达**：`errcode == 0` 只代表受理；最终结果要看「消息发送失败事件」回调（`origin = 4` 事件体的 `fail_msgid` / `fail_type`）。
- **`msgid` 幂等**：≤32 字节、匹配 `[0-9a-zA-Z_-]*`、**须在客服账号内唯一**；请求指定则响应原样返回，未指定则系统生成。重发同 `msgid` 是宿主幂等去重手段，SDK 不代做。
- **事件响应 code 只能用一次**：`send_msg_on_event` 的 `code` 由事件回调下发；**欢迎语 / 结束会话场景有效期仅 20 秒**；除「用户进入会话」外，响应消息仅支持会话处于**获取该 code 时的状态**。`E2` 应答的 `msg_code` 就是这类 code（会话**初次**变更为 2/3 时是回复语 code、变更为 4 时是结束语 code）。
- **素材先上传后引用**：image / voice / video / file 的 `media_id` 与 link / miniprogram 的 `thumb_media_id` 均由「上传临时素材」（media 域）产生，**本域无上传端点**。
- **`sync_msg` 读取面**：仅能取**最近 3 天内**的消息，**不支持读取通过 `send_msg` 发出的消息**；媒体大小上限 图/语音/视频/文件 = **2MB / 2MB / 10MB / 20MB**，超出官方改返回**文本提示消息**。
- 会话消息内容、客户 id、接待人员 id 均属个人/经营数据：示例用假值，消息正文与名单不落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E15 | ✅ | ✅（id 全为密文） | ✅（同） |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkKfSessionService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **15** | 否 |
| `IWechatWorkInternalKfSessionService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyKfSessionService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderKfSessionService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `Kf`，装配入口 `AddKfApi()`；子接口 `InheritedFrom = nameof(WechatWorkKfSessionService)`。

## 4. 端点

### E1. `GetServiceStateAsync` — 获取客户服务状态

```csharp
[Post("/cgi-bin/kf/service_state/get")]
Task<GetKfServiceStateResponse> GetServiceStateAsync(
    [Body] GetKfServiceStateRequest request, CancellationToken cancellationToken = default);
```

- 请求：`open_kfid`（必填）+ `external_userid`（必填）——**读取语义走 POST**（本域 §14 点名）。
- 响应：`service_state`（`int?`）与 `servicer_userid`（`string?`，**仅状态 3 时有效**；第三方 / 代开发拿到密文 `open_userid`）。

| `service_state` | 语义 |
|---|---|
| 0 | 未处理 |
| 1 | 由智能助手接待 |
| 2 | 待接入池排队中 |
| 3 | 由人工接待 |
| 4 | 已结束 / 未开始（**不可通过 API 变更**） |

### E2. `TransitServiceStateAsync` — 变更客户服务状态

```csharp
[Post("/cgi-bin/kf/service_state/trans")]
Task<TransitKfServiceStateResponse> TransitServiceStateAsync(
    [Body] TransitKfServiceStateRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `open_kfid` | `OpenKfId` | `string?` | **是** | 客服账号 |
| `external_userid` | `ExternalUserId` | `string?` | **是** | 客户 |
| `service_state` | `ServiceState` | `int?` | **是** | 目标状态 **0 / 1 / 2 / 3**（4 不可经 API 设置） |
| `servicer_userid` | `ServicerUserId` | `string?` | 条件 | **`service_state = 3` 时必填**，且该接待人员须处于「正在接待」；第三方 / 代开发填密文 `open_userid` |

响应：`msg_code`（`string?`）——会话**初次**变更为 2/3 时下发**回复语 code**、变更为 4 时下发**结束语 code**；只能用于紧随其后的一次 `send_msg_on_event`（E14/E15）。
**接待人员未在企业微信激活 ⇒ 官方返回 95014。**

### E3. `SyncMessageAsync` — 同步接收消息与事件

```csharp
[Post("/cgi-bin/kf/sync_msg")]
Task<SyncKfMessageResponse> SyncMessageAsync(
    [Body] SyncKfMessageRequest request, CancellationToken cancellationToken = default);
```

**请求参数**（`SyncKfMessageRequest`；**必填面由 KF1k 双向锁定**）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `open_kfid` | `OpenKfId` | `string`（**非可空**） | **是** | 指定拉取哪个客服账号 |
| `token` | `Token` | `string?` | 否 | 回调事件下发的同步令牌，≤128 字节，**10 分钟内有效**；**不填时官方有严格频率限制** ⇒ 正确用法是收到回调后尽快带上它拉取 |
| `cursor` | `Cursor` | `string?` | 否 | ≤64 字节；**首次不填**（不填则从最近 3 天内最早的消息开始），之后填上次返回的 `next_cursor` |
| `limit` | `Limit` | `int?` | 否 | **默认值与最大值都是 1000**；实际返回条数可能更少 |
| `voice_format` | `VoiceFormat` | `int?` | 否 | 语音返回格式：**0 Amr（默认）/ 1 Silk** |

**响应参数**（`SyncKfMessageResponse`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `msg_list[]` | `MessageList` | `List<KfSyncMessage>?` | 本次消息列表 |
| `has_more` | `HasMore` | `int?` | **官方以数字 0/1 承载布尔**（uint32）⇒ 原样 `int?`，非 0 须以 `next_cursor` 续拉 |
| `next_cursor` | `NextCursor` | `string?` | 回填下一次请求的 `cursor`；**官方强烈建议入库**，意外丢失会导致从头拉取而出现延迟消息 |

`KfSyncMessage`（**一份扁平报文承载全部消息类型位，不做运行时多态**；字段名逐字照官方，勿写成 `menu` / `card`）：

| 字段 | 属性 | 说明 |
|---|---|---|
| `msgid` | `MessageId` | 消息 ID（账号内唯一；撤回与失败事件以它定位） |
| `msgtype` | `MessageType` | text / image / voice / video / file / link / miniprogram / msgmenu / location / business_card / event / channels / channels_shop_product / channels_shop_order /（merged_msg 亦在承载位内） |
| `origin` | `Origin` | **3 微信客户发送 / 4 系统推送的事件消息 / 5 接待人员在企业微信客户端发送** |
| `open_kfid` | `OpenKfId` | 客服账号（**`msgtype = event` 时不返回**） |
| `servicer_userid` | `ServicerUserId` | 接待人员（**仅 `origin = 5` 返回**；`event` 不返回；第三方/代开发为密文） |
| `external_userid` | `ExternalUserId` | 客户（**`msgtype = event` 时不返回**） |
| `send_time` | `SendTime` | 发送时间戳（秒级） |
| `text` / `image` / `voice` / `video` / `file` / `link` / `miniprogram` / `msgmenu` / `location` | 同名属性 | 按 `msgtype` 填充其中一个；`text` 同步侧比发送侧多回传 **`menu_id`（客户点击的菜单项 ID）** |
| `business_card` / `channels` / `channels_shop_product` / `channels_shop_order` / `merged_msg` | `BusinessCard` / `Channels` / … | 仅同步侧出现的类型（`merged_msg` 的内层数组官方名是单数 **`item`**；条目 `msg_content` 是内层消息的 JSON 文本，**SDK 不做二次解析**） |
| `event` | `Event` | 事件体：`event_type` / `scene` / `scene_param` / **`welcome_code`** / `status` / `change_type` / `old_servicer_userid` / `new_servicer_userid` / **`fail_msgid`** / **`fail_type`** |

### E4–E13 的共用底座

E4–E13 十支方法**同路由 `POST /cgi-bin/kf/send_msg`、共用同一基类**（`SendKfMsgRequest`），差异只在消息体属性与固定 `msgtype`；响应统一为 `SendKfMsgResponse`（回传 `msgid`）。

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `touser` | `ToUser` | `string?` | **是** | 接收消息的客户 UserID |
| `open_kfid` | `OpenKfId` | `string?` | **是** | 发送消息的客服账号 |
| `msgid` | `MsgId` | `string?` | 否 | ≤32 字节、`[0-9a-zA-Z_-]*`，**账号内唯一**；指定则响应原样返回，否则系统生成 |
| `msgtype` | `MsgType` | `string?` | **是** | 由各子类对应固定取值（下列每支已标出） |

**AOT 形态说明**：「一个 msgtype 一个方法 + 一个请求 DTO」是本仓有意的落地方式（同路由多方法，对齐消息推送域先例）——源生成按**声明类型**序列化，折叠成一支带 `msgtype` 参数的方法会丢掉类型安全。**下面十支只列各自的消息体与专属限制。**

### E4. `SendTextMessageAsync` — 发送文本消息（`msgtype = text`）

```csharp
[Post("/cgi-bin/kf/send_msg")]
Task<SendKfMsgResponse> SendTextMessageAsync(
    [Body] SendKfTextMsgRequest request, CancellationToken cancellationToken = default);
```

- 消息体：`Text`（`KfTextMsgBody`，官方必填）→ **`content`** ≤2048 字节（**超过将被截断**，不是报错）。

### E5. `SendImageMessageAsync` — 发送图片消息（`msgtype = image`）

```csharp
[Post("/cgi-bin/kf/send_msg")]
Task<SendKfMsgResponse> SendImageMessageAsync(
    [Body] SendKfImageMsgRequest request, CancellationToken cancellationToken = default);
```

- 消息体：`Image`（`KfMediaMsgBody`，官方必填）→ **`media_id`**（经「上传临时素材」获取）。

### E6. `SendVoiceMessageAsync` — 发送语音消息（`msgtype = voice`）

```csharp
[Post("/cgi-bin/kf/send_msg")]
Task<SendKfMsgResponse> SendVoiceMessageAsync(
    [Body] SendKfVoiceMsgRequest request, CancellationToken cancellationToken = default);
```

- 消息体：`Voice`（`KfMediaMsgBody`）→ **`media_id`**；**与 image 共用同一载体类型**。

### E7. `SendVideoMessageAsync` — 发送视频消息（`msgtype = video`）

```csharp
[Post("/cgi-bin/kf/send_msg")]
Task<SendKfMsgResponse> SendVideoMessageAsync(
    [Body] SendKfVideoMsgRequest request, CancellationToken cancellationToken = default);
```

- 消息体：`Video`（`KfMediaMsgBody`）→ **`media_id`**。

### E8. `SendFileMessageAsync` — 发送文件消息（`msgtype = file`）

```csharp
[Post("/cgi-bin/kf/send_msg")]
Task<SendKfMsgResponse> SendFileMessageAsync(
    [Body] SendKfFileMsgRequest request, CancellationToken cancellationToken = default);
```

- 消息体：`File`（`KfMediaMsgBody`）→ **`media_id`**。
- **读取侧的媒体上限**（2MB 图 / 2MB 语音 / 10MB 视频 / 20MB 文件）见 E3；超限官方改回**文本提示消息**。

### E9. `SendLinkMessageAsync` — 发送链接消息（`msgtype = link`）

```csharp
[Post("/cgi-bin/kf/send_msg")]
Task<SendKfMsgResponse> SendLinkMessageAsync(
    [Body] SendKfLinkMsgRequest request, CancellationToken cancellationToken = default);
```

- 消息体：`Link`（`KfLinkMsgBody`）＝ **`title`** ≤128 字节（超过截断）+ `desc` ≤512 字节 + **`url`** ≤2048 字节（**须含 http / https 协议头**）+ **`thumb_media_id`**（缩略图临时素材）。

### E10. `SendMiniProgramMessageAsync` — 发送小程序消息（`msgtype = miniprogram`）

```csharp
[Post("/cgi-bin/kf/send_msg")]
Task<SendKfMsgResponse> SendMiniProgramMessageAsync(
    [Body] SendKfMiniProgramMsgRequest request, CancellationToken cancellationToken = default);
```

- 消息体：`MiniProgram`（`KfMiniProgramMsgBody`）＝ **`appid`** + `title` ≤64 字节 + **`thumb_media_id`**（封面建议 **520×416**）+ **`pagepath`**（官方要求**以 `.html` 为后缀**）。

### E11. `SendMenuMessageAsync` — 发送菜单消息（`msgtype = msgmenu`）

```csharp
[Post("/cgi-bin/kf/send_msg")]
Task<SendKfMsgResponse> SendMenuMessageAsync(
    [Body] SendKfMenuMsgRequest request, CancellationToken cancellationToken = default);
```

- 消息体：`Menu`（`KfMenuMsgBody`，属性名与官方 msgtype 同为 **`msgmenu`**，不是 `menu`）＝ `head_content` ≤1024 字节 + `list[]` + `tail_content` ≤1024 字节。
- 菜单项数量：**本端点 ≤50 个**，其中 **`click` + `view` + `miniprogram` 合计 ≤10**（`send_msg_on_event` 侧降到 **≤10**，见 E15）。
- 菜单项 `KfMenuMsgListItem` 按 `type` 四选一：

| `type` | 子体 | 字段 |
|---|---|---|
| `click` 点击回复 | `click` | `id` ≤128 字节（建议只用 a-z A-Z 0-9 `_`，**不建议用 `#`，可能被截断**）+ **`content`** 必填 1~128 字节 |
| `view` 跳转链接 | `view` | **`url`** 必填 1~2048 字节 + `content` 1~1024 字节 |
| `miniprogram` 跳转小程序 | `miniprogram` | **`appid`** 必填 1~32 字节 |
| `text` 文本 | `text` | **`content`** 必填 1~1024 字节 |

### E12. `SendLocationMessageAsync` — 发送位置消息（`msgtype = location`）

```csharp
[Post("/cgi-bin/kf/send_msg")]
Task<SendKfMsgResponse> SendLocationMessageAsync(
    [Body] SendKfLocationMsgRequest request, CancellationToken cancellationToken = default);
```

- 消息体：`Location`（`KfLocationMsgBody`）＝ `name` + `address` + **`latitude`**（`double?`，**范围 -90~90**，负数南纬）+ **`longitude`**（`double?`，**范围 -180~180**，负数西经）。

### E13. `SendCaLinkMessageAsync` — 发送获客链接消息（`msgtype = ca_link`）

```csharp
[Post("/cgi-bin/kf/send_msg")]
Task<SendKfMsgResponse> SendCaLinkMessageAsync(
    [Body] SendKfCaLinkMsgRequest request, CancellationToken cancellationToken = default);
```

- 消息体：`CaLink`（`KfCaLinkMsgBody`）→ **`link_url`**（官方必填，须是**获客助手创建的链接**，形如 `https://work.weixin.qq.com/ca/xxxxxx`）。
- 经此添加的客户**不计入**获客助手「打开链接的客户数」统计。

### E14. `SendTextMsgOnEventAsync` — 发送事件响应文本消息

```csharp
[Post("/cgi-bin/kf/send_msg_on_event")]
Task<SendKfMsgResponse> SendTextMsgOnEventAsync(
    [Body] SendKfEventTextMsgRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `code` | `Code` | `string?` | **是** | 事件回调下发（或 E2 返回的 `msg_code`）；**仅可使用一次**，欢迎语 / 结束会话场景**有效期仅 20 秒**；除「用户进入会话」外，**仅支持会话处于获取该 code 时的状态** |
| `msgid` | `MsgId` | `string?` | 否 | 同 `send_msg` 底座口径（≤32 字节、`[0-9a-zA-Z_-]*`） |
| `msgtype` | `MsgType` | `string?` | **是** | 本子类固定 `text` |
| `text` | `Text` | `KfTextMsgBody` | **是** | `content` |

### E15. `SendMenuMsgOnEventAsync` — 发送事件响应菜单消息

```csharp
[Post("/cgi-bin/kf/send_msg_on_event")]
Task<SendKfMsgResponse> SendMenuMsgOnEventAsync(
    [Body] SendKfEventMenuMsgRequest request, CancellationToken cancellationToken = default);
```

- 底座字段同 E14，`msgtype` 固定为 **`msgmenu`**，消息体为 `Menu`（`KfMenuMsgBody`）。
- **菜单项总数 ≤10**（比 E11 的 50 更严，且 `click`/`view`/`miniprogram` 仍受合计 ≤10 约束）。

**调用示例**（收到回调后 `sync_msg` 续拉 → 判状态 → 用事件 code 发欢迎语 → 普通 `send_msg` 回复）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Kf;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "kf-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddKfApi());

var session = provider.GetRequiredService<IWechatWorkInternalKfSessionService>();
string openKfId = "wkOpenKfId假值";

// ① 同步消息：token 来自回调事件（10 分钟内有效）；首次不填 cursor 即从 3 天内最早消息开始
//    next_cursor 官方强烈建议入库，丢了会从头重拉
string? cursor = null;
bool hasMore = true;
while (hasMore)
{
    var sync = await session.SyncMessageAsync(
        new SyncKfMessageRequest { OpenKfId = openKfId, Cursor = cursor, Limit = 1000 },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(sync, "/cgi-bin/kf/sync_msg");

    // 官方明令：不能用「msg_list 是否为空」判停——可能 has_more=1 而本页为空
    hasMore = sync.HasMore != 0;
    cursor = sync.NextCursor ?? cursor;

    foreach (KfSyncMessage msg in sync.MessageList ?? new List<KfSyncMessage>())
    {
        // origin：3 客户 / 4 系统事件 / 5 接待人员；msgtype=event 时 open_kfid / external_userid 不返回
        if (msg.MessageType == "event")
        {
            KfSyncEventMsgBody? evt = msg.Event;
            if (evt?.WelcomeCode is { Length: > 0 } welcomeCode)
            {
                // 欢迎语 code：仅可使用一次、有效期仅 20 秒 ⇒ 立即调用，不排队不重试
                var hello = await session.SendTextMsgOnEventAsync(
                    new SendKfEventTextMsgRequest
                    {
                        Code = welcomeCode,
                        MsgType = "text",
                        Text = new KfTextMsgBody { Content = "您好，这里是示例欢迎语" },
                    },
                    cancellationToken);
                WechatWorkException.ThrowIfFailed(hello, "/cgi-bin/kf/send_msg_on_event");
            }
            // 发送失败事件：以 fail_msgid 定位是哪条消息没送达（errcode=0 只代表受理）
            if (evt?.FailMessageId is { Length: > 0 } failMsgId)
                logger.LogWarning("消息发送失败事件：msgid {MsgId}，fail_type {FailType}", failMsgId, evt.FailType);
            continue;
        }
        logger.LogInformation("收到消息 msgid {MsgId}，msgtype {MsgType}，origin {Origin}",
            msg.MessageId, msg.MessageType, msg.Origin);
    }
}

// ② 查状态，再决定能否主动发送（仅「新接入待处理」或「由智能助手接待」可发）
var state = await session.GetServiceStateAsync(
    new GetKfServiceStateRequest { OpenKfId = openKfId, ExternalUserId = "wmCustomer假值" }, cancellationToken);
WechatWorkException.ThrowIfFailed(state, "/cgi-bin/kf/service_state/get");

if (state.ServiceState is 0 or 1)
{
    var sent = await session.SendTextMessageAsync(
        new SendKfTextMsgRequest
        {
            ToUser = "wmCustomer假值",
            OpenKfId = openKfId,
            MsgId = "conv-2026-10-30-0001",     // ≤32 字节、账号内唯一 ⇒ 宿主用它做幂等
            MsgType = "text",
            Text = new KfTextMsgBody { Content = "示例回复文本" },
        },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(sent, "/cgi-bin/kf/send_msg");
    // errcode=0 不等于已送达：最终结果看「消息发送失败事件」回调
    logger.LogInformation("已受理发送，msgid {MsgId}", sent.MsgId);
}

// ③ 转人工：service_state=3 时 servicer_userid 必填且须处于「正在接待」；未激活返回 95014
var transit = await session.TransitServiceStateAsync(
    new TransitKfServiceStateRequest
    {
        OpenKfId = openKfId,
        ExternalUserId = "wmCustomer假值",
        ServiceState = 3,
        ServicerUserId = "zhangsan",     // 假值；第三方/代开发填密文 open_userid
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(transit, "/cgi-bin/kf/service_state/trans");
// 初次变更为 2/3 返回回复语 code、变更为 4 返回结束语 code；状态 4 本身不可经 API 设置
string? msgCode = transit.MsgCode;
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `access_token` vs `sync_msg` 的 `token` | 前者 Query 注入的应用令牌，后者是**回调下发的同步令牌**（≤128 字节、10 分钟有效），走请求体字段（KF1c 锁定） | 把 `token` 当 `[Token]` 声明，或漏传后裸轮询撞频控 |
| `has_more`（数字）vs 布尔 | 官方 uint32 **0/1** ⇒ 本仓 `int?`；**不可用「`msg_list` 是否为空」判停**（可能 `has_more = 1` 而本页为空） | 改成 `bool?` 即反序列化失败；按空列表停 → 丢消息 |
| `cursor` 缺失的代价 | 官方强烈建议**入库** `next_cursor`；丢失会**从头拉取**，表现为延迟消息重复 | 每轮重启都从 3 天窗口起点重拉 |
| 48 小时 5 条 | 客户发消息后 48 小时内企业**最多下发 5 条**（客户再发可重新计时），且仅「新接入待处理 / 智能助手接待」状态可发 | 超限被拒并影响客户体验 |
| `errcode == 0` | 只代表**受理**；真实结果看失败事件回调的 `fail_msgid` / `fail_type` | 记账「已送达」 |
| `msgid` | 账号内唯一、≤32 字节、`[0-9a-zA-Z_-]*`；指定则原样返回、未指定则系统生成 ⇒ **幂等靠它** | 每次重发换 msgid → 重复送达 |
| `send_msg` vs `send_msg_on_event` | 后者必须有事件 `code`，**欢迎语 / 结束语场景 code 仅 20 秒有效且只能用一次**；除「进入会话」外只接受**获取该 code 时的会话状态** | 排队重试同一个 code → 必然失败 |
| `msg_code` vs `welcome_code` | 前者来自 **E2 状态变更应答**（初次转 2/3 回复语 code、转 4 结束语 code），后者来自 **`sync_msg` 的 event 体**（进入会话等）；两者都是 `send_msg_on_event` 的 `code` 入参 | 以为只有事件才给 code |
| 菜单项上限 | `send_msg` 的 `msgmenu.list` **≤50**（click+view+miniprogram 合计 ≤10）；`send_msg_on_event` **≤10** | 用同一上限 |
| `msgmenu` 的类型值 | 官方 msgtype 是 **`msgmenu`**（属性名也是 `msgmenu`），不是 `menu` | 按直觉写 `menu` → 字段丢失 |
| `ca_link` | msgtype 是 `ca_link`，`link_url` 必须是**获客助手创建的链接**；经此添加**不计入**获客助手「打开链接的客户数」统计 | 与 E9 的 `link` 混用 |
| `miniprogram.pagepath` | 官方要求**以 `.html` 为后缀**；`thumb_media_id` 必填（封面建议 520×416） | 路径不带 .html → 发送失败 |
| 读取面限制 | `sync_msg` **只给最近 3 天**，且**读不到通过 `send_msg` 发出的消息**；媒体超 2/2/10/20 MB 时官方返回**文本提示消息**而非原消息 | 以为能拉全量历史或自发消息 |
| `merged_msg` 内层 | 官方数组字段名是单数 **`item`**；条目 `msg_content` 是**内层消息的 JSON 文本，SDK 不做二次解析** | 期待强类型聊天记录 |
| 字段名照抄 | `msgmenu` / `business_card` / `channels_shop_product` / `channels_shop_order` / `merged_msg` / `open_kfid` / `send_time` 均逐字保留（KF1j） | 写成驼峰或缩写 → 反序列化取不到 |
| `origin` 与 `servicer_userid` | `servicer_userid` **仅 `origin = 5` 返回**；`msgtype = event` 时 `open_kfid` / `external_userid` **都不返回** | 事件条目按必有字段解析 |
| 状态 4 | **不可通过 API 变更**；`service_state = 3` 时 `servicer_userid` 必填且须「正在接待」，未激活 → **95014** | 试图用 API 结束会话 |
| 十支 vs 一条路由 | `/cgi-bin/kf/send_msg` 上挂 10 个方法（每 msgtype 一个 DTO）是**有意的 AOT 形态**，不是重复声明 | 折叠成一支泛型方法 → 破坏契约与守卫 |
| 动词 | 本族 5 条路由**全部 POST**（含 `service_state/get`、`sync_msg`） | 改 GET → 打坏 KF1c 与 G5 |

SDK 侧通用约束：**不自动判错**、**不做本地必填校验**（`open_kfid` 之外均为可空字段，照常发出）、**不做 48 小时/5 条窗口记账**、**不代托管 `next_cursor`**、**不做 msgtype 校验**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 受理成功 | E3 成功仍要看 `has_more`；E4–E15 成功**不等于送达**，失败另有事件回调 |
| **95014** | 官方业务码（E2 注释留档） | 目标接待人员未在企业微信激活 | 转人工接待（`service_state = 3`）时 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`msgid` 重复、`token` / `code` 过期或已用、发送窗口超限、消息体必填字段缺失等业务码**接口注释未逐条留档**；`fail_type` 的取值集亦未摘录 ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| `fail_type` 取值集 | 注释只说「取值以官方参数表为准」⇒ **未逐项核验** | 官方 94670 事件参数表 |
| `event_type` 取值集 | 同上（`KfSyncEventMsgBody.EventType` 注释未列举） ⇒ 未逐项核验 | 官方 94670 / 94671 事件列表 |
| `change_type` 与 `status` 的语义 | 注释标「会话状态变更类型 / 状态值（变更类事件携带）」，未给枚举 ⇒ 未核验 | 官方事件文档 |
| 事件响应消息 code 的完整清单 | `welcome_code` / `msg_code` 之外还有哪些场景 ⇒ 未逐项核验 | 官方 95122 / 94910 |
| 业务错误码全集 | 仅留档 95014 ⇒ 未逐项核验 | 官方各页 + 全局错误码页 |
| 端点频率数值 | 注释仅写「不填 `token` 时有严格的频率限制」，**未给数值** ⇒ 未逐项核验，勿凭记忆补 | 官方访问频率附录 90312 |
| `send_msg` 的 5 条窗口是否含菜单消息 | 注释按会话口径写「最多下发 5 条」，未逐类型区分 ⇒ 未核验 | 官方 94677 |
| `voice_format` 与播放端兼容 | 只给 0 Amr / 1 Silk，未给格式差异 ⇒ 未核验 | 官方 94670 |
| 撤回消息面 | `msgid` 注释提到「撤回与失败事件均以该值定位」，但**本域未见撤回端点声明** ⇒ 未核验其归属 | 官方消息撤回页（未在本文断言） |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/KF/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/KF/README.md)（§5 sync_msg 续拉协议、§6 状态机与 `msg_code`、§7 发送语义、§8 场景值、§14 POST 与频率汇总）
- 同域：[KfAccount.md](./KfAccount.md)（`open_kfid` 与 `scene` 来源）· [KfServicer.md](./KfServicer.md)（「正在接待」状态由人员侧决定）· [KfCustomer.md](./KfCustomer.md)（`external_userid` 维度资料）· [KfStatistics.md](./KfStatistics.md)（会话结果的统计面）· [KfKnowledge.md](./KfKnowledge.md)（智能助手接待所用的问答库）
- 跨包：`kf_msg_or_event` **回调事件**（含 `token` 的签发）属 **Callback 包**，本域只声明 HTTP 端点；`welcome_code` 的取得路径即在回调载荷里
- 素材邻域：`media_id` / `thumb_media_id` 全部来自「上传临时素材」（media 域，三天有效）
- 官方页：[获取客户服务状态（94669）](https://developer.work.weixin.qq.com/document/path/94669) · [同步消息（94670）](https://developer.work.weixin.qq.com/document/path/94670) · [发送消息（94677）](https://developer.work.weixin.qq.com/document/path/94677) · [发送欢迎语等事件响应消息（95122）](https://developer.work.weixin.qq.com/document/path/95122)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
