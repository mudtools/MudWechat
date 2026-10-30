# 管理文件权限 · 成员 / 分享设置 / 分享链接 / 权限详情 / 安全设置

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Wedrive` 域 · 管理文件权限族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkWedriveFileAclService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/IWechatWorkWedriveFileAclService.cs)（父，6 支）＋ [_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/IWechatWorkWedriveFileAclService_Internal.cs) / [_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/IWechatWorkWedriveFileAclService_Provider.cs) / [_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/IWechatWorkWedriveFileAclService_ThirdParty.cs)（三个零端点空标记） |
| DTO 源文件 | [DataModels/Wedrive/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedrive/)（6 个请求 + 2 个响应 + 6 个嵌套结构，`SerializerClassName = Wedrive` → `WedriveJsonContext`） |
| 契约守卫 | [WechatWedriveContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWedriveContractGuards.cs)（WR1 `WedriveFileAclRoutes` 6 支 + 无负载 4 支 / WR5 `get_file_permission` 路由越前缀、`share_url` vs `space_share_url`、`file_member_list` obj-vs-数组、`watermark` 共用结构 / WR2 层级 / WR4 DTO 登记）＋ G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | [93658](https://developer.work.weixin.qq.com/document/path/93658)（新增成员）/ [97888](https://developer.work.weixin.qq.com/document/path/97888)（删除成员）/ [97889](https://developer.work.weixin.qq.com/document/path/97889)（分享设置）/ [97890](https://developer.work.weixin.qq.com/document/path/97890)（分享链接）/ [97891](https://developer.work.weixin.qq.com/document/path/97891)（权限详情）/ [97892](https://developer.work.weixin.qq.com/document/path/97892)（安全设置）——均为自建应用页；第三方 95860 / 97959 / 97960 / 97961 / 97962 / 97965，代开发 96848 / 97922 / 97923 / 97924 / 97925 / 97926 |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：WR1~WR5 断言 + 6 支端点 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/README.md) 第 2、8 节（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

单个文件 / 文件夹的**成员授权与分享面**：

| 端点 | 作用 | 语义 |
|---|---|---|
| E1 `file_acl_add` | 新增成员 / 部门 | **增量**（`auth` 仅 `1`） |
| E2 `file_acl_del` | 删除成员 / 部门 | **增量** |
| E3 `file_setting` | 分享范围设置 | 字段级部分写 |
| E4 `file_share` | 取文件分享链接 | 只读（`share_url`） |
| E5 `get_file_permission` | 读完整权限详情 | 只读 |
| E6 `file_secure_setting` | 改文件安全设置 / 水印 | 字段级部分写，**仅在线文档** |

**三处结构性提醒**：

- **E5 的路由不走 `wedrive/file_` 前缀**：官方即 `get_file_permission`（WR5 专门断言）。按前缀批量拼路由会漏掉它。
- **文件权限成员复用空间成员结构**（`WedriveSpaceAclMember`），本域**不另建成员类型**。
- **分享链接有两条不同契约**：空间的叫 `space_share_url`（见 [SpacePermission.md](./SpacePermission.md) E4），文件的叫 `share_url`（E4）。**不得混用**。

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

- **文件侧 `auth` 只有 `1`**（仅浏览 / 仅下载）。空间侧的 1 / 4 / 7 不适用——**不要把空间权限值搬到文件上**。
- **管理员审批范围**：E3 的 `auth_scope` 取 `4`（企业内需管理员审批）/ `5`（企业外需管理员审批）时，**仅有管理员时可设置**。
- **安全设置仅在线文档**：E6 官方明确「仅支持在线文档类型」，对普通文件 / 文件夹调用会失败。
- **水印写入口只 4 个字段**：`text` / `margin_type` / `show_visitor_name` / `show_text`；`force_by_admin` / `force_by_space_admin` 是**读取回显专用**。`show_visitor_name` 仅专业版支持。
- **字段级部分写**：E3 的 `auth` 不填保持原状；E6 的水印各字段不填保持原样。
- **频率**：本族接口注释**无任何数值型频率限制**，走 90312 默认阈值。
- **数据安全**：`share_url` 是可外发凭据；`file_member_list` / `co_auth_list` 含 `userid` / `departmentid`；`watermark.text` 可能含企业标识。日志只记条数与布尔位。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 | 官方文档 |
|---|---|---|---|---|
| E1 新增成员（文件） | ✅ | ✅ | ✅ | 93658 / 95860 / 96848 |
| E2 删除成员（文件） | ✅ | ✅ | ✅ | 97888 / 97959 / 97922 |
| E3 分享设置 | ✅ | ✅ | ✅ | 97889 / 97960 / 97923 |
| E4 获取分享链接 | ✅ | ✅ | ✅ | 97890 / 97961 / 97924 |
| E5 获取文件权限信息 | ✅ | ✅ | ✅ | 97891 / 97962 / 97925 |
| E6 修改文件安全设置 | ✅ | ✅ | ✅ | 97892 / 97965 / 97926 |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkWedriveFileAclService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **6** | 否 |
| `IWechatWorkInternalWedriveFileAclService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0（继承 6） | 是 |
| `IWechatWorkThirdPartyWedriveFileAclService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0（继承 6） | 是 |
| `IWechatWorkProviderWedriveFileAclService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0（继承 6） | 是 |

注册组 `Wedrive`、装配入口 `AddWedriveApi()`；三个子接口均 `InheritedFrom = nameof(WechatWorkWedriveFileAclService)`。

## 4. 端点

> 六支的凭据面完全一致（`Wechat.AccessToken` + Query `access_token`），表中不再重复。

### E1. `AddFileAclAsync` — 新增成员（文件权限）

```csharp
[Post("/cgi-bin/wedrive/file_acl_add")]
Task<WechatWorkResponse> AddFileAclAsync(
    [Body] AddWedriveFileAclRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/file_acl_add` |
| 官方文档 | 93658 / 95860 / 96848 |

**请求**（`AddWedriveFileAclRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `fileid` | `Fileid` | `string?` | 官方必填 |
| `auth_info` | `AuthInfo` | `List<WedriveSpaceAclMember>?` | 官方必填，可一次多个；`auth` **仅 `1`** |

**响应**：无业务负载。

> `type`（1 个人 / 2 部门）与 `departmentid` 官方标注「**后续将废弃**」；成员结构复用 `WedriveSpaceAclMember`。

### E2. `DelFileAclAsync` — 删除成员（文件权限）

```csharp
[Post("/cgi-bin/wedrive/file_acl_del")]
Task<WechatWorkResponse> DelFileAclAsync(
    [Body] DelWedriveFileAclRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/file_acl_del` |
| 官方文档 | 97888 / 97959 / 97922 |

**请求**：`fileid` + `auth_info`（均官方必填，可一次多个）。**响应**：无业务负载。

### E3. `SetFileSettingAsync` — 分享设置

```csharp
[Post("/cgi-bin/wedrive/file_setting")]
Task<WechatWorkResponse> SetFileSettingAsync(
    [Body] SetWedriveFileSettingRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/file_setting` |
| 官方文档 | 97889 / 97960 / 97923 |

**请求**（`SetWedriveFileSettingRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `fileid` | `Fileid` | `string?` | 官方必填 |
| `auth_scope` | `AuthScope` | `ulong?` | 官方必填：`1` 指定人 / `2` 企业内 / `3` 企业外 / `4` 企业内需管理员审批 / `5` 企业外需管理员审批（4、5 **仅管理员可设**） |
| `auth` | `Auth` | `ulong?` | 普通文档 `1` 仅浏览可下载 / `4` 仅预览（仅专业版）；微文档仅 `1`；**不填保持原状** |

**响应**：无业务负载。

### E4. `GetFileShareUrlAsync` — 获取分享链接

```csharp
[Post("/cgi-bin/wedrive/file_share")]
Task<GetWedriveFileShareUrlResponse> GetFileShareUrlAsync(
    [Body] GetWedriveFileShareUrlRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/file_share` |
| 官方文档 | 97890 / 97961 / 97924 |

**请求**：`fileid`（`string?`，官方必填）。

**响应**：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `share_url` | `ShareUrl` | `string?` | 文件分享链接（**区别于**空间邀请链接 `space_share_url`） |

> 链接是凭据：不得落日志；本域**无撤销分享端点**、有效期无留档（§7）。

### E5. `GetFilePermissionAsync` — 获取文件权限信息

```csharp
[Post("/cgi-bin/wedrive/get_file_permission")]
Task<GetWedriveFilePermissionResponse> GetFilePermissionAsync(
    [Body] GetWedriveFilePermissionRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/get_file_permission`（**不走 `wedrive/file_` 前缀**） |
| 官方文档 | 97891 / 97962 / 97925 |

**请求**：`fileid`（`string?`，官方必填）。

**响应**（`GetWedriveFilePermissionResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `share_range` | `ShareRange` | `WedriveFileShareRange?` | 分享范围（6 键） |
| `secure_setting` | `SecureSetting` | `WedriveFileSecureSetting?` | 文件安全配置（4 键） |
| `inherit_father_auth` | `InheritFatherAuth` | `WedriveFileInheritFatherAuth?` | 从父路径继承的权限 |
| `file_member_list` | `FileMemberList` | `List<WedriveSpaceAclMember>?` | 文档成员及其他授权列表（`fileid` 为文档时返回） |
| `co_auth_list` | `CoAuthList` | `List<WedriveSpaceAclMember>?` | 分享指定的部门 / 成员列表 |
| `watermark` | `Watermark` | `WedriveFileWatermark?` | 水印设置 |

**`share_range`**（`WedriveFileShareRange`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `enable_corp_internal` | `EnableCorpInternal` | `bool?` | 是否企业内可访问 |
| `corp_internal_auth` | `CorpInternalAuth` | `ulong?` | 企业内权限：`1` 仅浏览 / `4` 仅预览（仅专业版）/ **`255` 无权限或需审批** |
| `enable_corp_external` | `EnableCorpExternal` | `bool?` | 是否企业外可访问 |
| `corp_external_auth` | `CorpExternalAuth` | `ulong?` | 企业外权限（取值同上） |
| `corp_internal_approve_only_by_admin` | `CorpInternalApproveOnlyByAdmin` | `bool?` | 企业内是否管理员审批 |
| `corp_external_approve_only_by_admin` | `CorpExternalApproveOnlyByAdmin` | `bool?` | 企业外是否管理员审批 |

**`secure_setting`**（`WedriveFileSecureSetting`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `enable_readonly_copy` | `EnableReadonlyCopy` | `bool?` | 是否允许只读复制 |
| `modify_only_by_admin` | `ModifyOnlyByAdmin` | `bool?` | 是否仅管理员可修改 |
| `enable_readonly_comment` | `EnableReadonlyComment` | `bool?` | 是否开启只读评论 |
| `ban_share_external` | `BanShareExternal` | `bool?` | 是否禁止分享到企业外（**文件侧用 `ban_`**，与空间回显 `enable_share_external` 不同名） |

**`inherit_father_auth`**（`WedriveFileInheritFatherAuth`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `inherit` | `Inherit` | `bool?` | 是否继承父路径权限 |
| `auth_list` | `AuthList` | `List<WedriveSpaceAclMember>?` | 继承的成员授权列表（**返回示例形态**） |
| `member_list` | `MemberList` | `List<WedriveSpaceAclMember>?` | 文件夹 / 文档成员列表（**官方参数表形态**，示例未出现） |

> 官方参数表与返回示例在 `file_member_list`（obj vs 数组）与 `inherit_father_auth`（`member_list` vs `auth_list`）上各执一说 ⇒ 本仓**两个成员列表都承载**，按数组建模（WR5）。

**`watermark`**（`WedriveFileWatermark`）：`text` / `margin_type` / `show_visitor_name` / `show_text` / `force_by_admin` / `force_by_space_admin`，见 E6。

### E6. `SetFileSecureSettingAsync` — 修改文件安全设置

```csharp
[Post("/cgi-bin/wedrive/file_secure_setting")]
Task<WechatWorkResponse> SetFileSecureSettingAsync(
    [Body] SetWedriveFileSecureSettingRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/file_secure_setting` |
| 官方文档 | 97892 / 97965 / 97926 |

**请求**（`SetWedriveFileSecureSettingRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `fileid` | `Fileid` | `string?` | 官方必填；**仅在线文档类型** |
| `watermark` | `Watermark` | `WedriveFileWatermark?` | 水印设置；**仅 4 个可写字段** |

**`watermark` 可写 / 只读分野**

| 字段（JSON） | 属性 | 类型 | 写入口 | 说明 |
|---|---|---|---|---|
| `text` | `Text` | `string?` | ✅ | 水印文本 |
| `margin_type` | `MarginType` | `ulong?` | ✅ | `1` 低密度 / `2` 高密度 |
| `show_visitor_name` | `ShowVisitorName` | `bool?` | ✅（仅专业版） | 是否显示访问人名称 |
| `show_text` | `ShowText` | `bool?` | ✅ | 是否展示水印文本 |
| `force_by_admin` | `ForceByAdmin` | `bool?` | ❌ 仅回显 | 管理员是否强制使用水印 |
| `force_by_space_admin` | `ForceBySpaceAdmin` | `bool?` | ❌ 仅回显 | 空间管理员是否强制使用水印 |

**响应**：无业务负载。

**调用示例**（加成员 → 设分享范围 → 取链接 → 读权限详情 → 设水印 → 删成员）

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

var fileAcl = provider.GetRequiredService<IWechatWorkInternalWedriveFileAclService>();
string fileId = "FILEID-FAKE-PLACEHOLDER";

// ① 增量加成员：文件侧 auth 只有 1（仅浏览 / 仅下载）
var added = await fileAcl.AddFileAclAsync(
    new AddWedriveFileAclRequest
    {
        Fileid = fileId,
        AuthInfo =
        [
            new WedriveSpaceAclMember { Type = 1, Userid = "staff_0001", Auth = 1 },
        ],
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(added, "/cgi-bin/wedrive/file_acl_add");

// ② 分享范围：4 / 5（需管理员审批）仅有管理员时可设；auth 不填保持原状
var setting = await fileAcl.SetFileSettingAsync(
    new SetWedriveFileSettingRequest
    {
        Fileid = fileId,
        AuthScope = 2,          // 企业内
        Auth = 1,               // 普通文档：仅浏览（可下载）
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(setting, "/cgi-bin/wedrive/file_setting");

// ③ 分享链接：凭据，只记「已取到」
var share = await fileAcl.GetFileShareUrlAsync(
    new GetWedriveFileShareUrlRequest { Fileid = fileId },
    cancellationToken);
WechatWorkException.ThrowIfFailed(share, "/cgi-bin/wedrive/file_share");
logger.LogInformation("分享链接已获取：{HasUrl}", !string.IsNullOrEmpty(share.ShareUrl));

// ④ 读权限详情：路由不走 wedrive/file_ 前缀；file_member_list 等按数组承载
var perm = await fileAcl.GetFilePermissionAsync(
    new GetWedriveFilePermissionRequest { Fileid = fileId },
    cancellationToken);
WechatWorkException.ThrowIfFailed(perm, "/cgi-bin/wedrive/get_file_permission");
logger.LogInformation("企业内权限 {InternalAuth}，成员 {Members} 条，继承 {Inherit}",
    perm.ShareRange?.CorpInternalAuth,
    perm.FileMemberList?.Count ?? 0,
    perm.InheritFatherAuth?.Inherit);

// ⑤ 安全设置：仅在线文档；水印只开放 4 个写字段
var secured = await fileAcl.SetFileSecureSettingAsync(
    new SetWedriveFileSecureSettingRequest
    {
        Fileid = fileId,
        Watermark = new WedriveFileWatermark
        {
            Text = "内部资料",
            MarginType = 2,             // 高密度
            ShowText = true,
            ShowVisitorName = false,
            // ForceByAdmin / ForceBySpaceAdmin 为只读回显，写入口不开放
        },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(secured, "/cgi-bin/wedrive/file_secure_setting");

// ⑥ 删除成员：只靠 type + userid 定位
var removed = await fileAcl.DelFileAclAsync(
    new DelWedriveFileAclRequest
    {
        Fileid = fileId,
        AuthInfo = [new WedriveSpaceAclMember { Type = 1, Userid = "staff_0001" }],
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(removed, "/cgi-bin/wedrive/file_acl_del");
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| E5 路由 | `get_file_permission` **不带** `wedrive/file_` 前缀 | 按前缀拼 ⇒ 404 |
| `share_url` vs `space_share_url` | 文件分享 vs 空间邀请 | 混用 ⇒ 拿错分享面 |
| 文件 `auth` vs 空间 `auth` | 文件只 `1`；空间有 1 / 4 / 7 | 搬空间值 ⇒ 失败 |
| `auth_scope` 4 / 5 | 仅管理员可设 | 普通成员调用 ⇒ 失败 |
| E6 适用范围 | **仅在线文档** | 对普通文件 / 文件夹调用 ⇒ 失败 |
| `watermark` 写面 | 只 4 字段；`force_*` 是回显 | 写 `force_by_admin` ⇒ 被忽略 |
| `show_visitor_name` | 仅专业版支持 | 非专业版设置 ⇒ 失败 |
| `file_member_list` 形态 | 官方参数表列 obj、示例为数组 | 按 obj 建模型 ⇒ 反序列化失败 |
| `inherit_father_auth` | 参数表 `member_list`、示例 `auth_list` | 只建一个 ⇒ 漏数据 |
| 文件侧字段名 | `ban_share_external`（回显也用 `ban_`） | 套空间回显的 `enable_share_external` ⇒ 拿不到 |
| `corp_internal_auth = 255` | 表示「无权限或需审批」 | 当普通权限值比较 ⇒ 语义错 |
| 成员结构 | 复用空间成员类型 | 另建类型 ⇒ 守卫打红 |
| 权限端点语义 | 增量加 / 删 | 当整体替换 ⇒ 误删 |
| HTTP 动词 | 六条**全部 POST** | 改 GET ⇒ 404/405 |

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 请求成功 | 判 `IsSuccess` |
| 40014 / 42001 / 42007 | 令牌失效码 | 应用令牌类错误 | 统一失效码处理 |
| 42011 | `InvalidProviderAccessToken` | 服务商令牌不合法 / 过期 | 第三方 / 代开发链路 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，装配缺陷 |

非在线文档调 E6、非管理员设审批范围、文件不存在、`auth` 传非 1 的具体业务码以官方页为准（仓内未逐项核验）。其余见[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90312）（90312）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 分享链接有效期与撤销面 | 只返回 `share_url`；本域 31 条路由无撤销端点 | 官方 97890 |
| `file_member_list` / `co_auth_list` / `inherit_father_auth` 的权威形态 | 官方参数表与示例互相矛盾 | 官方 97891 |
| `corp_*_auth` 除 1 / 4 / 255 外是否还有值 | 未成文 | 官方 97891 |
| E1/E2 单次成员上限 | 只写「可一次多个」 | 官方 93658 / 97888 |
| 文件安全设置可写面 | 回显 4 键，写入口只有 `watermark` | 官方 97892 |
| 本族频率 / 并发上限 | 无任何数值 | 官方各页 + 90312 |
| 真账号验证 | 本批仅内部证据核验 | — |

## 8. 延伸阅读

- 同域兄弟：[File.md](./File.md)（文件本体）· [SpacePermission.md](./SpacePermission.md)（空间邀请链接 `space_share_url`）· [Space.md](./Space.md)
- 邻域：[../Wedoc/DocPermission.md](../Wedoc/DocPermission.md)（文档成员与安全设置是**另一套 priv 口径**，勿套用）
- 域总览：[Wedrive README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/README.md)（第 8 节）
- 官方页：[新增成员 93658](https://developer.work.weixin.qq.com/document/path/93658) · [分享设置 97889](https://developer.work.weixin.qq.com/document/path/97889) · [获取文件权限信息 97891](https://developer.work.weixin.qq.com/document/path/97891) · [修改文件安全设置 97892](https://developer.work.weixin.qq.com/document/path/97892)
