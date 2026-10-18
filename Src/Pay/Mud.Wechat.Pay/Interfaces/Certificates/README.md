# Certificates 域 · 平台证书

微信支付「平台证书」域 SDK（APIv3 获取平台证书列表，1 端点）。 接口：[IWechatPayCertificatesService](./IWechatPayCertificatesService.cs)；注册组 `Certificates`；模块枚举 `PayModule.Certificates`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay.DataModels/Certificates`](../../../Mud.Wechat.Pay.DataModels/Certificates/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayCertificatesService = 1；按 HTTP 特性实计 **1** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayCertificatesService` | `GetPlatformCertificatesAsync` | GET | `/v3/certificates` | 商户证书 / APIv3 签名 | 获取平台证书列表。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012551764<br>（官方接口名 `DirectAPIv3Cer… |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**官方文档**（普通商户文档中心，2026-10-09 逐页核验）：
下载平台证书 https://pay.weixin.qq.com/doc/v3/merchant/4012551764
（概览 `4024350132`；页面更新时间 2024.09.13）。

**用途**：应答 / 回调验签需要**平台证书公钥**。本端点用于**定时拉取并缓存**平台证书，
以处理 `Wechatpay-Serial` 轮换（未知 serial ⇒ 触发一次刷新后仍失败即拒，**不得刷新失败即放行**，
见红线 §7.3）。

**无 `[Token]`、走商户签名**（守卫 PAY-B1）：形态与
`IWechatPayTransactionsService` 一致，详见其 remarks。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `GetPlatformCertificatesAsync` — 获取平台证书列表。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4012551764
（官方接口名 `DirectAPIv3Certificates`）。

- **声明接口**：[IWechatPayCertificatesService](./IWechatPayCertificatesService.cs)
- **官方契约**：`GET /v3/certificates`
- **返回**：`PlatformCertificatesResponse` — 平台证书列表（`data[]`，证书内容为 AES-256-GCM 密文），见 `PlatformCertificatesResponse`。

**备注（原文）**

- **官方契约**：**GET** `/v3/certificates`；**无 query / path 参数**； / 必带 `Accept: application/json`。支持商户类型：**普通商户**。
- **频率限制**：单个商户号 **1000 次/s**（官方原文）。仍应**定时低频拉取**并本地缓存， / 逐请求拉取会带来无谓往返。
- **解密**：应答中的证书为密文（`encrypt_certificate`），须用 APIv3 密钥经 / `WechatPayAesGcmCodec` 解密为 PEM 明文后方可构造 X509 证书验签。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay.DataModels/Certificates`](../../../Mud.Wechat.Pay.DataModels/Certificates/) 内共 **3** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **3** 个，`SerializerClassName` = `Certificates`。

AOT 源生成上下文：[`CertificatesJsonContext.g.cs`](./../../../Mud.Wechat.Pay.DataModels/Generated/CertificatesJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `PlatformCertificatesResponse` | 应答 | [Certificates/PlatformCertificatesResponse.cs](../../../Mud.Wechat.Pay.DataModels/Certificates/PlatformCertificatesResponse.cs) | `data`, `serial_no`, `effective_time`, `expire_time`, `encrypt_certificate`, `algorithm`, `nonce`, `associated_data`, `ciphertext` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.Certificates;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.Certificates));

var service = app.Services.GetRequiredService<IWechatPayCertificatesService>();
var ct = CancellationToken.None;
var response = await service.GetPlatformCertificatesAsync(ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/certificates");
// 占位值仅用于编译示意；业务值须按 IWechatPayCertificatesService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetPlatformCertificatesAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 业务错误码全集 | 本仓接口 XML 注释**未逐条摘录**业务错误码 ⇒ **未逐项核验** | 需要按码分支时先核验官方页，再补接口注释 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayCertificatesService](./IWechatPayCertificatesService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay.DataModels/Certificates`](../../../Mud.Wechat.Pay.DataModels/Certificates/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
