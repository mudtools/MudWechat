# 素材管理 · 临时素材、图片与异步上传任务（三类应用公共面）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Media` 域 · 素材管理公共面族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkMediaService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Media/IWechatWorkMediaService.cs)（父，承载全部 6 支，`IsAbstract = true` 不可注入）· [IWechatWorkInternalMediaService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Media/IWechatWorkInternalMediaService.cs) · [IWechatWorkThirdPartyMediaService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Media/IWechatWorkThirdPartyMediaService.cs) · [IWechatWorkProviderMediaService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Media/IWechatWorkProviderMediaService.cs)（三者均为零差异空标记） |
| DTO 源文件 | [DataModels/Media/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Media/)（7 型，`SerializerClassName = Media` → `MediaJsonContext`；`RequestModel/` 子目录仅作组织、不入命名空间） |
| 契约守卫 | [WechatMediaContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMediaContractGuards.cs)（`MediaEndpoints_ShouldMatchOfficialRoutes` 6 支路由 + 二进制下载返回类型与可选 `Range` 头 / `MediaInterfaceHierarchy_ShouldConvergeOnAbstractParentWithMediaRegistry` 三类等宽空标记 / `MediaTokenBinding_ShouldBeAccessAndProviderTokenInjectedViaQuery` / `MediaDataModels_ShouldBeRegisteredInJsonContext` 7 型登记）＋ G5 Query 令牌白名单 ＋ TO1~TO3 归属键 |
| 官方文档 | 上传临时素材 [90253](https://developer.work.weixin.qq.com/document/path/90253) · 获取临时素材 90256 · 上传图片 90254 · 获取高清语音素材 90255 · 异步上传临时素材 96219（第三方同页 90389 / 90390 / 90392 / 90391 / 97126；服务商代开发 96484 / 96486 / 96485 / 96487 / 96488） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对，并用 `EmitCompilerGeneratedFiles` 复测了生成体；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 24 篇的可运行示例合并逐字编译，0 错误（本篇 2 段：1 段为文件级宿主类型即 `IFormContent` 实现，1 段为调用侧（E1 上传 → E2 下载 → E3 图片 → E5/E6 异步任务链路））；本批 25 段、全仓累计 150 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：MD1a/MD2/MD3/MD4 四组守卫断言 + 接口与 DTO XML 注释（含守卫类 remarks 的形态说明）+ 生成体实测 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Media/README.md) · 官方：本批未重新取文 |

## 1. 这个接口族做什么

企业微信服务端的**素材出入口**：所有「发消息要带文件」的场景（图片 / 语音 / 视频 / 普通文件、图文消息封面、直播附图、客服消息素材……）都先在这里换成 `media_id` 或图片 URL，再由业务域引用。6 支端点分三层：

| 层 | 端点 | 用途 |
|---|---|---|
| 同步上传 | E1 `media/upload`（multipart）· E3 `media/uploadimg`（multipart） | 换**三天有效**的 `media_id` / 换**永久**图片 URL |
| 下载读取 | E2 `media/get`（**GET，返回 `byte[]`**）· E4 `media/get/jssdk`（**GET，返回 `byte[]`**） | 凭 `media_id` 取文件本体；E4 取 JSSDK `uploadVoice` 的高清语音 |
| 异步任务 | E5 `media/upload_by_url` · E6 `media/get_upload_by_url_result` | 让官方从 CDN URL 拉大文件（≤200MB）转存为临时素材，凭 `jobid` 取结果 |

形态：官方对**三类应用开放完全一致的 6 支** ⇒ 全收敛父接口 + 三个零差异空标记子接口（MD2）。

**本域不含的两件事**：① 服务商通道「上传临时素材」（`/cgi-bin/service/media/upload`，走 `provider_access_token`、仅第三方开放）另立一族，见 [ServiceMedia.md](./ServiceMedia.md)；② E4 的 `media_id` 由**前端 JSSDK `uploadVoice`** 产生，本 SDK 没有对应的高清语音上传端点。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider`（三类等宽） | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方/代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险：官方契约强制 Query，无法改 Header） | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

第三方 / 代开发消费授权企业级令牌（scope = `authCorpId`），调用前须 `IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`（命名空间 `Mud.Wechat.Work.Abstractions.Authentication`；`permanent_code` 只写参数名，不得进日志与异常消息）。

### 2.2 权限与业务前置

- **E5 / E6（异步上传）须配置「客户联系」可调用接口的应用并具有客户联系权限**——这是本域唯一有明确权限要求的分支；其余 4 支接口注释未限定权限。
- 量纲闸门（E1，接口注释原文）：所有文件**必须大于 5 个字节**；`image` **10MB**、仅 JPG/PNG；`voice` **2MB**、播放长度 ≤60s、仅 AMR；`video` **10MB**、仅 MP4；`file` **20MB**。
- 量纲闸门（E3）：图片 **5B ~ 2MB**（注意远小于 E1 的 10MB），且**每企业每月 ≤3000 张、每天 ≤1000 张**。
- E5 的 `url` **必须支持 Range 分块下载**（腾讯云 COS 链接需设为「公有读」），`scene` **目前仅支持 1**（客户联系入群欢迎语素材），`type` **目前仅支持 `video` / `file`**（图片与语音暂不支持），视频/普通文件上限 **200MB**。
- `media_id` **三天内有效、同一企业内应用之间可共享** ⇒ 它是短期句柄，不能当持久资产入库后长期复用；发消息时的展示文件名由上传时的 `filename` 决定。
- E3 返回的图片 URL **永久有效但仅限企业微信环境**：用于图文消息正文、欢迎语等；**放到非企微页面的图片会被屏蔽**。
- 素材内容本身可能是人脸、语音、定位轨迹、内部文件等**个人数据或敏感资料**；`media_id` 是可换回内容的标识 ⇒ 示例用假路径，**不要把 `media_id` 或下载字节写进日志**。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 `media/upload` · E2 `media/get` · E3 `media/uploadimg` · E4 `media/get/jssdk` · E5 `upload_by_url` · E6 `get_upload_by_url_result` | ✅ | ✅ | ✅ |
| 服务商通道 `service/media/upload` | ❌ | ✅（见 [ServiceMedia.md](./ServiceMedia.md)） | ❌ |

三类子接口**声明面为空**（MD2 逐接口断言 `DeclaredOnly` 方法集恒空）⇒ 注入哪一支都拿到同一套 6 支能力，选择依据只有令牌归属与调用前置。

## 3. 接口族与注册

```csharp
// Program.cs：多应用底座先行，再注册素材管理模块
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");   // 或 AddWechatApp(o => { ... })
builder.Services.AddWechatWorkServices(b => b.AddMediaApi());
```

| 项 | 值 |
|---|---|
| 模块枚举 | `WechatModule.Media` |
| 注册入口 | `WechatWorkServiceBuilder.AddMediaApi()`（→ 源生成 `AddMediaWebApiHttpClient()`，无签入源文件） |
| 注册组名 | `"Media"`（公共面三支子接口与服务商通道子接口**共用同一注册组**，MD2/MD2b 锁定） |
| 可注入接口 | `IWechatWorkInternalMediaService` / `IWechatWorkThirdPartyMediaService` / `IWechatWorkProviderMediaService`（命名空间 `Mud.Wechat.Work`） |
| 不可注入 | `IWechatWorkMediaService`（父，`IsAbstract = true`，`RegistryGroupName` 空，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

### E1. `UploadTemporaryMediaAsync` — 上传临时素材

**签名**

```csharp
[Post("/cgi-bin/media/upload")]
Task<UploadMediaResponse> UploadTemporaryMediaAsync(
    [Query("type")] string type,
    [MultipartForm] IFormContent formData,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/media/upload` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/media/upload?type=TYPE&access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 自建 · 第三方 · 代开发 |
| Content-Type | `multipart/form-data`，**文件字段名必须为 `media`**，须含 `filename` / 长度 / `content-type` |

**请求参数**

| 字段 | 位置 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|---|
| `type` | Query | — | `string` | **是** | `image` / `voice` / `video` / `file` |
| — | Form | `formData` | `IFormContent` | **是** | 文件本体，由**宿主实现** `Mud.HttpUtils.IFormContent`（字段名 `media`） |

响应 `UploadMediaResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `type` | `Type` | `string?` | 回显媒体类型 |
| `media_id` | `MediaId` | `string?` | 素材 id，**三天有效**，同企业内应用共享 |
| `created_at` | `CreatedAt` | `long?` | 上传时间戳（Unix 秒） |

> ⚠️ **本端点在当前依赖版本下不可用**（文件字节不上 wire），原因与复现见 §5「上传腿」与 §7。签名、参数与文档口径均有效，实际上传须等上游修复。

**调用示例**（宿主实现 `IFormContent`，与 E2/E3/E5/E6 串成一条完整链路）

表单类型是**宿主侧类型**（C# 不支持在方法体内声明类型，故单独成段）：

```csharp
// 文件级示例：本段就是宿主工程里的一个普通类型文件（含 using，可直接落进自己的项目）
using System.Net.Http.Headers;
using Mud.HttpUtils;

/// <summary>
/// 单文件上传表单：官方要求文件字段名为 <c>media</c>，并携带 filename 与 content-type。
/// </summary>
public sealed class MediaUploadForm : IFormContent
{
    private readonly Stream _stream;
    private readonly string _fileName;
    private readonly string _contentType;

    public MediaUploadForm(Stream stream, string fileName, string contentType)
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
using Mud.Wechat.Work;                        // 可注入子接口 + AddWechatWorkServices / AddMediaApi
using Mud.Wechat.Work.Abstractions;           // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.Media;        // 请求/响应 DTO

// ① 注册自建应用（凭据来自宿主配置/环境变量，不得写入仓库与日志）
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "corp-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});

// ② 注册素材管理模块
builder.Services.AddWechatWorkServices(b => b.AddMediaApi());

// ③ 注入自建子接口；第三方 / 代开发改注入对应子接口并先 UseCorpScope
var media = provider.GetRequiredService<IWechatWorkInternalMediaService>();

// ④ E1 上传临时素材（image ≤10MB、JPG/PNG、>5 字节）
await using (var file = File.OpenRead(config["Demo:ImagePath"]!))
{
    var uploaded = await media.UploadTemporaryMediaAsync(
        "image", new MediaUploadForm(file, "welcome.png", "image/png"), cancellationToken);
    WechatWorkException.ThrowIfFailed(uploaded, "/cgi-bin/media/upload");
    var mediaId = uploaded.MediaId;

    // ⑤ E2 取回素材本体：返回 byte[]，失败时官方回 JSON、由判定器抛 WechatWorkException
    //    ⇒ 二进制端点没有 errcode 字段，不能再调用 ThrowIfFailed
    var bytes = await media.GetTemporaryMediaAsync(mediaId, null, cancellationToken);
    logger.LogInformation("临时素材大小：{Size} 字节", bytes?.Length ?? 0);
}

// ⑥ E3 上传图片换永久 URL（5B~2MB；URL 仅企微环境可见）
await using (var cover = File.OpenRead(config["Demo:CoverPath"]!))
{
    var image = await media.UploadImageAsync(
        new MediaUploadForm(cover, "cover.jpg", "image/jpeg"), cancellationToken);
    WechatWorkException.ThrowIfFailed(image, "/cgi-bin/media/uploadimg");
    logger.LogInformation("图片 URL 已获取（长度 {Length}）", image.Url?.Length ?? 0);
}

// ⑦ E5 → E6 大文件异步链路：官方从 CDN 拉取并转存（scene 目前仅 1，type 仅 video/file）
var job = await media.UploadMediaByUrlAsync(new UploadMediaByUrlRequest
{
    Scene = 1,
    Type = "file",
    FileName = "manual.pdf",
    Url = config["Demo:FileCdnUrl"]!,   // 须支持 Range 分块下载
    Md5 = config["Demo:FileMd5"]!,      // 32 字节以内，不一致返回 301019
}, cancellationToken);
WechatWorkException.ThrowIfFailed(job, "/cgi-bin/media/upload_by_url");

// jobid 60 分钟内有效；任务完成时官方推 upload_media_job_finish 事件（只带 JobId），结果须主动查
var result = await media.GetUploadMediaByUrlResultAsync(
    new GetUploadMediaByUrlResultRequest { JobId = job.JobId }, cancellationToken);
WechatWorkException.ThrowIfFailed(result, "/cgi-bin/media/get_upload_by_url_result");
if (result.Status == 1)
{
    // 1 处理中：宿主自行掌握重试节奏（jobid 时效是硬窗口），不要当成失败
    logger.LogInformation("异步上传任务仍在处理中，稍后凭同一 jobid 续查");
}
else if (result.Status == 2)
{
    // 2 完成：该 media_id 与普通上传的不通用，只能用于「获取临时素材」与入群欢迎语素材
    logger.LogInformation("异步上传完成，created_at={CreatedAt}", result.Detail?.CreatedAt);
}
else
{
    // 3 异常失败：真正的失败码在 detail.errcode（830001 / 830003 / 45001 / 301019 …）
    logger.LogError("异步上传失败 detail.errcode={ErrorCode}", result.Detail?.ErrorCode);
}
```

### E2. `GetTemporaryMediaAsync` — 获取临时素材

**签名**

```csharp
[Get("/cgi-bin/media/get")]
Task<byte[]?> GetTemporaryMediaAsync(
    [Query("media_id")] string mediaId,
    [Header("Range")] string? range = null,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `GET` `/cgi-bin/media/get` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/media/get?media_id=MEDIA_ID&access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入） |
| 成功响应 | **二进制文件流**（`Content-Type` / `Content-disposition` 等头部 + 文件内容）；分块下载时状态码 **206**，响应含 `Accept-Ranges` 与 `Content-Range` |
| 失败响应 | JSON（如 `40007 invalid media_id`），由 errcode 判定器抛 `WechatWorkException` |

**请求参数**

| 字段 | 位置 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|---|
| `media_id` | Query | `mediaId` | `string` | **是** | 来源为「上传临时素材」（E1）与「异步上传临时素材」（E5/E6） |
| `Range` | Header | `range` | `string?` | 否 | 如 `bytes=0-1023`；**MD1a 锁定该参数存在、可缺省**（普通素材无需分块） |

**返回**：`byte[]?`——**没有响应 DTO、不入 JSON 上下文登记面**（MD4 只登记 7 型）。

**官方硬约束**：异步上传得到的 `media_id` **超过 20M 时必须用 `Range` 分块下载且每块 ≤20M**，否则返回 **830002**（其它 `media_id` 文件过大同样回 830002，建议分块 ≤20M）；**加密存储**场景下分片长度**只能是 16 字节的倍数**，否则无法分片解密——以 `Content-Range` 结束位置 + 1 是否等于 size 判断是否为最后一个分片。

### E3. `UploadImageAsync` — 上传图片

**签名**

```csharp
[Post("/cgi-bin/media/uploadimg")]
Task<UploadImageResponse> UploadImageAsync(
    [MultipartForm] IFormContent formData,
    CancellationToken cancellationToken = default);
```

| 字段 | 位置 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| — | Form | `IFormContent` | **是** | multipart 表单，须含 `filename` / `content-type`（官方此端点无 `type` 参数） |

**返回**：`UploadImageResponse.Url`（`string?`，JSON `url`）——**永久有效**，但仅能在企业微信环境展示（图文消息正文、欢迎语等），非企微页面引用会被屏蔽。

量纲：图片 **5B ~ 2MB**；配额 **每企业每月 3000 张、每天 1000 张**。

### E4. `GetHdVoiceMediaAsync` — 获取高清语音素材

**签名**

```csharp
[Get("/cgi-bin/media/get/jssdk")]
Task<byte[]?> GetHdVoiceMediaAsync(
    [Query("media_id")] string mediaId,
    [Header("Range")] string? range = null,
    CancellationToken cancellationToken = default);
```

与 E2 形状完全相同（同参数、同二进制返回），差别全在语义：

- `media_id` 只能来自**前端 JSSDK `uploadVoice`**，与 E1 上传的语音不是同一来源；
- 素材格式 **speex、16K 采样率**（E1 侧语音是 amr、8K），音质更高，适合语音识别等场景；转码需 Speex 官方解码库 + 微信提供的解码库；
- **仅企业微信 2.4 及以上版本支持，暂不支持鸿蒙系统上传的语音**。

### E5. `UploadMediaByUrlAsync` — 生成异步上传任务

**签名**

```csharp
[Post("/cgi-bin/media/upload_by_url")]
Task<UploadMediaByUrlResponse> UploadMediaByUrlAsync(
    [Body] UploadMediaByUrlRequest request,
    CancellationToken cancellationToken = default);
```

**请求体** `UploadMediaByUrlRequest`（五字段**均官方必填**）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `scene` | `Scene` | `int?` | 场景值，**目前仅支持 1**（客户联系入群欢迎语素材） |
| `type` | `Type` | `string?` | **目前仅支持 `video` / `file`**，≤32 字节；视频仅 MP4（图片与语音暂不支持） |
| `filename` | `FileName` | `string?` | ≤128 字节，决定使用该 `media_id` 发消息时的展示名 |
| `url` | `Url` | `string?` | ≤1024 字节，**须支持 Range 分块下载**；腾讯云 COS 需设「公有读」 |
| `md5` | `Md5` | `string?` | ≤32 字节，官方下载后校验一致性，不一致 → **301019** |

**返回**：`UploadMediaByUrlResponse.JobId`（JSON `jobid`，**60 分钟内有效**）。

任务完成时官方向应用回调地址推送 **`upload_media_job_finish`** 事件，**只携带 JobId、不含结果** ⇒ 结果必须凭 E6 主动查（回调载荷面不在本域）。

### E6. `GetUploadMediaByUrlResultAsync` — 查询异步上传任务结果

**签名**

```csharp
[Post("/cgi-bin/media/get_upload_by_url_result")]
Task<GetUploadMediaByUrlResultResponse> GetUploadMediaByUrlResultAsync(
    [Body] GetUploadMediaByUrlResultRequest request,
    CancellationToken cancellationToken = default);
```

**请求体**：`GetUploadMediaByUrlResultRequest.JobId`（JSON `jobid`，官方必填，≤128 字节、60 分钟内有效）。

**返回** `GetUploadMediaByUrlResultResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `status` | `Status` | `int?` | **1 处理中 / 2 完成 / 3 异常失败** |
| `detail` | `Detail` | `MediaUploadJobDetail?` | 任务完成或失败时返回 |

`MediaUploadJobDetail`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `errcode` | `ErrorCode` | `int?` | **任务级**返回码：`status = 3` 时非 0，其余为 0。常见值 **830001** url 非法（不支持 Range）、**830003** url 下载数据失败、**45001** 文件大小越界（应 5 字节 ~ 200M）、**301019** MD5 不匹配 |
| `errmsg` | `ErrorMessage` | `string?` | 任务码文本描述 |
| `media_id` | `MediaId` | `string?` | `status = 2` 时返回，三天内有效 |
| `created_at` | `CreatedAt` | `long?` | `status = 2` 时返回（Unix 秒） |

> **判成功只看 `Status` 与 `Detail.ErrorCode`**：顶层 `errcode` 表达的是「接口调用是否成功」，`status = 1`（处理中）与 `status = 3`（任务失败）都可能是顶层 0。

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **上传腿（现行缺陷）** | 本批用 `EmitCompilerGeneratedFiles` 复测生成体：`Mud.HttpUtils.Generator` **3.0.9** 对 `[MultipartForm] IFormContent` 发射的是<br>`var __formDataContent = new StringContent(formData.ToString() ?? "");` + `__multipartContent.Add(__formDataContent, "formData");`<br>⇒ **`ToHttpContent()` / `ToHttpContentAsync()` 从不被调用**，表单里只有一个名为 `formData` 的字符串部分，**文件字节不上 wire**，字段名也不是官方要求的 `media` | 宿主无论怎么正确实现 `IFormContent` 都上传不了；官方侧表现为「缺少参数/文件」类错误，容易被误判成宿主实现写错 |
| 接口注释口径 vs 发射形态 | 注释称「`[MultipartForm]` 才走 `IFormContent.ToHttpContentAsync` 通路」——那是 3.0.1 期为绕 `CS1061` 留下的**编译期**结论，与本批实测的**运行期**发射不符 ⇒ 该句**不成立** | 按注释信任通路，不去复测生成体 |
| `[FormContent]` vs `[MultipartForm]` | 前者在生成器 3.0.1 下发射 `GetFormDataContentAsync`（运行时接口无此成员，`CS1061`）；本仓统一用后者 | 换标注 → 编译期即失败 |
| `Mud.HttpUtils.DefaultFormContent` | 组件里唯一可见的现成 `IFormContent` 实现是 `DefaultFormContent(Dictionary<string,string>)`，产出 `application/x-www-form-urlencoded` ⇒ **不是**带文件的 multipart | 用它「凑一个实现」→ 官方直接拒 |
| **本地 fail-fast 例外** | 生成体对 E1 `type` / E2·E4 `media_id` 空白、以及 `formData == null` **直接抛 `ArgumentNullException`**——这是本域对「SDK 不做本地必填校验」通则的例外（实测生成体，见 §7 复现命令） | 以为空值会交给官方判定，实际在客户端就抛 |
| **二进制端点不能判 errcode** | E2 / E4 返回 `byte[]?`，**不是** `WechatWorkResponse` 子类 ⇒ 不能对其调用 `WechatWorkException.ThrowIfFailed`；失败已由判定器在传输层抛出 | 对 `byte[]` 调判定器编译不过；或以为 `null` 代表业务错误码 |
| `media_id` **两套不通用** | E5/E6 返回的 `media_id` 与 E1 的使用场景不通用，目前只适用于「获取临时素材」（>20M 须 Range）与入群欢迎语素材（`scene = 1`） | 拿异步任务的 `media_id` 去发消息 → 官方拒绝 |
| E1 vs E3 不是同一件事 | E1 换**三天有效**的 `media_id`（四类媒体、上限 10/2/10/20MB）；E3 换**永久 URL**（仅图片、≤2MB、月度配额） | 用 E3 的 URL 当 `media_id`，或把 E1 的 `media_id` 当长期资源存储 |
| E2 vs E4 的来源不同 | E2 收 E1/E5 的 `media_id`（语音为 amr/8K）；E4 只收 **JSSDK `uploadVoice`** 的 id（speex/16K） | 拿 E1 上传的语音 id 调 E4 → 40007 |
| 动词分布 | E2/E4 官方即 **GET**（且返回二进制）；E1/E3/E5/E6 为 POST | 为「统一风格」改动词 → 打坏 MD1a 与 G5 |
| 令牌形态 | `access_token` 走 **Query**（MUD005），与服务商通道族（`provider_access_token`）是**两套令牌、两族接口** | 用服务商令牌调公共面，或反之 |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200，须 `WechatWorkException.ThrowIfFailed`）、**不做翻页与任务状态管理**（`jobid` 轮询节奏、`Range` 分块拼接由宿主实现）、**不校验文件大小/格式/配额**（越界由官方 `errcode` 表达）、**不做 media_id 有效期跟踪**（三天窗口须宿主自行落库与续期）。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E5 成功只代表任务已受理，结果仍要 E6 查 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（组件自动刷新令牌重试） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |
| 40007 | 业务码 | `invalid media_id` | E2 / E4 最常见；多为三天过期或来源不匹配 |
| 830002 | 业务码 | 文件过大、须分块下载 | E2 / E4：加 `Range` 且分块 ≤20M；加密场景分片长度须 16 字节倍数 |
| 830001 | 任务码（`detail.errcode`） | url 非法 | E5 提交的 url 不支持 Range 分块下载 |
| 830003 | 任务码 | url 下载数据失败 | 检查 CDN 可达性与权限（COS 需公有读） |
| 45001 | 任务码 | 文件大小超过限制（5 字节 ~ 200M） | E5 侧自行裁剪，本仓不校验 |
| 301019 | 任务码 | 文件 MD5 不匹配 | 核对 `md5` 是否为**下载源文件**的 MD5 |

上传类失败（字段名不是 `media`、文件 ≤5 字节 / 超类型上限、格式不符、图片配额用尽）的**具体码接口注释未逐条留档**；且在 §5「上传腿」缺陷下，官方返回的更可能是「缺少参数」类码而非素材校验码 ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| **上传腿不可用** | 本批实测（3.0.9 生成体）：`[MultipartForm] IFormContent` 不发射文件部分。复现命令：<br>`dotnet build Src/Work/Mud.Wechat.Work/Mud.Wechat.Work.csproj -c Release -f net8.0 --no-incremental -p:EmitCompilerGeneratedFiles=true -p:CompilerGeneratedFilesOutputPath=<目录>`，再看 `Mud/Wechat/Work/Interfaces/Internal/WechatWorkMediaService.g.cs` 的 `UploadTemporaryMediaAsync` / `UploadImageAsync` | 上游 `Mud.HttpUtils.Generator` 修复后重跑该命令 + 补一条 wire 级测试；届时**同批**修正接口注释里「`ToHttpContentAsync` 通路」那句与本节 |
| 本仓零 `IFormContent` 实现类、零 wire 级测试 | 上传腿缺陷不被 MD1a~MD4 与任何守卫覆盖（守卫只锁签名形态、路由、令牌与 JSON 登记）⇒ 属**守卫盲区** | 需要运行期保证时补宿主侧集成测试 |
| 上传校验码全集 | 接口注释只给量纲，未给各量纲对应的 `errcode` ⇒ 未逐项核验 | 官方 90253 / 90254 各页 + 全局错误码页 |
| 端点频率限制 | 本域注释**未留任何数值型阈值** ⇒ 未逐项核验，按官方全局访问频率附录执行 | 官方「访问频率限制」附录 |
| `upload_media_job_finish` 事件载荷 | 注释说明该事件「仅携带 JobId」，回调接收面不在本域 | `Callback` 包的事件族载荷 |
| 异步上传 `scene` / `type` 的后续扩展 | 注释写「目前仅支持 1」「图片与语音暂不支持」——是当期口径，非永久约束 | 官方 96219 / 97126 |
| E4 的高清语音上传端点 | 官方上传走前端 JSSDK `uploadVoice`，**服务端 API 无对应上传端点** ⇒ 本 SDK 无该面 | 官方 JSSDK `uploadVoice` 文档 |
| 真账号运行验证 | 本批仅编译级 + 生成体静态复测，未做真账号跑通 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Media/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Media/README.md)（7 支端点速览、六接口令牌表、逐端点契约明细与 DTO 表）
- 同域另一族：[ServiceMedia.md](./ServiceMedia.md)（服务商通道上传临时素材，`provider_access_token`、仅第三方开放）
- 同线跨域（`media_id` 的下游消费方）：[KfSession.md](../KF/KfSession.md)（客服消息 image/voice/video/file 与 link/miniprogram 封面素材）· [Living.md](../Living/Living.md)（活动直播封面 / 分享图 / 附图 mediaId）· [Attachment.md](../ExternalContact/Attachment.md)（客户联系素材上传，同一 multipart 缺陷的登记处）· [Moment.md](../ExternalContact/Moment.md)（朋友圈图片/视频素材）
- 素材上传的**另一条不通用路径**：群机器人 `webhook/upload_media`（`Webhook` 域，凭 `key` 而非令牌）
- 官方页：[上传临时素材（90253）](https://developer.work.weixin.qq.com/document/path/90253) · [获取临时素材（90256）](https://developer.work.weixin.qq.com/document/path/90256) · [上传图片（90254）](https://developer.work.weixin.qq.com/document/path/90254) · [获取高清语音素材（90255）](https://developer.work.weixin.qq.com/document/path/90255) · [异步上传临时素材（96219）](https://developer.work.weixin.qq.com/document/path/96219)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
