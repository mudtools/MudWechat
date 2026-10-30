# 版本和容量管理 · 专业版信息 / 容量信息

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Wedrive` 域 · 版本和容量管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkWedriveCapacityService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/IWechatWorkWedriveCapacityService.cs)（父，2 支）＋ [_Internal](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/IWechatWorkWedriveCapacityService_Internal.cs) / [_Provider](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/IWechatWorkWedriveCapacityService_Provider.cs) / [_ThirdParty](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/IWechatWorkWedriveCapacityService_ThirdParty.cs)（三个零端点空标记） |
| DTO 源文件 | 仅 2 个响应 DTO：[GetWedriveProInfoResponse.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedrive/GetWedriveProInfoResponse.cs)、[GetWedriveCapacityInfoResponse.cs](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedrive/GetWedriveCapacityInfoResponse.cs)（**无请求 DTO**，`SerializerClassName = Wedrive`） |
| 契约守卫 | [WechatWedriveContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWedriveContractGuards.cs)（WR1 `WedriveCapacityRoutes` 2 支 + **参数表恰 1 个且必须是 `CancellationToken`** 的循环断言 / WR3 令牌 / WR4 DTO 登记 / WR5 容量字段名）＋ G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | [95856](https://developer.work.weixin.qq.com/document/path/95856)（自建，单页两路由）/ [95861](https://developer.work.weixin.qq.com/document/path/95861)（第三方）/ [96849](https://developer.work.weixin.qq.com/document/path/96849)（代开发） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：WR1~WR5 断言 + 2 支端点 XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/README.md) 第 6 节（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

企业微盘的**账务面**：是否开通专业版、专业版席位用了多少、还剩多久，以及全员 / 专业的容量总数。

| 端点 | 作用 | 请求体 |
|---|---|---|
| E1 `mng_pro_info` | 专业版开通状态 + 席位用量 + 到期时间 | **空对象 `{}`** |
| E2 `mng_capacity` | 全员容量总数 + 专业容量总数 | **空对象 `{}`** |

两支**没有请求参数**——官方请求包体是空对象 `{}`，本 SDK 因此**不声明请求 DTO、不加 `[Body]` 参数**（对齐 `get_openid_migration` 无请求体先例）。方法签名只有一个 `CancellationToken`，WR1 对此有逐端点断言。

这两支回答了「能不能用专业能力」：`auth = 4`（可预览）、`enable_watermark`、`show_visitor_name` 都仅专业版可设。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `Internal` / `ThirdParty` / `Provider`（三类等宽开放） | 注册期 `AddWechatApp` |
| 自建必填配置 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp` | 运行期归属校验 |
| 企业作用域 | 第三方 / 代开发须先 `UseCorpScope(appKey, authCorpId, permanentCode)` | 调用前 |

### 2.2 权限与业务前置

- **令牌来源有专门口径**：本族接口注释写明「企业需要使用『**微盘**』secret 所获取的 accesstoken 来调用」；自建应用还须配置到「可调用应用」列表，第三方 / 代开发须具有「微盘」权限。**用普通应用的 secret 换的令牌不保证可用。**
- **单位是字节**：`total_capacity_for_all` / `total_capacity_for_vip` 单位 **B**，SDK 不做换算、不格式化。
- **时间戳**：`pro_expire_time` 是**精确到秒**的时间戳，不是日期字符串。
- **频率**：本族接口注释**无任何数值型频率限制**，走 90312 默认阈值。
- **数据安全**：本族只返回聚合数字（是否专业版、席位数、容量），不含个人数据，可安全记日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 | 官方文档 |
|---|---|---|---|---|
| E1 获取盘专业版信息 | ✅ | ✅ | ✅ | 95856 / 95861 / 96849 |
| E2 获取盘容量信息 | ✅ | ✅ | ✅ | 95856 / 95861 / 96849 |

> 两支**共用同一个官方文档页**（每类应用一页承载两条路由），因此三列文档号完全相同。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkWedriveCapacityService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **2** | 否 |
| `IWechatWorkInternalWedriveCapacityService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0（继承 2） | 是 |
| `IWechatWorkThirdPartyWedriveCapacityService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0（继承 2） | 是 |
| `IWechatWorkProviderWedriveCapacityService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0（继承 2） | 是 |

注册组 `Wedrive`、装配入口 `AddWedriveApi()`；三个子接口均 `InheritedFrom = nameof(WechatWorkWedriveCapacityService)`。

## 4. 端点

> 两支的凭据面完全一致（`Wechat.AccessToken` + Query `access_token`），表中不再重复。

### E1. `GetProInfoAsync` — 获取盘专业版信息

```csharp
[Post("/cgi-bin/wedrive/mng_pro_info")]
Task<GetWedriveProInfoResponse> GetProInfoAsync(
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/mng_pro_info` |
| 官方文档 | 95856 / 95861 / 96849 |

**请求**：无（官方空对象 `{}`）。

**响应**（`GetWedriveProInfoResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `is_pro` | `IsPro` | `bool?` | 是否专业版 |
| `total_vip_acct_num` | `TotalVipAcctNum` | `ulong?` | 专业版账号总数 |
| `use_vip_acct_num` | `UseVipAcctNum` | `ulong?` | 已使用的专业版账号数 |
| `pro_expire_time` | `ProExpireTime` | `ulong?` | 专业版到期时间（**时间戳，精确到秒**） |

> `is_pro = false` 时，其余三个字段是否有值官方未明确 ⇒ 只判 `IsPro`，不要假设席位与到期时间一定存在。

### E2. `GetCapacityInfoAsync` — 获取盘容量信息

```csharp
[Post("/cgi-bin/wedrive/mng_capacity")]
Task<GetWedriveCapacityInfoResponse> GetCapacityInfoAsync(
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedrive/mng_capacity` |
| 官方文档 | 95856 / 95861 / 96849 |

**请求**：无（官方空对象 `{}`）。

**响应**（`GetWedriveCapacityInfoResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `total_capacity_for_all` | `TotalCapacityForAll` | `ulong?` | 全员容量总数（单位 **B**） |
| `total_capacity_for_vip` | `TotalCapacityForVip` | `ulong?` | 专业容量总数（单位 **B**） |

**调用示例**（查专业版状态与容量，判断能否开专业能力）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Wedrive;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "drive-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];   // 官方口径：须是「微盘」secret 换取的 access_token
});
builder.Services.AddWechatWorkServices(b => b.AddWedriveApi());

var capacity = provider.GetRequiredService<IWechatWorkInternalWedriveCapacityService>();

// ① 专业版信息：无请求参数，只有 CancellationToken
var pro = await capacity.GetProInfoAsync(cancellationToken);
WechatWorkException.ThrowIfFailed(pro, "/cgi-bin/wedrive/mng_pro_info");
logger.LogInformation("专业版 {IsPro}，席位 {Used}/{Total}，到期 {Expire}",
    pro.IsPro, pro.UseVipAcctNum, pro.TotalVipAcctNum, pro.ProExpireTime);

// ② 容量信息：单位 B，SDK 不换算
var cap = await capacity.GetCapacityInfoAsync(cancellationToken);
WechatWorkException.ThrowIfFailed(cap, "/cgi-bin/wedrive/mng_capacity");
logger.LogInformation("全员容量 {All} B，专业容量 {Vip} B",
    cap.TotalCapacityForAll, cap.TotalCapacityForVip);

// ③ 专业能力前置：非专业版时不要调用 auth = 4 / enable_watermark / show_visitor_name
if (pro.IsPro != true)
{
    logger.LogInformation("非专业版：跳过水印与可预览权限设置");
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 请求体 | 官方空对象 `{}`，SDK **没有**请求 DTO | 自己造一个空 DTO 传入 ⇒ 与契约不符 |
| 方法参数 | 恰 1 个 `CancellationToken` | 多传 `[Body]` ⇒ 守卫打红 |
| 容量单位 | **B** | 当 KB / GB 展示 ⇒ 差 3 个数量级 |
| `pro_expire_time` | **秒级时间戳** | 当日期字符串解析 ⇒ 失败 |
| `is_pro = false` | 其余字段是否有值未成文 | 强取席位 / 到期 ⇒ 可能为 `null` |
| 令牌来源 | 官方口径是「微盘」secret | 普通应用 secret ⇒ 可能无权限 |
| HTTP 动词 | 两条**全部 POST** | 改 GET（因为「只是查询」）⇒ 404/405 |
| 频率 | 官方无独立数字，走 90312 | 自编阈值 ⇒ 与官方不符 |

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 请求成功 | 判 `IsSuccess` |
| 40014 / 42001 / 42007 | 令牌失效码 | 应用令牌类错误 | 统一失效码处理 |
| 42011 | `InvalidProviderAccessToken` | 服务商令牌不合法 / 过期 | 第三方 / 代开发链路 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，装配缺陷 |

令牌非「微盘」secret 换取、未开通企业微盘的具体业务码以官方页为准（仓内未逐项核验）。其余见[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90312）（90312）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 容量告警阈值 | 只有总数，无「已用 / 剩余」与告警线 | 官方 95856 |
| `is_pro = false` 时其余字段形态 | 未成文 | 官方 95856 |
| 容量不足的推送事件 | `wedrive_insufficient_capacity` 落 Callback 域，本域无端点 | [Callback 载荷](../../../../../Src/Work/Mud.Wechat.Work.Callback/Events/Payloads/WeDrive/) |
| 本族频率 / 并发上限 | 无任何数值 | 官方 95856 + 90312 |
| 真账号验证 | 本批仅内部证据核验 | — |

## 8. 延伸阅读

- 同域兄弟：[Space.md](./Space.md) · [SpacePermission.md](./SpacePermission.md)（`auth = 4` / `enable_watermark` 的专业版前置）· [FilePermission.md](./FilePermission.md)（`show_visitor_name` 仅专业版）· [Vip.md](./Vip.md)（高级功能席位分配）
- 域总览：[Wedrive README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedrive/README.md)（第 6 节）
- 官方页：[获取盘专业版信息 / 获取盘容量信息 95856](https://developer.work.weixin.qq.com/document/path/95856)（自建，单页两路由）
