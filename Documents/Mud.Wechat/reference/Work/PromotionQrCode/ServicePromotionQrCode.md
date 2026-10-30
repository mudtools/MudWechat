# 推广二维码 · 企业注册（服务商 provider 令牌）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `PromotionQrCode` 域 · 企业注册族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [PromotionQrCode/IWechatWorkServicePromotionQrCodeService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/PromotionQrCode/IWechatWorkServicePromotionQrCodeService.cs)（父，**零端点**，`IsAbstract = true` 不可注入）· [PromotionQrCode/IWechatWorkThirdPartyServicePromotionQrCodeService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/PromotionQrCode/IWechatWorkThirdPartyServicePromotionQrCodeService.cs)（唯一子接口，承载 2 支端点） |
| DTO 源文件 | [DataModels/PromotionQrCode/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/PromotionQrCode/)（全域 8 型、`SerializerClassName = PromotionQrCode` → `PromotionQrCodeJsonContext`；本族 6 型：`GetRegisterCodeRequest` / `GetRegisterCodeResponse` / `GetRegisterInfoRequest` / `GetRegisterInfoResponse` / `ContactSyncCredential` / `RegisterAuthUserInfo`） |
| 契约守卫 | [WechatPromotionQrCodeContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatPromotionQrCodeContractGuards.cs)（PQ1：全域恰 4 路由且动词一并锁定 · **PQ2**：本族零端点父 + 唯一第三方子接口 · PQ4：令牌绑定 · PQ5：8 型全量登记 · PQ6：字段名与「未填即清空」陷阱）＋ G5 ＋ TO1 |
| 官方文档 | 获取注册码 [90581](https://developer.work.weixin.qq.com/document/path/90581) · 查询注册状态 [90582](https://developer.work.weixin.qq.com/document/path/90582)（**仅第三方应用开发文档树**） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为 PQ1/PQ2/PQ4/PQ5/PQ6 断言 + 2 份接口文件 XML 注释 + 6 型本族 DTO 逐字段比对；域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 21 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（生成注册码 → 24 小时内查状态，`contact_sync` 条件返回分支））；本批 21 段、全仓累计 171 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：PQ1/PQ2/PQ4/PQ5/PQ6 断言 + 本族 2 份接口文件 XML 注释 + 6 型 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/PromotionQrCode/README.md) · 官方：本批未重新渲染取文 |

## 1. 这个接口族做什么

**服务商用自己的推广包生成注册码，引导客户企业自助注册**——这是「企业注册」链路的入口段，2 支端点：

| # | 方法 | 路由 | 说明 |
|---|---|---|---|
| E1 | `GetRegisterCodeAsync` | `POST /cgi-bin/service/get_register_code` | 按推广包生成注册码（`register_code`），可预填企业名 / 管理员信息 |
| E2 | `GetRegisterInfoAsync` | `POST /cgi-bin/service/get_register_info` | 查询该注册码对应的注册状态；注册成功返回 `corpid` 等注册信息 |

**族形态是「零端点父接口 + 唯一第三方子接口」**：官方**仅在第三方应用开发文档树**开放本族端点，自建与代开发**无对应文档页** ⇒ 父接口 `IWechatWorkServicePromotionQrCodeService` 零端点，2 支全部声明在 `IWechatWorkThirdPartyServicePromotionQrCodeService` 上（PQ2 断言继承链上恰等于该唯一子接口，自建 / 代开发子接口**刻意不存在**）。

**本域是「两族两凭据」**：本族走服务商 `provider_access_token`（基座托管）；同域 [ContactSync.md](./ContactSync.md)（通讯录迁移收尾）走**注册链返回的通讯录迁移 `access_token`**（显式传参、基座**不**托管）。两者**不可互换**——官方在两处明文「请注意与 `provider_access_token` 的区别」。

**本域没有二维码图片**：`register_code` 是**注册链接里的码字符串**（≤512 字节），不是图片字节。「返回 `image/png` 二进制」的陷阱属于 `Authentication` 邻域的 `get_app_qrcode`，**与本域无关**（不要借 `result_type` 之类的形状）。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.ThirdParty`（仅第三方 / 服务商；自建与代开发官方无本页） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.ProviderAccessToken` → Query `provider_access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险：官方契约强制 Query，无法改 Header） | 每次请求 |
| 令牌归属键 | **不声明 `TokenManagerKey`**：provider 令牌族凭据来源唯一；PQ4 反向断言「不等于 `InternalAccessToken` / `CorpAccessToken`」，TO1 同口径 | — |

`provider_access_token` 是**服务商级凭据**（跨全部授权企业共享一份）⇒ **调用前不需要 `UseCorpScope`**。敏感凭据，日志 / 遥测 / 异常消息只写参数名。

### 2.2 权限与业务前置

- **推广包前置**：`template_id` 须先在「服务商管理端 → 应用管理 → 推广二维码」创建（本域**只有消费端**，没有建包端点）。官方必填，≤ 128 字节。
- **注册码一次性**：`register_code` **只能消费一次**，消费发生在**访问注册链接**时；`expires_in` 是链接有效期（官方**示例值 600 秒**）。
- **查询窗口 24 小时**：`register_code` 生成后 **24 小时内**可被 E2 查询——这与上面的 `expires_in` 是**两个不同的时间窗**。
- **E2 只收官方签发的码**：仅支持「注册完成回调事件」或 E1 返回的 `register_code`，自造 / 改写无效。
- **E2 仅全新创建企业**：错误码 **84024**「非全新创建企业，不支持使用该接口查询」。
- **`state` 上限 128 字节**（仅英文字母与数字）：本域的上限是 **128**，`Authentication` 邻域定制授权链接的 `state` 是 **32**——**跨域复用同一个渠道标识常量必然在一侧越界**；SDK 不做本地长度校验，越界由 `errcode` 表达。
- **`follow_user` 必须是服务商所在企业成员**（单个 userid 字符串，不是数组）；配置后该注册码创建的企业在服务商后台报备记录会自动标注跟进人。
- **预填项**：`corp_name` / `admin_name` / `admin_mobile` 传入后，注册填写页相应字段自动带入——注意这三项是**企业与管理员的真实信息**，示例一律假值。
- **`contact_sync` 权限极大**：E2 响应里的 `contact_sync.access_token` 具备**全部通讯录读写权限**、30 分钟有效、且被 [ContactSync.md](./ContactSync.md) 的「设置通讯录同步完成」立即作废 ⇒ **不落日志、不进遥测、不进异常消息、不写 URL**。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 `service/get_register_code` | ❌ | ✅ | ❌ |
| E2 `service/get_register_info` | ❌ | ✅ | ❌ |

> PQ2 的继承链断言要求本族只能有 `IWechatWorkThirdPartyServicePromotionQrCodeService` 一个子接口；官方若补自建 / 代开发页，层级需重开。

## 3. 接口族与注册

```csharp
// Program.cs：多应用底座先行，再注册推广二维码模块（两族共用一个模块与注册组）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddPromotionQrCodeApi());
```

| 项 | 值 |
|---|---|
| 模块枚举 | `WechatModule.PromotionQrCode` |
| 注册入口 | `WechatWorkServiceBuilder.AddPromotionQrCodeApi()`（→ 源生成 `AddPromotionQrCodeWebApiHttpClient()`） |
| 注册组名 | `"PromotionQrCode"`（两族同组；父接口保持空组，N3 锁定） |
| 可注入接口 | `IWechatWorkThirdPartyServicePromotionQrCodeService`（命名空间 `Mud.Wechat.Work`，`InheritedFrom = WechatWorkServicePromotionQrCodeService`） |
| 不可注入 | `IWechatWorkServicePromotionQrCodeService`（父，零端点，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

### E1. `GetRegisterCodeAsync` — 获取注册码

**签名**（声明于唯一第三方子接口）

```csharp
[Post("/cgi-bin/service/get_register_code")]
Task<GetRegisterCodeResponse> GetRegisterCodeAsync(
    [Body] GetRegisterCodeRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/service/get_register_code` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/service/get_register_code?provider_access_token=PROVIDER_ACCESS_TOKEN` |
| 凭据参数 | `provider_access_token`（Query 注入，SDK 自动完成） |
| 适用应用类型 | 仅第三方 |
| 请求体 | `GetRegisterCodeRequest` |

**请求体** `GetRegisterCodeRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `template_id` | `TemplateId` | `string` | 是 | 推广包 ID，≤ 128 字节（PQ6 断言为**非可空** `string`） |
| `corp_name` | `CorpName` | `string?` | 否 | 企业名称，注册页自动预填 |
| `admin_name` | `AdminName` | `string?` | 否 | 管理员姓名，注册页自动预填 |
| `admin_mobile` | `AdminMobile` | `string?` | 否 | 管理员手机号，注册页自动预填 |
| `state` | `State` | `string?` | 否 | 自定义状态值；**仅英文字母与数字、≤ 128 字节**；指定后 E2 与「注册完成回调事件」会回传 |
| `follow_user` | `FollowUser` | `string?` | 否 | 跟进人 userid；**必须是服务商所在企业成员**；单个字符串，不是数组 |

**返回** `GetRegisterCodeResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `register_code` | `RegisterCode` | `string` | 注册码，≤ 512 字节；**只能消费一次** |
| `expires_in` | `ExpiresIn` | `int` | 注册链接有效期（秒），官方**示例值 600** |

**调用示例**（生成注册码，只落有效期不落码本身）

```csharp
using Mud.Wechat.Work;                         // 可注入子接口 + AddWechatWorkServices / AddPromotionQrCodeApi
using Mud.Wechat.Work.Abstractions;            // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.PromotionQrCode; // 本域请求 / 响应 DTO

// ① 注册第三方应用（provider_access_token 由基座按路由键注入）
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "suite-a";
    o.AppType = WechatAppType.ThirdParty;
    o.CorpId = config["Wechat:CorpId"];
    o.ProviderSecret = config["Wechat:ProviderSecret"];
    o.SuiteId = config["Wechat:SuiteId"];
    o.SuiteSecret = config["Wechat:SuiteSecret"];
});

// ② 注册推广二维码模块（两族同组）
builder.Services.AddWechatWorkServices(b => b.AddPromotionQrCodeApi());

// ③ 注入企业注册族子接口（父接口零端点、不可注入）
var register = provider.GetRequiredService<IWechatWorkThirdPartyServicePromotionQrCodeService>();

var code = await register.GetRegisterCodeAsync(new GetRegisterCodeRequest
{
    TemplateId = "TPL_FAKE_0001",            // 推广包 ID（服务商管理端创建）
    CorpName = "示例新企业",                  // 预填项：假值
    AdminName = "李四",
    AdminMobile = "+8613800000000",
    State = "channel0001",                   // 仅字母数字，本域上限 128 字节（勿套邻域的 32）
    FollowUser = "sp_staff_fake0001",        // 必须是服务商所在企业成员（假 userid）
}, cancellationToken);
WechatWorkException.ThrowIfFailed(code, "/cgi-bin/service/get_register_code");

// ④ register_code 只能消费一次；只落有效期，不把码写进日志
logger.LogInformation("注册码已生成，链接有效期 {ExpiresIn} 秒", code.ExpiresIn);
```

### E2. `GetRegisterInfoAsync` — 查询注册状态

**签名**

```csharp
[Post("/cgi-bin/service/get_register_info")]
Task<GetRegisterInfoResponse> GetRegisterInfoAsync(
    [Body] GetRegisterInfoRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/service/get_register_info` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/service/get_register_info?provider_access_token=PROVIDER_ACCESS_TOKEN` |
| 凭据参数 | `provider_access_token`（Query 注入） |
| 请求体 | `GetRegisterInfoRequest`（`{"register_code": "..."}`） |
| 适用应用类型 | 仅第三方 |

**请求体** `GetRegisterInfoRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `register_code` | `RegisterCode` | `string` | 是 | 仅接受「注册完成回调事件」或 E1 返回的码；**生成后 24 小时内可查** |

**返回** `GetRegisterInfoResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `corpid` | `Corpid` | `string?` | 企业 corpid（注册成功时返回） |
| `contact_sync` | `ContactSync` | `ContactSyncCredential?` | 通讯录迁移凭证；**仅当推广包开启通讯录迁移时返回** |
| `auth_user_info` | `AuthUserInfo` | `RegisterAuthUserInfo?` | 授权管理员信息（本仓只承载 `userid`） |
| `state` | `State` | `string?` | 回传 E1 指定过的 `state`；未指定则**无该字段** |
| `template_id` | `TemplateId` | `string?` | 推广包 ID |

嵌套 `ContactSyncCredential`：`access_token`（`string`，通讯录迁移凭证，官方示例有效期 **1800 秒**）、`expires_in`（`int`）。嵌套 `RegisterAuthUserInfo`：`userid`（`string?`）。

**调用示例**（`contact_sync` 按可空处理；迁移凭证不落日志）

```csharp
var register = provider.GetRequiredService<IWechatWorkThirdPartyServicePromotionQrCodeService>();

var info = await register.GetRegisterInfoAsync(
    new GetRegisterInfoRequest { RegisterCode = code.RegisterCode }, cancellationToken);
WechatWorkException.ThrowIfFailed(info, "/cgi-bin/service/get_register_info");
// 84024 = 非全新创建企业，不支持用本接口查询

if (info.ContactSync is null)
{
    // 该推广包未开启通讯录迁移：没有迁移凭证，ContactSync 族（设置可见范围 / 同步完成）无从调用
    logger.LogInformation("注册企业 {Corpid}（无通讯录迁移）", info.Corpid);
}
else
{
    // 迁移凭证只能在内存中传给下一步；不落日志、不进异常消息、不拼进 URL
    string contactToken = info.ContactSync.AccessToken;
    logger.LogInformation("注册企业 {Corpid}，通讯录迁移凭证有效期 {ExpiresIn} 秒",
        info.Corpid, info.ContactSync.ExpiresIn);

    // 下一步见 ContactSync.md：SetAuthorizedAppScopeAsync / SetContactSyncSuccessAsync
    _ = contactToken;
}
```

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **`register_code` 只能消费一次** | 消费发生在访问注册链接时 | 把同一个码分发给多个企业 → 第二个必然失败 |
| **两个不同的时间窗** | 链接有效期 `expires_in`（示例 600 秒）≠ 查询窗口 24 小时 | 用 600 秒判断「还能不能查」→ 误判过期 |
| **本域没有二维码图片** | `register_code` 是字符串，不是图片字节 | 按「下载二维码」设计流程 → 拿到的码无法当图用 |
| **`state` 上限 128 字节** | 与 `Authentication` 邻域的 32 字节不同 | 跨域共用常量 → 一侧越界 |
| **`state` 仅字母数字** | 不是任意字符串 | 塞 URL / JSON 片段 → 官方拒绝 |
| **`follow_user` 是单个 userid** | 不是数组；且须为服务商企业成员 | 传列表 → 类型不符；传客户企业成员 → 无效 |
| **`contact_sync` 条件返回** | 仅推广包开启通讯录迁移时才有 | 直接 `.AccessToken` → NRE；当成「注册失败」→ 误报 |
| **84024** | 非全新创建企业不支持查询 | 对存量企业反复重试 → 永不成功 |
| **迁移凭证 ≠ `provider_access_token`** | 官方两处明文提示 | 把 provider 令牌传给 [ContactSync.md](./ContactSync.md) 的端点 → 调用失败 |
| **迁移凭证权限极大** | 全部通讯录读写权限、30 分钟有效、可被立即作废 | 落日志 / 存库明文 / 拼 URL → 凭据泄露 |
| **E2 只收官方码** | 自造 / 改写无效 | 自行拼接码做「重放」→ 查询无果 |
| **父接口零端点** | 2 支都在子接口 | 去父接口找 → 找不到；搬进父接口 → PQ2 打红 |
| **无自建 / 代开发子接口** | 官方仅第三方树有本页 | 找 `IWechatWorkInternalServicePromotionQrCodeService` → 类型不存在（不要「补齐」） |

SDK 侧通用约束：**不自动判错**、**不校验 `state` 长度与字符集**、**不缓存 `register_code`**、**不托管 `contact_sync.access_token`**、**不拼接注册链接**（链接模板本仓未留档，见 §7）。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| 84024 | 接口注释留档 | 非全新创建企业，不支持用本接口查询 | 不对存量企业调用 E2；转走其他注册 / 授权链路 |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座自动恢复，调用方不手写重试） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`register_code` 过期 / 已消费、`template_id` 无效、`follow_user` 非本企业成员等业务码**接口注释未逐条留档** ⇒ 见 §7。判错为**单层普通信封**（两个响应类型都继承 `WechatWorkResponse`）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 注册链接的拼接模板 | 本域只产出 `register_code` 字符串；「把码拼成可点击注册链接」的 URL 形状与参数名本仓未留档 ⇒ **未逐项核验** | 官方 90581 与前端跳转页 |
| `expires_in` 的实际上限 | 只有官方示例值 600 / 1800 | 不得把示例值当长期常量硬编码 |
| 业务错误码全集 | 除 84024 外未留档 | 官方 90581 / 90582 + 全局错误码页 |
| 频率限制 | 两处 remarks 为「官方页面未给出本端点的独立频率限制，走官方全局访问频率限制」 | 官方对应页与频率附录 |
| `template_id` 的签发面 | 本域无建包端点 | 服务商管理端 |
| `auth_user_info` 的字段全集 | 本仓只承载 `userid`，官方是否另有姓名 / 手机号**未逐项核验** | 官方 90582（补字段须同批改 PQ5 的 8 型计数） |
| 「注册完成回调事件」载荷 | 在 Callback 包，不属本域 | Callback 包与官方指令回调页 |
| 官方页正文 | 本批未重新渲染取文 | 官方 90581 / 90582 |
| 真账号运行验证 | 本批仅编译级验证；本族需服务商账号 + 真实推广包 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/PromotionQrCode/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/PromotionQrCode/README.md)（4 支全表、两族两凭据对照、DTO 8 型清单）
- 同域近邻：[ContactSync.md](./ContactSync.md)（通讯录迁移收尾，**显式传参的迁移 `access_token`**）
- 邻域对照：[../Authentication/ProviderAuthentication.md](../Authentication/ProviderAuthentication.md)（`state` 的 32 字节口径、`get_app_qrcode` 的二进制返回形态——**不要借到本域**）
- 官方页：[获取注册码（90581）](https://developer.work.weixin.qq.com/document/path/90581) · [查询注册状态（90582）](https://developer.work.weixin.qq.com/document/path/90582)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
