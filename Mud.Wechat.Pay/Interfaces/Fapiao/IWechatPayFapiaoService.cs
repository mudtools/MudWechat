// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.Fapiao;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「电子发票」域 SDK（APIv3 区块链电子发票，<b>5 端点</b>：开具 + 查询 + 冲红 +
/// 获取发票下载信息 + 插入用户卡包）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（<b>普通商户</b>文档中心，2026-10-09 逐页核验，更新时间均为 2025.09.26）：
/// 开具 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538301"/>、
/// 查询 <see href="https://pay.weixin.qq.com/docs/merchant/apis/fapiao/fapiao-applications/get-fapiao-applications.html"/>、
/// 冲红 <see href="https://pay.weixin.qq.com/docs/merchant/apis/fapiao/fapiao-applications/reverse-fapiao-applications.html"/>、
/// 获取下载信息 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538335"/>、
/// 插入卡包 <see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538365"/>。
/// 各页均标注<b>支持商户：【普通商户】</b>。
/// </para>
/// <para>
/// <b>✅ 更正设计方案 §2.5 的一条记录</b>：该处记「电子发票 · 普通商户文档中心<b>无独立入口</b>；
/// 服务商侧 <c>partner/4015792574</c>」—— <b>实测不成立</b>：普通商户侧有完整电子发票文档（本域各页均在其内）。
/// 服务商侧另有「开具<b>通用行业</b>电子发票」（<c>/fapiao-applications/<b>issue-general</b></c>，与商户侧不同路由）
/// 及其 22 项 API 清单，属另一套面，本域不覆盖。
/// </para>
/// <para>
/// <b>⚠️ 两个非 JSON 端点<b>刻意不在本生成式接口内</b></b>（它们不是「漏实现」）：
/// </para>
/// <list type="bullet">
/// <item>
/// <b>上传电子发票文件</b>（<c>POST /v3/new-tax-control-fapiao/fapiao-applications/upload-fapiao-file</c>，已核）：
/// 官方契约为 <c><b>multipart/form-data</b></c>（表单字段 <c>file</c> + <c>meta</c>），
/// 且 <c>meta.digest</c> 要求 <b>SM3</b> 摘要（官方字段名写作 <c>digest_alogrithm</c>，<b>少一个 r，属官方拼写</b>）——
/// <b>SM3 不在 .NET BCL 内</b>，须由调用方计算后传入 ⇒ 声明式 <c>[Body]</c> JSON 端点无法承载。
/// </item>
/// <item>
/// <b>下载发票文件</b>：官方注明该下载<b>不支持签名和验签</b>，且 URL 由「获取下载信息」动态给出
/// （官方《下载发票文件》页称有效期 <b>30s</b>）⇒ 与账单下载同属「动态 URL + 非 JSON」形态。
/// </item>
/// </list>
/// <para>
/// 当前落点：<see cref="GetFapiaoFilesAsync"/> 只<b>返回</b> <c>download_url</c>，宿主可自行取用；
/// 待后续按账单下载通道（<c>IWechatPayBillDownloadService</c>）的同款模式单独建模
/// （含主机白名单前置闸）。<b>不要凭推断补路由</b>。
/// </para>
/// <para>
/// <b>通知已覆盖（归回调包）</b>：<c>FAPIAO.USER_APPLIED</c>（抬头填写完成）、
/// <c>FAPIAO.ISSUED</c>（开具成功）、<c>FAPIAO.CARD_INSERTED</c>（插卡成功）、
/// <c>FAPIAO.REVERSED</c>（冲红成功）、<c>FAPIAO.CARD_DISCARDED</c>（卡券作废）——
/// 见 <c>WechatPayCallbackContext.GetFapiao</c> / <c>GetFapiaoUserApplied</c>。
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

    /// <summary>
    /// 冲红电子发票。官方文档：
    /// <see href="https://pay.weixin.qq.com/docs/merchant/apis/fapiao/fapiao-applications/reverse-fapiao-applications.html"/>。
    /// </summary>
    /// <param name="fapiaoApplyId">发票申请单号（官方 path <c>fapiao_apply_id</c>，必填 string(32)）。</param>
    /// <param name="request">冲红请求体，见 <see cref="FapiaoReverseRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/new-tax-control-fapiao/fapiao-applications/{fapiao_apply_id}/reverse</c>；
    /// 更新时间 2025.09.26，<b>支持商户：【普通商户】</b>。</para>
    /// <para>
    /// <b>无应答包体</b>：官方标注 <b>202 Accepted</b> 且无任何应答字段 ⇒ 返回 <c>Task</c>。
    /// 官方原文：「本接口成功返回<b>仅代表冲红请求已被受理</b>，当冲红完成时，微信支付会根据商户配置的
    /// 回调地址进行回调通知，商户也可以通过【查询电子发票】接口获取冲红结果及红票信息」。
    /// </para>
    /// <para>
    /// <b>重试约束（官方原文）</b>：红字发票开具<b>失败</b>时，可用<b>相同的</b>
    /// <c>fapiao_information.fapiao_id</c> 重新填写冲红原因<b>重试</b>，<b>每次重试间隔为 5 分钟</b>。
    /// </para>
    /// <para>
    /// <b>前置约束</b>：<b>仅</b>在微信支付侧开具的电子发票才允许冲红；冲红会同时把发票从用户卡包中删除。
    /// </para>
    /// </remarks>
    [Post("/v3/new-tax-control-fapiao/fapiao-applications/{fapiaoApplyId}/reverse")]
    Task ReverseFapiaoAsync(
        [Path] string fapiaoApplyId,
        [Body] FapiaoReverseRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取发票下载信息。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538335"/>。
    /// </summary>
    /// <param name="fapiaoApplyId">发票申请单号（官方 path <c>fapiao_apply_id</c>，必填 string(32)）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>发票下载信息列表（含 <c>download_url</c>），见 <see cref="FapiaoFilesResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/new-tax-control-fapiao/fapiao-applications/{fapiao_apply_id}/fapiao-files</c>；
    /// 仅 path 参数，无 body / query（更新时间 2025.09.26）。</para>
    /// <para>
    /// <b>⚠️ 有下载链接才算真能下载</b>：官方原文「开票状态为 <c>ISSUED</c> 的发票才能获取到发票文件下载链接」
    /// ⇒ <c>download_url</c> <b>仅当</b>该张发票 <c>status = ISSUED</c> 时存在（其它状态下为 <c>null</c>），
    /// 消费侧<b>不得</b>假定列表里每项都有链接。
    /// </para>
    /// <para>
    /// <b>本端点只返回地址，不下载</b>：文件须再用官方《下载发票文件》按其 <c>download_url</c> 取回
    /// （该下载<b>不支持签名和验签</b>，与普通 APIv3 请求不同）。本域暂未建模下载动作，
    /// 故 <c>download_url</c> 须由宿主自行取用（<b>严禁</b>改写或拼接该 URL）。
    /// </para>
    /// </remarks>
    [Get("/v3/new-tax-control-fapiao/fapiao-applications/{fapiaoApplyId}/fapiao-files")]
    Task<FapiaoFilesResponse> GetFapiaoFilesAsync(
        [Path] string fapiaoApplyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 将电子发票插入微信用户卡包。官方文档：<see href="https://pay.weixin.qq.com/doc/v3/merchant/4012538365"/>。
    /// </summary>
    /// <param name="fapiaoApplyId">发票申请单号（官方 path <c>fapiao_apply_id</c>，必填 string(32)）。</param>
    /// <param name="request">插卡请求体，见 <see cref="FapiaoInsertCardsRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/new-tax-control-fapiao/fapiao-applications/{fapiao_apply_id}/insert-cards</c>；
    /// 更新时间 2025.09.26，<b>支持商户：【普通商户】</b>。</para>
    /// <para>
    /// <b>无应答包体</b>：官方标注 <b>202 Accepted</b> ⇒ 返回 <c>Task</c>；官方原文「返回成功仅代表请求被受理，
    /// 插卡完成后由微信支付回调通知，也可通过【查询电子发票】接口获取插卡结果及卡券信息」。
    /// </para>
    /// <para>
    /// <b>两段式前置（官方原文）</b>：调用本接口前<b>必须</b>先调《上传电子发票文件》拿到
    /// <c>fapiao_media_id</c>（<b>三天内有效</b>）—— 该上传接口是 <c>multipart/form-data</c>，
    /// <b>不是</b> JSON 端点，故不在本生成式接口内。
    /// </para>
    /// <para>
    /// <b>非微信支付场景</b>：须先经《获取用户授权链接》取得用户授权后才能调用本接口；
    /// 微信支付场景无需额外授权。
    /// </para>
    /// </remarks>
    [Post("/v3/new-tax-control-fapiao/fapiao-applications/{fapiaoApplyId}/insert-cards")]
    Task InsertFapiaoCardsAsync(
        [Path] string fapiaoApplyId,
        [Body] FapiaoInsertCardsRequest request,
        CancellationToken cancellationToken = default);
}
