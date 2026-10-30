# 商户号管理 · 详情查询与使用范围（仅自建应用）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Pay` 域 · 收款商户号管理族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [Merchant/IWechatWorkPayMerchantService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/Merchant/IWechatWorkPayMerchantService.cs)（父，**零端点**，`IsAbstract = true` 不可注入）· [Merchant/IWechatWorkInternalPayMerchantService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/Merchant/IWechatWorkInternalPayMerchantService.cs)（唯一子接口，承载 2 支端点） |
| DTO 源文件 | [DataModels/Pay/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Pay/)（全域 60 型、`SerializerClassName = Pay` → `PayJsonContext`；本族 4 型：`GetPayMerchantRequest` / `GetPayMerchantResponse` / `PayMerchantUseScope` / `SetPayMerchantUseScopeRequest`） |
| 契约守卫 | [WechatPayContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatPayContractGuards.cs)（PAY1b：本族 2 路由，含「`getmerchant` 单词连写」反直觉点 · PAY2b：父零端点 + 仅自建单子接口 · PAY3：16 接口 `AccessToken` + Query + `access_token` · PAY4：全域 60 型，本族 4 型）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 查询商户号详情 [93666](https://developer.work.weixin.qq.com/document/path/93666) · 设置商户号使用范围 [93666](https://developer.work.weixin.qq.com/document/path/93666)（同一页） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为 PAY1b/PAY2b/PAY3/PAY4 + 接口 XML 注释 + 4 型 DTO 逐项比对；域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 21 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（查商户号详情 → 整组提交使用范围，成员/部门清单只落状态码））；本批 21 段、全仓累计 171 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：PAY1b/PAY2b/PAY3/PAY4 断言 + 本族 2 份接口文件 XML 注释 + 4 型 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/README.md)（端点速览第 7/8 行、域级契约）· 官方：本批未重新渲染取文 |

## 1. 这个接口族做什么

**查一个已绑定商户号的底细，并限定「谁能用它收款」**——本族 2 支端点：

| # | 方法 | 路由 | 说明 |
|---|---|---|---|
| E1 | `GetMerchantAsync` | `POST /cgi-bin/externalpay/getmerchant` | 查商户号详情（名称、绑定状态、使用范围） |
| E2 | `SetMerchantUseScopeAsync` | `POST /cgi-bin/externalpay/set_mch_use_scope` | 设置使用范围（成员 / 部门 / 标签） |

**族形态是「零端点父接口 + 仅自建单子接口」**：官方**仅向企业自建应用**开放，代开发与第三方**均暂不支持** ⇒ 父接口 `IWechatWorkPayMerchantService` 不含端点、也不设第三方 / 代开发子接口（PAY2b 锁定）。

**两处路由反直觉**（PAY1b 逐字锁定）：
- E1 路由是 `getmerchant`——**单词连写**，不是 `get_merchant`；
- E2 路由在 `externalpay/` 下（不是 `miniapppay/`），且是 `set_mch_use_scope`（`mch` 简写而非 `merchant`）。

同域近邻：[MchApply.md](./MchApply.md)（商户号从哪来——进件申请）· [Bill.md](./Bill.md) · [FundFlow.md](./FundFlow.md) · [Order.md](./Order.md) · [Refund.md](./Refund.md) · [TradeBill.md](./TradeBill.md)。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal`（仅自建） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险：官方契约强制 Query，无法改 Header） | 每次请求 |
| 令牌归属键 | `Wechat.AccessToken@Internal`（`WechatTokenManagerKeys.InternalAccessToken`） | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

### 2.2 权限与业务前置

- **应用须配置到「对外收款 - 可调用接口的应用」**。
- **只有「已绑定」的商户号才能设范围**：E2 对绑定中（`bind_status = 1`）或已撤销（`3`）的商户号不生效；`allow_use_scope` 也**只对已绑定商户号返回**。
- **`mch_id` 上限 32 字节**（微信支付商户号）。
- **范围是三选多**：`user`（成员 userid）/ `partyid`（部门 id）/ `tagid`（标签 id）三类可同时设置；不设某类即该类不受限（以官方口径为准，见 §7）。
- **个人 / 组织数据**：`user` / `partyid` / `tagid` 指向具体成员、部门、标签，示例一律假值。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 / E2 | ✅ | ❌ | ❌ |

## 3. 接口族与注册

```csharp
// Program.cs：多应用底座先行，再注册支付模块（七族共用同一模块与注册组）
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddPayApi());
```

| 项 | 值 |
|---|---|
| 模块枚举 | `WechatModule.Pay` |
| 注册入口 | `WechatWorkServiceBuilder.AddPayApi()`（→ 源生成 `AddPayWebApiHttpClient()`） |
| 注册组名 | `"Pay"`（七族共用一个注册组；父接口保持空组，N3 锁定） |
| 可注入接口 | `IWechatWorkInternalPayMerchantService`（命名空间 `Mud.Wechat.Work`） |
| 不可注入 | `IWechatWorkPayMerchantService`（父，零端点，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

> **本族必填性不用类型表达**：`mch_id` 等字段官方必填但 DTO 可空（`string?`），SDK 不做本地校验。

### E1. `GetMerchantAsync` — 查询商户号详情

**签名**（声明于唯一自建子接口）

```csharp
// 路由为官方原文「getmerchant」单词连写，勿改 get_merchant（PAY1b 锁定）
[Post("/cgi-bin/externalpay/getmerchant")]
Task<GetPayMerchantResponse> GetMerchantAsync(
    [Body] GetPayMerchantRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/externalpay/getmerchant` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/externalpay/getmerchant?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，应用自身令牌） |
| 适用应用类型 | 仅自建 |
| 请求体 | `GetPayMerchantRequest` |

**请求体** `GetPayMerchantRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `mch_id` | `MchId` | `string?` | 是 | 微信支付商户号（≤32 字节） |

**返回** `GetPayMerchantResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `mch_id` | `MchId` | `string?` | 微信支付商户号（≤32 字节） |
| `merchant_name` | `MerchantName` | `string?` | 商户号全称（≤50 字节） |
| `bind_status` | `BindStatus` | `int?` | `1` 申请中 / `2` 已绑定 / `3` 已撤销 |
| `allow_use_scope` | `AllowUseScope` | `PayMerchantUseScope?` | 使用范围；**仅已绑定（`2`）时返回** |

`PayMerchantUseScope`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `user` | `User` | `List<string>?` | 使用范围内的成员 userid |
| `partyid` | `PartyId` | `List<int>?` | 使用范围内的部门 id |
| `tagid` | `TagId` | `List<int>?` | 使用范围内的标签 id |

> 注意字段名是 `partyid` / `tagid`——**全小写无下划线**，不是 `party_id` / `tag_id`。

### E2. `SetMerchantUseScopeAsync` — 设置商户号使用范围

**签名**

```csharp
[Post("/cgi-bin/externalpay/set_mch_use_scope")]
Task<WechatWorkResponse> SetMerchantUseScopeAsync(
    [Body] SetPayMerchantUseScopeRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/externalpay/set_mch_use_scope` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/externalpay/set_mch_use_scope?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入） |
| 适用应用类型 | 仅自建 |
| 请求体 | `SetPayMerchantUseScopeRequest` |
| 返回 | `WechatWorkResponse`（仅 `errcode` / `errmsg`） |

**请求体** `SetPayMerchantUseScopeRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `mch_id` | `MchId` | `string?` | 是 | 微信支付商户号（≤32 字节）；**须已绑定** |
| `allow_use_scope` | `AllowUseScope` | `PayMerchantUseScope?` | 是 | 使用范围（结构同 E1 返回） |

**调用示例**（先查态、再按结果设范围）

```csharp
using Mud.Wechat.Work;                         // 可注入子接口 + AddPayApi
using Mud.Wechat.Work.Abstractions;            // AddWechatApp
using Mud.Wechat.Work.Abstractions.Enums;      // WechatAppType
using Mud.Wechat.Work.Abstractions.Exceptions; // WechatWorkException
using Mud.Wechat.Work.DataModels.Pay;          // 请求 / 响应 DTO

// ① 注册自建应用（本域只对自建开放）
builder.Services.AddWechatApp(o =>
{
    o.AppKey = "corp-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});

// ② 注册支付模块（七族同组）
builder.Services.AddWechatWorkServices(b => b.AddPayApi());

// ③ 注入自建子接口（父接口零端点、不可注入）
var merchants = provider.GetRequiredService<IWechatWorkInternalPayMerchantService>();

// ④ 先查详情：bind_status 决定后续动作
string mchId = config["Pay:MchId"] ?? string.Empty;
var detail = await merchants.GetMerchantAsync(new GetPayMerchantRequest { MchId = mchId }, cancellationToken);
WechatWorkException.ThrowIfFailed(detail, "/cgi-bin/externalpay/getmerchant");

if (detail.BindStatus != 2)
{
    // 1 = 申请中（等进件流程） 3 = 已撤销（需重新申请）：都不设范围
    logger.LogInformation("商户号状态 {Status}，跳过范围设置", detail.BindStatus);
    return;
}

// ⑤ 仅在已绑定时设置范围（成员 / 部门 / 标签三类可同时给）
var scoped = await merchants.SetMerchantUseScopeAsync(new SetPayMerchantUseScopeRequest
{
    MchId = mchId,
    AllowUseScope = new PayMerchantUseScope
    {
        User = new List<string> { "zhangsan" },
        PartyId = new List<int> { 1 },
        TagId = new List<int> { 2 },
    },
}, cancellationToken);
WechatWorkException.ThrowIfFailed(scoped, "/cgi-bin/externalpay/set_mch_use_scope");

// ⑥ 只落状态，不落成员/部门清单
logger.LogInformation("商户号使用范围已更新（errcode {Errcode}）", scoped.ErrorCode);
```

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **`getmerchant` 单词连写** | 官方路由原文无下划线 | 「顺手规范化」成 `get_merchant` → PAY1b 打红 + 线上 404 |
| **E2 在 `externalpay/` 下** | `externalpay/set_mch_use_scope`（不是 `miniapppay/`） | 套用同域下单/退款族的 `miniapppay/` 前缀 → 404 |
| `set_mch_use_scope` 用 `mch` 简写 | 不是 `set_merchant_use_scope` | 按 E1 的 `merchant` 拼 → 404 |
| **父接口零端点** | 2 支全在 `IWechatWorkInternalPayMerchantService` | 去父接口找端点 → 找不到；搬进父接口 → PAY2b 打红 |
| **仅自建** | 第三方 / 代开发无子接口 | 找 `ThirdParty` / `Provider` 子接口 → 类型不存在 |
| **范围只对已绑定生效** | `bind_status = 2` 才有意义 | 对申请中/已撤销商户号设范围 → 空转成功 |
| **`allow_use_scope` 仅已绑定时返回** | 其他状态字段为 `null` | 把 `null` 当「范围为空」→ 误判权限缺失 |
| `partyid` / `tagid` 无下划线 | 官方字段名原文全小写连写 | 写成 `party_id` → 服务端不识别（DTO 已按官方原文命名） |
| `user` 是**成员** | 企业成员 userid，不是外部联系人 | 塞 `external_userid` → 设置无效 |
| 商户号从哪来 | 由 [MchApply.md](./MchApply.md) 进件生成，不是微信支付商户平台自建号 | 拿自有商户号调 → 官方拒 |
| E2 返回是统一信封 | 只有 `errcode` / `errmsg`，无业务体 | 试图读「设置后的范围」→ 得再调 E1 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不校验 `bind_status`**、**不校验 `mch_id` 长度**。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座自动恢复，调用方不手写重试） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

商户号不存在、未绑定、成员不在可见范围等业务码**接口注释未逐条留档** ⇒ 见 §7。判错为**单层普通信封**。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 本族注释未摘录业务码 ⇒ **未逐项核验** | 官方 93666 + 全局错误码页 |
| 「三类范围都不填」的语义 | 注释只写「支持按成员、部门或标签设置」，未写清「全不填 = 全员可用」还是「= 谁都不能用」 | 官方 93666 |
| 使用范围上限 | 各类列表的条数上限未核验 | 官方 93666 |
| 频率限制 | 本族注释未给独立频率 | 官方 93666 与频率附录 |
| 官方页正文 | 本批未重新渲染取文 | 官方 93666 |
| 真账号运行验证 | 本批仅编译级验证 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Pay/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/README.md)（15 支全表、令牌矩阵、DTO 清单）
- 同域近邻：[MchApply.md](./MchApply.md)（商户号的来源）· [Bill.md](./Bill.md) · [FundFlow.md](./FundFlow.md) · [Order.md](./Order.md) · [Refund.md](./Refund.md) · [TradeBill.md](./TradeBill.md)
- 官方页：[查询商户号详情 / 设置使用范围（93666）](https://developer.work.weixin.qq.com/document/path/93666)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
