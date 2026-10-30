# 管理空间 · 新建 / 重命名 / 解散 / 获取信息

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Wedrive` 域 · 管理空间族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkWedriveSpaceService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/IWechatWorkWedriveSpaceService.cs)（父，4 支）＋ [IWechatWorkInternalWedriveSpaceService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/IWechatWorkWedriveSpaceService_Internal.cs) / [_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/IWechatWorkWedriveSpaceService_Provider.cs) / [_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/IWechatWorkWedriveSpaceService_ThirdParty.cs)（三个零端点空标记） |
| DTO 源文件 | [DataModels/Wedrive/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedrive/)（4 个请求 + 4 个响应，平铺无子目录，`SerializerClassName = Wedrive` → `WedriveJsonContext`） |
| 契约守卫 | [WechatWedriveContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWedriveContractGuards.cs)（WR1 路由表 `WedriveSpaceRoutes` 4 支 + 全量 31 条 `allRoutes` / WR2 层级 / WR3 令牌 / WR4 DTO 登记 / WR5 字段名与 `spaceid` 拼写）＋ G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | [93655](https://developer.work.weixin.qq.com/document/path/93655)（新建）/ [97856](https://developer.work.weixin.qq.com/document/path/97856)（重命名）/ [97857](https://developer.work.weixin.qq.com/document/path/97857)（解散）/ [97858](https://developer.work.weixin.qq.com/document/path/97858)（获取空间信息）——均为自建应用页；第三方 95857 / 97872 / 97873 / 97874，代开发 96845 / 97862 / 97863 / 97864 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：WR1~WR5 断言 + 4 支端点 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/README.md) 第 1、2、4 节（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

微盘的一切内容都装在**空间**（space）里，空间是最外层的隔离与配额单位。本族是空间的完整生命周期：

| 端点 | 作用 | 有业务负载 |
|---|---|---|
| E1 `space_create` | 新建空间，返回 `spaceid` | 是（`spaceid`） |
| E2 `space_rename` | 重命名空间 | 否 |
| E3 `space_dismiss` | **解散**空间（不可逆） | 否 |
| E4 `space_info` | 获取空间信息（**旧版**） | 是（`space_info`） |

两条必须先记住的事实：

- **创建者是应用本身**，不是你传入的某个成员。`auth_info` 只是把成员/部门加进这个由应用拥有的空间。
- E4 是**旧版**信息端点。官方另有一个「获取空间信息（新版）」挂在权限族（`new_space_info`），多返回安全设置与已退出成员名单——见 [SpacePermission.md](./SpacePermission.md)。两者是两条独立路由、两个不同构响应类型，**不是别名**。

空间内的一切定位都是两级的：`spaceid` → 目录/文件的 `fileid`。**根目录没有独立 id，就是 `spaceid` 本身**（`fatherid` 填 `spaceid`），见 [File.md](./File.md)。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `Internal` / `ThirdParty` / `Provider`（三类等宽开放） | 注册期 `AddWechatApp` |
| 自建必填配置 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp` | 运行期归属校验 |
| 企业作用域 | 第三方 / 代开发须先 `UseCorpScope(appKey, authCorpId, permanentCode)` | 调用前 |

### 2.2 权限与业务前置

- 自建应用须配置到「**可调用应用**」列表中，使用该应用 secret 换取的 access_token 调用。
- 第三方应用与服务商代开发须具有「**微盘**」权限。
- **管理员名额**：`auth = 7`（应用空间管理员）在新建时**最多 3 个**，且**不支持设为部门**；添加成员端点（见 [SpacePermission.md](./SpacePermission.md) E1）口径为「连同已设置的管理员最多 3 个」。
- **专业版闸**：`auth = 4`（可预览）仅**专业版微盘企业**可设置。专业版状态可经 [Capacity.md](./Capacity.md) E1 查询。
- **空间类型**：`space_sub_type` 目前**只支持 0**（普通空间）。共享 / 群组 / 个人等其他形态在本域无任何留档，**不得据名称推断**。
- **解散不可逆**：E3 之后空间内文件不可访问，调用前必须确认空间归属（尤其是一次性拿到多个 `spaceid` 的批处理）。
- **频率**：本族接口注释**无任何数值型频率限制**，走[访问频率限制 90312](https://developer.work.weixin.qq.com/document/path/90312) 的默认阈值。接口注释警示**不得凭第三方 SDK 注释或记忆补数**。
- **数据安全**：`spaceid` / `auth_info[].userid` / `departmentid` 属可关联个人或组织的标识，**日志只记空间数与成员数**；重命名涉及的空间名可能含业务信息，同样不落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 | 官方文档 |
|---|---|---|---|---|
| E1 新建空间 | ✅ | ✅ | ✅ | 93655 / 95857 / 96845 |
| E2 重命名空间 | ✅ | ✅ | ✅ | 97856 / 97872 / 97862 |
| E3 解散空间 | ✅ | ✅ | ✅ | 97857 / 97873 / 97863 |
| E4 获取空间信息（旧版） | ✅ | ✅ | ✅ | 97858 / 97874 / 97864 |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkWedriveSpaceService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **4** | 否 |
| `IWechatWorkInternalWedriveSpaceService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0（继承 4） | 是 |
| `IWechatWorkThirdPartyWedriveSpaceService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0（继承 4） | 是 |
| `IWechatWorkProviderWedriveSpaceService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0（继承 4） | 是 |

注册组 `Wedrive`、装配入口 `AddWedriveApi()`；三个子接口均 `InheritedFrom = nameof(WechatWorkWedriveSpaceService)`。

> 三类应用的端点面**完全一致**，因此端点只声明在父接口上一次。子接口是零差异空标记，只承载应用类型与令牌归属键——**不要**为了「对称」在子接口重复声明端点（WR2 对子接口声明面断言恒 0）。

## 4. 端点

> 四支的凭据面完全一致（`Wechat.AccessToken` + Query `access_token` + `application/json` 源生成序列化），表中不再重复。

### E1. `CreateSpaceAsync` — 新建空间

```csharp
[Post("/cgi-bin/wedrive/space_create")]
Task<CreateWedriveSpaceResponse> CreateSpaceAsync(
    [Body] CreateWedriveSpaceRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/space_create` |
| 官方文档 | 93655 / 95857 / 96845 |

**请求**（`CreateWedriveSpaceRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `space_name` | `SpaceName` | `string?` | 官方必填，空间名 |
| `auth_info` | `AuthInfo` | `List<WedriveSpaceAclMember>?` | 空间其他成员；`auth = 7` 最多 3 个且不支持部门；`auth = 4` 仅专业版 |
| `space_sub_type` | `SpaceSubType` | `ulong?` | 空间类型：`0` 普通（目前只支持 0） |

**响应**（`CreateWedriveSpaceResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `spaceid` | `Spaceid` | `string?` | 新空间的 id（**无下划线**，照抄官方拼写） |

**成员结构**（`WedriveSpaceAclMember`，本域权限模型的唯一载体）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `type` | `Type` | `ulong?` | `1` 个人 / `2` 部门 |
| `userid` | `Userid` | `string?` | `type = 1` 时填写 |
| `departmentid` | `Departmentid` | `ulong?` | `type = 2` 时填写（32 位整型） |
| `auth` | `Auth` | `ulong?` | `1` 仅下载 / `4` 可预览（仅专业版）/ `7` 应用空间管理员 |

> `userid` 与 `departmentid` 按 `type` 择一填写，SDK **不做**本地择一校验；错配由应答 `errcode` 表达。

### E2. `RenameSpaceAsync` — 重命名空间

```csharp
[Post("/cgi-bin/wedrive/space_rename")]
Task<WechatWorkResponse> RenameSpaceAsync(
    [Body] RenameWedriveSpaceRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/space_rename` |
| 官方文档 | 97856 / 97872 / 97862 |

**请求**（`RenameWedriveSpaceRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `spaceid` | `Spaceid` | `string?` | 官方必填 |
| `space_name` | `SpaceName` | `string?` | 官方必填，重命名后的空间名 |

**响应**：无业务负载，仅 `errcode` / `errmsg`。

### E3. `DismissSpaceAsync` — 解散空间

```csharp
[Post("/cgi-bin/wedrive/space_dismiss")]
Task<WechatWorkResponse> DismissSpaceAsync(
    [Body] DismissWedriveSpaceRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/space_dismiss` |
| 官方文档 | 97857 / 97873 / 97863 |

**请求**：`spaceid`（`string?`，官方必填）。**响应**：无业务负载。

> **危险操作**：解散后空间内文件不可访问，官方未提供恢复端点的留档。批处理时建议先 `GetSpaceInfoAsync` 核对 `space_name` 再解散，不要凭 `spaceid` 前缀猜。

### E4. `GetSpaceInfoAsync` — 获取空间信息（旧版）

```csharp
[Post("/cgi-bin/wedrive/space_info")]
Task<GetWedriveSpaceInfoResponse> GetSpaceInfoAsync(
    [Body] GetWedriveSpaceInfoRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/space_info` |
| 官方文档 | 97858 / 97874 / 97864 |

**请求**：`spaceid`（`string?`，官方必填）。

**响应**（`GetWedriveSpaceInfoResponse` → `WedriveSpaceInfo`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `spaceid` | `Spaceid` | `string?` | 空间 id |
| `space_name` | `SpaceName` | `string?` | 空间名 |
| `auth_list` | `AuthList` | `WedriveSpaceAuthList?` | 成员与部门权限列表 |
| `space_sub_type` | `SpaceSubType` | `ulong?` | `0` 普通空间 |

**`auth_list`**（`WedriveSpaceAuthList`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `auth_info` | `AuthInfo` | `List<WedriveSpaceAclMember>?` | 成员/部门权限列表 |
| `quit_userid` | `QuitUserid` | `List<string>?` | **仅新版返回**；旧版 E4 不回显 |

**注意**：`WedriveSpaceInfo` 上**不存在** `secure_setting`（WR5 反向断言）。要读安全设置或已退出成员，**必须**调 [SpacePermission.md](./SpacePermission.md) E5，用 E4 会静默拿不到。

**调用示例**（建空间 → 改名 → 读回 → 解散）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Wedrive;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "drive-a";
    o.AppType = WechatAppType.Internal;          // 三类应用均可；本示例用自建
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddWedriveApi());

var space = provider.GetRequiredService<IWechatWorkInternalWedriveSpaceService>();

// ① 建空间：创建者是应用本身；auth = 7 最多 3 个且不支持部门
List<WedriveSpaceAclMember> members =
[
    new WedriveSpaceAclMember { Type = 1, Userid = "staff_0001", Auth = 7 },
    new WedriveSpaceAclMember { Type = 1, Userid = "staff_0002", Auth = 1 },
];
var created = await space.CreateSpaceAsync(
    new CreateWedriveSpaceRequest
    {
        SpaceName = "示例空间",
        SpaceSubType = 0,                        // 目前仅支持 0（普通空间）
        AuthInfo = members,
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(created, "/cgi-bin/wedrive/space_create");
string spaceId = created.Spaceid ?? throw new InvalidOperationException("响应未返回 spaceid");
logger.LogInformation("已创建空间，成员 {Count} 人", members.Count);

// ② 重命名：无业务负载，只需判 errcode
var renamed = await space.RenameSpaceAsync(
    new RenameWedriveSpaceRequest { Spaceid = spaceId, SpaceName = "示例空间（改名）" },
    cancellationToken);
WechatWorkException.ThrowIfFailed(renamed, "/cgi-bin/wedrive/space_rename");

// ③ 读回（旧版）：只有基础信息与成员权限，没有 secure_setting / quit_userid
var info = await space.GetSpaceInfoAsync(
    new GetWedriveSpaceInfoRequest { Spaceid = spaceId },
    cancellationToken);
WechatWorkException.ThrowIfFailed(info, "/cgi-bin/wedrive/space_info");
logger.LogInformation("空间类型 {SubType}，成员 {Members} 条",
    info.SpaceInfo?.SpaceSubType,
    info.SpaceInfo?.AuthList?.AuthInfo?.Count ?? 0);   // 只记条数，不打印 userid

// ④ 解散：不可逆；调用前可先核对空间名
var dismissed = await space.DismissSpaceAsync(
    new DismissWedriveSpaceRequest { Spaceid = spaceId },
    cancellationToken);
WechatWorkException.ThrowIfFailed(dismissed, "/cgi-bin/wedrive/space_dismiss");
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `spaceid` vs `space_id` | 官方拼写**无下划线**（WR5 逐个 `JsonNameShouldBe`） | 换成 `space_id` ⇒ 字段恒空 |
| 「创建者」 | **是应用本身**，不是 `auth_info` 里的人 | 以为传了 userid 就是他的空间 ⇒ 权限模型错 |
| `space_info` vs `new_space_info` | 两条独立路由、两个不同构响应 | 用旧版读安全设置 / 退出成员 ⇒ 静默拿不到 |
| `auth = 7` | 新建时最多 3 个，且**不支持部门** | 传第 4 个或部门 ⇒ 失败 |
| `auth = 4` | 仅专业版微盘企业可设 | 非专业版设置 ⇒ 失败 |
| `space_sub_type` | 只留档 `0` | 按后台空间分类猜其他值 ⇒ 无依据 |
| 解散 | 不可逆，文件随之不可访问 | 批量脚本凭前缀猜 id ⇒ 误删 |
| `userid` / `departmentid` | 按 `type` 择一，SDK 不校验 | 两个都填 / 都不填 ⇒ 应答报错 |
| HTTP 动词 | 四条**全部 POST**，含两条查询语义 | 「顺手」改 GET ⇒ 404/405 |
| 频率 | 官方无独立数字，走 90312 | 自编阈值 ⇒ 与官方不符 |

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 请求成功 | 判 `IsSuccess` |
| 40014 / 42001 / 42007 | 令牌失效码 | 应用令牌类错误 | 统一失效码处理 |
| 42011 | `InvalidProviderAccessToken` | 服务商令牌不合法 / 过期 | 第三方 / 代开发链路 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，装配缺陷 |

名额超限、空间不存在、无权限解散、非专业版设 `auth = 4` 的具体业务码以官方页为准（仓内未逐项核验）。其余见[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90312）（90312）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 本族频率 / 并发上限 | 4 份接口文件**无任何频率数值** | 官方各页 + 90312 |
| `space_sub_type` 取值全集 | 仅留档 `0` | 官方 93655 |
| `auth = 7`「最多 3 个」的计数口径 | 应用自身是否占名额 ⇒ 未核验 | 官方 93655 / 93656 |
| 空间名长度上限 | 注释只写「官方必填」，无字符数 | 官方 93655 |
| 解散后是否有回收站 / 恢复窗口 | 无留档 | 官方 97857 |
| 真账号验证 | 本批仅内部证据核验 | — |

## 8. 延伸阅读

- 同域兄弟：[SpacePermission.md](./SpacePermission.md)（成员、安全设置、**新版**空间信息）· [File.md](./File.md)（空间内文件）· [Capacity.md](./Capacity.md)（专业版状态）
- 邻域边界：[../Wedoc/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/README.md)（`wedoc/*` 是文档域，与微盘 id 体系不通用）
- 域总览：[Wedrive README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/README.md)（第 1、4 节）
- 官方页：[新建空间 93655](https://developer.work.weixin.qq.com/document/path/93655) · [重命名空间 97856](https://developer.work.weixin.qq.com/document/path/97856) · [解散空间 97857](https://developer.work.weixin.qq.com/document/path/97857) · [获取空间信息 97858](https://developer.work.weixin.qq.com/document/path/97858)
