# 客户联系 · 获客助手

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `ExternalContact` 域 · 获客助手族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkExternalContactCustomerAcquisitionService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/CustomerAcquisition/IWechatWorkExternalContactCustomerAcquisitionService.cs)（父，承载全部 9 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/CustomerAcquisition/IWechatWorkExternalContactCustomerAcquisitionService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/CustomerAcquisition/IWechatWorkExternalContactCustomerAcquisitionService_Provider.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/CustomerAcquisition/IWechatWorkExternalContactCustomerAcquisitionService_ThirdParty.cs)（均空标记） |
| DTO 源文件 | [DataModels/ExternalContact/CustomerAcquisition/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ExternalContact/CustomerAcquisition/)（`SerializerClassName = CustomerAcquisition`） |
| 契约守卫 | [WechatExternalContactCustomerAcquisitionContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactCustomerAcquisitionContractGuards.cs)（CA1 路由表 / CA2 层级与三空标记 / CA3 令牌绑定 / CA4 JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 链接管理 97297/97394/97398 · 获客客户 97298/97395/97399 · 额度与统计 97375/97396/97400 · 成员多次收消息 100130/100134/100133 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 18 篇的可运行示例合并逐字编译，0 错误（本篇 1 段）；本批 20 段、全仓累计 102 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：CA1~CA4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.6、§4 陷阱 3/10/11）· 官方：本批未重新取文，正文口径继承域 README 留档 |

## 1. 这个接口族做什么

「获客助手」= 一条**可投放到外部渠道的加人链接**，配一套额度与统计：

| 能力块 | 端点 |
|---|---|
| 获客链接管理 | E1 列 link_id · E2 链接详情 · E3 创建 · E4 编辑 · E5 删除 |
| 由链接添加的客户 | E6 `customer_acquisition/customer` |
| 额度与使用量 | E7 `customer_acquisition_quota`（**GET，本族唯一 GET**）· E8 按天统计 |
| 会话线索 | E9 `get_chat_info`（成员多次收消息详情，凭回调 `chat_key`） |

链接的生命周期：**创建返回 `link_id` + 实际 `url`** ⇒ 把 `url` 投放到渠道 ⇒ 客户经它加人 ⇒ E6 取回这批客户与其 `state`，E8 看点击与转化，E7 看额度还剩多少（**获客助手是按额度计费的能力**）。

形态：官方对三类应用开放完全一致的 9 支端点 ⇒ 全收敛父接口，三支子接口空标记（CA2）。除 E7 外**全部 POST**。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.ThirdParty` / `WechatAppType.Provider` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（CA3 统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp` | 运行期归属校验 |

### 2.2 权限与业务前置

- **使用范围**：`range.user_list`（成员，最多 500 人）与 `range.department_list`（部门，覆盖总人数最多 500）**不可同时为空**；成员/部门须**在应用可见范围或「客户可建联成员」范围内**（后者见 [FollowUser.md](./FollowUser.md) E2）。
- **优先分配 `priority_option`**：`priority_type` = `1` 全企业范围内优先分配给有好友关系的成员 / `2` 指定范围内优先分配（此时 `priority_userid_list` **必填，最多 1000 人**）；**仅部分经营类目企业支持**。
- **E4 编辑是覆盖式**：`range` 与 `priority_option` 传值即整体覆盖。
- **只能编辑 / 删除当前应用创建的链接**；`link_id` 类查询同样限本应用创建。
- 分页上限各端点不同：E1 `limit` **最大 100**；E6 `limit` **最大 1000**。
- E8 统计：**最小粒度为日**（起止时间戳自动转换到所在日，**闭区间**）；**仅可查最近 180 天**；**起止相差不可超过 30 天**。
- E9 `chat_key` 来自「成员多次收消息事件」回调，**回调后 30 分钟内有效，须及时调用**。
- `skip_verify` 默认 `true`；`mark_source` 默认 `true` 且**仅对「营销获客」应用生效**。
- 获客助手是**按额度计费**的能力 ⇒ 上线前先查 E7 的 `balance`。
- `external_userid`、`state`、链接 `url` 属敏感/业务数据：示例用假值，不落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E9 | ✅ | ✅ | ✅ |

（获客**组件**与代支付账单是另一族，见 [AcquisitionComponent.md](./AcquisitionComponent.md)——那族**仅第三方**，且账单端点走 `suite_access_token`。）

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkExternalContactCustomerAcquisitionService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **9** | 否 |
| `IWechatWorkInternalExternalContactCustomerAcquisitionService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyExternalContactCustomerAcquisitionService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderExternalContactCustomerAcquisitionService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `ExternalContact`，装配入口 `AddExternalContactApi()`；子接口 `InheritedFrom = nameof(WechatWorkExternalContactCustomerAcquisitionService)`。

## 4. 端点

### E1. `GetAcquisitionLinkListAsync` — 获取获客链接列表

```csharp
Task<GetAcquisitionLinkListResponse> GetAcquisitionLinkListAsync(
    [Body] GetAcquisitionLinkListRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/customer_acquisition/list_link`。
- 请求：`limit`（`int?`，**最大 100**）、`cursor`（`string?`，首次不填）。
- 响应：`link_id_list`（`List<string>?`，**只有 id**）、`next_cursor`（`string?`，**无更多数据时不返回该字段**）。

### E2. `GetAcquisitionLinkAsync` — 获取获客链接详情

```csharp
Task<GetAcquisitionLinkDetailResponse> GetAcquisitionLinkAsync(
    [Body] GetAcquisitionLinkDetailRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/customer_acquisition/get`（**读取即 POST**）；请求 `link_id`（必填，须为本应用创建）。
- 响应 `link`（`AcquisitionLink`）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `link_id` | `LinkId` | `string?` | 链接 id |
| `link_name` | `LinkName` | `string?` | 名称，最长 30 个字符 |
| `url` | `Url` | `string?` | **实际可投放的链接** |
| `create_time` | `CreateTime` | `long?` | 创建时间（Unix 秒） |
| `skip_verify` | `SkipVerify` | `bool?` | 添加时是否免验证，默认 `true` |
| `mark_source` | `MarkSource` | `bool?` | 标记添加来源，默认 `true`，仅营销获客应用生效 |
| `range.user_list[]` | `Range.UserList` | `List<string>?` | 关联成员，**≤500 人** |
| `range.department_list[]` | `Range.DepartmentList` | `List<long>?` | 关联部门，覆盖总人数 **≤500** |
| `priority_option.priority_type` | `PriorityOption.PriorityType` | `int?` | `1` 全企业优先有好友关系成员 / `2` 指定范围内优先分配 |
| `priority_option.priority_userid_list[]` | `PriorityOption.PriorityUseridList` | `List<string>?` | `priority_type = 2` 时必填，**≤1000 人** |

### E3. `CreateAcquisitionLinkAsync` — 创建获客链接

```csharp
Task<CreateAcquisitionLinkResponse> CreateAcquisitionLinkAsync(
    [Body] CreateAcquisitionLinkRequest request, CancellationToken cancellationToken = default);
```

**请求参数**（`CreateAcquisitionLinkRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `link_name` | `LinkName` | `string?` | **是** | 最长 30 个字符 |
| `range` | `Range` | `AcquisitionLinkRange?` | 是（条件） | 成员与部门**不可同时为空**；覆盖总人数 ≤500 |
| `skip_verify` | `SkipVerify` | `bool?` | 否 | 默认 `true` |
| `priority_option` | `PriorityOption` | `AcquisitionPriorityOption?` | 否 | **仅部分经营类目企业支持** |
| `mark_source` | `MarkSource` | `bool?` | 否 | 默认 `true`；仅营销获客应用生效 |

**响应参数**：`link`（含 `link_id`、`link_name`、`url`、`create_time`）。

**调用示例**（本族主示例：建链接 → 取客户 → 查额度与按天统计）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.ExternalContact.CustomerAcquisition;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "crm-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddExternalContactApi());

var acq = provider.GetRequiredService<IWechatWorkInternalExternalContactCustomerAcquisitionService>();

// ① 建链接：range 的成员与部门不可同时为空，覆盖总人数 ≤500
var created = await acq.CreateAcquisitionLinkAsync(
    new CreateAcquisitionLinkRequest
    {
        LinkName = "官网首页入口",
        Range = new AcquisitionLinkRange { UserList = new List<string> { "zhangsan" } },  // 假值
        SkipVerify = true,
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(created, "/cgi-bin/externalcontact/customer_acquisition/create_link");
string linkId = created.Link?.LinkId ?? throw new InvalidOperationException("官方未返回 link_id");

// ② 由该链接添加的客户（link_id 必填；limit 最大 1000；须是本应用创建的链接）
string? cursor = null;
int customers = 0;
do
{
    var page = await acq.GetAcquisitionCustomerListAsync(
        new GetAcquisitionCustomerListRequest { LinkId = linkId, Limit = 1000, Cursor = cursor },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/externalcontact/customer_acquisition/customer");

    foreach (AcquisitionCustomerItem item in page.CustomerList ?? new List<AcquisitionCustomerItem>())
    {
        // chat_status：0 客户未发消息 / 1 已发消息 / 2 未知
        customers++;
    }
    cursor = page.NextCursor;
} while (!string.IsNullOrEmpty(cursor));

// ③ 额度：本族唯一 GET，且路由是 customer_acquisition_quota（下划线、不是子路径）
var quota = await acq.GetAcquisitionQuotaAsync(cancellationToken);
WechatWorkException.ThrowIfFailed(quota, "/cgi-bin/externalcontact/customer_acquisition_quota");
logger.LogInformation("获客助手累计用量 {Total}，剩余 {Balance}，额度批次 {Batches}",
    quota.Total, quota.Balance, quota.QuotaList?.Count ?? 0);

// ④ 按天统计：闭区间、粒度为日、跨度 ≤30 天、只查最近 180 天
long end = new DateTimeOffset(new DateTime(2026, 10, 28, 0, 0, 0, DateTimeKind.Utc)).ToUnixTimeSeconds();
long start = end - 29L * 24 * 3600;
var stat = await acq.GetAcquisitionLinkStatisticAsync(
    new GetAcquisitionLinkStatisticRequest { LinkId = linkId, StartTime = start, EndTime = end },
    cancellationToken);
WechatWorkException.ThrowIfFailed(stat, "/cgi-bin/externalcontact/customer_acquisition/statistic");
logger.LogInformation("点击链接客户数 {Clicks}，新增客户数 {NewCnt}（获客 {Rate:P1}）",
    stat.ClickLinkCustomerCnt, stat.NewCustomerCnt,
    stat.ClickLinkCustomerCnt > 0 ? (double)stat.NewCustomerCnt / stat.ClickLinkCustomerCnt : 0d);
```

### E4. `UpdateAcquisitionLinkAsync` — 编辑获客链接

```csharp
Task<WechatWorkResponse> UpdateAcquisitionLinkAsync(
    [Body] UpdateAcquisitionLinkRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/customer_acquisition/update_link`；请求 `link_id`（必填）+ `link_name?`（≤30 字符）+ `range?`（**覆盖式**，≤500 人）+ `skip_verify?` + `priority_option?`（**覆盖式**）+ `mark_source?`。
- 仅能编辑当前应用创建的链接；应答仅 `errcode`/`errmsg`。

### E5. `DeleteAcquisitionLinkAsync` — 删除获客链接

```csharp
Task<WechatWorkResponse> DeleteAcquisitionLinkAsync(
    [Body] DeleteAcquisitionLinkRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/customer_acquisition/delete_link`；请求 `link_id`（必填，须本应用创建）。

### E6. `GetAcquisitionCustomerListAsync` — 获取由获客链接添加的客户

```csharp
Task<GetAcquisitionCustomerListResponse> GetAcquisitionCustomerListAsync(
    [Body] GetAcquisitionCustomerListRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/customer_acquisition/customer`；请求 `link_id`（必填）+ `limit`（**最大 1000**）+ `cursor`。
- 响应 `customer_list[]`：

| 字段 | 属性 | 说明 |
|---|---|---|
| `external_userid` | `ExternalUserid` | 客户 |
| `userid` | `Userid` | **通过该链接添加此客户的跟进人** |
| `chat_status` | `ChatStatus` | **0 客户未发消息 / 1 客户已发消息 / 2 状态未知** |
| `state` | `State` | 渠道参数（在获客链接后拼 `customer_channel=自定义串`，**≤64 字节，超出被截断**） |

`next_cursor` 无更多数据时返回空。

### E7. `GetAcquisitionQuotaAsync` — 查询剩余使用量

```csharp
Task<GetAcquisitionQuotaResponse> GetAcquisitionQuotaAsync(CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | **`GET`** `/cgi-bin/externalcontact/customer_acquisition_quota` |
| 请求 | **无请求体**（本族唯一 GET；注意是 `customer_acquisition_quota` 这个**下划线连写的独立路径**，不是 `customer_acquisition/quota`） |

**响应参数**：`total`（历史累计使用量）、`balance`（剩余使用量）、`quota_list[]` → `AcquisitionQuotaItem`：`expire_date`（**额度过期时间戳取过期日的零点**，实际过期时间取决于额度购买时间）+ `balance`（该批即将过期的额度数量）。

### E8. `GetAcquisitionLinkStatisticAsync` — 查询获客链接使用详情

```csharp
Task<GetAcquisitionLinkStatisticResponse> GetAcquisitionLinkStatisticAsync(
    [Body] GetAcquisitionLinkStatisticRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/customer_acquisition/statistic`；请求 `link_id` + `start_time` + `end_time`（**均必填**）。
- 统计口径：**最小粒度为日**（起止时间戳自动转换到所在日，**闭区间**）；**最近 180 天内**；**跨度 ≤30 天**。
- 响应：`click_link_customer_cnt`（点击链接的客户数）、`new_customer_cnt`（新增客户数）。

### E9. `GetAcquisitionChatInfoAsync` — 获取成员多次收消息详情

```csharp
Task<GetAcquisitionChatInfoResponse> GetAcquisitionChatInfoAsync(
    [Body] GetAcquisitionChatInfoRequest request, CancellationToken cancellationToken = default);
```

- 路由 `POST /cgi-bin/externalcontact/customer_acquisition/get_chat_info`；请求 `chat_key`（必填，来自「成员多次收消息事件」回调，**回调后 30 分钟内有效**）。
- 响应：`userid`、`external_userid`、`chat_info` → `AcquisitionChatInfo`：`recv_msg_cnt`（成员收到的该客户消息次数）、`link_id`（成员添加该客户所用的获客链接）、`state`（该次添加时的链接自定义参数）。

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `customer_acquisition_quota` vs `customer_acquisition/statistic` | 前者是**独立下划线路径 + GET + 无请求体**，后者是子路径 POST | 按「统一成子路径」改写 → 打坏 CA1 与 G5 |
| E1 列表 vs E2 详情 | 列表只给 `link_id`（`limit` ≤100），详情要逐条取 | 以为一次拿全链接信息 |
| E4 的覆盖式 | `range` 与 `priority_option` **整体覆盖**，不是增量 | 想「加一个部门」却把原范围换掉 |
| `link_id` vs `config_id` | 获客链接用 `link_id`；「联系我」用 `config_id`（[ContactWay.md](./ContactWay.md)） | 混传 id → 参数错误 |
| `state` 的来源 | 获客链接侧是 URL 上拼的 `customer_channel=`（≤64 字节、超出截断）；「联系我」侧是请求字段 `state`（≤30 字符） | 用同一长度校验 → 一侧被截断 |
| `chat_status = 2` | 是「**未知**」，不是「失败」 | 当未发消息统计，转化率算低 |
| E8 的粒度 | 时间戳会被**归整到所在日**且闭区间 | 用同一天两次统计相加 → 重复计数 |
| E8 的窗口 | 最近 180 天、跨度 ≤30 天 | 一次拉一年 → 失败或数据不全 |
| `priority_option` 支持面 | **仅部分经营类目企业支持** | 不支持的企业传了 → 报错或被忽略 |
| `chat_key` 时效 | 回调后 **30 分钟**内有效 | 攒批处理 → 换不到会话详情 |
| 额度 `expire_date` | 是**过期日零点**，实际过期取决于购买时间 | 按零点精确扣减 |
| 链接归属 | 只能编辑/删除/查询**本应用创建**的链接 | 换应用后取不到既有链接 |
| 获客助手 vs 获客组件 | 本族是获客助手（三类应用）；组件/代支付账单在 [AcquisitionComponent.md](./AcquisitionComponent.md)（仅第三方、账单走 `suite_access_token`） | 找错族、令牌用错 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）、**不做额度预检与时间归整**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E6 成功仍要按 `next_cursor` 续拉 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法/过期/关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，装配缺陷 |

本族接口注释**未留档业务错误码**（额度不足、范围超 500、链接非本应用创建、统计窗口越界、`chat_key` 过期）⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 注释未列码值 ⇒ 未逐项核验 | 官方各端点页 |
| 各端点频率 | 注释未给 QPS ⇒ 未逐项核验 | 官方 + 全局频率附录 |
| 获客链接总数上限 | 注释只给范围人数（500 / 1000）与 `limit`，未给企业链接数上限 | 官方 97297 |
| 「支持优先分配的经营类目」清单 | 注释只说「仅部分经营类目企业支持」，未列类目 | 官方 97297 |
| `customer_channel` 与 `state` 的映射细节 | 注释说在链接后拼 `customer_channel=自定义串`，未说明与 E6 返回 `state` 的完整映射规则 | 官方 97298 |
| 额度的购买/续费面 | 本域只读额度，无购买端点 | 官方服务商侧说明 |
| E8 是否支持按链接分组返回 | 响应只有两个总量字段，未给逐日数组 ⇒ 未逐项核验（多日请求返回的是区间合计） | 官方 97375 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.6 端点表、§4 陷阱 3 分页口径、陷阱 10 读取即 POST、陷阱 11 删除路由拼写）
- 同域：[AcquisitionComponent.md](./AcquisitionComponent.md)（获客组件与代支付账单）· [FollowUser.md](./FollowUser.md)（「客户可建联成员」决定 range 可用范围）· [ContactWay.md](./ContactWay.md)（另一类加人入口）· [Customer.md](./Customer.md)（客户详情里的 `state`）
- 跨包：`chat_key` 来自「成员多次收消息事件」回调（属 **Callback 包**）
- 官方页：[获客链接管理（自建 97297）](https://developer.work.weixin.qq.com/document/path/97297) · [获客客户列表（97298）](https://developer.work.weixin.qq.com/document/path/97298) · [额度与统计（97375）](https://developer.work.weixin.qq.com/document/path/97375) · [成员多次收消息详情（100130）](https://developer.work.weixin.qq.com/document/path/100130)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
