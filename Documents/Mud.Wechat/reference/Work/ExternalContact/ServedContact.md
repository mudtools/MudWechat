# 客户联系 · 已服务的外部联系人

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `ExternalContact` 域 · 已服务客户族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkExternalContactServedContactService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/ServedContact/IWechatWorkExternalContactServedContactService.cs)（父，**零端点**）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/ServedContact/IWechatWorkExternalContactServedContactService_Internal.cs)（承载唯一 1 支） |
| DTO 源文件 | [DataModels/ExternalContact/ServedContact/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ExternalContact/ServedContact/)（3 型，`SerializerClassName = ServedContact`，登记断言见 SC4 → `ServedContactJsonContext`） |
| 契约守卫 | [WechatExternalContactServedContactContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactServedContactContractGuards.cs)（SC1 路由 / SC2 父接口零端点 + 自建子接口 `HaveCount(1)` + 无第三方/代开发子接口 / SC3 令牌绑定 / SC4 JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 仅企业自建应用：[99434](https://developer.work.weixin.qq.com/document/path/99434) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 18 篇的可运行示例合并逐字编译，0 错误（本篇 1 段）；本批 20 段、全仓累计 102 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：SC1~SC4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.15、§4 陷阱 1）· 官方：本批未重新取文，正文口径继承接口注释留档 |

## 1. 这个接口族做什么

单支端点：**分页遍历企业内所有「已服务的外部联系人」**，并带出每个联系人是**谁加的**、以及**加入了哪些群**：

| 端点 | 用途 |
|---|---|
| E1 `contact_list` | `cursor` + `limit` 分页拉取记录（客户 / 非客户 / 群三种归属信息混在一张表里） |

与 [Customer.md](./Customer.md) 的分工：`externalcontact/list` 是「**某个成员**添加了哪些客户」，本端点是「**全企业被服务过的外部联系人**」视角，并且会返回**未被标记为客户**的外部联系人（这部分只有临时 id 与脱敏昵称）。

形态：**官方仅向企业自建应用开放**（第三方应用与服务商代开发暂不支持）⇒ 父接口零端点、**不设第三方 / 代开发子接口**，唯一端点声明在自建子接口（SC2 同时断言「父接口 `DeclaredOnly` 为空」与「自建子接口恰好 1 支」）。这一形态与 `IWechatWorkContactRulesService`、[Dial.md](../Dial/Dial.md) 同族。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | **仅** `WechatAppType.Internal`（第三方 / 代开发无子接口，官方暂不支持） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（SC3 断言父 + 自建两接口一致） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

### 2.2 权限与业务前置

- 须使用**配置到「客户联系 可调用接口的应用」中的 secret** 获取的 `access_token` 调用。
- **一轮完整遍历必须自始至终**：`cursor` 为空即视为「新的一轮」——**每次以空 `cursor` 起头请求时，返回的临时 id 与 `next_cursor` 都会变化**，因此 `tmp_openid` **只在同一轮遍历内唯一**，不可跨轮次去重、不可落库长期引用。
- `cursor` **有有效期**（注释明确「请勿缓存后使用」）⇒ 遍历中途长时间暂停后再续可能失效。
- `next_cursor` **有效期 4 小时**；无更多数据时返回空。
- `limit` **默认 1000**。
- 记录字段按「身份」二选一返回：**客户** → `external_userid`；**其他外部联系人** → `name`（**脱敏昵称**）；**群聊** → 被标记为客户群时返回 `chat_id`，未被标记时返回 `chat_name`。
- 昵称、群名、临时 id 均属个人数据：示例用假值，遍历结果整表不要落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 | ✅ | ❌（暂不支持，官方文档树无对应页） | ❌（同） |

⇒ **不要**为第三方 / 代开发补子接口：SC2 断言子接口集合不漂移，且这类「官方未开放 ⇒ 零端点」的口径是本仓守卫的通用形态。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkExternalContactServedContactService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`，**零端点**） | 0 | 否 |
| `IWechatWorkInternalExternalContactServedContactService` | `Mud.Wechat.Work` | 自建子接口（**承载全部 1 支**） | **1** | 是 |

注册组 `ExternalContact`，装配入口 `AddExternalContactApi()`；子接口 `InheritedFrom = nameof(WechatWorkExternalContactServedContactService)`。

## 4. 端点

### E1. `GetServedContactListAsync` — 获取已服务的外部联系人

```csharp
[Post("/cgi-bin/externalcontact/contact_list")]
Task<GetServedContactListResponse> GetServedContactListAsync(
    [Body] GetServedContactListRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/externalcontact/contact_list` |
| 令牌 | Query `access_token` |

**请求参数**（`GetServedContactListRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `cursor` | `Cursor` | `string?` | 否 | 上一次调用返回；**首次不填**；有有效期，勿缓存后使用 |
| `limit` | `Limit` | `int?` | 否 | 每次最大记录数，**默认 1000** |

**响应参数**（`GetServedContactListResponse`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `info_list[]` | `InfoList` | `List<ServedContactInfo>?` | 记录列表 |
| `info_list[].is_customer` | `IsCustomer` | `bool?` | 该外部联系人**是否被成员标记为客户** |
| `info_list[].tmp_openid` | `TmpOpenid` | `string?` | 外部联系人**临时 id**（可用于去重，**有效期 4 小时**、**仅在一轮完整遍历中唯一**） |
| `info_list[].external_userid` | `ExternalUserid` | `string?` | **仅客户返回** |
| `info_list[].name` | `Name` | `string?` | **仅其他外部联系人返回**，且是**脱敏昵称** |
| `info_list[].follow_userid` | `FollowUserid` | `string?` | **添加该联系人的成员或群主**的 userid |
| `info_list[].chat_id` | `ChatId` | `string?` | 群聊 id（**群被标记为客户群时**返回） |
| `info_list[].chat_name` | `ChatName` | `string?` | 群名（**群未被标记为客户群时**返回） |
| `info_list[].add_time` | `AddTime` | `long?` | **首次添加 / 进群**时间（Unix 秒） |
| `next_cursor` | `NextCursor` | `string?` | 下一页游标，**有效期 4 小时**；无更多数据返回空 |

**调用示例**（单轮完整遍历：按身份分流，客户取 `external_userid`、非客户只能用临时 id 去重）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.ExternalContact.ServedContact;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "crm-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddExternalContactApi());

var served = provider.GetRequiredService<IWechatWorkInternalExternalContactServedContactService>();

// 单轮遍历：cursor 为空即「新的一轮」，同一轮内 tmp_openid 才可比；跨轮次（含重跑）tmp_openid 会变
var customerIds = new HashSet<string>(StringComparer.Ordinal);
var tmpIdsOfNonCustomers = new List<string>();
var groupKeys = new List<string>();
string? cursor = null;

do
{
    var page = await served.GetServedContactListAsync(
        new GetServedContactListRequest { Cursor = cursor, Limit = 1000 }, cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/externalcontact/contact_list");

    foreach (ServedContactInfo info in page.InfoList ?? new List<ServedContactInfo>())
    {
        // 客户侧有 external_userid；非客户只有脱敏昵称 + 临时 id，不能当长期主键用
        if (info.IsCustomer == true && info.ExternalUserid is { Length: > 0 } external)
        {
            customerIds.Add(external);
        }
        else if (info.TmpOpenid is { Length: > 0 } tmp)
        {
            tmpIdsOfNonCustomers.Add(tmp);
        }

        // 群信息同样二选一：标记为客户群给 chat_id，否则只给群名
        if (info.ChatId is { Length: > 0 } chatId) groupKeys.Add(chatId);
        else if (info.ChatName is { Length: > 0 }) groupKeys.Add($"name:{info.Name}");
    }

    // next_cursor 有效期 4 小时：遍历中途不要长时间挂起后再续
    cursor = page.NextCursor;
} while (!string.IsNullOrEmpty(cursor));

logger.LogInformation("本轮遍历：客户 {Customer} 人，非客户临时 id {Tmp} 条，群线索 {Chat} 条",
    customerIds.Count, tmpIdsOfNonCustomers.Count, groupKeys.Count);
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `tmp_openid` vs `external_userid` | 前者是**本轮遍历内的临时 id**（每轮都变、有效期 4 小时），后者才是可长期使用的客户主键 | 把 `tmp_openid` 落库做主键 → 下一轮全部对不上 |
| 空 `cursor` 起头 | 视为**新的一轮**，返回的临时 id 与 `next_cursor` **全部变化** | 中途重启用空 cursor 续拉 → 同一客户被算两次 |
| `is_customer` = true 的记录 | 返回 `external_userid`；**非客户只返回 `name`（脱敏昵称）** | 统一读 `ExternalUserid` → 非客户恒 null |
| `chat_id` vs `chat_name` | 群被标记为**客户群**才给 `chat_id`，否则只给群名 | 按 `chat_id` 聚合 → 非客户群全丢 |
| `follow_userid` | 语义是「**添加该联系人的成员或群主**」，客户与群两种记录共用一个字段 | 当成「跟进人列表」（那是 [Customer.md](./Customer.md) 的 `follow_user`） |
| `add_time` | **首次添加 / 进群**时间，两种记录共用 | 当成入群时间或好友添加时间分别统计 |
| 分页判完 | `next_cursor` 为空即完；**不要**依赖 `limit` 返回值大小 | 提前退出漏数据 |
| `externalcontact/list` vs `externalcontact/contact_list` | 前者按成员取客户列表（无分页），后者全企业遍历（含非客户与群） | 用前者做全量盘点 → 漏非客户与群维度 |
| 应用类型 | **仅自建**：第三方 / 代开发子接口不存在，DI 里解析不到 | 按本域其它族的「三空标记」形态注入 → `InvalidOperationException` |
| 动词 | 读取语义却 POST（路由 `contact_list` 也不带 `get_` 前缀） | 改 GET / 改路由 → 打坏 SC1 与 G5 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）、**不做本地必填校验**、**不校验 cursor 有效期**；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 成功仍须按 `next_cursor` 决定是否续拉 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`cursor` 过期 / 非法（跨轮次复用）的具体码**接口注释未留档** ⇒ 见 §7。应用未配置到「客户联系可调用接口的应用」时的失败码同样未留档。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| `cursor` 过期的错误码与续拉策略 | 注释只说「有有效期，请勿缓存后使用」，未给码与恢复方式 ⇒ **未逐项核验** | 官方 99434 + 全局错误码页 |
| 非客户外部联系人的进一步识别 | 只给 `tmp_openid` 与脱敏昵称，未给把临时 id 转成可用标识的端点 ⇒ **未核验**（本域 `to_service_external_userid` 与「账号 ID 转换」域均不消费该临时 id） | 官方 99434 |
| 群记录的完整性 | 注释未说明同一客户群会否为每个成员各返回一条，或是否去重 ⇒ 未核验 | 官方 99434 |
| `limit` 上限 | 注释只给默认 1000，未给上界 ⇒ 未逐项核验 | 官方 99434 |
| 业务错误码全集 | 未逐条摘录 ⇒ 未逐项核验 | 官方 99434 |
| 端点频率限制 | 注释未给 QPS ⇒ 未逐项核验 | 官方全局频率附录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.15 端点表、§4 陷阱 1「external_userid / unionid 键不对称」）
- 同域：[Customer.md](./Customer.md)（按成员取客户列表与客户详情、批量详情）· [GroupChat.md](./GroupChat.md)（`chat_id` 的详情侧）· [FollowUser.md](./FollowUser.md)（哪些成员配置了客户联系功能）
- 同形态参照（零端点父接口 + 仅自建子接口）：[Dial.md](../Dial/Dial.md)、通讯录规则 `IWechatWorkContactRulesService`
- 官方页：[获取已服务的外部联系人（99434）](https://developer.work.weixin.qq.com/document/path/99434)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
