# 客户联系 · 客户管理

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `ExternalContact` 域 · 客户管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkExternalContactCustomerService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/Customer/IWechatWorkExternalContactCustomerService.cs)（父，10 支公共面）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/Customer/IWechatWorkExternalContactCustomerService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/Customer/IWechatWorkExternalContactCustomerService_Provider.cs)（空标记）· [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/Customer/IWechatWorkExternalContactCustomerService_ThirdParty.cs)（1 支差异端点） |
| DTO 源文件 | [DataModels/ExternalContact/Customer/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ExternalContact/Customer/)（含 `RequestModel/`；`SerializerClassName = Customer`） |
| 契约守卫 | [WechatExternalContactCustomerContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactCustomerContractGuards.cs)（CU1 路由表 11 条 / CU2 层级与第三方 `HaveCount(1)` / CU3 令牌绑定 / CU4 JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 列表 92113/92264/96314 · 详情 92114/92265/96315 · 批量 92994/93010/96316 · 备注 92115/92694/96317 · 规则组 94883/99540/99543 · external_userid 转换 95195（仅第三方） |
| 状态 | `草稿` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 待编译验证 |
| 证据基础 | 内部：CU1~CU4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.1 速览、§4 陷阱 1/2/3/10/13）· 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

客户联系的主数据面：**读客户、改备注、管规则组**。

| 能力块 | 端点 |
|---|---|
| 读客户 | E1 某成员的客户列表 · E2 客户详情（基本信息 + 全部跟进人）· E3 批量详情（多成员、分页） |
| 写客户 | E4 修改客户备注信息 |
| 客户联系规则组（6 支） | E5 列表 · E6 详情 · E7 管理范围 · E8 创建 · E9 编辑 · E10 删除 |
| 身份转换（仅第三方） | E11 代开发 `external_userid` → 第三方 `external_userid` |

E1–E10 是三类应用一致的公共面（收敛父接口，自建与代开发子接口为空标记）；E11 只在第三方子接口上。

两条先立住的结构认知：

1. **客户信息分两层**：`external_contact`（客户本身）与 `follow_user[]` / `follow_info`（**每个企业成员对这个客户的跟进关系**）。`add_way`、`state`、`remark_mobiles`、`oper_userid` 都挂在**跟进人层**，不在客户基本信息层——读「这个客户是从哪个渠道加来的」必须走跟进人结构。
2. **本域不做 unionid 转换**。`/cgi-bin/idconvert/unionid_to_external_userid` 与 `/cgi-bin/idconvert/batch/external_userid_to_pending_id` 曾在本族第三方子接口重复声明，现已收敛为「账号ID」域 `IWechatWorkAccountIdService` 的**单一所有者**；第三方侧请改用 `ConvertUnionidToExternalUserIdAsync` / `ConvertExternalUserIdToPendingIdAsync`（见 [AccountId/IdConvert.md](../AccountId/IdConvert.md)）。本域只保留 E11。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.ThirdParty` / `WechatAppType.Provider`（E11 仅第三方） | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（CU3 对四接口统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp` | 运行期归属校验 |

```csharp
// 第三方 / 代开发：企业级令牌一企一份，先切作用域再调用
using (switcher.UseCorpScope("third-party-a", authCorpId, permanentCode))
{
    var converted = await thirdPartyCustomer.ConvertToServiceExternalUserIdAsync(request, ct);
}
```

### 2.2 权限与业务前置

- **读取 / 备注端点**：自建应用须配置到「**客户联系 - 可调用接口的应用**」中；第三方 / 代开发须具有「**客户基础信息**」权限。
- **规则组端点（E5–E10）**：须具有「**管理客户联系规则组**」权限，且**应用只能获取与管理由本应用创建的规则组**。
- 只能取到可见范围内、且配置了客户联系功能的成员的客户；「**营销获客**」应用只能取到该应用带来的客户。
- 分页口径三套：E3 `limit` **最大 100、默认 50**、`userid_list` **≤100**；E5/E7 `limit` **默认与上限均 1000**；E2 只在**跟进人 > 500 人**时才需要 `cursor`。E1 无分页。
- E4：各可填字段**不可同时为空**；`remark` ≤20 字符、`description` ≤150 字符、`remark_company` ≤20 字符（仅微信用户）；`remark_mobiles` **传值即整体覆盖**，清除全部要传含空字符串的元素；`remark_pic_mediaid` 经素材接口取得。
- E8/E9 **危险操作约束**：**仅支持串行调用**，勿并发创建/修改；单次最多 **20 个管理员**与 **100 个管理节点**；管理组最大层级 **5 层**；每组管理范围内最多 **3000 个节点**；E9 的 `admin_list` / `privilege` **传值即整体覆盖**。
- E8 带父规则组时，管理范围必须是父组子集，且**完全继承父组权限**（传入的 `privilege` 被忽略）。
- 敏感字段口径：`avatar` / `gender` / `unionid` / `remark_mobiles` 第三方与代开发**不可获取**（`gender` 不可得时统一返回 `0`）；`corp_full_name` **仅自建应用**返回。
- `external_userid`、`unionid`、手机号、备注与描述均敏感数据：示例用假值，不得进日志、遥测或异常消息；`corpsecret` / `permanent_code` 等只写参数名。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E10 公共面 | ✅ | ✅ | ✅ |
| E11 代开发 external_userid 转换 | ❌ | ✅（95195） | ❌ |

CU2 断言：自建与代开发子接口 `DeclaredOnly` 端点集为空、第三方子接口恰有 **1** 条差异端点。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkExternalContactCustomerService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | 10 | 否 |
| `IWechatWorkInternalExternalContactCustomerService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyExternalContactCustomerService` | `Mud.Wechat.Work` | 第三方子接口 | **1** | 是 |
| `IWechatWorkProviderExternalContactCustomerService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `ExternalContact`（16 子族共用 `AddExternalContactWebApiHttpClient()`），模块枚举 `WechatModule.ExternalContact`，装配入口 `AddExternalContactApi()`；子接口 `InheritedFrom = nameof(WechatWorkExternalContactCustomerService)`。

## 4. 端点

动词分布：E1/E2 为 **GET**（本族仅有的两支 GET），E3–E11 全为 **POST**（含 E5/E6/E7 这些查询语义端点）。

### E1. `GetCustomerListAsync` — 获取客户列表

```csharp
Task<GetCustomerListResponse> GetCustomerListAsync(
    [Query("userid")] string userid, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | **`GET`** `/cgi-bin/externalcontact/list` |
| 令牌 | Query `access_token` |

- 请求：`userid`（企业成员 userid，非外部联系人）。
- 响应：`external_userid`（`List<string>?`）——该成员添加的全部客户 id，**无分页**。

### E2. `GetCustomerDetailAsync` — 获取客户详情

```csharp
Task<GetCustomerDetailResponse> GetCustomerDetailAsync(
    [Query("external_userid")] string externalUserid,
    [Query("cursor")] string? cursor = null,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | **`GET`** `/cgi-bin/externalcontact/get` |
| 请求 | Query `external_userid`（必填）、`cursor`（**跟进人 > 500 人**时才需要） |

**响应参数**（`GetCustomerDetailResponse`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `external_contact` | `ExternalContact` | `ExternalContactInfo?` | 客户基本信息 |
| `external_contact.external_userid` / `name` / `type` | 同名 | `string?` / `string?` / `int?` | `type`：**1 微信用户 / 2 企业微信用户** |
| `external_contact.avatar` / `gender` / `unionid` | 同名 | `string?` / `int?` / `string?` | **第三方与代开发不可获取**；`gender` 0 未知 / 1 男 / 2 女，不可得时返回 `0` |
| `external_contact.corp_name` / `corp_full_name` | 同名 | `string?` | 仅企业微信用户返回；`corp_full_name` **仅自建应用** |
| `external_contact.position` / `external_profile` | 同名 | — | 仅企业微信用户 |
| `external_contact.is_subscribe` / `subscriber_info` / `remark` / `remark_mobiles` / `tag_id` | 同名 | — | **家校沟通场景**（家长关注「学校通知」）才出现的可选字段；`remark_mobiles` 第三方不返回 |
| `follow_user[]` | `FollowUser` | `List<CustomerFollowUser>?` | 跟进人列表（**完整标签结构**） |
| `follow_user[].remark` / `description` / `createtime` | 同名 | — | 成员对该客户的备注/描述/添加时间 |
| `follow_user[].tags[]` | `Tags` | `List<CustomerFollowTag>?` | 元素含 `group_name` / `tag_name` / `tag_id`（`type = 2` 自定义标签**不返回 tag_id**）/ `type` |
| `follow_user[].add_way` | `AddWay` | `int?` | 添加来源枚举（挂在跟进人层，**不在客户基本信息层**） |
| `follow_user[].state` / `oper_userid` / `wechat_channels` | 同名 | — | 渠道参数 / 发起添加者 / 视频号来源（`add_way = 10`） |
| `next_cursor` | `NextCursor` | `string?` | 跟进人 > 500 时返回，下次经 `cursor` 传入 |

**本端点注意事项**

- `follow_user[].tags[].type`：**1 企业设置 / 2 用户自定义 / 3 规则组标签**——只有 1/3 有 `tag_id`。
- `oper_userid` 语义随添加方式变（成员主动添加=成员 userid；客户主动添加=客户的外部联系人 userid；共享/管理员分配=对应成员或管理员 userid）。

### E3. `BatchGetCustomerDetailsAsync` — 批量获取客户详情

```csharp
Task<BatchGetCustomerDetailsResponse> BatchGetCustomerDetailsAsync(
    [Body] BatchGetCustomerDetailsRequest request, CancellationToken cancellationToken = default);
```

- 路由 **`POST`** `/cgi-bin/externalcontact/batch/get_by_user`（查询走 POST）。

**请求参数**

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `userid_list` | `UseridList` | `List<string>?` | 是 | 企业成员 userid，**≤100** |
| `cursor` | `Cursor` | `string?` | 否 | 首次可不填 |
| `limit` | `Limit` | `int?` | 否 | **最大 100、默认 50**；超上限取上限 |

**响应参数**

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `external_contact_list[]` | `ExternalContactList` | `List<BatchCustomerContactItem>?` | 每项 = `external_contact` + **单个** `follow_info` |
| `follow_info.tag_id[]` | `TagId` | `List<string>?` | **只返回 tag_id 列表**（企业标签与规则组标签；个人标签不返回），与 E2 的 `tags[]` 完整结构不同 |
| `next_cursor` | `NextCursor` | `string?` | 空即无更多 |
| `fail_info.unlicensed_userid_list[]` | `FailInfo.UnlicensedUseridList` | `List<string>?` | 无有效互通许可的成员 |

**失败语义**：请求中**全部** userid 都无有效互通许可 ⇒ 接口直接报 **701008**；**部分**无许可 ⇒ 仍返回成功，并在 `fail_info` 列出。

### E4. `UpdateCustomerRemarkAsync` — 修改客户备注信息

```csharp
Task<WechatWorkResponse> UpdateCustomerRemarkAsync(
    [Body] UpdateCustomerRemarkRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `userid` | `Userid` | `string?` | 是 | 企业成员 userid（须是该客户的跟进人且在可见范围内） |
| `external_userid` | `ExternalUserid` | `string?` | 是 | 外部联系人 userid |
| `remark` | `Remark` | `string?` | 条件 | ≤20 字符 |
| `description` | `Description` | `string?` | 条件 | ≤150 字符 |
| `remark_company` | `RemarkCompany` | `string?` | 条件 | ≤20 字符，仅微信用户有效 |
| `remark_mobiles` | `RemarkMobiles` | `List<string>?` | 条件 | **整体覆盖**；清除全部传含空字符串的元素 |
| `remark_pic_mediaid` | `RemarkPicMediaid` | `string?` | 条件 | 备注图片 mediaid |

**各可填字段不可同时为空**；应答仅 `errcode`/`errmsg`。

### E5–E7. 规则组读取三支

```csharp
Task<GetCustomerStrategyListResponse> GetCustomerStrategyListAsync([Body] GetCustomerStrategyListRequest request, CancellationToken cancellationToken = default);
Task<GetCustomerStrategyDetailResponse> GetCustomerStrategyDetailAsync([Body] GetCustomerStrategyDetailRequest request, CancellationToken cancellationToken = default);
Task<GetCustomerStrategyRangeResponse> GetCustomerStrategyRangeAsync([Body] GetCustomerStrategyRangeRequest request, CancellationToken cancellationToken = default);
```

| 端点 | 路由 | 请求 | 响应 |
|---|---|---|---|
| E5 列表 | `POST /cgi-bin/externalcontact/customer_strategy/list` | `cursor?` / `limit?`（默认与上限 1000） | `strategy[]`（元素仅 `strategy_id`）+ `next_cursor`（无更多时不返回） |
| E6 详情 | `POST /cgi-bin/externalcontact/customer_strategy/get` | `strategy_id`（`long`，必填） | `strategy`（含 `parent_id` / `strategy_name` / `create_time` / `admin_list` / `privilege`） |
| E7 管理范围 | `POST /cgi-bin/externalcontact/customer_strategy/get_range` | `strategy_id` + `cursor?` / `limit?`（默认与上限 1000） | `range[]` + `next_cursor` |

范围节点 `CustomerStrategyRangeNode`：`type`（**1 成员 / 2 部门**）、`userid`（仅 type=1）、`partyid`（仅 type=2，`long?`）。

`privilege`（`CustomerStrategyPrivilege`，全 `bool?`）的口径：

- **基础权限不可取消**：`view_customer_data` / `view_room_list` / `contact_me` / `join_room`（`view_customer_list` 为查看客户列表）。
- 其余（`share_customer`、`oper_resign_customer`、`oper_resign_group`、`send_customer_msg`、`edit_welcome_msg`、`view_behavior_data`、`view_room_data`、`send_group_msg`、`room_deduplication`、`rapid_reply`、`onjob_customer_transfer`、`edit_anti_spam_rule`、`export_customer_list`、`export_customer_data`、`export_customer_group_list`、`manage_customer_tag`）**官方默认 `true`**。
- **创建 / 编辑时「未传 = 不设置」**（不是显式 false）。

### E8. `CreateCustomerStrategyAsync` — 创建规则组

```csharp
Task<CreateCustomerStrategyResponse> CreateCustomerStrategyAsync(
    [Body] CreateCustomerStrategyRequest request, CancellationToken cancellationToken = default);
```

请求：`strategy_name`、`parent_id?`（无父组可不填）、`admin_list?`（**≤20**）、`privilege?`（**有父组时被忽略**）、`range?`（**≤100 个节点**）。
响应：`strategy_id`（`long?`）。

**危险操作**：**仅支持串行调用**；层级 ≤5、单组管理范围 ≤3000 节点；带父组时管理范围必须是父组子集并完全继承父组权限。

### E9. `UpdateCustomerStrategyAsync` — 编辑规则组及其管理范围

```csharp
Task<WechatWorkResponse> UpdateCustomerStrategyAsync(
    [Body] UpdateCustomerStrategyRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/customer_strategy/edit`；请求 `strategy_id`（必填）+ `strategy_name?`（不填不改）+ `admin_list?` / `privilege?`（**空则不编辑、传值即整体覆盖**）+ `range_add?` / `range_del?`（**增删列表**，不是整体覆盖）。
- 同样「仅串行调用、单次 ≤20 管理员与 ≤100 节点」。

### E10. `DeleteCustomerStrategyAsync` — 删除规则组

```csharp
Task<WechatWorkResponse> DeleteCustomerStrategyAsync(
    [Body] DeleteCustomerStrategyRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/customer_strategy/del`；请求 `strategy_id`（`long`，必填）；只能删本应用创建的规则组。

### E11. `ConvertToServiceExternalUserIdAsync` — 代开发 external_userid 转第三方（仅第三方）

```csharp
Task<ConvertToServiceExternalUserIdResponse> ConvertToServiceExternalUserIdAsync(
    [Body] ConvertToServiceExternalUserIdRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/to_service_external_userid`；声明于第三方子接口。
- 用途：企业**同时授权**了同一服务商的第三方应用与代开发应用时，把代开发侧拿到的 `external_userid` 换成第三方主体的 `external_userid`（代开发自建应用已升级后无需调用）。
- 请求 `external_userid`（`string?`）；响应 `external_userid`（`string?`，第三方应用主体下的外部联系人 ID）。

**调用示例**（本族主示例：批量详情翻页 + 跟进人渠道参数读取 + 备注整体覆盖）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.ExternalContact.Customer;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "crm-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddExternalContactApi());

var customers = provider.GetRequiredService<IWechatWorkInternalExternalContactCustomerService>();

// ① 批量取客户详情：userid_list ≤100、limit ≤100（默认 50）、cursor 翻页
string? cursor = null;
var channelCounts = new Dictionary<int, int>();
do
{
    var resp = await customers.BatchGetCustomerDetailsAsync(
        new BatchGetCustomerDetailsRequest
        {
            UseridList = new List<string> { "zhangsan", "lisi" },   // 假值
            Cursor = cursor,
            Limit = 100,
        },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/externalcontact/batch/get_by_user");

    foreach (BatchCustomerContactItem item in resp.ExternalContactList ?? new List<BatchCustomerContactItem>())
    {
        // add_way 挂在 follow_info 一层，不在 external_contact 一层。
        int addWay = item.FollowInfo?.AddWay ?? 0;
        channelCounts[addWay] = channelCounts.TryGetValue(addWay, out int n) ? n + 1 : 1;
    }

    // 部分成员无互通许可时接口仍成功，须单独读 fail_info
    foreach (string unlicensed in resp.FailInfo?.UnlicensedUseridList ?? new List<string>())
    {
        logger.LogWarning("成员无有效互通许可：{Userid}", unlicensed);
    }

    cursor = resp.NextCursor;
} while (!string.IsNullOrEmpty(cursor));

// ② 改备注：可填字段不可同时为空；remark_mobiles 传值即整体覆盖
var remark = await customers.UpdateCustomerRemarkAsync(
    new UpdateCustomerRemarkRequest
    {
        Userid = "zhangsan",                 // 假值
        ExternalUserid = "<external_userid>",
        Remark = "重点客户",                  // ≤20 字符
        Description = "由官网渠道添加",        // ≤150 字符
        RemarkMobiles = new List<string> { string.Empty },   // 含空字符串元素 = 清除全部备注手机号
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(remark, "/cgi-bin/externalcontact/remark");

logger.LogInformation("渠道分布：{Channels}", channelCounts);
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| E2 `follow_user[].tags[]`（完整结构）vs E3 `follow_info.tag_id[]`（只有 id） | 同一标签信息两种形态 | 在批量结果里读 `tag_name` → 恒空 |
| `external_contact` 层 vs `follow_user`/`follow_info` 层 | `add_way`/`state`/`oper_userid`/`remark_mobiles` 只在跟进人层 | 从客户基本信息取渠道参数 → 取不到 |
| E1 `userid` vs E2 `external_userid` | 前者是企业成员账号、后者是外部联系人 | 传反 → 空列表或参数错误 |
| E2 的分页条件 | **跟进人 > 500** 才返回 `next_cursor` | 以为无分页 → 大客户漏跟进人 |
| E3 `limit`（≤100，默认 50）vs E5/E7 `limit`（默认与上限 1000）vs 统计族 `offset+limit` | 三种分页口径并存，游标与偏移不可混用 | 把 1000 套到批量详情 → 被截到 100 |
| E4 `remark_mobiles` | **整体覆盖**语义；清除要传含空字符串的元素 | 以为追加 → 旧号码被抹掉 |
| E9 `admin_list`/`privilege` vs `range_add`/`range_del` | 前者整体覆盖、后者增删列表 | 用覆盖思维传 range → 误删管理范围 |
| E8 的 `privilege` 与父规则组 | 有父组时**被忽略**，权限完全继承父组 | 以为能给子组单独配权限 |
| 规则组归属 | 应用**只能读写本应用创建的**规则组 | 换 AppKey 后找不到既有规则组 |
| unionid ↔ external_userid 转换 | 属「账号ID」域单一所有者，**不在本族** | 在本族找 `ConvertUnionidToExternalUserIdAsync` → 不存在 |
| E3 的 701008 vs `fail_info` | 全部无许可直接报错；部分无许可仍成功并列出 | 只判 `errcode` → 漏掉部分失败成员 |
| `gender` 的 0 | 第三方/代开发不可得时官方**返回 0**，与「未知」同值 | 把 0 当「性别未知」统计 |
| E1/E2 的 GET vs E5/E6/E7 的 POST | 规则组三读取端点官方即 POST（CU1 锁定） | 改写动词 → 打坏 CU1 与 G5 |

SDK 侧通用约束：**不自动判错**、**不串行化 E8/E9**、**不做本地可见性预判**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E3 成功仍可能带 `fail_info` |
| 701008 | 官方业务码（接口注释留档） | 请求内所有 userid 均无有效互通许可 | 批量详情专属 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法/过期/关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，装配缺陷 |

其余业务码（权限未配置、规则组非本应用创建、`add_way` 非法、字段超长）**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| `add_way` 完整枚举 | 注释称「详见类型注释」，本批未逐项展开取值表 ⇒ 未逐项核验 | `ExternalContactInfo` / `CustomerFollowUser` 注释 + 官方 92114 |
| `ExternalContactInfo` / `CustomerFollowUser` 响应字段全集 | 家校场景字段（`is_subscribe` / `subscriber_info` / `tag_id`）是否对普通企业返回**未逐项核验** | 官方 92114 |
| E1/E2 的频率与单次上限 | 注释未给数值（E1 无分页但客户数量上限未留档） | 官方 92113/92114 |
| 规则组数量上限与命名约束 | 注释给了管理员/节点/层级/范围上限，未给企业规则组总数上限 | 官方 94883 |
| 业务错误码全集 | 仅留档 701008 | 官方各端点页 |
| 守卫 CU 系列类级 remarks 的端点数 | 类级 remarks 仍写「第三方子接口独有 3 条身份转换差异端点」，而 CU1 `HaveCount(11)` 与 CU2 `HaveCount(1)` 是收敛后的权威值 ⇒ **以断言为准**，该 remarks 属待订正注释 | 见守卫源文件 CU1/CU2 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.1 速览、§4 陷阱 1/2/3/10/13）
- 同域：[Tag.md](./Tag.md)（标签的两面）· [ContactWay.md](./ContactWay.md)（`state` 区分的「联系我」）· [Statistics.md](./Statistics.md)（成员/群聊统计）· [JobInheritance.md](./JobInheritance.md) / [ResignedInheritance.md](./ResignedInheritance.md)（客户转接）
- 跨域：unionid / pending_id 转换见 [AccountId/IdConvert.md](../AccountId/IdConvert.md)；客户变更事件推送属 `Callback` 包
- 官方页：[获取客户列表（自建 92113）](https://developer.work.weixin.qq.com/document/path/92113) · [获取客户详情（92114）](https://developer.work.weixin.qq.com/document/path/92114) · [批量获取客户详情（92994）](https://developer.work.weixin.qq.com/document/path/92994) · [客户联系规则组（94883）](https://developer.work.weixin.qq.com/document/path/94883)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
