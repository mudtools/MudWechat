# Apply4SubMerchant 域 · 特约商户进件（服务商为特约商户进件 + 结算账户查询与修改）

微信支付 APIv3「特约商户进件」**6 支声明式端点**（申请单族 3：提交申请单 + 按申请单号 / 按业务申请编号
查状态；结算账户族 3：修改结算账户 + 查询结算账户 + 查询结算账户修改申请状态）。
接口 [IWechatPayApply4SubMerchantService.cs](./IWechatPayApply4SubMerchantService.cs)，
注册组 `Apply4SubMerchant`，模块枚举 `PayModule.Apply4SubMerchant`
（Pay 线**没有** `AddApply4SubMerchantApi()` 这种公有方法，装配入口是
`AddWechatPayApi(b => b.AddModules(PayModule.Apply4SubMerchant))`，内部调用源生成的
`AddApply4SubMerchantWebApiHttpClient()`，见 [../../Extensions/PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、
> 本域契约守卫（[WechatPayApply4SubMerchantContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayApply4SubMerchantContractGuards.cs)
> 与全局 [WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、
> 本域 DTO（[../../../Mud.Wechat.Pay/DataModels/Apply4SubMerchant/](../../DataModels/Apply4SubMerchant)）。
> 不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

计数权威有两处，须同时成立：**全局 `PAY-B5`** 的本域路由名册（`RoutesOf(asm,
"IWechatPayApply4SubMerchantService")` 逐条 `BeEquivalentTo`，6 条）锁路由集合，
**本域 `A4S-B2`** 的 `routes.Should().HaveCount(6, …)` 锁数量（其断言消息原文要求
「数量变化须同批更新 A4S-B1 与 Tests/ContractBaseline.cs」）；`A4S-B1` 逐支锁方法与路由原文。

| # | 方法 | HTTP 动词 | 路由 | 认证/签名方式 | 备注 |
|---|---|---|---|---|---|
| 1 | `SubmitApplymentAsync` | POST | `/v3/applyment4sub/applyment/` | 商户证书 / APIv3 请求签名（敏感字段另需 `Wechatpay-Serial`） | **结尾斜杠是官方原文，不可删**；应答只有 `applyment_id` |
| 2 | `QueryApplymentStateByApplymentIdAsync` | GET | `/v3/applyment4sub/applyment/applyment_id/{applymentId}` | 同上 | 仅 path；`[Path]` 参数统一 `string`（官方声明 integer） |
| 3 | `QueryApplymentStateByBusinessCodeAsync` | GET | `/v3/applyment4sub/applyment/business_code/{businessCode}` | 同上 | path `business_code` string(124) |
| 4 | `ModifySettlementAsync` | POST | `/v3/apply4sub/sub_merchants/{subMchId}/modify-settlement` | 同上（`account_number` / `account_name` 须加密） | 应答只有 `application_no` ⇒ 异步受理 |
| 5 | `QuerySettlementAsync` | GET | `/v3/apply4sub/sub_merchants/{subMchId}/settlement` | 同上 | 选填 query `account_number_rule` |
| 6 | `QuerySettlementModifyApplicationAsync` | GET | `/v3/apply4sub/sub_merchants/{subMchId}/application/{applicationNo}` | 同上 | **双 path** + 选填 query `account_number_rule` |

## 认证：本线没有 access_token

APIv3 全程用**商户 API 证书**做 `WECHATPAY2-SHA256-RSA2048` 请求签名，签名在传输层
`WechatPayAuthorizationHandler` 完成。本域特有的一处是**平台公钥校验面**：
进件与修改结算账户的敏感字段要用**微信支付公钥**（推荐）或平台证书公钥加密上送，
请求头 `Wechatpay-Serial` 指向对应公钥 ID / 证书序列号（由传输层按商户配置写入，声明式签名不带该参数）。
因此：

- 本接口**不声明 `[Token]`**，守卫 `PAY-B1` 对整条 Pay 线 fail-closed。
- 走命名客户端 `wechat-pay`（`WechatPayHttpClientNames.TypeName`），BaseAddress 恒为
  `https://api.mch.weixin.qq.com`。
- `[AllowAnyStatusCode]` 必需：业务失败是 HTTP 4xx/5xx + `{"code","message"}`，落到
  `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

## 「两个资源族 + 两种定位键」：本域最易写错的一处

进件申请单在 `/v3/applyment4sub/applyment…`，结算账户在 `/v3/apply4sub/sub_merchants/{sub_mchid}…`
—— 两族前缀互为「4sub」的镜像（`applyment4sub` vs `apply4sub`），**不得交叉**：
`A4S-B2` 断言含 `/sub_merchants/` 的路由必须以 `/v3/apply4sub/` 开头、含 `/applyment/` 的路由必须以
`/v3/applyment4sub/` 开头。申请单族**不含 `sub_mchid`**（提交时特约商户号还不存在，`A4S-B1` 逐支断言
参数名不得出现 `subMchId`）；结算账户族恒以 `subMchId` 定位。

## 该用哪种商户配置形态（本域最容易踩空的一处）

- 签名主体是**服务商**商户号：`Authorization` 头的 `mchid` 由 `WechatPayMerchantConfig.AuthorizationMchId` 给出。
- 服务商形态（`SpMchId` + `SubMchId`）由 `Validate()` 强制两者**成对**，而进件阶段特约商户号还不存在
  ⇒ 进件族三支应以**服务商自有商户号注册的普通商户形态**进入调用作用域。
- 结算账户三支虽然 path 带 `sub_mchid`，**它是每次调用传入的参数，不是商户作用域**
  ⇒ 本域任何端点都**不得**依赖 `WechatPayMerchantConfig.SubMchId`。
- 多商户切换经 `IWechatPayMerchantContext.UseMerchant(...)`。

## 异步状态机：`applyment_state` 八值 + 两处人工环节

`Apply4SubApplymentStates`（`A4S-B5` 逐值锁定）：

| 取值 | 语义 | 下一步由谁做 |
|---|---|---|
| `APPLYMENT_STATE_EDITTING` | 提交发生错误导致（官方拼写如此，非用户可停留态） | 商户重新提交 |
| `APPLYMENT_STATE_AUDITING` | 审核中 | 等待 |
| `APPLYMENT_STATE_REJECTED` | 已驳回，读 `audit_detail` 改资料 | 商户改后以**同 `business_code`** 重提 |
| `APPLYMENT_STATE_TO_BE_CONFIRMED` | 待账户验证 | **超级管理员**用 `sign_url` 人工完成 |
| `APPLYMENT_STATE_TO_BE_SIGNED` | 待签约 | **超级管理员**用 `sign_url` 人工完成 |
| `APPLYMENT_STATE_SIGNING` | 开通权限中 | 等待 |
| `APPLYMENT_STATE_FINISHED` | 完成，`sub_mchid` **此时才出现** | 可用于后续调用 |
| `APPLYMENT_STATE_CANCELED` | 已作废 | 换新编号 |

判定口径（接口 XML 原文）：**判据只用 `applyment_state`**；`applyment_state_msg` 是人读文本；
`sub_mchid` 仅 `FINISHED` 后出现。SDK 无法代为推进 `TO_BE_CONFIRMED` / `TO_BE_SIGNED`。

同理，支 4 的应答只有 `application_no`，「拿到申请单号」**不等于**结算账户已改，
生效与否须由支 6 轮询 `Apply4SubSettlementAuditResults` 判定。

## 同名不同值：两组枚举**不得合并**（`A4S-B4`）

| 场景 | 进件侧字段 | 结算账户侧字段 | 守卫理由 |
|---|---|---|---|
| 账户类型 | `bank_account_type` → `Apply4SubBankAccountTypes`（`BANK_ACCOUNT_TYPE_*`） | `account_type` → `Apply4SubSettlementAccountTypes`（`ACCOUNT_TYPE_*`） | 两套值集，混用即静默传错枚举 |
| 验证 / 审核结果 | — | `verify_result` → 查询结算账户用 `Apply4SubSettlementVerifyResults`（`VERIFY_*`）、查修改申请用 `Apply4SubSettlementAuditResults`（`AUDIT_*`） | 同名字段两套语义 ⇒ 两个类型 |

⇒ 支 5 与支 6 的应答**不是**同一类型：`Apply4SubSettlementResponse` 上**没有** `account_name` 与
`verify_finish_time`，`Apply4SubSettlementModifyApplicationResponse` 才有（`A4S-B6` 分别断言继承
`WechatPayResponse`，并锁前者不得有 `VerifyFinishTime`）。

## 敏感字段：加密上送 + 掩码回读，SDK 不加解密

- 须加密（RSAES-OAEP，用**微信支付公钥**或平台证书公钥）的字段：联系人 / 经营者 / 法定代表人 /
  受益人的姓名、证件号、手机号、邮箱、证件地址，以及 `account_name`（开户名称）、
  `account_number`（银行账号）。本 SDK **只做透传**，加解密由宿主完成。
- 回读侧**永不**返回明文账号 / 户名，一律掩码，掩码规则由选填 query `account_number_rule` 指定
  （常量类 `Apply4SubAccountNumberRules`：`ACCOUNT_NUMBER_RULE_MASK_V1` = 前 2 后 2，
  `ACCOUNT_NUMBER_RULE_MASK_V2` = 前 6 后 4）。
- 这些字段值**不得**写日志、遥测或异常消息；本文与示例一律用假值。真实密钥、证书、
  `serial_number` 只走环境变量或本机 User Secrets。
- **口径冲突提示**：支 5 / 支 6 的 `<param>` 注释写「不传默认 `MASK_V1`」，而被守卫 `A4S-B5`
  锁定的常量值是 `ACCOUNT_NUMBER_RULE_MASK_V1`。传给官方的是哪一个字符串，注释与常量类不一致 ⇒
  见文末缺口表；调用时请显式用 `Apply4SubAccountNumberRules.MaskV1` / `MaskV2`，不要传裸字面量。

## 频率限制与重入：三处不同码、一处不同 HTTP 状态

| 端点 | 官方限制（接口 XML 原文） | 超限码 |
|---|---|---|
| 支 4 修改结算账户 | 接口 **20 次 / 分钟**，且**同一特约商户每日仅 5 次** | `429 FREQENCY_LIMIT`（官方拼写少一个 U，`Apply4SubMerchantErrorCodes.FrequencyLimit`，**不得纠正**） |
| 支 5 查询结算账户 | **100 次 / 秒** | 公共码 |
| 支 2 / 支 3 查申请状态 | 页面标注频率限制为 **HTTP 403**（与其它页的 429 不同，照原文） | `RATE_LIMITED`（`Apply4SubMerchantErrorCodes.RateLimited`） |

重入语义：同一 `business_code` 在旧单仍「流程进行中」时重复提交返回 `400 PROCESSING`
⇒ 这是**重入而非幂等**，改资料重试须先撤单或换编号。

本域专有码常量类 `Apply4SubMerchantErrorCodes`（`A4S-B5` 锁值）共 5 项：
`APPLYMENT_NOT_EXIST` / `PROCESSING` / `ORDER_NOT_EXIST` / `FREQENCY_LIMIT` / `RATE_LIMITED`；
其余（`PARAM_ERROR` / `INVALID_REQUEST` / `SIGN_ERROR` / `NO_AUTH` / `SYSTEM_ERROR`）走公共
`WechatPayErrorCodes`。

## 条件必填与主体类型：类型上全可空，官方语义上按主体分叉

`Apply4SubSubjectTypes`（`A4S-B5`，5 值）与支 1 的注释给出的条件必填：

| 主体类型 | 必填块 |
|---|---|
| 个体工商户 / 企业 | `business_license_info` |
| 政府机关 / 事业单位 / 社会组织 | `certificate_info` |
| 金融机构 | `finance_institution_info`（`Apply4SubFinanceTypes`） |
| 企业 / 社会组织且受益人不止经营者一人 | `ubo_info_list`（**最多 4 人**） |

SDK **不做**任何本地拦截或条件必填校验（类型一律可空）⇒ 缺件由官方以 `PARAM_ERROR` 表达。
`sales_scenes_type`（`Apply4SubSalesScenesTypes`，6 值）、`contact_id_doc_type`
（`Apply4SubIdDocTypes`，8 值）、证件类 `Apply4SubCertificateTypes`（10 值）、
`id_holder_type`（`Apply4SubIdHolderTypes`）同为字符串常量集，逐值由 `A4S-B5` 锁定。

## 媒体字段是「先上传、再引用」

进件里所有照片 / PDF / 视频字段填的都是「图片上传 API」或「视频上传 API」返回的 **MediaID**；
那两个上传端点属 P0-7 商户经营批次（`MerchantMedia` 域，只有 multipart 手工通道），
**本域不重复声明** ⇒ 本域 6 支里没有任何上传端点，`A4S-B1` 的路由名册也没有它们。
开户意愿视频（`Apply4SubAdditionInfo.LegalPersonVideo`）须用**视频**上传 API，不是图片上传 API。

## 序列化面

本域 DTO 全部标 `[HttpJsonSerializable(SerializerClassName = "Apply4SubMerchant")]`，
生成上下文落盘见
[../../../Mud.Wechat.Pay/DataModels/Generated/Apply4SubMerchantJsonContext.g.cs](../../DataModels/Generated/Apply4SubMerchantJsonContext.g.cs)，
由 [../../Extensions/PayJsonResolverExtensions.cs](../../Extensions/PayJsonResolverExtensions.cs) 登记。
守卫 `A4S-B7` 是本域计数与接线的权威：DTO **28** 个（申请单族 4 + 主体族 8 + 联系人 1 +
经营信息族 11 + 结算账户族 4）、逐个登记进 `Apply4SubMerchantJsonContext`、
`SerializerClassName` 必须为 `Apply4SubMerchant`。
`A4S-B3` 锁定的官方字段名（含**不得纠正**的拼写）样本：`business_code`、`bank_account_info`、
`applyment_state`、`applyment_state_msg`、`sign_url`、`sub_mchid`、`field_name`、`subject_type`、
`ubo_info_list`、`id_holder_type`、`contact_id_doc_type`、`sales_scenes_type`、`merchant_shortname`、
`settlement_id`、`bank_account_type`、`application_no`、`verify_finish_time`。
时间字段恒 `string`；示例里**不得**引用生成的 `Apply4SubMerchantJsonContext`（`internal`）。

## 快速开始

```csharp
// 1) 凭据基座：进件族须用「服务商自有商户号」的普通商户形态配置。
//    配置只放**密钥名**，真实 APIv3 key / 私钥 / 证书内容 / serial_number 只走环境变量或本机 User Secrets。
builder.Services.AddPayApp(builder.Configuration, "WechatPayMerchants");
// 2) 按模块装配本域
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.Apply4SubMerchant));

var svc = scope.ServiceProvider.GetRequiredService<IWechatPayApply4SubMerchantService>();

// 支 1：提交申请单（敏感字段须宿主先用微信支付公钥加密，这里只给占位假值）
var submitted = await svc.SubmitApplymentAsync(new Apply4SubApplymentRequest
{
    BusinessCode = "SP-20261018-0001",                  // 商户侧唯一编号；重入时同编号返 400 PROCESSING
    ContactInfo = new Apply4SubContactInfo(),           // 联系人：姓名/手机号等字段须密文
    SubjectInfo = new Apply4SubSubjectInfo
    {
        SubjectType = Apply4SubSubjectTypes.Enterprise,  // 取值只走常量类，不写裸字面量
    },
    BusinessInfo = new Apply4SubBusinessInfo(),
    SettlementInfo = new Apply4SubSettlementInfo(),
    BankAccountInfo = new Apply4SubBankAccountInfo
    {
        BankAccountType = Apply4SubBankAccountTypes.Corporate,  // BANK_ACCOUNT_TYPE_*，勿与 ACCOUNT_TYPE_* 混用
        AccountName = "<密文：开户名称>",                        // 占位符：真实值不落日志
        AccountBank = "占位银行名",
        AccountNumber = "<密文：银行账号>",
    },
}, ct);
WechatPayException.ThrowIfFailed(submitted, "/v3/applyment4sub/applyment/");
long? applymentId = submitted.ApplymentId;               // 只是受理号，不代表入驻完成

// 支 3：用商户侧编号查状态（applymentId 走 path，故以字符串传入）
var state = await svc.QueryApplymentStateByBusinessCodeAsync("SP-20261018-0001", ct);
try
{
    WechatPayException.ThrowIfFailed(state, "/v3/applyment4sub/applyment/business_code/{business_code}");
}
catch (WechatPayException ex) when (ex.PayErrorCode == Apply4SubMerchantErrorCodes.ApplymentNotExist)
{
    // 400 APPLYMENT_NOT_EXIST：编号写错或尚未落单
}

if (state.ApplymentState == Apply4SubApplymentStates.ToBeSigned
    || state.ApplymentState == Apply4SubApplymentStates.ToBeConfirmed)
{
    string? humanStepUrl = state.SignUrl;   // 交超级管理员人工完成，SDK 不代跑
}

List<Apply4SubAuditDetail> rejects = new List<Apply4SubAuditDetail>();
foreach (Apply4SubAuditDetail d in state.AuditDetail ?? new List<Apply4SubAuditDetail>())
{
    rejects.Add(d);   // 驳回明细：field_name + reject_reason，按字段定位改资料
}

// 支 5：查结算账户（掩码回读；显式指定展示规则）
var settlement = await svc.QuerySettlementAsync(
    state.SubMchId!, Apply4SubAccountNumberRules.MaskV2, ct);
WechatPayException.ThrowIfFailed(settlement, "/v3/apply4sub/sub_merchants/{sub_mchid}/settlement");
string? maskedAccount = settlement.AccountNumber;   // 恒为掩码，永不明文

// 支 6：查修改申请状态（VERIFY_* 与 AUDIT_* 是两套值，本支读的是 AUDIT_*）
var application = await svc.QuerySettlementModifyApplicationAsync(
    state.SubMchId!, "AP20261018000001", null, ct);
string? auditResult = application.VerifyResult;      // Apply4SubSettlementAuditResults
string? finishedTime = application.VerifyFinishTime; // 只有本类型有此字段（A4S-B6）
```

`[Path]` / `[Query]` 参数是本域方法的**位置参数**，`[Path]` 恒为 `string`（官方 `applyment_id`
声明为 Long、`account_number_rule` 为枚举串，均在序列化时按字符串替换进路径 / query）。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| `account_number_rule` 默认值的可传字符串 | 支 5 / 支 6 注释写「不传默认 `MASK_V1`」，常量类 `A4S-B5` 锁定的值是 `ACCOUNT_NUMBER_RULE_MASK_V1` ⇒ **注释与常量口径不一致** | 复核官方原文后统一；在此之前本文只推荐显式传常量 |
| 进件申请单请求体的全量必填面 | 注释只给出「按主体类型的条件必填」四条 ⇒ 其余块的必填口径**未逐项核验** | 不得从条件必填外推；需要时逐页补 remarks |
| 支 1 的图片 / 视频上传 API 契约 | 注释只给官方页号（`4012760490` / `4012761084`），端点属 `MerchantMedia` 手工通道，本域不声明 | 上传面查 `MerchantMedia`，勿在本域加路由 |
| 三支端点级 QPS 之外的其它限流面 | 只有注释里印出的 20/分钟、每日 5 次、100/秒三处 ⇒ 其它页**未逐项核验** | 记为未核验，不补第三方数值 |
| 超级管理员账户验证 / 签约的官方后续动作细节 | 注释只说明「须人工用 `sign_url` 完成」 | 人工链路属宿主，本文不外推 |
| 驳回后「同 `business_code` 覆盖重提」的状态前提 | 注释给出 `PROCESSING` 表示旧单进行中，但哪些 `applyment_state` 允许覆盖**未逐项核验** | 需要精确重入矩阵时先取证 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（异步审核、加密口径、条件必填、限频与码表） | `IWechatPayApply4SubMerchantService.cs` 类级与各方法 XML 注释 |
| 6 条路由名册（计数权威之一） | `../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs` 的 `PAY-B5` |
| 逐支路由 / path 与 query 位置 / 「申请单族无 subMchId」 | `../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayApply4SubMerchantContractGuards.cs` 的 `A4S-B1` |
| 数量 6 + 两族前缀不得交叉 + 结尾斜杠 | 同文件 `A4S-B2` |
| 官方字段名锁定（含不得纠正的拼写） | 同文件 `A4S-B3` |
| 两组同名枚举不得合并 | 同文件 `A4S-B4` |
| 状态与枚举取值、`account_number_rule`、专有错误码 | 同文件 `A4S-B5` |
| 应答继承与「支 5 无 verify_finish_time」 | 同文件 `A4S-B6` |
| DTO 数 28 + JsonContext 与 `SerializerClassName` 登记 | 同文件 `A4S-B7` |
| 跨域总端点数与接口数 | `../../../../../Tests/ContractBaseline.cs`（`Baseline.Pay.Endpoints` / `Baseline.Pay.HttpApiInterfaces`） |
| DTO 字段名与类型 | `../../../Mud.Wechat.Pay/DataModels/Apply4SubMerchant/` |
| 模块枚举与注册接线 | `../../Extensions/PayModule.cs`、`../../Extensions/PayServiceBuilder.cs`、`../../Extensions/PayJsonResolverExtensions.cs` |
| 支付线总体口径 | `../../README.md` |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方路由与限制，**同批**改 `PAY-B5` 的本域路由名册、`A4S-B2` 的
   `HaveCount(6)` 与 `../../../../../Tests/ContractBaseline.cs` 的 `Baseline.Pay.Endpoints`，再改本文速览表。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `Apply4SubMerchant`）→ 跑
   `../../../../../scripts/AddHttpJsonSerializable.ps1` + `../../../../../scripts/GenerateJsonContext.ps1`
   → 更新 `A4S-B7` 的 **28** 计数。
3. 改枚举 / 常量 → `A4S-B4`（分域）与 `A4S-B5`（逐值）会直接红，那是守卫在阻止「合并同名枚举」
   与「纠正官方拼写」，不是守卫过期。
4. 改模块面 → `../../Extensions/PayModule.cs` 的 `Apply4SubMerchant` 成员与
   `../../Extensions/PayServiceBuilder.cs` 的 `AddApply4SubMerchantWebApiHttpClient()` 须同批。
5. **不要**给本域任何端点加 `[Token]`（`PAY-B1` fail-closed）；**不要**把 `sub_mchid` 做成商户作用域；
   **不要**删 `/v3/applyment4sub/applyment/` 的结尾斜杠；**不要**把 query `account_number_rule`
   下沉进 body 或 path；**不要**在 DTO 里放 `DateTime`；**不要**把明文银行卡号 / 户名写进日志或异常。
6. 回调类资源通知（若有）的验签与解密属 [../../../Mud.Wechat.Pay.Callback/README.md](../../../Mud.Wechat.Pay.Callback/README.md)，
   不在本域加断言。最后跑 `../../../../../scripts/verify-build.ps1` 与
   `../../../../../scripts/audit-config-keys.ps1`，全绿才算完成。
