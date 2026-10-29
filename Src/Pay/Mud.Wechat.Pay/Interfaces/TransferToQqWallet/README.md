# TransferToQqWallet 域 · 转账到 QQ 钱包

微信支付 APIv3「转账到QQ钱包」域：**3 支声明式端点**（发起 / 查询 / 撤销）。
接口 [IWechatPayTransferToQqWalletService.cs](./IWechatPayTransferToQqWalletService.cs)，注册组 `TransferToQqWallet`，
模块枚举 `PayModule.TransferToQqWallet`（装配入口
`AddWechatPayApi(b => b.AddModules(PayModule.TransferToQqWallet))`，内部调用源生成的
`AddTransferToQqWalletWebApiHttpClient()`，见 [../../Extensions/PayServiceBuilder.cs](../../Extensions/PayServiceBuilder.cs)）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本域契约守卫
> [WechatPayTransferToQqWalletContractGuards.cs](../../../../../Tests/Mud.Wechat.Pay.Tests/ContractGuards/WechatPayTransferToQqWalletContractGuards.cs)
> （`TQW-B1`~`TQW-B4`）与本域 DTO（`../../../Mud.Wechat.Pay/DataModels/TransferToQqWallet/`）。
> 不复述外部官方文档正文；注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

计数权威是本域守卫 `TQW-B1`（接口拓扑 + 路由前缀 `transfer-to-qq-wallet-bills` 名册，正对照取
`IWechatPayMerchantFundService` 的两支路由）；三支均无 `[Query]` / `[Path]` 之外的定位面。

| # | 方法 | HTTP 动词 | 路由 | 认证/签名方式 | 频率限制（官方明印） | 备注 |
|---|---|---|---|---|---|---|
| 1 | `CreateTransferToQqWalletBillAsync` | POST | `/v3/fund-app/mch-transfer/transfer-to-qq-wallet-bills` | 商户证书 / APIv3 请求签名【普通商户】 | **100 次/s** | 应答 5 行，含 `create_time` |
| 2 | `QueryTransferToQqWalletBillAsync` | GET | `/v3/fund-app/mch-transfer/transfer-to-qq-wallet-bills/{outBillNo}` | 同上 | **200 次/s** | 应答 13 行 |
| 3 | `CancelTransferToQqWalletBillAsync` | POST | `/v3/fund-app/mch-transfer/transfer-to-qq-wallet-bills/{outBillNo}/cancel` | 同上 | **20 次/s** | **零请求体**、官方**印有**应答表（5 行，含 `update_time`） |

三支官方**各只印一页**、均为【普通商户】⇒ 一支路由一方法，不存在商户/服务商双页比对。
本域是 Pay 线里**少数在官方页明印端点级 QPS** 的域（三档各不相同），且**不得**跨域套用限频数字
—— 相邻的免确认授权转账域档位就与本域不同形。

## 认证：本线没有 access_token

APIv3 商户证书 `WECHATPAY2-SHA256-RSA2048` 请求签名，传输层 `WechatPayAuthorizationHandler` 注入，
本接口不声明 `[Token]`（全局守卫 `PAY-B1` fail-closed）；命名客户端 `wechat-pay`、
BaseAddress 恒 `https://api.mch.weixin.qq.com`；`[AllowAnyStatusCode]` + `WechatPayException.ThrowIfFailed`
构成唯一判错面。多商户经 `IWechatPayMerchantContext.UseMerchant(...)` 选作用域。

## 与商家转账（Transfer）域的关系：同资源段前缀，不同契约

本域全部走 `/fund-app/mch-transfer/transfer-to-qq-wallet-bills/*`，接口 remarks 明写
**DTO 不得与 `IWechatPayTransferService` 互相套用**（那是另一套注册组 `Transfer`）。
两支域之间可核对到的**逐字**差异（以各自 DTO 的 `JsonPropertyName` 为准）：

| 面 | 本域（`TransferToQqWallet`） | `Transfer` 域 |
|---|---|---|
| 应答里的商户号键 | `mchid`（三支应答均是） | `mch_id`（守卫 `TR2` 专门锁这一差异） |
| 收款方标识 | `openid` 与 `user_qq` **二选一必填** | 以 `openid` 为主 |
| 撤销定位键 | 只有 `out_bill_no`（path），**不存在**按微信单号撤销的官方面 | 另有按微信单号的支路 |
| 端点级 QPS | 官方明印 100 / 200 / 20 | 官方**未印**数字 |

接口 remarks 里出现「请求/应答多 `qq_wallet_openid`」的字样，而本域 DTO 与守卫 `TQW-B3` 的键名册
实际是 `openid` / `user_qq` ⇒ **以 DTO 名册与守卫为准**（remarks 措辞与代码不一致，见文末缺口）。

## 发起支的必填集与两处条件必填

请求体键集（`TQW-B3` 名册）：`appid` / `out_bill_no` / `transfer_scene_id` / `openid` / `user_qq` /
`user_name` / `transfer_amount` / `transfer_remark` / `notify_url` / `transfer_scene_report_infos`。

| 键 | 口径 |
|---|---|
| `appid` | 必填，`string(32)`；**QQ 互联开放平台**的 AppID，须与商户号有绑定关系 |
| `out_bill_no` | 必填，`string(32)`；仅数字与大小写字母、商户系统内唯一 ⇒ 本域幂等键（查询与撤销都以它定位） |
| `transfer_scene_id` | 必填，`string(36)`；官方举例 `1401`（活动奖励），**全集需在商户平台申请后确定**，本仓不建值表 |
| `openid` / `user_qq` | **二选一必填**（各自单标「选填」）⇒ SDK 不做本地互斥拦截，缺配由应答 `code` 表达 |
| `user_name` | 选填，但**转账金额 ≥ 2,000 元时必填**；须**加密传入**（标准 RSA 或国密，公钥由微信侧提供） |
| `transfer_amount` | 必填，`long?`，单位**分** |
| `transfer_remark` | 必填，`string(32)`，UTF-8，用户收款时可见 |
| `notify_url` | 选填，`string(256)`；**必须 HTTPS、公网可访问、不能携带参数** |
| `transfer_scene_report_infos` | 必填**数组**，元素两键 `info_type`（`string(15)`，固定值，按场景报备说明）+ `info_content`（`string(32)`） |

**敏感数据红线**：`user_name` 是收款人实名、`user_qq` 是收款账号，二者**不得**进日志、遥测或异常消息；
`user_name` 的密文形态由宿主负责，**SDK 不代为加解密**（本域声明式签名不带 `Wechatpay-Serial` 头，
涉密头由传输层按商户配置注入）。

## `state` 是三套不同的值表，别按「一套状态机」写判定

同一字段名在本域三支应答里取值集合**不同**（各 DTO 逐字摘录）：

| 端点 | `state` 取值 | 说明 |
|---|---|---|
| 发起 | `ACCEPTED` / `PROCESSING` / `WAIT_USER_CONFIRM` / `TRANSFERING` / `SUCCESS`（5 值） | 官方对 `PROCESSING` 的原文处置：长时间停留 ⇒ 查账户余额，不足则**充值后原单重试** |
| 查询 | 上述 5 值 **+ `FAIL` / `CANCELING` / `CANCELLED`**（8 值） | 只有查询面能看到失败与撤销态 |
| 撤销 | `CANCELING` / `CANCELLED`（2 值） | `CANCELING` 是「撤销请求受理成功」⇒ 仍非终态 |

⇒ 判「转账成功」只在 `SUCCESS`；判「已撤销完成」只在 `CANCELLED`；看到 `CANCELING` / `PROCESSING`
不得当终态。发起支的应答表里**没有** `FAIL`，不要在发起结果里分支 `FAIL`。

## 撤销支：零请求体但有类型化应答

`CancelTransferToQqWalletBillAsync([Path] string outBillNo, CancellationToken)` —— 定位键全在路径，
**不建**请求 DTO（接口 remarks 明写「既不得返回裸 `Task`，也不得凭空造请求 DTO」）。
应答 `TransferToQqWalletBillCancelResponse` 为 `mchid` / `out_bill_no` / `transfer_bill_no` / `state` / `update_time`。
`[PayModule.TransferToQqWallet]` 的 XML 注释里「撤销支官方未印应答表 ⇒ 返回裸 `Task`」一句与本域代码、
守卫 `TQW-B1`/`TQW-B3` 冲突 ⇒ **以代码与守卫为准**。

## 查询与撤销的错误码面

发起支官方码表：`400 PARAM_ERROR 参数错误` / `400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则` /
`401 SIGN_ERROR 验证不通过` / `500 SYSTEM_ERROR 系统异常，请稍后重试`。
查询与撤销两支只印 `404 NOT_FOUND 转账单不存在` ⇒ 两支码表**不同**，不得互挂。
`401 SIGN_ERROR` 属凭据面问题（商户证书序列号 / 私钥 / 平台证书轮换），恢复动作在配置与证书续期侧，
不在业务重试里 —— 换单重试只会重复失败。

## 快速开始

```csharp
builder.Services.AddPayApp(builder.Configuration, "WechatPay");   // 凭据底座只配**密钥名**，真实值走环境变量或本机 User Secrets
builder.Services.AddWechatPayApi(b => b.AddModules(PayModule.TransferToQqWallet));

var qq = scope.ServiceProvider.GetRequiredService<IWechatPayTransferToQqWalletService>();

// 1) 发起转账（金额单位分；openid 与 user_qq 二选一；user_name 须由宿主加密后上送）
var created = await qq.CreateTransferToQqWalletBillAsync(new TransferToQqWalletBillCreateRequest
{
    Appid = "1100000000",                       // 占位符：QQ 互联 AppID，须与商户号绑定
    OutBillNo = "QQ20261018000001",             // 仅数字与大小写字母，商户内唯一
    TransferSceneId = "1401",                   // 官方举例值：活动奖励
    Openid = "occupy-openid-000000000000000000000",
    TransferAmount = 100,                       // 1 元 = 100 分
    TransferRemark = "活动奖励",
    NotifyUrl = "https://example.com/qq/notify",// 必须 HTTPS、不带参数
    UserName = "<RSA/国密加密后的收款人姓名密文>", // 假值占位：明文姓名禁止入日志与异常消息
    TransferSceneReportInfos = new List<TransferToQqWalletBillCreateRequestTransferSceneReportInfosItem>
    {
        new()
        {
            InfoType = "活动名称",               // 固定值，按场景报备说明给
            InfoContent = "十月拉新活动",
        },
    },
}, cancellationToken: ct);
WechatPayException.ThrowIfFailed(created, "/v3/fund-app/mch-transfer/transfer-to-qq-wallet-bills");
if (created.State == "PROCESSING")
{
    // 官方处置原文：查余额是否充足，不足则充值后**原单重试**（不要换 out_bill_no）
}

// 2) 按商户单号查询（8 值 state 只有这一面能看到）
var bill = await qq.QueryTransferToQqWalletBillAsync("QQ20261018000001", ct);
WechatPayException.ThrowIfFailed(bill, "/v3/fund-app/mch-transfer/transfer-to-qq-wallet-bills/{out_bill_no}");
var succeeded = bill.State == "SUCCESS";

// 3) 撤销：零请求体，应答仍带 state（CANCELING 不是终态，须继续查询到 CANCELLED）
var cancelled = await qq.CancelTransferToQqWalletBillAsync("QQ20261018000001", ct);
WechatPayException.ThrowIfFailed(cancelled, "/v3/fund-app/mch-transfer/transfer-to-qq-wallet-bills/{out_bill_no}/cancel");
```

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| `transfer_scene_id` 取值全集 | 官方只举 `1401`（活动奖励），需「商户平台-产品中心-商家转账」申请 ⇒ 本仓**不建值表**，未逐项核验 | 不得照第三方表补数；需要时逐场景核验 |
| `transfer_scene_report_infos` 各场景的 `info_type` 固定值 | remarks 指向「转账场景报备信息字段说明」，本仓**未摘录** ⇒ 未逐项核验 | 建常量表前必须先核验该说明 |
| 单笔/单日金额上限、最小金额 | DTO 只写单位分 ⇒ 上下限**未逐项核验**，SDK 无本地金额闸 | 越界由应答 `code` 表达，不引入本地闸 |
| 撤销的业务前置条件（哪些状态可撤） | 官方应答只给 2 值 state，可撤销状态集合**未逐项核验** | 不得写「任意状态可撤」或从邻域借规则 |
| `user_name` 加密所用公钥与算法选择细节 | remarks 只说「RSA 或国密，公钥由微信侧提供」⇒ 具体密钥配置面**未逐项核验** | 加密在宿主侧，SDK 不代做 |
| 回调通知（`notify_url`）报文形态 | 由 [../../../Mud.Wechat.Pay.Callback/README.md](../../../Mud.Wechat.Pay.Callback/README.md) 的资源类型分发承载，本接口文件不含回调契约 | 回调面查 Callback 包 |
| 接口 remarks 的 `qq_wallet_openid` 措辞 | 与本域 DTO / 守卫 `TQW-B3` 的 `openid`+`user_qq` 名册不一致 ⇒ 属 remarks 文字漂移 | 以 DTO 与守卫为准；修 remarks 须另批走代码改动授权 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文、三档 QPS、各支错误码、撤销支「零请求体+有应答」裁决 | `IWechatPayTransferToQqWalletService.cs` 的类级与方法级 XML 注释 |
| 接口拓扑 / 路由前缀名册 / 正对照 | `TransferToQqWalletTopology_ShouldBeOneInterfaceOnOneGroup`（`TQW-B1`） |
| 三支路由与 path/query 形态（本域零 query） | `TransferToQqWalletEndpoints_ShouldMatchOfficialRoutesAndParameterRosters`（`TQW-B2`） |
| 请求与三支应答的字段名册 | `TransferToQqWalletDtos_ShouldExposeOfficialFieldRosters`（`TQW-B3`） |
| DTO 数 5、AOT 登记与 DI 接线 | `TransferToQqWalletDataModels_ShouldBeRegisteredInJsonContextAndWiredIntoDi`（`TQW-B4`） |
| DTO 字段名与类型 | `../../../Mud.Wechat.Pay/DataModels/TransferToQqWallet/` |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方路由、**官方明印的 QPS**、错误码与必填口径，**同批**改 `TQW-B1`
   的名册与 `TQW-B2` 的 per-endpoint 参数面；跨域总数同步 `../../../../../Tests/ContractBaseline.cs`。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取 `TransferToQqWallet`）→ 跑
   `../../../../../scripts/AddHttpJsonSerializable.ps1` + `../../../../../scripts/GenerateJsonContext.ps1`
   → 更新 `TQW-B3` 名册与 `TQW-B4` 的 5 计数。
3. 改 `state` 值表 → 三支各一套，**必须分别**改（发起 5 / 查询 8 / 撤销 2），不得合并成一套。
4. **不要**给撤销支造请求 DTO 或改成裸 `Task`；**不要**把 `mchid` 写成 `mch_id`（那是 `Transfer` 域）；
   **不要**把本域 DTO 与 `Transfer` 域互套；**不要**给任何方法加 `[Token]`。
5. 最后跑 `../../../../../scripts/verify-build.ps1` 与 `../../../../../scripts/audit-config-keys.ps1`，全绿才算完成。
