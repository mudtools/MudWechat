// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2 <b>委托代扣（PAP）服务商侧</b> 5 支端点
/// （H5 预签约 / APP 预签约 / 查签约 / 解约 / 扣款）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（APIv2 服务商侧，2026-10-18 逐页核验）：
/// 解约 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011988374"/>、
/// H5 预签约 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011988368"/>、
/// APP 预签约 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011988366"/>、
/// 查签约 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011988379"/>、
/// 扣款 <see href="https://pay.weixin.qq.com/doc/v2/partner/4011988372"/>。</para>
/// <para><b>全部 5 支均不需商户证书</b> ⇒ 整族走声明式生成（同 <see cref="IWechatPayV2PapayService"/>）。</para>
/// <para>
/// <b>与商户侧的路由关系</b>：签约查询与扣款各带 <c>/partner</c> 段
/// （<c>/papay/partner/querycontract</c>、<c>/pay/partner/pappayapply</c>），
/// 但<b>解约两支不带</b> —— <c>/papay/deletecontract</c> 与商户侧<b>同一路径</b>、
/// H5 与 APP 预签约则确实是 <c>/papay/partner/*</c>。这是官方文档的原始形态，不是笔误，
/// 故本族 DTO 与商户侧<b>不共用</b>（字段面多了 <c>sub_appid</c>/<c>sub_mch_id</c>），路径共用。
/// </para>
/// <para><b>H5 预签约是本族唯一的 GET</b>，查询串签名与 <c>sign_type</c> 默认 HMAC-SHA256 的口径
/// 同 <see cref="IWechatPayV2PapayService.H5EntrustWebAsync"/>。</para>
/// <para><b>权限前提</b>：H5 预签约官方页面注明「暂未全面开放，需单独申请」。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PartnerPapayV2", HttpClient = WechatPayV2HttpClientNames.TypeName)]
public interface IWechatPayV2PartnerPapayService
{
    /// <summary>
    /// 服务商委托代扣扣款（APIv2）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4011988372"/>。
    /// </summary>
    /// <param name="request">扣款请求体，见 <see cref="V2PartnerPapPayApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>受理结果，见 <see cref="V2PartnerPapPayApplyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/pay/partner/pappayapply</c>；XML 报文；<c>trade_type</c> 恒为 <c>PAP</c>。</para>
    /// <para><b>频率上限</b>：150qps；同一商户 + 同一订单号<b>只扣一次</b>（幂等），
    /// <c>err_code = ORDER_ACCEPTED</c> 表示已受理、勿重复发起。</para>
    /// <para><b>扣款规则</b>：顺序为先零钱、次信用卡/借记卡；新签约 12 小时内可直接扣款，
    /// 之后遵循预约规则，预约扣款限北京时间 <b>07:00–22:00</b>；周期性扣款需提前通知用户。</para>
    /// <para><b>与商户侧字段差异</b>：多 <c>sub_appid</c>/<c>sub_mch_id</c>/<c>receipt</c>，
    /// 且 <c>spbill_create_ip</c> 本页为<b>必填</b>（商户页为选填）。</para>
    /// </remarks>
    [Post("/pay/partner/pappayapply")]
    Task<V2PartnerPapPayApplyResponse> PapPayApplyAsync(
        V2PartnerPapPayApplyRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商 APP 预签约（APIv2）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4011988366"/>。
    /// </summary>
    /// <param name="request">APP 预签约请求体，见 <see cref="V2PartnerPreEntrustWebRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>预签约结果（<c>pre_entrustweb_id</c> 等），见 <see cref="V2PartnerPreEntrustWebResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/papay/partner/preentrustweb</c>；XML 报文；<c>sign_type</c> 缺省默认 <b>MD5</b>。</para>
    /// <para><b>有效期</b>：<c>pre_entrustweb_id</c> <b>2 小时</b>内有效；
    /// 签约通道分叉（<c>miniprogram_username</c>/<c>miniprogram_path</c>）口径同商户侧
    /// <see cref="IWechatPayV2PapayService.PreEntrustWebAsync"/>。</para>
    /// </remarks>
    [Post("/papay/partner/preentrustweb")]
    Task<V2PartnerPreEntrustWebResponse> PreEntrustWebAsync(
        V2PartnerPreEntrustWebRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商 H5 预签约（APIv2，返回签约页跳转 URL）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4011988368"/>。
    /// </summary>
    /// <param name="appId">应用 ID（Query <c>appid</c>，必填 string(32)）：服务商申请的公众号或移动应用 appid。</param>
    /// <param name="mchId">服务商商户号（Query <c>mch_id</c>，必填 string(32)）。</param>
    /// <param name="subMchId">子商户号（Query <c>sub_mch_id</c>，必填 string(32)）。</param>
    /// <param name="planId">协议模板 ID（Query <c>plan_id</c>，必填 string(28)）。</param>
    /// <param name="contractCode">商户侧签约协议号（Query <c>contract_code</c>，必填 string(128)）：只能数字与大小写字母，同一商户号下唯一。</param>
    /// <param name="requestSerial">商户请求签约序列号（Query <c>request_serial</c>，必填 int64）：唯一、禁止 0 开头、纯数字。</param>
    /// <param name="contractDisplayAccount">签约用户名称（Query <c>contract_display_account</c>，必填 string(128)）：不支持 UTF8 非 3 字节编码字符。</param>
    /// <param name="notifyUrl">签约成功回调地址（Query <c>notify_url</c>，必填 string(256)）：外网可访问、不携带参数；须对参数值 encode。</param>
    /// <param name="timestamp">系统当前时间（Query <c>timestamp</c>，必填 string(10)）。</param>
    /// <param name="clientIp">用户客户端 IP（Query <c>clientip</c>，必填 string(32)）：同时支持 IPV4 与 IPV6。</param>
    /// <param name="version">版本（Query <c>version</c>，必填 string(8)）：固定值 <c>1.0</c>，默认已填。</param>
    /// <param name="signType">签名算法（Query <c>sign_type</c>，选填 string(32)）：<b>官方本页缺省即 HMAC-SHA256</b>，故默认显式传该值。</param>
    /// <param name="subAppid">子商户绑定的 appid（Query <c>sub_appid</c>，选填 string(32)）：需服务商在商户平台为子商户绑定。</param>
    /// <param name="deviceId">设备号（Query <c>deviceid</c>，选填 string(32)）：Android 填 IMEI 一次 MD5、iOS 填 IDFA 一次 MD5；风控参数，官方建议填写。</param>
    /// <param name="mobile">用户手机号（Query <c>mobile</c>，选填 string(11)）：风控参数。</param>
    /// <param name="email">用户邮箱（Query <c>email</c>，选填）：风控参数。</param>
    /// <param name="qq">用户 QQ 号（Query <c>qq</c>，选填）：风控参数。</param>
    /// <param name="openId">用户微信 openid（Query <c>openid</c>，选填 string(128)）：风控参数。</param>
    /// <param name="creid">用户身份证号（Query <c>creid</c>，选填）：风控参数。</param>
    /// <param name="outerId">用户在商户侧的标识（Query <c>outerid</c>，选填）：风控参数。</param>
    /// <param name="returnAppid">签约完成后返回的应用 ID（Query <c>return_appid</c>，选填 string(32)）：需模板具有指定返回 app 权限。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>签约发起结果（<c>redirect_url</c>），见 <see cref="V2PartnerH5EntrustWebResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/papay/partner/h5entrustweb</c>；参数与 <c>sign</c> 均在查询串。签名口径与商户侧同：以解码后原值拼串，由 Handler 的 Query 分支追加 <c>sign</c>。</para>
    /// <para><b>取值编码边界</b>：组件层 URL 校验以 URI 显示形态判定合法性 ⇒ 展示类参数请传 ASCII（详见商户侧同名方法注释）。</para>
    /// <para><b>风控参数是用户身份信息</b>（<c>mobile</c>/<c>email</c>/<c>creid</c> 等）：本 SDK 不落日志、
    /// 不入库，宿主须自行确保收集与向微信支付传输已合法征得用户授权。</para>
    /// <para><b>前端约束（官方硬性）</b>：须加
    /// <c>&lt;meta name="referrer" content="no-referrer-when-downgrade"&gt;</c>；
    /// 跳转禁用 <c>window.location.replace</c>，须用 <c>window.location.href =</c>。</para>
    /// <para><b>通知条件</b>：仅签约成功才异步通知 <c>notify_url</c>。</para>
    /// </remarks>
    [Get("/papay/partner/h5entrustweb")]
    Task<V2PartnerH5EntrustWebResponse> H5EntrustWebAsync(
        [Query("appid")] string appId,
        [Query("mch_id")] string mchId,
        [Query("sub_mch_id")] string subMchId,
        [Query("plan_id")] string planId,
        [Query("contract_code")] string contractCode,
        [Query("request_serial")] long requestSerial,
        [Query("contract_display_account")] string contractDisplayAccount,
        [Query("notify_url")] string notifyUrl,
        [Query("timestamp")] string timestamp,
        [Query("clientip")] string clientIp,
        [Query("version")] string version = "1.0",
        [Query("sign_type")] string signType = "HMAC-SHA256",
        [Query("sub_appid")] string? subAppid = null,
        [Query("deviceid")] string? deviceId = null,
        [Query("mobile")] string? mobile = null,
        [Query("email")] string? email = null,
        [Query("qq")] string? qq = null,
        [Query("openid")] string? openId = null,
        [Query("creid")] string? creid = null,
        [Query("outerid")] string? outerId = null,
        [Query("return_appid")] string? returnAppid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商查询委托代扣签约关系（APIv2）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4011988379"/>。
    /// </summary>
    /// <param name="request">查签约请求体，见 <see cref="V2PartnerQueryContractRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>签约关系详情（含 <c>contract_state</c> / <c>sub_openid</c>），见 <see cref="V2PartnerQueryContractResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/papay/partner/querycontract</c>；XML 报文。<b>频率上限</b>：默认 300qps。</para>
    /// <para><b>两种查询模式</b>：<c>contract_id</c> 或 <c>plan_id</c> + <c>contract_code</c>，返回结果相同。</para>
    /// </remarks>
    [Post("/papay/partner/querycontract")]
    Task<V2PartnerQueryContractResponse> QueryContractAsync(
        V2PartnerQueryContractRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 服务商解除委托代扣签约协议（APIv2）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4011988374"/>。
    /// </summary>
    /// <param name="request">解约请求体，见 <see cref="V2PartnerDeleteContractRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>解约结果，见 <see cref="V2PartnerDeleteContractResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/papay/deletecontract</c>（服务商文档给的 path 与商户侧<b>完全相同</b>，
    /// 无 <c>/papay/partner/</c> 段）；请求体靠 <c>sub_mch_id</c> 区分归属。</para>
    /// <para><b>两种解约模式</b>：<c>contract_id</c> 或 <c>plan_id</c> + <c>contract_code</c>。</para>
    /// </remarks>
    [Post("/papay/deletecontract")]
    Task<V2PartnerDeleteContractResponse> DeleteContractAsync(
        V2PartnerDeleteContractRequest request, CancellationToken cancellationToken = default);
}
