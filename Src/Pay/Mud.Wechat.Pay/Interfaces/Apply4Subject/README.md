# Apply4Subject 域 · 主体进件 / 商户开户意愿确认（服务商为公司主体提交进件申请）

微信支付 APIv3「主体进件（商户开户意愿确认）」**5 支声明式端点**（申请单族 4：提交申请单 +
query 定位查审核结果 + 按业务编号 / 按申请单号撤销；商户状态族 1：获取商户开户意愿确认状态）。
接口 [IWechatPayApply4SubjectService.cs](./IWechatPayApply4SubjectService.cs)，
注册组 `Apply4Subject`，模块枚举 `PayModule.Apply4Subject`
（Pay 线**没有** `AddApply4SubjectApi()` 这种公有方法，装配入口是
`AddWechatPayApi(b => b.AddModules(PayModule.Apply4Subject))`，内部调用源生成的
`AddApply4SubjectWebApiHttpClient()`，见 [../../Extensions/PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、
> 本域契约守卫（[WechatPayApply4SubjectContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayApply4SubjectContractGuards.cs)
> 与全局 [WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、
> 本域 DTO（[../../../Mud.Wechat.Pay/Apply4Subject/](../../../Mud.Wechat.Pay/Apply4Subject/)）。
> 不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

计数权威有两处，须同时成立：**全局 `PAY-B5`** 的本域路由名册（`RoutesOf(asm,
"IWechatPayApply4SubjectService")` 逐条 `BeEquivalentTo`，5 条）锁路由集合，
**本域 `A4SJ-B2`** 的 `routes.Should().HaveCount(5, …)` 锁数量（断言消息原文要求
「数量变化须同批更新 A4SJ-B1 与 Tests/ContractBaseline.cs」）；`A4SJ-B1` 逐支锁方法、路由原文与
path / query 位置不可互换。

| # | 方法 | HTTP 动词 | 路由 | 认证/签名方式 | 备注 |
|---|---|---|---|---|---|
| 1 | `SubmitApplymentAsync` | POST | `/v3/apply4subject/applyment/` | 商户证书 / APIv3 请求签名（敏感字段另需 `Wechatpay-Serial`） | **结尾斜杠是官方原文，不可删**；应答只有 `applyment_id` |
| 2 | `QueryApplymentStateAsync` | GET | `/v3/apply4subject/applyment` | 同上 | **同资源无尾斜杠**；定位键走 query `applyment_id` / `business_code`，均选填、至少传一个 |
| 3 | `CancelApplymentByBusinessCodeAsync` | POST | `/v3/apply4subject/applyment/{businessCode}/cancel` | 同上 | 无请求体，成功 `204` ⇒ 返回类型是裸 `Task` |
| 4 | `CancelApplymentByApplymentIdAsync` | POST | `/v3/apply4subject/applyment/{applymentId}/cancel` | 同上 | 与支 3 是同页两个 path 变体，**不得合并成一个方法** |
| 5 | `QueryMerchantAuthorizeStateAsync` | GET | `/v3/apply4subject/applyment/merchants/{subMchId}/state` | 同上 | 本域唯一以商户号定位的端点；应答只有 `authorize_state` |

## 认证：本线没有 access_token

APIv3 全程用**商户 API 证书**做 `WECHATPAY2-SHA256-RSA2048` 请求签名，签名在传输层
`WechatPayAuthorizationHandler` 完成。本域额外有**平台公钥面**：联系人 / 法定代表人 / 受益人的
姓名、证件号、手机号、证件居住地址须用**微信支付公钥**（推荐）或平台证书公钥以 RSAES-OAEP 加密上送，
请求头 `Wechatpay-Serial` 指向对应公钥 ID / 证书序列号（由传输层按商户配置写入，声明式签名不带该参数）。
因此：

- 本接口**不声明 `[Token]`**，整条 Pay 线由守卫 `PAY-B1` fail-closed。
- 走命名客户端 `wechat-pay`（`WechatPayHttpClientNames.TypeName`），BaseAddress 恒为
  `https://api.mch.weixin.qq.com`。
- `[AllowAnyStatusCode]` 必需：业务失败是 HTTP 4xx/5xx + `{"code","message"}`，落到
  `WechatPayResponse.Code` / `Message`，由 `WechatPayException.ThrowIfFailed` 判错。

## 支持的商户类型与本域身份

接口 remarks 原文：四页支持商户是**渠道商 + 从业机构（银行）+ 从业机构（支付机构）**，
**不含「普通服务商」** —— 与本线另一进件域 `IWechatPayApply4SubMerchantService`（普通服务商）不同。
文档面因此落在服务商文档中心（`/doc/v3/partner/`）而非直连接口。

## 该用哪种商户配置形态（本域最容易踩空的一处）

- 签名主体是**服务商 / 从业机构自己**的商户号：`Authorization` 头的 `mchid` 由
  `WechatPayMerchantConfig.AuthorizationMchId` 给出。
- 服务商形态（`SpMchId` + `SubMchId`）由 `Validate()` 强制两者**成对**，而进件阶段被进件主体还不存在
  ⇒ 本域端点须以**服务商自有商户号注册的普通商户形态**进入调用作用域。
- 支 5 的 `sub_mchid` 是**调用期 path 参数**，**不得**改从 `WechatPayMerchantConfig.SubMchId` 取。
- 请求体里的 `channel_id`（从业机构下的渠道商户号）同样是**调用期字段**，不是商户作用域配置：
  从业机构调用时必填、渠道商户自身调用时不填（`Apply4SubjectApplymentRequest.ChannelId` 的原文口径）。
- 多商户切换经 `IWechatPayMerchantContext.UseMerchant(...)`。

## 与特约商户进件（`apply4sub`）是两套互不兼容契约

本域最大的踩空点是「形似神异」，四处差异任一被顺手对齐即破坏契约（守卫 `A4SJ-B1` / `A4SJ-B2` /
`A4SJ-B3` / `A4SJ-B4` / `A4SJ-B6` / `A4SJ-B7` 逐处锁定）：

| 维度 | 本域 `apply4subject` | 特约商户进件 `apply4sub` |
|---|---|---|
| 资源前缀 | `/v3/apply4subject/applyment`（单一前缀） | 申请单 `/v3/applyment4sub/applyment` + 结算账户 `/v3/apply4sub/sub_merchants` |
| 查询定位 | **单端点 + query**（`applyment_id` / `business_code`） | **两个 path 端点** |
| 状态应答 | 只有 4 个字段 | 含 `sign_url` / `sub_mchid` / `applyment_state_msg` / `audit_detail[]` |
| 人工确认载体 | `qrcode_data`（**base64 图片数据**，最长 30000 字符） | `sign_url`（给人打开的链接） |
| 驳回粒度 | `reject_param` + `reject_reason`，一次**只指一个字段** | `audit_detail[]` 数组多项 |
| 营业执照块 | `business_licence_info`（英式） | `business_license_info`（美式） |
| 是否金融机构 | `is_finance_institution` | `finance_institution` |
| 单位证明函 | `company_prove_copy` | `certificate_letter_copy` |
| 顶层结构 | `identification_info` / `ubo_info_list` 为**顶层** | 同义信息嵌在 `subject_info.identity_info` |

`A4SJ-B2` 另有交叉污染正对照：本域路由**不得**出现 `apply4sub/` 或 `applyment4sub` 字面；
两域的类型不共用、不互转。

## 同资源异形斜杠：删或加斜杠在这里是真实契约变更

`A4SJ-B2` 同时断言 `/v3/apply4subject/applyment/`（POST，带尾斜杠）与
`/v3/apply4subject/applyment`（GET，无尾斜杠）两支都在名册里，并显式
`.NotEndWith("/")` 锁查询支 —— 「统一两个写法」会把其中一支改成官方不存在的资源。
两个撤销 path 变体的段字面**完全相同**（都是单段 + `/cancel`），故填错定位键不会报「参数类型不符」，
只会得到「查无此单」语义的 `INVALID_REQUEST` ⇒ 靠方法名与占位符参数名（`{businessCode}` /
`{applymentId}`，生成器 `HTTPCLIENT013` 强制用 C# 参数名）区分，`A4SJ-B1` 分别锁定。

## 异步状态机：`applyment_state` 八值 + 两处扫码环节

`Apply4SubjectApplymentStates`（`A4SJ-B5` 逐值锁定，官方拼写**不得纠正**：`EDITTING` 双 T、
`WAITTING` 双 T、`FREEZED` 非 `FROZEN`）：

| 取值 | 语义 | 下一步由谁做 |
|---|---|---|
| `APPLYMENT_STATE_EDITTING` | 官方语义为「提交申请发生错误导致」，非用户可停留态 | 商户重新提交 |
| `APPLYMENT_STATE_WAITTING_FOR_AUDIT` | 审核中 | 等待 |
| `APPLYMENT_STATE_WAITTING_FOR_CONFIRM_CONTACT` | 待确认联系信息 | 商家**扫 `qrcode_data`** |
| `APPLYMENT_STATE_WAITTING_FOR_CONFIRM_LEGALPERSON` | 待账户验证 | 商家**扫 `qrcode_data`** |
| `APPLYMENT_STATE_PASSED` | 审核通过，**仍需扫码确认** | 商家扫码；本态**不等于**入驻完成 |
| `APPLYMENT_STATE_REJECTED` | 审核驳回，读 `reject_param` + `reject_reason` | 商户改后重提 |
| `APPLYMENT_STATE_FREEZED` | 已冻结，原因同样落 `reject_param` / `reject_reason` | 按原因处置 |
| `APPLYMENT_STATE_CANCELED` | 已作废，终态 | 换新编号 |

`A4SJ-B4` 锁定的两域关系：8 值里**只与进件域重合 `EDITTING` / `REJECTED` / `CANCELED` 三值**，
本域无 `APPLYMENT_STATE_FINISHED`、进件域无 `APPLYMENT_STATE_PASSED`
⇒ **不存在可写的状态映射表**，两域流程判定必须各写各的。

判据只用 `applyment_state`；SDK 无法代为推进两个 `WAITTING_FOR_CONFIRM_*` 态。
`qrcode_data` 出现于 `WAITTING_FOR_CONFIRM_CONTACT` / `WAITTING_FOR_CONFIRM_LEGALPERSON` /
`PASSED` / `FREEZED` 四态，须 base64 解码成图片，**不是 URL、不是小程序 path**。

## 支 5 是另一个维度：`authorize_state` 只有 2 值

`Apply4SubjectAuthorizeStates`：`AUTHORIZE_STATE_UNAUTHORIZED` / `AUTHORIZE_STATE_AUTHORIZED`
（`A4SJ-B5`）。它回答「该**特约商户号**有没有完成实名认证」，与申请单审核进度是两个独立维度
⇒ **不得**用 `authorize_state` 判进件是否通过，也不得用支 2 的结果判授权。

## 错误码：本域**没有**专有码（与进件域相反）

`A4SJ-B5` 显式断言本命名空间下**不得存在任何 `*ErrorCodes` 类**，并拿进件域的
`Apply4SubMerchantErrorCodes`（确有 `APPLYMENT_NOT_EXIST`）作正对照。后果是判错面的实质差异：

- 四页业务错误码表只有公共码 `INVALID_REQUEST` / `NO_AUTH` / `SYSTEM_ERROR`
  （支 1 的 remarks 另印 `PARAM_ERROR` / `SIGN_ERROR`），全部走公共
  `WechatPayErrorCodes`。
- **不得**按 `APPLYMENT_NOT_EXIST` 判「单不存在」，也**没有** `PROCESSING` 判「同编号重入」
  ⇒ 重入与「查无此单」都只能落到 `INVALID_REQUEST` / `NO_AUTH`，需要凭 `business_code`
  换新编号重试。

## 条件必填与主体类型：类型上全可空，官方语义按主体分叉

`Apply4SubjectSubjectTypes`（`A4SJ-B4` 锁 6 值，比进件域**多** `SUBJECT_TYPE_MICRO`，
且事业单位是官方原文的 `SUBJECT_TYPE_INSTITUTIONS_CLONED`，本域**无** `SUBJECT_TYPE_INSTITUTIONS`）：

| 主体类型 | 必填块（支 1 remarks 原文） |
|---|---|
| 企业 / 个体工商户 | `subject_info.business_licence_info` |
| 政府机关 / 事业单位 / 社会组织 | `subject_info.certificate_info` |
| 小微商户 | `subject_info.assist_prove_info` |
| 金融机构（`is_finance_institution = true`） | `subject_info.finance_institution_info` |
| 政府机关 / 事业单位 | 另需 `subject_info.company_prove_copy` |

SDK **不做**任何本地拦截或条件必填校验（类型一律可空）⇒ 缺件由官方以 `PARAM_ERROR` 表达。
取值面同样由 `A4SJ-B5` 逐值锁定：证件 `Apply4SubjectIdDocTypes`（8 值）、
登记证书 `Apply4SubjectCertificateTypes`（10 值，且**证书类型与主体类型严格绑定**：
事业单位只有 `CERTIFICATE_TYPE_2388`、政府机关只有 `CERTIFICATE_TYPE_2389`）、
小微经营类型 `Apply4SubjectMicroBizTypes`（3 值）、金融机构类型 `Apply4SubjectFinanceTypes`（6 值）、
持有人类型 `Apply4SubjectHolderTypes`（`LEGAL` / `SUPER`，**裸值无前缀**，
`contact_type` 与 `id_holder_type` 共用）。

## 三处「有效日期」是**装着数组文本的字符串**，两处是起止双字段

- 区间字符串形态（官方示例值 `["2017-10-28","长期"]`，类型仍 `string`，做成 `List<string>` 会
  `400 PARAM_ERROR`）：`licence_valid_date`、`cert_valid_date`、`identification_valid_date`。
- 起止双字段形态：`contact_period_begin` / `contact_period_end`、`ubo_period_begin` / `ubo_period_end`
  （`YYYY-MM-DD`，长期填「长期」）。

⇒ 同域内两种形态并存，勿「顺手统一」。另有一处官方示例与字段表冲突：示例报文里有单数
`ubo_info`，字段表只有 `ubo_info_list`（且标「即将下线处理，无需传入」）⇒
**以字段表为权威**，`A4SJ-B7` 断言请求类型里**不得**出现 `ubo_info`。
`identification_info.owner` 与 `ubo_info_list` 同为官方标注即将下线的字段，照原文保留建模、新接入建议不填。

## 待确认商户号：创建即校验，报错文案不可用于筛选

`addition_info.confirm_mchid_list`（最多 20 个）填了就在**创建申请单时**校验三条硬约束
（商户号全称与主体名称一致、特殊行业商户的结算规则 ID 与经营许可证匹配、单申请单最多 20 个），
不满足即创建失败；且官方把多个商户号的失败原因拼在**同一条消息**里（「商户号:原因;商户号:原因」）
⇒ 剔除失败项须按标点切分，**不得按文案匹配**。

## 敏感字段与日志红线

- 须加密：`contact_info` 的 `name` / `mobile` / `id_card_number`（三者官方标 string(1024)「需加密」，
  长度上限是为密文预留的），`identification_info` 的 `identification_name` / `identification_number` /
  `identification_address`，`ubo_info_list[]` 的 `ubo_id_doc_name` / `ubo_id_doc_number` / `ubo_id_doc_address`。
- 不加密：所有照片类字段填图片上传 API 返回的 **MediaID**。本域 5 支里没有任何上传端点
  （上传面属 `MerchantMedia` 域的 multipart 手工通道），`A4SJ-B1` 名册亦无。
- 「条件必填」在 `identification_info` 特别绕：块内字段官方写「当证件持有人类型为法定代表人时填写」
  ⇒ `id_holder_type = SUPER`（经办人）时这些字段**整体留空**、改由 `contact_info` 承载经办人证件，
  而 `authorize_letter_copy`（法定代表人说明函）反而**必须**上传。
- 这些字段值**不得**写日志、遥测或异常消息；本文与示例一律用假值。真实密钥、证书、
  `serial_number` 只走环境变量或本机 User Secrets。

## 序列化面

本域 DTO 全部标 `[HttpJsonSerializable(SerializerClassName = "Apply4Subject")]`，
生成上下文落盘见
[../../../Mud.Wechat.Pay/Generated/Apply4SubjectJsonContext.g.cs](../../../Mud.Wechat.Pay/Generated/Apply4SubjectJsonContext.g.cs)，
由 [../../Extensions/PayJsonResolverExtensions.cs](../../Extensions/PayJsonResolverExtensions.cs) 登记。
守卫 `A4SJ-B7` 是本域计数与接线的权威：DTO **14** 个（申请单族 5 + 主体族 6 + 联系人 1 +
法定代表人 / 受益人 2）、逐个登记进 `Apply4SubjectJsonContext`、`SerializerClassName` 必须为
`Apply4Subject`。`A4SJ-B3` 锁定的官方字段名样本（含**不得纠正**的英式拼写）：`channel_id`、
`business_code`、`identification_info`、`ubo_info_list`、`applyment_id`、`is_finance_institution`、
`business_licence_info`、`company_prove_copy`、`assist_prove_info`、`licence_number`、
`licence_valid_date`、`contact_id_doc_type`、`contact_type`、`id_holder_type`、
`identification_front_copy`、`authorize_letter_copy`、`applyment_state`、`qrcode_data`、
`reject_param`、`reject_reason`、`authorize_state`、`confirm_mchid_list`、`operation_copy_list`、
`finance_license_pics`。
时间字段恒 `string`；示例里**不得**引用生成的 `Apply4SubjectJsonContext`（`internal`）。

## 快速开始

```csharp
// 1) 凭据基座：本域须用「服务商 / 从业机构自有商户号」的普通商户形态配置。
//    配置只放**密钥名**，真实 APIv3 key / 私钥 / 证书内容 / serial_number 只走环境变量或本机 User Secrets。
builder.Services.AddPayApp(builder.Configuration, "WechatPayMerchants");
// 2) 按模块装配本域
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.Apply4Subject));

var svc = scope.ServiceProvider.GetRequiredService<IWechatPayApply4SubjectService>();

// 支 1：提交主体进件申请单（敏感字段须宿主先用微信支付公钥加密，这里只给占位假值）
var submitted = await svc.SubmitApplymentAsync(new Apply4SubjectApplymentRequest
{
    ChannelId = "占位渠道商户号",                       // 从业机构调用时必填，是请求体字段而非商户作用域
    BusinessCode = "SUBJECT-20261018-0001",             // 数字/字母/下划线，每编号对应一个申请单
    ContactInfo = new Apply4SubjectContactInfo
    {
        ContactType = Apply4SubjectHolderTypes.Legal,   // LEGAL / SUPER 是裸值
        Name = "<密文：联系人姓名>",
        Mobile = "<密文：联系人手机号>",
    },
    SubjectInfo = new Apply4SubjectSubjectInfo
    {
        SubjectType = Apply4SubjectSubjectTypes.Enterprise,   // 本域取值，勿抄进件域
        IsFinanceInstitution = false,
        BusinessLicenceInfo = new Apply4SubjectBusinessLicenceInfo
        {
            LicenceNumber = "91110000MA00000000",       // 占位假值：企业须 18 位且以 9 开头
            LicenceCopy = "<MediaID>",
            MerchantName = "示例科技有限公司",
            LegalPerson = "示例姓名",
            CompanyAddress = "示例注册地址",
            LicenceValidDate = "[\"2017-10-28\",\"长期\"]",  // 官方形态：字符串内嵌数组文本
        },
    },
    IdentificationInfo = new Apply4SubjectIdentificationInfo
    {
        IdentificationType = Apply4SubjectIdDocTypes.IdCard,
        IdentificationName = "<密文：证件姓名>",
        IdentificationNumber = "<密文：证件号码>",
        IdentificationFrontCopy = "<MediaID>",
    },
    AdditionInfo = new Apply4SubjectAdditionInfo
    {
        ConfirmMchidList = new List<string> { "1900000001" },  // 最多 20 个，创建即校验
    },
}, ct);
WechatPayException.ThrowIfFailed(submitted, "/v3/apply4subject/applyment/");
long? applymentId = submitted.ApplymentId;   // 只有 1 个应答字段（A4SJ-B6），不代表入驻完成

// 支 2：query 定位（两键至少传一个，SDK 不做本地拦截）
var state = await svc.QueryApplymentStateAsync(null, "SUBJECT-20261018-0001", ct);
WechatPayException.ThrowIfFailed(state, "/v3/apply4subject/applyment");

// 本域**没有**专有错误码常量类（A4SJ-B5）⇒ 判「查无此单 / 重入」只能落公共码
if (state.Code == WechatPayErrorCodes.InvalidRequest)
{
    // INVALID_REQUEST：编号写错、尚未落单，或同编号重入
}

if (state.ApplymentState == Apply4SubjectApplymentStates.WaittingForConfirmContact
    || state.ApplymentState == Apply4SubjectApplymentStates.WaittingForConfirmLegalperson)
{
    // qrcode_data 是 base64 的小程序码图片数据，不是链接：解码后交商家扫码，SDK 无法代为推进
    byte[] qrcodeImage = Convert.FromBase64String(state.QrcodeData!);
}

string? rejectedField = state.RejectParam;    // 一次只指出一个字段 ⇒ 多字段驳回须按轮次逐项修
string? rejectedWhy = state.RejectReason;

// 支 3 / 支 4：撤销（同页两个定位键变体，204 无包体 ⇒ 返回裸 Task）
await svc.CancelApplymentByBusinessCodeAsync("SUBJECT-20261018-0001", ct);

// 支 5：查「该特约商户号的开户意愿确认状态」—— 与申请单状态是两个独立维度
var authorize = await svc.QueryMerchantAuthorizeStateAsync("1900000001", ct);
WechatPayException.ThrowIfFailed(authorize, "/v3/apply4subject/applyment/merchants/{sub_mchid}/state");
bool confirmed = authorize.AuthorizeState == Apply4SubjectAuthorizeStates.Authorized;
```

`[Path]` / `[Query]` 参数是本域方法的**位置参数**，`[Path]` 恒为 `string`
（官方 `applyment_id` 声明为 Integer，仍按字符串替换进路径 / query）。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 支 2 两键同传时的官方优先级 | 注释只写「官方未定义二者同传时的优先级，由调用方保证；官方 curl 示例即两键同传」 | **不得**在 SDK 侧加拦截或自定优先级；需要确切语义时先取证 |
| 提交申请单请求体的全量必填面 | 注释只给出「按主体类型的条件必填」五条 ⇒ 其余块与块内字段的必填口径**未逐项核验** | 不得从条件必填外推；需要时逐页补 remarks |
| 撤销后能否复用同一 `business_code` | 注释原文：官方未给错误码区分撤销与「同编号重提」的先后关系 | 稳妥做法是换新编号；本文不写「可复用」结论 |
| `PASSED` 之后商户号从何而来 | 本域状态应答**无** `sub_mchid` 字段（`A4SJ-B6` 禁列），支 5 又要求已有商户号 | 两态衔接属官方流程面，注释未给 ⇒ 未逐项核验 |
| 三支以上端点的 QPS / 并发限制 | 本域注释**未印任何限频数值**（与进件域有 20/分钟、每日 5 次、100/秒三处不同） | 记为未核验，不补第三方数值 |
| 图片上传 API 的契约 | 注释只说明「照片类字段填 MediaID」，端点属 `MerchantMedia` 手工通道 | 上传面查 `MerchantMedia`，勿在本域加路由 |
| 本域错误码表的完整码集 | 注释列出的公共码为 `INVALID_REQUEST` / `NO_AUTH` / `SYSTEM_ERROR`（支 1 另加 `PARAM_ERROR` / `SIGN_ERROR`）⇒ 各页码表是否一致**未逐项核验** | 判错只走 `WechatPayErrorCodes` 公共常量 |
| `confirm_mchid_list` 三条校验的失败码 | 注释只给出报错消息的**拼接格式**，未给 HTTP 状态与 `code` | 按标点切分商户号，不按文案匹配 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（异步受理、加密口径、条件必填、两域四处差异） | `IWechatPayApply4SubjectService.cs` 类级与各方法 XML 注释 |
| 5 条路由名册（计数权威之一） | `../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs` 的 `PAY-B5` |
| 逐支路由 / path 与 query 位置 / 「申请单族无 subMchId」/ 撤销无请求体 | `../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayApply4SubjectContractGuards.cs` 的 `A4SJ-B1` |
| 数量 5 + 单一前缀 + 尾斜杠异形 + 两个 cancel 变体 | 同文件 `A4SJ-B2` |
| 官方字段名锁定（英式 `licence` 等，不得纠正） | 同文件 `A4SJ-B3` |
| 状态与主体类型两域「同名不同值」不得合并 | 同文件 `A4SJ-B4` |
| 各枚举逐值 + 「本域无专有错误码类」断言 | 同文件 `A4SJ-B5` |
| 应答继承、撤销裸 `Task`、状态应答六禁列字段、提交应答仅 1 字段 | 同文件 `A4SJ-B6` |
| DTO 数 14 + JsonContext 与 `SerializerClassName` 登记 + 不建单数 `ubo_info` | 同文件 `A4SJ-B7` |
| 跨域总端点数与接口数 | `../../../../../Tests/ContractBaseline.cs`（`Baseline.Pay.Endpoints` / `Baseline.Pay.HttpApiInterfaces`） |
| DTO 字段名与类型 | `../../../Mud.Wechat.Pay/Apply4Subject/` |
| 模块枚举与注册接线 | `../../Extensions/PayModule.cs`、`../../Extensions/PayServiceBuilder.cs`、`../../Extensions/PayJsonResolverExtensions.cs` |
| 支付线总体口径 | `../../README.md` |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方路由与限制，**同批**改 `PAY-B5` 的本域路由名册、`A4SJ-B2` 的
   `HaveCount(5)` 与 `../../../../../Tests/ContractBaseline.cs` 的 `Baseline.Pay.Endpoints`，再改本文速览表。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `Apply4Subject`）→ 跑
   `../../../../../scripts/AddHttpJsonSerializable.ps1` + `../../../../../scripts/GenerateJsonContext.ps1`
   → 更新 `A4SJ-B7` 的 **14** 计数。
3. 改枚举 / 常量 → `A4SJ-B4`（两域重合面）与 `A4SJ-B5`（逐值）会直接红，那是守卫在阻止
   「合并 `applyment_state` / `subject_type`」与「纠正 `EDITTING` / `WAITTING` / `FREEZED` /
   `_CLONED` 拼写」，不是守卫过期。
4. 改模块面 → `../../Extensions/PayModule.cs` 的 `Apply4Subject` 成员与
   `../../Extensions/PayServiceBuilder.cs` 的 `AddApply4SubjectWebApiHttpClient()` 须同批。
5. **不要**给本域任何端点加 `[Token]`（`PAY-B1` fail-closed）；**不要**把 `sub_mchid` / `channel_id`
   做成商户作用域；**不要**删 POST 的尾斜杠或给 GET 加上；**不要**把 query 定位键下沉进 body 或
   改成 path 变体；**不要**给本域新建 `*ErrorCodes` 类（`A4SJ-B5` 禁列）；**不要**把三处区间字符串
   改成 `List<string>`；**不要**在 DTO 里放 `DateTime`；**不要**把明文姓名 / 证件号 / 手机号写进日志或异常。
6. 回调类资源通知（若有）的验签与解密属 [../../../Mud.Wechat.Pay.Callback/README.md](../../../Mud.Wechat.Pay.Callback/README.md)，
   不在本域加断言。最后跑 `../../../../../scripts/verify-build.ps1` 与
   `../../../../../scripts/audit-config-keys.ps1`，全绿才算完成。
