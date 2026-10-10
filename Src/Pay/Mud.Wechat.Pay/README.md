# Mud.Wechat.Pay

微信支付 **APIv3** SDK **主包**：10 个业务域 54 端点的声明式客户端、模块注册器（`PayModule`）、四类非声明式服务（小程序调起签名 / 账单下载 / 发票文件上传 / 平台证书刷新）、AOT JsonContext 合并。

> **与企业微信线是两条不同产品线**：本包凭据是**商户 API 证书 RSA 私钥的请求签名**（`Authorization: WECHATPAY2-SHA256-RSA2048 …`），**没有 `access_token`**、四个 `Mud.Wechat.Pay*` 包**零 `[Token]` 声明**（守卫 `PAY-B1` fail-closed），BaseAddress 恒为 `https://api.mch.weixin.qq.com`。
> 企业微信的「企业支付」`AddPayApi()` 与「收银台」`AddPayToolApi()`（`Src/Work/`，走企微 `access_token`）与本包**无关**；本包入口名为 `AddPayApp()`（凭据底座）与 `AddWechatPayApi()`（业务接口）。

## 内容

- **声明式业务客户端**（`Interfaces/`，10 个接口 / 54 端点，全部落 `namespace Mud.Wechat.Pay`，实现类由 `Mud.HttpUtils.Generator` 产出到 `Mud.Wechat.Pay.Internal`）：
  - `Interfaces/Transactions/` `IWechatPayTransactionsService` — 基础交易 4 端点：JSAPI/小程序下单、微信订单号查单、商户订单号查单、关闭订单
  - `Interfaces/Refund/` `IWechatPayRefundService` — 退款 3 端点：申请退款、查询单笔退款、发起异常退款
  - `Interfaces/Bill/` `IWechatPayBillService` — 账单 2 端点：申请交易账单、申请资金账单（文件另走下载通道）
  - `Interfaces/Certificates/` `IWechatPayCertificatesService` — 平台证书 1 端点：获取平台证书列表（应答/回调验签与序列号轮换）
  - `Interfaces/ProfitSharing/` `IWechatPayProfitSharingService` — 分账 9 端点：接收方添加/删除、请求分账 + 查询、回退单 + 查询、解冻、待分金额查询、分账账单
  - `Interfaces/PayScore/` `IWechatPayPayScoreService` — 支付分 11 端点：服务订单创建/查询/取消/完结/修改/收款/同步 + 授权预授权/解除/按授权码或 openid 查询
  - `Interfaces/Combine/` `IWechatPayCombineService` — 合单支付 6 端点：JSAPI/Native/APP/H5 下单 + 合单查询 + 合单关单（两面路由共用，域 DTO 以服务商面建模）
  - `Interfaces/Transfer/` `IWechatPayTransferService` — 商家转账 6 端点（含「不换单重试」所需的查单/撤销/电子回单）
  - `Interfaces/Fapiao/` `IWechatPayFapiaoService` — 电子发票 5 端点：开具、查询、冲红、获取下载信息、插卡
  - `Interfaces/MarketingFavor/` `IWechatPayMarketingFavorService` — 代金券 7 端点：批次创建/启动/暂停/续期/查询 + 发券 + 券详情（**创建成功 ≠ 可发放**，须先启动批次）
- **模块注册器**（`Extensions/`）：`AddWechatPayApi(...)` + `PayServiceBuilder`（`AddAllApis()` / `AddModules(params PayModule[])` / `Build()`）。`PayModule` 10 个成员即各接口 `[HttpClientApi(RegistryGroupName = …)]` 的组名，**叫 `Transactions` 等而非 `Pay`**——本仓「企业支付」已占用 `WechatModule.Pay` 与生成的 `AddPayWebApiHttpClient()`，同名会跨程序集歧义。
- **非声明式服务**（官方列在 TOC 内但无 HTTP 形态，或路由/应答非 JSON）：
  - `Transactions/` `IWechatPayMiniProgramPaySignService` — 小程序调起支付五参数（**本地 RSA 签名、无请求路由**）
  - `Download/` `IWechatPayBillDownloadService` — 账单文件下载通道（`download_url` 动态给出、应答非 JSON）
  - `Fapiao/` `IWechatPayFapiaoFileService` — 发票文件上传（`multipart/form-data` + SM3 摘要）
  - `Certificates/` `WechatPayPlatformCertificateRefresher` — 未知序列号时的按需刷新（三道防放大闸 + fail-closed）
- **AOT JsonContext 合并**（`Extensions/PayJsonResolverExtensions.cs`）：`Build()` 把 DataModels 的 12 个生成上下文经 `JsonTypeInfoResolver.Combine` 注进 `wechat-pay` 命名客户端管线。

## 用法

### 注册

```csharp
// Program.cs：凭据底座（Abstractions 的 AddPayApp）必须先于业务接口
builder.Services.AddSingleton<ISecretProvider>(new MyVaultSecretProvider());   // 密钥端口由宿主实现
builder.Services.AddPayApp(builder.Configuration, "WechatPayMerchants")
                .AddWechatPayApi(b => b.AddModules(PayModule.Transactions, PayModule.Refund));
// 或全量：.AddWechatPayApi(b => b.AddAllApis())
```

`AddPayApp` 与 `AddWechatPayApi` 各管一段：`Build()` 会校验 `IWechatPayMerchantManager` 已注册，漏装底座**立即抛出并点名修复方式**（支付无 errcode 令牌自愈，坏配置拖到真实下单才暴露的排查成本极高）。

### 下单并下发小程序调起参数

```csharp
public sealed class OrderAppService(
    IWechatPayTransactionsService transactions,
    IWechatPayMiniProgramPaySignService paySign)
{
    public async Task<WechatPayMiniProgramPaySign> CreateAsync(
        string appId, string openId, string outTradeNo, long totalInCents, string notifyUrl, CancellationToken ct)
    {
        var response = await transactions.CreateJsapiOrderAsync(new JsapiPrepayRequest
        {
            AppId = appId,
            OutTradeNo = outTradeNo,                                   // 6-32 字符、商户号下唯一
            Description = "订单描述",                                  // 官方必填
            NotifyUrl = notifyUrl,                                     // 须与回调路由一致（见 Callback 包）
            Amount = new JsapiAmountInfo { Total = totalInCents, Currency = "CNY" },
            Payer = new JsapiPayerInfo { OpenId = openId },
        }, ct);

        // prepay_id 仅 2 小时有效；paySign 由服务端用商户私钥算出，私钥绝不下发到端上
        return await paySign.CreatePaySignAsync(appId, response.PrepayId!, ct);
    }
}
```

### 多商户作用域

单商户宿主零样板（唯一商户即默认商户）；多商户宿主未开作用域时解析**直接 fail-fast**，不会静默取错商户：

```csharp
public sealed class SubMchCaller(IWechatPayMerchantContext merchantContext, IWechatPayRefundService refund)
{
    public Task<RefundResponse> RefundForAsync(string merchantKey, RefundApplyRequest request, CancellationToken ct)
    {
        using (merchantContext.UseMerchant(merchantKey))   // using 一次性作用域，释放即还原
        {
            return refund.CreateRefundAsync(request, ct);   // 签名头与 mchid 均解析到该商户
        }
    }
}
```

### 错误处理

APIv3 业务失败是 **HTTP 4xx/5xx + `{"code","message"}`**，接口一律 `[AllowAnyStatusCode]` 让错误体原样落到 `WechatPayResponse.Code`/`Message`，再由 `WechatPayException` 判定：

```csharp
try
{
    var order = await transactions.QueryByOutTradeNoAsync(outTradeNo, mchId, ct);
}
catch (WechatPayException ex) when (ex.PayErrorCode == WechatPayErrorCodes.OrderNotExist)
{
    // 官方业务码为字符串（WechatPayErrorCodes 常量表）；NumericFailureSentinel = -1 表示无官方码
}
```

也可对响应显式断言：`WechatPayException.ThrowIfFailed(response, requestUri)`。

## 依赖

- `Mud.Wechat.Pay.Abstractions`、`Mud.Wechat.Pay.DataModels`、企业微信/公众号/支付共用的叶层 `Mud.Wechat.Abstractions`
- `Mud.HttpUtils` 3.0.3 + `Mud.HttpUtils.Generator` 3.0.3（分析器，`PrivateAssets=all`）
- **不引用** `Mud.Wechat.Work*` / `Mud.Wechat.OfficialAccount*`（守卫 `PAY-B11` 锁定工程引用隔离）

## 说明

- **目标框架为受控例外**：`net6.0;net8.0;net10.0`，无 `netstandard2.0`——回调 `resource` 解密强制 `AEAD_AES_256_GCM`，而 `AesGcm` 在 ns2.0 不存在；手写 GCM 与引入第三方密码学包均被否（守卫 `PAY-B9` / `AB-G8` 锁定，签名与加解密全走 BCL、AOT 安全）。
- **无 `[Token]`**：签名在传输层 `WechatPayAuthorizationHandler` 完成，端点方法只管路由与报文。主包 csproj 仍带 `<NoWarn>MUD005</NoWarn>` 只是「防未来误加时误读」，真正的禁令由 `PAY-B1` 断言。
- **不用默认 `IEnhancedHttpClient` 实例**：走命名客户端 `wechat-pay`（`WechatPayHttpClientNames`），否则会与企微线 `qyapi.weixin.qq.com` 撞 BaseUrl 并给企微请求套上商户签名头。SSRF 白名单由 `AddPayApp` 经公用层窄入口登记，**数组零改动**（`PAY-B8`）。
- **端点/路由/字段名的权威是契约守卫**，不是本文与 `PayModule` 的 XML 注释（后者仍留有早期「首批 N 端点」口径）：`Tests/Mud.Wechat.Pay.Tests/ContractGuards/` 11 个文件——`PAY-B1`（零 `[Token]`）、`PAY-B2`（签名/验签串黄金向量）、`PAY-B3`（验签与时间窗 fail-closed）、`PAY-B4`（GCM 参数与黄金向量）、`PAY-B5`（54 端点计数 + 官方路由 + snake_case 字段照抄原文）、`PAY-B6`（无反射、无第三方密码学）、`PAY-B7`（凭据不入日志）、`PAY-B9`（TFM）、`PAY-B11`（引用隔离）+ 逐域守卫（分账 / 支付分 / 合单 / 转账 / 发票 / 代金券）与 `WechatPayCapitalRulingContractGuards`（未建模域裁决，如 `/v3/merchant/fund`）。
- **发票文件下载有意不实现**：官方文件域名 `pay.wechatpay.cn` 不在进程级白名单内，白名单零改动优先级高于该便利（`PAY-B8`）。账单 `download_url` 5 分钟有效且一次性，取到即下载。
- 新增 `[HttpJsonSerializable]` DTO 后运行 `scripts/AddHttpJsonSerializable.ps1` + `scripts/GenerateJsonContext.ps1`（`mud-jsonctx`）重新生成，并把新上下文登记进 `PayJsonResolverExtensions`；未登记类型在 Native AOT 下会因无元数据失败（JIT 下靠反射侥幸可用）。
