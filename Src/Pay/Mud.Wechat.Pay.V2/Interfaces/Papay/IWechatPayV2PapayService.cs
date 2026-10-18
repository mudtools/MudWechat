// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2 <b>委托代扣（PAP）</b>域，直连商户侧 6 支端点
/// （支付中签约 / 扣款 / APP 预签约 / H5 预签约 / 查签约 / 解约）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（APIv2 商户侧，2026-10-18 逐页核验）：
/// 解约 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011987432"/>、
/// H5 预签约 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011987295"/>、
/// APP 预签约 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011986804"/>、
/// 查签约 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011987640"/>、
/// 支付中签约 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011987320"/>、
/// 扣款 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011987377"/>。</para>
/// <para><b>全部 6 支均不需商户证书</b>（与 <see cref="IWechatPayV2SecureService"/> 的 <c>secapi</c> 族不同），
/// 故整族走声明式生成，挂在 APIv2 命名客户端（<c>api.mch.weixin.qq.com</c>）上，
/// 报文与签名规则同 <see cref="IWechatPayV2Service"/>：XML + 报文体 <c>sign</c>、不声明 <c>[Token]</c>。</para>
/// <para><b>H5 预签约是本族唯一的 GET</b>：参数与 <c>sign</c> 都在<b>查询串</b>里，
/// 由 <c>WechatPayV2SigningHandler</c> 的查询串签名分支补写 ⇒ 入参以 <c>[Query]</c> 逐参声明，
/// 不给请求 DTO。该页 <c>sign_type</c> 缺省时官方默认 <b>HMAC-SHA256</b>（本族 POST 默认 MD5），
/// 故方法把默认实参写成 <c>"HMAC-SHA256"</c> 而不是留空 —— 留空会让传输层按 MD5 签、
/// 微信侧按 HMAC-SHA256 验，症状是每次都 <c>SIGNERROR</c>。</para>
/// <para><b>应答语义</b>：业务失败同样用 <b>HTTP 200</b> + <c>return_code=FAIL</c> 表达，
/// 判定看报文体里的两级标识，不看状态码（故标 <c>[AllowAnyStatusCode]</c>）。</para>
/// <para><b>权限前提</b>：H5 预签约官方页面注明「该业务接口暂未全面开放，需单独申请后方可使用」。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "PapayV2", HttpClient = WechatPayV2HttpClientNames.TypeName)]
public interface IWechatPayV2PapayService
{
    /// <summary>
    /// 支付中签约（统一下单与签约合并，APIv2）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011987320"/>。
    /// </summary>
    /// <param name="request">支付中签约请求体，见 <see cref="V2ContractOrderRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>受理结果（<c>prepay_id</c> / <c>contract_result_code</c>），见 <see cref="V2ContractOrderResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/pay/contractorder</c>；XML 报文，MD5/HMAC-SHA256 签名。</para>
    /// <para><b>双异步回调</b>：支付结果走 <c>notify_url</c>、签约结果走 <c>contract_notify_url</c> ⇒
    /// 应答只代表受理，两笔结果都要等回调；<b>签名在 URL encode 之前计算</b>（官方原文）。</para>
    /// </remarks>
    [Post("/pay/contractorder")]
    Task<V2ContractOrderResponse> ContractOrderAsync(
        V2ContractOrderRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 委托代扣扣款（APIv2）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011987377"/>。
    /// </summary>
    /// <param name="request">扣款请求体，见 <see cref="V2PapPayApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>受理结果，见 <see cref="V2PapPayApplyResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/pay/pappayapply</c>；XML 报文，签名同上。<c>trade_type</c> 恒为 <c>PAP</c>。</para>
    /// <para><b>频率上限</b>：150qps。<b>异步受理</b>：<c>err_code = ORDER_ACCEPTED</c> 表示已受理，
    /// <b>不得重复发起</b>；扣款结果以 <c>notify_url</c> 异步通知为准。</para>
    /// <para><b>协议类错误</b>：<c>CONTRACT_NOT_EXIST</c> 协议不存在、<c>CONTRACTERROR</c> 协议已过期。</para>
    /// </remarks>
    [Post("/pay/pappayapply")]
    Task<V2PapPayApplyResponse> PapPayApplyAsync(
        V2PapPayApplyRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// APP 预签约（APIv2，返回预签约 id 交 SDK 调起）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011986804"/>。
    /// </summary>
    /// <param name="request">APP 预签约请求体，见 <see cref="V2PreEntrustWebRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>预签约结果（<c>pre_entrustweb_id</c> 等），见 <see cref="V2PreEntrustWebResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/papay/preentrustweb</c>；XML 报文；<c>sign_type</c> 缺省默认 <b>MD5</b>。</para>
    /// <para><b>有效期</b>：<c>pre_entrustweb_id</c> <b>2 小时</b>内有效。</para>
    /// <para><b>签约通道分叉</b>：模板为 2025-09-23 及之后申请、或已申请 <c>WXLaunchMiniProgram</c> 权限时
    /// 返回 <c>miniprogram_username</c>/<c>miniprogram_path</c> ⇒ 走 LaunchMiniProgram SDK；
    /// 未返回则只能 OpenBusinessWebview SDK。<b>两个值都必须原样填入</b>，不得改写。</para>
    /// </remarks>
    [Post("/papay/preentrustweb")]
    Task<V2PreEntrustWebResponse> PreEntrustWebAsync(
        V2PreEntrustWebRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// H5 预签约（APIv2，返回签约页跳转 URL）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011987295"/>。
    /// </summary>
    /// <param name="appId">应用 ID（Query <c>appid</c>，必填 string(32)）：直连商户申请的公众号或移动应用 appid。</param>
    /// <param name="mchId">商户号（Query <c>mch_id</c>，必填 string(32)）。</param>
    /// <param name="planId">协议模板 ID（Query <c>plan_id</c>，必填 string(28)）。</param>
    /// <param name="contractCode">商户侧签约协议号（Query <c>contract_code</c>，必填 string(128)）：只能数字与大小写字母，同一商户号下唯一。</param>
    /// <param name="requestSerial">商户请求签约序列号（Query <c>request_serial</c>，必填 int64）：唯一、禁止 0 开头、纯数字。</param>
    /// <param name="contractDisplayAccount">签约用户名称（Query <c>contract_display_account</c>，必填 string(128)）：签约页「开通账号」展示；不支持 UTF8 非 3 字节编码字符，<b>勿传微信昵称</b>。</param>
    /// <param name="notifyUrl">签约成功回调地址（Query <c>notify_url</c>，必填 string(256)）：http/https 开头、外网可访问、不携带参数。</param>
    /// <param name="timestamp">系统当前时间（Query <c>timestamp</c>，必填 string(10)）：10 位秒级时间戳。</param>
    /// <param name="clientIp">用户客户端 IP（Query <c>clientip</c>，必填 string(32)）：同时支持 IPV4 与 IPV6。</param>
    /// <param name="version">版本（Query <c>version</c>，必填 string(8)）：固定值 <c>1.0</c>，默认已填。</param>
    /// <param name="signType">签名算法（Query <c>sign_type</c>，选填 string(32)）：<c>MD5</c> 或 <c>HMAC-SHA256</c>；<b>官方本页缺省即 HMAC-SHA256</b>，故默认显式传 HMAC-SHA256。</param>
    /// <param name="returnAppid">签约完成后返回的应用 ID（Query <c>return_appid</c>，选填 string(32)）：需模板具有指定返回 app 权限。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>签约发起结果（<c>redirect_url</c>），见 <see cref="V2H5EntrustWebResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/papay/h5entrustweb</c>；参数与 <c>sign</c> 均在查询串。</para>
    /// <para><b>查询串签名</b>：无报文体 ⇒ 由 <c>WechatPayV2SigningHandler</c> 的 Query 分支按
    /// <b>解码后原值</b>排序拼串后追加 <c>sign</c> 参数（与体侧同一算法，只是载体换成 URL）。</para>
    /// <para><b>取值编码边界</b>：组件层 URL 校验以 URI 的<b>显示形态</b>判定合法性，
    /// 含非 ASCII 的查询值会在进入本线 Handler 前被判为非法 URL ⇒
    /// <c>contract_display_account</c> 一类展示字段请传 ASCII（官方亦要求 3 字节 UTF8 内、禁表情符号）。
    /// 该判定属上游 <c>Mud.HttpUtils</c> 行为，本线不绕过。</para>
    /// <para><b>前端约束（官方硬性）</b>：页面须加
    /// <c>&lt;meta name="referrer" content="no-referrer-when-downgrade"&gt;</c>；
    /// 跳转<b>不得</b>用 <c>window.location.replace</c>，须用 <c>window.location.href =</c>。</para>
    /// <para><b>有效期</b>：<c>redirect_url</c> <b>10 分钟</b>；请求页面地址须在微信后台（申请 H5 签约权限时）配置。</para>
    /// <para><b>通知条件</b>：仅<b>签约成功</b>才异步通知 <c>notify_url</c>，失败不通知。</para>
    /// </remarks>
    [Get("/papay/h5entrustweb")]
    Task<V2H5EntrustWebResponse> H5EntrustWebAsync(
        [Query("appid")] string appId,
        [Query("mch_id")] string mchId,
        [Query("plan_id")] string planId,
        [Query("contract_code")] string contractCode,
        [Query("request_serial")] long requestSerial,
        [Query("contract_display_account")] string contractDisplayAccount,
        [Query("notify_url")] string notifyUrl,
        [Query("timestamp")] string timestamp,
        [Query("clientip")] string clientIp,
        [Query("version")] string version = "1.0",
        [Query("sign_type")] string signType = "HMAC-SHA256",
        [Query("return_appid")] string? returnAppid = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询委托代扣签约关系（APIv2）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011987640"/>。
    /// </summary>
    /// <param name="request">查签约请求体，见 <see cref="V2QueryContractRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>签约关系详情（含 <c>contract_state</c>），见 <see cref="V2QueryContractResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/papay/querycontract</c>；XML 报文。<b>频率上限</b>：300qps。</para>
    /// <para><b>两种查询模式</b>：<c>contract_id</c> 单键，或 <c>plan_id</c> + <c>contract_code</c> 组合键，返回结果相同。</para>
    /// <para><b>状态枚举</b>：<c>contract_state</c> = <c>0</c> 已签约 / <c>1</c> 未签约 / <c>9</c> 签约进行中；
    /// 解约相关字段仅在 <c>state=1</c> 时有效。</para>
    /// </remarks>
    [Post("/papay/querycontract")]
    Task<V2QueryContractResponse> QueryContractAsync(
        V2QueryContractRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 解除委托代扣签约协议（APIv2）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011987432"/>。
    /// </summary>
    /// <param name="request">解约请求体，见 <see cref="V2DeleteContractRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>解约结果，见 <see cref="V2DeleteContractResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/papay/deletecontract</c>；XML 报文。
    /// 该 path <b>商户侧与服务商侧共用</b>（服务商文档给的也是 <c>/papay/deletecontract</c>，
    /// 无 <c>/papay/partner/</c> 段），差异只在字段面（见 <see cref="IWechatPayV2PartnerPapayService"/>）。</para>
    /// <para><b>两种解约模式</b>：<c>contract_id</c> 或 <c>plan_id</c> + <c>contract_code</c>。</para>
    /// <para><b>解约回调</b>：商户可在商户后台设置解约回调地址，解约时微信通知解约信息，内容与签约返回一致。</para>
    /// </remarks>
    [Post("/papay/deletecontract")]
    Task<V2DeleteContractResponse> DeleteContractAsync(
        V2DeleteContractRequest request, CancellationToken cancellationToken = default);
}
