# Images 域 · 图片素材

腾讯广告 Marketing API v3.0「图片素材」域：声明式 **3 支端点**（`images/get|update|delete`）
+ **1 支 multipart 手写上传通道**（`images/add`）。
接口 [IWechatAdsImageService.cs](./IWechatAdsImageService.cs) 与
[IWechatAdsImageUploadService.cs](./IWechatAdsImageUploadService.cs)，注册组 `Images`，
模块枚举 `AdsModule.Images`（`AddImagesApi()` 同批装配声明式三支与上传通道）。逐页核验 2026-10-11。

> 事实来源限定：逐参数原文以两份接口文件的 XML 注释为准；路由 / 计数 / 上传路径以
> [AdsContractGuards.cs](../../../../../Tests/Mud.Wechat.Ads.Tests/ContractGuards/AdsContractGuards.cs) 为准。

## 端点速览

| 通道 | 方法 | 动词 | 路由 | 权限 | `user_token` |
|---|---|---|---|---|---|
| 声明式 | `GetAsync` | GET | `/v3.0/images/get` | `ads_management,account_management` | 无 |
| 声明式 | `UpdateAsync` | POST | `/v3.0/images/update` | `ads_management` | 无 |
| 声明式 | `DeleteAsync` | POST | `/v3.0/images/delete` | `ads_management` | 无 |
| **手写** | `UploadAsync` | POST | `/v3.0/images/add`（`multipart/form-data`） | 本页权限未留档（缺口） | 无 |

- **本域四页一个 `user_token` 都不带**：官方没有任何一页列「特定请求参数」表 ⇒
  `RestrictedWriteSurfaces_ShouldCarryUserToken_WhenOfficialPageListsIt` 用反射面**反向**断言
  （出现 `user_token` 即与官方不符）。
- **`get` 页权限是两值并列**（`ads_management,account_management`），`update` / `delete` 只有
  `ads_management` ⇒ 与「同域权限同一」的 Funds 域不同，本域**跨权限族**。
- 凭据（`access_token` + `timestamp` + `nonce`）恒由 `AdsAuthorizationHandler` 成组注入，
  声明式与手写通道**同一注入语义**（守卫 `ADS-B1`：禁止声明式 `[Token]`）。

## 上传通道为什么不在声明式接口里

官方 `images/add` 的 `Content-Type` 是 `multipart/form-data`（文件字段 `file`），声明式
`[HttpClientApi]` 面承载不了文件流（组件对复杂类型走「逐属性展平」，无文件部分语义）⇒
走手写 `IEnhancedHttpClient` 传输，实现类 `AdsMaterialUploadService`
（**图片与视频两支上传共用同一实现类**，两个 `ServiceType` 各持实例）。
它不经 `[HttpClientApi]`、没有路由反射面 ⇒ 路径字面量锁在实现类私有常量 `ImagesAddPath` 上，
由 `MaterialUploadChannels_ShouldMatchOfficialPaths` 取值断言（改名即红）。

## `image_id` 是字符串，不是整数

`AdsImageInfo.ImageId` / `AdsImageUpdateRequest.ImageId` / `AdsImageDeleteRequest.ImageId` /
上传应答 `AdsImageUploadResult.ImageId` 全部是 `string?`（官方本页即字符串形态）；
而**邻域 videos 的 `video_id` 是 `long?`**（见 [../Videos/README.md](../Videos/README.md)）。
两支素材 id 在跨域聚合（如 `async_tasks` 的清理 spec：`image_id_list` 是 `List<string>`、
`video_id_list` 是 `List<string>`、`component_id_list` 是 `List<long>`）时**不得互转**，
类型以官方逐页原文为准，不由本 SDK 「顺手统一」。

## 查询面的八个参数全部可空

`GetAsync` 的 `account_id` / `organization_id` / `filtering` / `page` / `page_size` /
`label_id` / `business_scenario` / `need_aigc_flag` 在签名上**全部为可空默认参数**，
序列化按「不传即缺席」处理 ⇒ 本域允许「只带凭据裸查」。要点：

- `filtering` 是 **JSON 数组字符串**，构造入口唯一 `AdsQueryJson.Filtering`（复合 Query 参数
  只能是单个 JSON 字面量，守卫 `ADS-B2` 锁键名）。
- `need_aigc_flag`（官方 `boolean`）决定是否回 `aigc_flag` ⇒ 不传即不返回，**不是**返回 false。
- 应答 `data.list[]` 为 **21 字段**（含 `sample_aspect_ratio` / `similarity_status` /
  `owner_account_id` 等），另有 `page_info` 分页元信息。

## 写面只改描述，删面必填集更窄

| 端点 | 官方键数 | 必填 | 应答 `data` |
|---|---|---|---|
| `update` | 4（`account_id` / `organization_id` / `image_id` / `description`） | `image_id` + `description` | 只回 `image_id` |
| `delete` | 3（`account_id` / `organization_id` / `image_id`） | **仅 `image_id`** | 只回 `image_id` |

`delete` 的必填集比 `videos/delete`（`account_id` + `video_id` 双必填）**窄一支** ⇒ 这是官方两页
的逐页事实，不是本仓漏标；照视频域口径给删图片补 `account_id` 必填反而与官方不符。
两支写端点都是「官方动词 POST 的修改/删除」，路由严格按 `/v3.0/images/{update,delete}` 原文，
不做 RESTful 改写（守卫 `ADS-B2`）。

## 上传通道的表单面

| 表单字段 | 事实 |
|---|---|
| `account_id` / `organization_id` | 二者按调用场景给（本方法参数为可空） |
| `upload_type` | 必填 `enum`，二选一：`UPLOAD_TYPE_FILE`（配文件流）/ `UPLOAD_TYPE_BYTES`（配 base64 串）。常量在 `AdsMaterialUploadService.UploadTypeFile` / `UploadTypeBytes`，值照官方枚举原文 |
| `signature` | 必填，**MD5 内容摘要**（非凭据，可入日志；异常消息只带端点路径与信封码） |
| `file` / `fileName` | `UPLOAD_TYPE_FILE` 时必填。文件流挂到 multipart 的文件部分，**不整体读入内存** |
| `bytes` | `UPLOAD_TYPE_BYTES` 时必填，base64 文本 |
| `image_usage` | 选填 `enum`；**本页可选值未逐项核验** ⇒ 不建枚举、取值以应答与官方页为准 |
| `description` | 选填 |
| `resize_width` / `resize_height` / `resize_file_size` | 选填 `integer`，官方缩放参数 |

**上传方式二选一是本地校验的例外**：`AdsMaterialUploadService` 对「`upload_type` 为空」
「`signature` 为空」「`UPLOAD_TYPE_FILE` 未给流」「`UPLOAD_TYPE_BYTES` 未给字节串」四条
**抛 `ArgumentException`**（参数形状错，不消耗一次网络往返）。这与声明式面「条件必填一律不本地拦截」
的口径**不冲突**——条件必填指的是官方跨字段的业务约束，此处是本通道自身的入参契约。
官方尺寸上限未逐项核验（图片素材一般 ≤10MB），越界仍由应答 `code` 表达。

**流生命周期**：`UploadAsync` **不关闭**调用方传入的流 ⇒ 由调用方 `using` 或显式释放。

## 快速开始

```csharp
builder.Services.AddAdsApp(builder.Configuration, "WechatAds");
builder.Services.AddWechatAdsApi(b => b.AddImagesApi());  // 声明式三支 + 上传通道同批装配

// 1) 上传（文件方式）
using var fs = File.OpenRead("banner.jpg");
var uploaded = await imageUpload.UploadAsync(
    accountId: 1234567890,
    uploadType: AdsMaterialUploadService.UploadTypeFile,
    signature: md5OfBanner,          // 官方必填：内容 MD5
    file: fs,
    fileName: "banner.jpg",
    cancellationToken: ct);
WechatAdsException.ThrowIfFailed(uploaded, "/v3.0/images/add");
var imageId = uploaded.Data!.ImageId;   // string，勿当整数用

// 2) 查列表（filtering 走唯一入口）
var filtering = AdsQueryJson.Filtering(new AdsFiltering
{
    Field = "image_id",
    Operator = "IN",
    Values = [imageId!],
});
var list = await images.GetAsync(1234567890, filtering: filtering, pageSize: 100, cancellationToken: ct);
WechatAdsException.ThrowIfFailed(list, "/v3.0/images/get");

// 3) 删除（官方仅 image_id 必填）
var deleted = await images.DeleteAsync(new AdsImageDeleteRequest { ImageId = imageId }, ct);
WechatAdsException.ThrowIfFailed(deleted, "/v3.0/images/delete");
```

**判错只有一层**：本域四支都是普通信封应答（`code == 0` 即成功），**没有**批量族/异步族的
「逐条 `code`」双层语义 ⇒ 不要照 `adgroups` 批量支去 `data.list[i]` 里找 `code`。

## 逐页集合差异（合并即抹平官方差异）

| 面 | `images/get` | `images/add` | 邻域对照 |
|---|---|---|---|
| 素材 id 类型 | `string` | 应答 `image_id` 为 `string` | `videos` 两支均为 `long` |
| 权限 | `ads_management,account_management` | 未留档（缺口） | `videos` 四页一律 `ads_management` |
| 删除必填 | 仅 `image_id` | — | `videos/delete` 需 `account_id` + `video_id` |
| 上传方式 | — | `upload_type` 二选一 | `videos/add` **无** `upload_type`，只有文件流 |
| 应答字段数 | 21 | 9 | `videos/get` 35 |

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| `images/add` 页「所属权限」 | 本仓接口 remarks **未摘录**（只记了 get/update/delete 三页） | 重新逐页核验该页 → 补 `IWechatAdsImageUploadService` remarks → 同批更新本文 |
| `image_usage` 可选值 | **未逐项核验** ⇒ 以 `string` 传，不建枚举 | 需要用途筛选时先核验官方页，不得从邻域借枚举 |
| 上传尺寸上限 | 官方逐项上限**未核验**（素材经验值 ≤10MB）⇒ 不本地拦截 | 越界由应答 `code` 表达，不引入本地大小闸 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文 | 本目录两份接口文件的 XML 注释 |
| 声明式路由 / 计数（恰 3） | `ImageAndVideoEndpoints_ShouldMatchOfficialRouteTable`（`ADS-B2`）+ `ImagesRoutes` |
| 上传通道路径常量 | `MaterialUploadChannels_ShouldMatchOfficialPaths` |
| 「本域无 `user_token`」的反证 | `RestrictedWriteSurfaces_ShouldCarryUserToken_WhenOfficialPageListsIt` |
| DTO JSON 名不得改 | `AdsDataModelJsonNames_ShouldStayOfficialSnakeCase` |
| 官方文档 | `https://developers.e.qq.com/v3.0/docs/api/{images/get,images/add,images/update,images/delete}` |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与限制，**同批**更新 `ImagesRoutes` 与计数断言（3）。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `Images`）→ 跑
   `AddHttpJsonSerializable.ps1` + `GenerateJsonContext.ps1` → 更新登记断言（守卫 `ADS-B6`）。
3. 改上传通道 → 路径常量、表单字段名、二选一的本地校验三处须同批；本文的表单表同步。
4. **不要**给本域任何端点加 `user_token`；**不要**把 `image_id` 改成整数「对齐」视频域。
5. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。
