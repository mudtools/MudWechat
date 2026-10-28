# Transfer 域 · 商家转账（转账到零钱 / 电子回单 / 免确认收款授权）

微信支付 APIv3「商家转账」域：**10 支声明式端点**（发起 + 两支查单 + 两支查电子回单 + 撤销 +
两支申请电子回单 + 免确认授权发起 + 授权后转账）。
接口 [IWechatPayTransferService.cs](./IWechatPayTransferService.cs)，注册组 `Transfer`，
模块枚举 `PayModule.Transfer`（装配入口 `AddWechatPayApi(b => b.AddModules(PayModule.Transfer))`，
内部调用源生成的 `AddTransferWebApiHttpClient()`，见 [../../Extensions/PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本域契约守卫
> [WechatPayTransferContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayTransferContractGuards.cs)
> （`TR1`~`TR7`）+ 全局
> [WechatPayDomainContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayDomainContractGuards.cs)
> 与本域 DTO/常量（`../../../Mud.Wechat.Pay/Transfer/`）。
> 不复述外部官方文档正文；注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

计数权威是**全局**守卫 `PAY-B5` 的路由名册（`RoutesOf(asm, "IWechatPayTransferService")` 逐条
`BeEquivalentTo`，共 10 条）；本域守卫 `TR1`~`TR7` 锁路由/字段名/值表，**不**断言方法数。
十支均【普通商户】。

| # | 方法 | HTTP 动词 | 路由 | 认证/签名方式 | 备注 |
|---|---|---|---|---|---|
| 1 | `CreateTransferBillAsync` | POST | `/v3/fund-app/mch-transfer/transfer-bills` | 商户证书 / APIv3 请求签名 | 应答含 `state` 与 `package_info`（**非**成功凭据） |
| 2 | `QueryByOutBillNoAsync` | GET | `/v3/fund-app/mch-transfer/transfer-bills/out-bill-no/{outBillNo}` | 同上 | 红线配套支；仅 30 天内的单 |
| 3 | `QueryByTransferBillNoAsync` | GET | `/v3/fund-app/mch-transfer/transfer-bills/transfer-bill-no/{transferBillNo}` | 同上 | 与 2 应答同形 ⇒ 共用类型 |
| 4 | `QueryElecsignByOutBillNoAsync` | GET | `/v3/fund-app/mch-transfer/elecsign/out-bill-no/{outBillNo}` | 同上 | `download_url` 仅 10 分钟有效 |
| 5 | `QueryElecsignByTransferBillNoAsync` | GET | `/v3/fund-app/mch-transfer/elecsign/transfer-bill-no/{transferBillNo}` | 同上 | 与 4 共用 `TransferElecsignResponse` |
| 6 | `RevokeTransferAsync` | POST | `/v3/fund-app/mch-transfer/transfer-bills/out-bill-no/{outBillNo}/cancel` | 同上 | **无请求体**；`CANCELING` 非终态 |
| 7 | `ApplyElecsignByOutBillNoAsync` | POST | `/v3/fund-app/mch-transfer/elecsign/out-bill-no` | 同上 | 应答只有 `state` / `create_time`，与 8 共用 |
| 8 | `ApplyElecsignByTransferBillNoAsync` | POST | `/v3/fund-app/mch-transfer/elecsign/transfer-bill-no` | 同上 | 两段式的「申请」段 |
| 9 | `CreateTransferBillWithAuthorizationAsync` | POST | `/v3/fund-app/mch-transfer/transfer-bills/pre-transfer-with-authorization` | 同上 | 发起转账 + 完成免确认收款授权 |
| 10 | `TransferByAuthorizationAsync` | POST | `/v3/fund-app/mch-transfer/transfer-bills/transfer` | 同上 | 须先有授权单号（二选一） |

> **接口文件与守卫的旧表述已过期**：该文件类级 `<summary>` 仍写「首批 1 端点」、remarks 写
> 「本域现已 6 端点齐全」，`PayModule.Transfer` 注释写「首批 2 端点」⇒ **以 `PAY-B5` 的 10 条路由名册为准**。

## 资金安全红线：遇错**不得**换单重试

这是本域第一原则（官方原文，逐字落在接口 remarks 里）：

1. 发起转账遇到错误码时**不要**换 `out_bill_no` 重试 ⇒ 先经第 2 或第 3 支查清原单结果。
2. `state` 为 `ACCEPTED` / `PROCESSING` 时官方要求**原单重试**（不换单）；
   只有 `FAIL` 终态才允许**重新生成单据**再发起。
3. 违反的后果是**重复转账资金风险** ⇒ 本域必须两个查询端点齐备才谈生产可用（这也是 `PAY-B5`
   把两支查单都锁进名册的理由）。

## 两套同名 `state`，值表完全不同（守卫分别锁死）

转账单 `state`（`TransferBillStates`，8 值，`TR4` 精确相等断言）：

| 值 | 终态 | 官方语义要点 |
|---|---|---|
| `ACCEPTED` | 否 | 转账已受理，**可原单重试** |
| `PROCESSING` | 否 | 锁定资金中；长期停留 ⇒ 查余额，不足则充值后**原单重试** |
| `WAIT_USER_CONFIRM` | 否 | 待收款用户确认（资金已锁定，可拉起收款确认页） |
| `TRANSFERING` | 否 | 转账中，可再次拉起确认页重试确认收款 |
| `SUCCESS` | **是** | 转账成功 |
| `FAIL` | **是** | 转账失败；**只有此处**允许重新生成单据 |
| `CANCELING` | 否 | 撤销请求已受理，须继续查单 |
| `CANCELLED` | **是** | 撤销完成 |

电子回单 `state`（`TransferElecsignStates`，`TR6`）：`GENERATING` / `FINISHED` / `FAILED` ——
**两支共用同一个 JSON 键名 `state`，但值集合互不相通，不得混用**（常量类 remarks 明写）。

## 电子回单是两段式异步：申请 → 轮询 → 下载 → 校验

- 申请（第 7、8 支）与查询（第 4、5 支）分两段；首次调用常回 `GENERATING` ⇒ **必须轮询**到
  `FINISHED` 才拿到 `hash_value` / `download_url`。
- `download_url` **有效期仅 10 分钟**，过期须重新调查询支；官方要求**原样使用、勿拼接**。
- 下载后**必须**用 `hash_type` / `hash_value` 比对文件摘要（算法取值见 `TransferElecsignHashTypes`
  = `{SHA256, SM3}`，守卫 `TR6`），**不得**只凭「下载成功」采信回单内容。
- 回单支的收款人姓名（`user_name`）若发起时传过，查询会回，且**解密**由宿主负责（SDK 不代做）。

## 撤销支：无请求体、只能撤「用户确认收款之前」

`RevokeTransferAsync([Path] string outBillNo, CancellationToken)` —— 官方字段表**未定义任何 body 字段**
⇒ 不建请求 DTO。返回 `TransferRevokeResponse`（`out_bill_no` / `transfer_bill_no` / `state` / `update_time` 四键）。

- 时机：「商户通过转账接口发起付款后，在**用户确认收款之前**可以通过该接口撤销付款」⇒
  已确认收款或已完成的单据不能撤销。
- 异步语义：返回成功**仅表示撤销请求已受理** ⇒ 拿到 `CANCELING` 必须继续用第 2 支查到 `CANCELLED`。
- 路由对照（防串用，守卫 `TR5`）：本域是**普通商户**面 `/v3/fund-app/mch-transfer/transfer-bills/…`，
  服务商侧另有 `/v3/fund-app/mch-transfer/**partner**/transfer-bills/…` ⇒ `TR5` 断言商户路由在场、
  `/partner/` 变体**不在**本域路由集合内，二者**不可互换**。

## `mch_id` 与 `mchid`：本域用带下划线的那一种

守卫 `TR2` 专门锁这个差异：本域应答的商户号键是 **`mch_id`**（`TransferBillQueryResponse.MchId` →
`mch_id`，`TransferAuthorizedTransferResponse.MchId` → `mch_id`），而基础交易域关单请求体用的是
**`mchid`**（`CloseOrderRequest.MchId` → `mchid`）、转账到 QQ 钱包域应答又回到 `mchid`。
⇒ JSON 名照页不得「统一」，C# 属性名 `MchId` 与 `Mchid` 两种写法**跨域不得互换**。

同类陷阱：本域早期 DTO（`TransferBillRequest`）属性名是 `AppId` / `OpenId`（大写 I），
后批 DTO（`TransferPreTransferWithAuthorizationRequest` / `TransferAuthorizedTransferRequest`）是
`Appid` / `Openid` —— **JSON 名两者都是 `appid` / `openid`**，差异只在 C# 属性名，照声明写。

## 值表：三张中文取值表 + 一张「只收确证值」的场景表

守卫 `TR7` 用精确相等锁住四张常量表（`../../../Mud.Wechat.Pay/Transfer/TransferConstants.cs`）：

| 常量类 | 规模 | 要点 |
|---|---|---|
| `TransferRecvStyleTypes` | 2 值 | `user_recv_style.type` ∈ `{CONFIRM_PAGE, RED_PACKET}`（英文）。**红包样式有硬约束**：单笔金额 ≤ 200 元且仅部分场景支持 ⇒ 传 `RED_PACKET` 前须先确认场景支持 |
| `TransferUserRecvPerceptions` | **14 个中文取值** | `user_recv_perception`；本仓支付线罕见的**非英文**取值 ⇒ `TR7` 用「必须含中日韩统一表意文字」锁住，「顺手规范化」成英文即红 |
| `TransferSceneReportInfoTypes` | **13 个中文取值** | `transfer_scene_report_infos[].info_type`，同为中文 |
| `TransferSceneIds` | **只 1 值：`1000`** | 官方**无**集中值表（原文指向「商户平台-产品中心-商家转账-产品设置」）⇒ 只收录逐字确证的现金营销 `1000`，**不臆造**；不要照二手「1000–1011 全场景表」补齐 |

`transfer_scene_report_infos[].info_type` 的 DTO remarks 另有一条官方原文口径：
「**取值随转账场景而定，且有多个字段时需填写完整**」⇒ 少传报备项会被判参数错，
哪几项必填须按**具体场景页**逐场景核对（本仓不做本地校验，也没有场景-字段映射表）。

## 免确认收款授权的两支是一条链，不是一次调用

1. 第 9 支 `pre-transfer-with-authorization`：发起转账并**完成授权**，应答里的 `package_info` 只是
   **调起用户确认收款的凭据，不等于转账成功**；授权子对象
   `TransferPreTransferWithAuthorizationRequestAuthorizationInfo` 携带
   `user_display_name` / `out_authorization_no` / `authorization_notify_url`。
2. 第 10 支 `transfer`（用户授权后转账）：`authorization_id`（微信侧授权单号）与
   `out_authorization_no`（商户侧授权单号）**二选一必填** ⇒ SDK 不做本地互斥判定，缺配由应答 `code` 表达；
   该请求体**没有** `openid`（收款账户由授权决定）。

## 敏感字段与日志红线

`user_name`（收款人实名）是官方标注的**需加密字段**（微信支付公钥或平台证书公钥加密上送，
金额 ≥ 2,000 元时必填），`openid` 是用户标识：两者**不得**进日志、遥测、异常消息或本文示例。
**SDK 不代为加解密**，声明式签名也不承载 `Wechatpay-Serial` 头（由传输层按商户配置注入）。
凭据侧同理：`AddPayApp` 的商户配置只允许出现**密钥名**（`PrivateKeySecretName` /
`ApiKeySecretName`），真实私钥、APIv3 密钥、证书序列号一律走环境变量或本机 User Secrets。

## 认证

同本线口径：APIv3 商户证书 `WECHATPAY2-SHA256-RSA2048` 请求签名、无 `access_token`、
不声明 `[Token]`（`PAY-B1` fail-closed）；命名客户端 `wechat-pay`、BaseAddress 恒
`https://api.mch.weixin.qq.com`；`[AllowAnyStatusCode]` 让 4xx/5xx 的 `{"code","message"}` 落进
`WechatPayResponse`，判错走 `WechatPayException.ThrowIfFailed`；转账结果回调归
[../../../Mud.Wechat.Pay.Callback/README.md](../../../Mud.Wechat.Pay.Callback/README.md)（本域接口文件不含回调契约）。

## 快速开始

```csharp
builder.Services.AddPayApp(builder.Configuration, "WechatPay");   // 凭据底座只配**密钥名**，真实值走环境变量或本机 User Secrets
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.Transfer));

var transfer = scope.ServiceProvider.GetRequiredService<IWechatPayTransferService>();

// 1) 发起转账（金额单位分；user_name 须由宿主加密后上送，示例给占位符）
var bill = await transfer.CreateTransferBillAsync(new TransferBillRequest
{
    AppId = "wx0000000000000000",              // 占位符：须与商户号绑定
    OutBillNo = "TB20261018000001",            // 业务幂等键：仅数字与大小写字母，商户内唯一
    TransferSceneId = TransferSceneIds.CashMarketing,   // 常量表只收录逐字确证的 1000
    OpenId = "occupy-openid-000000000000000000000",
    TransferAmount = 100,                       // 1 元 = 100 分
    TransferRemark = "活动奖励",
    NotifyUrl = "https://example.com/transfer/notify",
    UserRecvPerception = TransferUserRecvPerceptions.CashReward,   // 常量名是英文、**值是中文**（"现金奖励"），勿英文化
    UserRecvStyle = new TransferUserRecvStyle { Type = TransferRecvStyleTypes.ConfirmPage },
    UserName = "<按 APIv3 加密后的收款人姓名密文>",   // 金额 >= 2000 元时必填；明文禁止入日志
    TransferSceneReportInfos = new List<TransferSceneReportInfo>
    {
        new()
        {
            InfoType = TransferSceneReportInfoTypes.ActivityName,       // 值是中文（"活动名称"），且按场景而定
            InfoContent = "十月拉新活动",
        },
    },
}, cancellationToken: ct);
WechatPayException.ThrowIfFailed(bill, "/v3/fund-app/mch-transfer/transfer-bills");
// package_info 只是「调起用户确认收款」的凭据，不是转账成功；bill.State 仍可能是 ACCEPTED

// 2) 红线动作：遇错**先查单**，不换单
var q = await transfer.QueryByOutBillNoAsync("TB20261018000001", ct);
WechatPayException.ThrowIfFailed(q, "/v3/fund-app/mch-transfer/transfer-bills/out-bill-no/{out_bill_no}");
switch (q.State)
{
    case TransferBillStates.Success: break;                       // 终态：成功
    case TransferBillStates.Fail: break;                           // 唯一允许「重新生成单据」的时点
    case TransferBillStates.Accepted:
    case TransferBillStates.Processing: break;                     // 原单重试，**不要**换 out_bill_no
    default: break;                                                 // 非终态：继续查单（30 天内可查）
}

// 3) 用户确认收款前撤销：无请求体，CANCELING 不是终态
var revoked = await transfer.RevokeTransferAsync("TB20261018000001", ct);
// 仍需用第 2 支查到 Cancelled 才算撤销完成

// 4) 电子回单：申请 → 轮询 FINISHED → 10 分钟内下载 → 用 hash 校验
await transfer.ApplyElecsignByOutBillNoAsync(new TransferElecsignApplyRequest { OutBillNo = "TB20261018000001" }, ct);
var receipt = await transfer.QueryElecsignByOutBillNoAsync("TB20261018000001", ct);
if (receipt.State == TransferElecsignStates.Finished)
{
    // receipt.DownloadUrl 原样使用（勿拼接），下载后按 receipt.HashType / receipt.HashValue 比对摘要
}
```

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| `transfer_scene_id` 取值全集 | 官方无集中值表，常量表**只收录 `1000`**；`TransferAuthorizedTransferRequest` 的 remarks 举例提到 `1006`（企业报销），但该值**未逐项核验** ⇒ 不入常量表 | 不得凭推断补路由或补取值；需要时逐场景页核验 |
| 转账额度/笔数上限（单笔、单日、单场景） | remarks 只指向「转账额度说明」⇒ **未逐项核验**，SDK 无本地金额闸 | 越界由应答 `code` 表达 |
| 各支端点级 QPS | 十支官方页**未印**端点级数字 ⇒ 不凭第三方 SDK 补数 | 记为「未印」 |
| 本域错误码逐支名册 | 接口 remarks 未逐支摘录（除红线原文）⇒ **未逐项核验** | 需要按码分支时先核验 |
| 电子回单 `FAILED` 后的恢复动作 | 值表有 `FAILED`，官方处置说明**未逐项核验** | 不得写「重新申请即可」一类推断 |
| APP / JSAPI 调起用户确认收款 | 官方清单里的第 ②③ 项属**客户端 SDK**，本仓不建模 ⇒ 未核验 | 需要时按客户端文档另建，勿补成服务端端点 |
| 转账结果回调报文形态 | 官方清单第 ⑦ 项归回调包 ⇒ 本域接口文件不含契约 | 查 `Mud.Wechat.Pay.Callback` |
| 接口文件 / `PayModule` 注释里的端点数（1 / 6 / 2） | 与 `PAY-B5` 的 10 条名册不一致 ⇒ 属文字漂移，**以守卫为准** | 修注释属代码改动，须另批授权 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 红线原文、可撤销时机、两段式与 10 分钟时限、30 天查询窗、授权二选一 | `IWechatPayTransferService.cs` 的类级与方法级 XML 注释 |
| 10 条路由名册（**计数权威**） | `WechatPayDomainContractGuards.cs` 的 `PAY-B5`（`RoutesOf(..., "IWechatPayTransferService")`） |
| 端点路由与方法照官方原文、无 path/query | `TransferEndpoints_ShouldMatchOfficialRoutes`（`TR1`） |
| `mch_id`（本域）vs `mchid`（关单请求体）字段名锁定 | `TransferDtos_ShouldExposeOfficialFieldNames`（`TR2`） |
| DTO 数 15、应答基底含 `code`/`message`、AOT 登记 | `TransferDataModels_ShouldBeRegisteredInJsonContext`（`TR3`） |
| `state` 8 值与「可原单重试」判定入口 | `TransferBillStates_ShouldMatchOfficialValues`（`TR4`） |
| 撤销支商户路由在场、`/partner/` 变体缺席 | `RevokeTransfer_ShouldUseMerchantRoute_NotPartnerRoute`（`TR5`） |
| 回单状态 3 值与摘要算法 `{SHA256, SM3}` | `ElecsignConstants_ShouldMatchOfficialValues`（`TR6`） |
| 收款样式、14 条中文感知、13 条中文报备类型、场景 id 只 1 值 | `RecvStyleAndSceneValueTables_ShouldMatchOfficialValues`（`TR7`） |
| DTO 与常量声明 | `../../../Mud.Wechat.Pay/Transfer/`（`TransferDtos.cs` / `TransferP12Dtos.cs` / `TransferConstants.cs`） |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方路由与限制，**同批**改 `PAY-B5` 的本域路由名册与
   `Baseline.Pay.Endpoints`（`../../../../../Tests/ContractBaseline.cs`），再改本文速览表与
   接口类级 summary 的过期端点数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `Transfer`）→ 跑
   `../../../../../scripts/AddHttpJsonSerializable.ps1` + `../../../../../scripts/GenerateJsonContext.ps1`
   → 更新 `TR3` 的 15 计数与 `TR2` 的字段名册。
3. 改值表 → `TransferBillStates`（8）/ `TransferElecsignStates`（3）/ `TransferRecvStyleTypes`（2）/
   `TransferUserRecvPerceptions`（14 中文）/ `TransferSceneReportInfoTypes`（13 中文）/ `TransferSceneIds`
   六张表**分别**改，各有对应守卫；`TransferSceneIds` 的「只收确证值」裁决要扩必须先有官方场景页依据。
4. **不要**为撤销支建请求 DTO；**不要**引入 `/partner/` 变体（那是服务商面）；**不要**把 `mch_id`
   改成 `mchid`；**不要**把中文取值「规范化」成英文；**不要**在示例里写真实姓名/证件/密钥；
   **不要**给任何方法加 `[Token]`。
5. 最后跑 `../../../../../scripts/verify-build.ps1` 与 `../../../../../scripts/audit-config-keys.ps1`，全绿才算完成。
