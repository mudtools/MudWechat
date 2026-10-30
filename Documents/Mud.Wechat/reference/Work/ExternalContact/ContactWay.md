# 客户联系 · 联系我与加入群聊

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `ExternalContact` 域 · 「联系我」与入群方式族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkExternalContactContactWayService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/ContactWay/IWechatWorkExternalContactContactWayService.cs)（父，承载全部 10 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/ContactWay/IWechatWorkExternalContactContactWayService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/ContactWay/IWechatWorkExternalContactContactWayService_Provider.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/ContactWay/IWechatWorkExternalContactContactWayService_ThirdParty.cs)（均空标记） |
| DTO 源文件 | [DataModels/ExternalContact/ContactWay/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ExternalContact/ContactWay/)（含 `RequestModel/`；`SerializerClassName = ContactWay`） |
| 契约守卫 | [WechatExternalContactContactWayContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactContactWayContractGuards.cs)（CW1 路由表 / CW2 层级与三空标记 / CW3 令牌绑定 / CW4 JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 「联系我」92228 / 95724 / 96348 · 临时会话同页 · 入群方式 92229 / 99546 / 99547 |
| 状态 | `草稿` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 待编译验证 |
| 证据基础 | 内部：CW1~CW4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.4、§4 陷阱 4/5/10/11）· 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

两个「**入口配置**」子系统，共用一套 `config_id` 生命周期：

| 能力块 | 端点 |
|---|---|
| 「联系我」方式（客户加成员的入口） | E1 配置 · E2 读详情 · E3 列 config_id · E4 更新 · E5 删除 |
| 临时会话 | E6 `close_temp_chat`（结束成员×客户的临时会话） |
| 「加入群聊」方式（客户入群的入口） | E7 配置 · E8 读详情 · E9 更新 · E10 删除 |

`config_id` 的语义是**长期凭据式标识**：E1/E7 创建后返回，E2/E4/E5 与 E8/E9/E10 都以它定位；**删除后该 config_id 作废、二维码立即失效**，要恢复只能重新创建（不是「重新启用」）。

形态：官方对三类应用开放完全一致的 10 支端点 ⇒ 全部收敛父接口，三支子接口为空标记（CW2 断言）。

**10 支全部 POST**——包括 E2 读详情、E3 列表、E8 读入群配置这三「读取即 POST」端点（域 README §4 陷阱 10 明确点名）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.ThirdParty` / `WechatAppType.Provider` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（CW3 对四接口统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp` | 运行期归属校验 |

### 2.2 权限与业务前置

| 面 | 权限要求 |
|---|---|
| E1–E6「联系我」/临时会话 | 自建须配置到「客户联系 - 可调用接口的应用」；第三方/代开发须具「**配置『联系我』二维码**」权限（营销获客应用还须把客户建联方式配置为「联系我」二维码） |
| E7–E10「加入群聊」 | 第三方/代开发须具「企业客户权限 → 客户群-配置『加入群聊』二维码」权限，**且已购买「加入群聊」高级接口** |

共同的归属与范围约束：传入的成员与部门须在**应用可见范围**内；**应用只能获取与管理由本应用创建的**「联系我」方式与「加入群聊」组件。

容量与行业限制（接口注释留档）：

- **每个企业 API 配置「联系我」上限 50 万个**（**临时会话不计入**）。
- **每个联系方式最多 100 个使用成员**（`user` ≤100，仅多人类型可配 `party`）。
- **临时会话二维码仅医疗行业企业可创建**，且仅支持单人、**每日最多 10 万个**。
- E3 列表：**不含临时会话模式**，且**仅可查询 2021-07-10 之后创建的记录**；`start_time` 默认 90 天前、`end_time` 默认当前；`limit` 默认 100、**最大 1000**。
- E7/E9：`chat_id_list` 官方必填、**最多 5 个群**；`auto_create_room` 默认 `1`（群满自动建群），`room_base_name` 最长 40 个 UTF-8 字符。
- `state`：「联系我」不超过 30 个字符；入群方式不超过 30 个 UTF-8 字符（配置后，经该方式入群的成员在「获取客户群详情」的 `member_list` 里会额外返回该 `state`）。
- `mark_source` **仅对「营销获客」应用生效**，且**只能由创建该配置的应用更新**。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E10 | ✅ | ✅ | ✅ |

三类一致 ⇒ 父接口承载全部端点，子接口空标记（注入哪支只决定令牌归属与作用域）。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkExternalContactContactWayService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **10** | 否 |
| `IWechatWorkInternalExternalContactContactWayService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyExternalContactContactWayService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderExternalContactContactWayService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `ExternalContact`，装配入口 `AddExternalContactApi()`；子接口 `InheritedFrom = nameof(WechatWorkExternalContactContactWayService)`。

## 4. 端点

### E1. `AddContactWayAsync` — 配置客户联系「联系我」方式

```csharp
Task<AddContactWayResponse> AddContactWayAsync(
    [Body] AddContactWayRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/externalcontact/add_contact_way` |
| 令牌 | Query `access_token` |

**请求参数**（`AddContactWayRequest`）——**条件必填矩阵**：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `type` | `Type` | `int?` | **是** | `1` 单人 / `2` 多人 |
| `scene` | `Scene` | `int?` | **是** | `1` 在小程序中联系 / `2` 通过二维码联系 |
| `style` | `Style` | `int?` | 否 | **仅 `scene = 1` 生效**（小程序里的按钮样式） |
| `remark` | `Remark` | `string?` | 否 | 最多 30 个字符，**超长报错** |
| `skip_verify` | `SkipVerify` | `bool?` | 否 | 添加时是否免验证，默认 `true` |
| `state` | `State` | `string?` | 否 | 自定义渠道标识，≤30 字符（客户来源分析靠它） |
| `user` | `User` | `List<string>?` | **条件** | `type = 1` 时**必填且仅 1 个**；`type = 2` 时可选、**≤100** |
| `party` | `Party` | `List<long>?` | 否 | **仅 `type = 2` 有效** |
| `is_temp` | `IsTemp` | `bool?` | 否 | 默认 `false`；`true` 时仅支持单人，**仅医疗行业企业可创建** |
| `expires_in` | `ExpiresIn` | `int?` | 否 | 临时二维码有效期（秒），默认 7 天、最多 14 天；仅临时会话生效 |
| `chat_expires_in` | `ChatExpiresIn` | `int?` | 否 | 临时会话有效期（秒），默认添加客户后 24 小时、最多 14 天 |
| `unionid` | `Unionid` | `string?` | 否 | 指定后仅该客户可发起临时会话；仅临时会话模式生效 |
| `mark_source` | `MarkSource` | `bool?` | 否 | 仅「营销获客」应用生效 |
| `conclusions` | `Conclusions` | `ContactWayConclusion?` | 否 | 结束语，**仅临时会话模式生效**（结构见下） |

**结束语 `conclusions`** 四种消息类型（同一对象内并列，按需填一种）：

| 字段 | 属性 | 口径 |
|---|---|---|
| `text.content` | `Text.Content` | 最长 **4000 字节** |
| `image.media_id` | `Image.MediaId` | **构造时只填 media_id**（经上传临时素材取得）；**读取时官方返回 `pic_url`** |
| `link.title` / `picurl` / `desc` / `url` | `Link.*` | 标题 ≤128 字节、描述 ≤512 字节；**描述键是 `desc`、封面键是 `picurl`** |
| `miniprogram.title` / `pic_media_id` / `appid` / `page` | `MiniProgram.*` | 标题 ≤64 字节；封面建议 520×416；**`appid` 必须关联到企业** |

**响应参数**：`config_id`（`string?`）、`qr_code`（`string?`，**仅 `scene = 2` 返回**）。

**调用示例**（本族主示例：创建多人二维码「联系我」→ 读详情 → 更新覆盖）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.ExternalContact.ContactWay;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "crm-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddExternalContactApi());

var contactWay = provider.GetRequiredService<IWechatWorkInternalExternalContactContactWayService>();

// ① 创建多人「联系我」二维码：type/scene 必填，user ≤100
var created = await contactWay.AddContactWayAsync(
    new AddContactWayRequest
    {
        Type = 2,                                  // 2 多人
        Scene = 2,                                 // 2 二维码（scene=1 才有 style）
        Remark = "线下门店A",                       // ≤30 字符，超长报错
        State = "store-a",                         // ≤30 字符，用于渠道归因
        SkipVerify = true,
        User = new List<string> { "zhangsan", "lisi" },   // 假值，须在可见范围内
        Party = new List<long> { 2 },                     // 仅 type=2 有效
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(created, "/cgi-bin/externalcontact/add_contact_way");
string configId = created.ConfigId ?? throw new InvalidOperationException("官方未返回 config_id");
logger.LogInformation("已创建「联系我」，二维码链接是否为空：{HasQr}", string.IsNullOrEmpty(created.QrCode));

// ② 读取详情（读取即 POST，config_id 走 body）
var detail = await contactWay.GetContactWayAsync(
    new GetContactWayRequest { ConfigId = configId }, cancellationToken);
WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/externalcontact/get_contact_way");
logger.LogInformation("配置 type={Type} scene={Scene} 成员数={UserCount}",
    detail.ContactWay?.Type, detail.ContactWay?.Scene, detail.ContactWay?.User?.Count ?? 0);

// ③ 更新：传入字段即覆盖（user 是整体覆盖，不是追加）
var updated = await contactWay.UpdateContactWayAsync(
    new UpdateContactWayRequest
    {
        ConfigId = configId,
        User = new List<string> { "zhangsan" },    // 覆盖为单人（type 仍是 2 时合法）
        Remark = "线下门店A-调整",
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(updated, "/cgi-bin/externalcontact/update_contact_way");
```

### E2. `GetContactWayAsync` — 获取「联系我」方式详情

```csharp
Task<GetContactWayResponse> GetContactWayAsync(
    [Body] GetContactWayRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/get_contact_way`；请求 `config_id`（**必填**）。
- 响应 `contact_way`（`ContactWayConfig`）：`config_id` / `type` / `scene` / `style` / `remark` / `skip_verify` / `state` / `qr_code`（仅 `scene = 2`）/ `user[]` / `party[]` / `is_temp` / `expires_in` / `chat_expires_in` / `unionid` / `mark_source` / `conclusions`。
- 只能取本应用创建的配置。

### E3. `GetContactWayListAsync` — 获取「联系我」列表

```csharp
Task<ListContactWayResponse> GetContactWayListAsync(
    [Body] ListContactWayRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `start_time` | `StartTime` | `long?` | 否 | 创建时间起，**默认 90 天前** |
| `end_time` | `EndTime` | `long?` | 否 | 创建时间止，**默认当前时间** |
| `cursor` | `Cursor` | `string?` | 否 | 首次不填 |
| `limit` | `Limit` | `int?` | 否 | **默认 100、最大 1000** |

**响应参数**：`contact_way[]`（元素**仅含 `config_id`**）、`next_cursor`（**无更多数据时不返回该字段**）。

⚠️ 这是「**列表只给 id、详情要逐条取**」的两段式：列表**不含临时会话**、且**只覆盖 2021-07-10 之后创建**的记录，早于该日期的配置在此接口里查不到。

### E4. `UpdateContactWayAsync` — 更新「联系我」方式

```csharp
Task<WechatWorkResponse> UpdateContactWayAsync(
    [Body] UpdateContactWayRequest request, CancellationToken cancellationToken = default);
```

- 请求：`config_id`（**必填**）+ 可覆盖字段 `remark` / `skip_verify` / `style` / `state` / `user` / `party` / `expires_in` / `chat_expires_in` / `unionid` / `mark_source` / `conclusions`。
- **传入即覆盖原值**（`user`、`party`、`conclusions` 都是整体覆盖，不是追加）。
- **已失效的临时会话不允许编辑**；`user` / `party` 须在可见范围内；`mark_source` 只能由创建该配置的应用更新。

### E5. `DeleteContactWayAsync` — 删除「联系我」方式

```csharp
Task<WechatWorkResponse> DeleteContactWayAsync(
    [Body] DeleteContactWayRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/del_contact_way`；请求 `config_id`（必填）。
- **删除后该 config_id 作废、二维码立即失效**，不可恢复，只能重新 E1。

### E6. `CloseTempChatAsync` — 结束临时会话

```csharp
Task<WechatWorkResponse> CloseTempChatAsync(
    [Body] CloseTempChatRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/close_temp_chat`；请求 `userid` + `external_userid`（**均必填**）。
- **成员与客户之间必须存在有效的临时会话**才可结束；**通过其他方式添加的外部联系人无法经本接口关闭会话**。

### E7. `AddGroupChatJoinWayAsync` — 配置客户群进群方式

```csharp
Task<AddGroupChatJoinWayResponse> AddGroupChatJoinWayAsync(
    [Body] AddGroupChatJoinWayRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `scene` | `Scene` | `int?` | **是** | `1` 群的小程序插件 / `2` 群的二维码插件 |
| `chat_id_list` | `ChatIdList` | `List<string>?` | **是** | **最多 5 个群** |
| `remark` | `Remark` | `string?` | 否 | ≤30 字符，**超长截断**（与 E1 的「超长报错」不同） |
| `auto_create_room` | `AutoCreateRoom` | `int?` | 否 | `0` 否 / `1` 是，**默认 1** |
| `room_base_name` | `RoomBaseName` | `string?` | 否 | 自动建群的群名前缀，最长 40 个 UTF-8 字符 |
| `room_base_id` | `RoomBaseId` | `long?` | 否 | 自动建群的起始序号 |
| `state` | `State` | `string?` | 否 | ≤30 个 UTF-8 字符；会回显在客户群详情的 `member_list` |
| `mark_source` | `MarkSource` | `bool?` | 否 | 默认 `true`；仅营销获客应用生效 |

**响应参数**：`config_id`（`string?`）——后续 E8/E9/E10 的定位键。

**调用示例**（「加入群聊」：建群二维码 → 读回链接）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.ExternalContact.ContactWay;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "crm-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddExternalContactApi());

var joinWayApi = provider.GetRequiredService<IWechatWorkInternalExternalContactContactWayService>();

var joined = await joinWayApi.AddGroupChatJoinWayAsync(
    new AddGroupChatJoinWayRequest
    {
        Scene = 2,                                        // 2 群的二维码插件
        ChatIdList = new List<string> { "<chat_id>" },    // 必填，最多 5 个
        AutoCreateRoom = 1,
        RoomBaseName = "会员服务群",                        // ≤40 UTF-8 字符
        RoomBaseId = 1,
        State = "vip-entry",
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(joined, "/cgi-bin/externalcontact/groupchat/add_join_way");

var wayDetail = await joinWayApi.GetGroupChatJoinWayAsync(
    new GetGroupChatJoinWayRequest { ConfigId = joined.ConfigId ?? string.Empty },
    cancellationToken);
WechatWorkException.ThrowIfFailed(wayDetail, "/cgi-bin/externalcontact/groupchat/get_join_way");
// qr_code 这一个字段在两种 scene 下含义不同：scene=2 是二维码链接、scene=1 是小程序插件链接。
logger.LogInformation("入群配置 scene={Scene}，群数 {ChatCount}",
    wayDetail.JoinWay?.Scene, wayDetail.JoinWay?.ChatIdList?.Count ?? 0);
```

### E8. `GetGroupChatJoinWayAsync` — 获取进群方式配置

```csharp
Task<GetGroupChatJoinWayResponse> GetGroupChatJoinWayAsync(
    [Body] GetGroupChatJoinWayRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/groupchat/get_join_way`；请求 `config_id`（必填）。
- 响应 `join_way`（`GroupChatJoinWayConfig`）：`config_id` / `scene` / `remark` / `auto_create_room` / `room_base_name` / `room_base_id` / `chat_id_list[]` / `qr_code` / `state` / `mark_source`。
- **`qr_code` 一个字段两种含义**：`scene = 2` 是二维码链接、`scene = 1` 是小程序插件链接。

### E9. `UpdateGroupChatJoinWayAsync` — 更新进群方式配置

```csharp
Task<WechatWorkResponse> UpdateGroupChatJoinWayAsync(
    [Body] UpdateGroupChatJoinWayRequest request, CancellationToken cancellationToken = default);
```

- 请求：`config_id`（必填）、`scene`（**必填**）、`chat_id_list`（**必填，≤5**）+ 其余可选字段。
- **覆盖式更新**（传入字段整体覆盖原配置）；`mark_source` 只能由创建此二维码的应用更新。

### E10. `DeleteGroupChatJoinWayAsync` — 删除进群方式配置

```csharp
Task<WechatWorkResponse> DeleteGroupChatJoinWayAsync(
    [Body] DeleteGroupChatJoinWayRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/groupchat/del_join_way`；请求 `config_id`（必填）。

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| E1/E7 的 `config_id` vs E4/E9 的更新语义 | config_id 长期有效；**update 是覆盖、不是追加**（`user`/`party`/`chat_id_list`/`conclusions` 全量替换） | 传单个成员当「新增」→ 其它成员被摘掉 |
| `remark` 越界行为 | E1/E4「联系我」侧**超长报错**；E7/E9「加入群聊」侧**超长截断** | 用同一套长度校验/容错策略 → 一侧静默丢字、一侧突然失败 |
| `qr_code` 的双含义 | 同一字段在 `scene = 2` 是二维码、`scene = 1` 是小程序插件链接 | 当成固定二维码 URL 展示 |
| E3 列表 vs E2 详情 | 列表元素**只有 `config_id`**；且**不含临时会话**、**只有 2021-07-10 之后**创建的记录 | 以为列表即全量 → 漏早期与临时会话配置 |
| `state` 的用途 | 「联系我」的 `state` 用于渠道归因；入群方式的 `state` 会回显在**客户群详情 `member_list`** | 只在客户来源里找入群 state |
| `type` 与 `user`/`party` | `type = 1` 时 `user` 必填且**只能 1 个**；`party` **仅 `type = 2` 有效** | 单人配 party → 被忽略或报错 |
| `is_temp` | **仅医疗行业企业可创建**，仅单人；且不占用 50 万配额、不出现在 E3 列表 | 非医疗企业用它做常规入口 |
| 读取的动词 | E2/E3/E8 全是 POST（参数在 body） | 改 GET → 打坏 CW1 与 G5 |
| 删除路由拼写 | `del_contact_way` / `groupchat/del_join_way` 用缩写 `del_` | 与 ProductAlbum 的 `delete_` 混用 |
| 归属边界 | 应用只能读写**本应用创建**的配置；`mark_source` 只能由创建方更新 | 多应用共管同一入口 → 静默失败 |
| 结束语图片字段 | 构造填 `image.media_id`，读取返回 `pic_url` | 读回时期待 media_id → 空 |
| 「加入群聊」高级接口 | 第三方/代开发须**已购买**才可用 E7–E10 | 未购买时按权限问题排查方向错 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）、**不做配额与行业前置校验**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E1/E7 成功还要看 `config_id` 是否返回 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法/过期/关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，装配缺陷 |

本族接口注释**未留档业务错误码**（config_id 非法、非本应用创建、超配额、成员不可见、临时会话已失效等场景只有文字口径）⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 注释未列码值 ⇒ 未逐项核验 | 官方 92228 / 92229 |
| 各端点频率 | 注释只给「每日 10 万个临时会话」这类总量，未给 QPS ⇒ 未逐项核验 | 官方 + 全局频率附录 |
| `style` 的取值集合 | 注释只说「小程序中联系的按钮样式」，未列可选值 | 官方 92228 |
| 临时会话「每日 10 万个」的计数口径 | 按企业还是按应用、是否含失败请求，未留档 | 官方 92228 |
| `skip_verify` 默认值差异 | 注释写「默认为 true」，未说明显式传 `false` 与不传的报文差异 | 官方 92228 |
| E1 的 `conclusions` 是否对非临时会话生效 | 注释明确「仅临时会话模式生效」，但 E4 的 conclusions 亦同限定 ⇒ 未逐项核验 | 官方 92228 |
| 「加入群聊」高级接口的购买状态查询 | 本域无对应端点 | 官方服务商侧说明 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.4 端点表、§4 陷阱 4「config_id 生命周期」、陷阱 5「联系我条件必填」、陷阱 10「读取即 POST」、陷阱 11「删除路由拼写」）
- 同域：[Customer.md](./Customer.md)（客户详情的 `state` 就是这个渠道标识的落点）· [GroupChat.md](./GroupChat.md)（入群方式影响的群详情与 `member_list.state`）· [FollowUser.md](./FollowUser.md)（成员须已配置客户联系功能）
- 官方页：[配置客户联系「联系我」方式（自建 92228）](https://developer.work.weixin.qq.com/document/path/92228) · [配置客户群进群方式（92229）](https://developer.work.weixin.qq.com/document/path/92229)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
