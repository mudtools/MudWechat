// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2.Transport;

/// <summary>APIv2 线命名 HttpClient 的名称、域名与注入类型名（供 <c>[HttpClientApi]</c> 引用）。</summary>
/// <remarks>
/// <para>
/// <b>与 APIv3 分占两个命名客户端的理由</b>：载体不同（APIv3 走 JSON + <c>Authorization</c> 头，
/// APIv2 走 XML + 报文体 <c>sign</c>），序列化器与签名 Handler 均不同；共享同一命名客户端会让两条线
/// 互相挂上门不对路的 Handler。类型名另占一个 DI 键的理由见 <c>WechatPayHttpClientNames</c>。
/// </para>
/// <para>
/// <b>域名已在 SSRF 白名单内</b>：本线四个域名 <c>api.mch</c>、<c>api2.mch</c>、<c>apihk.mch</c>、
/// <c>fraud.mch</c> 均由 <c>WechatApiHosts</c> 的 <c>weixin.qq.com</c> 一项以「等于域或以其子域结尾」的口径覆盖，
/// 本线<b>零改动</b>，也不给商户级 <c>BaseUrl</c> 覆盖留门。
/// </para>
/// <para>
/// <b>四个域名是「闭集」而不是「配置项」</b>：每个常量各自对应官方文档「接口说明」栏里的请求域名，
/// 由代码字面量选路，商户配置无从参与 —— 新增第五个域名的唯一入口是改这个类并同步
/// <c>WechatPayV2SecureChannelProvider</c> 的白名单校验（该通道不经 <c>IHttpClientFactory</c>，
/// 组件的 UrlValidator 不替我们把门，校验收口在 Provider 内）。
/// 「要客户端证书」与「挂在证书域名上」是两个<b>互相独立</b>的官方事实：
/// <c>/secapi/mch/addsubdevconfig</c> 要证书却挂 <c>api.mch</c>，故证书族按 (商户, 域名) 双键缓存通道。
/// </para>
/// </remarks>
public static class WechatPayV2HttpClientNames
{
    /// <summary>普通端点（<c>/pay/*</c>，无证书）的命名客户端名。</summary>
    public const string ClientName = "wechat-pay-v2";

    /// <summary>普通端点主域名（官方 <c>api.mch</c>，与 APIv3 同域）。</summary>
    public const string BaseAddress = "https://api.mch.weixin.qq.com";

    /// <summary>
    /// 涉密端点（<c>/secapi/pay/*</c> 与需证书的账单下载）的命名客户端名<b>前缀</b>。
    /// </summary>
    /// <remarks>
    /// 双向 TLS 的客户端证书<b>按商户</b>绑定，而 <c>IHttpClientFactory</c> 的主 Handler
    /// 按<b>客户端名</b>缓存并复用连接池 —— 一个共享名无法承载多商户证书。
    /// 故安全侧按 <c>{前缀}{merchantKey}</c> 命名，逐商户一条独立管道（含独立连接池），
    /// 由 <c>WechatPayV2SecureChannelProvider</c> 首次调用时惰性装配并缓存。
    /// </remarks>
    public const string SecureClientNamePrefix = "wechat-pay-v2-secure:";

    /// <summary>涉密端点域名（官方 <c>api2.mch</c>，强制双向 TLS）。</summary>
    public const string SecureBaseAddress = "https://api2.mch.weixin.qq.com";

    /// <summary>
    /// 境外/全球商户端点域名（官方 <c>apihk.mch</c>，同样强制双向 TLS）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>存在的理由</b>：跨境/境外产品线（境外机构进件子商户、商户媒体上传）的官方页面
    /// 「接口说明」栏<b>只印 <c>apihk.mch.weixin.qq.com</c>，全文不出现 <c>api.mch</c></b>
    /// （逐页核验：上传图片 <c>doc/global/v2/zh/4013641477</c>、
    /// 进件/修改/查询 <c>4013636642</c> / <c>4013636644</c> / <c>4013636643</c>）
    /// ⇒ 这两族<b>不能</b>复用 <see cref="BaseAddress"/>，否则请求打到国内域名上找不到路由。
    /// </para>
    /// <para>
    /// <b>判据取「接口说明」栏而非同页的示例 URL</b>：global 站部分页面在别处也印出
    /// <c>api.mch</c> 形态的地址，两处不一致时以本接口自身的接口说明栏为准并在此留痕。
    /// </para>
    /// </remarks>
    public const string GlobalBaseAddress = "https://apihk.mch.weixin.qq.com";

    /// <summary>
    /// 防欺诈侧端点域名（官方 <c>fraud.mch</c>，需双向 TLS）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>存在的理由</b>：获取防欺诈公钥（<c>/risk/getpublickey</c>）页的接口地址栏印的是
    /// <c>https://fraud.mch.weixin.qq.com/risk/getpublickey</c>，<b>既非 <c>api.mch</c> 亦非 <c>api2.mch</c></b>
    /// ⇒ 不能复用同前缀的违规记录查询（那一支官方给的是 <c>api.mch</c>）——
    /// 域名与路由前缀在 APIv2 是<b>两件独立的事</b>，各页各判。
    /// </para>
    /// <para>
    /// <b>与 <c>WechatApiHosts</c> 的关系</b>：该域仍是 <c>weixin.qq.com</c> 的子域，
    /// 故 SSRF 白名单零改动；但证书通道自建（不经 <c>IHttpClientFactory</c>），
    /// 必须同时补 <c>WechatPayV2SecureChannelProvider</c> 的闭集校验，两处共同构成第四个域名的接入口。
    /// </para>
    /// </remarks>
    public const string FraudBaseAddress = "https://fraud.mch.weixin.qq.com";

    /// <summary>
    /// 普通端点客户端类型的全限定 metadata name，供
    /// <c>[HttpClientApi(HttpClient = ...)]</c> 使用（生成器按 metadata name 精确查找，不能用 <c>nameof</c>）。
    /// </summary>
    public const string TypeName = "Mud.Wechat.Pay.V2.Transport.IWechatPayV2HttpClient";

    /// <summary>拼出某商户的安全通道客户端名。</summary>
    /// <param name="merchantKey">商户键（<c>WechatPayMerchantConfig.MerchantKey</c>）。</param>
    /// <returns>已注册的命名客户端名。</returns>
    public static string SecureClientName(string merchantKey) => SecureClientNamePrefix + merchantKey;
}
