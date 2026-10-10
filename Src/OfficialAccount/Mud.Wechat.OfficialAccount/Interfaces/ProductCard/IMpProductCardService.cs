// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「商品卡片」域 SDK（1 端点；端点位于 /channels/ec/ 视频号小店域前缀）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：<see href="https://developers.weixin.qq.com/doc/subscription/api/draftbox/shop/api_ecsgetproductcardinfo.html"/>
/// （官方接口英文名 <c>ecsgetproductcardinfo</c>；2026-10-07 逐页核验，subscription 订阅号域命中；
/// 页面标题「获取商品卡片的DOM结构」）。
/// </para>
/// <para>
/// <b>域级约束</b>：适用范围公众号 ✔ / 服务号 ✔（无「仅认证」）；<b>不支持第三方平台调用</b>（官方原文）；
/// 「product_key 和 DOM 只会在需要该字段的文章类型及卡片类型的请求中返回」；不同文章类型支持的卡片类型
/// 范围不同——图片消息（newspic）支持小卡/文字链接/条卡；图文消息（news）支持大卡/小卡/文字链接
/// （<b>不支持条卡</b>）。配套能力：草稿 newspic 文章的 <c>product_info.footer_product_info.product_key</c>
/// 即来自本端点。
/// </para>
/// <para><b>令牌路由</b>：消费 <see cref="MpTokenTypes.AccessToken"/>，Query 注入（MUD005 已知接受风险）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ProductCard", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpProductCardService
{
    /// <summary>
    /// 获取商品卡片的 DOM 结构。官方文档：<see href="https://developers.weixin.qq.com/doc/subscription/api/draftbox/shop/api_ecsgetproductcardinfo.html"/>
    /// （官方接口英文名 <c>ecsgetproductcardinfo</c>）。
    /// </summary>
    /// <param name="request">获取请求（product_id / article_type / card_type 均必填；按 article_type 选型 card_type）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品卡信息（<c>product_key</c> + <c>DOM</c> 结构；按文章类型与卡片类型按需返回）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>；<b>路径前缀为 /channels/ec/（视频号小店域，非 /cgi-bin/）</b>——官方路径安排，照抄原文。
    /// </para>
    /// <para>官方错误码：<c>10170001</c>（不合法的商品 ID）/ <c>10170002</c>（不支持的文章类型）/
    /// <c>10170003</c>（不支持的卡片类型）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/service/product/getcardinfo")]
    Task<MpProductCardInfoResponse> GetProductCardInfoAsync(
        [Body] MpProductCardInfoRequest request,
        CancellationToken cancellationToken = default);
}
