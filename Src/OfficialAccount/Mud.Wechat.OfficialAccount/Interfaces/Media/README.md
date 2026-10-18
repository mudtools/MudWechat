# Media 域 · 素材管理

微信公众号 / 服务号「素材管理」域：**11 条路由分两张面孔**——JSON 通道 **8 端点**（临时上传 / 永久上传 /
计数 / 列表 / 删除 / uploadimg / uploadnews / uploadvideo）落接口
[IMpMediaService.cs](./IMpMediaService.cs)，下载通道 **3 端点**（临时素材 / JSSDK 高清语音 / 永久素材）
落 [../../Media/IMpMediaDownloadService.cs](../../Media/IMpMediaDownloadService.cs)（**注意落位**：该文件在
包根 `Media/` 而非 `Interfaces/Media/`）。注册组 `Media`，模块枚举 `MpModule.Media`
（`AddMediaApi()` ⇒ 生成入口 `AddMediaWebApiHttpClient()`，并附带注册 `IMpMediaDownloadService`）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文断言只来自本仓接口文件的 XML 注释、本仓契约守卫
> [MpMediaContractGuards.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpMediaContractGuards.cs)
> （`MD1`~`MD11`）、[MpQueryTokenWhitelistGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpQueryTokenWhitelistGuard.cs)（`QT1`）
> 与本仓 DTO（[DataModels/Media/](../../../Mud.Wechat.OfficialAccount.DataModels/Media/)）。
> **计数以守卫为准**：`MD1` 钉 JSON 通道「恰 8 条路由、其中只有 1 条 GET」，`MD2` 钉下载通道 3 方法且
> 不进生成管线。包 README「`Media/` 6（上传侧）」与 `MpModule.Media` remarks「上传 1 端点 + 下载 2 端点」
> 均为**过期口径**（详见文末缺口），不作本文依据。

## 端点速览

| # | 方法 | HTTP 动词 | 路由 | 权限/令牌 | 备注 |
|---|---|---|---|---|---|
| 1 | `UploadTempMediaAsync` | POST | `/cgi-bin/media/upload` | 应用级 `access_token`（Query）+ `type`（Query） | multipart，文件字段名 `media` |
| 2 | `AddMaterialAsync` | POST | `/cgi-bin/material/add_material` | 应用级 `access_token`（Query）+ `type`（Query） | multipart；视频另附表单字段 `description` |
| 3 | `GetMaterialCountAsync` | GET | `/cgi-bin/material/get_materialcount` | 应用级 `access_token`（Query） | 本域唯一 GET；`count` 前无下划线，照抄官方 |
| 4 | `BatchGetMaterialAsync` | POST | `/cgi-bin/material/batchget_material` | 应用级 `access_token`（Query） | 请求体 `type`/`offset`/`count`（1~20） |
| 5 | `DelMaterialAsync` | POST | `/cgi-bin/material/del_material` | 应用级 `access_token`（Query） | 返回 `MpResponse`；与 `get_material` 共用请求体 DTO |
| 6 | `UploadImageAsync` | POST | `/cgi-bin/media/uploadimg` | 应用级 `access_token`（Query） | 返回 **url 非 media_id**；不占 100000 图片额度；无 `type` |
| 7 | `UploadNewsAsync` | POST | `/cgi-bin/media/uploadnews` | 应用级 `access_token`（Query） | **纯 JSON**（非 multipart）；articles 1~8；media_id 仅群发用 |
| 8 | `UploadVideoAsync` | POST | `/cgi-bin/media/uploadvideo` | 应用级 `access_token`（Query） | multipart **无文件**，仅表单字段 `media_id`/`title`/`description` |
| 9 | `DownloadTemporaryMediaAsync` | GET | `/cgi-bin/media/get` | 手工注入 `access_token`（Query），**下载通道无 `[Token]`** | 视频返回 JSON `{"video_url":…}`，非流非 302 |
| 10 | `DownloadJssdkVoiceAsync` | GET | `/cgi-bin/media/get/jssdk` | 手工注入 `access_token`（Query） | **仅认证**（本页字段表核验）；speex 16K |
| 11 | `GetPermanentMaterialAsync` | POST | `/cgi-bin/material/get_material` | 手工注入 `access_token`（Query） | 三形态：图文 `news_item` / 视频 `down_url` / 图片·语音二进制流 |

**令牌口径**：1~8 消费应用级 `access_token` 并以 Query 注入（`MD4`、MUD005）；9~11 落
`IMpMediaDownloadService`（`MD2`）——该接口**不声明 `[Token]`**，令牌经 `IMpAppContext.AccessTokenManager`
显式获取后手工注入（`QT1` 白名单只登记 `IMpMediaService` 上传面，下载面不入白名单）。

## 契约要点（代码名读不出的部分）

1. **两张面孔：临时素材 vs 永久素材**
   - 临时（`/cgi-bin/media/upload`）：**3 天有效**（官方「注意事项」原文「媒体文件在微信后台保存时间为 3 天，
     3 天后 media_id 失效」）；`media_id` 可复用；用于多媒体消息的 `media_id` 最多复用 **10 条**。
   - 永久（`/cgi-bin/material/add_material`）：数量上限——图文/图片 **100000**，其他类型 **1000**；
     永久图片素材带 URL 返回，**仅腾讯系域名内可用**（域名外被屏蔽）。永久图片另支持 `bmp`，语音支持 mp3/wma/wav/amr。
   - `type` 取值见 [MpMediaTypes](./IMpMediaService.cs)：`image`/`voice`/`video`/`thumb`，`news` 仅查询/删除类端点用（不可上传）。

2. **类型与大小限制（字段表口径，SDK 不做本地拦截）**：图片 10M（PNG/JPEG/JPG/GIF）、
   语音 2M 且 ≤60s（AMR/MP3）、视频 10M（MP4）、缩略图 64KB（JPG）。官方「注意事项」另写「图片 2MB / 视频 10MB」——
   **两处表述不一致**，SDK 以字段表（10M）为口径并把矛盾照录进 remarks，越界由官方错误码表达。

3. **上传是 multipart 手工通道**：`IFormContent` 由调用方构建（本仓不提供实现类，流生命周期由调用方掌握），
   须标 `[MultipartForm]`；**文件字段名必须为 `media`**。`AddMaterialAsync` 的视频描述走
   `[Form(FieldName = "description")]` 表单项（JSON 字符串）。`UploadImageAsync` **无 Query 业务参数**（仅令牌）；
   `UploadVideoAsync` 无文件、只有三个表单字段。`type` 一律在 Query（`MD7`）。

4. **downloadimg / uploadimg 归属只建模一次**：`uploadimg` 官方同时列于「群发」与「永久素材」，本域承载，
   群发域交叉引用、不建第二份声明（`MD1` 断言 uploadimg 只建模一次）。

5. **素材下载双通道（I3 裁决，勿并入生成管线）**：9~11 走 `IMpMediaDownloadService` 的
   `SendRawAsync` 原始响应直读 + **Content-Type 分支判错**，不进 JSON 反序列化管线：
   - JSON + `errcode != 0` → 抛 `MpException`；令牌失效码 `{40001, 40014, 42001}`（与 `MpTokenInvalidationDetector` 同源）
     先失效本应用 access_token 并**重试一次**。
   - JSON + `video_url`（`media/get` 对视频的官方形态）→ `MpMediaDownloadResult.VideoUrl`。
   - 其余 Content-Type → 文件流（`MpMediaDownloadResult.Content`，文件名取 `Content-Disposition`）。
   - `get_material` 三形态用 `MpPermanentMaterialResult` 承载；**SDK 不落盘**，流交宿主消费（结果对象实现 `IDisposable`）。
   - `MD11` 钉两条手工下载通道（本域 + 非税账单）的应用上下文解析须在环境上下文缺失时回退默认应用。

6. **服务器端调用**：官方原文「接口须在服务器端调用，不可在前端（小程序、网页、APP 等）直接调用」。

## 快速开始

```csharp
builder.Services.AddMpApp(builder.Configuration, "MpApps");
builder.Services.AddMpServices(b => b.AddMediaApi());

// 注入 IMpMediaService（JSON 通道，注册组 Media）与 IMpMediaDownloadService（下载通道）

// 1) 上传临时素材：type 走 Query，文件走 multipart（IFormContent 由调用方构建、流自持）
MpUploadTempMediaResponse uploaded = await media.UploadTempMediaAsync(MpMediaTypes.Image, form, ct);
MpException.ThrowIfFailed(uploaded, "/cgi-bin/media/upload");
string tempMediaId = uploaded.MediaId ?? string.Empty;   // 3 天有效，可复用

// 2) 永久素材列表：count 取值 1~20；条目按 type 分流（news / 非 news 两形态）
MpBatchGetMaterialResponse listed = await media.BatchGetMaterialAsync(new MpBatchGetMaterialRequest
{
    Type = MpMediaTypes.Image,
    Offset = 0,
    Count = 20,
}, ct);
MpException.ThrowIfFailed(listed, "/cgi-bin/material/batchget_material");
List<MpMaterialListItem> items = listed.Items ?? new List<MpMaterialListItem>();

// 3) 删除永久素材（临时素材不可经本接口删除）
MpException.ThrowIfFailed(
    await media.DelMaterialAsync(new MpMediaIdRequest { MediaId = permanentMediaId }, ct),
    "/cgi-bin/material/del_material");

// 4) 下载临时素材：响应可能是文件流或视频 video_url，走独立下载通道；结果对象须释放
using (MpMediaDownloadResult downloaded = await mediaDownload.DownloadTemporaryMediaAsync(tempMediaId, ct))
{
    string? videoUrl = downloaded.VideoUrl;   // 视频：非流；其余：Content 为文件流
    Stream? stream = downloaded.Content;       // SDK 不落盘，宿主自行读取
}

// 5) 永久素材三形态统一信封（图文 news_item / 视频 down_url / 图片·语音二进制流）
using (MpPermanentMaterialResult material = await mediaDownload.GetPermanentMaterialAsync(permanentMediaId, ct))
{
    IReadOnlyList<MpMaterialNewsItem>? news = material.NewsItems;
    string? downUrl = material.VideoDownUrl;
    Stream? binary = material.Content;
}
```

凭据红线：`access_token`、`appsecret`、`openid` 只作参数名，**不得**进日志、遥测或异常消息
（`MpException` 构造期已对含令牌的 URL 脱敏）。`media_id` 本身非秘密，但泄露可被复用，仍不建议写日志。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| `IMpMediaDownloadService.cs` **落位不一致** | 文件在包根 `Media/` 而非 `Interfaces/Media/` | 本文档链接用 `../../Media/...`；域 README 与接口目录分处两地的现状暂不改（改则触及其他引用），如需归位走单独批次 |
| 包 README「`Media/` 6（上传侧）」 | 与守卫 `MD1` 的 8 JSON 端点 + `MD2` 的 3 下载端点冲突 | 以守卫为准；回填包 README 前不引用其计数 |
| `MpModule.Media` remarks「上传 1 端点 + 下载 2 端点」 | 与 `MD1`/`MD2`（8 + 3）冲突，属待回填文本 | 以守卫为准，remarks 不作依据 |
| 官方类型大小「注意事项」与字段表不一致 | 注意事项写「图片 2MB / 视频 10MB」，字段表写图片 10M | SDK 采字段表口径，矛盾照录 remarks，不本地拦截 |
| `description` 字段必填性矛盾 | 官方字段表标可选、正文写「视频需要」 | 建模为可选参数，矛盾照录 `AddMaterialAsync` remarks |
| `get_material` 旧版页面含 `need_open_comment`/`only_fans_can_comment` | 新版页面无此两字段 ⇒ 按新版建模 | 若官方回补需同批改 `MpPermanentMaterialResult` 与 `MD2` 信封裁决 |
| Media DTO 数 | `MD5` 钉 `expectedCount = 15` 登记进 `MediaJsonContext`（`SerializerClassName = Media`） | 漂移先核对官方再同批改基线，勿只改数字 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐端点原文、3 天有效、类型大小、数量上限、双通道裁决 | [IMpMediaService.cs](./IMpMediaService.cs) 与 [../../Media/IMpMediaDownloadService.cs](../../Media/IMpMediaDownloadService.cs) 的 XML 注释 |
| 8 条 JSON 路由 / 只 1 GET / uploadimg 建模一次 / 3 下载路径常量 | `MD1` = `MediaEndpoints_ShouldMatchOfficialRoutes` |
| 下载通道不进管线（3 方法、信封形态、`IDisposable`） | `MD2` = `DownloadChannel_ShouldStayOutOfGeneratedPipeline` |
| 直接注册（无子接口）| `MD3` = `MediaInterfaceHierarchy_ShouldRegisterDirectly` |
| 令牌绑定 + 下载服务无 `[Token]` | `MD4` = `MediaTokenBinding_ShouldBeAccessTokenInjectedViaQuery` |
| DTO 登记 + `expectedCount = 15` | `MD5` = `MediaDataModels_ShouldBeRegisteredInJsonContext` |
| 官方字段名与共用 DTO（`MpMediaIdRequest` 共用 get/del） | `MD6` = `MediaDataModels_ShouldLockOfficialFieldsAndSharedDtos` |
| type 在 Query、文件在 multipart、uploadimg 无业务 Query | `MD7` = `UploadEndpoints_ShouldTakeTypeViaQueryAndFileViaMultipart` |
| `MpModule.Media` + `AddMediaApi()` + 白名单 | `MD8` = `MediaModule_ShouldBeRegisteredAndWhitelisted` |
| `40004` / `40007` / `40009` | `MD9` = `MediaErrorCodes_ShouldMatchVerifiedOfficialValues` |
| 下载通道 JSON 解释走源生成快车道 | `MD10` = `DownloadChannel_ShouldUseSourceGeneratedJsonTypeInfo` |
| 两条手工下载通道回退默认应用 | `MD11` = `ManualDownloadChannels_ShouldFallBackToDefaultApp` |
| 请求/响应字段名与类型 | [DataModels/Media/](../../../Mud.Wechat.OfficialAccount.DataModels/Media/)（15 个 DTO） |
| 下载结果信封 | [../../Media/MpMediaDownloadResult.cs](../../Media/MpMediaDownloadResult.cs)、[../../Media/MpPermanentMaterialResult.cs](../../Media/MpPermanentMaterialResult.cs) |
| 生成上下文清单 | [Generated/MediaJsonContext.g.cs](../../../Mud.Wechat.OfficialAccount.DataModels/Generated/MediaJsonContext.g.cs)、[MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与逐页限制，**同批**更新 `MD1` 的 JSON 路由表（8）与 `MD2` 的下载方法集，
   以及 [MpRouteCountGuard.cs](../../../../../Tests/Mud.Wechat.OfficialAccount.Tests/ContractGuards/MpRouteCountGuard.cs)（`RC1`/`RC2`）
   与 [../../../Tests/ContractBaseline.cs](../../../../../Tests/ContractBaseline.cs) 基线。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName = Media`）→ 跑
   [../../../../../scripts/AddHttpJsonSerializable.ps1](../../../../../scripts/AddHttpJsonSerializable.ps1) +
   [../../../../../scripts/GenerateJsonContext.ps1](../../../../../scripts/GenerateJsonContext.ps1)
   → 同批更新 `MD5` 的 `expectedCount`（15）。
3. 确认 [Generated/MediaJsonContext.g.cs](../../../Mud.Wechat.OfficialAccount.DataModels/Generated/MediaJsonContext.g.cs) 与
   [MpJsonResolverExtensions.cs](../../Extensions/MpJsonResolverExtensions.cs) 的合并清单**逐项对齐**——
   漏登记在 JIT 下无症状、只在 Native AOT 首次真实调用失败。
4. 新增/调整模块注册面 → 同批改 [MpModule.cs](../../Extensions/MpModule.cs)、[MpServiceBuilder.cs](../../Extensions/MpServiceBuilder.cs)
   的 `AddMediaApi()`，并同步 `QT1` 白名单（下载服务不入白名单）。回填 `MpModule.Media` remarks 与包 README 的过期计数。
5. **不要**把下载端点折进生成管线（破坏 `MD2`）；**不要**据「注意事项」的 2MB 本地拦截上传（口径采字段表）；
   **不要**给 `IMpMediaDownloadService` 加 `[Token]`。
6. 最后跑 [../../../../../scripts/verify-build.ps1](../../../../../scripts/verify-build.ps1)，全绿才算完成。
