# 文档内容 · 读取节点树与批量编辑

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Wedoc` 域 · 管理文档内容族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkWedocDocumentService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/Document/IWechatWorkWedocDocumentService.cs)（父，承载全部 2 支）＋ 自建 [IWechatWorkWedocDocumentService_Internal.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/Document/IWechatWorkWedocDocumentService_Internal.cs) / 第三方 [_ThirdParty.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/Document/IWechatWorkWedocDocumentService_ThirdParty.cs) / 代开发 [_Provider.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/Document/IWechatWorkWedocDocumentService_Provider.cs)（三个零端点子接口） |
| DTO 源文件 | [DataModels/Wedoc/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedoc/) 与 [RequestModel/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedoc/RequestModel/)（本族请求/响应 2 型 + 文档节点树与操作体 30 余型，`SerializerClassName = Wedoc` → `WedocJsonContext`） |
| 契约守卫 | [WechatWedocContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWedocContractGuards.cs)（WD2 本族 2 路由 / 九族 69 端点总表 / 层级与令牌 / DTO 292 型登记 + 拼写陷阱断言 `verison` / `blod`）＋ G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | 编辑文档内容 [97626](https://developer.work.weixin.qq.com/document/path/97626) / [98027](https://developer.work.weixin.qq.com/document/path/98027) / [98034](https://developer.work.weixin.qq.com/document/path/98034)；获取文档数据 [101161](https://developer.work.weixin.qq.com/document/path/101161) / [101188](https://developer.work.weixin.qq.com/document/path/101188) / [101170](https://developer.work.weixin.qq.com/document/path/101170)（顺序为自建 / 第三方 / 代开发） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：WD2 路由表 + DTO 拼写陷阱断言 + 2 支端点 XML 注释 + 操作体 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/README.md) 逐端点核验（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

`doc_type = 3` 的**文档**（Word 形态）内容读写只有两支，构成「先读树、再改」的两步闭环：

| 端点 | 用途 | 读/写 |
|---|---|---|
| E1 `GetDocumentDataAsync` | 取文档版本号 + 内容根节点 `Node` 树 | 读 |
| E2 `BatchUpdateDocumentAsync` | 一次提交 ≤30 个编辑操作（替换/插入/删除/插图/分页/表格/段落/文本属性） | 写 |

**顺序不可颠倒**：官方明示「每次更新前需先调用获取文档数据接口获取各节点最新位置」。文档没有「绝对坐标」，所有编辑操作都按 `index`（从 0 开始的线性位置）定位，而这个位置由 E1 的节点树 `begin` / `end` 给出。

与其它族的边界：本族**只对 `doc_type = 3` 的文档有效**；表格是 [Spreadsheet.md](./Spreadsheet.md)、智能表格是 [SmartSheet.md](./SmartSheet.md)、智能文档是 [SmartDoc.md](./SmartDoc.md)，三者的内容模型完全不同、不可互调。文档的**生命周期**（建/删/改名）在 [Wedoc.md](./Wedoc.md)。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方 / 代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp` | 运行期归属校验，错配抛 `WechatTokenOwnerMismatchException` |
| 企业作用域 | 第三方 / 代开发必须先 `UseCorpScope(appKey, authCorpId, permanentCode)` | 调用前 |

### 2.2 权限与业务前置

- **自建应用**：须使用配置到「可调用应用」列表中的应用 secret 所获取的 `access_token` 调用。
- **第三方 / 代开发**：应用须具有「**文档**」权限。
- 可编辑范围：**仅该应用自己创建的文档**；企业需开启 API 功能、管理员需授予 API 编辑权限。
- `docid` 必须来自 [Wedoc.md](./Wedoc.md) E1 的创建响应并已持久化，**本域无按名称反查入口**。
- 版本闸：E2 的 `verison` 与文档最新版本**相差不能超过 100 版**；不传则由服务端按最新版本执行（仍有并发风险）。
- 批量闸：单次 `requests` **≤30 个操作**；**一处失败整批不生效**（全或无）。
- 规模闸（E2 内）：插入表格**行 ≤100、列 ≤60、单元格总数 ≤1000**；`ranges`（替换/更新属性）个数 **≤10**。
- **数据安全**：文档正文、节点文本都是业务内容；调用日志只记 `docid`、版本号与操作数量，**不得打印 `text` / `text_property` / 节点树**。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 | 官方文档页（自建 / 第三方 / 代开发） |
|---|---|---|---|---|
| E1 获取文档数据 | ✅ | ✅ | ✅ | 101161 / 101188 / 101170 |
| E2 编辑文档内容 | ✅ | ✅ | ✅ | 97626 / 98027 / 98034 |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkWedocDocumentService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **2** | 否 |
| `IWechatWorkInternalWedocDocumentService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyWedocDocumentService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderWedocDocumentService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

- 注册组 `Wedoc`，装配入口 `AddWedocApi()`（与 [Wedoc.md](./Wedoc.md) 共用同一注册组），模块枚举 `WechatModule.Wedoc`。
- 子接口 `InheritedFrom = nameof(WechatWorkWedocDocumentService)`；端点全在父接口声明。

## 4. 端点

### E1. `GetDocumentDataAsync` — 获取文档数据

```csharp
[Post("/cgi-bin/wedoc/document/get")]
Task<GetWedocDocumentDataResponse> GetDocumentDataAsync(
    [Body] GetWedocDocumentDataRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/wedoc/document/get`（官方即 POST） |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`GetWedocDocumentDataRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 文档 id（`doc_type = 3`） |

**响应参数**（`GetWedocDocumentDataResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `version` | `Version` | `long?` | 当前文档版本号；作为 E2 `verison` 的输入 |
| `document` | `Document` | `WedocDocumentNode?` | 内容根节点（整棵 `Node` 树） |

`WedocDocumentNode` 元素：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `begin` | `Begin` | `long?` | 节点起始位置（从 0 开始） |
| `end` | `End` | `long?` | 节点结束位置 |
| `type` | `Type` | `string?` | `Document` 文档 / `MainStory` 主节点 / `Section` 节 / `Paragraph` 段落 / `Table` 表格 / `TableRow` 行 / `TableCell` 单元格 / `Text` 文本容器 / `Drawing` 图形对象 |
| `text` | `Text` | `string?` | 仅 `Text` 类型节点有值 |
| `property` | `Property` | `WedocDocumentProperty?` | 节点属性（对齐 / 缩进 / 段落属性 / 文本属性等，按节点类型取用） |
| `children` | `Children` | `List<WedocDocumentNode>?` | 子节点 |

**本端点注意事项**

- 树是**线性位置视图**：`begin` / `end` 是编辑操作 `index` 的唯一合法来源，不要自己按字符数推算。
- `version` 用于 E2 的 `verison`：两者相差 >100 版 E2 会失败；写完再读会拿到新版本号。
- 返回的是**整棵文档树**，长文档响应很大；需要频繁编辑时按「读一次、批量改多次（每次 ≤30 操作）」组织，不要每个操作前都重读。

### E2. `BatchUpdateDocumentAsync` — 编辑文档内容

```csharp
[Post("/cgi-bin/wedoc/document/batch_update")]
Task<WechatWorkResponse> BatchUpdateDocumentAsync(
    [Body] BatchUpdateWedocDocumentRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/wedoc/document/batch_update` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`BatchUpdateWedocDocumentRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 文档 id |
| `Verison` | `verison` | `long?` | 否 | 基于哪个版本修改（取 E1 的 `version`）；**官方示例拼写 `verison`**，本仓照抄；与最新版本差 ≤100 |
| `Requests` | `requests` | `List<WedocDocumentUpdateOperation>?` | 是 | 操作列表，**≤30 个**；**一处失败整批不生效** |

**操作体**（`WedocDocumentUpdateOperation`）：八个字段**互斥**，一个对象只填一个（填多个仅一个生效，SDK 不拦截）

| JSON 键 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `replace_text` | `ReplaceText` | `WedocDocumentReplaceText` | 替换文本；`text` + `ranges`（≤10 个 `{start_index, length}`） |
| `insert_text` | `InsertText` | `WedocDocumentInsertText` | 插入文本；`text` + `location.index` |
| `delete_content` | `DeleteContent` | `WedocDocumentDeleteContent` | 删除内容；`range` = `{start_index, length}` |
| `insert_image` | `InsertImage` | `WedocDocumentInsertImage` | 插图；`image_id`（来自 [Material.md](./Material.md) 上传）+ `location` + `width` / `height` |
| `insert_page_break` | `InsertPageBreak` | `WedocDocumentInsertPageBreak` | 插入分页符；`location` |
| `insert_table` | `InsertTable` | `WedocDocumentInsertTable` | 插入表格；`rows` ≤100、`cols` ≤60、单元格 ≤1000；`location` |
| `insert_paragraph` | `InsertParagraph` | `WedocDocumentInsertParagraph` | 插入段落；`location` |
| `update_text_property` | `UpdateTextProperty` | `WedocDocumentUpdateTextProperty` | 更新文本属性；`text_property` + `ranges`（≤10） |

`WedocDocumentTextProperty` 字段：`blod`（**官方拼写**，意为加粗）、`color`（十六进制 RRGGBB）、`background_color`。

**响应参数**：仅 `errcode` / `errmsg`（`WechatWorkResponse`）。

**本端点注意事项**

- **全或无**：30 个操作里有一个不合法，整批不生效——不要以为「前面成功的已经改了」。
- **`verison` 是官方拼写陷阱**：参数表写 `version`、示例写 `verison`，本仓按「以官方 JSON 示例为准」照抄 `verison`；手写 JSON 时写 `version` 会静默失效（服务端当未传版本处理）。
- `blod` 同理是官方拼写（bold 的笔误），不要「顺手修正」。
- 位置全部相对**读取时的树**：同一批内多个操作的位置不会自动随前面的插入/删除位移，需要自行按操作顺序计算。
- 一个操作对象只填一个字段：填多个时官方「仅一个生效」，哪个生效未定义。

**调用示例**（读树定位 → 插入一段文本 → 校验结果）

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

// 第三方 / 代开发改用 IWechatWorkThirdPartyWedocDocumentService / IWechatWorkProviderWedocDocumentService，
// 并在调用前 switcher.UseCorpScope(appKey, authCorpId, permanentCode)。
var document = provider.GetRequiredService<IWechatWorkInternalWedocDocumentService>();
string docId = "DOCID-FAKE-PLACEHOLDER";       // 来自创建接口并已持久化

// ① 先读：拿版本号与节点树；index 只能来自树的 begin/end，不能自己数
var data = await document.GetDocumentDataAsync(
    new GetWedocDocumentDataRequest { Docid = docId }, cancellationToken);
WechatWorkException.ThrowIfFailed(data, "/cgi-bin/wedoc/document/get");
long version = data.Version ?? throw new InvalidOperationException("响应未返回 version");
var root = data.Document ?? throw new InvalidOperationException("响应未返回 document");

// ② 在树里找第一个 Paragraph，取它的 end 作为插入位置
static IEnumerable<WedocDocumentNode> Flatten(WedocDocumentNode node)
{
    yield return node;
    foreach (var child in node.Children ?? new List<WedocDocumentNode>())
    {
        foreach (var descendant in Flatten(child))
        {
            yield return descendant;
        }
    }
}

var paragraph = Flatten(root).FirstOrDefault(n => n.Type == "Paragraph");
long insertAt = paragraph?.End ?? root.End ?? 0;

// ③ 一次提交 ≤30 个操作：此处只插一段文本；verison 拼写照抄官方示例，blod 同理
var updated = await document.BatchUpdateDocumentAsync(
    new BatchUpdateWedocDocumentRequest
    {
        Docid = docId,
        Verison = version,
        Requests =
        [
            new WedocDocumentUpdateOperation
            {
                InsertText = new WedocDocumentInsertText
                {
                    Text = "由 SDK 写入的示例段落",
                    Location = new WedocDocumentLocation { Index = insertAt },
                },
            },
        ],
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(updated, "/cgi-bin/wedoc/document/batch_update");
logger.LogInformation("文档 {DocId} 批量编辑完成（基于版本 {Version}）", docId, version);
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 先读后写 vs 直接写 | 位置必须来自 E1 的 `begin` / `end` | 自行推算 index ⇒ 内容错位或失败 |
| `verison` vs `version` | 官方示例拼写为 `verison`，本仓照抄 | 手写 `version` ⇒ 版本条件静默丢失 |
| `blod` vs `bold` | 官方参数表原文 `blod` | 写 `bold` ⇒ 属性不生效 |
| 批量失败语义 | ≤30 个操作，**一处失败整批不生效** | 以为部分成功 ⇒ 状态判断错误 |
| 操作体互斥 | 八个字段只能填一个 | 填多个时「仅一个生效」且不可预期 |
| 版本差闸 | `verison` 与最新版本差 ≤100 | 超差直接失败，长事务须重读 |
| 插入表格规模 | 行 ≤100、列 ≤60、单元格 ≤1000 | 超限失败（整批回滚） |
| `ranges` 上限 | 替换 / 更新属性各 ≤10 个范围 | 超限失败 |
| 文档 vs 表格 | 本族只服务 `doc_type = 3` | 对表格 `docid` 调用 ⇒ 业务错 |
| 图片 `image_id` 来源 | 先经 [Material.md](./Material.md) 上传拿 URL / id | 直接填外链 ⇒ 失败 |
| 正文数据安全 | 文本与属性属业务内容 | 打印 `text` ⇒ 内容泄露进日志 |

SDK 侧通用约束：不自动判错、不做本地条数与互斥校验、不自动重读重试。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 请求成功 | 判 `IsSuccess` |
| 640017 / 640018 / 640008 | 域 README 留档 | 未开启 API / 未授编辑权限 / 调用者无管理权限 | 权限前置 |
| 640054 | 域 README 留档 | `docid` 不是目标类型（如按智能文档调用） | 核对 `doc_type` |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 应用令牌类错误 | 统一失效码处理 |
| 42011 | `InvalidProviderAccessToken` | 服务商令牌不合法 / 过期 | 失效码集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，装配缺陷 |

本族注释未留档**专属**业务码：位置越界、版本超差、表格规模超限、操作体冲突的具体码值以官方页为准（仓内未逐项核验）。其余见官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| `verison` / `version` 服务端真实取值 | 参数表与示例不一致，本仓按示例照抄；服务端是否两键都认 ⇒ 未核验 | 官方 97626 |
| 位置越界错误码 | `index` 超出文档长度时的具体业务码 ⇒ 未留档 | 官方 97626 + 90455 |
| 同批多操作的位置位移语义 | 官方是否自动纠正后续操作位置 ⇒ 未核验（本仓按「不自动」处理） | 官方 97626 |
| 并发编辑冲突 | 两人同时基于同一版本改，后者是失败还是覆盖 ⇒ 未核验 | 官方 97626 |
| 节点属性 `property` 的子字段全集 | 各类节点的属性结构（对齐、边框、字号等）未逐字段成文 | DTO `WedocDocumentProperty` 一族 |
| 文本长度上限 | 单次 `insert_text` / `replace_text` 的 `text` 长度上限 ⇒ 未核验 | 官方 97626 |
| 本族频率限制 | 官方未给独立数值频率 ⇒ 走全局 | 90454 |
| 真账号验证 | 本批仅内部证据核验，未对真企业调用 | — |

## 8. 延伸阅读

- 同域兄弟：[Wedoc.md](./Wedoc.md)（建/删/改名）· [Spreadsheet.md](./Spreadsheet.md)（表格内容）· [SmartSheet.md](./SmartSheet.md)（智能表格内容）· [SmartDoc.md](./SmartDoc.md)（智能文档内容）
- 素材：[Material.md](./Material.md)（`insert_image` 的 `image_id` 来源）
- 权限：[DocPermission.md](./DocPermission.md)（`doc_get_auth` 预检）
- 域总览：[Wedoc README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/README.md)
- 官方页：[编辑文档内容 97626](https://developer.work.weixin.qq.com/document/path/97626) · [获取文档数据 101161](https://developer.work.weixin.qq.com/document/path/101161)
