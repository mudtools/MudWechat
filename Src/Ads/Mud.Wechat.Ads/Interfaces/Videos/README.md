# Videos 域 · 视频素材

腾讯广告 Marketing API v3.0「视频素材」域：声明式 **3 支端点**（`videos/get|update|delete`）
+ **1 支 multipart 手写上传通道**（`videos/add`）。
接口 [IWechatAdsVideoService.cs](./IWechatAdsVideoService.cs) 与
[IWechatAdsVideoUploadService.cs](./IWechatAdsVideoUploadService.cs)，注册组 `Videos`，
模块枚举 `AdsModule.Videos`（`AddVideosApi()` 同批装配声明式三支与上传通道）。逐页核验 2026-10-11。

> 事实来源限定：逐参数原文以两份接口文件的 XML 注释为准；路由 / 计数 / 上传路径以
> [AdsContractGuards.cs](../../../../../Tests/Mud.Wechat.Ads.Tests/ContractGuards/AdsContractGuards.cs) 为准。

## 端点速览

| 通道 | 方法 | 动词 | 路由 | 权限 | `user_token` |
|---|---|---|---|---|---|
| 声明式 | `GetAsync` | GET | `/v3.0/videos/get` | `ads_management` | 无 |
| 声明式 | `UpdateAsync` | POST | `/v3.0/videos/update` | `ads_management` | 无 |
| 声明式 | `DeleteAsync` | POST | `/v3.0/videos/delete` | `ads_management` | 无 |
| **手写** | `UploadAsync` | POST | `/v3.0/videos/add`（`multipart/form-data`） | `ads_management` | 无 |

- **四端点「所属权限」均为 `ads_management`**（本域同权限族，不像 Images 的 `get` 页跨两族）。
- **四页均未另列 `user_token`** ⇒ 一个都不带；`RestrictedWriteSurfaces_ShouldCarryUserToken_WhenOfficialPageListsIt`
  对 `IWechatAdsVideoService` 做反射面**反向**断言。
- 全局参数 `access_token` + `timestamp` + `nonce` 由 `AdsAuthorizationHandler` 成组注入；手写通道
  注入的是带同一 Handler 的 `IAdsHttpClient`，注入语义与声明式面**完全一致**（守卫 `ADS-B1`）。

## 上传通道为什么不在声明式接口里

官方 `videos/add` 的 `Content-Type` 是 `multipart/form-data`，**文件字段名 `video_file`**（与
Images 的 `file` 不同名），声明式面承载不了文件流 ⇒ 走手写传输，实现类与图片共用
`AdsMaterialUploadService`（一个类同时 assignable 到 `IWechatAdsImageUploadService` 与
`IWechatAdsVideoUploadService`，两个 `ServiceType` 各持实例）。
路径字面量锁在私有常量 `VideosAddPath`，由 `MaterialUploadChannels_ShouldMatchOfficialPaths` 取值断言。

## 应答是 35 字段的「编解码双面」

`videos/get` 的 `data.list[]` 共 **35 字段**，是本线最宽的素材条目，成对出现的编解码面：

| 面 | 字段 |
|---|---|
| 视频侧 | `video_codec` / `video_bit_rate` / `video_frames` / `video_fps` / `video_profile_name` |
| 音频侧 | `audio_codec` / `audio_bit_rate` / `audio_sample_rate` / `audio_profile_name` |
| 关键帧 | `max_keyframe_interval` / `min_keyframe_interval` / `key_frame_image_url` |
| 时长 | `image_duration_millisecond` / `audio_duration_millisecond` |
| 其它 | `scan_type` / `sample_aspect_ratio` / `similarity_status` / `system_status` / `status` / `aigc_flag` |

**`video_fps` 是 `decimal?`**（本线唯一的十进制字段，帧率非整数）⇒ 不要按 `long` 收。
**`video_id` / `cover_image_id` 是 `long?`，但条目里的封面键是 `cover_id` 且为 `string?`**：
官方两页形态不同（上传回 `cover_image_id` 整数、查询回 `cover_id` 字符串），
建模照逐页原文，**不「顺手统一」**（同 Images 的 `image_id` 是 `string`，两支素材 id 类型不对称）。

## 写面必填集：与 Images 相反的一侧

| 端点 | 官方键数 | 必填 | 应答 `data` |
|---|---|---|---|
| `update` | 4（`account_id` / `organization_id` / `video_id` / `description`） | `video_id` + `description` | 只回 `video_id` |
| `delete` | 3（`account_id` / `organization_id` / `video_id`） | **`account_id` + `video_id`** | 只回 `video_id` |

`videos/delete` 双必填、`images/delete` 仅 `image_id` 必填 —— **两支删除端点必填集不同构**，
是官方逐页事实。照图片域口径把 `account_id` 留空去删视频会被官方以 `code` 拒绝。
「修改视频描述」与「删除视频」官方动词都是 POST，路由按原文 `/v3.0/videos/{update,delete}`。

## 查询面的八个参数全部可空

`GetAsync` 的 `account_id` / `organization_id` / `filtering` / `page` / `page_size` / `label_id` /
`business_scenario` / `need_aigc_flag` 全为可空默认参数，形态与 `images/get` 逐参数同构。
`filtering` 是 JSON 数组字符串，入口唯一 `AdsQueryJson.Filtering`；`need_aigc_flag` 不传即不回 `aigc_flag`。

## 上传通道的表单面

| 表单字段 | 事实 |
|---|---|
| `account_id` / `organization_id` | 按调用场景给（参数可空，空值**不上送**——对齐官方「选填不送空串」口径） |
| `signature` | **必填**，MD5 内容摘要（非凭据） |
| `video_file` | **必填**文件流，参数名 `videoFile`；本方法**不关闭**调用方传入的流 |
| `fileName` | 必填，作为 `video_file` 部分的 filename |
| `description` | 选填 |
| `adcreative_template_id` | 选填 `integer`（创意形式 id） |

**与 `images/add` 的三处不同**：① 无 `upload_type` 二选一（官方本支只有文件流形态）；
② 文件字段名 `video_file` 而非 `file`；③ 无 `image_usage` / `resize_*` 缩放面，取而代之
`adcreative_template_id`。
官方尺寸上限未逐项核验（视频素材一般 ≤10MB），越界由应答 `code` 表达；通道只做
`signature` 非空、`videoFile` 非 null、`fileName` 非空三条入参契约校验。
上传应答 `data` 恰 2 字段：`video_id` + `cover_image_id`（官方自动抽帧的封面）。

## 快速开始

```csharp
builder.Services.AddAdsApp(builder.Configuration, "WechatAds");
builder.Services.AddWechatAdsApi(b => b.AddVideosApi());  // 声明式三支 + 上传通道同批装配

// 1) 上传视频（通道不关闭传入的流）
using var fs = File.OpenRead("spot.mp4");
var uploaded = await videoUpload.UploadAsync(
    accountId: 1234567890,
    signature: md5OfSpot,            // 官方必填：内容 MD5
    videoFile: fs,
    fileName: "spot.mp4",
    adcreativeTemplateId: 1023,
    cancellationToken: ct);
// 通道内部已 ThrowIfFailed，成功才返回；data 只回 video_id + cover_image_id
long videoId = uploaded.Data!.VideoId!.Value;

// 2) 改描述（官方 4 键，必填 video_id + description）
var updated = await videos.UpdateAsync(new AdsVideoUpdateRequest
{
    AccountId = 1234567890,
    VideoId = videoId,
    Description = "春季促销 15s",
}, ct);
WechatAdsException.ThrowIfFailed(updated, "/v3.0/videos/update");
```

**判错只有一层**：本域四支均为普通信封应答，`code == 0` 即成功，**没有**逐条 `code` 的双层语义。

## 逐页集合差异（合并即抹平官方差异）

| 面 | `videos/get` | `videos/add` | 邻域对照（Images） |
|---|---|---|---|
| 素材 id 类型 | `video_id` 为 `long` | 应答 `video_id` / `cover_image_id` 为 `long` | `image_id` 两支均为 `string` |
| 封面 id | 条目 `cover_id` 为 `string` | — | 无对应字段 |
| 帧率类型 | `video_fps` 为 `decimal` | — | 无对应字段 |
| 删除必填 | — | — | `images/delete` 仅 `image_id` |
| 上传方式 | — | 只有文件流，**无** `upload_type` | `upload_type` 二选一 |
| 文件字段名 | — | `video_file` | `file` |
| 应答字段数 | 35 | 2 | 21 / 9 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文 | 本目录两份接口文件的 XML 注释 |
| 声明式路由 / 计数（恰 3） | `ImageAndVideoEndpoints_ShouldMatchOfficialRouteTable`（`ADS-B2`）+ `VideosRoutes` |
| 上传通道路径常量、两域共用实现 | `MaterialUploadChannels_ShouldMatchOfficialPaths` |
| 「本域无 `user_token`」的反证 | `RestrictedWriteSurfaces_ShouldCarryUserToken_WhenOfficialPageListsIt` |
| 35 / 2 字段的实际建模面 | `AdsVideoInfo` / `AdsVideoUploadResult`（`Src/Ads/Mud.Wechat.Ads/DataModels/Videos/`） |
| 官方文档 | `https://developers.e.qq.com/v3.0/docs/api/{videos/get,videos/add,videos/update,videos/delete}` |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与限制，**同批**更新 `VideosRoutes` 与计数断言（3）。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `Videos`）→ 跑两个脚本 →
   更新登记断言（守卫 `ADS-B6`）。
3. 改上传通道 → `VideosAddPath`、`video_file` 字段名、三条入参校验须同批；本文表单表同步。
4. **不要**给本域加 `user_token`；**不要**把 `video_id` 改成字符串去「对齐」图片域；
   **不要**把 `images/delete` 的单必填口径套到 `videos/delete`。
5. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。
