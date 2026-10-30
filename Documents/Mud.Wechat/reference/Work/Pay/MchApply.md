# 对外收款账户进件 · 申请单 / 状态 / 图片（仅自建应用）

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Pay` 域 · 创建对外收款账户族（进件） |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [MchApply/IWechatWorkPayMchApplyService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/MchApply/IWechatWorkPayMchApplyService.cs)（父，**零端点**，`IsAbstract = true` 不可注入）· [MchApply/IWechatWorkInternalPayMchApplyService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/MchApply/IWechatWorkInternalPayMchApplyService.cs)（唯一子接口，承载 3 支端点） |
| DTO 源文件 | [DataModels/Pay/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Pay/)（全域 60 型、`SerializerClassName = Pay` → `PayJsonContext`；本族 16 型：`ApplyPayMchRequest` / `ApplyPayMchResponse` / `GetPayApplymentStatusRequest` / `GetPayApplymentStatusResponse` / `PayApplymentStatus` / `PayApplymentAuditDetail` / `PayAccountValidation` / `UploadPayImageResponse` / `PayBusinessLicenseInfo` / `PayFinanceInstitutionInfo` / `PayIdCardInfo` / `PayContactInfo` / `PayAccountInfo` / `PayBankCardSupplement` / `PaySalesSceneInfo` / `PayPicIdList`） |
| 契约守卫 | [WechatPayContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatPayContractGuards.cs)（PAY1d：本族 3 路由 + **E3 必须以 `[MultipartForm]` 承载** · PAY2b：父零端点 + 仅自建单子接口 · PAY3：16 接口 `AccessToken` + Query + `access_token` · PAY4：全域 60 型，本族 16 型）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 提交申请单 [98973](https://developer.work.weixin.qq.com/document/path/98973) · 查询申请单状态 [98974](https://developer.work.weixin.qq.com/document/path/98974) · 提交图片 [98972](https://developer.work.weixin.qq.com/document/path/98972)（三页均仅自建应用） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为 PAY1d/PAY2b/PAY3/PAY4 + 接口 XML 注释 + 16 型 DTO 逐项比对；E3 的运行期缺陷另据 `Mud.HttpUtils.Generator` 3.0.9 生成体实测复现，见 §5；域 README 记录逐端点核验日期为 2026-10-18） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 21 篇的可运行示例合并逐字编译，0 错误（本篇 1 段（提交进件 → 轮询申请单状态；图片上传段仅写签名与路由，生成器缺陷见 §5））；本批 21 段、全仓累计 171 段；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：PAY1d/PAY2b/PAY3/PAY4 断言 + 本族 2 份接口文件 XML 注释 + 16 型 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/README.md)（端点速览第 4~6 行、域级契约）· 官方：本批未重新渲染取文 |

## 1. 这个接口族做什么

**把一家「还没收款能力」的企业，通过递交材料申请出一个对外收款账户**——本族 3 支端点覆盖「先传图 → 再递材料 → 后查进度」的完整进件链路：

| # | 方法 | 路由 | 说明 |
|---|---|---|---|
| E1 | `ApplyMchAsync` | `POST /cgi-bin/miniapppay/apply_mch` | 提交创建对外收款账户的申请单（主体 / 证照 / 管理员 / 结算账户 / 经营场景） |
| E2 | `GetApplymentStatusAsync` | `POST /cgi-bin/miniapppay/get_applyment_status` | 按 `out_request_no` 查申请单状态（申请 / 签约 / 驳回 / 验证链接） |
| E3 | `UploadImageAsync` | `POST /cgi-bin/miniapppay/upload_image` | 提交图片，换 `open_wx_pay_media_id` 供 E1 的图片类字段使用 |

**族形态是「零端点父接口 + 仅自建单子接口」**：官方**仅向企业自建应用**开放，代开发与第三方**均暂不支持** ⇒ 父接口 `IWechatWorkPayMchApplyService` 不含端点、也不设第三方 / 代开发子接口（PAY2b 锁定）。

> **E3 当前在本 SDK 不可用（运行期缺陷）**：`Mud.HttpUtils.Generator` **3.0.9** 对 `[MultipartForm] IFormContent` 发射的请求体不含文件字节 ⇒ E3 无法真正上传图片。**E1 / E2 不受影响**。详见 §5 首行；这是上游生成器缺陷，本仓七线全部 `[MultipartForm] IFormContent` 端点同病，不要在宿主侧手写 multipart 绕过。

同域近邻：[Bill.md](./Bill.md)（对外收款记录，三类应用公共面）· [FundFlow.md](./FundFlow.md) · [Merchant.md](./Merchant.md)（商户号查询 / 使用范围）· [Order.md](./Order.md) · [Refund.md](./Refund.md) · [TradeBill.md](./TradeBill.md)。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal`（仅自建） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌注入 | `TokenInjectionMode.Query`（MUD005 已知接受风险：官方契约强制 Query，无法改 Header） | 每次请求（E3 亦然：令牌在 Query、文件在 body） |
| 令牌归属键 | `Wechat.AccessToken@Internal`（`WechatTokenManagerKeys.InternalAccessToken`） | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

### 2.2 权限与业务前置

- **应用须配置到「对外收款 - 可调用接口的应用」**。
- **申请单按应用隔离**：提交与查询**均只针对该应用本身提交的申请单**；换应用查不到。
- **图片按应用隔离**：E1 里所有图片类字段只能填**本应用**经 E3 拿到的 `open_wx_pay_media_id`。
- **图片 ID 生命周期**：上传后 **30 天**过期；被用于成功提交的申请单**全部走完流程**后立即失效 ⇒ 不能在库里长期缓存复用。
- **图片规格**：文件 **5B ~ 2MB**，仅 JPG/JPEG、PNG、BMP。
- **配额**：同一提现人员最多 **200** 个商户号，同一企业最多 **2000** 个商户号。
- **提现人约束**：`userid` 须实名认证、与超级管理员为**同一人**、且在应用可见范围内。
- **`out_request_no` 是幂等键**：1~32 字符，E2 用它查询 ⇒ 重试 E1 应沿用同一编号。
- **个人数据**：法定代表人与管理员姓名 / 证件号 / 手机号 / 邮箱 / 银行账号 / 结算卡号均属个人或敏感信息，示例一律假值且**不得**落日志。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1 / E2 / E3 | ✅ | ❌ | ❌ |

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
| 可注入接口 | `IWechatWorkInternalPayMchApplyService`（命名空间 `Mud.Wechat.Work`） |
| 不可注入 | `IWechatWorkPayMchApplyService`（父，零端点，命名空间 `Mud.Wechat.Work.Interfaces`） |

## 4. 端点契约明细

> **本族必填性不用类型表达**：进件材料字段官方几乎全为必填，但 DTO 一律可空（`string?` / `int?`），SDK 不做本地校验，缺项由官方在响应里报。唯一的本地 fail-fast 在 E3（见该节）。

### E1. `ApplyMchAsync` — 提交创建对外收款账户的申请单

**签名**（声明于唯一自建子接口）

```csharp
[Post("/cgi-bin/miniapppay/apply_mch")]
Task<ApplyPayMchResponse> ApplyMchAsync(
    [Body] ApplyPayMchRequest request,
    CancellationToken cancellationToken = default);
```

**HTTP 与凭据**

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/miniapppay/apply_mch` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/miniapppay/apply_mch?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入，应用自身令牌） |
| 适用应用类型 | 仅自建 |
| 请求体 | `ApplyPayMchRequest` |

**请求体** `ApplyPayMchRequest`（顶层字段）：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `out_request_no` | `OutRequestNo` | `string?` | 是 | 业务申请编号（1~32 字符），**E2 用它查询**；重试请沿用 |
| `organization_type` | `OrganizationType` | `int?` | 是 | `0` 企业 / `1` 个体 / `2` 社会团体组织 / `3` 事业单位 |
| `business_license_info` | `BusinessLicenseInfo` | `PayBusinessLicenseInfo?` | 是 | 营业执照 / 登记证书 |
| `finance_institution_info` | `FinanceInstitutionInfo` | `PayFinanceInstitutionInfo?` | 否 | 金融机构许可证（主体为金融机构时必填） |
| `merchant_short_name` | `MerchantShortName` | `string?` | 是 | 商户简称（约 ≤21 汉字），展示在支付完成页 |
| `id_card_info` | `IdCardInfo` | `PayIdCardInfo?` | 是 | 经营者 / 法人证件 |
| `owner` | `Owner` | `bool?` | 否 | 法人是否受益人；**主体为企业时必填** |
| `ubo_info` | `UboInfo` | `PayIdCardInfo?` | 否 | 受益人证件；仅企业且法人非受益人时填 |
| `contact_info` | `ContactInfo` | `PayContactInfo?` | 是 | 超级管理员 |
| `account_info` | `AccountInfo` | `PayAccountInfo?` | 是 | 结算账户 |
| `sales_scene_info` | `SalesSceneInfo` | `PaySalesSceneInfo?` | 是 | 经营场景证明 |
| `business_id` | `BusinessId` | `int?` | 是 | 经营范围（如 `1` 餐饮 / `2` 零售 / `28` 培训机构，取值以官方为准） |
| `qualifications` | `Qualifications` | `PayPicIdList?` | 否 | 特殊资质图片 ID（最多 5 张） |
| `business_addition_pics` | `BusinessAdditionPics` | `PayPicIdList?` | 否 | 补充材料图片 ID（最多 5 张） |
| `userid` | `UserId` | `string?` | 是 | 提现人员 userid（实名、与超管同一人、在可见范围内） |

**嵌套结构**：

| 类型 | 关键字段 |
|---|---|
| `PayBusinessLicenseInfo` | `cert_type`（仅社会团体）/ `business_license_copy_open_wx_pay_media_id`（证照扫描件 ID，必填）/ `business_license_number`（个体 15 或 18 位、企业仅 18 位）/ `merchant_name`（2~128 字符）/ `legal_person`（2~100 字符）/ `company_address` / `business_time_begin_time` / `business_time_end_time`（`YYYY-MM-DD`，长期填「长期」） |
| `PayFinanceInstitutionInfo` | `finance_type`（`BANK_AGENT` / `PAYMENT_AGENT` / `INSURANCE` / `TRADE_AND_SETTLE` / `OTHER`）/ `finance_license_pics_open_wx_pay_media_id`（≤5 张） |
| `PayIdCardInfo` | `id_card_copy_open_wx_pay_media_id`（人像面，必填）/ `id_card_national_open_wx_pay_media_id`（国徽面，护照免）/ `id_card_name` / `id_card_number` / `id_card_address`（企业主体必填；超管处不填）/ `id_card_valid_time_begin` / `id_card_valid_time`（长期填「长期」）/ `id_doc_type`（`0`/`8` 身份证、`1` 护照、`2` 香港通行证、`3` 澳门通行证、`4` 台湾通行证、`5` 外国人居留证、`6` 港澳居民证、`7` 台湾居民证） |
| `PayContactInfo` | `contact_type`（字符串枚举 `"65"` 法人 / `"66"` 经办人）/ `contact_info`（**属性名 `ContactDetail`**，结构同 `PayIdCardInfo` 但不填居住地址）/ `business_authorization_letter_open_wx_pay_media_id`（经办人时需上传）/ `mobile_phone` / `contact_email` |
| `PayAccountInfo` | `bank_account_type`（`74` 对公 / `75` 对私；个体必填）/ `account_bank` / `account_name` / `bank_address_code` / `bank_name`（非直连银行必填）/ `account_number` / `bank_card_supplement` |
| `PayBankCardSupplement` | `settlement_certificate_open_wx_pay_media_id` / `relationship_certificate_open_wx_pay_media_id` / `other_certificate_open_wx_pay_media_id`（≤3 张） |
| `PaySalesSceneInfo` | `type`（`1` 线下门店 / `2` 线上店铺）/ `store_url`（线上，≤1024 字节）/ `store_pic_open_wx_pay_media_id`（线上，与 `store_url` 至少一项）/ `address_code`（线下，6 位）/ `offline_address`（线下，4~512 字符）/ `entrance_pic_open_wx_pay_media_id`（线下门头照）/ `indoor_pic_open_wx_pay_media_id`（线下内景照） |
| `PayPicIdList` | `id`（`List<string>`；官方 JSON 形状为 `{"id": ["..."]}`） |

**返回** `ApplyPayMchResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `errcode` / `errmsg` | 继承自 `WechatWorkResponse` | — | 统一信封；须 `ThrowIfFailed` |
| `error_description` | `ErrorDescription` | `string?` | 参数错误时的具体错误信息（官方在参数错误时返回） |

### E2. `GetApplymentStatusAsync` — 查询申请单状态

**签名**

```csharp
[Post("/cgi-bin/miniapppay/get_applyment_status")]
Task<GetPayApplymentStatusResponse> GetApplymentStatusAsync(
    [Body] GetPayApplymentStatusRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/miniapppay/get_applyment_status` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/miniapppay/get_applyment_status?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入） |
| 适用应用类型 | 仅自建 |

**请求体** `GetPayApplymentStatusRequest`：

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `out_request_no` | `OutRequestNo` | `string?` | 是 | 提交 E1 时填写的业务申请编号（1~32 字符） |

**返回** `GetPayApplymentStatusResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `status` | `Status` | `PayApplymentStatus?` | 申请单具体状态（嵌套结构见下） |
| `apply_state` | `ApplyState` | `int?` | 申请阶段：`0` 初始 / `1` 申请中 / `2` 绑定成功 / `3` 已撤销 / `4` 绑定失败 / `5` 未申请 / `6` 待法人验证 |
| `real_sign_state` | `RealSignState` | `int?` | 签约阶段：`0` 不可签约 / `1` 未签约 / `2` 已签约 |
| `reject_reason` | `RejectReason` | `string?` | 驳回理由 |

`PayApplymentStatus`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `applyment_state` | `ApplymentState` | `string?` | `CHECKING` 资料校验中 / `NEED_SIGN` 待签约 / `ACCOUNT_NEED_VERIFY` 待账户验证 / `FINISH` 完成 / `AUDITING` 审核中 / `FROZEN` 已冻结 / `REJECTED` 已驳回 / `CANCELED` 已作废 |
| `applyment_state_desc` | `ApplymentStateDesc` | `string?` | 状态描述 |
| `sign_state` | `SignState` | `string?` | `UNSIGNED` 未签约 / `SIGNED` 已签约 / `NOT_SIGNABLE` 不可签约（一般处于已驳回、已冻结、机器校验中） |
| `sign_url` | `SignUrl` | `string?` | 签约链接（`NEED_SIGN` 或 `UNSIGNED` 时返回，**永久有效**，须超级管理员用已实名微信扫码） |
| `sub_mchid` | `SubMchid` | `string?` | 电商平台二级商户号（`NEED_SIGN` 或 `FINISH` 时返回） |
| `audit_detail` | `AuditDetail` | `List<PayApplymentAuditDetail>?` | 驳回原因详情（`REJECTED` / `FROZEN` 时返回）；元素 `{param_name, reject_reason}` |
| `account_validation` | `AccountValidation` | `PayAccountValidation?` | 汇款账户验证信息（签约阶段 2 且状态 `ACCOUNT_NEED_VERIFY` 时返回） |
| `legal_validation_url` | `LegalValidationUrl` | `string?` | 法人验证链接（建议转二维码给法人扫码；法人证件与营业执照匹配时返回） |

`PayAccountValidation`：`account_name` / `account_no`（对私时返回）/ `pay_amount`（汇款金额，分）/ `destination_account_number` / `destination_account_name` / `destination_account_bank` / `city` / `remark` / `deadline`。

### E3. `UploadImageAsync` — 提交图片（⚠ 当前生成器缺陷，见 §5）

**签名**

```csharp
[Post("/cgi-bin/miniapppay/upload_image")]
Task<UploadPayImageResponse> UploadImageAsync(
    [MultipartForm] IFormContent formData,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法 / 路由 | `POST` `/cgi-bin/miniapppay/upload_image` |
| 完整 URL 形态 | `https://qyapi.weixin.qq.com/cgi-bin/miniapppay/upload_image?access_token=ACCESS_TOKEN` |
| 凭据参数 | `access_token`（Query 注入） |
| 请求体形态 | `multipart/form-data`（`[MultipartForm]`，参数类型 `IFormContent`） |
| 适用应用类型 | 仅自建 |

**参数**

| 参数 | 位置 | 类型 | 说明 |
|---|---|---|---|
| `formData` | Form | `IFormContent` | 由调用方构建的 multipart 表单内容（含 filename / filelength / content-type 信息） |

**返回** `UploadPayImageResponse`：

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `open_wx_pay_media_id` | `OpenWxPayMediaId` | `string?` | 图片 ID（**30 天**过期；所有使用它的申请单走完流程后立即失效） |

> **不要按本节签名直接上线**：生成器 3.0.9 下该端点不会把文件字节发出去（§5 首行给出复现结论）。在缺陷修复前，E1 的图片类字段只能使用**其他途径**（例如先前已上传入库）取得的 `open_wx_pay_media_id`，且仍须满足「本应用上传」这一官方约束。

### 端到端调用示例（E1 + E2；图片 ID 来自外部）

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
var mchApply = provider.GetRequiredService<IWechatWorkInternalPayMchApplyService>();

// ④ 图片 ID 只能来自本应用的 upload_image（E3 现受生成器缺陷影响，此处假定已另行取得）
string licensePic = config["Pay:MediaId:BusinessLicense"] ?? string.Empty;

// ⑤ 提交申请单：out_request_no 是幂等键，重试必须沿用
const string requestNo = "apply-demo-0001";
var applied = await mchApply.ApplyMchAsync(new ApplyPayMchRequest
{
    OutRequestNo = requestNo,
    OrganizationType = 0,
    MerchantShortName = "示例商户",
    BusinessLicenseInfo = new PayBusinessLicenseInfo
    {
        BusinessLicenseCopyOpenWxPayMediaId = licensePic,
        BusinessLicenseNumber = "913000000000000000",
        MerchantName = "示例科技有限公司",
        LegalPerson = "张三",
    },
    IdCardInfo = new PayIdCardInfo
    {
        IdCardName = "张三",
        IdCardNumber = "110101199001010000",
        IdCardValidTimeBegin = "2020-01-01",
        IdCardValidTime = "长期",
        IdDocType = 0,
    },
    Owner = true,
    ContactInfo = new PayContactInfo
    {
        ContactType = "65",
        ContactDetail = new PayIdCardInfo
        {
            IdCardName = "张三",
            IdCardNumber = "110101199001010000",
            IdCardValidTimeBegin = "2020-01-01",
            IdCardValidTime = "长期",
        },
        MobilePhone = "13800000000",
        ContactEmail = "admin@example.com",
    },
    AccountInfo = new PayAccountInfo
    {
        BankAccountType = 74,
        AccountBank = "中国银行",
        AccountName = "示例科技有限公司",
        BankAddressCode = "110000",
        AccountNumber = "0000000000000000",
    },
    SalesSceneInfo = new PaySalesSceneInfo
    {
        Type = 2,
        StoreUrl = "https://example.com",
    },
    BusinessId = 2,
    UserId = "zhangsan",
}, cancellationToken);
WechatWorkException.ThrowIfFailed(applied, "/cgi-bin/miniapppay/apply_mch");
if (!string.IsNullOrEmpty(applied.ErrorDescription))
{
    // 参数错误详情：只落策略性摘要，不落证件号/卡号
    logger.LogWarning("进件参数错误：{Reason}", applied.ErrorDescription);
    return;
}

// ⑥ 查进度：applyment_state 驱动分支；签名/法人验证链接交人工处理
var status = await mchApply.GetApplymentStatusAsync(
    new GetPayApplymentStatusRequest { OutRequestNo = requestNo }, cancellationToken);
WechatWorkException.ThrowIfFailed(status, "/cgi-bin/miniapppay/get_applyment_status");

switch (status.Status?.ApplymentState)
{
    case "FINISH":
        logger.LogInformation("进件完成，二级商户号已下发");
        break;
    case "NEED_SIGN":
    case "ACCOUNT_NEED_VERIFY":
        // sign_url / legal_validation_url 属一次性引导链接，不写日志
        logger.LogInformation("进件待签约或待账户验证，状态描述：{Desc}", status.Status.ApplymentStateDesc);
        break;
    default:
        logger.LogInformation("进件状态 {State}（阶段 {Stage}）",
            status.Status?.ApplymentState, status.ApplyState);
        break;
}
```

## 5. 陷阱与易混对照

| 陷阱 | 口径 | 错误做法的后果 |
|---|---|---|
| **E3 上传腿（现行缺陷，最重要）** | `Mud.HttpUtils.Generator` **3.0.9** 对 `[MultipartForm] IFormContent` 发射的是<br>`var __formDataContent = new StringContent(formData.ToString() ?? "");` + `__multipartContent.Add(__formDataContent, "formData");`<br>⇒ **`ToHttpContent()` / `ToHttpContentAsync()` 从不被调用**，表单里只有一个名为 `formData` 的字符串部分，**文件字节不上 wire** | 宿主无论如何实现 `IFormContent` 都传不上图；官方侧表现为参数/文件缺失类错误。**这是上游生成器缺陷，七线全部 `[MultipartForm] IFormContent` 端点同病**；不要在宿主侧手写 multipart 绕过（会掩盖缺陷且在升级后重复） |
| 接口注释口径 vs 发射形态 | E3 的 XML 注释称「`[MultipartForm]` 才走 `IFormContent.ToHttpContentAsync` 通路」——这是 3.0.1 期为绕 `CS1061` 留下的**编译期**结论，与 3.0.9 的**运行期**发射不符 ⇒ 注释该句**不成立** | 按注释信任通路 → 误判为宿主实现写错 |
| `[FormContent]` vs `[MultipartForm]` | 前者在生成器 3.0.1 下发射 `GetFormDataContentAsync`（运行时接口无此成员，`CS1061`）；本仓统一用后者（PAY1d 断言 E3 必须标 `[MultipartForm]`） | 改成 `[Body]`（JSON 通路）→ 守卫打红且官方直接拒 |
| `Mud.HttpUtils.DefaultFormContent` | 组件里唯一现成的 `IFormContent` 实现是 `DefaultFormContent(Dictionary<string,string>)`，产出 `application/x-www-form-urlencoded` ⇒ **不是**带文件的 multipart | 用它「凑一个实现」→ 官方直接拒 |
| **父接口零端点** | 3 支全在 `IWechatWorkInternalPayMchApplyService` | 去父接口找端点 → 找不到；搬进父接口 → PAY2b 打红 |
| **仅自建** | 第三方 / 代开发无子接口 | 找 `ThirdParty` / `Provider` 子接口 → 类型不存在 |
| **`out_request_no` 是幂等键** | E1 提交 + E2 查询共用；1~32 字符 | 每次重试换新编号 → 产生重复申请单，且查旧编号查不到 |
| 申请单与图片**按应用隔离** | 只认本应用提交的申请单、本应用上传的图片 | 跨应用复用 `open_wx_pay_media_id` → 官方拒 |
| **图片 ID 会过期** | 30 天，或申请单全部走完流程后立即失效 | 长期缓存复用 → 提交时图片无效 |
| 图片规格 | **5B ~ 2MB**，JPG/JPEG / PNG / BMP | 传 2.1MB 或 WEBP → 官方拒 |
| `PayContactInfo.ContactDetail` | 属性名与 JSON 字段名 `contact_info` **不一致**（避免与顶层 `ApplyPayMchRequest.ContactInfo` 重名） | 按字段名猜属性名 → 编译不过 |
| `contact_type` 是**字符串**枚举 | `"65"` 法人 / `"66"` 经办人（不是 `int`） | 传 `65` 数字 → 反序列化/提交口径不一致 |
| 超管为法人时只需姓名 + 证件号 | 与法人身份证一致；经办人才必填证件类型 | 法人场景硬塞全部字段 → 冗余且易与工商信息冲突 |
| `business_license_number` 位数分主体 | 个体 15 或 18 位；企业仅 18 位 | 企业填 15 位 → 官方拒 |
| 日期格式是字符串 | `YYYY-MM-DD`；长期有效填「长期」 | 传时间戳 → 官方拒 |
| 两个 `contacts` 层级 | 顶层 `contact_info`（超管）/ 嵌套 `contact_info`（证件）——后者的 C# 属性叫 `ContactDetail` | 把证件塞进顶层超管对象 → 结构错 |
| `apply_state` vs `applyment_state` | 前者 `int` 阶段（0~6），后者 `string` 状态机 | 用 `int` 比较字符串状态 → 分支永不命中 |
| `real_sign_state` vs `sign_state` | 前者 `int` 签约阶段（0~2），后者 `string` 签约状态 | 混用 → 分支错 |
| **验证链接是一次性引导** | `sign_url` 永久有效但须超管扫码；`legal_validation_url` 建议转二维码 | 写日志/遥测/异常消息 → 敏感链接外泄 |
| `sub_mchid` 何时有值 | 仅 `NEED_SIGN` / `FINISH` | 其他状态当空值就是异常 → 误报 |
| 配额 | 提现人 ≤200 商户号、企业 ≤2000 | 批量进件不预检 → 中途被拒 |

SDK 侧通用约束：**不自动判错**（须 `WechatWorkException.ThrowIfFailed`）、**不本地校验进件材料**（除 E3 的 `formData` 空值 fail-fast）、**不重试申请单**（幂等键由宿主维护）。

## 6. 失败处理与错误码

| 码 | 常量 / 来源 | 含义 | 处置 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | — |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合（基座自动恢复，调用方不手写重试） |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

E1 的参数级错误还附带 `error_description`：**先 `ThrowIfFailed` 再读 `ErrorDescription`**（errcode 非 0 时该字段才有意义）。具体业务码全集、驳回码表**接口注释未逐条留档** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| E3 运行期可用性 | 受上游 `Mud.HttpUtils.Generator` 3.0.9 缺陷阻塞；上游 HEAD（3.1.0 未发布）同病 | NuGet 上游发版说明 + 本仓 `.docs` 侧 DM-14 取证 |
| 业务错误码 / 驳回码全集 | 本族注释只留 `error_description` 与驳回原因字段，未摘码表 | 官方 98973 / 98974 + 全局错误码页 |
| `business_id` 取值范围 | 注释只举 `1` / `2` / `28`，完整对照表未落仓 | 官方 98973 |
| `organization_type` 与可选材料矩阵 | 哪些主体需要 `finance_institution_info` / `bank_card_supplement` 的长表未落仓 | 官方 98973 |
| 频率限制 | 本族注释未给独立频率 | 官方各页与频率附录 |
| 官方页正文 | 本批未重新渲染取文 | 官方 98973 / 98974 / 98972 |
| 真账号运行验证 | 本批仅编译级验证；进件涉及真实企业材料，Sample 未跑 | 见本仓 `.docs` 侧验证记录 |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Pay/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Pay/README.md)（15 支全表、令牌矩阵、DTO 清单）
- 同域近邻：[Merchant.md](./Merchant.md)（进件成功后查商户号与使用范围）· [Bill.md](./Bill.md) · [FundFlow.md](./FundFlow.md) · [Order.md](./Order.md) · [Refund.md](./Refund.md) · [TradeBill.md](./TradeBill.md)
- 同形态上传缺陷记录：[ExternalContact/Attachment.md](../ExternalContact/Attachment.md) §5（95098 上传附件资源，同一根因）
- 官方页：[提交申请单（98973）](https://developer.work.weixin.qq.com/document/path/98973) · [查询申请单状态（98974）](https://developer.work.weixin.qq.com/document/path/98974) · [提交图片（98972）](https://developer.work.weixin.qq.com/document/path/98972)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
