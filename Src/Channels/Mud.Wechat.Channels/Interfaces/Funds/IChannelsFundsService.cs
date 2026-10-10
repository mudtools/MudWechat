// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「资金结算」域 SDK（16 端点：账户余额 / 结算账户 / 提现 /
/// 资金流水 / 订单流水 / 银行·城市·支行查询 / 资金二维码）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与「微信支付 APIv3」「企微企业支付」概念严格区分（设计方案 v1 §4.6）</b>：本域是小店
/// <b>结算 / 提现 / 流水</b>（凭据为小店 <c>access_token</c>），<b>不是</b>支付收单
/// （<c>Mud.Wechat.Pay</c> 为商户私钥签名）也<b>不是</b>企微对外收款 / 收银台（Work 线）。
/// 守卫 <c>ChannelsFundsContractGuards</c> 措辞锁定该边界。
/// </para>
/// <para>
/// <b>路由前缀并存（设计方案 v1 §4.3）</b>：本域同时使用 <c>/channels/ec/funds/*</c>（9 端点）与
/// <c>/shop/funds/*</c>（7 端点，官方历史前缀，照抄原文不得改写）。
/// </para>
/// <para>
/// <b>金额单位</b>：本域全部金额字段（余额 / 提现 / 流水 / 结算）单位均为<b>分</b>。
/// </para>
/// <para>
/// <b>MUD005 已知接受风险</b>：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Funds", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsFundsService
{
    /// <summary>
    /// 获取账户余额（<c>getbalance</c>，可提现余额 + 待结算余额 + 二级商户号）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>账户余额（<c>available_amount</c> 可提现余额 / <c>pending_amount</c> 待结算余额，单位：分）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/funds/funds/api_getbalance"/></para>
    /// <para>官方契约：<b>POST</b> + 空请求体（官方原文「调用接口时传空的json串即可」）。</para>
    /// <para>官方业务限制：第三方平台代调用所属权限集 id 为 <c>138</c>；本接口无特殊错误码（走通用错误码）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/funds/getbalance")]
    Task<ChannelsGetBalanceResponse> GetBalanceAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取结算账户（<c>getbankacct</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>结算账户信息（<c>account_info</c>：账户类型 / 开户银行 / 省市编码 / 联行号 / 全称 / 账号 / 账户名称）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/funds/funds/api_getbankacct"/></para>
    /// <para>官方契约：<b>POST</b> + 空请求体（官方原文「调用接口时传空的json串即可」）。</para>
    /// <para>官方业务限制：第三方平台代调用所属权限集 id 为 <c>138</c>；账户类型枚举见 <see cref="ChannelsBankAccountTypes"/>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/funds/getbankacct")]
    Task<ChannelsGetBankAcctResponse> GetBankAccountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改结算账户（<c>setbankacct</c>）。
    /// </summary>
    /// <param name="request">修改请求（<c>account_info</c> 内 <c>bank_account_type</c>/<c>account_bank</c>/<c>bank_address_code</c>/<c>account_number</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/funds/funds/api_setbankacct"/></para>
    /// <para>
    /// 官方业务限制：①<b>每天允许修改结算账户的次数为 5 次</b>；②结算账户需要与店铺的主体一致
    /// （对公为公司名称，对私为法人名称）；③企业类型只能绑定对公账户，个体工商户可以绑定对公或者对私账户。
    /// </para>
    /// <para>成功修改后触发官方「结算账户变更回调」事件。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>138</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/funds/setbankacct")]
    Task<ChannelsResponse> SetBankAccountAsync(
        [Body] ChannelsSetBankAcctRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商户提现（<c>submitwithdraw</c>）。
    /// </summary>
    /// <param name="request">提现请求（<c>amount</c> 必填，单位：分）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>提现单号与二维码 ticket。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/funds/funds/api_submitwithdraw"/></para>
    /// <para>
    /// 官方业务限制：<b>每天最多允许提现一次</b>；成功后将触发官方「提现回调」与「提现二维码回调」两个事件。
    /// </para>
    /// <para>
    /// <b>官方文档描述互换（照录）</b>：官方参数表把 <c>withdraw_id</c> 说明写为「二维码ticket」、
    /// <c>qrcode_ticket</c> 写为「提现单号」——径自矛盾；字段语义以字面名为准（<c>qrcode_ticket</c> 供
    /// <c>qrcode/get</c> / <c>qrcode/check</c> 使用）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>138</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/funds/submitwithdraw")]
    Task<ChannelsSubmitWithdrawResponse> SubmitWithdrawAsync(
        [Body] ChannelsSubmitWithdrawRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取提现记录列表（<c>getwithdrawlist</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>page_num</c>/<c>page_size</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>提现单号列表与总数（<c>withdraw_ids</c>/<c>total_num</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/funds/funds/api_getwithdrawlist"/></para>
    /// <para>官方业务限制：本接口无特殊注意事项；专属错误码 <c>10022002</c>（违规行为，橱窗被禁止使用）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>138</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/funds/getwithdrawlist")]
    Task<ChannelsGetWithdrawListResponse> GetWithdrawListAsync(
        [Body] ChannelsGetWithdrawListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取提现记录（<c>getwithdrawdetail</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>withdraw_id</c> 必填，可从获取提现记录列表接口获取）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>提现明细（金额 / 时间 / 备注 / 银行 / 状态）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/funds/funds/api_getwithdrawdetail"/></para>
    /// <para>官方业务限制：本接口无特殊注意事项；提现状态枚举见 <see cref="ChannelsWithdrawStatuses"/>。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>138</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/funds/getwithdrawdetail")]
    Task<ChannelsGetWithdrawDetailResponse> GetWithdrawDetailAsync(
        [Body] ChannelsGetWithdrawDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取资金流水列表（<c>getfundsflowlist</c>，对账主入口）。
    /// </summary>
    /// <param name="request">查询请求（全字段选填；深翻页时 <c>next_key</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>流水单号列表与翻页信息（<c>flow_ids</c>/<c>has_more</c>/<c>next_key</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/funds/funds/api_getfundsflowlist"/></para>
    /// <para>
    /// 官方业务限制：①当日资金流水<b>出账时间为次日 16 点后</b>；②先用后付订单需用户确认收货自动付款后才会产生资金流水；
    /// ③<b><c>end_time</c> 受出账约束</b>——16 点前最大为前 1 天 0 点 0 分 0 秒，16 点后最大为当天 0 点 0 分 0 秒，
    /// 超过最大值将按最大值进行查询；④翻页需回传 <c>next_key</c>（page 为上一页加一且 page_size 相同才生效），
    /// <c>page * page_size &gt;= 10000</c> 时必填。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>138</c>、<c>141</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/funds/getfundsflowlist")]
    Task<ChannelsGetFundsFlowListResponse> GetFundsFlowListAsync(
        [Body] ChannelsGetFundsFlowListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取资金流水详情（<c>getfundsflowdetail</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>flow_id</c> 必填，可通过获取资金流水列表获取）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>流水详情（<c>funds_flow</c>：资金类型 / 收支 / 金额 / 余额 / 关联信息列表）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/funds/funds/api_getfundsflowdetail"/></para>
    /// <para>官方业务限制：本接口无特殊注意事项；<c>funds_type</c> 枚举见 <see cref="ChannelsFundsTypes"/>；
    /// 专属错误码 <c>10021302</c>（暂无数据）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>138</c>、<c>141</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/funds/getfundsflowdetail")]
    Task<ChannelsGetFundsFlowDetailResponse> GetFundsFlowDetailAsync(
        [Body] ChannelsGetFundsFlowDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询订单流水列表（<c>listorderflow</c>，订单结算信息）。
    /// </summary>
    /// <param name="request">查询请求（<c>order_settle_state</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单流水列表（<c>data_list</c>）与总数（<c>total_count</c>，仅供参考）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/funds/funds/api_listorderflow"/></para>
    /// <para>
    /// 官方业务限制：①<b>本接口不支持云调用</b>；②所有项目的结算状态与 <c>order_settle_state</c> 相同
    /// （官方注意事项原文）；③翻页推荐使用分页上下文 <c>use_page_ctx</c> + <c>page_ctx</c>（保证不遗漏不重复，<c>offset</c> 不生效）。
    /// </para>
    /// <para>官方错误码：<c>669900000</c>（参数错误，具体查看 errmsg）/ <c>669900001</c>（系统异常，请重试）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>138</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/funds/listorderflow")]
    Task<ChannelsListOrderFlowResponse> ListOrderFlowAsync(
        [Body] ChannelsListOrderFlowRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询城市列表（<c>getcity</c>，开户银行省市编码来源）。
    /// </summary>
    /// <param name="request">查询请求（<c>province_code</c> 必填，经「查询大陆银行省份列表」获取）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>城市信息列表（<c>city_name</c>/<c>city_code</c>/<c>bank_address_code</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/funds/bank/api_ecgetcity"/></para>
    /// <para>官方业务限制：<b>本接口不支持云调用</b>；专属错误码 <c>9710001</c>（暂无数据）；
    /// 官方适用范围表同时放行「小程序」与「微信小店」两类账号。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>138</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/funds/getcity")]
    Task<ChannelsGetCityResponse> GetCityListAsync(
        [Body] ChannelsGetCityRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询大陆银行省份列表（<c>getprovince</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>省份信息列表（<c>province_name</c> 省份简称 / <c>province_code</c> 省份编码）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/funds/bank/api_ecgetprovince"/></para>
    /// <para>官方契约：<b>POST</b> + 空请求体（官方原文「调用接口时传空的json串即可」）；<b>不支持云调用</b>。</para>
    /// <para>官方适用范围表同时放行「小程序」与「微信小店」两类账号。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>138</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/funds/getprovince")]
    Task<ChannelsGetProvinceResponse> GetProvinceListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 搜索银行列表（<c>getbanklist</c>，修改结算账户的前置查询）。
    /// </summary>
    /// <param name="request">搜索请求（全字段选填，默认对公）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>银行信息列表（<c>bank_code</c>/<c>bank_id</c>/<c>bank_name</c>/<c>need_branch</c>/<c>bank_type</c>/<c>account_bank</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/funds/bank/api_ecgetbanklist"/></para>
    /// <para>官方业务限制：<b>不支持云调用</b>；专属错误码 <c>9710001</c>（暂无数据）。</para>
    /// <para><b>官方文档矛盾照录</b>：入参 <c>bank_type</c> 说明为「1 对私，2 对公；默认对公」，返回参数
    /// <c>bank_type</c> 说明为「1 对公，2 对私」——两处口径相反，SDK 按入参参数表建模（<see cref="ChannelsGetBankListRequest.BankType"/>）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>138</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/funds/getbanklist")]
    Task<ChannelsGetBankListResponse> GetBankListAsync(
        [Body] ChannelsGetBankListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询支行列表（<c>getsubbranch</c>，获取开户银行联行号）。
    /// </summary>
    /// <param name="request">查询请求（<c>bank_code</c>/<c>city_code</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>支行信息列表与银行别名等（<c>data</c>/<c>total_count</c>/<c>count</c>/<c>account_bank_code</c>/<c>bank_alias</c>/<c>bank_alias_code</c>/<c>account_bank</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/funds/bank/api_ecgetsubbranch"/></para>
    /// <para>官方业务限制：<b>不支持云调用</b>；专属错误码 <c>9710001</c>（暂无数据）。</para>
    /// <para><b>官方文档矛盾照录</b>：参数表 <c>data</c> 元素字段名为 <c>bank_id</c>/<c>bank_name</c>，
    /// 代码示例为 <c>branch_id</c>/<c>branch_name</c>——SDK 按参数表建模。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>138</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/funds/getsubbranch")]
    Task<ChannelsGetSubBranchResponse> GetSubBranchListAsync(
        [Body] ChannelsGetSubBranchRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据卡号查银行信息（<c>getbankbynum</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>account_number</c> 银行卡号必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>银行信息列表（<c>data</c>）与总数（<c>total_count</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/funds/bank/api_ecgetbankbynum"/></para>
    /// <para>官方业务限制：<b>不支持云调用</b>；专属错误码 <c>9710001</c>（暂无数据）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>138</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/funds/getbankbynum")]
    Task<ChannelsGetBankByNumResponse> GetBankByCardNumberAsync(
        [Body] ChannelsGetBankByNumRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取二维码（<c>qrcode/get</c>，<c>qrcode_buf</c> 为 base64 编码的二维码二进制，需 base64 解码）。
    /// </summary>
    /// <param name="request">查询请求（<c>qrcode_ticket</c> 必填，可从商户提现接口获取）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>二维码二进制（base64 编码字符串）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/funds/qrcode/api_ecgetqrcode"/></para>
    /// <para>官方业务限制：<b>不支持云调用</b>；官方错误码：<c>-2</c>（token 太长）/ <c>60208</c>（错误的 ticket）/ <c>60220</c>（ticket 已失效）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>116</c>、<c>138</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/funds/qrcode/get")]
    Task<ChannelsGetFundsQrcodeResponse> GetFundsQrcodeAsync(
        [Body] ChannelsQrcodeTicketRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询扫码状态（<c>qrcode/check</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>qrcode_ticket</c> 必填，可从商户提现接口获取）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>扫码状态（<c>status</c>/<c>self_check_err_code</c>/<c>self_check_err_msg</c>/<c>scan_user_type</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/funds/qrcode/api_eccheckqrcode"/></para>
    /// <para>官方业务限制：<b>不支持云调用</b>；扫码状态枚举见 <see cref="ChannelsFundsQrcodeStatuses"/>，
    /// 扫码者身份枚举见 <see cref="ChannelsFundsScanUserTypes"/>。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>85</c>、<c>116</c>、<c>138</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/shop/funds/qrcode/check")]
    Task<ChannelsCheckFundsQrcodeResponse> CheckFundsQrcodeAsync(
        [Body] ChannelsQrcodeTicketRequest request,
        CancellationToken cancellationToken = default);
}