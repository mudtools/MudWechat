// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2 <b>资金</b>域（服务商侧小微商户）1 支端点：按日期查询自动提现。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（APIv2 合作伙伴侧，2026-10-18 逐页核验）：查询提现状态
/// <see href="https://pay.weixin.qq.com/doc/v2/partner/4014115346"/>
/// —— 页面标题是「查询提现状态」、挂在「小微商户资料修改及提现」章节下，
/// 而路径写的是 <c>queryautowithdrawbydate</c>；标题与路径不同名，按路径建族名、在此留痕。</para>
/// <para><b>需商户证书 ⇒ 手写</b>：本页「是否需要证书」为<b>「需要。详见证书使用。」</b>，
/// 错误码表亦给出 <c>CA_ERROR</c>｜商户证书验证失败｜「到商户平台下载证书，请求带上证书后重试」，
/// 失败示例的 <c>err_code</c> 即 <c>CA_ERROR</c> ⇒ 落 <c>WechatPayV2SecureInvoker</c>。</para>
/// <para><b>域名是 <c>api.mch</c></b>：本页「接口链接」栏为
/// <c>https://api.mch.weixin.qq.com/fund/queryautowithdrawbydate</c>，无 <c>api2.mch</c>
/// ⇒ 又是一支「要证书但挂普通域名」的端点（与 <see cref="IWechatPayV2RiskService"/>、
/// <c>IWechatPayV2BillCommentService</c> 同形）。</para>
/// <para><b>本支面向小微商户</b>：<c>mch_id</c> 与 <c>sub_mch_id</c> <b>同为必填</b>，
/// 请求面<b>没有</b> <c>appid</c>／<c>sub_appid</c> ⇒ 照抄不得按通用形状补齐。</para>
/// </remarks>
public interface IWechatPayV2FundService
{
    /// <summary>
    /// 按日期查询自动提现单（APIv2，服务商侧小微商户，需证书）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4014115346"/>。
    /// </summary>
    /// <param name="request">查询请求体，见 <see cref="V2AutoWithdrawQueryRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>提现单状态，见 <see cref="V2AutoWithdrawQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/fund/queryautowithdrawbydate</c>；XML 应答、
    /// 应答<b>带 <c>sign</c> 与 <c>sign_type</c></b> ⇒ 走常规应答验签。</para>
    /// <para><b>算法仅一档、且 <c>sign_type</c> 参与签名</b>：官方「签名类型，目前支持HMAC-SHA256。
    /// 该字段需参与签名sign的计算」⇒ 默认值钉为 <c>HMAC-SHA256</c>，它既是算法开关又是签名入参，
    /// 清空必 <c>SIGNERROR</c>。</para>
    /// <para><b>日期口径反直觉</b>：格式 <c>YYYYMMDD</c>、仅支持最近 <b>30</b> 天，
    /// 且<b>提现单日期比交易日晚一天</b>（提现单在交易次日汇总前一天结算结果后生成）⇒
    /// 想查某天的钱要传<b>次日</b>，传错会得到「查无此单」而不是错误。</para>
    /// <para><b>应答是单个对象、不是列表</b>：官方返回表就一条提现单，
    /// <c>withdraw_status</c> 七档状态机与 <c>fail_reason</c>、三个时间字段
    /// （<c>create_time</c>/<c>success_time</c>/<c>refund_time</c>）同层平铺。</para>
    /// <para><b>失败码</b>：<c>SYSTEM_ERROR</c> / <c>CA_ERROR</c> / <c>FREQUENCY_LIMITED</c>（请降低访问频率）/
    /// <c>NO_AUTH</c> —— 本族是<b>下划线形态</b>（与商户侧各页的 <c>SYSTEMERROR</c> 不同），判错两种拼写都要认。</para>
    /// <para><b>金额与账号信息</b>：<c>amount</c> 为分单位的提现金额，属资金流水数据；
    /// 本 SDK 不落日志、不入库，异常消息只含路由与官方字段名（PAY-B7 同线口径）。</para>
    /// </remarks>
    Task<V2AutoWithdrawQueryResponse> QueryAutoWithdrawByDateAsync(
        V2AutoWithdrawQueryRequest request, CancellationToken cancellationToken = default);
}
