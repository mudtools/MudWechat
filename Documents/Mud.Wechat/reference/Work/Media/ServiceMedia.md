# 素材管理 · 服务商通道上传临时素材（仅第三方应用）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Media` 域 · 服务商上传临时素材族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkServiceMediaService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Media/IWechatWorkServiceMediaService.cs)（父，**零端点**、`IsAbstract = true` 不可注入）· [IWechatWorkThirdPartyServiceMediaService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Media/IWechatWorkThirdPartyServiceMediaService.cs)（**唯一**子接口，承载全部 1 支） |
| DTO 源文件 | [DataModels/Media/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Media/)（与公共面**共用同一族 DTO**：响应 `UploadMediaResponse`，`SerializerClassName = Media` → `MediaJsonContext`） |
| 契约守卫 | [WechatMediaContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatMediaContractGuards.cs)（`ServiceMediaEndpoints_ShouldMatchOfficialRoutes` 路由 + `attachment_type` 必须选填 / `ServiceMediaFamily_ShouldHaveExactlyThirdPartyChildAndAbstractParent` 继承链上**只有**第三方子接口 / `MediaTokenBinding_ShouldBeAccessAndProviderTokenInjectedViaQuery` 本族 `ProviderAccessToken` / `MediaDataModels_ShouldBeRegisteredInJsonContext`）＋ G5 Query 令牌白名单 ＋ TO1（非 `AccessToken` 族**不得**叠加归属域键） |
| 官方文档 | 服务商上传临时素材 [99310](https://developer.work.weixin.qq.com/document/path/99310)（**仅第三方应用文档树有本页**；自建与代开发无对应页） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对，并用 `EmitCompilerGeneratedFiles` 复测了生成体；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 24 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（服务商通道上传，复用 [Media.md](./Media.md) 的 `MediaUploadForm` 宿主类型））；本批 25 段、全仓累计 150 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：MD1b/MD2b/MD3/MD4 + TO1 断言 + 接口与 DTO XML 注释 + 生成体实测 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Media/README.md) · 官方：本批未重新取文 |

## 1. 这个接口族做什么

服务商（第三方应用）**以自己的 `provider_access_token`** 上传一个临时素材，换回 `media_id`——它与三类应用公共面的「上传临时素材」是**两条独立的通道**，不是同一端点的两种鉴权。1 支端点：

| 端点 | 用途 |
|---|---|
| E1 `POST /cgi-bin/service/media/upload` | multipart 上传 `image` / `voice` / `video` / `file`，返回 `type` / `media_id` / `created_at` |

形态：**零端点父接口 + 唯一第三方子接口承载**（MD2b 断言继承链上恰等于 `{ IWechatWorkThirdPartyServiceMediaService }`，自建 / 代开发子接口**不存在**，形态对齐 `IWechatWorkIdentitySuiteService` 与微信客服组件域）。这不是漏挂，而是官方无对应文档页 ⇒ **不得**为「对齐三类应用」补 `IWechatWorkInternalServiceMediaService` / `IWechatWorkProviderServiceMediaService`。

**本族只有上传腿**：官方未开放服务商版的「获取临时素材 / 上传图片 / 高清语音 / 异步上传」，取回素材仍走企业侧 `access_token` 的 [Media.md](./Media.md) E2。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | 仅 `WechatAppType.ThirdParty` | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.ProviderAccessToken` → Query `provider_access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险：官方契约强制 Query，无法改 Header） | 每次请求 |
| 令牌归属键 | **不声明 `TokenManagerKey`**：`ProviderAccessToken` 族凭据来源唯一、族内无错配空间，TO1 反向锁定「不得叠加 `@Internal` / `@Corp`」 | — |

`provider_access_token` 是**服务商级凭据**（跨全部授权企业共享一份，不区分 `authCorpId`）⇒ **调用前不需要 `UseCorpScope`**，也**不该**进入日志、遥测或异常消息；文档与排查一律只写参数名。

### 2.2 权限与业务前置

- 须是**第三方应用（套件）**并已取得服务商令牌；企业自建应用与服务商代开发**官方不开放本端点**。
- 量纲闸门（接口注释原文）：所有文件**必须大于 5 个字节**；`image` **2MB**、仅 JPG/PNG；`voice` **2MB**、播放长度 ≤60s、仅 AMR；`video` **10MB**、仅 MP4；`file` **20MB**。
  ⇒ **图片上限与公共面不同**（公共面 `media/upload` 为 10MB，本端点为 2MB），不要照抄公共面的数值。
- `filename` 规则（本族注释独有）：**不能以正斜线（`/`）开始，不能包含反斜线（`\`）与冒号（`:`）**。
- `attachment_type`（官方选填）：仅特定场景使用（注释举例 **3 - 收银台**），普通场景不指定。
- `media_id` **三天内有效、同一企业内应用之间可共享**（注释口径）；它属可换回内容的标识，**不要落日志**。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 `service/media/upload` | ❌ | ✅ | ❌ |

## 3. 接口族与注册

```csharp
// Program.cs：多应用底座先行，再注册素材管理模块（服务商通道与公共面同组）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddMediaApi());
```

| 项 | 值 |
|---|---|
| 模块枚举 | `WechatModule.Media` |
| 注册入口 | `WechatWorkServiceBuilder.AddMediaApi()`（→ 源生成 `AddMediaWebApiHttpClient()`） |
| 注册组名 | `"Media"`（**与公共面三支子接口同组**，MD2b 锁定；不为服务商通道另立模块） |
| 可注入接口 | `IWechatWorkThirdPartyServiceMediaService`（命名空间 `Mud.Wechat.Work`，`InheritedFrom = WechatWorkServiceMediaService`） |
| 不可注入 | `IWechatWorkServiceMediaService`（零端点父，命名空间 `Mud.Wechat.Work.Interfaces`，`RegistryGroupName` 空） |

## 4. 端点契约明细

### E1. `UploadServiceMediaAsync` — 服务商上传临时素材

**签名**

```csharp
[Post("/cgi-bin/service/media/upload")]
Task<UploadMediaResponse> UploadServiceMediaAsync(
    [Query("type")] string type,
    [MultipartForm] IFormContent formData,
    [Query("attachment_type")] int? attachmentType = null,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/service/media/upload` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/service/media/upload?type=TYPE&attachment_type=N&provider_access_token=PROVIDER_ACCESS_TOKEN` |
| 凭据参数 | `provider_access_token`（Query 注入，SDK 自动完成；服务商级凭据） |
| 适用应用类型 | **仅第三方应用** |
| Content-Type | `multipart/form-data`，**文件字段名必须为 `media`**，须含 `filename` / 长度 / `content-type` |

**请求参数**

| 字段 | 位置 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|---|
| `type` | Query | — | `string` | **是** | `image` / `voice` / `video` / `file` |
| — | Form | `formData` | `IFormContent` | **是** | 文件本体，由**宿主实现** `Mud.HttpUtils.IFormContent`（字段名 `media`；`filename` 受 2.2 的斜线/冒号规则约束） |
| `attachment_type` | Query | `attachmentType` | `int?` | 否 | 附件类型，特定场景（如 **3 - 收银台**）才填；**MD1b 锁定该参数有默认值、不得改必填** |

**返回** `UploadMediaResponse`（与公共面 E1 同一 DTO）：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `type` | `Type` | `string?` | 回显媒体类型 |
| `media_id` | `MediaId` | `string?` | 素材 id，**三天有效** |
| `created_at` | `CreatedAt` | `long?` | 上传时间戳（Unix 秒） |

> ⚠️ **本端点在当前依赖版本下不可用**（文件字节不上 wire），与公共面 E1/E3 同源，见 §5 与 §7。

**调用示例**（复用 [Media.md](./Media.md) E1 的宿主表单类型 `MediaUploadForm`——同一契约要求：字段名 `media`、携带 `filename` 与 `content-type`）

```csharp
using Mud.Wechat.Work;                        // 可注入子接口 + AddWechatWorkServices / AddMediaApi
using Mud.Wechat.Work.Abstractions;           // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.Media;        // UploadMediaResponse

// ① 注册第三方套件应用（凭据来自宿主配置/环境变量，不得写入仓库与日志）
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "suite-a";
    o.AppType = WechatAppType.ThirdParty;
    o.CorpId = config["Wechat:CorpId"];
    o.ProviderSecret = config["Wechat:ProviderSecret"];
    o.SuiteId = config["Wechat:SuiteId"];
    o.SuiteSecret = config["Wechat:SuiteSecret"];
});

// ② 注册素材管理模块（服务商通道与公共面同组）
builder.Services.AddWechatWorkServices(b => b.AddMediaApi());

// ③ 注入服务商通道子接口：令牌是服务商级，不需要 UseCorpScope
var serviceMedia = provider.GetRequiredService<IWechatWorkThirdPartyServiceMediaService>();

// ④ 上传（图片上限 2MB，注意与公共面 10MB 不同；filename 不含 \ 与 :，不以 / 开头）
await using var file = File.OpenRead(config["Demo:FilePath"]!);

var uploaded = await serviceMedia.UploadServiceMediaAsync(
    "image", new MediaUploadForm(file, "receipt.png", "image/png"), null, cancellationToken);
WechatWorkException.ThrowIfFailed(uploaded, "/cgi-bin/service/media/upload");
logger.LogInformation("服务商通道上传完成，type={Type}", uploaded.Type);

// ⑤ 收银台等特定场景才填 attachment_type（官方选填；同一 Stream 已被上一次请求消费，须重新打开）
await using var second = File.OpenRead(config["Demo:FilePath"]!);
var withAttachment = await serviceMedia.UploadServiceMediaAsync(
    "file", new MediaUploadForm(second, "receipt.pdf", "application/pdf"), 3, cancellationToken);
WechatWorkException.ThrowIfFailed(withAttachment, "/cgi-bin/service/media/upload");
```

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **两套令牌、两个族** | 本族 `ProviderAccessToken`（Query `provider_access_token`）；公共面 6 支 `AccessToken`（Query `access_token`，scope = 企业/应用）。路由也不同（`/cgi-bin/service/media/upload` vs `/cgi-bin/media/upload`）⇒ 因此**独立成族**而非公共面第 7 支 | 拿服务商令牌调公共面（或反之）；把端点「合并」进 `IWechatWorkMediaService` → 直接打红 MD2/MD2b |
| **无归属域键** | `IWechatWorkThirdPartyServiceMediaService` 的 `[Token]` **只有** `TokenType` / `InjectionMode` / `Name`；TO1 明确「非 `AccessToken` 族已无歧义，不得叠加 `@Internal`/`@Corp`」 | 给子接口补 `TokenManagerKey` → TO1 违规清单命中 |
| **不需要企业作用域** | 服务商令牌跨授权企业唯一 ⇒ 无需 `UseCorpScope`；而公共面第三方子接口**必须**先切到目标授权企业 | 为本族多余地进入企业作用域；或反过来在调公共面素材上传时漏切作用域 |
| **图片上限与公共面不同** | 本族 `image` ≤ **2MB**；公共面 `media/upload` `image` ≤ **10MB**（voice/video/file 两边分别一致：2MB/10MB/20MB） | 照抄公共面数值 → 官方拒 2~10MB 的图片 |
| `filename` 形状 | 本族注释独有：不能以 `/` 开始、不能含 `\` 与 `:` | 用绝对路径当文件名 → 参数非法 |
| 本族**只有上传** | 官方未开放服务商版的获取/上传图片/高清语音/异步上传 ⇒ 想取回素材只能走企业侧 `media/get` | 在服务商族里补下载端点（无官方页支撑） |
| **上传腿（现行缺陷）** | 本批用 `EmitCompilerGeneratedFiles` 复测生成体：`Mud.HttpUtils.Generator` **3.0.9** 对 `[MultipartForm] IFormContent` 发射的是<br>`var __formDataContent = new StringContent(formData.ToString() ?? "");` + `__multipartContent.Add(__formDataContent, "formData");`<br>⇒ **`ToHttpContent()` / `ToHttpContentAsync()` 从不被调用**，文件字节不上 wire、字段名也不是 `media` | 宿主无论怎么正确实现 `IFormContent` 都上传不了；官方侧表现为「缺少参数/文件」类错误，易误判成宿主实现写错 |
| **本地 fail-fast 例外** | 生成体对 `type` 空白与 `formData == null` **直接抛 `ArgumentNullException`**（`attachment_type` 可缺省不校验）——这是「SDK 不做本地必填校验」通则的例外 | 以为空值会交给官方判定，实际在客户端就抛 |
| 标注选择 | 用 `[MultipartForm]` 而非 `[FormContent]`：后者在生成器 3.0.1 下发射运行时接口上不存在的 `GetFormDataContentAsync`（`CS1061`）；但「`[MultipartForm]` 才走 `ToHttpContentAsync` 通路」这句注释与本批实测发射不符 | 换标注编译期即失败；或按注释信任通路而不复测 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不校验文件量纲与 `filename` 形状**、**不跟踪 `media_id` 三天时效**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | 成功才拿得到 `media_id` |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`provider_access_token` 失效类码（服务商令牌体系）与上传校验类码（≤5 字节、超类型上限、格式不符、`filename` 含非法字符、`attachment_type` 组合非法）的**具体值接口注释未逐条留档**；且在 §5「上传腿」缺陷下，官方返回的更可能是「缺少参数」类码 ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| **上传腿不可用** | 本批实测（3.0.9 生成体）。复现命令：<br>`dotnet build Src/Work/Mud.Wechat.Work/Mud.Wechat.Work.csproj -c Release -f net8.0 --no-incremental -p:EmitCompilerGeneratedFiles=true -p:CompilerGeneratedFilesOutputPath=<目录>`，再看 `Mud/Wechat/Work/Internal/WechatWorkThirdPartyServiceMediaService.g.cs` 的 `UploadServiceMediaAsync` | 上游修复后重跑 + 补 wire 级测试，并**同批**修正接口注释与本节 |
| `media_id` 的归属与读取面 | 注释照抄公共面口径「同一企业内应用之间可以共享」，但本端点用的是**服务商级令牌、无企业 scope** ⇒ 该 `media_id` 究竟归属哪家企业、能否直接用企业 `access_token` 调 `media/get` 取回，**注释未说明** ⇒ 未核验 | 官方 99310 与 90256 联读；结论确定后同批补 2.2 |
| `attachment_type` 取值域 | 注释只给一个例子（3 - 收银台），完整枚举与「哪些 `type` 可与哪些 `attachment_type` 组合」未留档 | 官方 99310 参数表 |
| 上传校验码全集 | 只给量纲，未给各量纲对应 `errcode` ⇒ 未逐项核验 | 官方 99310 + 全局错误码页 |
| 端点频率限制 | 注释**未留数值型阈值** ⇒ 未逐项核验 | 官方「访问频率限制」附录 |
| 真账号运行验证 | 本批仅编译级 + 生成体静态复测 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Media/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Media/README.md)（7 支端点速览、六接口令牌表与域级契约）
- 同域另一族：[Media.md](./Media.md)（三类应用公共面 6 支：上传临时素材 / 获取临时素材 / 上传图片 / 高清语音 / 异步上传两支，含 `MediaUploadForm` 宿主实现）
- 服务商令牌体系：[ProviderAuthentication.md](../Authentication/ProviderAuthentication.md)（`provider_access_token` 的获取与刷新）
- 同形态先例（零端点父 + 唯一子接口承载）：[KfComponent.md](../KF/KfComponent.md)（微信客服组件域）· [AcquisitionComponent.md](../ExternalContact/AcquisitionComponent.md)（获客助手组件代支付流水族）
- 官方页：[服务商上传临时素材（99310）](https://developer.work.weixin.qq.com/document/path/99310)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
