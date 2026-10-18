// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「基础交易工具（原标准版交易组件 V1）」域——服务市场面接口（3 端点：校验跳转码 / 服务列表 / 服务订单列表）。
/// </summary>
/// <remarks>
/// <para><b>令牌形态（MP-X2 / MP-X3 同口径）</b>：本域以小程序 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c> 声明，<b>不新增令牌类型</b>。</para>
/// <para>
/// <b>令牌名分叉（守卫 PC4 锁定）</b>：<see cref="CheckServiceAuthAsync"/> 官方请求地址原文是
/// <c>component_access_token</c>（服务市场第三方跳转码核验形态），<see cref="GetServiceListAsync"/> 与
/// <see cref="GetServiceOrderListAsync"/> 是 <c>access_token</c>。本 SDK 一律按 <c>access_token</c> 注入，代调用形态由宿主供给令牌，
/// <b>不</b>引入 component 注入名（否则会新增 Query 凭据词表面，须走词表补齐 / 豁免二选一）。
/// </para>
/// <para>路由与官方页名不同源：三页位于 <c>API/service/{check_auth,get_service_list,get_service_order_list}.html</c>，请求地址为 <c>/product/service/{check_auth,get_list,get_order_list}</c>。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ProductCatalog", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaProductServiceMarketService
{
    /// <summary>
    /// 校验服务市场跳转码（<c>POST /product/service/check_auth</c>，官方请求地址凭据名为 <c>component_access_token</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/service/check_auth.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>code</c> 必填），见 <see cref="WxaProductServiceCheckAuthRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>授权信息，见 <see cref="WxaProductServiceCheckAuthResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/service/check_auth</c>；请求体只有 <c>code</c>（string，必填）——小商店服务市场跳转到第三方 URL 时带上的跳转码。</para>
    /// <para>应答包在 <c>data</c> 下：<c>appid</c> / <c>service_id</c>（无符号 64 位）/ <c>specification_id</c> / <c>order_id</c>（无符号 64 位）⇒ <c>long?</c> 建模。</para>
    /// <para><b>跳转码一次性</b>：<c>code</c> 过期或不存在即报 <c>1</c> ⇒ 非幂等、不可重放，宿主须即时消费（禁入日志）。令牌面 component / access 分叉见接口 remarks。</para>
    /// </remarks>
    [Post("/product/service/check_auth")]
    Task<WxaProductServiceCheckAuthResponse> CheckServiceAuthAsync(
        [Body] WxaProductServiceCheckAuthRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取服务列表（<c>POST /product/service/get_list</c>，请求参数表为空）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/service/get_service_list.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>服务列表，见 <see cref="WxaProductServiceListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/service/get_list</c>；请求参数表<b>零字段</b> ⇒ 不设请求 DTO。</para>
    /// <para>应答 <c>service_list[]</c> 含 <c>service_id</c> / <c>service_name</c> / <c>expire_time</c>（官方类型 string）/ <c>service_type</c>（枚举 <c>2</c> 商品、<c>3</c> 订单、<c>4</c> 物流）。</para>
    /// </remarks>
    [Post("/product/service/get_list")]
    Task<WxaProductServiceListResponse> GetServiceListAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取服务订单列表（<c>POST /product/service/get_order_list</c>，请求参数表为空）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/service/get_service_order_list.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>服务订单列表，见 <see cref="WxaProductServiceOrderListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/service/get_order_list</c>；请求参数表<b>零字段</b> ⇒ 不设请求 DTO。</para>
    /// <para>
    /// <b>官方字段名拼写陷阱（照原文钉住）</b>：创建时间官方键名是 <c>craete_time</c>（<b>craete</b>，官方原文即如此拼错），
    /// <b>不得</b>「顺手修正」为 <c>create_time</c>，否则真实网关返回体的键对不上、字段恒为 null。守卫 PC4 锁定。
    /// </para>
    /// <para><c>service_type</c> 枚举在此页只给 <c>2</c> 商品 / <c>3</c> 订单（比 get_list 少一个「4 物流」）；订单状态 <c>status</c> 是另一套枚举（1 生效中 / 2 已过期 / 3 已停用 / 4 未付款）。</para>
    /// </remarks>
    [Post("/product/service/get_order_list")]
    Task<WxaProductServiceOrderListResponse> GetServiceOrderListAsync(
        CancellationToken cancellationToken = default);
}
