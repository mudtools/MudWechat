# License 域 · 接口调用许可

企业微信「接口调用许可」域：本仓共落地 **27 支端点**，按官方分组切成四族，全部为
**零端点抽象父接口 + 唯一第三方子接口承载**形态——
订单管理族 **15 支**（守卫 LC1 + LC3 `InlineData` 15）、账号管理族 **9 支**（LC3 `InlineData` 9）、
应用管理族 **1 支**（LC3 `InlineData` 1）、自动激活设置族 **2 支**（LC3 `InlineData` 2）。
父接口 [IWechatWorkLicenseOrderService.cs](./IWechatWorkLicenseOrderService.cs) /
[IWechatWorkLicenseAccountService.cs](./IWechatWorkLicenseAccountService.cs) /
[IWechatWorkLicenseAppService.cs](./IWechatWorkLicenseAppService.cs) /
[IWechatWorkLicenseAutoActiveService.cs](./IWechatWorkLicenseAutoActiveService.cs) 均 `IsAbstract`、不注册；
端点声明于 [IWechatWorkLicenseOrderService_ThirdParty.cs](./IWechatWorkLicenseOrderService_ThirdParty.cs) /
[IWechatWorkLicenseAccountService_ThirdParty.cs](./IWechatWorkLicenseAccountService_ThirdParty.cs) /
[IWechatWorkLicenseAppService_ThirdParty.cs](./IWechatWorkLicenseAppService_ThirdParty.cs) /
[IWechatWorkLicenseAutoActiveService_ThirdParty.cs](./IWechatWorkLicenseAutoActiveService_ThirdParty.cs)。
注册组 `License`，模块枚举 `WechatModule.License`（[`AddLicenseApi()`](../../Extensions/WechatWorkServiceBuilder.cs)）。
端点数以守卫为权威：**LC1 断言 27（订单族 15 + 账号族 9 + 应用族 1 + 自动激活族 2）** ⇒ 本文按 27 写；
仓内另有两处旧叙述（`AddLicenseApi()` 的 XML 与 [../../README.md](../../README.md)）写作「四族 25 端点 / 订单管理族 13」，
与 LC1 不一致，属**待订正文档漂移**，见「已知留档缺口」。逐端点核验 2026-10-18。

> **事实来源限定**：本文全部断言只来自本仓三类证据——本目录 8 份接口文件的 XML 注释、
> [WechatLicenseContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatLicenseContractGuards.cs)（LC1~LC6）、
> [Src/Work/Mud.Wechat.Work/License/](../../../Mud.Wechat.Work/License/) 下的 DTO 声明；
> 端点数、路由、字段名以守卫断言为准，官方文档号仅作定位标识、不作为本文事实来源。
> 未能在本仓三类证据内核验的项目一律集中在文末「已知留档缺口」，标为**未逐项核验**，正文不作推断。

## 端点速览

令牌/开放面：本域**不消费** `access_token`。LC4 对**全部 8 个接口**断言
`TokenType = WechatTokenTypes.ProviderAccessToken`、`InjectionMode = Query`、`Name = "provider_access_token"`，
并断言 `TokenManagerKey` **既不等于** `InternalAccessToken` **也不等于** `CorpAccessToken`
（provider 令牌本身已无歧义，见 [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs) TO1）。
`provider_access_token` 是服务商级凭据：**不得进日志、遥测或异常消息**，示例一律用占位符。
「开放面」= 官方文档树：本域端点由**第三方应用开发**与**服务商代开发**两棵树共享同一端点页，
企业自建树无对应 API（LC3 断言继承链上不得出现自建 / 代开发子接口）⇒ 表中统一写
`provider_access_token`(Query)；第三方 + 代开发同页（下节解释代开发为何不另立子接口）。

### 订单管理族（LC1 / LC3，15 支）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `CreateNewOrderAsync` | POST | `/cgi-bin/license/create_new_order` | `provider_access_token`(Query)；第三方 + 代开发同页 | 97182；基础与互通账号可同时购 |
| 2 | `CreateRenewOrderJobAsync` | POST | `/cgi-bin/license/create_renew_order_job` | 同上 | 97183；任务第一步，回 `jobid` |
| 3 | `SubmitOrderJobAsync` | POST | `/cgi-bin/license/submit_order_job` | 同上 | 97183；任务第二步 |
| 4 | `ListOrderAsync` | POST | `/cgi-bin/license/list_order` | 同上 | 97184；`start_time`/`end_time` 成对、区间 ≤31 天 |
| 5 | `GetOrderAsync` | POST | `/cgi-bin/license/get_order` | 同上 | 97185；应答含**加密** corpid |
| 6 | `ListOrderAccountAsync` | POST | `/cgi-bin/license/list_order_account` | 同上 | 97186；分页 + **加密** corpid / userid |
| 7 | `CancelOrderAsync` | POST | `/cgi-bin/license/cancel_order` | 同上 | 97187 |
| 8 | `CreateNewOrderJobAsync` | POST | `/cgi-bin/license/create_new_order_job` | 同上 | 98887；多企业新购第一步，`buy_list` ≤10 |
| 9 | `SubmitNewOrderJobAsync` | POST | `/cgi-bin/license/submit_new_order_job` | 同上 | 98887 |
| 10 | `GetNewOrderJobResultAsync` | POST | `/cgi-bin/license/new_order_job_result` | 同上 | 98887；仅提交后 **7 天**内可查 |
| 11 | `GetUnionOrderAsync` | POST | `/cgi-bin/license/get_union_order` | 同上 | 98888；应答含**加密** corpid |
| 12 | `SubmitPayJobAsync` | POST | `/cgi-bin/license/submit_pay_job` | 同上 | 99420；默认用充值余额，提交后支付方式不可变更 |
| 13 | `GetPayJobResultAsync` | POST | `/cgi-bin/license/pay_job_result` | 同上 | 99420；**顶层 `errcode` ≠ 支付结果**；仅提交后 7 天内可查 |
| 14 | `QuerySupportPolicyAsync` | POST | `/cgi-bin/license/support_policy_query` | 同上 | 97208；一企 30 天最多一次；注释记该优惠政策 2023-03-31 到期 |
| 15 | `GetAccountBalanceAsync` | **GET** | `/cgi-bin/service/get_account_balance` | 同上 | 100138；本域唯一 GET、唯一不在 `/cgi-bin/license/` 前缀下的路由；`balance` 单位**分** |

### 账号管理族（LC1 / LC3，9 支）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 16 | `ActiveAccountAsync` | POST | `/cgi-bin/license/active_account` | `provider_access_token`(Query)；第三方 + 代开发同页 | 97188；单成员激活 |
| 17 | `BatchActiveAccountAsync` | POST | `/cgi-bin/license/batch_active_account` | 同上 | 97188；单次 ≤1000 员工；应答含**加密** userid |
| 18 | `ActiveAccountByTypeAsync` | POST | `/cgi-bin/license/active_account_by_type` | 同上 | 97188；`type` 1 基础 / 2 互通 |
| 19 | `GetActiveInfoByCodeAsync` | POST | `/cgi-bin/license/get_active_info_by_code` | 同上 | 97189；应答含**加密** corpid / userid |
| 20 | `BatchGetActiveInfoByCodeAsync` | POST | `/cgi-bin/license/batch_get_active_info_by_code` | 同上 | 97189；`active_code_list` ≤1000 |
| 21 | `ListActivedAccountAsync` | POST | `/cgi-bin/license/list_actived_account` | 同上 | 97190；**官方路由拼写少一个 i**（LC6 ①）；分页；**加密** userid |
| 22 | `GetActiveInfoByUserAsync` | POST | `/cgi-bin/license/get_active_info_by_user` | 同上 | 97191；同类账号每人至多一个；**加密** userid |
| 23 | `BatchTransferLicenseAsync` | POST | `/cgi-bin/license/batch_transfer_license` | 同上 | 97192；单次 ≤1000；接收方剩余 ≤20 天才可继承；**加密** userid |
| 24 | `BatchShareActiveCodeAsync` | POST | `/cgi-bin/license/batch_share_active_code` | 同上 | 97193；单次 ≤1000 且不超下游通讯录人数上限两倍 |

### 应用管理族（1 支）与自动激活设置族（2 支）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 25 | `GetAppLicenseInfoAsync` | POST | `/cgi-bin/license/get_app_license_info` | `provider_access_token`(Query)；第三方 + 代开发同页 | 97194；试用期字段官方拼写 `trail_info`（LC6 ②） |
| 26 | `SetAutoActiveStatusAsync` | POST | `/cgi-bin/license/set_auto_active_status` | 同上 | 97199 |
| 27 | `GetAutoActiveStatusAsync` | POST | `/cgi-bin/license/get_auto_active_status` | 同上 | 97200 |

## 1. 四族一令牌：本域是全线的**刻意例外**，不得向邻域对齐

Work 线绝大多数域的令牌路由键恒为 `Wechat.AccessToken`（AGENTS.md §5.1「`TokenType` 恒为
`Wechat.AccessToken`，不得按应用类型拆分」）。**License 是已定夺的例外**：27 支端点全部消费服务商
`provider_access_token`，Query 参数名即 `provider_access_token`，且四族父接口与四个子接口**都不声明
`TokenManagerKey`**（LC4 用「不等于两个归属域键」的反向断言锁定；归属键只在 `AccessToken` 家族里有意义）。
⇒ 改动本域时**不要**给子接口补 `TokenManagerKey`、**不要**把 `TokenType` 换回 `AccessToken`，
两类改法都会同时打红 LC4、G5 白名单与 TO 系列守卫。
`provider_access_token` 与 `suite_ticket`、`auth_code`、`permanent_code` 同属凭据：只写参数名，
不得进日志、遥测或异常消息。

## 2. 代开发不另立子接口：一个第三方子接口承载两棵文档树

LC3 对四族各断言一次：`parent` 的 `IsAbstract = true`、`RegistryGroupName` 为空、**声明面为空**（零端点）；
`parent` 的派生接口集合**恰等于** `{ 该唯一第三方子接口 }`；子接口 `RegistryGroupName = "License"`、
`InheritedFrom = nameof(<父接口去 I>)`、声明面**恰为 15 / 9 / 1 / 2**。
接口注释同口径：官方在第三方与代开发两棵树下**共享同一端点页**，自建树无对应 API ⇒
服务商代开发场景**复用同一 `IWechatWorkThirdPartyLicense*Service` 子接口**（令牌本身就是服务商级，无企业 scope 差异），
不因为「代开发」三个字补 `*Provider*` 子接口——那会直接违反 LC3 的派生集合等值断言。

## 3. 三步任务化下单：`jobid` 是任务句柄，九处字面量都是全小写连写

三条异步链路（守卫 LC6 ⑤ 把 `jobid` 的字面量与属性名在 **9 个** DTO 上钉死：
`CreateRenewLicenseOrderJobRequest` / `SubmitLicenseOrderJobRequest` / `CreateNewLicenseOrderJobRequest` /
`SubmitNewLicenseOrderJobRequest` / `GetNewOrderJobResultRequest` / `SubmitLicensePayJobResponse` /
`GetLicensePayJobResultRequest` / `CreateRenewLicenseOrderJobResponse` / `CreateNewLicenseOrderJobResponse`）：

| 链路 | 步骤 | 端点 |
|---|---|---|
| 续期 | ① 建任务（带 `account_list`）→ ② 提交（带 `jobid` + `buyer_userid` + `account_duration`） | #2 → #3 |
| 多企业新购 | ① 建任务（带 `buy_list`）→ ② 提交 → ③ 取结果 | #8 → #9 → #10 |
| 余额支付 | ① 建支付任务（带 `order_id` + `payer_userid`）→ ② 取结果 | #12 → #13 |

任务态结果的可查窗口是**硬时效**：#10「仅在提交多企业新购订单后 7 天内可获取」、
#13「仅在提交了余额支付订单任务后的 7 天内可获取」（接口注释原文）⇒ 超窗后无源可查，
必须在链路内落库 `jobid` 与结果。

## 4. 余额支付：顶层 `errcode == 0` **不等于**支付成功

LC6 ③ 锁定 [GetLicensePayJobResultResponse.cs](../../../Mud.Wechat.Work/License/GetLicensePayJobResultResponse.cs)
必须同时承载 `Status`（`int?`，JSON `status`）与 `PayJobResult`（`LicensePayJobResult?`，JSON `pay_job_result`），
并断言 `Status` 类型为 `int?`，守卫 reason 原文写明「`status` 官方为支付任务结果（1 成功 / 2 执行中 / 3 失败），
与顶层 `errcode` 语义不同」。⇒ 判支付结果**只认 `Status` 与 `PayJobResult.Errcode`**，
`WechatWorkException.ThrowIfFailed(resp)` 只用来判「接口调用是否成功」，二者不可混用。
另两条注释事实：#12 默认使用充值账户余额支付，**提交成功后该订单无法再变更支付方式**；
支付失败时顶层 `errcode` 仍返回 0（守卫类级 remarks ③）。

## 5. 官方拼写陷阱：两处「看起来像 bug」的键必须照抄

| 陷阱 | 锁定位置 | 口径 |
|---|---|---|
| 路由 `list_actived_account` | LC6 ① 直接断言 `ListActivedAccountAsync` 的 `[Post]` 字面量 | 官方原文即 `actived`（少一个 i），**不得**「顺手修正」为 `activated`；C# 方法名也跟着照抄 |
| 字段 `trail_info` | LC6 ② 断言 `GetAppLicenseInfoResponse.TrailInfo` → `trail_info`，及 `LicenseTrailInfo.StartTime` → `start_time`、`EndTime` → `end_time` | 官方原文即 `trail_info`（非 `trial_info`） |

LC6 另外锁定的照抄型键（改属性名或 JSON 名即红）：`base_count` / `external_contact_count` /
`auto_active_status` / `from_corpid` / `to_corpid` / `corp_link_type` / `suite_id` / `appid` /
`corpid` / `query_result` / `unsatisfied_reason` / `balance`（`long?`，单位分）/
`months` / `days` / `new_expire_time`（`long?`）。

## 6. 分页四支：`limit` ≤1000（默认 500）、游标字符串、时间区间成对且 ≤31 天

LC2 对 **4 个请求类型**（`ListLicenseOrderRequest`、`ListLicenseOrderAccountRequest`、
`GetUnionLicenseOrderRequest`、`ListLicenseActivedAccountRequest`）逐一断言：`Limit` 存在且类型
`int?`、`Cursor` 存在且类型 `string`；另对 `ListLicenseOrderRequest` 断言 `StartTime` / `EndTime`
存在且类型 `long?`。守卫 reason 与接口注释同口径：`limit` 官方最大值 **1000**、默认 **500**；
`start_time` 与 `end_time` **必须同时指定、不能单独指定**，且区间**不超过 31 天**。
应答侧统一是 `NextCursor`（`string?`）+ `HasMore`（`int?`）+ 业务列表三件套
（`OrderList` / `AccountList`）。本仓不做本地时间配对校验与裁剪，越界由应答 `errcode` 表达。

## 7. 批量上限与容量闸（全部由官方侧执行）

| 端点 | 注释留档的上限 |
|---|---|
| #2 `create_renew_order_job` | `account_list` 每次最多 **1000** 个；同一 `jobid` 最多关联 **1,000,000** 个基础账号 + **1,000,000** 个互通账号 |
| #8 `create_new_order_job` | `buy_list` 每次最多 **10** 个；每个 `jobid` 最多关联 **100,000** 个 `BuyInfo`；**测试企业不支持多企业下单**；`auto_active_status` 不填默认**开启** |
| #17 `batch_active_account` | 单次激活员工数 ≤ **1000** |
| #20 `batch_get_active_info_by_code` | `active_code_list` ≤ **1000** |
| #23 `batch_transfer_license` | 单次转移账号数 ≤ **1000**；接收成员许可剩余时长 **≤ 20 天**才可成功继承，否则报错 |
| #24 `batch_share_active_code` | 每次分配 ≤ **1000** 个，且不能超过下游/下级企业在上下游/企业互联通讯录中的**人数上限的两倍**；`corp_link_type` 0 上下游 / 1 企业互联，默认 0 |
| #1 `create_new_order` | 基础账号与互通账号**不能同时为 0**；单类型最多 **1,000,000** 个（[LicenseAccountCount.cs](../../../Mud.Wechat.Work/License/LicenseAccountCount.cs)），**服务商测试企业最多各 1000 个** |
| #14 `support_policy_query` | 一个企业 **30 天**内最多查询一次 |

购买时长的官方算法（#1 注释 + [LicenseAccountDuration.cs](../../../Mud.Wechat.Work/License/LicenseAccountDuration.cs)）：
总天数 = `months * 31 + days`（**每月按 31 天**计），最少 1 个月、最多 **60 个月（1860 天）**；
续期侧 `months` 与 `new_expire_time` **二者填其一**（守卫类级 remarks ④），
`new_expire_time` 不可为今天及过去、不可为 1860 天后，且须填**当天 24 时 0 分 0 秒**，
否则系统自动归整为当天 24 时；测试企业每次续期只能续 **1 个月**。
`active_account_by_type` 的 `Type`（`int`）：**1 基础账号 / 2 互通账号**；
激活状态 `LicenseActiveInfo.Status`（`int?`）取值 **1 未绑定 / 2 已绑定且有效 / 3 已过期 /
4 待转移 / 5 已合并 / 6 已分配给下游**（其中 5 已失效、不可重新用于激活或继承）。

## 8. 八支端点应答返回**加密**的 corpid / userid：不得跨接口按明文 join

守卫类级 remarks ⑤ 列全：`get_order`、`list_order_account`、`get_union_order`、`get_active_info_by_code`、
`list_actived_account`、`get_active_info_by_user`、`batch_active_account`、`batch_transfer_license`。
这些字段在 DTO 上是 `string?`（如 `LicenseActivedAccount.Userid`、`LicenseActiveInfo.Userid`、
`LicenseOrderDetail.Corpid`），但**密文形态** ⇒ 与通讯录明文 userid 不同值、互不可推；
`LicenseActiveInfo` 注释另记「未激活则不返回 userid 字段」。⇒ 跨域聚合（例如与
`ExternalContact` / `Contact` 域对齐成员）须先经官方解密面处理，本域不提供任何转换。

## 9. 激活码是凭据；许可事件回调不在本域

`active_code`（`LicenseActiveInfo.ActiveCode`、`LicenseOrderAccount.ActiveCode`、
`LicenseShareActiveCode.ActiveCode`）是可兑换许可的凭据：**不得进日志、遥测或异常消息**，
落库与展示须脱敏，示例用占位符。
四支许可事件通知 **97195 接口许可失效 / 97196 支付成功 / 97197 退款结果 / 97198 自动激活**
（守卫类级 remarks，仅服务商代开发文档树提供）**不落在本域**：由 Callback 包的许可族载荷承载，
守卫是 `WechatCallbackContractGuards` 的 CB 系列。⇒ 不要在本域加回调端点。

## 10. 权限口径：下单人 / 支付人必须是服务商侧有权限的管理员

订单子接口注释留档：「下单 / 支付类端点的下单人与支付人必须为服务商企业内具有**购买接口许可**权限的管理员
（该 userid 必须登录过企业微信，且企业微信已绑定微信）」。对应请求键是
`buyer_userid`（#1、#3）与 `payer_userid`（#12）；本仓不做权限预判。
本族**无独立频率限制**，走官方全局访问频率（四份子接口注释逐条写明）。

## 快速开始

```csharp
// Program.cs：多应用底座先行，再注册本模块
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddLicenseApi());

// 抽象父接口不注册；第三方与服务商代开发共用同一子接口（LC3）
public sealed class LicenseProvisioning(
    IWechatWorkThirdPartyLicenseOrderService orders,
    IWechatWorkThirdPartyLicenseAccountService accounts,
    IWechatWorkThirdPartyLicenseAppService appLicense,
    IWechatWorkThirdPartyLicenseAutoActiveService autoActive)
{
    // 1) 下单购买账号：基础与互通不能同时为 0；每月按 31 天折算
    public async Task<string> BuyAsync(string corpId, string buyerUserId, CancellationToken ct)
    {
        var resp = await orders.CreateNewOrderAsync(new CreateNewLicenseOrderRequest
        {
            Corpid = corpId,                              // string，官方必填
            BuyerUserid = buyerUserId,                    // 须有「购买接口许可」权限
            AccountCount = new LicenseAccountCount        // base_count / external_contact_count
            {
                BaseCount = 100,
                ExternalContactCount = 20,
            },
            AccountDuration = new LicenseAccountDuration  // months * 31 + days，最多 60 个月
            {
                Months = 12,
                Days = 0,
            },
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/license/create_new_order");
        return resp.OrderId!;
    }

    // 2) 分页拉取订单列表：start_time / end_time 必须成对且区间 ≤31 天，limit ≤1000（默认 500）
    public async Task<List<LicenseOrderSummary>> ListOrdersAsync(
        long fromUnixSeconds, long toUnixSeconds, CancellationToken ct)
    {
        var all = new List<LicenseOrderSummary>();
        string? cursor = null;
        while (true)
        {
            var resp = await orders.ListOrderAsync(new ListLicenseOrderRequest
            {
                StartTime = fromUnixSeconds,              // long?
                EndTime = toUnixSeconds,                  // long?，与 StartTime 成对
                Limit = 500,                              // int?，最大值 1000
                Cursor = cursor,                          // string?
            }, ct);
            WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/license/list_order");
            all.AddRange(resp.OrderList ?? []);
            if (resp.HasMore != 1 || string.IsNullOrEmpty(resp.NextCursor)) break;
            cursor = resp.NextCursor;
        }

        return all;
    }

    // 3) 激活账号（单成员，按类型）：active_code 属凭据，勿写日志
    public async Task ActivateAsync(string activeCode, string corpId, string userId, CancellationToken ct)
    {
        var resp = await accounts.ActiveAccountByTypeAsync(new ActiveLicenseAccountByTypeRequest
        {
            Type = 1,                                     // 1 基础账号 / 2 互通账号
            Corpid = corpId,
            Userid = userId,
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/license/active_account_by_type");
    }

    // 4) 余额支付：errcode == 0 只代表接口调用成功，支付结果看 status（1 成功 / 2 执行中 / 3 失败）
    public async Task<bool> PayWithBalanceAsync(string orderId, string payerUserId, CancellationToken ct)
    {
        var submitted = await orders.SubmitPayJobAsync(new SubmitLicensePayJobRequest
        {
            OrderId = orderId,
            PayerUserid = payerUserId,
        }, ct);
        WechatWorkException.ThrowIfFailed(submitted, "/cgi-bin/license/submit_pay_job");
        // 提交成功后该订单支付方式不可再变更；jobid 为全小写连写（LC6 ⑤）
        var result = await orders.GetPayJobResultAsync(
            new GetLicensePayJobResultRequest { Jobid = submitted.Jobid! }, ct);
        WechatWorkException.ThrowIfFailed(result, "/cgi-bin/license/pay_job_result");

        return result.Status == 1;                          // 不要只看顶层 errcode
    }

    // 5) 应用许可信息（试用期字段官方拼写 trail_info）与服务商充值余额（单位：分）
    public async Task<long?> TrailEndTimeAsync(string corpId, string suiteId, CancellationToken ct)
    {
        var resp = await appLicense.GetAppLicenseInfoAsync(new GetAppLicenseInfoRequest
        {
            Corpid = corpId,
            SuiteId = suiteId,
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/license/get_app_license_info");

        var balance = await orders.GetAccountBalanceAsync(ct);   // GET /cgi-bin/service/get_account_balance
        WechatWorkException.ThrowIfFailed(balance, "/cgi-bin/service/get_account_balance");

        return resp.TrailInfo?.EndTime ?? balance.Balance;       // 二者均为 long?，仅示例取值形状
    }

    // 6) 自动激活开关：0 关闭 / 1 开启（不填默认开启，见 #8 注释）
    public async Task SetAutoActiveAsync(string corpId, CancellationToken ct)
    {
        var resp = await autoActive.SetAutoActiveStatusAsync(new SetLicenseAutoActiveStatusRequest
        {
            Corpid = corpId,
            AutoActiveStatus = 1,                          // int，官方必填
        }, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/license/set_auto_active_status");
    }
}
```

判错为单层普通信封（`errcode == 0` 即接口调用成功）；**余额支付结果属双层语义**，
须按 §4 另读 `status` / `pay_job_result.errcode`。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 本域端点总数口径冲突 | LC1 断言 **27**（订单族 15），但 [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs) 的 `AddLicenseApi()` XML 与 [../../README.md](../../README.md) 写「四族 **25** 端点 / 订单管理族 **13**」 | **以守卫为权威**（本文按 27）；两处旧叙述待同批订正，本文不改动任何代码或既有文档 |
| 独立频率限制 | 四份子接口注释逐条写「官方页面未给出本族端点的独立频率限制，走官方全局访问频率限制」⇒ 全局频率的具体数值**未逐项核验** | 需要自建限流时先核验官方全局频控页 |
| `order_status` / `order_type` 取值全集 | [LicenseOrderDetail.cs](../../../Mud.Wechat.Work/License/LicenseOrderDetail.cs) 只有 `OrderStatus`（`int?`）与 `OrderType`（`int?`），注释无取值表 ⇒ **未逐项核验** | 以 `int?` 承载，判定时先核验，勿借邻域枚举 |
| `query_result` / `unsatisfied_reason` 取值语义 | [QueryLicenseSupportPolicyResponse.cs](../../../Mud.Wechat.Work/License/QueryLicenseSupportPolicyResponse.cs) 只有类型（`int?` / `List<int>?`）；且注释记该优惠政策 2023-03-31 到期后不再支持查询 ⇒ 现行可用性与码值含义**未逐项核验** | 该端点保留但不用于判定；如需使用先核验官方页 |
| 价格与订单金额单位 | `LicenseOrderDetail.Price` 为 `long?`，注释未标单位（仅 #15 的 `balance` 明确「单位分」）⇒ **未逐项核验** | 不得据 `balance` 推断 `price` 也是分 |
| `license_status`（#25）取值 | `GetAppLicenseInfoResponse.LicenseStatus` 为 `int?`，注释无取值表 ⇒ **未逐项核验** | 不建枚举、不做「等于某常量才算有效」的判定 |
| 测试企业阈值的判定入口 | 注释多处出现「服务商测试企业最多各购买 1000 个 / 不支持多企业下单 / 每次只能续 1 个月」，但「如何识别当前企业是测试企业」在本域无对应端点 ⇒ **未在本域核验** | 需要该判定前先核验官方页，勿从邻域借 |
| #14 的 30 天限频计数维度 | 注释只写「一个企业在 30 天内最多只能查询一次」，超限错误码**未逐项核验** | 判错只按 `errcode != 0` 通用处理 |
| 回调四事件（97195~97198）载荷形状 | 本域零承载，属 Callback 包 CB 系列 ⇒ 载荷字段**未在本域核验** | 事件接入看 Callback 包，勿在本域加面 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（批量上限、7 天窗口、31 天区间、测试企业阈值、每月 31 天、权限口径、无独立频率） | 本目录 8 份接口文件的 XML 注释 |
| 路由表与全域计数（恰 27，互不重复） | [WechatLicenseContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatLicenseContractGuards.cs) `Routes`（L63–L155）+ LC1（L156–L177） |
| 分页字段与类型（`Limit int?` / `Cursor string?` / `StartTime`、`EndTime` `long?`） | LC2（L179–L210） |
| 四族层级与各族声明面计数（15 / 9 / 1 / 2）、父接口零端点、派生集合恰一 | LC3 `Theory`（L212–L248） |
| `ProviderAccessToken` + Query + `provider_access_token` + **不声明归属域键** | LC4（L250–L286） |
| JSON 上下文全量登记（74 型 = 请求 26 + 响应 22 + 嵌套 26，`SerializerClassName = "License"`） | LC5（L288–L316） |
| 官方契约陷阱（`list_actived_account`、`trail_info`、`status`/`pay_job_result`、时长超集、`jobid` 九处、照抄键集、`balance` 单位分） | LC6（L318–L383，辅助 `JsonNameShouldBe` L385–L393） |
| 开放面 / 加密标识 / 回调归属等域级结论 | 同文件类级 remarks（L22–L50） |
| DTO 属性名 / 类型 / JSON 名 | [../../../Mud.Wechat.Work/License/](../../../Mud.Wechat.Work/License/)（含 `RequestModel/` 26 份请求体） |
| 上下文源生成文件 | [../../../Mud.Wechat.Work/Generated/LicenseJsonContext.g.cs](../../../Mud.Wechat.Work/Generated/LicenseJsonContext.g.cs) |
| 归属键「非 AccessToken 家族不得带归属键」的总线口径 | [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)（TO1~TO3） |
| Query 令牌注入白名单中的本域接口名 | [WechatContractGuards.QueryTokenInjection.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.QueryTokenInjection.cs)（G5） |
| 许可事件回调载荷（97195~97198） | `WechatCallbackContractGuards`（CB 系列），不在本域 |
| 模块注册三段式 | [WechatModule.cs](../../Extensions/WechatModule.cs)、[WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)、[WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs)（L170） |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档号与批量/时效约束，**同批**改
   [WechatLicenseContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatLicenseContractGuards.cs)
   的 `Routes` 表 + LC1 双 `HaveCount(27)`，以及 LC3 `InlineData` 的族计数（15 / 9 / 1 / 2）；
   改 `start_time`/`end_time`/`limit`/`cursor` 字段名或类型须同批改 LC2 的四个类型清单与类型断言。
2. **不要**给本域任何接口补 `TokenManagerKey` 或把 `TokenType` 改为 `Wechat.AccessToken`；
   **不要**新增 `*Internal*` / `*Provider*` 子接口（LC3 的派生集合等值断言与 LC4 的反向断言会同时打红）。
   若官方改为向自建开放，须先核验官方页，再同批改 LC3 / LC4 / G5 白名单与本域接口落位形态。
3. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 必须是 `License`）→ 跑
   `../../../../../scripts/AddHttpJsonSerializable.ps1` 与 `../../../../../scripts/GenerateJsonContext.ps1` →
   更新 LC5 的全量计数（当前 **74**：请求 26 + 响应 22 + 嵌套 26）与
   [WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs)（L170）。
   LC5 是「命名空间下所有顶层类」的**全量**断言：新增任何 `Mud.Wechat.Work.DataModels.License` 类型都必须登记，
   否则直接打红；漂移 reason 要求「先核对官方文档再同批调整本守卫」。
4. 改官方拼写相关键（`list_actived_account`、`trail_info`、`jobid`、`base_count` 等）→ 必须同批改 LC6，
   且属性名保持与 JSON 名同一拼写口径（`Jobid` / `TrailInfo` / `ListActivedAccountAsync`），不得「修正」。
5. 要接许可事件（97195~97198）→ **不落在本域**：改 Callback 包的许可族载荷与 CB 系列守卫，
   并同步本文 §9 的归属表述。
6. 模块三段式同批：[WechatModule.cs](../../Extensions/WechatModule.cs) 的 `License` remarks、
   [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs) 的 `AddLicenseApi()` XML（其
   「25 / 13」表述须订正为守卫的 27 / 15）、可注入子接口的 `RegistryGroupName = "License"`。
7. 本文与 [../../README.md](../../README.md) 的域内端点数、族划分表述须同步更新；
   最后跑 `../../../../../scripts/verify-build.ps1` 与 `../../../../../scripts/audit-config-keys.ps1`，全绿才算完成。
