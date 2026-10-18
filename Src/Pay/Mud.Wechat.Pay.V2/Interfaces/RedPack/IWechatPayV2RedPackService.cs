// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2 现金红包族（<b>5 支端点，全部需商户 API 证书</b>）：
/// 发放红包 / 发放小程序红包 / 查询红包记录 / 发放企业红包 / 查询企业红包记录。
/// </summary>
/// <remarks>
/// <para>
/// <b>为什么不走声明式生成</b>：本族五支官方全部要求 TLS 握手期呈现<b>商户 API 证书</b>，
/// 而证书按商户绑定、<c>IHttpClientFactory</c> 的主 Handler 与连接池按<b>客户端名</b>复用
/// ⇒ 一条命名客户端承载不了多商户证书。故与 <see cref="IWechatPayV2SecureService"/> 同一形态，
/// 由 <c>WechatPayV2RedPackService</c> 走<b>每商户一条独立通道</b>，报文与签名规则完全同源。
/// </para>
/// <para>
/// <b>域名是 <c>api.mch</c> 而不是 <c>api2.mch</c></b>：这是本族最容易被「顺手对齐」搞错的一处 ——
/// <c>/secapi/pay/*</c> 五支在 <c>api2.mch</c>，红包五支官方给的是 <c>api.mch</c> 却<b>同样要证书</b>。
/// 发到 <c>api2.mch</c> 是 404，退到无证书命名客户端则握手期不呈现证书被拒。
/// </para>
/// <para>
/// <b>双签</b>：发放企业红包（<see cref="SendWorkWxRedPackAsync"/>）在 <c>sign</c> 之外还要求
/// <c>workwx_sign</c>，密钥是<b>请求里 <c>agentid</c> 对应的企业微信自建应用 Secret</b>而非商户 APIv2 密钥；
/// 该 Secret <b>不进支付配置面</b>，由宿主实现 <c>IWechatPayV2WorkWxSecretProvider</c> 桥接，
/// 未注册即 fail-fast。算法、字段白名单与「先 <c>workwx_sign</c> 后 <c>sign</c>」的顺序见
/// <c>WechatPayV2WorkWxSigner</c>。
/// </para>
/// <para><b>凭据前提</b>：须为该商户配置 <c>ApiV2KeySecretName</c> 与 <c>ApiV2CertificateSecretName</c>（口令默认取商户号，
/// 改过则另配 <c>ApiV2CertificatePasswordSecretName</c>）；缺失即 fail-fast 并点名配置项。</para>
/// <para>
/// <b>不出敏感信息</b>（PAY-B7 同线红线）：<c>re_openid</c> 与 <c>risk_info</c> 是用户身份/风控信息，
/// SDK 侧不落日志、不进异常消息；异常只含商户键、路由与官方返回字段名。
/// </para>
/// </remarks>
public interface IWechatPayV2RedPackService
{
    /// <summary>
    /// 发放现金红包（APIv2，需商户证书）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011974053"/>（服务商版
    /// <see href="https://pay.weixin.qq.com/doc/v2/partner/4011983137"/>）。
    /// </summary>
    /// <param name="request">发放请求体，见 <see cref="V2SendRedPackRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>发放结果，含红包流水号 <c>send_listid</c>，见 <see cref="V2SendRedPackResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/mmpaymkttransfers/sendredpack</c>（域名 <c>api.mch</c>）；
    /// XML 报文 + 报文内 <c>sign</c> + <b>双向证书</b>。</para>
    /// <para><b>资金安全</b>：本端点<b>直接出资</b>。<c>mch_billno</c> 是唯一幂等键 ⇒
    /// 失败或超时<b>必须沿用原单号</b>重试；换单号重试在 <c>SYSTEMERROR</c> 场景下会造成<b>重复出资</b>。</para>
    /// <para><b>官方限制</b>（2026-10-18 逐页核验）：发送频率默认 <b>30 次/秒</b>；单个红包 <b>1–200 元</b>；
    /// 商户<b>单日出资金额上限 100 万元</b>、<b>单用户单日收款上限 1000 元</b>、
    /// <b>单用户单日可领 10 个</b>；金额超出区间时 <c>scene_id</c> 变为必填。</para>
    /// <para><b>接入资格</b>：<b>新申请商户号</b>须「入驻超过 90 天 + 连续正常交易 30 天」才能发放，
    /// 且<b>移动应用 appid 不可用</b> ⇒ 未取得资格即调用只会拿到权限类错误，接入前先核对商户平台状态。</para>
    /// </remarks>
    Task<V2SendRedPackResponse> SendRedPackAsync(
        V2SendRedPackRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发放小程序红包（APIv2，需商户证书）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011937425"/>。
    /// </summary>
    /// <param name="request">发放请求体（<c>notify_way</c> 必填固定值 <c>MINI_PROGRAM_JSAPI</c>），见 <see cref="V2SendMiniProgramRedPackRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>发放结果，含小程序端领取用的 <c>package</c>，见 <see cref="V2SendMiniProgramRedPackResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/mmpaymkttransfers/sendminiprogramhb</c>；XML + <b>双向证书</b>。</para>
    /// <para><b>官方限制</b>：发送频率默认 <b>30 次/秒</b>；<b>新申请商户号</b>须「入驻超过 90 天 +
    /// 连续正常交易 30 天」方可使用小程序红包。</para>
    /// <para><b>与普通红包的差异</b>：应答额外带回 <c>package</c>，小程序端须把它原样传给
    /// <c>wx.openBusinessView</c> 才能唤起领取页；<b>不得</b>自行拼装或复用 <c>send_listid</c>。</para>
    /// </remarks>
    Task<V2SendMiniProgramRedPackResponse> SendMiniProgramRedPackAsync(
        V2SendMiniProgramRedPackRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询红包记录（APIv2，需商户证书）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011981612"/>（小程序红包语境同路径页
    /// <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011937431"/>）。
    /// </summary>
    /// <param name="request">查询请求体（<c>bill_type</c> 必填固定值 <c>MCHT</c>），见 <see cref="V2GetRedPackInfoRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>红包记录，见 <see cref="V2GetRedPackInfoResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/mmpaymkttransfers/gethbinfo</c>；XML + <b>双向证书</b>。</para>
    /// <para><b>时间窗口</b>：官方本页明确<b>仅支持查询 30 天内</b>的红包订单 ⇒ 超期查询不是数据缺失而是契约外请求。</para>
    /// <para><b>字段名陷阱</b>：本支请求参数是 <c>appid</c>（发放两支是 <c>wxappid</c>）⇒ 照抄官方，不做统一修正。</para>
    /// <para><b>应答形态陷阱</b>：裂变红包的领取明细 <c>hblist</c> 官方以 <b>JSON 字符串</b>塞进单个 XML 字段返回
    /// ⇒ 本 SDK 保持字符串交宿主解析（APIv2 扁平字段表承载不了嵌套结构）。</para>
    /// </remarks>
    Task<V2GetRedPackInfoResponse> GetRedPackInfoAsync(
        V2GetRedPackInfoRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发放企业红包（APIv2，需商户证书 <b>且需 <c>workwx_sign</c> 双签</b>）。
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90275"/>。
    /// </summary>
    /// <param name="request">发放请求体，见 <see cref="V2SendWorkWxRedPackRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>发放结果，见 <see cref="V2SendWorkWxRedPackResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/mmpaymkttransfers/sendworkwxredpack</c>；XML + <b>双向证书</b>
    /// + <c>sign</c> + <c>workwx_sign</c>。</para>
    /// <para><b>双签前提</b>：容器须注册 <c>IWechatPayV2WorkWxSecretProvider</c>（把 <c>agentid</c>
    /// 换成对应企业微信自建应用 Secret）；未注册或该 <c>agentid</c> 取不到 Secret 时<b>直接抛错</b>，
    /// 不降级为单签发出 —— 官方对这支强制双签，单签只会换回 <c>SIGNERROR</c> 并把配置缺失伪装成签名错误。</para>
    /// <para><b>官方限制</b>（企业微信开发者中心本页）：发送频率与发送个数上限默认 <b>1800 次/分钟</b>；
    /// 单个红包 <b>1–200 元</b>；商户<b>单日出资金额上限 100 万元</b>、
    /// <b>单用户单日收款上限 1000 元</b>、<b>单用户单日可领 10 个</b>。</para>
    /// <para><b>应答只有 <c>sign</c></b>：官方本页应答不带 <c>workwx_sign</c> ⇒ 验签按商户 APIv2 密钥单算。</para>
    /// </remarks>
    Task<V2SendWorkWxRedPackResponse> SendWorkWxRedPackAsync(
        V2SendWorkWxRedPackRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询企业红包记录（APIv2，需商户证书）。
    /// 官方文档：<see href="https://developer.work.weixin.qq.com/document/path/90276"/>。
    /// </summary>
    /// <param name="request">查询请求体，见 <see cref="V2QueryWorkWxRedPackRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>红包记录，见 <see cref="V2QueryWorkWxRedPackResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/mmpaymkttransfers/queryworkwxredpack</c>；XML + <b>双向证书</b>。</para>
    /// <para><b>时间窗口</b>：官方本页明确<b>仅支持查询 30 天内</b>的红包订单。</para>
    /// <para><b>不需双签</b>：查询族官方只要求 <c>sign</c> ⇒ 即使未注册企微 Secret 端口也能正常使用本支。</para>
    /// <para><b>字段名陷阱</b>：请求里的单号参数名是 <c>bill_no</c>（不是 <c>mch_billno</c>），照抄官方。</para>
    /// </remarks>
    Task<V2QueryWorkWxRedPackResponse> QueryWorkWxRedPackAsync(
        V2QueryWorkWxRedPackRequest request, CancellationToken cancellationToken = default);
}
