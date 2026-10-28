// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2 <b>账单评论</b>域 1 支端点：按时间区间批量拉取订单评价数据。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（APIv2 商户侧，2026-10-18 逐页核验）：拉取订单评价数据
/// <see href="https://pay.weixin.qq.com/doc/v2/merchant/4014326528"/>。</para>
/// <para><b>需商户证书 ⇒ 手写</b>：本页「是否需要证书」为<b>「需要」</b>，且失败示例的
/// <c>return_msg</c> 直接就是「商户证书错误」⇒ 与 <see cref="IWechatPayV2RiskService"/> 同一理由，
/// 由 <c>WechatPayV2SecureInvoker</c> 承载（证书按商户绑定，声明式管道无从拿到）。</para>
/// <para><b>域名是 <c>api.mch</c></b>：本页「接口链接」栏为
/// <c>https://api.mch.weixin.qq.com/billcommentsp/batchquerycomment</c>，全文无 <c>api2.mch</c>
/// ⇒ 要证书却挂普通域名，正是通道按 (商户, 域名) 双键缓存的那类端点。</para>
/// <para><b>本支没有应答 DTO</b>：官方「成功时，数据以文本表格的方式返回」，
/// 失败时才回 XML 信封 ⇒ 本接口返回<b>应答原文字符串</b>（与 <c>/pay/downloadbill</c> 同一口径），
/// 由调用方按官方列序自行解析；SDK <b>不猜</b>列义、不落库、不解析评论内容。</para>
/// </remarks>
public interface IWechatPayV2BillCommentService
{
    /// <summary>
    /// 批量拉取订单评价数据（APIv2，需证书）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4014326528"/>。
    /// </summary>
    /// <param name="request">拉取请求体，见 <see cref="V2BatchQueryCommentRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>
    /// 应答<b>原文</b>：成功时为文本表格（首行 = 本批最后一条记录的 <c>offset</c>，
    /// 之后每行一笔订单评论、以反引号开头、逗号分隔「评论时间／支付订单号／评论星级／评论内容」）；
    /// 失败时为 XML 信封，原样交回由调用方判别。
    /// </returns>
    /// <remarks>
    /// <para><b>官方契约</b>：路径 <c>/billcommentsp/batchquerycomment</c>；
    /// 官方页<b>不写请求方式</b>（全文无「请求方式」字样）⇒ 按 XML 请求体 + 证书通道落 <b>POST</b>，
    /// 该选择由契约守卫锁定、不声称出自该页。</para>
    /// <para><b>分页是游标</b>：官方「第一行返回当前查询到的数据最后一条的 <c>offset</c>」⇒
    /// 续拉用上一批首行的数字；<c>limit</c> 最大 <b>200</b>、默认 200；
    /// 循环终止条件应是「本批条数 &lt; limit」或空批，而不是把 offset 往上涨。</para>
    /// <para><b>窗口只有三个月</b>：官方「接口只能拉取最近三个月以内的评价数据」，
    /// 越界返回 <c>TIME_EXPIRE</c>（解决方案原文「请拉取90天内的数据」）；
    /// 区间内无数据返回 <c>NO_COMMENT</c>（不是错误，应当正常收敛）。</para>
    /// <para><b>失败码</b>：<c>SYSTEMERROR</c> / <c>NO_COMMENT</c> / <c>TIME_EXPIRE</c> / <c>PARAM_ERROR</c>
    /// 四支，<b>无</b> <c>CERT_ERROR</c>（证书问题以 <c>return_msg</c>=「商户证书错误」的形态出现）。</para>
    /// <para><b>评论内容是用户输入</b>：文本表里的「评论内容」是终端用户自由文本 ⇒
    /// <b>不得</b>原样写日志或异常消息（PAY-B7 同线口径），也不得进遥测标签。</para>
    /// </remarks>
    Task<string> BatchQueryCommentAsync(
        V2BatchQueryCommentRequest request, CancellationToken cancellationToken = default);
}
