# 素材管理 · 图片上传 / 整文件上传 / 分块上传

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Wedoc` 域 · 素材管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkWedocMaterialService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/Material/IWechatWorkWedocMaterialService.cs)（父，承载全部 5 支）＋ 自建 [IWechatWorkWedocMaterialService_Internal.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/Material/IWechatWorkWedocMaterialService_Internal.cs) / 第三方 [_ThirdParty.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/Material/IWechatWorkWedocMaterialService_ThirdParty.cs) / 代开发 [_Provider.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/Material/IWechatWorkWedocMaterialService_Provider.cs)（三个零端点子接口） |
| DTO 源文件 | [DataModels/Wedoc/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedoc/) 与 [RequestModel/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedoc/RequestModel/)（本族 5 个请求 + 4 个专用响应；请求侧必填字段全部以非可空类型表达，`SerializerClassName = Wedoc` → `WedocJsonContext`） |
| 契约守卫 | [WechatWedocContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWedocContractGuards.cs)（WD14 本族 5 路由 + WD16 字段名陷阱 + WD17 请求侧必填非可空 / 九族 69 端点总表 / WD4 层级 / WD5 令牌 / WD7 DTO 292 型登记）＋ G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | 图片：[99933](https://developer.work.weixin.qq.com/document/path/99933) / [101875](https://developer.work.weixin.qq.com/document/path/101875) / [101879](https://developer.work.weixin.qq.com/document/path/101879)；整文件：[101870](https://developer.work.weixin.qq.com/document/path/101870) / [101876](https://developer.work.weixin.qq.com/document/path/101876) / [101880](https://developer.work.weixin.qq.com/document/path/101880)；分块：[101871](https://developer.work.weixin.qq.com/document/path/101871) / [101877](https://developer.work.weixin.qq.com/document/path/101877) / [101881](https://developer.work.weixin.qq.com/document/path/101881)（按「自建 / 第三方 / 代开发」顺序） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：WD14 路由表 + WD16/WD17 字段与必填断言 + 5 支端点 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/README.md) 第 12 节（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

往文档里放**图片与文件**素材，共三种上传形态：

| 形态 | 端点 | 适用 |
|---|---|---|
| 图片 | E1 `image_upload` | 图片素材，Base64 内联 |
| 整文件 | E2 `file_upload` | 单文件 ≤**10M**，Base64 内联一次传完 |
| 分块 | E3 `file_upload_init` → E4 `file_upload_part` → E5 `file_upload_finish` | 单文件 ≤**20G**，按固定 **2M** 分块 |

**两条上传路径互斥**：`file_upload` 与 `file_upload_init/_part/_finish` 是同域两形态，**不要混用**——10M 以内可以直接整传，超过 10M 必须走分块三步。

**秒传短路**：E3 先按 `block_sha` 判断文件是否已存在。`hit_exist = true` 时官方直接回 `file_url`，**流程即完成**，不要再调 E4 / E5；只有 `hit_exist = false` 才拿 `upload_key` 继续。

与其它族的边界：本族只**上传素材拿 URL**，不把素材挂进文档内容；要把图片块 / 文件块写进智能文档，拿到 URL 或 `file_id` 后走 [SmartDoc.md](./SmartDoc.md) E8（`image_props.url` / `file_props.file_id`）。文档本身先由 [Wedoc.md](./Wedoc.md) E1 建好。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方 / 代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp` | 运行期归属校验 |
| 企业作用域 | 第三方 / 代开发必须先 `UseCorpScope(...)` | 调用前 |

### 2.2 权限与业务前置

- 自建应用须用「可调用应用」列表内 secret 换取的 `access_token`；第三方 / 代开发须具有「**文档**」权限；**仅可操作该应用自己创建的文档**。
- `docid` 来自 [Wedoc.md](./Wedoc.md) E1，四类对象（文档 / 表格 / 智能表格 / 智能文档）都可作为素材宿主。
- **规模闸**：整文件 ≤**10M**；分块文件 ≤**20G**、块大小**固定 2M（2097152 字节）**、`index` **从 1 起**、并发建议 ≤**10**；`file_name` ≤**255** 字符（**英文算 1、汉字算 2**）。
- **Base64 只填本体**：官方明确要求不得带 `data:application/x-javascript;base64` 之类前缀。SDK **按字符串透传、不编解码、不做本地大小硬拦**，越界由应答 `errcode` 表达。
- **频率**：本族三张官方页（99933 / 101870 / 101871）**均无独立数值型频率限制**，走[访问频率限制 90312](https://developer.work.weixin.qq.com/document/path/90312) 的默认阈值（每企业单接口 ≤1 万次/分、≤15 万次/小时）。接口注释明确警示**不得凭第三方 SDK 注释或记忆补数**。
- **数据安全**：`base64_content` / `file_base64_content` 是文件正文（可能含个人或商业数据），`file_url` / `url` 是可直接取回内容的链接；**日志只记 `docid`、文件名长度、字节数与错误码**，不得打印 Base64 正文或 URL。

### 2.3 官方开放的应用类型

三类应用端点一致，官方为每个端点各开一页：

| 端点 | 自建 | 第三方 | 代开发 | 文档号（自建 / 第三方 / 代开发） |
|---|---|---|---|---|
| E1 上传图片 | ✅ | ✅ | ✅ | 99933 / 101875 / 101879 |
| E2 上传文件（整文件） | ✅ | ✅ | ✅ | 101870 / 101876 / 101880 |
| E3 文件分块上传初始化 | ✅ | ✅ | ✅ | 101871 / 101877 / 101881 |
| E4 上传文件分块 | ✅ | ✅ | ✅ | 101871 / 101877 / 101881 |
| E5 文件分块上传完成 | ✅ | ✅ | ✅ | 101871 / 101877 / 101881 |

E3–E5 共用同一张官方页（「分块上传文档文件」，一页三路由）。该页权限表首行**多出一句**「企业需要使用『微盘』secret 所获取的 accesstoken 来调用」，与同页另三行并存（官方原文如此，本域按 `access_token` 链路调用）。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkWedocMaterialService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **5** | 否 |
| `IWechatWorkInternalWedocMaterialService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyWedocMaterialService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderWedocMaterialService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `Wedoc`、装配入口 `AddWedocApi()`；子接口 `InheritedFrom = nameof(WechatWorkWedocMaterialService)`。

## 4. 端点

> 以下 5 支的凭据面完全一致（`Wechat.AccessToken` + Query `access_token` + `application/json` 源生成序列化），表中不再重复。请求侧官方必填字段在 DTO 上均为**非可空**类型（WD17），漏填会在编译期而非运行期暴露。

### E1. `UploadImageAsync` — 上传图片

```csharp
[Post("/cgi-bin/wedoc/image_upload")]
Task<UploadWedocImageResponse> UploadImageAsync(
    [Body] UploadWedocImageRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/image_upload` |
| 官方文档 | 99933 / 101875 / 101879 |

**请求**（`UploadWedocImageRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `docid` | `Docid` | `string` | 文档 id（必填） |
| `base64_content` | `Base64Content` | `string` | 图片 Base64（必填）；**字段名无 `file_` 前缀** |

**响应**（`UploadWedocImageResponse`）：`url`（`string?`）/ `width`（`int?`）/ `height`（`int?`）/ `size`（`long?`）。

**注意**：响应链接字段官方名是 **`url`**（不是 `image_url`）；`width` / `height` / `size` 官方参数表标 `string`、示例是裸数字，本仓**按示例**建模为数值（WD16 锁定）。官方本页未给图片大小上限与频率数字。

### E2. `UploadFileAsync` — 上传文件（整文件）

```csharp
[Post("/cgi-bin/wedoc/file_upload")]
Task<UploadWedocFileResponse> UploadFileAsync(
    [Body] UploadWedocFileRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/file_upload` |
| 官方文档 | 101870 / 101876 / 101880 |

**请求**（`UploadWedocFileRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `docid` | `Docid` | `string` | 文档 id（必填） |
| `file_name` | `FileName` | `string` | 文件名（必填），≤255 字符、英文 1 / 汉字 2 |
| `file_base64_content` | `FileBase64Content` | `string` | 文件 Base64（必填），**≤10M**；字段名带 `file_` 前缀 |

**响应**：`file_url`（`string?`）。

**注意**：>10M 的文件官方要求改用 E3–E5 分块上传，本端点不会替你分段。字段名与 E1 不同（`base64_content` vs `file_base64_content`）是官方原文，**不要统一**。

### E3. `InitFileUploadAsync` — 文件分块上传初始化

```csharp
[Post("/cgi-bin/wedoc/file_upload_init")]
Task<InitWedocFileUploadResponse> InitFileUploadAsync(
    [Body] InitWedocFileUploadRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/file_upload_init` |
| 官方文档 | 101871 / 101877 / 101881（与 E4 / E5 同页） |

**请求**（`InitWedocFileUploadRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `docid` | `Docid` | `string` | 文档 id（必填） |
| `file_name` | `FileName` | `string` | 文件名（必填） |
| `size` | `Size` | `ulong` | 文件字节数（必填），最大 **20G** |
| `block_sha` | `BlockSha` | `List<string>` | 分块**累积** SHA1（必填），顺序与分块一致 |

**响应**（`InitWedocFileUploadResponse`）：`hit_exist`（`bool?`）/ `upload_key`（`string?`）/ `file_url`（`string?`）。

**注意**：`block_sha` 不是「每块的独立 SHA1」，而是**累积值**——按 2M 分块后逐块 `sha1_update`，每块记录当前 state 的十六进制；最后一块（可小于 2M）在 update 后再 `sha1_final`，即整文件 sha1。**本仓不实现该算法**，由宿主算好填入。`hit_exist = true` ⇒ 流程结束，不要调 E4 / E5。

### E4. `UploadFilePartAsync` — 上传文件分块

```csharp
[Post("/cgi-bin/wedoc/file_upload_part")]
Task<WechatWorkResponse> UploadFilePartAsync(
    [Body] UploadWedocFilePartRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/file_upload_part` |
| 官方文档 | 101871 / 101877 / 101881（与 E3 / E5 同页） |

**请求**（`UploadWedocFilePartRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `upload_key` | `UploadKey` | `string` | E3 返回的凭证（必填） |
| `index` | `Index` | `int` | 分块号（必填），**从 1 起**（不是 0 基） |
| `file_base64_content` | `FileBase64Content` | `string` | 该块 Base64（必填），固定 2M 切块 |

**响应**：仅 `errcode` / `errmsg`。

**注意**：官方建议**并发上传，并发数 ≤10**；每块固定 2M，最后一块按实际大小。分块号错位（从 0 起或跳号）会导致 E5 组装失败。

### E5. `FinishFileUploadAsync` — 文件分块上传完成

```csharp
[Post("/cgi-bin/wedoc/file_upload_finish")]
Task<FinishWedocFileUploadResponse> FinishFileUploadAsync(
    [Body] FinishWedocFileUploadRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/file_upload_finish` |
| 官方文档 | 101871 / 101877 / 101881（与 E3 / E4 同页） |

**请求**：`upload_key`（`string`，必填）。**响应**：`file_url`（`string?`）。

**注意**：必须在**全部分块成功之后**调用；`hit_exist = true` 的分支不走本端点。

**调用示例**（图片 → 整文件 → 分块三跳）

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

// 第三方 / 代开发改用 IWechatWorkThirdPartyWedocMaterialService / IWechatWorkProviderWedocMaterialService
// 并在调用前 switcher.UseCorpScope(appKey, authCorpId, permanentCode)。
var material = provider.GetRequiredService<IWechatWorkInternalWedocMaterialService>();
string docId = "DOCID-FAKE-PLACEHOLDER";

// ① 图片：Base64 只填本体，不带 data: 前缀；URL 与正文均不入日志
var image = await material.UploadImageAsync(
    new UploadWedocImageRequest { Docid = docId, Base64Content = "QUJD" },   // 假数据："ABC"
    cancellationToken);
WechatWorkException.ThrowIfFailed(image, "/cgi-bin/wedoc/image_upload");
logger.LogInformation("图片上传完成：{Width}x{Height}，{Size} 字节",
    image.Width, image.Height, image.Size);

// ② 整文件（≤10M）：字段名是 file_base64_content
var file = await material.UploadFileAsync(
    new UploadWedocFileRequest
    {
        Docid = docId,
        FileName = "示例说明.txt",
        FileBase64Content = "QUJD",                                        // 假数据
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(file, "/cgi-bin/wedoc/file_upload");

// ③ 分块（>10M）：init → part(index 从 1) → finish；命中秒传则跳过 part/finish
var init = await material.InitFileUploadAsync(
    new InitWedocFileUploadRequest
    {
        Docid = docId,
        FileName = "large-example.bin",
        Size = 3_145_728,                                                  // 假数据：3MB = 2 块
        BlockSha = ["0123456789abcdef0123456789abcdef01234567", "89abcdef0123456789abcdef0123456789abcdef"],
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(init, "/cgi-bin/wedoc/file_upload_init");

if (init.HitExist == true)
{
    logger.LogInformation("命中秒传，流程结束");
}
else
{
    string uploadKey = init.UploadKey ?? throw new InvalidOperationException("未命中秒传但缺少 upload_key");

    for (int index = 1; index <= 2; index++)
    {
        var part = await material.UploadFilePartAsync(
            new UploadWedocFilePartRequest
            {
                UploadKey = uploadKey,
                Index = index,
                FileBase64Content = "QUJD",                                // 假数据，真实场景每块 2M
            },
            cancellationToken);
        WechatWorkException.ThrowIfFailed(part, "/cgi-bin/wedoc/file_upload_part");
    }

    var finish = await material.FinishFileUploadAsync(
        new FinishWedocFileUploadRequest { UploadKey = uploadKey },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(finish, "/cgi-bin/wedoc/file_upload_finish");
    logger.LogInformation("分块上传完成");
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 图片 vs 文件内容字段 | 图片 `base64_content`；文件 `file_base64_content` | 传错键 ⇒ 参数错误 |
| 响应链接字段 | 图片 `url`；文件 `file_url` | 找错属性 ⇒ 空值 |
| 两种文件上传 | `file_upload`（≤10M）与 `file_upload_init/_part/_finish`（≤20G） | 拿整传传大文件 ⇒ 超限 |
| 秒传短路 | `hit_exist = true` 已给 `file_url` | 继续调 part / finish ⇒ 无意义或报错 |
| `block_sha` 语义 | **累积** SHA1，不是每块独立 SHA1 | 算法错 ⇒ 秒传判定失效 |
| 分块大小 | 固定 2M（2097152 字节） | 自定块长 ⇒ 组装失败 |
| `index` 基 | **从 1 起** | 从 0 起 ⇒ 错位 |
| `file_name` 长度 | ≤255 字符，**汉字算 2** | 按字符数估 ⇒ 超限 |
| 分块并发 | 官方建议 ≤10 | 无限并发 ⇒ 服务端压力与失败率 |
| Base64 前缀 | 官方禁止 `data:...;base64,` 前缀 | 服务端解析失败 |
| 频率 | 三页均无独立数字，走 90312 默认阈值 | 自行编造「N 次/分」写进代码限流 |
| 素材 URL / 内容 | `file_url` / `url` / Base64 正文 | 落日志 ⇒ 泄露文件内容 |

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 请求成功 | 判 `IsSuccess` |
| 640017 / 640018 / 640008 | 域 README 留档 | 未开启 API / 未授编辑权限 / 无管理权限 | 权限前置 |
| 40014 / 42001 / 42007 | 令牌失效码 | 应用令牌类错误 | 统一失效码处理 |
| 42011 | `InvalidProviderAccessToken` | 服务商令牌不合法 / 过期 | 失效码集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，装配缺陷 |

超过 10M / 20G、块大小不符、`index` 错位、`upload_key` 失效的具体业务码官方页未逐项留档（§7）。其余见[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90312)（90312）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 图片大小上限 | 官方图片页未给上限与频率数字 | 官方 99933 |
| 秒传判定口径 | `block_sha` 相同即命中？跨文档是否共享已上传文件 ⇒ 未成文 | 官方 101871 |
| `upload_key` 有效期 | 未成文，长任务须考虑中途过期 | 官方 101871 |
| 分块失败重试语义 | 某块失败后能否重传同号块 ⇒ 未成文 | 官方 101871 |
| 上传后素材的生命周期 | 未挂到文档内容的素材是否会被清理 ⇒ 未成文 | 官方 101870 |
| 具体业务错误码 | 超限 / 格式错误的码未逐项留档 | 官方 101870 / 101871 |
| 真账号验证 | 本批仅内部证据核验 | — |

## 8. 延伸阅读

- 同域兄弟：[Wedoc.md](./Wedoc.md)（先有 `docid`）· [SmartDoc.md](./SmartDoc.md)（把素材挂成 `BLOCK_TYPE_IMAGE` / `BLOCK_TYPE_FILE` 块）· [Document.md](./Document.md)（文档对象里的图片 / 文件节点）
- 权限：[DocPermission.md](./DocPermission.md)（上传能力仍受文档成员权限约束）
- 域总览：[Wedoc README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/README.md)（第 12 节「素材上传」）
- 官方页：[上传文档图片 99933](https://developer.work.weixin.qq.com/document/path/99933) · [上传文档文件 101870](https://developer.work.weixin.qq.com/document/path/101870) · [分块上传文档文件 101871](https://developer.work.weixin.qq.com/document/path/101871) · [访问频率限制 90312](https://developer.work.weixin.qq.com/document/path/90312)
