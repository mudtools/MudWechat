// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ShopFunds;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「自定义交易组件 · 提现与提款二维码」域 SDK（4 端点：发起提现 / 获取提现记录详情 /
/// 获取提现记录列表 / 生成提款二维码；官方「小程序支付管理服务 → 提现接口 + 提款二维码」分区）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。
/// 域级事实（前缀两线分持、强制校验来源 IP、频率数字空缺、敏感面）见
/// <see cref="IWxaShopFundsAccountService"/> 的类型注释，三支接口共用同一注册组 <c>ShopFunds</c>。
/// </para>
/// <para>
/// <b>本族三支提现端点的必填集比余额面多两把键</b>：<c>biz_type</c>（官方参数表写「业务类型，填 1」，
/// 即当前只公布一个取值）与 <c>mchid</c>（商户号）——两者在 <c>submitwithdraw</c> /
/// <c>getwithdrawdetail</c> / <c>scanwithdraw</c> 三页均为必填，宿主须自行持有商户号，官方不提供查询端点。
/// </para>
/// <para>
/// <b>提现成功后不可重复（官方量化上限）</b>：<c>submitwithdraw</c> 返回码 <c>9700101</c> 即
/// 「超出每日只能提现一次的限制」⇒ 本端点<b>非幂等</b>，一律不得放进自动重试 / 定时轮询管道。
/// </para>
/// <para>
/// <b>扫码验证身份提现是「跨线半成品」</b>：<see cref="GenerateQrcodeAsync"/> 只产出
/// <c>qrcode_ticket</c>，官方用于取二维码图片（<c>/shop/funds/qrcode/get</c>）与查扫码状态
/// （<c>/shop/funds/qrcode/check</c>）的两页本仓已由<b>小店线</b>承载（不同令牌），
/// 故小程序侧无法在本线内闭环轮询扫码状态 ⇒ 宿主须知情，勿在本域内「顺手补齐」那两支。
/// </para>
/// <para><b>敏感面（AGENTS §8）</b>：银行账号（<c>bank_num</c>）、银行名称、提现金额与备注 / 银行附言均属个人金融信息 ⇒ 禁止写入日志、遥测或异常消息。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ShopFunds", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaShopFundsWithdrawService
{
    /// <summary>
    /// 发起提现（<c>POST /shop/funds/submitwithdraw</c>，官方页面标题「发起提现」）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/wxafunds/API/funds/submitwithdraw.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>amount</c> / <c>biz_type</c> / <c>mchid</c> 必填，<c>remark</c> / <c>bank_memo</c> 选填），见 <see cref="WxaShopFundsSubmitWithdrawRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>提现单号（<c>out_request_no</c>）与二维码 ticket（<c>qrcode_ticket</c>），见 <see cref="WxaShopFundsSubmitWithdrawResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体。</para>
    /// <para><b>金额单位官方明写「单位：分」</b>（<c>amount</c> 参数表原文）⇒ 本仓 <c>amount</c> 为 <c>long</c> 分口径、不做换算；本域内唯一给出单位的金额字段。</para>
    /// <para>
    /// <b>每天只能提现一次</b>：官方返回码 <c>9700101</c>「超出每日只能提现一次的限制」，另有通用 <c>-1</c> 系统异常
    /// ⇒ <b>非幂等、成功后不可重复</b>：重试即撞该码；请求超时等不确定态须先用 <see cref="GetWithdrawDetailAsync"/>
    /// （凭宿主自持的 <c>out_request_no</c>）确认结果，而不是重发。
    /// </para>
    /// <para><b>提现不是即时到账</b>：应答只给单号与二维码 ticket，终态由 <c>withdraw_info.status</c>（见 <see cref="WxaShopFundsWithdrawStatuses"/>）表达，含 <c>FAIL</c> / <c>REFUND</c>（提现退票）等失败态。</para>
    /// <para><b><c>qrcode_ticket</c> 与 <see cref="GenerateQrcodeAsync"/> 的 ticket 同一种凭据</b>（官方说明「可用于获取二维码和查询扫码状态，扫码验证身份提现」），取图 / 查状态两支不在本线（见类型注释）。</para>
    /// <para><b>敏感面</b>：<c>amount</c>（提现金额）、<c>remark</c> 与 <c>bank_memo</c> 不得进日志、遥测或异常消息。</para>
    /// </remarks>
    [Post("/shop/funds/submitwithdraw")]
    Task<WxaShopFundsSubmitWithdrawResponse> SubmitWithdrawAsync(
        [Body] WxaShopFundsSubmitWithdrawRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取提现记录详情（<c>POST /shop/funds/getwithdrawdetail</c>，官方页面标题「获取提现记录详情」）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/wxafunds/API/funds/getwithdrawdetail.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>out_request_no</c> / <c>biz_type</c> / <c>mchid</c> 三支全必填），见 <see cref="WxaShopFundsWithdrawDetailRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>提现详情（<c>withdraw_info</c>），见 <see cref="WxaShopFundsWithdrawDetailResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；返回码 <c>-1</c> 系统异常、<c>9710001</c> 暂无数据。</para>
    /// <para><b>官方原文把 <c>withdraw_info</c> 的说明写成「退款详情信息」</b>（页面标题与字段名均为提现）⇒ 本仓按「提现详情」建模，措辞差异仅留档，不得据此另建退款 DTO。</para>
    /// <para><b>官方回包示例是残缺 JSON</b>（<c>"withdraw_info"{ ... }</c> 缺冒号）⇒ 字段名取参数表（示例只用于取值形态），本仓按参数表落地。</para>
    /// <para>
    /// <b>金额与时间单位混用须照录</b>：<c>amount</c> 为 <c>number</c>（本页未标单位，与 <c>submitwithdraw</c> 的「分」口径并存但不可互推）；
    /// <c>create_time</c> / <c>update_time</c> 为秒级时间戳（示例 1590733930）。
    /// </para>
    /// <para><b>提现终态判定看 <c>status</c></b>（枚举 8 值，见 <see cref="WxaShopFundsWithdrawStatuses"/>），<c>reason</c> 只在失败态携带；<c>errcode=0</c> 仅代表查询成功。</para>
    /// <para><b>敏感面</b>：<c>bank_num</c>（银行账户）、<c>bank_name</c>、<c>amount</c>、<c>remark</c>、<c>bank_memo</c> 禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/shop/funds/getwithdrawdetail")]
    Task<WxaShopFundsWithdrawDetailResponse> GetWithdrawDetailAsync(
        [Body] WxaShopFundsWithdrawDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取提现记录列表（<c>POST /shop/funds/scanwithdraw</c>，官方页面标题「获取提现记录列表」）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/wxafunds/API/funds/scanwithdraw.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>page_num</c> / <c>page_size</c> / <c>biz_type</c> / <c>mchid</c> 四支全必填），见 <see cref="WxaShopFundsScanWithdrawRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>提现单号列表（<c>out_request_nos</c>）与总数（<c>total_num</c>），见 <see cref="WxaShopFundsScanWithdrawResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；返回码 <c>-1</c> 系统异常、<c>9710001</c> 暂无数据。</para>
    /// <para><b>路由名与页面标题不一致</b>（路由 <c>scanwithdraw</c>、标题「获取提现记录列表」，无日期入参）⇒ 照官方路由原文，不得改写为 <c>getwithdrawlist</c>（该拼写属小店线 <c>/channels/ec/funds/getwithdrawlist</c>）。</para>
    /// <para><b>翻页口径</b>：官方参数表明写 <c>page_num</c>「页码，从 1 开始」+ <c>page_size</c>「每页大小」，本族<b>无</b>官方上限数字（区别于流水列表的 ≤100）；应答只返回单号列表，逐条详情仍须调 <see cref="GetWithdrawDetailAsync"/>。</para>
    /// <para><b>回包 <c>total_num</c> 只在示例出现、未进官方参数表</b>⇒ 本仓按示例建模为可空，宿主不得假定必有。</para>
    /// </remarks>
    [Post("/shop/funds/scanwithdraw")]
    Task<WxaShopFundsScanWithdrawResponse> GetWithdrawListAsync(
        [Body] WxaShopFundsScanWithdrawRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 生成提款二维码（<c>POST /shop/funds/qrcode/gen</c>，官方页面标题「生成二维码」）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/funds/qrcode/gen.html"/>。
    /// </summary>
    /// <param name="request">请求体（唯一字段 <c>identity_type</c>，必填），见 <see cref="WxaShopFundsGenQrcodeRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>二维码 ticket（<c>qrcode_ticket</c>，官方原文「可用于获取二维码和检查扫码状态」），见 <see cref="WxaShopFundsGenQrcodeResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体；返回码表只有通用 <c>-1</c> 系统异常。</para>
    /// <para><b><c>identity_type</c> 官方枚举表只公布一个取值</b>（<c>1</c> = 管理员，见 <see cref="WxaShopFundsIdentityTypes"/>）⇒ 本仓不扩充；次管理员等身份只出现在「查询扫码状态」页的 <c>scan_user_type</c> 侧，不属本请求体。</para>
    /// <para>
    /// <b>ticket 有效期官方未给数字</b>：本页与同族「获取二维码 / 查询扫码状态」两页均无「N 分钟 / N 秒」类期限，
    /// 失效只以错误码 <c>60220</c>（ticket 已失效）、<c>60208</c>（错误的 ticket）与 <c>status=3</c>（已失效）表达
    /// ⇒ 宿主不得自造本地过期时间，也不得把 ticket 当长期凭据缓存；本域<b>无</b>重生成以外的续期端点。
    /// </para>
    /// <para><b>本端点非幂等</b>：每次调用产出新 ticket（官方无「同一提现单复用 ticket」的说明），且取图 / 查扫码状态两支不在小程序线（见类型注释）⇒ 生成后若未在小程序侧闭环，只能重新生成。</para>
    /// <para><b>敏感面</b>：<c>qrcode_ticket</c> 是提现身份验证凭据，禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/shop/funds/qrcode/gen")]
    Task<WxaShopFundsGenQrcodeResponse> GenerateQrcodeAsync(
        [Body] WxaShopFundsGenQrcodeRequest request,
        CancellationToken cancellationToken = default);
}
