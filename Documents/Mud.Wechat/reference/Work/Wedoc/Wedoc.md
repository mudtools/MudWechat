# 文档 · 新建 / 重命名 / 删除 / 基础信息 / 分享

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Wedoc` 域 · 管理文档族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkWedocService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/IWechatWorkWedocService.cs)（父，承载全部 5 支）＋ 自建 [IWechatWorkWedocService_Internal.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/IWechatWorkWedocService_Internal.cs) / 第三方 [IWechatWorkWedocService_ThirdParty.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/IWechatWorkWedocService_ThirdParty.cs) / 代开发 [IWechatWorkWedocService_Provider.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/IWechatWorkWedocService_Provider.cs)（三个零端点类型化子接口） |
| DTO 源文件 | [DataModels/Wedoc/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedoc/) 与 [RequestModel/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedoc/RequestModel/)（本族 9 个文件 / 9 型，`SerializerClassName = Wedoc` → `WedocJsonContext`） |
| 契约守卫 | [WechatWedocContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWedocContractGuards.cs)（WD1 本族 5 路由 / 九族 69 端点总表 / 层级与开放面 / 令牌绑定 / DTO 292 型登记）＋ `WechatContractGuards` G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | 新建文档 [97460](https://developer.work.weixin.qq.com/document/path/97460) / [97464](https://developer.work.weixin.qq.com/document/path/97464) / [97470](https://developer.work.weixin.qq.com/document/path/97470)；重命名 [97736](https://developer.work.weixin.qq.com/document/path/97736) / [97745](https://developer.work.weixin.qq.com/document/path/97745) / [97740](https://developer.work.weixin.qq.com/document/path/97740)；删除 [97735](https://developer.work.weixin.qq.com/document/path/97735) / [97746](https://developer.work.weixin.qq.com/document/path/97746) / [97742](https://developer.work.weixin.qq.com/document/path/97742)；基础信息 [97734](https://developer.work.weixin.qq.com/document/path/97734) / [97747](https://developer.work.weixin.qq.com/document/path/97747) / [97743](https://developer.work.weixin.qq.com/document/path/97743)；分享 [97733](https://developer.work.weixin.qq.com/document/path/97733) / [97748](https://developer.work.weixin.qq.com/document/path/97748) / [97744](https://developer.work.weixin.qq.com/document/path/97744)（三档文档号顺序为自建 / 第三方 / 代开发） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：WD1 路由表 + 层级/令牌/DTO 断言 + 5 支端点 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/README.md) 逐端点核验（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

企业微信「文档」域共九族 69 支端点，本族是其中**唯一负责文档「生命周期」**的一族——**建、改名、删、查基础信息、拿分享链接**：

| 端点 | 用途 | 官方路由 |
|---|---|---|
| E1 `CreateDocumentAsync` | 新建文档 / 表格 / 智能表格 / 智能文档 | `wedoc/create_doc` |
| E2 `RenameDocumentAsync` | 重命名文档或收集表 | `wedoc/rename_doc` |
| E3 `DeleteDocumentAsync` | 删除文档 / 表格 / 智能表格 / 收集表 | `wedoc/del_doc` |
| E4 `GetDocumentBaseInfoAsync` | 读文档基础信息（名称、创建/修改时间、类型） | `wedoc/get_doc_base_info` |
| E5 `ShareDocumentAsync` | 取文档或收集表的分享链接 | `wedoc/doc_share` |

**本族是本域所有后续调用的起点**：官方**只在 E1 的响应里返回 `docid`**，其后八族（文档内容、表格内容、智能表格内容、智能文档内容、文档权限、收集表、素材、高级功能账号）全部以 `docid` / `formid` 定位对象，**本域没有「按空间节点反查 docid」的端点**（反查能力在 Wedrive 邻域）。丢了 `docid` 等于丢了这把钥匙。

与其它族的分界（`doc_type` 与族一一对应，不可交叉调用）：

| `doc_type` | 文档种类 | 创建方式 | 内容操作族 |
|---|---|---|---|
| `3` | 文档 | 本族 E1 | [Document.md](./Document.md) |
| `4` | 表格 | 本族 E1 | [Spreadsheet.md](./Spreadsheet.md) |
| `10` | 智能表格 | 本族 E1 | [SmartSheet.md](./SmartSheet.md) |
| `11` | 智能文档 | 本族 E1 | [SmartDoc.md](./SmartDoc.md) |
| — | 收集表 | **不走 E1**，走 [Form.md](./Form.md) 的创建端点 | [Form.md](./Form.md) |

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider`（三类均开放本族） | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方 / 代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`（`WechatTokenManagerKeys.InternalAccessToken`）；第三方 / 代开发 `Wechat.AccessToken@Corp`（`CorpAccessToken`） | 运行期归属校验，错配抛 `WechatTokenOwnerMismatchException` |
| 企业作用域 | 第三方 / 代开发**必须**先 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)` | 调用前 |
| 注入模式 | `TokenInjectionMode.Query`（MUD005 已接受风险：官方强制 Query，无法改 Header） | 每次请求 |

自建应用示例的装配按本表；第三方与代开发的令牌是**企业级、一企一份**（`scope = authCorpId`），不切作用域会在归属校验处 fail-fast——这不是可选的优化项。`permanent_code` 与 `AgentSecret` 只写参数名，不得进日志、遥测或异常消息。

### 2.2 权限与业务前置

- **自建应用**：须使用配置到「可调用应用」列表中的应用 secret 所获取的 `access_token` 调用（接口 remarks 原文）；**从 2023 年 12 月 1 日 0 点起不再支持系统应用 secret**（存量企业暂不受影响）。
- **第三方 / 代开发**：应用须具有「**文档**」权限。
- **可操作范围硬约束**：文档相关操作**只能作用于该应用自己创建的文档 / 收集表**——不是「应用可见的文档」，也不是「管理员授权的文档」。别人的文档即使管理员可见也拿不到。
- 企业侧开关：企业须**开启 API 功能**（未开启报 `640017`）且**管理员授予 API 编辑权限**（未授予报 `640018`）；调用者无管理权限报 `640008`。
- **`docid` 只在 E1 返回**：官方明示「需妥善保存」；本域无按空间节点反查的端点。
- 创建位置：E1 的 `spaceid` 与 `fatherid` 可选；**填了 `spaceid` 就必须同时填 `fatherid`**（`fileid`）。不填则落在默认位置。
- 命名长度：E1 的 `doc_name` 最多 255 字符、超出**截断**；E2 的 `new_name` 上限 255 且**英文算 1、汉字算 2**，超出同样截断——两处计数口径不同（见 §5）。
- 频率：官方三页**未给本族独立数值频率**，走全局访问频率限制（每企业单接口 1 万次/分、15 万次/小时一类）；具体数值以官方访问频率限制页为准（§7 列入缺口）。
- **数据安全**：`doc_name` 可能含业务信息，`share_url` 是带访问语义的链接——调用日志只记 `docid` 摘要与是否成功，`share_url`、`doc_name` 原文不要落日志或遥测。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 | 官方文档页（自建 / 第三方 / 代开发） |
|---|---|---|---|---|
| E1 新建文档 | ✅ | ✅ | ✅ | 97460 / 97464 / 97470 |
| E2 重命名文档 | ✅ | ✅ | ✅ | 97736 / 97745 / 97740 |
| E3 删除文档 | ✅ | ✅ | ✅ | 97735 / 97746 / 97742 |
| E4 获取文档基础信息 | ✅ | ✅ | ✅ | 97734 / 97747 / 97743 |
| E5 分享文档 | ✅ | ✅ | ✅ | 97733 / 97748 / 97744 |

> 三类应用在本族是**同一组端点、同一套请求/响应**（官方为每类应用各开一页，内容对齐），故父接口承载全部 5 支、三个子接口都是零差异空标记。三个子接口都**可注入 DI**，按 `AppType` 选型；父接口 `IsAbstract = true` 不注册。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkWedocService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **5** | 否 |
| `IWechatWorkInternalWedocService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyWedocService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderWedocService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

- 模块注册：`WechatModule.Wedoc` / `AddWedocApi()`；注册组名 `Wedoc`（本域九族的九个实现类共用这一组名，WD4 锁定）。
- 子接口 `InheritedFrom = nameof(WechatWorkWedocService)`，端点声明全部在父接口，子接口 `GetMethods(DeclaredOnly)` 恒为空。
- 同域其它八族各有独立父接口与注册入口，见 §8。

## 4. 端点

### E1. `CreateDocumentAsync` — 新建文档

```csharp
[Post("/cgi-bin/wedoc/create_doc")]
Task<CreateWedocDocumentResponse> CreateDocumentAsync(
    [Body] CreateWedocDocumentRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/wedoc/create_doc` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`CreateWedocDocumentRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Spaceid` | `spaceid` | `string?` | 否 | 目标空间 id；**填了就须同时填 `Fatherid`** |
| `Fatherid` | `fatherid` | `string?` | 否 | 父目录 `fileid`；根目录时填空间 `spaceid`。与 `Spaceid` 成对出现 |
| `DocType` | `doc_type` | `long?` | 是 | 3 文档 / 4 表格 / 10 智能表格 / 11 智能文档；**收集表不支持走本端点** |
| `DocName` | `doc_name` | `string?` | 是 | 文档名；官方最多 255 字符，超出**截断**（不报错） |
| `AdminUsers` | `admin_users` | `List<string>?` | 否 | 文档管理员 `userid` 列表；个人数据，示例用假值、不落日志 |

**响应参数**（`CreateWedocDocumentResponse : WechatWorkResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `url` | `Url` | `string?` | 新建文档的访问链接 |
| `docid` | `Docid` | `string?` | **文档 id，仅创建时返回**；本域后续全部调用靠它 |

**本端点注意事项**

- `docid` 是后续所有调用的定位键，**必须持久化**；官方不提供「按名称重查 `docid`」的接口。
- `doc_type` 决定后续可调用的内容族：选 3 就只能用 [Document.md](./Document.md)，选 10 就只能用 [SmartSheet.md](./SmartSheet.md)，类型错配在应答侧以 `640054` 一类业务码暴露。
- `doc_name` 超长是**截断**而非报错：写完立刻读回 E4 校验名称是否如预期，是低成本的兜底手段。
- 新建的智能表格**没有视图、字段与记录**（[SmartSheet.md](./SmartSheet.md) 的 E1 会再补子表），新建的智能文档**没有页面**——不要把「创建成功」当成「可用」。

**调用示例**（自建应用：建文档 → 存 `docid` → 读回基础信息 → 取分享链接）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Wedoc;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "wedoc-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddWedocApi());

// 第三方 / 代开发改为解析 IWechatWorkThirdPartyWedocService / IWechatWorkProviderWedocService，
// 并在调用前用 switcher.UseCorpScope(appKey, authCorpId, permanentCode) 进入目标企业作用域。
var wedoc = provider.GetRequiredService<IWechatWorkInternalWedocService>();

// ① 新建文档：doc_type 3=文档 / 4=表格 / 10=智能表格 / 11=智能文档；doc_name ≤255 字符（超长截断不报错）
var created = await wedoc.CreateDocumentAsync(
    new CreateWedocDocumentRequest
    {
        DocType = 3,
        DocName = "示例项目说明",
        AdminUsers = ["staff_0001"],          // userid 属个人数据：假值，且不要写日志
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(created, "/cgi-bin/wedoc/create_doc");
string docId = created.Docid ?? throw new InvalidOperationException("响应未返回 docid");
// docid 只在创建时返回 ⇒ 立即持久化（docId 落库）；本域没有反查端点

// ② 读回基础信息：create_time / modify_time 为秒级时间戳；doc_type 为 3/4/10
var info = await wedoc.GetDocumentBaseInfoAsync(
    new GetWedocDocumentBaseInfoRequest { Docid = docId }, cancellationToken);
WechatWorkException.ThrowIfFailed(info, "/cgi-bin/wedoc/get_doc_base_info");
logger.LogInformation("文档 {DocId} 类型 {DocType} 创建于 {CreateTime}",
    info.DocBaseInfo?.Docid, info.DocBaseInfo?.DocType, info.DocBaseInfo?.CreateTime);

// ③ 取分享链接：share_url 带访问语义，只转交用户侧，不打印明文
var shared = await wedoc.ShareDocumentAsync(
    new ShareWedocDocumentRequest { Docid = docId }, cancellationToken);
WechatWorkException.ThrowIfFailed(shared, "/cgi-bin/wedoc/doc_share");
logger.LogInformation("分享链接已就绪（长度 {Length}）", shared.ShareUrl?.Length ?? 0);
```

### E2. `RenameDocumentAsync` — 重命名文档

```csharp
[Post("/cgi-bin/wedoc/rename_doc")]
Task<WechatWorkResponse> RenameDocumentAsync(
    [Body] RenameWedocDocumentRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/wedoc/rename_doc` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`RenameWedocDocumentRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 二选一 | 文档 id；**与 `formid` 只能填其中一个**；仅可改应用自己创建的文档 |
| `Formid` | `formid` | `string?` | 二选一 | 收集表 id；与 `docid` 互斥 |
| `NewName` | `new_name` | `string?` | 是 | 新名称；最多 255 字符，**英文算 1、汉字算 2**，超出截断 |

**响应参数**：仅 `errcode` / `errmsg`（`WechatWorkResponse`，无业务负载）。

**本端点注意事项**

- `docid` / `formid` 互斥是**官方口径**，SDK 不做本地校验；两个都填的后果由官方业务码表达。
- 长度计数与 E1 **不同**：E1 按字符（255 字符），E2 按「英文 1 / 汉字 2」计权（255 权重）——同一名称在两处可能一个被接受、一个被截断。
- 只能改**应用自己创建**的文档 / 收集表，别人共享给你的文档改了会报业务错。

### E3. `DeleteDocumentAsync` — 删除文档

```csharp
[Post("/cgi-bin/wedoc/del_doc")]
Task<WechatWorkResponse> DeleteDocumentAsync(
    [Body] DeleteWedocDocumentRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/wedoc/del_doc`（官方路由就是 `del_doc`，**不是** `delete_doc`） |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`DeleteWedocDocumentRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 二选一 | 文档 / 表格 / 智能表格 id；与 `formid` 只能填一个；仅可删应用自建对象 |
| `Formid` | `formid` | `string?` | 二选一 | 收集表 id；与 `docid` 互斥 |

**响应参数**：仅 `errcode` / `errmsg`。

**本端点注意事项**

- 路由拼写是 `del_doc`：写成 `delete_doc` 会 404。
- 删除是**不可逆**操作，且失败也是 HTTP 200 + 非 0 `errcode`——调用前必须判 `IsSuccess`，不要用「没抛异常」当成功依据。
- 命中范围包括表格与智能表格；收集表走 `formid`，**不要**用 E1 创建收集表的替代路径。

### E4. `GetDocumentBaseInfoAsync` — 获取文档基础信息

```csharp
[Post("/cgi-bin/wedoc/get_doc_base_info")]
Task<GetWedocDocumentBaseInfoResponse> GetDocumentBaseInfoAsync(
    [Body] GetWedocDocumentBaseInfoRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/wedoc/get_doc_base_info` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`GetWedocDocumentBaseInfoRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 文档 id；**只认 `docid`**（无 `formid` 槽位） |

**响应参数**（`GetWedocDocumentBaseInfoResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `doc_base_info` | `DocBaseInfo` | `WedocDocBaseInfo?` | 基础信息对象 |

`WedocDocBaseInfo` 元素：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `docid` | `Docid` | `string?` | 文档 id |
| `doc_name` | `DocName` | `string?` | 文档名 |
| `create_time` | `CreateTime` | `long?` | 创建时间（秒级时间戳） |
| `modify_time` | `ModifyTime` | `long?` | 最后修改时间（秒级时间戳） |
| `doc_type` | `DocType` | `long?` | 文档类型；DTO 注释列出 3 文档 / 4 表格 / 10 智能表格（智能文档 `11` 未在响应注释中列出 ⇒ §7） |

**本端点注意事项**

- 这是本族**唯一的读接口**，也是「写后校验」的落点：E1 名称被截断、E2 改名结果、时间戳语义都以本端点为准。
- `docid` 不存在或不属于本应用时报业务错（不是空响应），判 `errcode` 而非判字段是否为空。
- 时间字段是**秒级** `long`，不是 ISO 字符串；需要展示时自行按 UTC+8 格式化。

### E5. `ShareDocumentAsync` — 分享文档

```csharp
[Post("/cgi-bin/wedoc/doc_share")]
Task<ShareWedocDocumentResponse> ShareDocumentAsync(
    [Body] ShareWedocDocumentRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/wedoc/doc_share`（名词路由，非动词 `share_doc`） |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`ShareWedocDocumentRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 二选一 | 文档 / 表格 / 智能表格 / 智能文档 id；与 `formid` 互斥；只能取应用自建文档 |
| `Formid` | `formid` | `string?` | 二选一 | 收集表 id；与 `docid` 互斥 |

**响应参数**（`ShareWedocDocumentResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `share_url` | `ShareUrl` | `string?` | 文档分享链接 |

**本端点注意事项**

- `share_url` 是**带访问语义的资源地址**：只允许转交用户侧打开，禁止写日志、遥测或长期入库。
- 只能访问**该应用创建的文档**；别人共享给你的文档拿不到分享链接。
- 链接有效期与鉴权方式官方页未在仓内留档（§7）。

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| E3 路由 `del_doc` vs `delete_doc` | 官方就是 `del_doc` | 404 |
| `docid` vs `formid` | E2/E3/E5 二选一，E4 只认 `docid` | 两个都填 / E4 塞 `formid` ⇒ 业务错 |
| 「文档」≠「收集表」 | 收集表**不能**用 E1 创建，走 [Form.md](./Form.md) | `doc_type` 填了也不生效 / 建出错误类型 |
| 名称长度口径 | E1 按字符 255；E2 按「英文 1 / 汉字 2」255 | 同一名称一处成功一处被截断 |
| 超长是截断不是报错 | 两处都**截断** | 静默改名 / 静默变短，必须读回校验 |
| `docid` 来源 | 只在 E1 返回，无反查端点 | 不持久化就只能重建文档 |
| 自建 vs 第三方令牌 | 第三方 / 代开发是企业级令牌，须先 `UseCorpScope` | 未切作用域抛 `WechatTokenOwnerMismatchException` |
| 可操作范围 | 仅应用**自己创建**的对象 | 管理员可见 ≠ 应用可操作，报 `640008` 一类错 |
| `doc_type` 与内容族 | 3/4/10/11 分别对 Document / Spreadsheet / SmartSheet / SmartDoc | 类型错配以 `640054` 暴露 |
| 「创建成功」≠「可用」 | 智能表格无字段、智能文档无页面 | 直接读内容拿空结果 |
| 令牌 Query | 官方强制 `access_token` 走 Query（MUD005） | 改成 Header 会鉴权失败 |
| `share_url` 处理 | 带访问语义的临时链接 | 落日志 / 入库即泄露面 |

SDK 侧通用约束：不自动判错、不做本地必填与互斥校验、不自动重试；`errcode != 0` 时 HTTP 仍是 200。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 请求成功 | 判 `IsSuccess` |
| 640017 | 域 README 留档 | 企业未开启 API 功能 | 需企业管理员在后台开启 |
| 640018 | 域 README 留档 | 管理员未授予 API 编辑权限 | 需管理员授权 |
| 640008 | 域 README 留档 | 调用者无管理权限 | 换 secret / 补权限 |
| 640054 | 域 README 留档 | `docid` 不是智能文档类型（类型错配族） | 核对 `doc_type` 与内容族 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 应用令牌类错误 | 按统一失效码处理 |
| 42011 | `InvalidProviderAccessToken` | 服务商令牌不合法 / 过期 | 失效码集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

本族接口注释未留档**专属**错误码：`docid` 不存在、`spaceid`/`fatherid` 不成对、名称非法等失败由官方业务码表达，码值以官方页为准（仓内未逐项核验）。其余错误码见官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）；仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| `doc_base_info.doc_type` 是否含 `11` | DTO 注释只列 3/4/10，智能文档为 11 ⇒ 响应侧口径未核验 | 官方 97734 |
| 按名称 / 空间节点反查 `docid` | 本域无此端点；Wedrive 邻域是否覆盖未逐项核验 | [Wedrive](../Wedrive/) |
| `spaceid` / `fatherid` 取值来源 | 空间 id 与目录 `fileid` 的获取入口不在本域 | Wedrive 域文档 |
| 本族频率限制 | 官方三页未给独立数值频率 ⇒ 走全局 | 90454 |
| `share_url` 有效期与鉴权 | 链接多久失效、是否需要成员身份 ⇒ 未核验 | 官方 97733 |
| `admin_users` 数量上限 | E1 管理员列表是否有条数限制 ⇒ 未核验 | 官方 97460 |
| 删除是否可恢复 | 官方文档树内是否有回收站 / 恢复接口 ⇒ 未核验 | 官方 97735 |
| 失败时的 `errcode` 全集 | `docid` 不存在、参数不成对的具体码值 ⇒ 未留档 | 官方三页 + 90455 |
| 真账号验证 | 本批仅内部证据核验，未对真企业调用 | — |

## 8. 延伸阅读

- 同域内容族：[Document.md](./Document.md)（文档内容）· [Spreadsheet.md](./Spreadsheet.md)（表格）· [SmartSheet.md](./SmartSheet.md)（智能表格）· [SmartDoc.md](./SmartDoc.md)（智能文档）
- 同域支撑族：[DocPermission.md](./DocPermission.md)（文档权限，`doc_get_auth` 预检）· [Form.md](./Form.md)（收集表）· [Material.md](./Material.md)（素材上传）· [Vip.md](./Vip.md)（高级功能账号，仅自建）
- 邻域：[Wedrive](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/)（微盘：空间、文件、权限；`docid` 反查面在此侧）
- 域总览：[Wedoc README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/README.md)（九族 69 端点、定位键、容量闸、拼写陷阱）
- 凭据与多应用：[AGENTS.md §5.1](../../../../../AGENTS.md)
- 官方页：[新建文档 97460](https://developer.work.weixin.qq.com/document/path/97460) · [重命名 97736](https://developer.work.weixin.qq.com/document/path/97736) · [删除 97735](https://developer.work.weixin.qq.com/document/path/97735) · [基础信息 97734](https://developer.work.weixin.qq.com/document/path/97734) · [分享 97733](https://developer.work.weixin.qq.com/document/path/97733)
