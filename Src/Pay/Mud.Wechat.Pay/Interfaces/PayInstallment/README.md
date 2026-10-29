# PayInstallment 域 · 微信分付曝光建议

微信支付「微信分付曝光建议」域 SDK（1 端点）。 接口：[IWechatPayPayInstallmentService](./IWechatPayPayInstallmentService.cs)；注册组 `PayInstallment`；模块枚举 `PayModule.PayInstallment`（装配入口见 [PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[WechatPayPayInstallmentContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPayInstallmentContractGuards.cs)、[WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Pay/DataModels/PayInstallment`](../../DataModels/PayInstallment)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IWechatPayPayInstallmentService = 1；按 HTTP 特性实计 **1** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IWechatPayPayInstallmentService` | `QueryPayInstallmentExposureAsync` | POST | `/v3/pay/instal/exposure` | 商户证书 / APIv3 签名 | 获取微信分付曝光建议。<br>官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4016196762（【普通商户】）。 |

## 令牌与认证

支付线**没有 `access_token`**：APIv3 全程用商户 API 证书做请求签名，签名在传输层处理器完成，端点方法只管路由与报文。因此本域接口零 `[Token]`（Pay 线红线由守卫锁定，见「事实来源」）；本域若有 `[AllowAnyStatusCode]`，其作用是让 APIv3 业务失败的 4xx/5xx 响应体（`code`/`message`）原样落到响应对象，而不是在传输层被抛成 `ApiException`。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**逐端点契约**：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与参数表必填口径。取证结果：本接口 1 支中**1 支官方只印一页**、**0 支印多页** ⇒ 一支路由一方法。

**无 `[Token]`、走商户签名**（守卫 PAY-B1 形态）：形态与 `IWechatPayTransactionsService` 一致，商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择，本域与公众平台令牌面无关。

**`[AllowAnyStatusCode]` 的必要性**：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，加上后错误体原样落到 `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

**频率限制口径**：1 / 1 支官方页**印有**端点级数字（已原样抄入各方法 remarks），其余支**未印** ⇒ 未印者不得凭第三方 SDK 补数。

**一支、官方只印商户页，且「查询」语义走 POST**：`POST /v3/pay/instal/exposure`（原文请求方式即 POST）⇒ 照页不得改成 GET。

**官方把限速与耗时都印在正文**：「接口频率：**每个商户限流200/s**，上线前必须与微信支付运营确认」 +「接口耗时：200ms」⇒ 数字照页入 remarks，且「上线前须与运营确认」是**前置动作**，不得省略为纯技术阈值。

**入参两键联动、SDK 不做本地校验**：`user_id_type` 决定用户标识口径，`openid` 官方标**选填** （传 OPENID 口径时才需给出）⇒ 两者均以可空参数承载，联动关系写在注释而**不**做成必传参数或抛异常。

**应答 `repayment_info` 是 array[object] 且官方给元素两键**（`repayment_type`、 `exist_cash_off_coupon`）⇒ 建元素类型而非 `JsonElement`；根键 `recommend_exposure` 是必填布尔， 含义是「**建议曝光**」而非「用户已开通分付」⇒ 不得据其判定开通态，只作曝光决策入参。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 1. `QueryPayInstallmentExposureAsync` — 获取微信分付曝光建议。
官方文档：https://pay.weixin.qq.com/doc/v3/merchant/4016196762（【普通商户】）。

- **声明接口**：[IWechatPayPayInstallmentService](./IWechatPayPayInstallmentService.cs)
- **官方契约**：`POST /v3/pay/instal/exposure`

- **请求体**：`PayInstallmentExposureRequest` — 获取微信分付曝光建议请求体，字段见 `PayInstallmentExposureRequest`。
- **返回**：`PayInstallmentExposureResponse` — 获取微信分付曝光建议，见 `PayInstallmentExposureResponse`。

**备注（原文）**

- **官方契约**：**POST** `/v3/pay/instal/exposure`；无 path / query 参数。
- **支持商户**【普通商户】。
- **频率限制**（官方原样）：接口频率：每个商户限流200/s，上线前必须与微信支付运营确认。
- **应答**见 `PayInstallmentExposureResponse`（官方应答表逐字段建模）。
- **业务错误码**：400 PARAM_ERROR：参数错误 / 400 INVALID_REQUEST：HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR：验证不通过 / 500 SYSTEM_ERROR：系统异常，请稍后重试。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Pay/DataModels/PayInstallment`](../../DataModels/PayInstallment) 内共 **3** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **3** 个，`SerializerClassName` = `PayInstallment`。

AOT 源生成上下文：[`PayInstallmentJsonContext.g.cs`](./../../DataModels/Generated/PayInstallmentJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `PayInstallmentExposureResponse` | 应答 | [PayInstallment/PayInstallmentExposureDtos.cs](../../DataModels/PayInstallment/PayInstallmentExposureDtos.cs) | `mchid`, `appid`, `user_id_type`, `openid`, `repayment_type`, `exist_cash_off_coupon`, `recommend_exposure`, `repayment_info` |
| `PayInstallmentExposureRequest` | 请求体 | [PayInstallment/PayInstallmentExposureDtos.cs](../../DataModels/PayInstallment/PayInstallmentExposureDtos.cs) | `mchid`, `appid`, `user_id_type`, `openid`, `repayment_type`, `exist_cash_off_coupon`, `recommend_exposure`, `repayment_info` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Pay;
using Mud.Wechat.Pay.DataModels.PayInstallment;

builder.Services.AddPayApp(builder.Configuration, "WechatPay");
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.PayInstallment));

var service = app.Services.GetRequiredService<IWechatPayPayInstallmentService>();
var ct = CancellationToken.None;
var response = await service.QueryPayInstallmentExposureAsync(new PayInstallmentExposureRequest(), ct);
// 业务失败判定：WechatPayException.ThrowIfFailed(response, "/v3/pay/instal/exposure");
// 占位值仅用于编译示意；业务值须按 IWechatPayPayInstallmentService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`QueryPayInstallmentExposureAsync`）；请求体 `PayInstallmentExposureRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IWechatPayPayInstallmentService](./IWechatPayPayInstallmentService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`WechatPayPayInstallmentContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayPayInstallmentContractGuards.cs)、[`WechatPayDomainContractGuards.cs`](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs) |
| 守卫中的代表性测试方法 | `PayInstallmentTopology_ShouldMatchInterfaceAndGroupRoster`、`PayInstallmentEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`、`PayInstallmentDtos_ShouldExposeOfficialFieldRosters`、`PayInstallmentDataModels_ShouldSplitRequestAndResponseBaseTypes`、`PayInstallmentAllDtos_ShouldBeAnnotatedAndContextRegistered`、`PayInstallmentRegistrars_ShouldBeWiredThroughPayModuleAndResolver`、`PayInterfaces_ShouldNotDeclareToken_AndShouldMatchModuleCount`、`Endpoints_ShouldMatchOfficialRoutes`、`Dtos_ShouldExposeOfficialFieldNames`、`Responses_ShouldDeriveFromWechatPayResponse`、`Interfaces_ShouldAllowAnyStatusCode_SoErrorBodiesAreReadable` |
| 模块枚举 / 装配入口 | [`PayModule.cs`](../../Extensions/PayModule.cs)、[`PayServiceBuilder.cs`](../../Extensions/PayServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Pay/DataModels/PayInstallment`](../../DataModels/PayInstallment) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Pay/Mud.Wechat.Pay/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`WechatPayPayInstallmentContractGuards.cs`、`WechatPayDomainContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**给本域任何端点加 `[Token]`（Pay 线红线）；**不要**在 DTO 里放 `DateTime`（rfc3339 一律 `string`）；**不要**把服务商面与直连面的字段口径互相搬运。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
