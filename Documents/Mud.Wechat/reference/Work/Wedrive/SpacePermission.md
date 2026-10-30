# 管理空间权限 · 成员 / 安全设置 / 邀请链接 / 空间信息（新版）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Wedrive` 域 · 管理空间权限族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkWedriveSpaceAclService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/IWechatWorkWedriveSpaceAclService.cs)（父，5 支）＋ [_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/IWechatWorkWedriveSpaceAclService_Internal.cs) / [_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/IWechatWorkWedriveSpaceAclService_Provider.cs) / [_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/IWechatWorkWedriveSpaceAclService_ThirdParty.cs)（三个零端点空标记） |
| DTO 源文件 | [DataModels/Wedrive/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedrive/)（4 个请求 + 2 个响应 + 4 个嵌套结构，`SerializerClassName = Wedrive` → `WedriveJsonContext`） |
| 契约守卫 | [WechatWedriveContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWedriveContractGuards.cs)（WR1 `WedriveSpaceAclRoutes` 5 支 / WR5 `ban_share_external` ↔ `enable_share_external` 反向断言与 `space_acl_del` 不携带 `auth` / WR2 层级 / WR4 DTO 登记）＋ G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | [93656](https://developer.work.weixin.qq.com/document/path/93656)（添加）/ [97875](https://developer.work.weixin.qq.com/document/path/97875)（移除）/ [97876](https://developer.work.weixin.qq.com/document/path/97876)（安全设置）/ [97877](https://developer.work.weixin.qq.com/document/path/97877)（邀请链接）/ [97878](https://developer.work.weixin.qq.com/document/path/97878)（空间信息新版）——均为自建应用页；第三方 95858 / 97947 / 97948 / 97949 / 97950，代开发 96846 / 97910 / 97911 / 97912 / 97913 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：WR1~WR5 断言 + 5 支端点 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/README.md) 第 2、3、4、8 节（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

创建空间之后，本族负责「谁能进来、能做什么、怎么分享出去」：

| 端点 | 作用 | 语义 |
|---|---|---|
| E1 `space_acl_add` | 添加成员 / 部门 | **增量**（可一次多个） |
| E2 `space_acl_del` | 移除成员 / 部门 | **增量**（可一次多个） |
| E3 `space_setting` | 改安全设置（水印 / 保密模式 / 邀请链接 / 默认范围 / 禁止外部分享） | **字段级部分写** |
| E4 `space_share` | 取空间邀请链接 | 只读（返回 `space_share_url`） |
| E5 `new_space_info` | 取空间信息（**新版**） | 只读（比旧版多安全设置与已退出成员） |

三条关键语义：

- **权限端点是增量，不是整体替换**。添加/移除一批成员只影响这批，不会重置其他成员。想「整体替换」就得自己算出差集，**不要**先删全部再加全部（中途失败会留下空权限窗口）。
- **设置端点是字段级部分写**。未填的 `bool?` / `ulong?` 靠 `WhenWritingNull` 不序列化，服务端保持原值。SDK 不提供「清空回默认」的显式开关（对比文档域的 `clear` 参数）。
- **E5 与旧版 E4（[Space.md](./Space.md)）不是别名**。新版额外回 `secure_setting`（12 键回显）与 `auth_list.quit_userid`（已退出空间成员）。

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

- **「仅本应用创建的空间」闸**：E3 官方明确「应用通过 API 调用**只支持设置由本应用创建的空间**」。非本应用创建的空间改设置会被拒。
- **管理员名额**：`auth = 7` 在 E1 口径为「**连同已设置的管理员**最多 3 个，且不支持将部门设为管理员」——已在空间里的管理员也占名额。
- **专业版闸**：E3 的 `enable_watermark` 仅专业版企业可设置；`auth = 4` 同样仅专业版。
- **邀请链接前置**：E4 返回链接的**是否需审批**由 E3 的 `share_url_no_approve` 决定，**默认权限**由 `share_url_no_approve_default_auth` 决定。两个开关不设，链接行为取空间既有设置。
- **频率**：本族接口注释**无任何数值型频率限制**，走 90312 的默认阈值。
- **数据安全**：`auth_info[].userid` / `departmentid` / `quit_userid` 属组织与个人标识，`space_share_url` 是**可申请加入空间的凭据**——日志只记成员数与布尔位，绝不打印链接原文或 userid。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 | 官方文档 |
|---|---|---|---|---|
| E1 添加成员/部门 | ✅ | ✅ | ✅ | 93656 / 95858 / 96846 |
| E2 移除成员/部门 | ✅ | ✅ | ✅ | 97875 / 97947 / 97910 |
| E3 安全设置 | ✅ | ✅ | ✅ | 97876 / 97948 / 97911 |
| E4 获取邀请链接 | ✅ | ✅ | ✅ | 97877 / 97949 / 97912 |
| E5 获取空间信息（新版） | ✅ | ✅ | ✅ | 97878 / 97950 / 97913 |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkWedriveSpaceAclService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **5** | 否 |
| `IWechatWorkInternalWedriveSpaceAclService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0（继承 5） | 是 |
| `IWechatWorkThirdPartyWedriveSpaceAclService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0（继承 5） | 是 |
| `IWechatWorkProviderWedriveSpaceAclService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0（继承 5） | 是 |

注册组 `Wedrive`、装配入口 `AddWedriveApi()`；三个子接口均 `InheritedFrom = nameof(WechatWorkWedriveSpaceAclService)`。

## 4. 端点

> 五支的凭据面完全一致（`Wechat.AccessToken` + Query `access_token`），表中不再重复。

### E1. `AddSpaceAclAsync` — 添加成员 / 部门

```csharp
[Post("/cgi-bin/wedrive/space_acl_add")]
Task<WechatWorkResponse> AddSpaceAclAsync(
    [Body] AddWedriveSpaceAclRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/space_acl_add` |
| 官方文档 | 93656 / 95858 / 96846 |

**请求**（`AddWedriveSpaceAclRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `spaceid` | `Spaceid` | `string?` | 官方必填 |
| `auth_info` | `AuthInfo` | `List<WedriveSpaceAclMember>?` | 官方必填，可一次多个；`auth = 7` 连同已有管理员最多 3 个且不支持部门 |

**响应**：无业务负载。

**成员结构**：`WedriveSpaceAclMember`（`type` / `userid` / `departmentid` / `auth`），取值见 [Space.md](./Space.md) E1。

### E2. `DelSpaceAclAsync` — 移除成员 / 部门

```csharp
[Post("/cgi-bin/wedrive/space_acl_del")]
Task<WechatWorkResponse> DelSpaceAclAsync(
    [Body] DelWedriveSpaceAclRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/space_acl_del` |
| 官方文档 | 97875 / 97947 / 97910 |

**请求**（`DelWedriveSpaceAclRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `spaceid` | `Spaceid` | `string?` | 官方必填 |
| `auth_info` | `AuthInfo` | `List<WedriveSpaceAclMember>?` | 官方必填，可一次多个；**官方请求形态不携带 `auth`** |

**响应**：无业务负载。

> **反直觉点**：移除只靠 `type + userid` 或 `type + departmentid` 定位，**没有** `auth`。本仓 `WedriveSpaceAclMember.Auth` 是 `ulong?` + `WhenWritingNull`，所以初始化时不填 `auth` 即不会序列化——**不要**为了「结构完整」硬塞一个 `auth`，官方请求形态并不接收它。

### E3. `SetSpaceSettingAsync` — 安全设置

```csharp
[Post("/cgi-bin/wedrive/space_setting")]
Task<WechatWorkResponse> SetSpaceSettingAsync(
    [Body] SetWedriveSpaceSettingRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/space_setting` |
| 官方文档 | 97876 / 97948 / 97911 |

**请求**（`SetWedriveSpaceSettingRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `spaceid` | `Spaceid` | `string?` | 官方必填 |
| `enable_watermark` | `EnableWatermark` | `bool?` | 是否启用水印（**仅专业版**）；不填保持原样 |
| `enable_confidential_mode` | `EnableConfidentialMode` | `bool?` | 是否开启保密模式；不填保持原样 |
| `share_url_no_approve` | `ShareUrlNoApprove` | `bool?` | 通过链接加入是否**无需审批**；不填保持原样 |
| `share_url_no_approve_default_auth` | `ShareUrlNoApproveDefaultAuth` | `ulong?` | 邀请链接默认权限：`1` 仅下载 / `2` 可编辑 / `4` 仅预览 / `5` 可上传下载 / `200` 自定义 |
| `default_file_scope` | `DefaultFileScope` | `ulong?` | 文件默认可查看范围：`1` 仅成员 / `2` 企业内 |
| `ban_share_external` | `BanShareExternal` | `bool?` | 是否**禁止**文件分享到企业外（`true` = 禁止） |

**响应**：无业务负载。

### E4. `GetSpaceShareUrlAsync` — 获取邀请链接

```csharp
[Post("/cgi-bin/wedrive/space_share")]
Task<GetWedriveSpaceShareUrlResponse> GetSpaceShareUrlAsync(
    [Body] GetWedriveSpaceShareUrlRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/space_share` |
| 官方文档 | 97877 / 97949 / 97912 |

**请求**：`spaceid`（`string?`，官方必填）。

**响应**（`GetWedriveSpaceShareUrlResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `space_share_url` | `SpaceShareUrl` | `string?` | 空间邀请链接（**区别于**文件分享链接 `share_url`，见 [FilePermission.md](./FilePermission.md) E4） |

> 链接是可外发的凭据：**不得**进日志、遥测或异常消息；文档示例一律用占位符。

### E5. `GetNewSpaceInfoAsync` — 获取空间信息（新版）

```csharp
[Post("/cgi-bin/wedrive/new_space_info")]
Task<GetWedriveNewSpaceInfoResponse> GetNewSpaceInfoAsync(
    [Body] GetWedriveNewSpaceInfoRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/new_space_info` |
| 官方文档 | 97878 / 97950 / 97913 |

**请求**：`spaceid`（`string?`，官方必填）。

**响应**（`GetWedriveNewSpaceInfoResponse` → `WedriveNewSpaceInfo`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `spaceid` | `Spaceid` | `string?` | 空间 id |
| `space_name` | `SpaceName` | `string?` | 空间名 |
| `auth_list` | `AuthList` | `WedriveSpaceAuthList?` | 成员/部门权限 + `quit_userid` |
| `space_sub_type` | `SpaceSubType` | `ulong?` | `0` 普通空间 |
| `secure_setting` | `SecureSetting` | `WedriveSpaceSecureSetting?` | **仅新版返回**的安全设置回显 |

**安全设置回显**（`WedriveSpaceSecureSetting`，12 键只读）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `enable_watermark` | `EnableWatermark` | `bool?` | 是否启用水印 |
| `add_member_only_admin` | `AddMemberOnlyAdmin` | `bool?` | 是否仅管理员可加成员 |
| `enable_share_url` | `EnableShareUrl` | `bool?` | 是否开启邀请链接 |
| `share_url_no_approve` | `ShareUrlNoApprove` | `bool?` | 链接加入是否无需审批 |
| `share_url_no_approve_default_auth` | `ShareUrlNoApproveDefaultAuth` | `ulong?` | 邀请链接默认权限（取值同 E3） |
| `enable_share_external` | `EnableShareExternal` | `bool?` | 是否**允许**分享到企业外（`true` = 允许） |
| `enable_share_external_admin` | `EnableShareExternalAdmin` | `bool?` | 是否仅管理员可分享到企业外 |
| `enable_space_add_external_member` | `EnableSpaceAddExternalMember` | `bool?` | 是否允许添加外部成员 |
| `enable_space_add_external_member_admin` | `EnableSpaceAddExternalMemberAdmin` | `bool?` | 是否仅管理员可加外部成员 |
| `enable_confidential_mode` | `EnableConfidentialMode` | `bool?` | 是否开启保密模式 |
| `default_file_scope` | `DefaultFileScope` | `ulong?` | 文件默认可查看范围 |
| `create_file_only_admin` | `CreateFileOnlyAdmin` | `bool?` | 是否仅管理员可新建文件 |

> **必须取反比对**：写入口是 `ban_share_external`（`true` = 禁止），回显是 `enable_share_external`（`true` = 允许）。「设 `ban_share_external = true` 后回显 `enable_share_external` 应为 `false`」。校验时写同值比对会恒定失败。

**调用示例**（加成员 → 改安全设置 → 取邀请链接 → 读新版空间信息）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Wedrive;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "drive-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddWedriveApi());

var spaceAcl = provider.GetRequiredService<IWechatWorkInternalWedriveSpaceAclService>();
string spaceId = "SPACEID-FAKE-PLACEHOLDER";

// ① 增量添加：auth = 7 连同已有管理员最多 3 个，且不支持部门
var added = await spaceAcl.AddSpaceAclAsync(
    new AddWedriveSpaceAclRequest
    {
        Spaceid = spaceId,
        AuthInfo =
        [
            new WedriveSpaceAclMember { Type = 1, Userid = "staff_0001", Auth = 1 },
            new WedriveSpaceAclMember { Type = 2, Departmentid = 1001, Auth = 1 },
        ],
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(added, "/cgi-bin/wedrive/space_acl_add");

// ② 移除：官方请求形态不携带 auth，只用 type + userid/departmentid 定位
var removed = await spaceAcl.DelSpaceAclAsync(
    new DelWedriveSpaceAclRequest
    {
        Spaceid = spaceId,
        AuthInfo = [new WedriveSpaceAclMember { Type = 2, Departmentid = 1001 }],
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(removed, "/cgi-bin/wedrive/space_acl_del");

// ③ 安全设置：字段级部分写；未填的键保持原值；仅能改本应用创建的空间
var setting = await spaceAcl.SetSpaceSettingAsync(
    new SetWedriveSpaceSettingRequest
    {
        Spaceid = spaceId,
        EnableConfidentialMode = true,
        ShareUrlNoApprove = false,               // 通过链接加入需审批
        ShareUrlNoApproveDefaultAuth = 1,        // 默认仅下载
        DefaultFileScope = 2,                    // 文件默认可查看范围为「企业内」
        BanShareExternal = true,                 // true = 禁止分享到企业外
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(setting, "/cgi-bin/wedrive/space_setting");

// ④ 邀请链接：凭据，只记「已取到」
var share = await spaceAcl.GetSpaceShareUrlAsync(
    new GetWedriveSpaceShareUrlRequest { Spaceid = spaceId },
    cancellationToken);
WechatWorkException.ThrowIfFailed(share, "/cgi-bin/wedrive/space_share");
logger.LogInformation("邀请链接已获取：{HasUrl}", !string.IsNullOrEmpty(share.SpaceShareUrl));

// ⑤ 新版空间信息：唯一能读到 secure_setting 与 quit_userid 的端点
var info = await spaceAcl.GetNewSpaceInfoAsync(
    new GetWedriveNewSpaceInfoRequest { Spaceid = spaceId },
    cancellationToken);
WechatWorkException.ThrowIfFailed(info, "/cgi-bin/wedrive/new_space_info");
bool? allowExternal = info.SpaceInfo?.SecureSetting?.EnableShareExternal;
// 上面设了 ban_share_external = true ⇒ 这里应为 false（读写两侧方向相反，勿同值比对）
logger.LogInformation("允许企业外分享：{Allow}，已退出成员 {Quit} 人",
    allowExternal,
    info.SpaceInfo?.AuthList?.QuitUserid?.Count ?? 0);
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `ban_share_external` vs `enable_share_external` | 写入口「禁止」，回显「允许」，**方向相反** | 同值比对 ⇒ 恒定误判 |
| E5 新版 vs E4 旧版 | 新版多 `secure_setting` + `quit_userid` | 用旧版读设置 ⇒ 静默拿到 `null` |
| `space_acl_del` 的 `auth` | 官方请求形态**不携带** | 硬塞 `auth` ⇒ 违背官方形态 |
| 权限端点语义 | **增量**添加/移除 | 当整体替换 ⇒ 误删他人权限或留空窗 |
| 设置端点语义 | **字段级部分写** | 以为不填等于清零 ⇒ 实际保持原值 |
| 设置作用域 | 仅**本应用创建**的空间 | 改他人空间 ⇒ 失败 |
| `auth = 7` 计数 | 「连同已设置的管理员」最多 3 个 | 只数本次传入 ⇒ 超限 |
| 部门做管理员 | 不支持 | 传 `type = 2` + `auth = 7` ⇒ 失败 |
| `space_share_url` vs `share_url` | 空间邀请链接 vs 文件分享链接 | 混用 ⇒ 拿错分享面 |
| 链接有效期 / 撤销 | 本域**无留档、无撤销端点** | 假设可撤销 ⇒ 无 API 可用 |
| `enable_watermark` | 仅专业版可设 | 非专业版设置 ⇒ 失败 |
| HTTP 动词 | 五条**全部 POST** | 改 GET ⇒ 404/405 |

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 请求成功 | 判 `IsSuccess` |
| 40014 / 42001 / 42007 | 令牌失效码 | 应用令牌类错误 | 统一失效码处理 |
| 42011 | `InvalidProviderAccessToken` | 服务商令牌不合法 / 过期 | 第三方 / 代开发链路 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，装配缺陷 |

非本应用创建的空间、管理员超 3 个、部门设为管理员、非专业版开专业能力的具体业务码以官方页为准（仓内未逐项核验）。其余见[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90312）（90312）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 本族频率 / 并发上限 | 5 份接口文件无任何频率数值 | 官方各页 + 90312 |
| 邀请链接有效期与撤销面 | 只返回字符串；本域 31 条路由无撤销端点 | 官方 97877 |
| 空间安全设置可写面是否窄于回显 | 写 6 键 vs 回显 12 键；多出的 6 键是否有 API 写入口 ⇒ 未核验 | 官方 97876 / 97878 |
| `auth_list.auth_info` 的分页 | 无分页参数，超大空间的返回上限未成文 | 官方 97878 |
| `quit_userid` 保留期 | 已退出成员会保留多久 ⇒ 未成文 | 官方 97878 |
| 真账号验证 | 本批仅内部证据核验 | — |

## 8. 延伸阅读

- 同域兄弟：[Space.md](./Space.md)（建/改名/解散/**旧版**空间信息）· [FilePermission.md](./FilePermission.md)（文件侧分享链接 `share_url`）· [Capacity.md](./Capacity.md)（专业版状态）
- 域总览：[Wedrive README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/README.md)（第 2、3 节）
- 官方页：[添加成员 93656](https://developer.work.weixin.qq.com/document/path/93656) · [安全设置 97876](https://developer.work.weixin.qq.com/document/path/97876) · [获取邀请链接 97877](https://developer.work.weixin.qq.com/document/path/97877) · [获取空间信息（新版）97878](https://developer.work.weixin.qq.com/document/path/97878)
