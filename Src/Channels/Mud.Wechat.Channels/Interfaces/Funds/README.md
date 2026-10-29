# Funds 域 · 资金结算

微信小店 / 视频号（channels 生态）「资金结算」域 SDK（16 端点：账户余额 / 结算账户 / 提现 /
资金流水 / 订单流水 / 银行·城市·支行查询 / 资金二维码）。 接口：[IChannelsFundsService](./IChannelsFundsService.cs)；注册组 `Funds`；模块枚举 `ChannelsModule.Funds`（装配入口见 [ChannelsServiceBuilder.cs](../../Extensions/ChannelsServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[ChannelsFundsContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsFundsContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Channels/DataModels/Funds`](../../DataModels/Funds)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IChannelsFundsService = 16；按 HTTP 特性实计 **16** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IChannelsFundsService` | `GetBalanceAsync` | POST | `/channels/ec/funds/getbalance` | `AccessToken`（Query），Query `access_token` | 获取账户余额（`getbalance`，可提现余额 + 待结算余额 + 二级商户号）。 |
| 2 | `IChannelsFundsService` | `GetBankAccountAsync` | POST | `/channels/ec/funds/getbankacct` | `AccessToken`（Query），Query `access_token` | 获取结算账户（`getbankacct`）。 |
| 3 | `IChannelsFundsService` | `SetBankAccountAsync` | POST | `/channels/ec/funds/setbankacct` | `AccessToken`（Query），Query `access_token` | 修改结算账户（`setbankacct`）。 |
| 4 | `IChannelsFundsService` | `SubmitWithdrawAsync` | POST | `/channels/ec/funds/submitwithdraw` | `AccessToken`（Query），Query `access_token` | 商户提现（`submitwithdraw`）。 |
| 5 | `IChannelsFundsService` | `GetWithdrawListAsync` | POST | `/channels/ec/funds/getwithdrawlist` | `AccessToken`（Query），Query `access_token` | 获取提现记录列表（`getwithdrawlist`）。 |
| 6 | `IChannelsFundsService` | `GetWithdrawDetailAsync` | POST | `/channels/ec/funds/getwithdrawdetail` | `AccessToken`（Query），Query `access_token` | 获取提现记录（`getwithdrawdetail`）。 |
| 7 | `IChannelsFundsService` | `GetFundsFlowListAsync` | POST | `/channels/ec/funds/getfundsflowlist` | `AccessToken`（Query），Query `access_token` | 获取资金流水列表（`getfundsflowlist`，对账主入口）。 |
| 8 | `IChannelsFundsService` | `GetFundsFlowDetailAsync` | POST | `/channels/ec/funds/getfundsflowdetail` | `AccessToken`（Query），Query `access_token` | 获取资金流水详情（`getfundsflowdetail`）。 |
| 9 | `IChannelsFundsService` | `ListOrderFlowAsync` | POST | `/channels/ec/funds/listorderflow` | `AccessToken`（Query），Query `access_token` | 查询订单流水列表（`listorderflow`，订单结算信息）。 |
| 10 | `IChannelsFundsService` | `GetCityListAsync` | POST | `/shop/funds/getcity` | `AccessToken`（Query），Query `access_token` | 查询城市列表（`getcity`，开户银行省市编码来源）。 |
| 11 | `IChannelsFundsService` | `GetProvinceListAsync` | POST | `/shop/funds/getprovince` | `AccessToken`（Query），Query `access_token` | 查询大陆银行省份列表（`getprovince`）。 |
| 12 | `IChannelsFundsService` | `GetBankListAsync` | POST | `/shop/funds/getbanklist` | `AccessToken`（Query），Query `access_token` | 搜索银行列表（`getbanklist`，修改结算账户的前置查询）。 |
| 13 | `IChannelsFundsService` | `GetSubBranchListAsync` | POST | `/shop/funds/getsubbranch` | `AccessToken`（Query），Query `access_token` | 查询支行列表（`getsubbranch`，获取开户银行联行号）。 |
| 14 | `IChannelsFundsService` | `GetBankByCardNumberAsync` | POST | `/shop/funds/getbankbynum` | `AccessToken`（Query），Query `access_token` | 根据卡号查银行信息（`getbankbynum`）。 |
| 15 | `IChannelsFundsService` | `GetFundsQrcodeAsync` | POST | `/shop/funds/qrcode/get` | `AccessToken`（Query），Query `access_token` | 获取二维码（`qrcode/get`，`qrcode_buf` 为 base64 编码的二维码二进制，需 base64 解码）。 |
| 16 | `IChannelsFundsService` | `CheckFundsQrcodeAsync` | POST | `/shop/funds/qrcode/check` | `AccessToken`（Query），Query `access_token` | 查询扫码状态（`qrcode/check`）。 |

## 令牌与认证

本域全部端点消费 `Wechat.Channels.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**与「微信支付 APIv3」「企微企业支付」概念严格区分（设计方案 v1 §4.6）**：本域是小店
**结算 / 提现 / 流水**（凭据为小店 `access_token`），**不是**支付收单
（`Mud.Wechat.Pay` 为商户私钥签名）也**不是**企微对外收款 / 收银台（Work 线）。
守卫 `ChannelsFundsContractGuards` 措辞锁定该边界。

**路由前缀并存（设计方案 v1 §4.3）**：本域同时使用 `/channels/ec/funds/*`（9 端点）与
`/shop/funds/*`（7 端点，官方历史前缀，照抄原文不得改写）。

**金额单位**：本域全部金额字段（余额 / 提现 / 流水 / 结算）单位均为**分**。

**MUD005 已知接受风险**：微信小店官方契约强制令牌走 Query 参数（`access_token`），
无法改用 Header；库内遥测与异常消息的 URL 已由组件 `SensitiveUrlRedactor` 与
`WechatChannelsException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 16 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。
- （出现 5 次）第三方平台代调用所属权限集 id 为 `138`。
- （出现 5 次）第三方平台代调用所属权限集 id 为 `85`、`138`。
- （出现 3 次）官方业务限制：**不支持云调用**；专属错误码 `9710001`（暂无数据）。

### 1. `GetBalanceAsync` — 获取账户余额（`getbalance`，可提现余额 + 待结算余额 + 二级商户号）。

- **声明接口**：[IChannelsFundsService](./IChannelsFundsService.cs)
- **官方契约**：`POST /channels/ec/funds/getbalance`
- **返回**：`ChannelsGetBalanceResponse` — 账户余额（`available_amount` 可提现余额 / `pending_amount` 待结算余额，单位：分）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/funds/funds/api_getbalance
- 官方契约：**POST** + 空请求体（官方原文「调用接口时传空的json串即可」）。
- 官方业务限制：第三方平台代调用所属权限集 id 为 `138`；本接口无特殊错误码（走通用错误码）。

### 2. `GetBankAccountAsync` — 获取结算账户（`getbankacct`）。

- **声明接口**：[IChannelsFundsService](./IChannelsFundsService.cs)
- **官方契约**：`POST /channels/ec/funds/getbankacct`
- **返回**：`ChannelsGetBankAcctResponse` — 结算账户信息（`account_info`：账户类型 / 开户银行 / 省市编码 / 联行号 / 全称 / 账号 / 账户名称）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/funds/funds/api_getbankacct
- 官方契约：**POST** + 空请求体（官方原文「调用接口时传空的json串即可」）。
- 官方业务限制：第三方平台代调用所属权限集 id 为 `138`；账户类型枚举见 `ChannelsBankAccountTypes`。

### 3. `SetBankAccountAsync` — 修改结算账户（`setbankacct`）。

- **声明接口**：[IChannelsFundsService](./IChannelsFundsService.cs)
- **官方契约**：`POST /channels/ec/funds/setbankacct`

- **请求体**：`ChannelsSetBankAcctRequest` — 修改请求（`account_info` 内 `bank_account_type`/`account_bank`/`bank_address_code`/`account_number` 必填）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/funds/funds/api_setbankacct
- 官方业务限制：①**每天允许修改结算账户的次数为 5 次**；②结算账户需要与店铺的主体一致 / （对公为公司名称，对私为法人名称）；③企业类型只能绑定对公账户，个体工商户可以绑定对公或者对私账户。
- 成功修改后触发官方「结算账户变更回调」事件。

### 4. `SubmitWithdrawAsync` — 商户提现（`submitwithdraw`）。

- **声明接口**：[IChannelsFundsService](./IChannelsFundsService.cs)
- **官方契约**：`POST /channels/ec/funds/submitwithdraw`

- **请求体**：`ChannelsSubmitWithdrawRequest` — 提现请求（`amount` 必填，单位：分）。
- **返回**：`ChannelsSubmitWithdrawResponse` — 提现单号与二维码 ticket。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/funds/funds/api_submitwithdraw
- 官方业务限制：**每天最多允许提现一次**；成功后将触发官方「提现回调」与「提现二维码回调」两个事件。
- **官方文档描述互换（照录）**：官方参数表把 `withdraw_id` 说明写为「二维码ticket」、 / `qrcode_ticket` 写为「提现单号」——径自矛盾；字段语义以字面名为准（`qrcode_ticket` 供 / `qrcode/get` / `qrcode/check` 使用）。

### 5. `GetWithdrawListAsync` — 获取提现记录列表（`getwithdrawlist`）。

- **声明接口**：[IChannelsFundsService](./IChannelsFundsService.cs)
- **官方契约**：`POST /channels/ec/funds/getwithdrawlist`

- **请求体**：`ChannelsGetWithdrawListRequest` — 查询请求（`page_num`/`page_size` 必填）。
- **返回**：`ChannelsGetWithdrawListResponse` — 提现单号列表与总数（`withdraw_ids`/`total_num`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/funds/funds/api_getwithdrawlist
- 官方业务限制：本接口无特殊注意事项；专属错误码 `10022002`（违规行为，橱窗被禁止使用）。

### 6. `GetWithdrawDetailAsync` — 获取提现记录（`getwithdrawdetail`）。

- **声明接口**：[IChannelsFundsService](./IChannelsFundsService.cs)
- **官方契约**：`POST /channels/ec/funds/getwithdrawdetail`

- **请求体**：`ChannelsGetWithdrawDetailRequest` — 查询请求（`withdraw_id` 必填，可从获取提现记录列表接口获取）。
- **返回**：`ChannelsGetWithdrawDetailResponse` — 提现明细（金额 / 时间 / 备注 / 银行 / 状态）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/funds/funds/api_getwithdrawdetail
- 官方业务限制：本接口无特殊注意事项；提现状态枚举见 `ChannelsWithdrawStatuses`。

### 7. `GetFundsFlowListAsync` — 获取资金流水列表（`getfundsflowlist`，对账主入口）。

- **声明接口**：[IChannelsFundsService](./IChannelsFundsService.cs)
- **官方契约**：`POST /channels/ec/funds/getfundsflowlist`

- **请求体**：`ChannelsGetFundsFlowListRequest` — 查询请求（全字段选填；深翻页时 `next_key` 必填）。
- **返回**：`ChannelsGetFundsFlowListResponse` — 流水单号列表与翻页信息（`flow_ids`/`has_more`/`next_key`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/funds/funds/api_getfundsflowlist
- 官方业务限制：①当日资金流水**出账时间为次日 16 点后**；②先用后付订单需用户确认收货自动付款后才会产生资金流水； / ③**`end_time` 受出账约束**——16 点前最大为前 1 天 0 点 0 分 0 秒，16 点后最大为当天 0 点 0 分 0 秒， / 超过最大值将按最大值进行查询；④翻页需回传 `next_key`（page 为上一页加一且 page_size 相同才生效）， / `page * page_size >= 10000` 时必填。
- 第三方平台代调用所属权限集 id 为 `138`、`141`。

### 8. `GetFundsFlowDetailAsync` — 获取资金流水详情（`getfundsflowdetail`）。

- **声明接口**：[IChannelsFundsService](./IChannelsFundsService.cs)
- **官方契约**：`POST /channels/ec/funds/getfundsflowdetail`

- **请求体**：`ChannelsGetFundsFlowDetailRequest` — 查询请求（`flow_id` 必填，可通过获取资金流水列表获取）。
- **返回**：`ChannelsGetFundsFlowDetailResponse` — 流水详情（`funds_flow`：资金类型 / 收支 / 金额 / 余额 / 关联信息列表）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/funds/funds/api_getfundsflowdetail
- 官方业务限制：本接口无特殊注意事项；`funds_type` 枚举见 `ChannelsFundsTypes`； / 专属错误码 `10021302`（暂无数据）。
- 第三方平台代调用所属权限集 id 为 `138`、`141`。

### 9. `ListOrderFlowAsync` — 查询订单流水列表（`listorderflow`，订单结算信息）。

- **声明接口**：[IChannelsFundsService](./IChannelsFundsService.cs)
- **官方契约**：`POST /channels/ec/funds/listorderflow`

- **请求体**：`ChannelsListOrderFlowRequest` — 查询请求（`order_settle_state` 必填）。
- **返回**：`ChannelsListOrderFlowResponse` — 订单流水列表（`data_list`）与总数（`total_count`，仅供参考）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/funds/funds/api_listorderflow
- 官方业务限制：①**本接口不支持云调用**；②所有项目的结算状态与 `order_settle_state` 相同 / （官方注意事项原文）；③翻页推荐使用分页上下文 `use_page_ctx` + `page_ctx`（保证不遗漏不重复，`offset` 不生效）。
- 官方错误码：`669900000`（参数错误，具体查看 errmsg）/ `669900001`（系统异常，请重试）。

### 10. `GetCityListAsync` — 查询城市列表（`getcity`，开户银行省市编码来源）。

- **声明接口**：[IChannelsFundsService](./IChannelsFundsService.cs)
- **官方契约**：`POST /shop/funds/getcity`

- **请求体**：`ChannelsGetCityRequest` — 查询请求（`province_code` 必填，经「查询大陆银行省份列表」获取）。
- **返回**：`ChannelsGetCityResponse` — 城市信息列表（`city_name`/`city_code`/`bank_address_code`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/funds/bank/api_ecgetcity
- 官方业务限制：**本接口不支持云调用**；专属错误码 `9710001`（暂无数据）； / 官方适用范围表同时放行「小程序」与「微信小店」两类账号。

### 11. `GetProvinceListAsync` — 查询大陆银行省份列表（`getprovince`）。

- **声明接口**：[IChannelsFundsService](./IChannelsFundsService.cs)
- **官方契约**：`POST /shop/funds/getprovince`
- **返回**：`ChannelsGetProvinceResponse` — 省份信息列表（`province_name` 省份简称 / `province_code` 省份编码）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/funds/bank/api_ecgetprovince
- 官方契约：**POST** + 空请求体（官方原文「调用接口时传空的json串即可」）；**不支持云调用**。
- 官方适用范围表同时放行「小程序」与「微信小店」两类账号。

### 12. `GetBankListAsync` — 搜索银行列表（`getbanklist`，修改结算账户的前置查询）。

- **声明接口**：[IChannelsFundsService](./IChannelsFundsService.cs)
- **官方契约**：`POST /shop/funds/getbanklist`

- **请求体**：`ChannelsGetBankListRequest` — 搜索请求（全字段选填，默认对公）。
- **返回**：`ChannelsGetBankListResponse` — 银行信息列表（`bank_code`/`bank_id`/`bank_name`/`need_branch`/`bank_type`/`account_bank`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/funds/bank/api_ecgetbanklist
- **官方文档矛盾照录**：入参 `bank_type` 说明为「1 对私，2 对公；默认对公」，返回参数 / `bank_type` 说明为「1 对公，2 对私」——两处口径相反，SDK 按入参参数表建模（`BankType`）。

### 13. `GetSubBranchListAsync` — 查询支行列表（`getsubbranch`，获取开户银行联行号）。

- **声明接口**：[IChannelsFundsService](./IChannelsFundsService.cs)
- **官方契约**：`POST /shop/funds/getsubbranch`

- **请求体**：`ChannelsGetSubBranchRequest` — 查询请求（`bank_code`/`city_code` 必填）。
- **返回**：`ChannelsGetSubBranchResponse` — 支行信息列表与银行别名等（`data`/`total_count`/`count`/`account_bank_code`/`bank_alias`/`bank_alias_code`/`account_bank`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/funds/bank/api_ecgetsubbranch
- **官方文档矛盾照录**：参数表 `data` 元素字段名为 `bank_id`/`bank_name`， / 代码示例为 `branch_id`/`branch_name`——SDK 按参数表建模。

### 14. `GetBankByCardNumberAsync` — 根据卡号查银行信息（`getbankbynum`）。

- **声明接口**：[IChannelsFundsService](./IChannelsFundsService.cs)
- **官方契约**：`POST /shop/funds/getbankbynum`

- **请求体**：`ChannelsGetBankByNumRequest` — 查询请求（`account_number` 银行卡号必填）。
- **返回**：`ChannelsGetBankByNumResponse` — 银行信息列表（`data`）与总数（`total_count`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/funds/bank/api_ecgetbankbynum

### 15. `GetFundsQrcodeAsync` — 获取二维码（`qrcode/get`，`qrcode_buf` 为 base64 编码的二维码二进制，需 base64 解码）。

- **声明接口**：[IChannelsFundsService](./IChannelsFundsService.cs)
- **官方契约**：`POST /shop/funds/qrcode/get`

- **请求体**：`ChannelsQrcodeTicketRequest` — 查询请求（`qrcode_ticket` 必填，可从商户提现接口获取）。
- **返回**：`ChannelsGetFundsQrcodeResponse` — 二维码二进制（base64 编码字符串）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/funds/qrcode/api_ecgetqrcode
- 官方业务限制：**不支持云调用**；官方错误码：`-2`（token 太长）/ `60208`（错误的 ticket）/ `60220`（ticket 已失效）。
- 第三方平台代调用所属权限集 id 为 `85`、`116`、`138`。

### 16. `CheckFundsQrcodeAsync` — 查询扫码状态（`qrcode/check`）。

- **声明接口**：[IChannelsFundsService](./IChannelsFundsService.cs)
- **官方契约**：`POST /shop/funds/qrcode/check`

- **请求体**：`ChannelsQrcodeTicketRequest` — 查询请求（`qrcode_ticket` 必填，可从商户提现接口获取）。
- **返回**：`ChannelsCheckFundsQrcodeResponse` — 扫码状态（`status`/`self_check_err_code`/`self_check_err_msg`/`scan_user_type`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/funds/qrcode/api_eccheckqrcode
- 官方业务限制：**不支持云调用**；扫码状态枚举见 `ChannelsFundsQrcodeStatuses`， / 扫码者身份枚举见 `ChannelsFundsScanUserTypes`。
- 第三方平台代调用所属权限集 id 为 `85`、`116`、`138`。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Channels/DataModels/Funds`](../../DataModels/Funds) 内共 **42** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **42** 个，`SerializerClassName` = `Funds`。

AOT 源生成上下文：[`FundsJsonContext.g.cs`](./../../DataModels/Generated/FundsJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ChannelsGetBalanceResponse` | 应答 | [Funds/Flow/ChannelsGetBalanceResponse.cs](../../DataModels/Funds/Flow/ChannelsGetBalanceResponse.cs) | `available_amount`, `pending_amount`, `sub_mchid` |
| `ChannelsGetBankAcctResponse` | 应答 | [Funds/SettleAccount/ChannelsGetBankAcctResponse.cs](../../DataModels/Funds/SettleAccount/ChannelsGetBankAcctResponse.cs) | `account_info` |
| `ChannelsResponse` | 应答 | [ChannelsResponse.cs](../../DataModels/ChannelsResponse.cs) | `errcode`, `errmsg` |
| `ChannelsSetBankAcctRequest` | 请求体 | [Funds/SettleAccount/ChannelsSetBankAcctRequest.cs](../../DataModels/Funds/SettleAccount/ChannelsSetBankAcctRequest.cs) | `account_info` |
| `ChannelsSubmitWithdrawResponse` | 应答 | [Funds/Withdraw/ChannelsSubmitWithdrawResponse.cs](../../DataModels/Funds/Withdraw/ChannelsSubmitWithdrawResponse.cs) | `withdraw_id`, `qrcode_ticket` |
| `ChannelsSubmitWithdrawRequest` | 请求体 | [Funds/Withdraw/ChannelsSubmitWithdrawRequest.cs](../../DataModels/Funds/Withdraw/ChannelsSubmitWithdrawRequest.cs) | `amount`, `remark`, `bank_memo` |
| `ChannelsGetWithdrawListResponse` | 应答 | [Funds/Withdraw/ChannelsGetWithdrawListResponse.cs](../../DataModels/Funds/Withdraw/ChannelsGetWithdrawListResponse.cs) | `withdraw_ids`, `total_num` |
| `ChannelsGetWithdrawListRequest` | 请求体 | [Funds/Withdraw/ChannelsGetWithdrawListRequest.cs](../../DataModels/Funds/Withdraw/ChannelsGetWithdrawListRequest.cs) | `page_num`, `page_size`, `start_time`, `end_time` |
| `ChannelsGetWithdrawDetailResponse` | 应答 | [Funds/Withdraw/ChannelsGetWithdrawDetailResponse.cs](../../DataModels/Funds/Withdraw/ChannelsGetWithdrawDetailResponse.cs) | `amount`, `create_time`, `update_time`, `reason`, `remark`, `bank_memo`, `bank_name`, `bank_num`, `status` |
| `ChannelsGetWithdrawDetailRequest` | 请求体 | [Funds/Withdraw/ChannelsGetWithdrawDetailRequest.cs](../../DataModels/Funds/Withdraw/ChannelsGetWithdrawDetailRequest.cs) | `withdraw_id` |
| `ChannelsGetFundsFlowListResponse` | 应答 | [Funds/Flow/ChannelsGetFundsFlowListResponse.cs](../../DataModels/Funds/Flow/ChannelsGetFundsFlowListResponse.cs) | `flow_ids`, `has_more`, `next_key` |
| `ChannelsGetFundsFlowListRequest` | 请求体 | [Funds/Flow/ChannelsGetFundsFlowListRequest.cs](../../DataModels/Funds/Flow/ChannelsGetFundsFlowListRequest.cs) | `page`, `page_size`, `next_key`, `start_time`, `end_time`, `transaction_id` |
| `ChannelsGetFundsFlowDetailResponse` | 应答 | [Funds/Flow/ChannelsGetFundsFlowDetailResponse.cs](../../DataModels/Funds/Flow/ChannelsGetFundsFlowDetailResponse.cs) | `funds_flow` |
| `ChannelsGetFundsFlowDetailRequest` | 请求体 | [Funds/Flow/ChannelsGetFundsFlowDetailRequest.cs](../../DataModels/Funds/Flow/ChannelsGetFundsFlowDetailRequest.cs) | `flow_id` |
| `ChannelsListOrderFlowResponse` | 应答 | [Funds/OrderFlow/ChannelsListOrderFlowResponse.cs](../../DataModels/Funds/OrderFlow/ChannelsListOrderFlowResponse.cs) | `total_count`, `data_list`, `page_ctx` |
| `ChannelsListOrderFlowRequest` | 请求体 | [Funds/OrderFlow/ChannelsListOrderFlowRequest.cs](../../DataModels/Funds/OrderFlow/ChannelsListOrderFlowRequest.cs) | `order_settle_state`, `order_state`, `order_pay_method`, `order_id`, `pagination_info`, `create_time_range` |
| `ChannelsGetCityResponse` | 应答 | [Funds/BankDict/ChannelsGetCityResponse.cs](../../DataModels/Funds/BankDict/ChannelsGetCityResponse.cs) | `data`, `total_count` |
| `ChannelsGetCityRequest` | 请求体 | [Funds/BankDict/ChannelsGetCityRequest.cs](../../DataModels/Funds/BankDict/ChannelsGetCityRequest.cs) | `province_code` |
| `ChannelsGetProvinceResponse` | 应答 | [Funds/BankDict/ChannelsGetProvinceResponse.cs](../../DataModels/Funds/BankDict/ChannelsGetProvinceResponse.cs) | `data`, `total_count` |
| `ChannelsGetBankListResponse` | 应答 | [Funds/BankDict/ChannelsGetBankListResponse.cs](../../DataModels/Funds/BankDict/ChannelsGetBankListResponse.cs) | `data` |
| `ChannelsGetBankListRequest` | 请求体 | [Funds/BankDict/ChannelsGetBankListRequest.cs](../../DataModels/Funds/BankDict/ChannelsGetBankListRequest.cs) | `offset`, `limit`, `key_words`, `bank_type` |
| `ChannelsGetSubBranchResponse` | 应答 | [Funds/BankDict/ChannelsGetSubBranchResponse.cs](../../DataModels/Funds/BankDict/ChannelsGetSubBranchResponse.cs) | `data`, `total_count`, `count`, `account_bank_code`, `bank_alias`, `bank_alias_code`, `account_bank` |
| `ChannelsGetSubBranchRequest` | 请求体 | [Funds/BankDict/ChannelsGetSubBranchRequest.cs](../../DataModels/Funds/BankDict/ChannelsGetSubBranchRequest.cs) | `bank_code`, `city_code`, `offset`, `limit` |
| `ChannelsGetBankByNumResponse` | 应答 | [Funds/BankDict/ChannelsGetBankByNumResponse.cs](../../DataModels/Funds/BankDict/ChannelsGetBankByNumResponse.cs) | `data`, `total_count` |
| `ChannelsGetBankByNumRequest` | 请求体 | [Funds/BankDict/ChannelsGetBankByNumRequest.cs](../../DataModels/Funds/BankDict/ChannelsGetBankByNumRequest.cs) | `account_number` |
| `ChannelsGetFundsQrcodeResponse` | 应答 | [Funds/Qrcode/ChannelsGetFundsQrcodeResponse.cs](../../DataModels/Funds/Qrcode/ChannelsGetFundsQrcodeResponse.cs) | `qrcode_buf` |
| `ChannelsQrcodeTicketRequest` | 请求体 | [Funds/Qrcode/ChannelsQrcodeTicketRequest.cs](../../DataModels/Funds/Qrcode/ChannelsQrcodeTicketRequest.cs) | `qrcode_ticket` |
| `ChannelsCheckFundsQrcodeResponse` | 应答 | [Funds/Qrcode/ChannelsCheckFundsQrcodeResponse.cs](../../DataModels/Funds/Qrcode/ChannelsCheckFundsQrcodeResponse.cs) | `status`, `self_check_err_code`, `self_check_err_msg`, `scan_user_type` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Channels;
using Mud.Wechat.Channels.DataModels.Funds;

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddFundsApi());

var service = app.Services.GetRequiredService<IChannelsFundsService>();
var ct = CancellationToken.None;
var response = await service.GetBalanceAsync(ct);
// 业务失败判定：WechatChannelsException.ThrowIfFailed(response, "/channels/ec/funds/getbalance");
// 占位值仅用于编译示意；业务值须按 IChannelsFundsService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetBalanceAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IChannelsFundsService](./IChannelsFundsService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`ChannelsFundsContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsFundsContractGuards.cs) |
| 守卫中的代表性测试方法 | `FundsEndpoints_ShouldMatchOfficialRoutes`、`FundsEndpoints_ShouldAllBePost_WithEmptyBodyRules`、`FundsInterface_ShouldBindChannelsToken_AndRegisterUnderFundsGroup`、`FundsDataModels_ShouldBeRegisteredInJsonContext`、`FundsDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`FundsErrorCodes_ShouldMatchVerifiedOfficialValues`、`FundsEnumConstants_ShouldLockOfficialLiteralValues`、`LeagueFundsPrefix_ShouldHaveZeroDeclaration_WhenPageRemovedFromOfficialCatalog` |
| 模块枚举 / 装配入口 | [`ChannelsModule.cs`](../../Extensions/ChannelsModule.cs)、[`ChannelsServiceBuilder.cs`](../../Extensions/ChannelsServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Channels/DataModels/Funds`](../../DataModels/Funds) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Channels/Mud.Wechat.Channels/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`ChannelsFundsContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `Wechat.Channels.AccessToken`；接口平铺在 `Mud.Wechat.Channels`，不得声明 `IsAbstract` 父接口（CH-X2）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
