# AbnormalFundProcessing 域 · 在途异常资金处理（付款单查询 / 付款指令发起 / 指令执行结果查询）

微信支付 APIv3「在途异常资金处理」**3 支声明式端点**（付款单查询 + 付款指令发起 + 指令执行结果查询）。
接口 [IWechatPayAbnormalFundProcessingService.cs](./IWechatPayAbnormalFundProcessingService.cs)，
注册组 `AbnormalFundProcessing`，模块枚举 `PayModule.AbnormalFundProcessing`
（Pay 线**没有** `AddAbnormalFundProcessingApi()` 这种公有方法，装配入口是
`AddWechatPayApi(b => b.AddModules(PayModule.AbnormalFundProcessing))`，内部调用源生成的
`AddAbnormalFundProcessingWebApiHttpClient()`，见 [../../Extensions/PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、
> 本域契约守卫（[WechatPayAbnormalFundProcessingContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayAbnormalFundProcessingContractGuards.cs)
> 与全局 [WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)）、
> 本域 DTO（[../../../Mud.Wechat.Pay/DataModels/AbnormalFundProcessing/](../../DataModels/AbnormalFundProcessing)）。
> 不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

计数权威是**本域守卫** `AFP-B1`
（`AbnormalFundProcessingTopology_ShouldBeOneInterfaceOnOneGroup`：方法数 `HaveCount(3)`、
`(动词, 路由)` 去重后也是 `HaveCount(3)`、动词集恒为 `Get` / `Post`、前缀 `/v3/abnormal-fund-processing`），
路由与 path / query 名册由 `AFP-B2` 逐支锁定。
**本域不在全局 `PAY-B5` 的路由名册里**（`PAY-B5` 只逐域列出部分域的路由），
本域接口只出现在全局 `PAY-B1` 的接口名册中（计数取 `Baseline.Pay.HttpApiInterfaces`）——
⇒ 改本域路由不会让 `PAY-B5` 变红，只会让 `AFP-B1/B2` 变红。

| # | 方法 | HTTP 动词 | 路由 | 认证/签名方式 | 备注 |
|---|---|---|---|---|---|
| 1 | `QueryAbnormalFundReceiptAsync` | GET | `/v3/abnormal-fund-processing/receipts/{receiptId}` | 商户证书 / APIv3 请求签名 | path `receipt_id`（string(64) 必填），零 query |
| 2 | `CreateAbnormalFundTransferInstructionAsync` | POST | `/v3/abnormal-fund-processing/receipts/{receiptId}/transfer-instructions` | 同上 | `[Body]` 请求体 + path `receipt_id`；应答 `instruction` 为**指令对象** |
| 3 | `QueryAbnormalFundTransferInstructionAsync` | GET | `/v3/abnormal-fund-processing/receipts/{receiptId}/transfer-instructions/{outInstructionNo}` | 同上 | **双 path**（`receipt_id` + `out_instruction_no`），零 query |

三支全部是「一支路由一方法」：接口 remarks 自述本域每支路由官方**只印一页**
⇒ 不存在同一路径的【普通商户】/【普通服务商】双页比对问题。

## 认证：本线没有 access_token

APIv3 全程用**商户 API 证书**做 `WECHATPAY2-SHA256-RSA2048` 请求签名，签名在传输层
`WechatPayAuthorizationHandler` 完成，端点方法只管路由与报文。因此：

- 本接口**不声明 `[Token]`**，守卫 `PAY-B1` 对整条 Pay 线 fail-closed（全包零 `[Token]`）；
  本域三个方法同样不带令牌声明（接口 remarks 原话：「无 `[Token]`、走商户签名（守卫 PAY-B1 形态）」）。
- 走命名客户端 `wechat-pay`（`WechatPayHttpClientNames.TypeName`），BaseAddress 恒为
  `https://api.mch.weixin.qq.com`；不复用默认 `IEnhancedHttpClient`（会与企微线撞 BaseUrl）。
- `[AllowAnyStatusCode]` 是本接口必需项：APIv3 业务失败是 HTTP 4xx/5xx + `{"code","message"}`，
  不加则组件在非 2xx 直接抛 `ApiException`、把官方业务码丢掉。
- 商户作用域经 `IWechatPayMerchantContext.UseMerchant(...)` 选择；真实密钥只走环境变量或本机
  User Secrets，配置里只放**密钥名**。

## 商户身份：守卫与接口注释口径冲突（未裁决，勿据此写代码）

同一件事在本仓有两处相反记载，本文如实并列、不选边：

| 出处 | 记载 |
|---|---|
| 守卫 `AFP-B1` 类 remarks | 「本域路由前缀为 `/v3/abnormal-fund-processing/`，全部是【普通商户】面」；逐端点注释把三支的身份标为普通商户 |
| 守卫 `AFP-B1` 正对照 | 用 `IWechatPayMerchantFundService`（商户资金域，2 支 GET）作同口径正对照 |
| 接口文件 XML | 三支「支持商户」均写【平台商户】　【普通服务商】，官方页 URL 挂在 `/doc/v3/partner/` 下 |
| 路由形态 | 路由**不含** `/partner/` 或 `sp_` / `sub_` 段，报文里也无服务商两层标识 |

⇒ 身份口径影响的是「该用哪种商户配置形态调用」，代码层面可确认的只有：报文零 `sub_mchid`、
签名主体即配置里的 `AuthorizationMchId`。进正式结论前须逐页复核，见文末缺口表。

## 三支应答不同形 ⇒ 三个响应类型（合并即抹平官方差异）

`AFP-B3`（`AbnormalFundProcessingDtoFields_ShouldCoverOfficialFieldTables`）锁的是 DTO 属性名与官方
字段逐字对应（含 `receipt_id` / `receipt_state` / `out_instruction_no` / `transfer_mode` /
`instruction_state`）。三支应答的**结构确实不同**：

| 类型 | 顶层形态 | 独有内容 |
|---|---|---|
| `AbnormalFundProcessingReceiptResponse` | 单据视图：`product_name` / `receipt_id` / `transfer_amount` / `receipt_state` / `create_time` / `last_update_time` / `instruction` | 指令以**嵌套子对象** `instruction` 出现 |
| `AbnormalFundProcessingInstructionCreateResponse` | 受理视图：与上表**同名的七节** + `instruction` 子对象 | 语义是「指令已受理」，不是「已入账」 |
| `AbnormalFundProcessingInstructionResponse` | 指令视图：`out_instruction_no` / `commander` / `transfer_mode` / `receiver` / `instruction_state` / `create_time` / `success_time` / `close_time` / `close_reason` / `notify_url` / `instruction_instance_id` | **平铺**，且独有 `close_time` / `close_reason` / `instruction_instance_id` |

⇒ 单据的 `instruction.instruction_instance_id` 只在前两支出现，第三支才有 `close_time` /
`close_reason`；「按名字差不多就共用一个类型」在本域会直接掉字段。

## 金额单位与两层状态机

1. **金额一律是分**：`transfer_amount.total` 类型是 `long?`（JSON 名 `total`），`currency` 官方固定回
   `CNY`。DTO 里**没有**元/分换算 helper；1 元填 `100`。
2. **单据状态 `receipt_state`（3 值，字符串）**：`RECEIPT_STATE_PENDING` 待处理 /
   `RECEIPT_STATE_PROGRESS` 处理中 / `RECEIPT_STATE_COMPLETED` 处理成功。
3. **指令状态 `instruction_state`（4 值，字符串）**：`INSTRUCTION_STATE_PENDING` 待执行 /
   `INSTRUCTION_STATE_IN_PROGRESS` 执行中 / `INSTRUCTION_STATE_CLOSED` 已关闭 /
   `INSTRUCTION_STATE_SUCCESS` 执行成功。
4. **条件返回字段（据 DTO remarks 的「仅当」条款，判空是必需的）**：
   - `instruction` 整节仅在 `receipt_state` 为 `RECEIPT_STATE_PROGRESS` / `RECEIPT_STATE_COMPLETED`
     **且该付款由商户发起**时返回；
   - `instruction.receiver` 只在 `transfer_mode` 指向用户/商户入账且状态满足条件时返回；
   - `success_time`、`instruction_instance_id` 只在 `INSTRUCTION_STATE_SUCCESS` 时返回；
   - `close_time`、`close_reason` 只在 `INSTRUCTION_STATE_CLOSED` 时返回。
   ⇒ 全部字段在 C# 侧都是可空类型，**不得**假定存在。
5. `INSTRUCTION_STATE_CLOSED` 的官方处置动作写在注释里：**更换**商户侧指令编号后重新发起指令；
   其余状态不得换编号重发。

## `out_instruction_no`：作用域是「付款单下唯一」，不是全局唯一

`AbnormalFundProcessingInstructionCreateRequest.OutInstructionNo`（`out_instruction_no`，必填
string(64)）的唯一性口径写在 DTO 注释原文：「商户需保证**同一笔在途异常资金付款单下**唯一」。
配套的重入信号是支 2 的业务码 `400 ALREADY_EXISTS`「当前在途异常资金付款单已在处理中或处理成功」
⇒ 拿到该码的正确动作是**改查指令执行结果**（支 3），不是换编号再发一次。
本 SDK **不做**任何本地唯一性或重入拦截。

## 错误码：本域没有专有常量类

`Src/Pay/Mud.Wechat.Pay/DataModels/AbnormalFundProcessing/` 目录下只有 `AbnormalFundProcessingDtos.cs`
一个文件 ⇒ **本域不存在取值常量类，也不存在 `*ErrorCodes` 类**（`AFP-B1`~`AFP-B3` 三支守卫也
不涉及常量类）。接口注释里逐支摘录的业务码为：

| 支 | 注释里印出的码 |
|---|---|
| 支 1 查付款单 | `400 PARAM_ERROR` / `400 INVALID_REQUEST` / `401 SIGN_ERROR` / `500 SYSTEM_ERROR` / `429 FREQUENCY_LIMIT_EXCEED` / `404 NOT_FOUND` |
| 支 2 发起指令 | 同上六项（`NOT_FOUND` 换成 `400 ALREADY_EXISTS`） |
| 支 3 查指令结果 | 同支 1，末项语义为「在途异常资金付款单指令不存在」 |

判错只有一条路：`WechatPayResponse` 上没有 HTTP 状态码属性，成功判定看 `IsSuccess`（`Code` 为空），
分类判定看 `WechatPayException.PayErrorCode` 与**字符串字面量**比对。
其中 `PARAM_ERROR` / `INVALID_REQUEST` / `SIGN_ERROR` / `SYSTEM_ERROR` / `NO_AUTH` 在公共
`WechatPayErrorCodes` 里有常量；`ALREADY_EXISTS`、`NOT_FOUND`、`FREQUENCY_LIMIT_EXCEED`
**没有**对应常量（公共类里只有 `RESOURCE_ALREADY_EXISTS` / `RESOURCE_NOT_EXISTS`，
是品牌域用的另一组码）⇒ 本域判错须按原样字符串比较，不得「就近借用」公共常量。

## 频率限制：三支官方页全部未印数字

接口 remarks 的取证结论：三支页面均**未印**端点级 QPS，只在错误码表给出
`429 RATELIMIT_EXCEEDED` / `FREQUENCY_LIMITED`（注释另在逐支码表里写作
`FREQUENCY_LIMIT_EXCEED`）⇒ 不得凭第三方 SDK 补数，本文也不给数值。

## 零 query、零列表支

三支全部以 path 定位（`receipt_id`，支 3 再加 `out_instruction_no`），没有任何 `[Query]` 参数，
也没有列表端点 ⇒ 他域的 `offset` / `limit` / `need_query_detail` 模板不得搬进来
（`AFP-B2` 对每支的 query 名册断言为空集）。

## 通知地址与回调面的分工

`notify_url`（支 2 请求体必填，string(255)；在应答里回显）官方约束写在 DTO 注释：只支持 HTTPS、
须外网可访问、**不能携带查询参数**。付款结果的**回调报文的验签与 `resource` 解密不在本域**，
由 [../../../Mud.Wechat.Pay.Callback/README.md](../../../Mud.Wechat.Pay.Callback/README.md) 承载；
本域接口文件不含任何回调契约 ⇒ 不要在本域加回调断言。

## 敏感数据与日志红线

`receiver.openid`（string(128)）、`receiver.appid`、`commander.mchid`、`instruction_instance_id`
等标识类字段**不得**写日志、遥测或异常消息；本文与示例里一律用占位符。
本域三支的请求头表在注释里**未列** `Wechatpay-Serial` ⇒ 按注释口径本域无「密文上送」契约面，
SDK 侧也不做加解密。APIv3 密钥、商户私钥、证书内容、真实 `serial_number` 一律不得出现在代码或文档里。

## 序列化面

本域 DTO 全部标 `[HttpJsonSerializable(SerializerClassName = "AbnormalFundProcessing")]`，
生成上下文落盘见
[../../../Mud.Wechat.Pay/DataModels/Generated/AbnormalFundProcessingJsonContext.g.cs](../../DataModels/Generated/AbnormalFundProcessingJsonContext.g.cs)，
由 [../../Extensions/PayJsonResolverExtensions.cs](../../Extensions/PayJsonResolverExtensions.cs)
登记（`AbnormalFundProcessingJsonContext.Default`）供 Native AOT 使用。
JSON 名一律 snake_case 原样；时间字段（`create_time` / `last_update_time` / `success_time` /
`close_time`）恒为 `string`（rfc3339 文本），**不进** `DateTime` / `DateTimeOffset`。
示例里**不得**引用生成的 `AbnormalFundProcessingJsonContext`（它是 `internal`）。

**注意**：与 `BS-B7` / `BT-B7` / `A4S-B7` 等域不同，本域守卫**没有** DTO 计数与 AOT/DI 接线断言
（只有 `AFP-B1`~`AFP-B3`）⇒ DTO 数量没有守卫权威，本文不给数。

## 快速开始

```csharp
// 1) 凭据基座（商户配置：mchid / 证书序列号 / 私钥与 APIv3 key 的**密钥名**；
//    真实值只走环境变量或本机 User Secrets，绝不下沉到本文或代码库）
builder.Services.AddPayApp(builder.Configuration, "WechatPayMerchants");
// 2) 按模块装配本域
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.AbnormalFundProcessing));

// 取服务
var svc = scope.ServiceProvider.GetRequiredService<IWechatPayAbnormalFundProcessingService>();

// 支 1：查付款单。金额单位是分；状态是字符串，本域无常量类
var receipt = await svc.QueryAbnormalFundReceiptAsync("4200000000202610180000000001", ct);
WechatPayException.ThrowIfFailed(receipt, "/v3/abnormal-fund-processing/receipts/{receipt_id}");

List<string> noteLines = new List<string>();
long? totalInFen = receipt.TransferAmount?.Total;         // null 即官方未回
noteLines.Add($"receipt_state={receipt.ReceiptState} total={totalInFen}");

// 支 2：发起付款指令。out_instruction_no 只需在「本付款单下」唯一
var created = await svc.CreateAbnormalFundTransferInstructionAsync(
    receipt.ReceiptId!,
    new AbnormalFundProcessingInstructionCreateRequest
    {
        OutInstructionNo = "AFP202610180001",             // 商户侧编号，付款单内唯一
        TransferMode = "TRANSFER_TO_ORIGINAL_RECEIVE_USER",
        NotifyUrl = "https://example.com/pay/abnormal-fund/notify",   // HTTPS、无查询串
        Receiver = new AbnormalFundProcessingInstructionCreateRequestReceiver
        {
            Appid = "wx0000000000000000",                 // 占位符：须与入账用户所属 AppID 一致
            Openid = "o_c_0000000000000000000000000",     // 占位符：不得进日志/遥测
        },
    },
    ct);

try
{
    WechatPayException.ThrowIfFailed(created,
        "/v3/abnormal-fund-processing/receipts/{receipt_id}/transfer-instructions");
}
catch (WechatPayException ex) when (ex.PayErrorCode == "ALREADY_EXISTS")
{
    // 受理重入：该单已在处理中或已处理成功 ⇒ 去查执行结果，不要换编号重发
}

// 支 3：查指令执行结果（双 path）
var instruction = await svc.QueryAbnormalFundTransferInstructionAsync(
    receipt.ReceiptId!, "AFP202610180001", ct);
WechatPayException.ThrowIfFailed(instruction,
    "/v3/abnormal-fund-processing/receipts/{receipt_id}/transfer-instructions/{out_instruction_no}");

string? state = instruction.InstructionState;    // INSTRUCTION_STATE_PENDING / _IN_PROGRESS / _CLOSED / _SUCCESS
string? closedReason = instruction.CloseReason;  // 仅 _CLOSED 时非 null
```

支 2 的形参是「`[Path] receiptId` + `[Body] request` + 取消令牌」三个，支 1 / 支 3 只有 `[Path]`
参数与取消令牌；`[Path]` 参数恒为 `string`，占位符名用 C# 参数名（`{receiptId}`）而非官方 snake_case。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 商户身份口径 | 守卫 `AFP-B1` remarks 写「全部是【普通商户】面」，接口 XML 写【平台商户】　【普通服务商】且官方页在 `/doc/v3/partner/` 树 ⇒ **两处冲突，未裁决** | 逐页复核后同批改守卫 remarks 与接口 XML，再回来定稿本文「商户身份」一节 |
| 支 3 的官方页号 | 接口 XML 记 `4016636882`，守卫 `AFP-B2` 的该支注释记 `partner/4015408022`（与支 2 同号）⇒ **两处不一致** | 以复核结果统一，不得在本文另立第三个页号 |
| 「指令发起支官方未印业务错误码表」 | 接口**类级** remarks 如此自述，但支 2 的**方法级** remarks 印了六条码（含 `ALREADY_EXISTS`）⇒ 自相矛盾 | 本文按方法级 remarks 列表行文；类级 remarks 的结论须修正后再引用 |
| 三支端点级 QPS 数字 | 官方页**未印**（接口 remarks 的取证结论）⇒ **未逐项核验**，不补第三方数值 | 记为「未印」，需要限流数值时先取证 |
| 本域 DTO 数量与 AOT 接线 | 守卫只到 `AFP-B3`，**无** DTO 计数、无 `PayJsonResolverExtensions` / `PayModule` / 注册委托接线断言 ⇒ 无守卫权威数值 | 本文不给 DTO 数；补守卫时按 `BS-B7` / `BT-B7` 口径同批加 |
| `receipt_state` / `instruction_state` 的常量类 | 本域目录下无常量文件 ⇒ 取值只能按字符串比对 | 若后续建常量类，须同批改本文与守卫 |
| 嵌套 `instruction` 与平铺指令的字段差异是否穷尽 | `AFP-B3` 只**抽查**关键 JSON 名（`receipt_id` / `receipt_state` / `out_instruction_no` / `transfer_mode` / `instruction_state`），**未逐项核验**全字段名册 | 需要全表比对时先补守卫断言 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（金额单位、必填口径、错误码、频率限制） | `IWechatPayAbnormalFundProcessingService.cs` 类级与各方法 XML 注释 |
| 本域 3 方法 / 3 条 (动词, 路由) / 前缀（计数权威） | `../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayAbnormalFundProcessingContractGuards.cs` 的 `AFP-B1` |
| 路由、path 键、query 空集 | 同文件 `AFP-B2` |
| DTO 官方字段名抽查 | 同文件 `AFP-B3` |
| 零 `[Token]`、`[AllowAnyStatusCode]`、接口名册（`Baseline.Pay.HttpApiInterfaces`） | `../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs` 的 `PAY-B1` |
| 跨域总端点数 | `../../../../../Tests/ContractBaseline.cs`（`Baseline.Pay.Endpoints`） |
| DTO 字段名与类型 | `../../../Mud.Wechat.Pay/DataModels/AbnormalFundProcessing/` |
| AOT 上下文与模块接线 | `../../../Mud.Wechat.Pay/DataModels/Generated/AbnormalFundProcessingJsonContext.g.cs`、`../../Extensions/PayJsonResolverExtensions.cs`、`../../Extensions/PayModule.cs`、`../../Extensions/PayServiceBuilder.cs` |
| 回调报文验签与解密 | `../../../Mud.Wechat.Pay.Callback/README.md` |
| 支付线总体口径 | `../../README.md` |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方路由与限制，**同批**改 `AFP-B1` 的方法数 / (动词, 路由) 断言与
   `AFP-B2` 的路由名册，再改 `../../../../../Tests/ContractBaseline.cs` 的 `Baseline.Pay.Endpoints`
   （本域路由不在 `PAY-B5` 名册内，但计入跨域总数），最后改本文速览表。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `AbnormalFundProcessing`）→ 跑
   `../../../../../scripts/AddHttpJsonSerializable.ps1` + `../../../../../scripts/GenerateJsonContext.ps1`
   → 确认 `AbnormalFundProcessingJsonContext.Default` 仍在 `../../Extensions/PayJsonResolverExtensions.cs` 里。
3. 改模块面 → `../../Extensions/PayModule.cs` 的 `AbnormalFundProcessing` 成员与
   `../../Extensions/PayServiceBuilder.cs` 的 `AddAbnormalFundProcessingWebApiHttpClient()` 挂接须同批。
4. **不要**给本域任何端点加 `[Token]`（`PAY-B1` fail-closed）；**不要**把 path 键改成 query
   （`AFP-B2` 断言 query 名册为空）；**不要**在 DTO 里放 `DateTime`；**不要**把三支应答合并成一型。
5. 本文是域文档，改动须与接口注释、守卫同一批提交；最后跑 `../../../../../scripts/verify-build.ps1`
   与 `../../../../../scripts/audit-config-keys.ps1`，全绿才算完成。
