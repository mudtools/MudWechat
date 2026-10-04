// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Pay;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「企业支付」模块「对外收款记录」域公共 SDK
/// （获取对外收款记录 + 获取收款项目的商户单号）。
/// <para>
/// 官方对三类应用开放完全一致的 2 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalPayBillService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyPayBillService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderPayBillService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）：
/// 企业自建为应用自身 access_token；第三方 / 代开发为授权企业级 access_token（scope = authCorpId），
/// 由多应用基座按当前应用上下文路由——企业级令牌须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>
/// 权限口径：自建应用须配置到「对外收款 - 可调用接口的应用」中；
/// 第三方 / 代开发应用须具有「对外收款」权限。
/// </para>
/// <para>
/// 官方约束：仅返回应用可见范围内用户的收款记录；4.1.0 及以上版本新增的部分商户号收款记录
/// 不返回给第三方 / 代开发应用；收款项目的商户单号仅允许获取由应用本身创建的收款项目；
/// 收款记录中的联系人信息（contact_info）第三方应用不可获取；
/// 自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkPayBillService
{
    /// <summary>
    /// 获取对外收款记录
    /// <para>获取企业对外收款记录（收款 / 退款交易单详情），
    /// 仅返回应用可见范围内用户的收款记录。</para>
    /// <para>官方业务限制：收款记录起止时间间隔不能超过 1 个月；
    /// 会过滤收款人不在可见范围中的记录，返回数可能小于 limit；无 next_cursor 时表示数据已全部拉取完。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetPayBillListRequest"/>：
    /// begin_time / end_time（间隔 ≤ 1 个月）/ payee_userid / cursor / limit（≤ 1000））。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>交易单详情列表（bill_list）与分页游标（next_cursor，无更多数据时不返回）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93667"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93727"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96701"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalpay/get_bill_list")]
    Task<GetPayBillListResponse> GetBillListAsync(
        [Body] GetPayBillListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取收款项目的商户单号
    /// <para>获取对外收款项目中每笔收款的商户单号；
    /// 仅允许获取由应用本身创建的收款项目的收款单号列表。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetPayPaymentInfoRequest"/>：payment_id，在发起对外收款时返回）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>收款单号列表（bill_list，每笔支付对应一个收款单号）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95944"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95936"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96702"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalpay/get_payment_info")]
    Task<GetPayPaymentInfoResponse> GetPaymentInfoAsync(
        [Body] GetPayPaymentInfoRequest request,
        CancellationToken cancellationToken = default);
}
