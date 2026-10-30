# 客户联系 · 上传附件资源

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `ExternalContact` 域 · 附件上传族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkExternalContactAttachmentService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/Attachment/IWechatWorkExternalContactAttachmentService.cs)（父，承载唯一 1 支）· [\_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/Attachment/IWechatWorkExternalContactAttachmentService_Internal.cs) · [\_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/Attachment/IWechatWorkExternalContactAttachmentService_Provider.cs) · [\_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/Attachment/IWechatWorkExternalContactAttachmentService_ThirdParty.cs)（均空标记） |
| DTO 源文件 | [DataModels/ExternalContact/Attachment/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/ExternalContact/Attachment/)（仅响应 1 型，`SerializerClassName = Attachment`，登记断言见 UA4 → `AttachmentJsonContext`；**请求侧无 DTO**，参数走 Query + multipart） |
| 契约守卫 | [WechatExternalContactAttachmentContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactAttachmentContractGuards.cs)（UA1 路由 + `parameters.HaveCount(4)` 签名形态 / UA2 层级与三空标记 / UA3 令牌绑定 / UA4 响应 DTO JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 自建 [95098](https://developer.work.weixin.qq.com/document/path/95098) · 第三方 [95178](https://developer.work.weixin.qq.com/document/path/95178) · 代开发 [96347](https://developer.work.weixin.qq.com/document/path/96347) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释 + 本批源生成物发射形态复测；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 18 篇的可运行示例合并逐字编译，0 错误（本篇 2 段：1 段为文件级宿主类型即 `IFormContent` 实现，1 段为调用侧）；本批 20 段、全仓累计 102 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：UA1~UA4 + 接口与 DTO XML 注释 + 本批 `EmitCompilerGeneratedFiles` 取到的 `WechatWorkExternalContactAttachmentService.g.cs` 生成体 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.16、§4 陷阱 13）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

单支端点：为「客户联系」场景上传附件素材，返回**三天有效**的 `media_id`。

| 端点 | 用途 |
|---|---|
| E1 `POST /cgi-bin/media/upload_attachment` | multipart/form-data 上传图片 / 视频 / 文件，返回 `media_id` |

下游消费方固定两处，由 `attachment_type` 区分：

| `attachment_type` | 消费方 | 允许的文件类型 |
|---|---|---|
| `1` | 朋友圈（[Moment.md](./Moment.md) 的 `add_moment_task` 附件） | 图片、视频 |
| `2` | 商品图册（[ProductAlbum.md](./ProductAlbum.md) 的 `attachments[].image.media_id`） | **仅图片** |

**路由前缀不在本域命名空间下**：官方把该端点挂在 `/cgi-bin/media/`（素材族前缀）而非 `/cgi-bin/externalcontact/`，本仓按**官方文档章节归属**把它落在 `ExternalContact/Attachment/`——写 URL 时以 UA1 锁定的字符串为准。

形态：官方对三类应用开放完全一致的 1 支端点 ⇒ 收敛父接口，三支子接口空标记（UA2）。

> ⚠️ **本端点在当前依赖版本下不可用**（文件字节不上 wire），详见 §5「上传腿」与 §7。声明面、签名与文档口径均有效，实际上传须等上游修复。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `WechatAppType.ThirdParty` / `WechatAppType.Provider` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（UA3 统一断言） | — |
| 令牌注入 | `TokenInjectionMode.Query` ⇒ 本端点 URL 上**同时**有 `access_token` + `media_type` + `attachment_type` 三个 Query 参数 | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方/代开发 `Wechat.AccessToken@Corp` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

第三方 / 代开发消费授权企业级令牌（scope = `authCorpId`），调用前须 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（命名空间 `Mud.Wechat.Work.Abstractions.Authentication`）。

### 2.2 权限与业务前置

- 自建应用须配置到「**客户联系 可调用接口的应用**」中；第三方 / 代开发应用须具有「**企业客户**」权限。
- `media_type`（官方必填）：`image` 图片 / `video` 视频 / `file` 普通文件；**但** `attachment_type = 2`（商品图册）**只接受图片**，`attachment_type = 1`（朋友圈）只接受图片与视频。
- `attachment_type`（官方必填）：`1` 朋友圈 / `2` 商品图册。
- **所有文件必须大于 5 字节且不超过 10MB**；图片仅支持 **JPG / PNG**（朋友圈图片长边 ≤10800 像素、短边 ≤1080 像素）；视频仅支持 **MP4**（时长 ≤30 秒）。
- multipart 表单中**文件字段名必须为 `media`**，并携带 `filename` / 文件长度 / `content-type` 信息。
- `media_id` **三天有效** ⇒ 上传后须在窗口内被图册或朋友圈任务引用，过期须重传。
- 上传的文件内容可能含人脸、商品图等个人/商业信息：日志只记 `media_id` 与大小，不记内容。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 | ✅ | ✅ | ✅ |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkExternalContactAttachmentService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **1** | 否 |
| `IWechatWorkInternalExternalContactAttachmentService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyExternalContactAttachmentService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderExternalContactAttachmentService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `ExternalContact`，装配入口 `AddExternalContactApi()`；子接口 `InheritedFrom = nameof(WechatWorkExternalContactAttachmentService)`。

## 4. 端点

### E1. `UploadAttachmentAsync` — 上传附件资源

```csharp
[Post("/cgi-bin/media/upload_attachment")]
Task<UploadAttachmentResponse> UploadAttachmentAsync(
    [Query("media_type")] string mediaType,
    [Query("attachment_type")] int attachmentType,
    [MultipartForm] IFormContent formData,
    CancellationToken cancellationToken = default);
```

**请求参数**（UA1 断言签名恰好 4 个形参：`mediaType` / `attachmentType` / `formData` / `cancellationToken`）

| 参数 | 位置 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `media_type` | **Query** | `string` | **是** | `image` / `video` / `file`（按 `attachment_type` 收窄） |
| `attachment_type` | **Query** | `int` | **是** | `1` 朋友圈 / `2` 商品图册 |
| 文件本体 | **Body**（`multipart/form-data`） | `IFormContent` | **是** | 由宿主实现 `IFormContent`，**文件字段名须为 `media`**，含 `filename` / 长度 / `content-type` |

- **请求侧没有 DTO 类型**：官方是表单上传，本仓不为表单建 DTO（UA4 因此只登记响应型）。
- 生成器对 `mediaType` 空白、`formData` 为 `null` **在方法入口即抛 `ArgumentNullException`**（本域少见的「SDK 本地 fail-fast」，见 §5）。
- 标注选择：接口注释留档——生成器 3.0.1 对 `[FormContent]` 发射的 `GetFormDataContentAsync` 调用在运行时接口上不存在（`CS1061`），故本端点用 `[MultipartForm]`。这是**编译期**结论；**运行期**发射形态见 §5。

**响应参数**（`UploadAttachmentResponse`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `type` | `Type` | `string?` | 媒体文件类型：`image` / `voice` / `video` / `file`（`voice` 属官方枚举，本端点按 2.2 的口径不会产生） |
| `media_id` | `MediaId` | `string?` | 素材 id，**三天有效** |
| `created_at` | `CreatedAt` | `long?` | 上传时间戳（Unix 秒） |

**调用示例**（宿主实现 `IFormContent` → 上传商品图册图片 → 把 `media_id` 交给 [ProductAlbum.md](./ProductAlbum.md)）

表单类型是**宿主侧类型**（C# 不支持在方法体内声明类型，故单独成段）：

```csharp
// 文件级示例：本段就是宿主工程里的一个普通类型文件（含 using，可直接落进自己的项目）
using System.Net.Http.Headers;
using Mud.HttpUtils;

/// <summary>
/// 单文件上传表单：官方要求文件字段名为 <c>media</c>，并携带 filename 与 content-type。
/// </summary>
public sealed class SingleMediaForm : IFormContent
{
    private readonly Stream _stream;
    private readonly string _fileName;
    private readonly string _contentType;

    public SingleMediaForm(Stream stream, string fileName, string contentType)
    {
        _stream = stream;
        _fileName = fileName;
        _contentType = contentType;
    }

    public HttpContent ToHttpContent()
    {
        var file = new StreamContent(_stream);
        file.Headers.ContentType = new MediaTypeHeaderValue(_contentType);
        var form = new MultipartFormDataContent();
        form.Add(file, "media", _fileName);
        return form;
    }

    public Task<HttpContent> ToHttpContentAsync(IProgress<long>? progress, CancellationToken cancellationToken)
        => Task.FromResult(ToHttpContent());
}
```

调用侧：

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.ExternalContact.Attachment;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "crm-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddExternalContactApi());

var attachments = provider.GetRequiredService<IWechatWorkInternalExternalContactAttachmentService>();

using var image = File.OpenRead("assets/product-01.jpg");   // 假路径：>5 字节且 ≤10MB，仅 JPG/PNG
var uploaded = await attachments.UploadAttachmentAsync(
    mediaType: "image",
    attachmentType: 2,                       // 2 = 商品图册（只接受图片）；1 = 朋友圈
    formData: new SingleMediaForm(image, "product-01.jpg", "image/jpeg"),
    cancellationToken);
WechatWorkException.ThrowIfFailed(uploaded, "/cgi-bin/media/upload_attachment");

// media_id 三天有效：须在窗口内被商品图册/朋友圈任务引用，过期重传
logger.LogInformation("上传完成：type {Type}，media_id {MediaId}，created_at {CreatedAt}",
    uploaded.Type, uploaded.MediaId, uploaded.CreatedAt);
```

> 示例可编译，但**当前依赖版本下运行期不会把文件字节发出去**（§5「上传腿」）——拿到 `media_id` 需等上游修复后复测。

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| **上传腿（现行缺陷）** | 本批用 `EmitCompilerGeneratedFiles` 复测生成体：`Mud.HttpUtils.Generator` **3.0.9** 对 `[MultipartForm] IFormContent` 发射的是<br>`var __formDataContent = new StringContent(formData.ToString() ?? "");` + `__multipartContent.Add(__formDataContent, "formData");`<br>⇒ **`ToHttpContent()` / `ToHttpContentAsync()` 从不被调用**，表单里只有一个名为 `formData` 的字符串部分，**文件字节不上 wire**，字段名也不是官方要求的 `media` | 宿主无论如何正确实现 `IFormContent` 都上传不了；官方侧表现为参数/文件缺失类错误 |
| 接口注释口径 vs 发射形态 | 注释称「`[MultipartForm]` 才走 `IFormContent.ToHttpContentAsync` 通路」——这是 3.0.1 期为绕 `CS1061` 留下的**编译期**结论，与本批实测的**运行期**发射不符 ⇒ 注释该句**不成立** | 按注释信任通路 → 误判为宿主实现写错 |
| `[FormContent]` vs `[MultipartForm]` | 前者在生成器 3.0.1 下发射 `GetFormDataContentAsync`（运行时接口无此成员，`CS1061`）；本仓统一用后者 | 换标注 → 编译期即失败 |
| `Mud.HttpUtils.DefaultFormContent` | 组件里唯一可见的现成 `IFormContent` 实现是 `DefaultFormContent(Dictionary<string,string>)`，产出 `application/x-www-form-urlencoded` ⇒ **不是**带文件的 multipart，不能用在本端点 | 用它「凑一个实现」→ 官方直接拒 |
| 本地 fail-fast | 生成体会在入口校验：`mediaType` 为空白或 `formData` 为 `null` ⇒ **抛 `ArgumentNullException`（不发请求）**。本域其它端点不做本地必填校验，此处是例外 | 以为所有必填都交给官方报错 → 异常类型预判错误 |
| 三个 Query 参数 | `access_token` + `media_type` + `attachment_type` 全在 Query，文件在 body（`MUD005` 已知接受风险） | 把 `media_type` 当 body 字段找 |
| 路由前缀 | 官方挂 `/cgi-bin/media/`，本仓归属 `ExternalContact/Attachment/` 目录 | 按目录推断 `externalcontact/` 前缀 → URL 写错 |
| `attachment_type` 1 vs 2 | 决定下游消费方与允许的类型集合（2 只收图片） | 用 1 拿到的 id 塞进商品图册 → 不被接受 |
| `media_id` 生命周期 | **三天**有效，且只对**对应 `attachment_type`** 的消费方有效 | 长期缓存 id → 图册/朋友圈报无效素材 |
| 尺寸与格式 | `>5 字节`、`≤10MB`；JPG/PNG（朋友圈图片长边 ≤10800、短边 ≤1080）；视频仅 MP4 且 ≤30 秒 | 边界值（正好 5 字节）或超尺寸被拒 |
| 响应 `type` 含 `voice` | 官方枚举照录，本端点按 2.2 口径不会产生 `voice` | 为 `voice` 写分支是死代码 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）、令牌走 Query 是官方契约（`MUD005`）；本端点的必填校验由**生成体**完成（与 §「本地 fail-fast」行一致）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本族的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 成功仅代表素材已入库，`media_id` 三天内有效 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

上传类失败（字段名不是 `media`、文件 ≤5 字节 / >10MB、格式或尺寸越界、`attachment_type` 与 `media_type` 组合非法）的**具体码接口注释未逐条留档**；且在 §5「上传腿」缺陷下，官方返回的更可能是「缺少参数」类码而非素材校验码 ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| **上传腿不可用** | 本批实测（3.0.9 生成体）：`[MultipartForm] IFormContent` 不发射文件部分。复现命令：<br>`dotnet build Src/Work/Mud.Wechat.Work/Mud.Wechat.Work.csproj -c Release -f net8.0 --no-incremental -p:EmitCompilerGeneratedFiles=true -p:CompilerGeneratedFilesOutputPath=<目录>`，再看 `Mud/Wechat/Work/Interfaces/Internal/WechatWorkExternalContactAttachmentService.g.cs` 的 `UploadAttachmentAsync` | 上游 `Mud.HttpUtils.Generator` 修复后重跑该命令 + 补一条 wire 级测试；届时**同批**修正接口注释里「`ToHttpContentAsync` 通路」那句与本节 |
| 本仓零 `IFormContent` 实现类、零 wire 级测试 | 该缺陷不被 UA1~UA4 与任何守卫覆盖（守卫只锁签名形态与路由），属**守卫盲区** | 需要运行期保证时补宿主侧集成测试 |
| 上传失败错误码清单 | 注释未逐条摘录 ⇒ **未逐项核验** | 官方 95098 + 全局错误码页 |
| 端点频率限制 | 注释未给 QPS ⇒ 未逐项核验 | 官方全局频率附录 |
| 同一文件重复上传是否复用 `media_id` | 注释未说明去重/幂等口径 ⇒ 未核验 | 官方 95098 |
| `file` 类型在客户联系侧的消费方 | 注释只给「朋友圈 / 商品图册」两条链路，`media_type = file` 的可用场景未说明 ⇒ 未核验 | 官方 95098 + [GroupMsg.md](./GroupMsg.md) 附件口径 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/ExternalContact/README.md)（§3.16 端点表、§4 陷阱 12/13）
- 同域：[ProductAlbum.md](./ProductAlbum.md)（`attachment_type = 2` 的 `media_id` 消费方）· [Moment.md](./Moment.md)（`attachment_type = 1` 的消费方与附件张数/时长限制）· [GroupMsg.md](./GroupMsg.md)（群发附件的另一套素材口径）
- 同线跨域：企业微信「临时素材」上传（`/cgi-bin/media/upload`）属素材管理接口，与本端点不是同一个 `attachment_type` 语义，勿混用
- 官方页：[上传附件资源（自建 95098）](https://developer.work.weixin.qq.com/document/path/95098) · [第三方应用 95178](https://developer.work.weixin.qq.com/document/path/95178) · [代开发应用 96347](https://developer.work.weixin.qq.com/document/path/96347)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
