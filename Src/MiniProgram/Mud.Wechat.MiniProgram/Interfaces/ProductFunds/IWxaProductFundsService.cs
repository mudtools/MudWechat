// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ProductFunds;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「基础交易工具（原标准版交易组件 V1）· 资金管理」域 SDK（7 端点：获取账户余额 /
/// 获取资金流水 / 获取资金流水列表 / 获取提现记录 / 获取提现记录列表 / 修改结算账户 / 商户提现；
/// 官方 <c>minishopopencomponent/API/funds</c> 分区）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本域是资金 / 结算 / 提现三支一体的申报与查询面</b>：查询侧（余额 / 流水 / 提现记录）为读，
/// 写入侧（修改结算账户、商户提现）为<b>资金级不可逆动作</b> ⇒ 后两支一律非幂等，不得放进自动重试管道。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。
/// </para>
/// <para>
/// <b>路由族统一 <c>/product/funds/*</c></b>：属小程序令牌面（基础交易工具），与小店线 <c>/channels/ec/*</c>
/// 无任何交叠；守卫 PF6 锁定本域路由不含 <c>/channels/</c> 前缀。
/// </para>
/// <para>
/// <b>敏感面（AGENTS §8）</b>：本域涉及个人金融信息 —— 银行账户（<c>account_number</c> / <c>bank_num</c>）、
/// 开户名与开户行、账户类型、提现金额、结算流水、二级商户号（<c>sub_mchid</c>）⇒ 上述字段及其承载的请求体 /
/// 应答体<b>一律不得写入日志、遥测或异常消息</b>。
/// </para>
/// <para>
/// <b>金额单位口径</b>：仅 <see cref="SubmitWithdrawAsync"/> 官方明写「单位：分」；其余含金额的端点
/// （余额 / 流水 / 提现记录）官方<b>本页未标注金额单位</b> ⇒ 不得据微信支付惯例断言，
/// 守卫 PF5 负锁「本域全部金额字段为整型（分）、禁 <c>double</c>/<c>decimal</c>」。
/// </para>
/// <para>
/// <b>调用限制</b>：<see cref="SetBankAccountAsync"/> 官方明写「每天允许修改结算账户的次数为 5 次」，
/// <see cref="SubmitWithdrawAsync"/> 官方明写「每天最多允许提现一次」；其余 5 支查询端点官方本页无端点级频率数字。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ProductFunds", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaProductFundsService
{
    /// <summary>
    /// 获取账户余额（<c>POST /product/funds/getbalance</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/funds/get_balance.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>可提现余额 / 待结算余额 / 二级商户号，见 <see cref="WxaProductFundsBalanceResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>，请求体为空对象 <c>{}</c>（官方「请求参数说明」表为空 ⇒ 本方法<b>无 [Body] 参数</b>）；Query 携带 <c>access_token</c>。官方本页无端点级频率数字。</para>
    /// <para><b>金额单位官方未标注</b>：<c>available_amount</c>（可提现余额）与 <c>pending_amount</c>（待结算余额）类型列均为 number，本页未写「分/元」⇒ 不得据惯例断言，守卫 PF5 负锁整型口径。</para>
    /// <para><b><c>sub_mchid</c>（二级商户号）属敏感结算标识</b>：不得写入日志、遥测或异常消息。</para>
    /// </remarks>
    [Post("/product/funds/getbalance")]
    Task<WxaProductFundsBalanceResponse> GetBalanceAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取资金流水（按订单号，<c>POST /product/funds/getorderflow</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/funds/get_order_flow.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>order_id</c> 必填），见 <see cref="WxaProductFundsOrderFlowRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>单笔资金流水（<c>order_flow</c> 对象），见 <see cref="WxaProductFundsOrderFlowResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体（仅 <c>order_id</c> 一支，number）；Query 携带 <c>access_token</c>。官方本页无端点级频率数字。</para>
    /// <para><b>应答包装形态为 <c>order_flow</c> 单对象</b>（与 <see cref="ScanOrderFlowAsync"/> 的 <c>order_flow[]</c> 数组不同形）⇒ 两类应答不得合并，元素类型 <see cref="WxaProductFundsOrderFlow"/> 共用。</para>
    /// <para><b><c>order_flow.status</c> 名实不一致（本域反直觉点）</b>：官方「回包参数说明」标 number、枚举表给 1..4（交易失败/成功/异常/待结算），但<b>回包示例给的是字符串</b>（<c>ORDER_FLOW_STATUS_TRANSACTION_PENDING_SETTLED</c>）⇒ 实际线格式为字符串，故建模为 <c>string?</c>，守卫 PF5 负锁。</para>
    /// <para><b>金额字段（price / order_price / discount_fee / service_fee / handling_fee / refund_fee）官方本页未标注单位</b>，一律整型（禁浮点）；不得写入日志、遥测或异常消息。</para>
    /// </remarks>
    [Post("/product/funds/getorderflow")]
    Task<WxaProductFundsOrderFlowResponse> GetOrderFlowAsync(
        [Body] WxaProductFundsOrderFlowRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取资金流水列表（<c>POST /product/funds/scanorderflow</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/funds/scan_order_flow.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>page_num</c> / <c>page_size</c> 均必填，<c>page_size</c> 必须 ≤ 100），见 <see cref="WxaProductFundsScanOrderFlowRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>总页数 / 总个数 + 流水数组（<c>order_flow[]</c>），见 <see cref="WxaProductFundsScanOrderFlowResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；Query 携带 <c>access_token</c>。官方本页无端点级频率数字，<c>page_size ≤ 100</c> 属<b>规模</b>限制，不得当频率解读。</para>
    /// <para><b>应答数组键名是 <c>order_flow</c>（复数列表）而非 <c>list</c></b>：与 <see cref="ScanWithdrawAsync"/> 的 <c>list</c> 键名不同 ⇒ 守卫 PF5 分别锁定。</para>
    /// <para><b>本页 status 枚举给出「string 映射」列</b>（1=ORDER_FLOW_STATUS_TRANSACTION_FAIL / 2=…SUC / 3=…ERR / 4=…PENDING_SETTLED）⇒ 与 <see cref="GetOrderFlowAsync"/> 同为字符串线格式，元素类型 <see cref="WxaProductFundsOrderFlow"/> 共用。</para>
    /// <para><b>金额字段官方本页未标注单位</b>，一律整型（禁浮点）；不得写入日志、遥测或异常消息。</para>
    /// </remarks>
    [Post("/product/funds/scanorderflow")]
    Task<WxaProductFundsScanOrderFlowResponse> ScanOrderFlowAsync(
        [Body] WxaProductFundsScanOrderFlowRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取提现记录（按提现单号，<c>POST /product/funds/getwithdrawdetail</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/funds/get_withdraw_detail.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>out_request_no</c> 必填），见 <see cref="WxaProductFundsWithdrawDetailRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>单笔提现记录（平铺字段），见 <see cref="WxaProductFundsWithdrawDetailResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体（仅 <c>out_request_no</c> 一支）；Query 携带 <c>access_token</c>。官方本页无端点级频率数字。</para>
    /// <para><b>应答为平铺单对象</b>（无 <c>list</c> 包装），时间键为 <c>create_time</c> / <c>update_time</c>；<see cref="ScanWithdrawAsync"/> 的列表元素则用 <c>wxpay_create_time</c> / <c>wxpay_update_time</c> ⇒ 两套提现应答形态不同，不得互相抄。</para>
    /// <para><b><c>status</c> 为字符串枚举</b>：CREATE_SUCCESS / SUCCESS / FAIL / REFUND / CLOSE / INIT（与流水侧的 ORDER_FLOW_STATUS_* 是两套取值）。</para>
    /// <para><b><c>bank_num</c>（银行账户）/ <c>bank_name</c>（银行名称）/ <c>amount</c>（金额）属个人金融信息</b>：不得写入日志、遥测或异常消息。金额单位官方本页未标注，整型口径。</para>
    /// </remarks>
    [Post("/product/funds/getwithdrawdetail")]
    Task<WxaProductFundsWithdrawDetailResponse> GetWithdrawDetailAsync(
        [Body] WxaProductFundsWithdrawDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取提现记录列表（<c>POST /product/funds/scanwithdraw</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/funds/scan_withdraw.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>page_num</c> / <c>page_size</c> 均必填），见 <see cref="WxaProductFundsScanWithdrawRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>提现记录数组（<c>list</c>）+ <c>total_num</c>，见 <see cref="WxaProductFundsScanWithdrawResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；Query 携带 <c>access_token</c>。官方本页无端点级频率数字。</para>
    /// <para><b>应答数组键名是 <c>list</c></b>（区别于 <see cref="ScanOrderFlowAsync"/> 的 <c>order_flow</c>），且<b>只有 <c>total_num</c> 无 <c>total_page</c></b>；元素时间键为 <c>wxpay_create_time</c> / <c>wxpay_update_time</c> ⇒ 与单条提现记录不同形，守卫 PF5 锁定。</para>
    /// <para><b><c>amount</c>（提现金额）/ <c>bank_memo</c>（银行附言）属敏感结算信息</b>：不得写入日志、遥测或异常消息。金额单位官方本页未标注，整型口径。</para>
    /// </remarks>
    [Post("/product/funds/scanwithdraw")]
    Task<WxaProductFundsScanWithdrawResponse> ScanWithdrawAsync(
        [Body] WxaProductFundsScanWithdrawRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改结算账户（<c>POST /product/funds/setbankaccount</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/funds/set_bank_account.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>account_info</c> 包装，内含银行账号等必填字段），见 <see cref="WxaProductFundsSetBankAccountRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>（官方应答无业务字段 ⇒ 复用基类 <see cref="WxaResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体（<c>account_info</c> 单对象包装）；Query 携带 <c>access_token</c>。</para>
    /// <para><b>频率限制</b>（官方正文首句）：<b>每天允许修改结算账户的次数为 5 次</b> ⇒ 覆盖式写入且受限流，不得放进自动重试管道。</para>
    /// <para><b><c>account_number</c>（银行账号）为必填敏感字段</b>，连同 <c>account_bank</c>（开户银行）/ <c>bank_account_type</c>（账户类型，ACCOUNT_TYPE_BUSINESS 对公 / ACCOUNT_TYPE_PRIVATE 经营者个人银行卡）属个人金融信息 ⇒ 请求体整体不得写入日志、遥测或异常消息。</para>
    /// <para><b>「其他银行」二选一必填</b>（官方说明）：开户银行为「其他银行」时须填 <c>bank_branch_id</c>（联行号）或 <c>bank_name</c>（开户银行全称含支行）之一；<c>account_bank4show</c> 为前端展示值，可空、缺省回落 <c>account_bank</c>。</para>
    /// </remarks>
    [Post("/product/funds/setbankaccount")]
    Task<WxaResponse> SetBankAccountAsync(
        [Body] WxaProductFundsSetBankAccountRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商户提现（<c>POST /product/funds/submitwithdraw</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/funds/submit_withdraw.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>amount</c> 必填、<b>单位分</b>；<c>remark</c> / <c>bank_memo</c> 选填），见 <see cref="WxaProductFundsSubmitWithdrawRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>二维码 ticket（<c>qrcode_ticket</c>，可用于获取二维码与查询扫码状态），见 <see cref="WxaProductFundsSubmitWithdrawResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；Query 携带 <c>access_token</c>。</para>
    /// <para><b>频率限制</b>（官方正文首句）：<b>每天最多允许提现一次</b> ⇒ 本端点是<b>资金级不可逆动作</b>，绝对不得放进自动重试管道。</para>
    /// <para><b>本域唯一官方明写金额单位的端点</b>：<c>amount</c> 官方标注「提现金额（单位：分）」⇒ 守卫 PF5 以此正对照其余端点「官方未标注单位、不据惯例断言」。</para>
    /// <para><b><c>qrcode_ticket</c> 是资金凭证</b>（后续生成提现二维码 / 查询扫码状态的入参）：不得写入日志、遥测或异常消息；<c>bank_memo</c>（银行附言）同属敏感结算信息。</para>
    /// </remarks>
    [Post("/product/funds/submitwithdraw")]
    Task<WxaProductFundsSubmitWithdrawResponse> SubmitWithdrawAsync(
        [Body] WxaProductFundsSubmitWithdrawRequest request,
        CancellationToken cancellationToken = default);
}
