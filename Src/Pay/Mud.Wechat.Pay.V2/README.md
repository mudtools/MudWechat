# Mud.Wechat.Pay.V2

微信支付 **APIv2** SDK **主包**：XML 报文 + 报文体内 `sign`（MD5 / HMAC-SHA256）+ 商户 API 证书双向 TLS 的传输基座，以及 **14 个模块 / 58 支端点**（57 条不同路径，`/papay/deletecontract` 为商户与服务商两族共用）。

> **与 APIv3 主包（`Mud.Wechat.Pay`）是两条独立传输线**：APIv3 走 JSON + `Authorization: WECHATPAY2-SHA256-RSA2048 …` 头签名；APIv2 走 XML + **报文体里的 `sign`**，业务失败也用 HTTP 200 + `return_code=FAIL` 表达。两线**不共用** DTO、序列化器、命名客户端与契约守卫，也**不共用密钥**（`ApiV2KeySecretName` 与 `ApiKeySecretName` 是两个字段、两把值）。

## 内容

- **`Interfaces/Pay/IWechatPayV2Service.cs`** — 声明式基础支付 8 端点（`[HttpClientApi(RegistryGroupName = "BasePayV2", HttpClient = IWechatPayV2HttpClient)]` + `[AllowAnyStatusCode]`）：
  `/pay/unifiedorder` 统一下单、`/pay/micropay` 付款码支付、`/pay/orderquery` 查单、`/pay/closeorder` 关单、`/pay/refundquery` 退款查询、`/pay/refundqueryv2` 退款查询 v2、`/pay/settlementquery` 结算查询、`/pay/queryexchagerate` 汇率查询（官方原文即少一个 `t`，照抄）。
- **`Interfaces/Secure/IWechatPayV2SecureService.cs`** — 需双向证书的 5 端点（**无** `[HttpClientApi]`，手写通道服务）：
  `/secapi/pay/refund`、`/secapi/pay/refundv2`、`/secapi/pay/reverse`、`/pay/downloadbill`、`/pay/downloadfundflow`。后两支成功应答是 **CSV 原文** ⇒ 返回 `Task<string>`、不做应答验签。
- **`Transport/`** — `WechatPayV2ContentSerializer`（DTO ⇄ XML，源生成 `PayV2JsonContext` 驱动）、`WechatPayV2XmlCodec`（扁平字段表 ⇄ `<xml>`）、`WechatPayV2SigningHandler`（出站补 `sign` + 入站验签）、`WechatPayV2HttpClient` / `WechatPayV2HttpClientNames`。
- **`Security/`** — `WechatPayV2SignatureBuilder`（stringA 规范化 + MD5 / HMAC-SHA256 + 验签）、`WechatPayV2Signer`（算法判定与应答双试）。
- **`Credential/`** — `IWechatPayV2CredentialProvider` / 默认实现：经 `ISecretProvider` 取 APIv2 签名密钥（校验 32 字节）与 `apiclient_cert.p12`（口令默认商户号，可另配密钥名）。
- **`Secure/`** — `WechatPayV2SecureChannelProvider` / `WechatPayV2SecureChannel`：逐商户一条独立 `HttpClient`（含独立连接池）承载双向 TLS。
- **`Extensions/`** — `PayV2Module` 枚举 + `PayV2ServiceBuilder` + `AddWechatPayV2Api()` 入口。

## 域清单（模块 ≡ 注册组 ∪ 手写族名单）

| 模块 | 端点 | 形态 | 域名 |
|---|---|---|---|
| `BasePayV2` | 13 = 8 声明式 + 5 证书 | 混合 | `api.mch` / `api2.mch` |
| `PapayV2` | 6 | 声明式 | `api.mch` |
| `PartnerPapayV2` | 5 | 声明式 | `api.mch` |
| `RedPackV2` | 5 | **手写证书族** | `api.mch` |
| `TransferV2` | 4 | **手写证书族**（含企微双签两支） | `api.mch` |
| `PartnerTransferV2` | 2 | **手写证书族** | `api.mch` |
| `DepositV2` | 8 = 5 声明式 + 3 证书 | 混合 | `api.mch` / `api2.mch` |
| `CustomsV2` | 3 | 声明式（**无** `nonce_str`） | `api.mch` |
| `ToolsV2` | 2 | 声明式 | `api.mch` |
| `RiskV2` | 1 | **手写证书族**（请求**无** `appid`） | `api.mch` |
| `SubMerchantConfigV2` | 3 | **手写证书族** | `api.mch` |
| `MerchantMicroV2` | 2 | **手写证书族**（敏感字段须加密） | `api.mch` |
| `InstitutionSubV2` | 3 | **手写证书族**（境外，仅 MD5） | `apihk.mch` |
| `MerchantMediaV2` | 1 | **手写证书族**（本线唯一 `multipart/form-data`） | `apihk.mch` |

## 接入

```csharp
services.AddPayApp(configuration, "WechatPayMerchants")   // 凭据底座（两线共用同一份商户配置）
        .AddWechatPayApi(builder => builder.AddAllApis())  // APIv3（可选）
        .AddWechatPayV2Api(builder => builder.AddAllApis()); // APIv2
```

`AddWechatPayV2Api` 在 `AddPayApp` **之后**调用，否则注册期即抛（`IWechatPayMerchantManager` 未注册）。

## 已踩陷阱（改动前必读）

1. **签名放 Handler，不放序列化器**：`sign` 的作用域是报文体全部非空顶层字段，必须在体成形后计算；而密钥出自异步 `ISecretProvider`，`ToHttpContent<T>` 是同步方法。两难只能在 `DelegatingHandler` 里同时解开。
2. **证书端点不能进命名客户端**：`IHttpClientFactory` 的主 Handler 按**客户端名**缓存、连接池亦按名复用 ⇒ 一个名字绑不了多商户证书，否则「商户 A 的请求跑在商户 B 的 TLS 会话上」。本线全部需证书端点（基础支付 5 支 + 红包 5 + 付款 4 + 服务商付款 2 + 押金 3 + 风控 1 + 子商户配置 3 + 小微 2 + 机构进件 3 + 媒体上传 1）一律走 `WechatPayV2SecureChannelProvider` 的逐商户通道，**别顺手给任何手写族接口补 `[HttpClientApi]`**。
3. **13 端点分两面由守卫加法锁定**：`WechatPayV2BasePaymentContractGuards` 的 `…_ShouldHaveEightEndpoints` + `…_ShouldHaveFiveEndpoints` + `…_ShouldSumToThirteen` 三处同批维护，任何一侧增减都会打红。
4. **应答验签双试**：官方应答体**不带** `sign_type`，算法实际由请求侧选定 ⇒ `WechatPayV2Signer.Verify` 对 MD5 与 HMAC-SHA256 **两种都试、任一命中即通过**（伪造仍需密钥，不削弱验签），否则会自伤成「HMAC 下单 ⇒ 应答验签必败」。应答若自带 `sign_type` 则严格按声明判。
5. **已带 `sign` 的请求不重签**（幂等放行）：字段表口径一旦与首发有差就会产出「体与 `sign` 互相矛盾」的报文，官方只回 `SIGNERROR`，比漏签更难查。回归锁：`WechatPayV2TransportPipelineTests.V2Client_ShouldNotResign_WhenBodyAlreadyCarriesSign`。
6. **`T == string` 直发原文**：CSV 下载类端点没有 XML 应答可反序列化，序列化器对该类型短路返回原始文本。
7. **未登记 DTO 即点名报错**（不回落到反射序列化）：新增 APIv2 DTO 必须标 `[HttpJsonSerializable(SerializerClassName = "PayV2")]` 并重跑 `scripts/AddHttpJsonSerializable.ps1` + `scripts/GenerateJsonContext.ps1`。
8. **密钥轮换需重启**：APIv2 签名密钥按商户缓存（`WechatPayV2SigningHandler` / `WechatPayV2SecureService` 各一份缓存），与 APIv3 缓存签名提供者同一口径。
9. **异常消息红线**（PAY-B7 同线）：任何异常与日志**不得**出现密钥值、`sign` 值、签名原文串或证书材料，只允许商户键、配置字段名与截断后的诊断文本。
10. **SSRF 白名单是「三个编译期常量」的闭集**：`api.mch.weixin.qq.com`、`api2.mch.weixin.qq.com`、`apihk.mch.weixin.qq.com` 都已被 `WechatApiHosts` 的 `weixin.qq.com` 后缀覆盖；本线**不给**商户级 `BaseUrl` 覆盖留门。证书通道绕过 `IHttpClientFactory`，其唯一的域名闸是 `WechatPayV2SecureChannelProvider.GetChannelAsync` 里的三段 `!=` 判断 ⇒ **新增第四个域名必须同时改常量表与这段白名单**（少改一处即误拒，多放一个商户可控值即绕过防线）。
11. **「要证书」与「挂在证书域名上」是两件独立的事**：`/secapi/mch/*`（子商户配置）与 `/applyment/micro/*`（小微）要商户证书却仍挂 `api.mch`；只有 `api2.mch` 上的 `/secapi/pay/*` 与国内 `api.mch` 上的 CSV 下载构成 `SecureBaseAddress` 的适用范围；境外两族（机构进件、媒体上传）官方「接口说明」栏只印 `apihk.mch`。照前缀猜域名 ⇒ 404。
12. **`sign_type` 的「有无」与「默认值」就是算法开关**：`WechatPayV2Signer` 只在字段表带 `sign_type=HMAC-SHA256` 时走 HMAC。故 DTO 的属性初始化器（`= "HMAC-SHA256"`、`= "1.0"`）属**契约面**：清空一个必填默认值 ⇒ 报文静默降级成 MD5 被打回 `SIGNERROR`；给官方没有 `sign_type` 的那几支（支付目录两支、机构进件三支、报关三支）补一个 ⇒ 同一症状。逐型锁定见 `WechatPayV2MerchantProfileContractGuards` 与 `…CustomsToolsRiskContractGuards`。
13. **本线唯一 `multipart/form-data` 端点**是 `/secapi/mch/uploadmedia`：字段仍按 XML 的同一张表签名、**文件分节不参与签名**（完整性由**小写** MD5 的 `media_hash` 承载），而应答仍是需验签的 XML ⇒ 走 `WechatPayV2SecureInvoker.PostMultipartAsync`，不要改用 `PostAsync` 把文件拼进 XML 体。
14. **敏感字段加密只有一条路**：`WechatPayV2SensitiveFieldEncrypter` 固定 `RSAES-OAEP + SHA-1 + MGF1(SHA-1)`（官方 Java 示例 `RSAWithSHA1AndMGF1Padding`），密钥是**微信支付平台证书**公钥，输出 Base64。这与 APIv3 的 `RSAES_OAEP_256` **不同源**，混用会得到「官方报参数错误」而不是「加密失败」。密文与 `cert_sn` 必须同源：按序列号从 `IWechatPayPlatformCertificateStore` 取证书，取不到即 fail-closed（`WechatPayV2SensitiveFieldEncrypter.Encrypt(plaintext, store, serialNumber)`）。
15. **双签端点的 Secret 端口是惰性可选依赖**：`sendworkwxredpack` / `paywwsptrans2pocket` 需要企微自建应用 Secret，经 `IServiceProvider.GetService<IWechatPayV2WorkWxSecretProvider>()` 在**调用时**解析。不要把它提成 `WechatPayV2SecureInvoker` 的构造参数——那会把全部宿主绑上企微凭据桥接。
16. **用户身份与金融字段零落地**：`mobile`/`email`/`creid`/`qq`/`outerid`/`risk_info`/`re_openid`/`openid`/`desc`、`enc_bank_no`/`enc_true_name`/`bank_no_md5`/`true_name_md5`、报关的 `cert_id`/`name`、小微的 `account_number`/`mobile_phone`/`email`（无论明文密文）、`violation_records` 原文、`sandbox_signkey` 与 `getpublickey` 返回的公钥材料——一律**不进日志、不入库、不入异常消息**（PAY-B7 同线口径）。

## 契约与守卫

- `WechatPayV2BasePaymentContractGuards` — 基础支付 13 支（8 + 5）、注册组、注入客户端、与 APIv3 的程序集隔离、`PayV2Module ≡ 注册组 ∪ 手写族名单` 总不变式。
- `WechatPayV2PapayContractGuards` — 委托代扣双族 11 支。
- `WechatPayV2CertFamilyContractGuards` — 整族手写的红包 / 企业付款 / 服务商付款（11 支）+ `HandWrittenModules` 名单（8 项）+ DI 单例与模块隔离。
- `WechatPayV2DepositContractGuards` — 押金「一域两形态」（5 声明式 + 3 证书）。
- `WechatPayV2CustomsToolsRiskContractGuards` — 报关 3 + 工具 2 + 风控 1 的例外事实（无 `nonce_str`、请求无 `appid`、官方不写请求方式）。
- `WechatPayV2MerchantProfileContractGuards` — 子商户配置 3 + 小微 2 + 机构进件 3 + 媒体上传 1：域名二分、`sign_type` 逐型、`app_id` vs `appid`、multipart 出口、SSRF 三常量闭集、加密器算法与 fail-closed。
- `Tests/Mud.Wechat.Pay.Tests/Transport/WechatPayV2TransportPipelineTests.cs` — 真实 DI 管道：`sign` 独立复算（MD5 / HMAC）、幂等放行、应答验签双向前置、跨 scope 单例、底座顺序 fail-fast、证书缺失 fail-fast。
