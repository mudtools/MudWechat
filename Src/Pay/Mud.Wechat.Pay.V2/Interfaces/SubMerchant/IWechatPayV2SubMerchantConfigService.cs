// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2 <b>服务商子商户配置</b>域 3 支端点（支付目录新增 / 查询 + 关注配置新增）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（APIv2 合作伙伴侧，2026-10-18 逐页核验）：
/// 新增子商户支付目录 <see href="https://pay.weixin.qq.com/doc/v2/partner/4014115404"/>、
/// 查询子商户配置 <see href="https://pay.weixin.qq.com/doc/v2/partner/4014115413"/>、
/// 新增子商户关注配置 <see href="https://pay.weixin.qq.com/doc/v2/partner/4014115399"/>。
/// 三页的「是否需要证书」栏均为<b>「是」</b>。</para>
/// <para><b>三支都要证书、却都挂在 <c>api.mch</c></b>：三页「接口说明」栏给的是
/// <c>https://api.mch.weixin.qq.com/secapi/mch/*</c> 与 <c>/secapi/mkt/*</c>，
/// <b>不</b>是 <c>api2.mch</c> ⇒ 本接口走 <see cref="WechatPayV2HttpClientNames.BaseAddress"/>。
/// 「<c>/secapi/</c> 前缀」不等于「证书域名」，这是本线已第二次出现的口径（同 <see cref="IWechatPayV2DepositSecureService"/>）。</para>
/// <para><b>为什么手写</b>：客户端证书按<b>商户</b>绑定，与「一条命名客户端一个连接池」的生成式模型冲突
/// （理由同 <see cref="IWechatPayV2SecureService"/>），故由 <c>WechatPayV2SecureInvoker</c>
/// 走逐商户证书通道；报文规则与声明式管道完全同源。</para>
/// <para><b>三支的签名面不统一，勿抽公共基类</b>：目录新增/查询两支<b>没有</b> <c>nonce_str</c>
/// 也<b>没有</b> <c>sign_type</c>（官方页全文无该二字 ⇒ 落到默认的 MD5 口径）；
/// 关注配置一支 <c>nonce_str</c> 与 <c>sign_type</c> 均<b>必填</b>，且官方原文
/// 「暂只支持HMAC-SHA256算法。该字段需参与签名sign的计算。」</para>
/// <para><b>不出密钥</b>（PAY-B7）：异常消息只含商户键、路由与官方返回字段名，
/// 不含密钥、<c>sign</c>、签名原文或证书材料。</para>
/// </remarks>
public interface IWechatPayV2SubMerchantConfigService
{
    /// <summary>
    /// 新增子商户 JSAPI 支付授权目录（APIv2，需证书）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4014115404"/>。
    /// </summary>
    /// <param name="request">配置请求体，见 <see cref="V2SubDevConfigAddRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>配置结果，见 <see cref="V2SubDevConfigAddResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/secapi/mch/addsubdevconfig</c>；XML；请求面 5 行
    /// （<c>appid</c>/<c>mch_id</c>/<c>sub_mch_id</c>/<c>jsapi_path</c>/<c>sign</c>），
    /// 全部必填，<b>没有</b> <c>nonce_str</c> 与 <c>sign_type</c>。</para>
    /// <para><b>只增不改不删</b>：官方原文「API只支持新增配置，不支持修改，如银行需要修改请先登录微信服务商后台手工删除后重新配置」
    /// ⇒ 本域<b>没有</b>删除端点（官方无该 HTTP 面），配错只能回后台。</para>
    /// <para><b>每次一个目录、每商户最多 5 个</b>：官方硬上限，第 6 次新增即失败 ⇒
    /// 批量刷目录时须自行计数，不要指望服务端截断。</para>
    /// <para><b>应答 <c>sign</c> 必填</b>：官方描述「微信返回的签名，银行服务商需验证签名」⇒
    /// 本 SDK 对标了 <c>sign</c> 的应答一律验签，不做「可选校验」。</para>
    /// </remarks>
    Task<V2SubDevConfigAddResponse> AddSubDevConfigAsync(
        V2SubDevConfigAddRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询子商户支付目录与 APPID 绑定关系（APIv2，需证书）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4014115413"/>。
    /// </summary>
    /// <param name="request">查询请求体，见 <see cref="V2SubDevConfigQueryRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>配置列表原文，见 <see cref="V2SubDevConfigQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/secapi/mch/querysubdevconfig</c>；XML；
    /// 请求面 4 行（比新增少一个 <c>jsapi_path</c>），同样<b>没有</b> <c>nonce_str</c> 与 <c>sign_type</c>。</para>
    /// <para><b>两支列表字段是「JSON 文本塞进 XML 字符串字段」</b>：<c>jsapi_path_list</c> 与
    /// <c>appid_config_list</c> 官方类型均为 String ⇒ 本 SDK 原样交回字符串，
    /// 不猜结构、不做二次解析（与 <see cref="V2ViolationRecordQueryResponse.ViolationRecords"/> 同一处置口径）。</para>
    /// <para><b>解析侧要容忍两处官方自身的不一致</b>：① 官方示例把键印成 <c>" subscribe_appid "</c>（带空格），
    /// 那是示例笔误、不是字段名；② 说明文字用大写 <c>NULL</c> 作「不区分绑定」的哨兵，示例却给小写
    /// <c>"null"</c> ⇒ 两种写法都可能出现。</para>
    /// </remarks>
    Task<V2SubDevConfigQueryResponse> QuerySubDevConfigAsync(
        V2SubDevConfigQueryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 新增子商户关注/推荐配置（APIv2，需证书，<b>仅 HMAC-SHA256</b>）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/partner/4014115399"/>。
    /// </summary>
    /// <param name="request">配置请求体，见 <see cref="V2RecommendConfigAddRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>配置结果，见 <see cref="V2RecommendConfigAddResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/secapi/mkt/addrecommendconf</c>；XML。
    /// <b>路径段是 <c>mkt</c> 不是 <c>mch</c></b> —— 与同域另两支只差这一段，是最易写错的一处。</para>
    /// <para><b>本支请求面没有 <c>appid</c></b>：主体标识只有 <c>mch_id</c> + <c>sub_mch_id</c>（官方表 8 行），照抄不得补。</para>
    /// <para><b>签名面与同域另两支相反</b>：<c>nonce_str</c> 必填、<c>sign_type</c> 必填且
    /// 「暂只支持HMAC-SHA256算法。该字段需参与签名sign的计算。」⇒
    /// <see cref="V2RecommendConfigAddRequest.SignType"/> 的默认值 <c>HMAC-SHA256</c> 属契约面：
    /// 清空它会把报文降成 MD5 而被官方拒。</para>
    /// <para><b><c>sub_appid</c> 无绑定关系时要传大写哨兵</b>：官方原文「如果没有填 sub_appid，此处请填值 NULL」
    /// ⇒ 既不能省略也不能填空串。</para>
    /// <para><b><c>subscribe_appid</c> 与 <c>receipt_appid</c> 二选一</b>：前者为推荐关注的公众号 APPID，
    /// 后者为支付凭证推荐小程序 appid（须通过微信认证且认证主体与服务商主体一致）。</para>
    /// </remarks>
    Task<V2RecommendConfigAddResponse> AddRecommendConfigAsync(
        V2RecommendConfigAddRequest request, CancellationToken cancellationToken = default);
}
