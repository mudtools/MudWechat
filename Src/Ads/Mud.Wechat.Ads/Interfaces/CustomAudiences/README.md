# CustomAudiences 域 · 人群管理

腾讯广告 Marketing API v3.0「人群管理」域：声明式 **4 支端点**（跨 `custom_audiences/*` 与
`custom_audience_files/get` 两支资源族）+ **1 支 multipart 手写上传通道**（`custom_audience_files/add`）。
接口 [IWechatAdsCustomAudienceService.cs](./IWechatAdsCustomAudienceService.cs) 与
[IWechatAdsCustomAudienceFileUploadService.cs](./IWechatAdsCustomAudienceFileUploadService.cs)，
注册组 `CustomAudiences`，模块枚举 `AdsModule.CustomAudiences`。逐页核验 2026-10-17。

> 事实来源限定：逐参数原文以两份接口文件的 XML 注释为准；路由 / 计数 / 上传路径以
> [AdsContractGuards.cs](../../../../../Tests/Mud.Wechat.Ads.Tests/ContractGuards/AdsContractGuards.cs) 为准。

## 端点速览

| 通道 | 方法 | 动词 | 路由 |
|---|---|---|---|
| 声明式 | `AddAsync` | POST | `/v3.0/custom_audiences/add` |
| 声明式 | `GetAsync` | GET | `/v3.0/custom_audiences/get` |
| 声明式 | `UpdateAsync` | POST | `/v3.0/custom_audiences/update` |
| 声明式 | `GetFilesAsync` | GET | `/v3.0/custom_audience_files/get` |
| **手写** | `UploadAsync` | POST | `/v3.0/custom_audience_files/add`（`multipart/form-data`） |

- **权限**：五页「所属权限」均为官方原文 `audience_management,ads_management,user_actions`（**三者任一**）。
- **五页均未另列 `user_token`** ⇒ 一个都不带（凭据仍由传输层成组注入 `access_token`/`timestamp`/`nonce`，`ADS-B1`）。
- **官方目录树另有 `custom_audiences/delete`**，本 SDK **未消费** ⇒ 不入路由表、不注册。
  守卫把计数钉成「恰 4」，落地该支时须同批改计数（4 → 5）。

## 上传通道为什么不在声明式接口里

官方 `custom_audience_files/add` 的 `Content-Type` 为 `multipart/form-data`（文件字段 `file`），
声明式 `[HttpClientApi]` 面承载不了文件流 ⇒ 走手写 `IEnhancedHttpClient` 传输（形态照素材两域的
`AdsMaterialUploadService`）。它**不经** `[HttpClientApi]`，没有路由反射面 ⇒ 路径字面量锁在实现类
`AdsCustomAudienceFileUploadService` 的私有常量 `FilesAddPath` 上，由
`MaterialUploadChannels_ShouldMatchOfficialPaths` 取值断言（改名即红）。

## `type` 与 `audience_spec` 的互斥映射

`custom_audiences/add` 必填 `account_id` / `name` / `type`。`type` 六支取值决定 `audience_spec` 里哪一支子 spec 生效：

| `type` | 生效子 spec（`AdsAudienceSpec`） |
|---|---|
| `LOOKALIKE` | `lookalike_spec` |
| `USER_ACTION` | `user_action_spec` |
| `KEYWORD` | `keyword_spec` |
| `AD` | `ad_rule_spec` |
| `COMBINE` | `combine_spec`（官方：最多允许用 **500** 个人群做组合） |
| `CUSTOMER_FILE` | **无 spec**——号码包经 `custom_audience_files/add` 上传 |

互斥校验交官方应答 `code` 表达，SDK 不做本地拦截（`ADS` 拼写以 DTO 文档原文为准）。

## `add` 的三条命名与幂等事实

- `name`：**1–32 字节**，**同帐号不许重名**。
- `outer_audience_id`：1–128 字节、只能数字/字母/下划线；官方约束「人群编码**不能与标签编码 `tag_code` 重复**」；
  该编码创建过人群时，重复创建**返回之前的 `audience_id`**（官方原文）⇒ 这是本域的业务幂等键。
- `description`（add 页）：0–**250** 字节。

## `update` 与 `add` 不同构（两处必踩）

1. 必填集是 `account_id` / `audience_id`，另有官方约束「**人群名称、人群描述、是否深度合作至少填写一个**」
   ⇒ 条件必填，SDK 不本地拦截。
2. `description` 在本页上限 **100** 字节，与 add 页的 250 **不同构** ⇒ 按 add 口径构造超长描述会被官方拒绝。
3. `cooperated`（是否深度合作）**仅创建者可改**（官方原文）。本页 `data` 为空载荷。

## 查询与文件列表的「条件出现」字段

- `custom_audiences/get`：`audience_id` 选填（不填即查全量分页），`page` ≥1 默认 1、`page_size` 1–100 默认 10。
  条目 `online_status` **仅在处理状态为成功时返回和生效**；`audience_spec` **仅回显与 `type` 相关的子 spec**。
- `custom_audience_files/get`：`audience_id` 官方原文「**只能是 `CUSTOMER_FILE` 类人群**」；
  `custom_audience_file_id` 来自上传通道。
  **双层判定**：文件处理结果看条目的 `process_code`（`0` 成功）与 `process_status`，
  外层信封 `code == 0` 只表示**查询成功** ⇒ 只看外层会把「处理失败的文件」当成正常。

## 上传通道的表单面

| 表单字段 | 事实 |
|---|---|
| `account_id` / `audience_id` | 必填；`audience_id` 只能是 `CUSTOMER_FILE` 类人群 |
| `user_id_type` | 必填 `enum`，17 支：`{ HASH_IDFA, HASH_IMEI, HASH_MOBILE_PHONE, IDFA, IMEI, WX_OPENID, WX_UNIONID, WECHAT_OPENID, SALTED_HASH_IMEI, SALTED_HASH_IDFA, OAID, HASH_OAID, SHA256_MOBILE_PHONE, MD5_SHA256_IMEI, MD5_SHA256_IDFA, MD5_SHA256_OAID, CAID }` |
| `file` | 必填文件流：txt 或 csv（每行一个号码），或**压缩后的 zip 包，不超过 100M**；大小判定不依赖本地拦截（流长度语义因流类型而异），越界由应答 `code` 表达 |
| `operation_type` | 选填，不填默认 `APPEND`；可选值 `{ APPEND, REDUCE }` |
| `open_app_id` | 选填；官方原文「当且仅当数据文件中的用户 id 类型为 `WECHAT_OPENID`（`WX_OPENID`）时有效」 |

**流生命周期**：`UploadAsync` **不关闭**调用方传入的流（与素材两域上传通道同口径）⇒ 由调用方 `using` 或显式释放。
应答 `data.custom_audience_file_id` 是后续 `custom_audience_files/get` 的过滤依据。

## 快速开始

```csharp
builder.Services.AddAdsApp(builder.Configuration, "WechatAds");
builder.Services.AddWechatAdsApi(b => b.AddCustomAudiencesApi());  // 声明式四支 + 上传通道同批装配

// 1) 建 CUSTOMER_FILE 类人群
var created = await audiences.AddAsync(new AdsCustomAudienceAddRequest
{
    AccountId = 1234567890,
    Name = "高价值号码包",
    Type = "CUSTOMER_FILE",
}, ct);
WechatAdsException.ThrowIfFailed(created, "/v3.0/custom_audiences/add");

// 2) 传号码包（流由调用方持有并释放；UploadAsync 不关闭传入的流）
using var fs = File.OpenRead("phones.csv");
var uploaded = await audienceFiles.UploadAsync(
    1234567890,                          // account_id（表单字段，必填）
    created.Data!.AudienceId!.Value,     // audience_id（只能是 CUSTOMER_FILE 类人群）
    userIdType: "HASH_MOBILE_PHONE",
    file: fs,
    fileName: "phones.csv",
    cancellationToken: ct);

// 3) 轮询处理结果：判条目 process_code，不判外层 code
var files = await audiences.GetFilesAsync(1234567890, created.Data!.AudienceId, cancellationToken: ct);
```

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（含 17 支 `user_id_type`） | 本目录两份接口文件的 XML 注释 |
| 声明式路由 / 计数（恰 4） | `CustomAudiencesEndpoints_ShouldMatchOfficialRouteTable`（`ADS-B2`）+ `CustomAudiencesRoutes` |
| 上传通道路径常量 | `MaterialUploadChannels_ShouldMatchOfficialPaths` |
| `type` ↔ 子 spec 互斥映射 | `AdsAudienceSpec` remarks（`.DataModels/CustomAudiences/`） |
| 官方文档 | `https://developers.e.qq.com/v3.0/docs/api/{custom_audiences/add,custom_audiences/get,custom_audiences/update,custom_audience_files/add,custom_audience_files/get}` |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方 URL 与限制，**同批**更新 `CustomAudiencesRoutes` 与计数断言。
2. 要落地 `custom_audiences/delete` → 逐页核验后补接口 + 路由表计数（4 → 5）+ 本文与接口 remarks 的「未消费」表述。
3. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `CustomAudiences`）→ 跑两个脚本 → 更新登记断言。
4. **不要**给本域任何端点加 `user_token`；**不要**把 add 页的 `description` 上限套到 update 页。
5. 最后跑 `scripts/verify-build.ps1` 与 `scripts/audit-config-keys.ps1`，全绿才算完成。
