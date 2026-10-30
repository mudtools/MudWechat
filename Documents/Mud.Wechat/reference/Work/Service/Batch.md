# 服务商异步任务 · 批量任务结果查询

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Service` 域 · 异步批量任务族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkServiceBatchService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Service/Batch/IWechatWorkServiceBatchService.cs)（父，承载唯一 1 支）＋ [IWechatWorkProviderServiceBatchService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Service/Batch/IWechatWorkServiceBatchService_Provider.cs)（服务商代开发子接口，零端点） |
| DTO 源文件 | [DataModels/Service/Batch/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Service/Batch/)（本族 1 个文件 / 3 型，`SerializerClassName = Service` → `ServiceJsonContext`） |
| 契约守卫 | [WechatServiceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatServiceContractGuards.cs)（SV1 5 路由 / SV2 父承载 + 唯一服务商子接口 / SV3 纯 `provider_access_token` 面 / SV4 17 型登记）＋ `WechatContractGuards` G5 ＋ N1~N3 |
| 官方文档 | [91882](https://developer.work.weixin.qq.com/document/path/91882) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：SV1 / SV2 / SV3 / SV4 断言 + 端点 XML 注释 + `WechatModule.Service` 注册说明 · 官方：本批未重新取文（本域无独立 README） |

## 1. 这个接口族做什么

服务商侧的异步任务（当前主要是通讯录 id 转译）提交后只返回 `jobid`，这一族就是**按 `jobid` 轮询任务状态与产物**的那一支：

| 端点 | 用途 | 同步性 |
|---|---|---|
| E1 `service/batch/getresult` | 查询服务商异步任务的处理状态与结果 | 同步查询（官方 GET） |

**与自建应用侧同名能力的区别**：自建应用侧是 `/cgi-bin/batch/getresult`（`access_token` 面，见 `IWechatWorkBatchService.GetBatchJobResultAsync`），响应含 `total` / `percentage` / `remaintime`；**本族路由带 `service/` 段、以 `provider_access_token` 鉴权，响应只有 `status` / `type` / `result` / `url`**，两套 DTO 不共用（SV4 锁定）。

**谁产生 `jobid`**：[Contact.md](./Contact.md) 的 E3「通讯录 id 转译」；未来官方若新增服务商异步任务，也由本端点统一轮询。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.ThirdParty` 或 `Provider`（服务商侧） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret`（四项齐备） | `WechatAppConfig.Validate()`，缺一启动即抛 |
| 令牌类型 | `WechatTokenTypes.ProviderAccessToken` → Query `provider_access_token` | — |
| 令牌归属键 | **无**（服务商级令牌，一服务商一份） | — |

### 2.2 权限与业务前置

- 本端点只对**服务商代开发 / 第三方应用开发**的服务端开放；企业自建应用无对应面。
- **不需要切企业作用域**：任务本身已绑定提交时的 `auth_corpid`，查询只认 `jobid`。
- `jobid` 由提交端返回，最大 64 字节；**必须持久化**——任务可能跨越进程重启。
- 轮询由调用方自行控制间隔；官方页面未给出独立频率限制，走全局访问频率限制。
- 响应里的 `url` 是带身份语义的临时资源地址：**不得进日志、遥测或对外持久化**。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 | 官方文档页 |
|---|---|---|---|---|
| E1 任务结果查询 | ❌ | ✅ | ✅ | 91882 |

> 官方正文请求地址为 `?provider_access_token=ACCESS_TOKEN&jobid=JOBID`，故继承链上只允许 `IWechatWorkProviderServiceBatchService` 一个子接口（SV2）。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkServiceBatchService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | 1 | 否 |
| `IWechatWorkProviderServiceBatchService` | `Mud.Wechat.Work` | 服务商代开发子接口（空标记） | 0 | 是 |

- 模块注册：`WechatModule.Service` / `AddServiceApi()`；注册组名 `Service`。
- 端点索引：E1 `GetServiceBatchJobResultAsync`。
- 端点声明在父接口，子接口 `GetMethods(DeclaredOnly)` 恒为空。

---

## 4. 端点

### E1. `GetServiceBatchJobResultAsync` — 获取异步批量任务结果

```csharp
[Get("/cgi-bin/service/batch/getresult")]
Task<GetServiceBatchJobResultResponse> GetServiceBatchJobResultAsync(
    [Query("jobid")] string jobId,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/service/batch/getresult`（**官方即 GET**，与本族检索四支的 POST 相反） |
| 凭据 | Query `provider_access_token` + Query `jobid` |
| 适用应用类型 | 第三方 / 代开发 |

**请求参数**

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `jobId` | `jobid` | `string` | 是 | 异步任务 id，走 **Query**，最大 64 字节（非可空） |

**响应参数**（`GetServiceBatchJobResultResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `status` | `Status` | `int?` | 1 任务开始 / 2 任务进行中 / 3 任务已完成 |
| `type` | `Type` | `string?` | 任务类型；目前有 `contact_id_translate` |
| `result` | `Result` | `ServiceBatchJobResultDetail?` | 详细处理结果，**仅任务完成后有效** |
| `url` | `Url` | `string?` | 产物地址，**仅任务完成后有效**；24 小时有效、须用户侧授权打开、不得落日志 |

`ServiceBatchJobResultDetail` 元素：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `contact_id_translate` | `ContactIdTranslate` | `ServiceContactIdTranslateResult?` | 通讯录 id 转译结果 |

`ServiceContactIdTranslateResult` 元素：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `url` | `Url` | `string?` | 转译产物下载地址（24 小时有效、须用户侧授权打开） |

**本端点注意事项**

- **终止条件是 `status = 3`**，不是「字段出现」或「`errcode=0`」：`result` / `url` 在 1 / 2 阶段本就可能缺失，不得以字段缺失判失败。
- **官方 GET**：`jobid` 走 Query，不要按「查询结果都 POST」的习惯发请求体。
- `url` 与 `result.contact_id_translate.url` 是**带身份语义的临时链接**：只允许转交给用户侧打开，禁止写日志 / 遥测 / 数据库长期留存。
- 与自建应用侧 `/cgi-bin/batch/getresult` 不是同一端点：路由、令牌、响应字段都不同，DTO 也不复用。

**调用示例**（提交转译 → 轮询到 `status=3`）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Service;

// builder / provider / config / logger / cancellationToken 由宿主提供
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "provider-a";
    o.AppType = WechatAppType.ThirdParty;     // 代开发改填 WechatAppType.Provider
    o.CorpId = config["Wechat:ProviderCorpId"];
    o.ProviderSecret = config["Wechat:ProviderSecret"];
    o.SuiteId = config["Wechat:SuiteId"];
    o.SuiteSecret = config["Wechat:SuiteSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddServiceApi());

var serviceContacts = provider.GetRequiredService<IWechatWorkProviderServiceContactService>();
var serviceBatch = provider.GetRequiredService<IWechatWorkProviderServiceBatchService>();

// ① 提交转译任务（详见 Contact.md E3），拿到 jobid
var submitted = await serviceContacts.TranslateContactIdsAsync(new TranslateServiceContactIdsRequest
{
    AuthCorpId = "wwAUTHCORPIDFAKE",               // 假值
    MediaIdList = ["MEDIA_ID_PLACEHOLDER"],        // 只写占位符
    OutputFileFormat = "pdf",
}, cancellationToken);
WechatWorkException.ThrowIfFailed(submitted, "/cgi-bin/service/contact/id_translate");
string jobId = submitted.JobId ?? throw new InvalidOperationException("转译任务未返回 jobid");

// ② 轮询：唯一终止条件是 status == 3；间隔由宿主控制，官方未给独立频率限制
GetServiceBatchJobResultResponse? job = null;
for (int attempt = 0; attempt < 30; attempt++)
{
    job = await serviceBatch.GetServiceBatchJobResultAsync(jobId, cancellationToken);
    WechatWorkException.ThrowIfFailed(job, "/cgi-bin/service/batch/getresult");
    if (job.Status == 3)
    {
        break;
    }
    await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
}
logger.LogInformation("任务状态 {Status}，类型 {Type}", job?.Status, job?.Type);   // 只记状态与类型

// ③ 任务完成后才取产物地址；url 不得进日志/遥测，只能在用户侧授权打开且 24 小时内有效
if (job?.Status == 3)
{
    string? downloadUrl = job.Url ?? job.Result?.ContactIdTranslate?.Url;
    if (downloadUrl is not null)
    {
        logger.LogInformation("产物地址已就绪（长度 {Length}），转交用户侧打开", downloadUrl.Length);
    }
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| HTTP 方法 | 官方 **GET**，`jobid` 走 Query | 发 POST 请求体 → 405 |
| 终止条件 | 只有 `status = 3` 才算完成 | 以 `errcode=0` 判断 → 任务未完成就取结果 |
| 字段有效性 | `result` / `url` 仅完成后有效 | 任务进行中读到空 → 误判失败 |
| 同能力异路由 | 自建侧是 `/cgi-bin/batch/getresult`（`access_token`） | 混用路由 / 令牌面 → 鉴权失败 |
| 响应形状 | 本族无 `total` / `percentage` / `remaintime` | 复用自建侧 DTO → 字段恒 `null` |
| `url` 安全 | 带身份、24 小时有效、须用户侧打开 | 落日志 / 长期入库 → 泄露与失效链接 |
| `jobid` 长度 | 最大 64 字节 | 自行拼接或截断 → 查不到任务 |
| 轮询频率 | 官方未给独立限制，走全局限频 | 高频死循环 → 限流 |

SDK 侧通用约束：不自动判错、不做本地必填校验、不自动轮询；令牌 Query 注入是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 请求成功 | **不代表任务完成**，还要看 `status` |
| 42011 | `InvalidProviderAccessToken` | 服务商令牌不合法 / 过期 | 失效码集合，基座按既有机制恢复 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 应用令牌类错误 | 按统一失效码处理 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

本族注释未留档**专属**业务错误码：`jobid` 不存在 / 已过期 / 不属于本服务商等失败由官方业务码表达，码值以官方页为准（仓内未逐项核验）。其余错误码见官方[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）；仓内 `appendix/错误码.md` 尚未成文。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 任务失败的状态表达 | 任务执行失败时是 `status` 某值，还是外层 `errcode` 非 0 ⇒ 未核验 | 官方 91882 |
| `jobid` 有效期 | 完成 / 失败后结果可保留多久 ⇒ 未核验 | 官方 91882 |
| 推荐轮询间隔 | 官方只给全局限频，未给本端点的建议间隔 ⇒ 未核验 | 90454 |
| `result` 的其他任务类型 | 除 `contact_id_translate` 外是否已有/计划其他键 ⇒ 未核验 | 官方 91882 |
| `url` 的鉴权方式 | 「用户侧 oauth2 / 单点登录打开」的具体参数形态 ⇒ 未逐项核验 | 官方页 |
| 并发任务上限 | 同一服务商可同时挂起多少异步任务 ⇒ 未核验 | 官方页 |

## 8. 延伸阅读

- 兄弟族：[Contact.md](./Contact.md)（检索四支 + id 转译提交端）
- 相关域：[Contacts/Batch.md](../Contacts/Batch.md)（自建应用侧批量任务，`access_token` 面，勿与本族混用）
- 数据模型：[ServiceBatchJobDtos.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Service/Batch/ServiceBatchJobDtos.cs)
- 令牌面：[Authentication/ProviderAuthentication.md](../Authentication/ProviderAuthentication.md)
- 凭据与多应用：仓库 [AGENTS.md §5.1](../../../../../AGENTS.md)
- 官方页：[获取异步任务结果 91882](https://developer.work.weixin.qq.com/document/path/91882)
