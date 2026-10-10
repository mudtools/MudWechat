// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Funds;

/// <summary>
/// 获取账户余额（<c>getbalance</c>）响应。
/// </summary>
/// <remarks>
/// 官方契约：<b>POST</b> + 空请求体（官方原文「调用接口时传空的json串即可」）；
/// 金额字段单位均为<b>分</b>。所属权限集 id：138（第三方平台代小店商家调用）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetBalanceResponse : ChannelsResponse
{
    /// <summary>获取或设置可提现余额（官方 <c>available_amount</c>，单位：分）。</summary>
    [JsonPropertyName("available_amount")]
    public long? AvailableAmount { get; set; }

    /// <summary>获取或设置待结算余额（官方 <c>pending_amount</c>，单位：分）。</summary>
    [JsonPropertyName("pending_amount")]
    public long? PendingAmount { get; set; }

    /// <summary>获取或设置二级商户号（官方 <c>sub_mchid</c>）。</summary>
    [JsonPropertyName("sub_mchid")]
    public string? SubMchId { get; set; }
}

/// <summary>
/// 获取结算账户（<c>getbankacct</c>）响应。
/// </summary>
/// <remarks>
/// 官方契约：<b>POST</b> + 空请求体（官方原文「调用接口时传空的json串即可」）。
/// 所属权限集 id：138。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetBankAcctResponse : ChannelsResponse
{
    /// <summary>获取或设置结算账户信息（官方 <c>account_info</c>）。</summary>
    [JsonPropertyName("account_info")]
    public ChannelsSettleAccountInfo? AccountInfo { get; set; }
}

/// <summary>结算账户信息（官方 <c>account_info</c> 对象，<c>getbankacct</c> 返回形态）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsSettleAccountInfo
{
    /// <summary>获取或设置账户类型（官方 <c>bank_account_type</c>，见 <see cref="ChannelsBankAccountTypes"/>）。</summary>
    [JsonPropertyName("bank_account_type")]
    public string? BankAccountType { get; set; }

    /// <summary>获取或设置开户银行（官方 <c>account_bank</c>）。</summary>
    [JsonPropertyName("account_bank")]
    public string? AccountBank { get; set; }

    /// <summary>获取或设置开户银行省市编码（官方 <c>bank_address_code</c>）。</summary>
    [JsonPropertyName("bank_address_code")]
    public string? BankAddressCode { get; set; }

    /// <summary>获取或设置开户银行联行号（官方 <c>bank_branch_id</c>）。</summary>
    [JsonPropertyName("bank_branch_id")]
    public string? BankBranchId { get; set; }

    /// <summary>获取或设置开户银行全称（官方 <c>bank_name</c>）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>获取或设置银行账号（官方 <c>account_number</c>）。</summary>
    [JsonPropertyName("account_number")]
    public string? AccountNumber { get; set; }

    /// <summary>获取或设置账户名称（官方 <c>account_name</c>）。</summary>
    [JsonPropertyName("account_name")]
    public string? AccountName { get; set; }
}

/// <summary>修改结算账户（<c>setbankacct</c>）请求体。</summary>
/// <remarks>
/// 官方契约：<b>POST</b> + 请求体 <c>{"account_info":{…}}</c>。
/// 账户信息提交形态（必填）与获取形态不同：额外承载 <c>account_bank4show</c>，且 <c>bank_account_type</c> /
/// <c>account_bank</c> / <c>bank_address_code</c> / <c>account_number</c> 为必填。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsSetBankAcctRequest
{
    /// <summary>获取或设置待修改的结算账户信息（官方 <c>account_info</c>，必填）。</summary>
    [JsonPropertyName("account_info")]
    public ChannelsSettleAccountModify AccountInfo { get; set; } = new();
}

/// <summary>结算账户修改明细（官方 <c>account_info</c> 对象，<c>setbankacct</c> 提交形态）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsSettleAccountModify
{
    /// <summary>
    /// 获取或设置账户类型（官方 <c>bank_account_type</c>，必填）——企业类型只能绑定对公账户，
    /// 个体工商户可以绑定对公或者对私账户（见 <see cref="ChannelsBankAccountTypes"/>）。
    /// </summary>
    [JsonPropertyName("bank_account_type")]
    public string BankAccountType { get; set; } = string.Empty;

    /// <summary>获取或设置开户银行（官方 <c>account_bank</c>，必填；经「搜索银行列表」获取）。</summary>
    [JsonPropertyName("account_bank")]
    public string AccountBank { get; set; } = string.Empty;

    /// <summary>获取或设置开户银行省市编码（官方 <c>bank_address_code</c>，必填；经「获取城市列表」获取）。</summary>
    [JsonPropertyName("bank_address_code")]
    public string BankAddressCode { get; set; } = string.Empty;

    /// <summary>获取或设置开户银行联行号（官方 <c>bank_branch_id</c>，选填；经「获取支行联号」获取）。</summary>
    [JsonPropertyName("bank_branch_id")]
    public string? BankBranchId { get; set; }

    /// <summary>
    /// 获取或设置开户银行全称（官方 <c>bank_name</c>，选填）——若开户银行为「其他银行」，
    /// 则需二选一填写「开户银行全称（含支行）」或「开户银行联行号」。
    /// </summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>获取或设置银行账号（官方 <c>account_number</c>，必填）。</summary>
    [JsonPropertyName("account_number")]
    public string AccountNumber { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置开户银行名称前端展示值（官方 <c>account_bank4show</c>，选填；为空时默认用 <c>account_bank</c> 字段）。
    /// </summary>
    [JsonPropertyName("account_bank4show")]
    public string? AccountBank4Show { get; set; }

    /// <summary>
    /// 获取或设置账户名称（官方 <c>account_name</c>，选填；需要修改结算账户名称的时候传入，
    /// 对公账户需要与公司名称一致，对私账户需要与法人姓名一致）。
    /// </summary>
    [JsonPropertyName("account_name")]
    public string? AccountName { get; set; }
}

/// <summary>商户提现（<c>submitwithdraw</c>）请求体。</summary>
/// <remarks>官方契约：<b>POST</b> + 请求体；<c>amount</c> 单位：分。</remarks>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsSubmitWithdrawRequest
{
    /// <summary>获取或设置提现金额（官方 <c>amount</c>，必填，单位：分）。</summary>
    [JsonPropertyName("amount")]
    public long Amount { get; set; }

    /// <summary>获取或设置提现备注（官方 <c>remark</c>，选填）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>获取或设置银行附言（官方 <c>bank_memo</c>，选填）。</summary>
    [JsonPropertyName("bank_memo")]
    public string? BankMemo { get; set; }
}

/// <summary>商户提现（<c>submitwithdraw</c>）响应。</summary>
/// <remarks>
/// <para>官方契约：调<b>每天最多允许提现一次</b>；成功后将触发「提现回调」与「提现二维码回调」事件。</para>
/// <para>
/// <b>官方文档描述互换（照录）</b>：官方参数表把 <c>withdraw_id</c> 的说明写为「二维码ticket,可用于获取
/// 二维码和查询二维码状态」、把 <c>qrcode_ticket</c> 写为「提现单号」——径自矛盾。字段名照抄官方原文，
/// 语义以字面名为准（<c>withdraw_id</c> = 提现单号，<c>qrcode_ticket</c> = 二维码 ticket，供
/// <c>qrcode/get</c> / <c>qrcode/check</c> 使用）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsSubmitWithdrawResponse : ChannelsResponse
{
    /// <summary>获取或设置提现单号（官方 <c>withdraw_id</c>；官方说明原文照录为「二维码ticket」——字段语义以字面名为准）。</summary>
    [JsonPropertyName("withdraw_id")]
    public string? WithdrawId { get; set; }

    /// <summary>获取或设置二维码 ticket（官方 <c>qrcode_ticket</c>；官方说明原文照录为「提现单号」——字段语义以字面名为准）。</summary>
    [JsonPropertyName("qrcode_ticket")]
    public string? QrcodeTicket { get; set; }
}

/// <summary>获取提现记录列表（<c>getwithdrawlist</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetWithdrawListRequest
{
    /// <summary>获取或设置页码（官方 <c>page_num</c>，必填）。</summary>
    [JsonPropertyName("page_num")]
    public int PageNum { get; set; }

    /// <summary>获取或设置每页大小（官方 <c>page_size</c>，必填）。</summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>获取或设置开始时间（官方 <c>start_time</c>，选填，unix 时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>获取或设置结束时间（官方 <c>end_time</c>，选填，unix 时间戳）。</summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }
}

/// <summary>获取提现记录列表（<c>getwithdrawlist</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetWithdrawListResponse : ChannelsResponse
{
    /// <summary>获取或设置提现单号列表（官方 <c>withdraw_ids</c>）。</summary>
    [JsonPropertyName("withdraw_ids")]
    public List<string>? WithdrawIds { get; set; }

    /// <summary>获取或设置提现单号总数（官方 <c>total_num</c>）。</summary>
    [JsonPropertyName("total_num")]
    public int? TotalNum { get; set; }
}

/// <summary>获取提现记录（<c>getwithdrawdetail</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetWithdrawDetailRequest
{
    /// <summary>获取或设置提现单号（官方 <c>withdraw_id</c>，必填；可从获取提现记录列表接口获取）。</summary>
    [JsonPropertyName("withdraw_id")]
    public string WithdrawId { get; set; } = string.Empty;
}

/// <summary>获取提现记录（<c>getwithdrawdetail</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetWithdrawDetailResponse : ChannelsResponse
{
    /// <summary>获取或设置金额（官方 <c>amount</c>，单位：分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>获取或设置创建时间（官方 <c>create_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("create_time")]
    public long? CreateTime { get; set; }

    /// <summary>获取或设置更新时间（官方 <c>update_time</c>，unix 时间戳）。</summary>
    [JsonPropertyName("update_time")]
    public long? UpdateTime { get; set; }

    /// <summary>获取或设置失败原因（官方 <c>reason</c>）。</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>获取或设置备注（官方 <c>remark</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>获取或设置银行附言（官方 <c>bank_memo</c>）。</summary>
    [JsonPropertyName("bank_memo")]
    public string? BankMemo { get; set; }

    /// <summary>获取或设置银行名称（官方 <c>bank_name</c>）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>获取或设置银行账户（官方 <c>bank_num</c>）。</summary>
    [JsonPropertyName("bank_num")]
    public string? BankNum { get; set; }

    /// <summary>获取或设置提现状态（官方 <c>status</c>，见 <see cref="ChannelsWithdrawStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }
}

/// <summary>获取资金流水列表（<c>getfundsflowlist</c>）请求体。</summary>
/// <remarks>
/// 官方契约：当日资金流水出账时间为<b>次日 16 点后</b>，先用后付订单需用户确认收货自动付款后才会产生资金流水；
/// <c>end_time</c> 受出账时间约束（16 点前最大为前 1 天 0 点，16 点后最大为当天 0 点，超上限按上限查询）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetFundsFlowListRequest
{
    /// <summary>获取或设置页码（官方 <c>page</c>，选填，从 1 开始）。</summary>
    [JsonPropertyName("page")]
    public int? Page { get; set; }

    /// <summary>获取或设置页大小（官方 <c>page_size</c>，选填，不填默认为 10）。</summary>
    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }

    /// <summary>
    /// 获取或设置分页参数（官方 <c>next_key</c>，选填）——翻页时写入上一页返回的 <c>next_key</c>
    /// （page 为上一页加一，并且 page_size 与上一页相同的时候才生效）；<c>page * page_size &gt;= 10000</c> 时必填。
    /// </summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }

    /// <summary>获取或设置流水产生的开始时间（官方 <c>start_time</c>，选填，unix 时间戳）。</summary>
    [JsonPropertyName("start_time")]
    public long? StartTime { get; set; }

    /// <summary>
    /// 获取或设置流水产生的结束时间（官方 <c>end_time</c>，选填，unix 时间戳）——受当日 16 点出账时间约束
    /// （16 点前最大为前 1 天 0 点 0 分 0 秒，16 点后最大为当天 0 点 0 分 0 秒；超过最大值将按最大值进行查询）。
    /// </summary>
    [JsonPropertyName("end_time")]
    public long? EndTime { get; set; }

    /// <summary>获取或设置支付单号（官方 <c>transaction_id</c>，选填）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }
}

/// <summary>获取资金流水列表（<c>getfundsflowlist</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetFundsFlowListResponse : ChannelsResponse
{
    /// <summary>获取或设置流水单号列表（官方 <c>flow_ids</c>）。</summary>
    [JsonPropertyName("flow_ids")]
    public List<string>? FlowIds { get; set; }

    /// <summary>获取或设置是否还有下一页（官方 <c>has_more</c>）。</summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }

    /// <summary>获取或设置分页参数（官方 <c>next_key</c>，深翻页时使用）。</summary>
    [JsonPropertyName("next_key")]
    public string? NextKey { get; set; }
}

/// <summary>获取资金流水详情（<c>getfundsflowdetail</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetFundsFlowDetailRequest
{
    /// <summary>获取或设置流水 id（官方 <c>flow_id</c>，必填；可通过获取资金流水列表获取）。</summary>
    [JsonPropertyName("flow_id")]
    public string FlowId { get; set; } = string.Empty;
}

/// <summary>获取资金流水详情（<c>getfundsflowdetail</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetFundsFlowDetailResponse : ChannelsResponse
{
    /// <summary>获取或设置流水信息（官方 <c>funds_flow</c>）。</summary>
    [JsonPropertyName("funds_flow")]
    public ChannelsFundsFlow? FundsFlow { get; set; }
}

/// <summary>资金流水信息（官方 <c>funds_flow</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsFundsFlow
{
    /// <summary>获取或设置流水 id（官方 <c>flow_id</c>）。</summary>
    [JsonPropertyName("flow_id")]
    public string? FlowId { get; set; }

    /// <summary>获取或设置资金类型（官方 <c>funds_type</c>，见 <see cref="ChannelsFundsTypes"/>）。</summary>
    [JsonPropertyName("funds_type")]
    public int? FundsType { get; set; }

    /// <summary>获取或设置流水类型（官方 <c>flow_type</c>：1 收入，2 支出）。</summary>
    [JsonPropertyName("flow_type")]
    public int? FlowType { get; set; }

    /// <summary>获取或设置流水金额（官方 <c>amount</c>，单位：分）。</summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>获取或设置余额（官方 <c>balance</c>，单位：分）。</summary>
    [JsonPropertyName("balance")]
    public long? Balance { get; set; }

    /// <summary>获取或设置流水关联信息列表（官方 <c>related_info_list</c>）。</summary>
    [JsonPropertyName("related_info_list")]
    public List<ChannelsFundsFlowRelatedInfo>? RelatedInfoList { get; set; }

    /// <summary>获取或设置记账时间（官方 <c>bookkeeping_time</c>）。</summary>
    [JsonPropertyName("bookkeeping_time")]
    public string? BookkeepingTime { get; set; }

    /// <summary>获取或设置备注（官方 <c>remark</c>）。</summary>
    [JsonPropertyName("remark")]
    public string? Remark { get; set; }

    /// <summary>获取或设置资金类型描述（官方 <c>funds_type_desc</c>）。</summary>
    [JsonPropertyName("funds_type_desc")]
    public string? FundsTypeDesc { get; set; }
}

/// <summary>资金流水关联信息（官方 <c>related_info_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsFundsFlowRelatedInfo
{
    /// <summary>
    /// 获取或设置关联类型（官方 <c>related_type</c>）：1 订单，2 售后，3 提现，4 运费险，5 服务保障，
    /// 6 送礼物，7 群送礼关联订单号列表，8 同城配送门店 id。
    /// </summary>
    [JsonPropertyName("related_type")]
    public int? RelatedType { get; set; }

    /// <summary>获取或设置关联订单号（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置关联售后单号（官方 <c>aftersale_id</c>）。</summary>
    [JsonPropertyName("aftersale_id")]
    public string? AftersaleId { get; set; }

    /// <summary>获取或设置关联提现单号（官方 <c>withdraw_id</c>）。</summary>
    [JsonPropertyName("withdraw_id")]
    public string? WithdrawId { get; set; }

    /// <summary>获取或设置记账时间（官方 <c>bookkeeping_time</c>）。</summary>
    [JsonPropertyName("bookkeeping_time")]
    public string? BookkeepingTime { get; set; }

    /// <summary>获取或设置关联运费险单号（官方 <c>insurance_id</c>）。</summary>
    [JsonPropertyName("insurance_id")]
    public string? InsuranceId { get; set; }

    /// <summary>获取或设置关联支付单号（官方 <c>transaction_id</c>）。</summary>
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; set; }

    /// <summary>获取或设置关联保障单号（官方 <c>guarantee_id</c>）。</summary>
    [JsonPropertyName("guarantee_id")]
    public long? GuaranteeId { get; set; }

    /// <summary>获取或设置关联礼物单号（官方 <c>present_id</c>）。</summary>
    [JsonPropertyName("present_id")]
    public long? PresentId { get; set; }

    /// <summary>
    /// 获取或设置群送礼关联订单号列表（官方 <c>group_present_sub_order_id_list</c>）——
    /// 群礼物与订单之间是一对多关系，多个订单号通过「,」来分隔（官方类型为 string）。
    /// </summary>
    [JsonPropertyName("group_present_sub_order_id_list")]
    public string? GroupPresentSubOrderIdList { get; set; }

    /// <summary>获取或设置同城配送门店 id（官方 <c>intra_city_shop_id</c>）。</summary>
    [JsonPropertyName("intra_city_shop_id")]
    public long? IntraCityShopId { get; set; }
}

/// <summary>查询订单流水列表（<c>listorderflow</c>）请求体。</summary>
/// <remarks>
/// 官方契约：<c>order_settle_state</c> 必填；分页形态为 <c>pagination_info</c>（支持分页上下文，
/// 可保证翻页过程订单不遗漏不重复）。所有子项目的结算状态均与 <c>order_settle_state</c> 相同（官方注意事项原文）。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsListOrderFlowRequest
{
    /// <summary>获取或设置订单结算状态（官方 <c>order_settle_state</c>，必填，见 <see cref="ChannelsOrderSettleStates"/>）。</summary>
    [JsonPropertyName("order_settle_state")]
    public int OrderSettleState { get; set; }

    /// <summary>获取或设置订单状态（官方 <c>order_state</c>，选填，见 <see cref="ChannelsOrderFlowOrderStates"/>）。</summary>
    [JsonPropertyName("order_state")]
    public int? OrderState { get; set; }

    /// <summary>获取或设置订单支付方式（官方 <c>order_pay_method</c>，选填，见 <see cref="ChannelsOrderFlowPayMethods"/>）。</summary>
    [JsonPropertyName("order_pay_method")]
    public int? OrderPayMethod { get; set; }

    /// <summary>获取或设置指定订单 id 查询（官方 <c>order_id</c>，选填）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置分页信息（官方 <c>pagination_info</c>，选填）。</summary>
    [JsonPropertyName("pagination_info")]
    public ChannelsOrderFlowPagination? PaginationInfo { get; set; }

    /// <summary>获取或设置订单创建时间范围（官方 <c>create_time_range</c>，选填）。</summary>
    /// <remarks><b>官方文档矛盾照录</b>：参数表字段名为 <c>create_time_range</c>，代码示例写作
    /// <c>order_create_time_range</c>——SDK 按参数表建模。</remarks>
    [JsonPropertyName("create_time_range")]
    public ChannelsOrderFlowTimeRange? CreateTimeRange { get; set; }
}

/// <summary>订单流水分页信息（官方 <c>pagination_info</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsOrderFlowPagination
{
    /// <summary>获取或设置页大小（官方 <c>limit</c>，必填）。</summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    /// <summary>获取或设置偏移量（官方 <c>offset</c>，选填）。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>
    /// 获取或设置是否使用分页上下文（官方 <c>use_page_ctx</c>，选填）——可保证翻页的过程中订单不会遗漏和重复，
    /// 此时 <c>offset</c> 不生效。
    /// </summary>
    [JsonPropertyName("use_page_ctx")]
    public bool? UsePageCtx { get; set; }

    /// <summary>获取或设置分页上下文（官方 <c>page_ctx</c>，选填）——每次翻页传入上一页回参的 <c>page_ctx</c>，第一页可不传或者传空字符串。</summary>
    [JsonPropertyName("page_ctx")]
    public string? PageCtx { get; set; }
}

/// <summary>订单创建时间范围（官方 <c>create_time_range</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsOrderFlowTimeRange
{
    /// <summary>获取或设置订单创建时间开始（官方 <c>begin</c>，选填，闭区间）。</summary>
    [JsonPropertyName("begin")]
    public long? Begin { get; set; }

    /// <summary>获取或设置订单创建时间结束（官方 <c>end</c>，选填，开区间）。</summary>
    [JsonPropertyName("end")]
    public long? End { get; set; }
}

/// <summary>查询订单流水列表（<c>listorderflow</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsListOrderFlowResponse : ChannelsResponse
{
    /// <summary>获取或设置满足条件的总数量（官方 <c>total_count</c>，仅供参考，翻页的过程可能变动）。</summary>
    [JsonPropertyName("total_count")]
    public int? TotalCount { get; set; }

    /// <summary>获取或设置订单流水列表（官方 <c>data_list</c>）。</summary>
    [JsonPropertyName("data_list")]
    public List<ChannelsOrderFlowItem>? DataList { get; set; }

    /// <summary>获取或设置分页上下文（官方 <c>page_ctx</c>，使用分页上下文时返回此字段）。</summary>
    [JsonPropertyName("page_ctx")]
    public string? PageCtx { get; set; }
}

/// <summary>订单流水条目（官方 <c>data_list</c> 数组元素）。</summary>
/// <remarks>金额字段单位均为<b>分</b>；结算/时间字段带「(预计)」前缀的为预估口径（官方原文照录）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsOrderFlowItem
{
    /// <summary>获取或设置订单 id（官方 <c>order_id</c>）。</summary>
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    /// <summary>获取或设置订单状态（官方 <c>order_state</c>）。</summary>
    [JsonPropertyName("order_state")]
    public int? OrderState { get; set; }

    /// <summary>获取或设置订单结算状态（官方 <c>order_settle_state</c>）。</summary>
    [JsonPropertyName("order_settle_state")]
    public int? OrderSettleState { get; set; }

    /// <summary>获取或设置订单创建时间（官方 <c>order_create_time</c>，单位：秒）。</summary>
    [JsonPropertyName("order_create_time")]
    public long? OrderCreateTime { get; set; }

    /// <summary>获取或设置订单支付时间（官方 <c>order_paid_time</c>，单位：秒）。</summary>
    [JsonPropertyName("order_paid_time")]
    public long? OrderPaidTime { get; set; }

    /// <summary>获取或设置订单支付方式（官方 <c>order_pay_method</c>）。</summary>
    [JsonPropertyName("order_pay_method")]
    public int? OrderPayMethod { get; set; }

    /// <summary>获取或设置订单类型（官方 <c>order_type</c>）。</summary>
    [JsonPropertyName("order_type")]
    public int? OrderType { get; set; }

    /// <summary>获取或设置商户实收金额（官方 <c>mch_received_amount</c>，单位：分）。</summary>
    [JsonPropertyName("mch_received_amount")]
    public long? MchReceivedAmount { get; set; }

    /// <summary>获取或设置支出金额（官方 <c>expense_amount</c>，单位：分）。</summary>
    [JsonPropertyName("expense_amount")]
    public long? ExpenseAmount { get; set; }

    /// <summary>获取或设置（预计）结算金额（官方 <c>mch_settle_amount</c>，单位：分）。</summary>
    [JsonPropertyName("mch_settle_amount")]
    public long? MchSettleAmount { get; set; }

    /// <summary>获取或设置（预计）商家货款结算时间（官方 <c>mch_settle_time</c>）。</summary>
    [JsonPropertyName("mch_settle_time")]
    public long? MchSettleTime { get; set; }

    /// <summary>获取或设置商品列表（官方 <c>product_list</c>）。</summary>
    [JsonPropertyName("product_list")]
    public List<ChannelsOrderFlowProduct>? ProductList { get; set; }

    /// <summary>获取或设置商品总金额（官方 <c>product_total_amount</c>，单位：分）。</summary>
    [JsonPropertyName("product_total_amount")]
    public long? ProductTotalAmount { get; set; }

    /// <summary>获取或设置运费金额（官方 <c>freight_amount</c>，单位：分）。</summary>
    [JsonPropertyName("freight_amount")]
    public long? FreightAmount { get; set; }

    /// <summary>获取或设置改价金额（官方 <c>change_down_price</c>，单位：分）。</summary>
    [JsonPropertyName("change_down_price")]
    public long? ChangeDownPrice { get; set; }

    /// <summary>获取或设置商户优惠金额（官方 <c>mch_discount_amount</c>，单位：分）。</summary>
    [JsonPropertyName("mch_discount_amount")]
    public long? MchDiscountAmount { get; set; }

    /// <summary>获取或设置积分抵扣金额（官方 <c>score_discount_amount</c>，单位：分）。</summary>
    [JsonPropertyName("score_discount_amount")]
    public long? ScoreDiscountAmount { get; set; }

    /// <summary>获取或设置用户实付金额（官方 <c>buyer_paid_amount</c>，单位：分）。</summary>
    [JsonPropertyName("buyer_paid_amount")]
    public long? BuyerPaidAmount { get; set; }

    /// <summary>获取或设置达人优惠金额（官方 <c>promoter_discount_amount</c>，单位：分）。</summary>
    [JsonPropertyName("promoter_discount_amount")]
    public long? PromoterDiscountAmount { get; set; }

    /// <summary>获取或设置平台优惠金额（官方 <c>platform_discount_amount</c>，单位：分）。</summary>
    [JsonPropertyName("platform_discount_amount")]
    public long? PlatformDiscountAmount { get; set; }

    /// <summary>获取或设置国家补贴金额（官方 <c>national_subsidy_discount_amount</c>，单位：分）。</summary>
    [JsonPropertyName("national_subsidy_discount_amount")]
    public long? NationalSubsidyDiscountAmount { get; set; }

    /// <summary>获取或设置补交运费（官方 <c>freight_make_up_amount</c>，单位：分）。</summary>
    [JsonPropertyName("freight_make_up_amount")]
    public long? FreightMakeUpAmount { get; set; }

    /// <summary>获取或设置跨店优惠金额（官方 <c>cross_shop_discount_amount</c>，单位：分）。</summary>
    [JsonPropertyName("cross_shop_discount_amount")]
    public long? CrossShopDiscountAmount { get; set; }

    /// <summary>获取或设置用户退款金额（官方 <c>buyer_refund_amount</c>，单位：分）。</summary>
    [JsonPropertyName("buyer_refund_amount")]
    public long? BuyerRefundAmount { get; set; }

    /// <summary>获取或设置（预计）平台优惠退款金额（官方 <c>platform_discount_refund_amount</c>，单位：分）。</summary>
    [JsonPropertyName("platform_discount_refund_amount")]
    public long? PlatformDiscountRefundAmount { get; set; }

    /// <summary>获取或设置达人优惠退款金额（官方 <c>promoter_discount_refund_amount</c>，单位：分）。</summary>
    [JsonPropertyName("promoter_discount_refund_amount")]
    public long? PromoterDiscountRefundAmount { get; set; }

    /// <summary>获取或设置原技术服务费（官方 <c>original_platform_commission_amount</c>，单位：分）。</summary>
    [JsonPropertyName("original_platform_commission_amount")]
    public long? OriginalPlatformCommissionAmount { get; set; }

    /// <summary>获取或设置（预计）技术服务费（官方 <c>platform_commission_amount</c>，单位：分）。</summary>
    [JsonPropertyName("platform_commission_amount")]
    public long? PlatformCommissionAmount { get; set; }

    /// <summary>获取或设置运费险补贴减免技术服务费（官方 <c>freight_insurance_subsidy_amount</c>，单位：分）。</summary>
    [JsonPropertyName("freight_insurance_subsidy_amount")]
    public long? FreightInsuranceSubsidyAmount { get; set; }

    /// <summary>获取或设置（预计）机构服务费（官方 <c>supplier_commission_amount</c>，单位：分）。</summary>
    [JsonPropertyName("supplier_commission_amount")]
    public long? SupplierCommissionAmount { get; set; }

    /// <summary>获取或设置机构服务费结算状态（官方 <c>supplier_commission_settle_state</c>）。</summary>
    [JsonPropertyName("supplier_commission_settle_state")]
    public int? SupplierCommissionSettleState { get; set; }

    /// <summary>获取或设置（预计）达人服务费（官方 <c>promoter_commission_amount</c>，单位：分）。</summary>
    [JsonPropertyName("promoter_commission_amount")]
    public long? PromoterCommissionAmount { get; set; }

    /// <summary>获取或设置达人服务费结算状态（官方 <c>promoter_commission_settle_state</c>）。</summary>
    [JsonPropertyName("promoter_commission_settle_state")]
    public int? PromoterCommissionSettleState { get; set; }

    /// <summary>获取或设置（预计）运费险金额（官方 <c>freight_insurance_amount</c>，单位：分）。</summary>
    [JsonPropertyName("freight_insurance_amount")]
    public long? FreightInsuranceAmount { get; set; }

    /// <summary>获取或设置运费险结算状态（官方 <c>freight_insurance_settle_state</c>）。</summary>
    [JsonPropertyName("freight_insurance_settle_state")]
    public int? FreightInsuranceSettleState { get; set; }

    /// <summary>获取或设置运费险补缴他单金额（官方 <c>freight_insurance_make_up_amount</c>，单位：分）。</summary>
    [JsonPropertyName("freight_insurance_make_up_amount")]
    public long? FreightInsuranceMakeUpAmount { get; set; }

    /// <summary>获取或设置运费险补缴他单订单 id 列表（官方 <c>freight_insurance_make_up_order_id_list</c>）。</summary>
    [JsonPropertyName("freight_insurance_make_up_order_id_list")]
    public List<string>? FreightInsuranceMakeUpOrderIdList { get; set; }

    /// <summary>获取或设置（预计）技术服务费结算时间（官方 <c>platform_commission_settle_time</c>，单位：秒）。</summary>
    [JsonPropertyName("platform_commission_settle_time")]
    public long? PlatformCommissionSettleTime { get; set; }

    /// <summary>获取或设置（预计）达人服务费结算时间（官方 <c>promoter_commission_settle_time</c>，单位：秒）。</summary>
    [JsonPropertyName("promoter_commission_settle_time")]
    public long? PromoterCommissionSettleTime { get; set; }

    /// <summary>获取或设置（预计）机构服务费结算时间（官方 <c>supplier_commission_settle_time</c>，单位：秒）。</summary>
    [JsonPropertyName("supplier_commission_settle_time")]
    public long? SupplierCommissionSettleTime { get; set; }

    /// <summary>获取或设置（预计）运费险结算时间（官方 <c>freight_insurance_settle_time</c>）。</summary>
    [JsonPropertyName("freight_insurance_settle_time")]
    public long? FreightInsuranceSettleTime { get; set; }

    /// <summary>获取或设置（预计）运费险补缴他单结算时间（官方 <c>freight_insurance_make_up_settle_time</c>）。</summary>
    [JsonPropertyName("freight_insurance_make_up_settle_time")]
    public long? FreightInsuranceMakeUpSettleTime { get; set; }

    /// <summary>获取或设置预付运费退回金额（官方 <c>pre_freight_refund_amount</c>，单位：分）。</summary>
    [JsonPropertyName("pre_freight_refund_amount")]
    public long? PreFreightRefundAmount { get; set; }

    /// <summary>获取或设置结算后支出（官方 <c>post_settlement_expense</c>）。</summary>
    [JsonPropertyName("post_settlement_expense")]
    public ChannelsOrderFlowPostSettlementExpense? PostSettlementExpense { get; set; }

    /// <summary>获取或设置结算前退款（官方 <c>refund_before_settlement</c>，单位：分）。</summary>
    [JsonPropertyName("refund_before_settlement")]
    public long? RefundBeforeSettlement { get; set; }

    /// <summary>获取或设置其他支出（官方 <c>other_expense_amount</c>，单位：分）。</summary>
    [JsonPropertyName("other_expense_amount")]
    public long? OtherExpenseAmount { get; set; }

    /// <summary>获取或设置平台服务费结算状态（官方 <c>platform_commission_settle_state</c>）。</summary>
    [JsonPropertyName("platform_commission_settle_state")]
    public int? PlatformCommissionSettleState { get; set; }

    /// <summary>获取或设置运费险补缴他单结算状态（官方 <c>freight_insurance_make_up_settle_state</c>）。</summary>
    [JsonPropertyName("freight_insurance_make_up_settle_state")]
    public int? FreightInsuranceMakeUpSettleState { get; set; }

    /// <summary>获取或设置同城配送门店 id（官方 <c>intra_city_shop_id</c>）。</summary>
    [JsonPropertyName("intra_city_shop_id")]
    public long? IntraCityShopId { get; set; }

    /// <summary>获取或设置供货商平台结算信息（官方 <c>supplier_platform_commission</c>）。</summary>
    [JsonPropertyName("supplier_platform_commission")]
    public ChannelsOrderFlowSupplierPlatformCommission? SupplierPlatformCommission { get; set; }

    /// <summary>获取或设置首次商家货款结算时间（官方 <c>first_mch_settle_time</c>，单位：秒）。</summary>
    [JsonPropertyName("first_mch_settle_time")]
    public long? FirstMchSettleTime { get; set; }
}

/// <summary>订单流水商品（官方 <c>product_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsOrderFlowProduct
{
    /// <summary>获取或设置商品 id（官方 <c>product_id</c>）。</summary>
    [JsonPropertyName("product_id")]
    public long? ProductId { get; set; }

    /// <summary>获取或设置商品规格列表（官方 <c>param_list</c>）。</summary>
    [JsonPropertyName("param_list")]
    public List<ChannelsOrderFlowProductParam>? ParamList { get; set; }

    /// <summary>获取或设置商品销售价格（官方 <c>sale_price</c>，单位：分）。</summary>
    [JsonPropertyName("sale_price")]
    public long? SalePrice { get; set; }

    /// <summary>获取或设置商品数量（官方 <c>count</c>）。</summary>
    [JsonPropertyName("count")]
    public int? Count { get; set; }

    /// <summary>获取或设置商品名称（官方 <c>product_name</c>）。</summary>
    [JsonPropertyName("product_name")]
    public string? ProductName { get; set; }

    /// <summary>获取或设置是否赠品（官方 <c>is_gift</c>）。</summary>
    [JsonPropertyName("is_gift")]
    public bool? IsGift { get; set; }
}

/// <summary>订单流水商品规格（官方 <c>param_list</c> 数组元素）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsOrderFlowProductParam
{
    /// <summary>获取或设置规格名称（官方 <c>key</c>）。</summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>获取或设置规格值（官方 <c>value</c>）。</summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

/// <summary>结算后支出（官方 <c>post_settlement_expense</c> 对象）。</summary>
/// <remarks>金额字段单位均为<b>分</b>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsOrderFlowPostSettlementExpense
{
    /// <summary>获取或设置买家退款（官方 <c>buyer_refund_amount</c>，单位：分）。</summary>
    [JsonPropertyName("buyer_refund_amount")]
    public long? BuyerRefundAmount { get; set; }

    /// <summary>获取或设置平台优惠退款金额（官方 <c>platform_discount_refund_amount</c>，单位：分）。</summary>
    [JsonPropertyName("platform_discount_refund_amount")]
    public long? PlatformDiscountRefundAmount { get; set; }

    /// <summary>获取或设置达人退款金额（官方 <c>promoter_refund_amount</c>，单位：分）。</summary>
    [JsonPropertyName("promoter_refund_amount")]
    public long? PromoterRefundAmount { get; set; }

    /// <summary>获取或设置运费险保费（官方 <c>freight_insurance_make_up_amount</c>，单位：分）。</summary>
    [JsonPropertyName("freight_insurance_make_up_amount")]
    public long? FreightInsuranceMakeUpAmount { get; set; }

    /// <summary>获取或设置运费险补缴本单结算状态（官方 <c>freight_insurance_make_up_settle_state</c>）。</summary>
    [JsonPropertyName("freight_insurance_make_up_settle_state")]
    public int? FreightInsuranceMakeUpSettleState { get; set; }

    /// <summary>获取或设置运费险补缴本单的订单 id（官方 <c>freight_insurance_make_up_order_id</c>）。</summary>
    [JsonPropertyName("freight_insurance_make_up_order_id")]
    public long? FreightInsuranceMakeUpOrderId { get; set; }
}

/// <summary>供货商平台结算信息（官方 <c>supplier_platform_commission</c> 对象）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsOrderFlowSupplierPlatformCommission
{
    /// <summary>获取或设置（预计）结算金额（官方 <c>settle_amount</c>，单位：分）。</summary>
    [JsonPropertyName("settle_amount")]
    public long? SettleAmount { get; set; }

    /// <summary>获取或设置（预计）结算时间（官方 <c>settle_time</c>，单位：秒）。</summary>
    [JsonPropertyName("settle_time")]
    public long? SettleTime { get; set; }

    /// <summary>获取或设置结算状态（官方 <c>settle_state</c>）。</summary>
    [JsonPropertyName("settle_state")]
    public int? SettleState { get; set; }
}

/// <summary>查询城市列表（<c>getcity</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetCityRequest
{
    /// <summary>获取或设置省份编码（官方 <c>province_code</c>，必填；经「查询大陆银行省份列表」获取）。</summary>
    [JsonPropertyName("province_code")]
    public int ProvinceCode { get; set; }
}

/// <summary>查询城市列表（<c>getcity</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetCityResponse : ChannelsResponse
{
    /// <summary>获取或设置城市信息列表（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public List<ChannelsCity>? Data { get; set; }

    /// <summary>获取或设置总数（官方 <c>total_count</c>）。</summary>
    [JsonPropertyName("total_count")]
    public int? TotalCount { get; set; }
}

/// <summary>城市信息（官方 <c>data</c> 数组元素，<c>getcity</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsCity
{
    /// <summary>获取或设置城市名称（官方 <c>city_name</c>）。</summary>
    [JsonPropertyName("city_name")]
    public string? CityName { get; set; }

    /// <summary>获取或设置城市编号（官方 <c>city_code</c>，可用于获取支行参数）。</summary>
    [JsonPropertyName("city_code")]
    public int? CityCode { get; set; }

    /// <summary>获取或设置开户银行省市编码（官方 <c>bank_address_code</c>）。</summary>
    [JsonPropertyName("bank_address_code")]
    public string? BankAddressCode { get; set; }
}

/// <summary>查询大陆银行省份列表（<c>getprovince</c>）响应。</summary>
/// <remarks>官方契约：<b>POST</b> + 空请求体（官方原文「调用接口时传空的json串即可」）。</remarks>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetProvinceResponse : ChannelsResponse
{
    /// <summary>获取或设置省份信息（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public List<ChannelsProvince>? Data { get; set; }

    /// <summary>获取或设置总数（官方 <c>total_count</c>）。</summary>
    [JsonPropertyName("total_count")]
    public int? TotalCount { get; set; }
}

/// <summary>省份信息（官方 <c>data</c> 数组元素，<c>getprovince</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsProvince
{
    /// <summary>获取或设置省份简称（官方 <c>province_name</c>）。</summary>
    [JsonPropertyName("province_name")]
    public string? ProvinceName { get; set; }

    /// <summary>获取或设置省份编码（官方 <c>province_code</c>，可用于获取城市列表）。</summary>
    [JsonPropertyName("province_code")]
    public int? ProvinceCode { get; set; }
}

/// <summary>搜索银行列表（<c>getbanklist</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetBankListRequest
{
    /// <summary>获取或设置偏移量（官方 <c>offset</c>，选填）。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>获取或设置每页数据大小（官方 <c>limit</c>，选填）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>获取或设置银行关键字（官方 <c>key_words</c>，选填）。</summary>
    [JsonPropertyName("key_words")]
    public string? KeyWords { get; set; }

    /// <summary>获取或设置银行类型（官方 <c>bank_type</c>，选填：1 对私银行，2 对公银行；默认对公）。</summary>
    /// <remarks><b>官方文档矛盾照录</b>：参数表说明为「1:对私银行,2:对公银行」，返回参数中 <c>bank_type</c> 说明为「1.对公，2.对私」——两处口径相反，SDK 按参数表（入参）建模。</remarks>
    [JsonPropertyName("bank_type")]
    public int? BankType { get; set; }
}

/// <summary>搜索银行列表（<c>getbanklist</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetBankListResponse : ChannelsResponse
{
    /// <summary>获取或设置银行账号数据（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public List<ChannelsBankInfo>? Data { get; set; }
}

/// <summary>银行信息（官方 <c>data</c> 数组元素，<c>getbanklist</c> / <c>getbankbynum</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsBankInfo
{
    /// <summary>获取或设置银行编码（官方 <c>bank_code</c>）。</summary>
    [JsonPropertyName("bank_code")]
    public string? BankCode { get; set; }

    /// <summary>
    /// 获取或设置银行联号（官方 <c>bank_id</c>）——官方类型为 number；官方 <c>getbanklist</c> 示例以字符串
    /// <c>"1001"</c> 返回、<c>getbankbynum</c> 示例以数字 <c>1026</c> 返回（文档矛盾照录）。
    /// </summary>
    [JsonPropertyName("bank_id")]
    public long? BankId { get; set; }

    /// <summary>获取或设置银行名称（官方 <c>bank_name</c>，不包括支行）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }

    /// <summary>获取或设置是否需要填写支行信息（官方 <c>need_branch</c>）。</summary>
    [JsonPropertyName("need_branch")]
    public bool? NeedBranch { get; set; }

    /// <summary>获取或设置银行类型（官方 <c>bank_type</c>：1 对公，2 对私——官方返回参数说明原文）。</summary>
    [JsonPropertyName("bank_type")]
    public int? BankType { get; set; }

    /// <summary>获取或设置开户银行（官方 <c>account_bank</c>）。</summary>
    [JsonPropertyName("account_bank")]
    public string? AccountBank { get; set; }
}

/// <summary>查询支行列表（<c>getsubbranch</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetSubBranchRequest
{
    /// <summary>获取或设置银行编码（官方 <c>bank_code</c>，必填；通过查询银行信息或者搜索银行信息获取）。</summary>
    [JsonPropertyName("bank_code")]
    public string BankCode { get; set; } = string.Empty;

    /// <summary>获取或设置城市编号（官方 <c>city_code</c>，必填；通过查询城市列表获取；官方示例写作字符串 <c>"571"</c>，类型按官方参数表为 number）。</summary>
    [JsonPropertyName("city_code")]
    public int CityCode { get; set; }

    /// <summary>获取或设置偏移量（官方 <c>offset</c>，选填）。</summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }

    /// <summary>获取或设置限制个数（官方 <c>limit</c>，选填）。</summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}

/// <summary>查询支行列表（<c>getsubbranch</c>）响应。</summary>
/// <remarks>
/// <b>官方文档矛盾照录</b>：参数表中 <c>data</c> 数组元素字段名为 <c>bank_id</c> / <c>bank_name</c>，
/// 代码示例为 <c>branch_id</c> / <c>branch_name</c>——SDK 按参数表建模。
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetSubBranchResponse : ChannelsResponse
{
    /// <summary>获取或设置其他银行信息（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public List<ChannelsSubBranch>? Data { get; set; }

    /// <summary>获取或设置总数（官方 <c>total_count</c>）。</summary>
    [JsonPropertyName("total_count")]
    public int? TotalCount { get; set; }

    /// <summary>获取或设置当前分页数量（官方 <c>count</c>）。</summary>
    [JsonPropertyName("count")]
    public int? Count { get; set; }

    /// <summary>获取或设置银行编码（官方 <c>account_bank_code</c>）。</summary>
    [JsonPropertyName("account_bank_code")]
    public long? AccountBankCode { get; set; }

    /// <summary>获取或设置银行别名（官方 <c>bank_alias</c>）。</summary>
    [JsonPropertyName("bank_alias")]
    public string? BankAlias { get; set; }

    /// <summary>获取或设置银行别名编码（官方 <c>bank_alias_code</c>）。</summary>
    [JsonPropertyName("bank_alias_code")]
    public string? BankAliasCode { get; set; }

    /// <summary>获取或设置银行名称（官方 <c>account_bank</c>）。</summary>
    [JsonPropertyName("account_bank")]
    public string? AccountBank { get; set; }
}

/// <summary>支行信息（官方 <c>data</c> 数组元素，<c>getsubbranch</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsSubBranch
{
    /// <summary>获取或设置银行联号（官方 <c>bank_id</c>——参数表字段名；官方示例写作 <c>branch_id</c>，文档矛盾照录）。</summary>
    [JsonPropertyName("bank_id")]
    public long? BankId { get; set; }

    /// <summary>获取或设置银行名称（官方 <c>bank_name</c>——参数表字段名；官方示例写作 <c>branch_name</c>，文档矛盾照录）。</summary>
    [JsonPropertyName("bank_name")]
    public string? BankName { get; set; }
}

/// <summary>根据卡号查银行信息（<c>getbankbynum</c>）请求体。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetBankByNumRequest
{
    /// <summary>获取或设置银行卡号（官方 <c>account_number</c>，必填）。</summary>
    [JsonPropertyName("account_number")]
    public string AccountNumber { get; set; } = string.Empty;
}

/// <summary>根据卡号查银行信息（<c>getbankbynum</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetBankByNumResponse : ChannelsResponse
{
    /// <summary>获取或设置银行信息（官方 <c>data</c>）。</summary>
    [JsonPropertyName("data")]
    public List<ChannelsBankInfo>? Data { get; set; }

    /// <summary>获取或设置总数（官方 <c>total_count</c>）。</summary>
    [JsonPropertyName("total_count")]
    public int? TotalCount { get; set; }
}

/// <summary>资金二维码 ticket 请求体（<c>qrcode/get</c> 与 <c>qrcode/check</c> 请求体字段集一致 ⇒ 共用）。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsQrcodeTicketRequest
{
    /// <summary>获取或设置二维码 ticket（官方 <c>qrcode_ticket</c>，必填；可从商户提现接口获取）。</summary>
    [JsonPropertyName("qrcode_ticket")]
    public string QrcodeTicket { get; set; } = string.Empty;
}

/// <summary>获取二维码（<c>qrcode/get</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsGetFundsQrcodeResponse : ChannelsResponse
{
    /// <summary>获取或设置二维码（官方 <c>qrcode_buf</c>，base64 编码二进制，需要 base64 解码后得到图片数据）。</summary>
    [JsonPropertyName("qrcode_buf")]
    public string? QrcodeBuf { get; set; }
}

/// <summary>查询扫码状态（<c>qrcode/check</c>）响应。</summary>
[HttpJsonSerializable(SerializerClassName = "Funds")]
public class ChannelsCheckFundsQrcodeResponse : ChannelsResponse
{
    /// <summary>获取或设置扫码状态（官方 <c>status</c>，见 <see cref="ChannelsFundsQrcodeStatuses"/>）。</summary>
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    /// <summary>获取或设置业务返回错误码（官方 <c>self_check_err_code</c>）。</summary>
    [JsonPropertyName("self_check_err_code")]
    public int? SelfCheckErrCode { get; set; }

    /// <summary>获取或设置业务返回错误信息（官方 <c>self_check_err_msg</c>）。</summary>
    [JsonPropertyName("self_check_err_msg")]
    public string? SelfCheckErrMsg { get; set; }

    /// <summary>获取或设置扫码者身份（官方 <c>scan_user_type</c>，见 <see cref="ChannelsFundsScanUserTypes"/>）。</summary>
    [JsonPropertyName("scan_user_type")]
    public int? ScanUserType { get; set; }
}

/// <summary>结算账户类型常量（官方 <c>bank_account_type</c> 枚举值）。</summary>
public static class ChannelsBankAccountTypes
{
    /// <summary>对公银行账户。</summary>
    public const string Business = "ACCOUNT_TYPE_BUSINESS";

    /// <summary>经营者个人银行卡。</summary>
    public const string Private = "ACCOUNT_TYPE_PRIVATE";
}

/// <summary>提现状态常量（官方 <c>status</c> 枚举值）。</summary>
public static class ChannelsWithdrawStatuses
{
    /// <summary>业务单已创建。</summary>
    public const string Init = "INIT";

    /// <summary>受理成功。</summary>
    public const string CreateSuccess = "CREATE_SUCCESS";

    /// <summary>提现成功。</summary>
    public const string Success = "SUCCESS";

    /// <summary>提现失败。</summary>
    public const string Fail = "FAIL";

    /// <summary>提现退票。</summary>
    public const string Refund = "REFUND";

    /// <summary>关单。</summary>
    public const string Close = "CLOSE";
}

/// <summary>资金类型常量（官方 <c>funds_type</c> 枚举值，<c>getfundsflowdetail</c>）。</summary>
public static class ChannelsFundsTypes
{
    /// <summary>未知。</summary>
    public const int Unknown = 0;

    /// <summary>订单支付。</summary>
    public const int OrderPay = 1;

    /// <summary>订单退款。</summary>
    public const int OrderRefund = 3;

    /// <summary>提现。</summary>
    public const int Withdraw = 4;

    /// <summary>提现退票。</summary>
    public const int WithdrawRefund = 5;

    /// <summary>达人佣金。</summary>
    public const int PromoterCommission = 10;

    /// <summary>技术服务费。</summary>
    public const int PlatformCommission = 11;

    /// <summary>带货机构服务费。</summary>
    public const int LoaderOrganizationCommission = 12;

    /// <summary>极速退款垫资。</summary>
    public const int ExpressRefundAdvance = 14;

    /// <summary>回补极速退款垫资。</summary>
    public const int ExpressRefundAdvanceSupplement = 15;

    /// <summary>运费险。</summary>
    public const int FreightInsurance = 16;

    /// <summary>预付运费退回。</summary>
    public const int PreFreightRefund = 17;

    /// <summary>平台优惠补贴。</summary>
    public const int PlatformSubsidy = 19;

    /// <summary>补交运费。</summary>
    public const int FreightMakeUp = 20;

    /// <summary>国补发放。</summary>
    public const int NationalSubsidy = 21;

    /// <summary>注销提现。</summary>
    public const int CancelWithdraw = 22;

    /// <summary>注销提现退票。</summary>
    public const int CancelWithdrawRefund = 23;

    /// <summary>平台赔付。</summary>
    public const int PlatformCompensation = 25;
}

/// <summary>订单结算状态常量（官方 <c>order_settle_state</c> 枚举值，<c>listorderflow</c>）。</summary>
public static class ChannelsOrderSettleStates
{
    /// <summary>无，查询全部。</summary>
    public const int All = 0;

    /// <summary>待结算。</summary>
    public const int Pending = 1;

    /// <summary>无需结算。</summary>
    public const int NotApplicable = 2;

    /// <summary>结算完成。</summary>
    public const int Settled = 60;

    /// <summary>部分结算。</summary>
    public const int Partial = 100;
}

/// <summary>订单状态常量（官方 <c>order_state</c> 枚举值，<c>listorderflow</c>）。</summary>
public static class ChannelsOrderFlowOrderStates
{
    /// <summary>全部。</summary>
    public const int All = 0;

    /// <summary>待发货。</summary>
    public const int PendingDelivery = 20;

    /// <summary>待收货。</summary>
    public const int PendingReceipt = 30;

    /// <summary>订单完成。</summary>
    public const int Completed = 100;
}

/// <summary>订单支付方式常量（官方 <c>order_pay_method</c> 枚举值，<c>listorderflow</c>）。</summary>
public static class ChannelsOrderFlowPayMethods
{
    /// <summary>全部。</summary>
    public const int All = 0;

    /// <summary>普通支付。</summary>
    public const int Normal = 1;

    /// <summary>先用后付。</summary>
    public const int PayLater = 2;
}

/// <summary>资金二维码扫码状态常量（官方 <c>status</c> 枚举值，<c>qrcode/check</c>）。</summary>
public static class ChannelsFundsQrcodeStatuses
{
    /// <summary>未扫码。</summary>
    public const int NotScanned = 0;

    /// <summary>已确认。</summary>
    public const int Confirmed = 1;

    /// <summary>已取消。</summary>
    public const int Canceled = 2;

    /// <summary>已失效。</summary>
    public const int Expired = 3;

    /// <summary>已扫码。</summary>
    public const int Scanned = 4;
}

/// <summary>扫码者身份常量（官方 <c>scan_user_type</c> 枚举值，<c>qrcode/check</c>）。</summary>
public static class ChannelsFundsScanUserTypes
{
    /// <summary>非管理员。</summary>
    public const int NonAdmin = 0;

    /// <summary>管理员。</summary>
    public const int Admin = 1;

    /// <summary>次管理员。</summary>
    public const int SubAdmin = 2;
}