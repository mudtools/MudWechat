// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Fapiao;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「电子发票」域 SDK（APIv3 区块链电子发票，<b>首批 2 端点</b>：开具 + 查询）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（<b>普通商户</b>文档中心，2026-10-09 核验）：
/// 开具电子发票 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538301"/>（更新 2025.09.26）、
/// 查询电子发票 <see href="https://pay.weixin.qq.com/docs/merchant/apis/fapiao/fapiao-applications/get-fapiao-applications.html"/>。
/// 两页均标注<b>支持商户：【普通商户】</b>。
/// </para>
/// <para>
/// <b>✅ 更正设计方案 §2.5 的一条记录</b>：该处记「电子发票 · 普通商户文档中心<b>无独立入口</b>；
/// 服务商侧 <c>partner/4015792574</c>」—— <b>实测不成立</b>：普通商户侧有完整电子发票文档（本域两页均在其内）。
/// 服务商侧另有「开具<b>通用行业</b>电子发票」（<c>/fapiao-applications/<b>issue-general</b></c>，与商户侧不同路由）
/// 及其 22 项 API 清单，属另一套面，本域不覆盖。
/// </para>
/// <para>
/// <b>本域尚未覆盖</b>：配置开发选项（<c>PATCH /v3/new-tax-control-fapiao/merchant/development-config</c>，已核）、
/// 获取用户抬头填写链接 / 获取用户填写抬头信息、冲红、获取下载信息、下载发票文件、上传发票文件、
/// 插入卡包，以及用户抬头填写完成 / 开票成功 / 插入卡包成功 / 冲红成功 / 卡券作废等通知（归回调包）。
/// <b>不要凭推断补路由</b>。
/// </para>
/// <para>
/// <b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1）：形态与
/// <see cref="IWechatPayTransactionsService"/> 一致，详见其 remarks。
/// </para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "NewTaxControlFapiao", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayFapiaoService
{
    /// <summary>
    /// 开具电子发票。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538301"/>。
    /// </summary>
    /// <param name="request">开票请求体，见 <see cref="FapiaoIssueRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/new-tax-control-fapiao/fapiao-applications</c>；无 path / query 参数。</para>
    /// <para>
    /// <b>返回类型为何是 <c>Task</c></b>：官方标注应答状态 <b>202 Accepted</b> 且<b>无任何应答字段</b>
    /// ⇒ 与 204 类端点同款处置（声明为无返回值，避免调用方去解一个不存在的包体）。
    /// </para>
    /// <para>
    /// <b>异步语义（官方原文）</b>：成功返回<b>仅代表开票请求已被受理</b>；
    /// 开票完成须经<b>回调通知</b>或 <see cref="QueryFapiaoApplicationsAsync"/> 获取 ——
    /// <b>不得</b>把 202 当成开票成功。
    /// </para>
    /// </remarks>
    [Post("/v3/new-tax-control-fapiao/fapiao-applications")]
    Task IssueFapiaoAsync(
        [Body] FapiaoIssueRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询电子发票。官方文档：<see href="https://pay.weixin.qq.com/docs/merchant/apis/fapiao/fapiao-applications/get-fapiao-applications.html"/>。
    /// </summary>
    /// <param name="fapiaoApplyId">发票申请单号（官方 path <c>fapiao_apply_id</c>，必填 string(32)）。</param>
    /// <param name="fapiaoId">商户发票单号（官方 query <c>fapiao_id</c>，<b>选填</b> string(32)）：用于在申请单内过滤单张发票。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>发票列表与开票结果，见 <see cref="FapiaoQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/new-tax-control-fapiao/fapiao-applications/{fapiao_apply_id}?fapiao_id=…</c>；
    /// path 必带 <c>fapiao_apply_id</c>，query 的 <c>fapiao_id</c> <b>选填</b>。</para>
    /// <para><b>官方建议</b>：「【将电子发票插入微信用户卡包】接口成功后，应调用本接口查询电子发票开票结果」。</para>
    /// <para><b>值域未核验</b>：<c>status</c> / <c>card_information.card_status</c> 的取值表本轮未取得 ⇒ SDK 不臆造常量。</para>
    /// </remarks>
    [Get("/v3/new-tax-control-fapiao/fapiao-applications/{fapiaoApplyId}")]
    Task<FapiaoQueryResponse> QueryFapiaoApplicationsAsync(
        [Path] string fapiaoApplyId,
        [Query("fapiao_id")] string? fapiaoId = null,
        CancellationToken cancellationToken = default);
}
