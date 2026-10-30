# 客户联系 · 商品图册

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `ExternalContact` 域 · 商品图册族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkExternalContactProductAlbumService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/ProductAlbum/IWechatWorkExternalContactProductAlbumService.cs)（父，承载全部 5 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/ProductAlbum/IWechatWorkExternalContactProductAlbumService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/ProductAlbum/IWechatWorkExternalContactProductAlbumService_Provider.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/ProductAlbum/IWechatWorkExternalContactProductAlbumService_ThirdParty.cs)（均空标记） |
| DTO 源文件 | [DataModels/ExternalContact/ProductAlbum/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ExternalContact/ProductAlbum/)（11 型，`SerializerClassName = ProductAlbum`，登记断言见 PA4 → `ProductAlbumJsonContext`） |
| 契约守卫 | [WechatExternalContactProductAlbumContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactProductAlbumContractGuards.cs)（PA1 路由表 5 支且互不重复 / PA2 层级与三空标记 / PA3 令牌绑定 / PA4 JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 自建 [95096](https://developer.work.weixin.qq.com/document/path/95096) · 第三方 [95131](https://developer.work.weixin.qq.com/document/path/95131) · 代开发 [96345](https://developer.work.weixin.qq.com/document/path/96345) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 18 篇的可运行示例合并逐字编译，0 错误（本篇 1 段）；本批 20 段、全仓累计 102 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：PA1~PA4 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.7、§4 陷阱 11）· 官方：本批未重新取文，正文口径继承接口注释留档 |

## 1. 这个接口族做什么

商品图册（商品相册）的完整 CRUD，5 支端点：

| 端点 | 用途 |
|---|---|
| E1 `add_product_album` | 创建图册，返回 `product_id` |
| E2 `get_product_album` | 按 `product_id` 取详情（含 `create_time`） |
| E3 `get_product_album_list` | `cursor` + `limit` 分页取列表 |
| E4 `update_product_album` | 按 `product_id` 编辑（只填要改的字段） |
| E5 `delete_product_album` | 按 `product_id` 删除 |

形态：官方对三类应用开放完全一致的 5 支端点 ⇒ 全收敛父接口，三支子接口空标记（PA2）。**5 支全部 POST**（E2/E3 是读取也走 POST，参数进 body）。

图册图片来自 [Attachment.md](./Attachment.md) 的「上传附件资源」（`attachment_type = 2`）拿到的 `media_id`；图册本身可被群发/朋友圈等场景作为附件引用（见 [GroupMsg.md](./GroupMsg.md)）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.ThirdParty` / `WechatAppType.Provider` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（PA3 统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

第三方 / 代开发消费的是**授权企业级**令牌（scope = `authCorpId`），调用前须先切作用域：`IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（命名空间 `Mud.Wechat.Work.Abstractions.Authentication`，返回 `IDisposable`，`using` 包裹）。

### 2.2 权限与业务前置

- 自建应用须配置到「**客户联系 可调用接口的应用**」中；第三方 / 代开发应用须具有「**管理商品图册**」权限（接口级 `remarks` 原文）。
- **读宽写窄**：E2 可读**企业内所有企业级**商品图册；E4 / E5 **仅可操作本应用自己创建**的图册。
- E1/E4 的 `attachments` **仅支持 `image` 类型**且**最多 9 个**；`image.media_id` **必须**来自 E「上传附件资源」且 `attachment_type = 2`（用其他途径拿到的素材 id 不被接受）。
- `description`（商品名称、特色等）**不超过 300 字**；`price` **单位为分**、最大不超过 **5 万元**；`product_sn` 不超过 **128 字节**且**仅支持数字和字母**。
- E3 的 `limit` **最大 100、默认 50**，**超限不报错而是取默认值**；自建应用调用只返回**应用可见范围内用户**的情况。
- 商品名、描述、价格是企业经营数据；示例一律用假值，且不要把整份图册打进日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E5 | ✅ | ✅ | ✅ |

三类应用的路由、动词与请求面**完全一致**（PA1 单一路由表覆盖三类），差异只在令牌归属键与权限口径。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkExternalContactProductAlbumService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **5** | 否 |
| `IWechatWorkInternalExternalContactProductAlbumService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyExternalContactProductAlbumService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderExternalContactProductAlbumService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `ExternalContact`，装配入口 `AddExternalContactApi()`；子接口 `InheritedFrom = nameof(WechatWorkExternalContactProductAlbumService)`。

## 4. 端点

签名与字段名以 [接口源文件](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/ProductAlbum/IWechatWorkExternalContactProductAlbumService.cs) 与 [DTO 目录](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ExternalContact/ProductAlbum/) 为准。「必填」列写的是**官方参数表口径**——本族 SDK 不做本地必填校验（本域做本地校验的是 [Attachment.md](./Attachment.md) 的上传端点）。

### E1. `AddProductAlbumAsync` — 创建商品图册

```csharp
[Post("/cgi-bin/externalcontact/add_product_album")]
Task<AddProductAlbumResponse> AddProductAlbumAsync(
    [Body] AddProductAlbumRequest request, CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/externalcontact/add_product_album` |
| 令牌 | Query `access_token` |

**请求参数**（`AddProductAlbumRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `description` | `Description` | `string?` | **是** | 商品名称、特色等描述，**≤300 字** |
| `price` | `Price` | `long?` | **是** | 商品价格，**单位分**，最大不超过 5 万元 |
| `product_sn` | `ProductSn` | `string?` | 否 | 商品编码，≤128 字节，仅数字与字母 |
| `attachments` | `Attachments` | `List<ProductAlbumAttachment>?` | **是** | 附件列表，**仅 `image`**、最多 9 个 |

`attachments[]`（`ProductAlbumAttachment`）→ `type`（`string?`，仅 `image`）+ `image`（`ProductAlbumImage`）→ `media_id`（`string?`，经「上传附件资源」`attachment_type = 2` 获得）。

**响应参数**（`AddProductAlbumResponse`）：`product_id`（`string?`）。

### E2. `GetProductAlbumAsync` — 获取商品图册详情

```csharp
[Post("/cgi-bin/externalcontact/get_product_album")]
Task<GetProductAlbumResponse> GetProductAlbumAsync(
    [Body] GetProductAlbumRequest request, CancellationToken cancellationToken = default);
```

- 请求：`product_id`（`string?`，必填）；**可读企业内所有企业级图册**。
- 响应：`product`（`ProductAlbumInfo?`）＝ `product_id` / `product_sn` / `description` / `price`（分）/ **`create_time`（仅本端点返回）** / `attachments`。

### E3. `GetProductAlbumListAsync` — 获取商品图册列表

```csharp
[Post("/cgi-bin/externalcontact/get_product_album_list")]
Task<GetProductAlbumListResponse> GetProductAlbumListAsync(
    [Body] GetProductAlbumListRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `limit` | `Limit` | `int?` | 否 | 最大 100，默认 50；**超限取默认值** |
| `cursor` | `Cursor` | `string?` | 否 | 上一次调用返回，首次不填 |

**响应参数**：`product_list`（`List<ProductAlbumInfo>?`）与 `next_cursor`（`string?`，**无更多数据时不返回该字段**，而非返回空串）。

### E4. `UpdateProductAlbumAsync` — 编辑商品图册

```csharp
[Post("/cgi-bin/externalcontact/update_product_album")]
Task<WechatWorkResponse> UpdateProductAlbumAsync(
    [Body] UpdateProductAlbumRequest request, CancellationToken cancellationToken = default);
```

- 请求：`product_id`（必填）+ `description` / `price` / `product_sn` / `attachments`（**只填要更新的字段**）。
- **仅可修改本应用自己创建的图册**；`attachments` 若更新，图片素材须同样来自「上传附件资源」。
- 响应：官方成功体仅 `errcode` / `errmsg` ⇒ 返回 `WechatWorkResponse`。

### E5. `DeleteProductAlbumAsync` — 删除商品图册

```csharp
[Post("/cgi-bin/externalcontact/delete_product_album")]
Task<WechatWorkResponse> DeleteProductAlbumAsync(
    [Body] DeleteProductAlbumRequest request, CancellationToken cancellationToken = default);
```

- 请求：`product_id`（必填）。**仅可删除本应用自己创建的图册**。
- 路由为 `delete_product_album`（官方即全称 `delete_`，**不是** `del_`——PA1 逐字锁定）。

**调用示例**（E1 创建 → E3 分页遍历 → E2 取详情）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.ExternalContact.ProductAlbum;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "crm-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddExternalContactApi());

var albums = provider.GetRequiredService<IWechatWorkInternalExternalContactProductAlbumService>();

// ① 创建：description / price / attachments 官方必填；price 单位是「分」
var created = await albums.AddProductAlbumAsync(
    new AddProductAlbumRequest
    {
        Description = "示例商品：保温杯（假数据）",
        Price = 12900,                       // 129.00 元
        ProductSn = "CUP001",
        Attachments = new List<ProductAlbumAttachment>
        {
            // media_id 必须是「上传附件资源」attachment_type=2 返回的素材 id（假值）
            new ProductAlbumAttachment
            {
                Type = "image",
                Image = new ProductAlbumImage { MediaId = "3aMediaIdPlaceholder" },
            },
        },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(created, "/cgi-bin/externalcontact/add_product_album");
string productId = created.ProductId ?? throw new InvalidOperationException("响应未返回 product_id");

// ② 分页遍历：limit 超限官方取默认值 50；next_cursor 不返回即取完
string? cursor = null;
do
{
    var page = await albums.GetProductAlbumListAsync(
        new GetProductAlbumListRequest { Limit = 100, Cursor = cursor }, cancellationToken);
    WechatWorkException.ThrowIfFailed(page, "/cgi-bin/externalcontact/get_product_album_list");

    foreach (ProductAlbumInfo p in page.ProductList ?? new List<ProductAlbumInfo>())
    {
        // 列表项不回填 create_time（只有 E2 详情返回），此处只做计数。
        logger.LogInformation("图册 {ProductId} 价格 {Price} 分，附件 {Count} 个",
            p.ProductId, p.Price, p.Attachments?.Count ?? 0);
    }
    cursor = page.NextCursor;
} while (!string.IsNullOrEmpty(cursor));

// ③ 详情：可读企业内所有企业级图册；列表里看不到 create_time 时来这里取
var detail = await albums.GetProductAlbumAsync(
    new GetProductAlbumRequest { ProductId = productId }, cancellationToken);
WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/externalcontact/get_product_album");
logger.LogInformation("商品 {ProductId} 创建于 {CreateTime}",
    detail.Product?.ProductId, detail.Product?.CreateTime);
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `delete_product_album`（E5）vs 本域其它族的 `del_*` | 官方对本族即用全称 `delete_`，`del_contact_way` / `del_corp_tag` / `del_intercept_rule` 等用缩写；本仓方法名统一 `Delete*`，**路由逐字照官方** | 手写 URL 用 `del_product_album` → 404 / 路由不识别 |
| `price` 单位 | **分**（`long?`），不是元 | 传 `129` 期望 129 元 → 客户看到 1.29 元 |
| `price` 上限 | ≤5 万元（即 5,000,000 分） | 超限被拒 |
| `attachments[].type` | 本族**只接受 `image`**；`video` / `file` 属朋友圈侧口径 | 传 `video` → 参数错误 |
| `image.media_id` 的来源 | **必须**来自「上传附件资源」且 `attachment_type = 2`；其它素材接口（如临时素材）拿到的 id 不被接受 | 创建成功但图片不显示，或直接报错 |
| 读宽 vs 写窄 | E2 读企业内所有企业级图册；E4 / E5 只能动**本应用创建**的图册 | 拿别的应用创建的 `product_id` 去编辑 → 失败 |
| `create_time` | **仅 E2 详情返回**，E3 列表项不回填 | 从列表读 `CreateTime` 恒为 null |
| `next_cursor`（E3） | 无更多数据时**不返回该字段**（反序列化后为 `null`），不是空字符串 | 只判 `!= ""` → 死循环 |
| `limit`（E3）超限 | 官方**取默认值 50**，不报错 | 以为传了 500 就多拿，实际一页 50 |
| E2/E3 的动词 | 读取语义但官方即 **POST**，参数走 body | 改 GET → 打坏 PA1 与 G5 |
| 上传腿可用性 | 见 [Attachment.md](./Attachment.md) §5：上游 `Mud.HttpUtils.Generator` 3.0.9 对 `[MultipartForm] IFormContent` 的发射不传文件字节 ⇒ 本族 E1/E4 的图片 `media_id` **无法经声明式端点自助获取** | 直接用 SDK 上传 → 官方报参数错误 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200，须自行 `WechatWorkException.ThrowIfFailed`）、**不做本地必填校验**（`description` / `price` / `attachments` 为 null 也会照常发出请求）；令牌走 Query 是官方契约（`MUD005` 已知接受风险）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E3 成功仍要看 `next_cursor` 是否缺失来决定续拉 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座自动重取令牌重试） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

E4 / E5 返回 `WechatWorkResponse`（仅 `errcode` / `errmsg`），业务失败信息只在 `errcode` 上 ⇒ **不能只看 HTTP 状态码**。本族其余业务码（`product_id` 不存在、非本应用创建、图片 `media_id` 非法、`description` 超长、`price` 越界）**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 注释只给约束与上限，未给「哪条约束对应哪个码」⇒ **未逐项核验** | 官方 95096 / 95131 / 96345 各端点页 |
| 端点频率限制 | 注释未给 QPS / 次数 ⇒ **未逐项核验** | 官方全局频率附录 |
| 「企业级图册」与「个人图册」的区分 | 注释称 E2 可读「企业内所有**企业级**」图册，未给图册归属字段 ⇒ **未建模、未核验**（`ProductAlbumInfo` 无 `owner`/`corp_type` 类字段） | 官方 95096 响应参数表 |
| `product_sn` 的唯一性与重复语义 | 注释只给长度与字符集 ⇒ 未核验 | 官方 95096 |
| E4 未传字段是否清空 | 注释说「仅需更新的字段才填」，未说明缺省字段是保留还是置空 ⇒ **未逐项核验** | 官方 95096 编辑节 |
| 图册被引用时的删除行为 | 已被群发/朋友圈引用的图册能否删除、删除后引用侧表现 ⇒ 注释未提 | 官方 95096 + [GroupMsg.md](./GroupMsg.md) / [Moment.md](./Moment.md) |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.7 端点表、§4 陷阱 11「删除路由拼写不一致」）
- 同域：[Attachment.md](./Attachment.md)（图片 `media_id` 的唯一合法来源，`attachment_type = 2`）· [GroupMsg.md](./GroupMsg.md)（群发附件）· [Moment.md](./Moment.md)（朋友圈附件口径差异）· [Customer.md](./Customer.md)（客户联系规则组与可见范围）
- 跨包：素材下载（「获取临时素材」）属企业微信「素材管理」接口，本域不承载
- 官方页：[商品图册（自建 95096）](https://developer.work.weixin.qq.com/document/path/95096) · [第三方应用 95131](https://developer.work.weixin.qq.com/document/path/95131) · [代开发应用 96345](https://developer.work.weixin.qq.com/document/path/96345)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
