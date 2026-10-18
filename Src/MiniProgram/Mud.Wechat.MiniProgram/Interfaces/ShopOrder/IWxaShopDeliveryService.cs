// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ShopOrder;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「自定义交易组件 V2 · 物流配送」域 SDK（3 端点：快递公司列表 / 确认收货 / 发货；
/// 官方 <c>minishopopencomponent2/API/delivery</c> 分区）。
/// </summary>
/// <remarks>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。
/// </para>
/// <para>
/// <b>路由拼写以「请求地址」为准，不以文档文件名</b>：确认收货官方页文件名为 <c>recieve.html</c>（拼写错误），
/// 但页内请求地址是 <c>/shop/delivery/receive</c> ⇒ 本仓按<b>请求地址</b>声明路由，守卫 SHPO-D 锁定；
/// 不得按文件名拼成 <c>/shop/delivery/recieve</c>。
/// </para>
/// <para>
/// <b>发货不是一次到位的幂等操作</b>：<c>send</c> 的 <c>finish_all_delivery=0</c> 允许分批发货（订单进入 <c>21</c>），
/// 全部发完后置 <c>1</c>（订单进入 <c>30</c>）；<c>delivery_type=1</c>（快递）时 <c>delivery_list</c> 必填，
/// <c>finish_all_delivery=1</c> 时 <c>ship_done_time</c> 必传 ⇒ <b>非幂等</b>，不得放进自动重试管道。
/// </para>
/// <para>
/// <b><c>receive</c> 的终态语义</b>：确认收货把订单推进到完成（<c>100</c>）并可触发分账/结算计时，
/// 官方无撤销端点 ⇒ 非幂等、不得自动重试；且只允许在已发货（<c>30</c>）阶段调用。
/// </para>
/// <para>
/// <b>快递公司 ID 必须取自 <c>get_company_list</c></b>：官方原文「将影响物流信息查询」⇒ 不得随意填写；
/// 本端点只读、可安全重试。
/// </para>
/// <para>
/// <b>官方端点级频率数字空缺</b>：3 页正文均无「次/分钟」类数字，也未给权限集编号与云调用说明 ⇒ 注释留档，
/// 不得凭第三方 SDK 或记忆补数。
/// </para>
/// <para>
/// <b>敏感面（AGENTS §8）</b>：<c>openid</c>、<c>waybill_id</c>（快递单号）、订单定位键与商品标识属个人信息 / 交易标识
/// ⇒ 不得写入日志、遥测或异常消息。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ShopOrder", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaShopDeliveryService
{
    /// <summary>
    /// 获取快递公司列表（<c>POST /shop/delivery/get_company_list</c>，空请求体 <c>{}</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/delivery/get_company_list.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>company_list[]</c>（<c>delivery_id</c> / <c>delivery_name</c>），见 <see cref="WxaShopDeliveryCompanyListResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 空对象请求体；本域官方无端点级频率数字。
    /// 官方请求体无任何业务字段 ⇒ 本方法不带 <c>[Body]</c> 参数。
    /// </para>
    /// <para>
    /// <b>用途</b>：返回值里的 <c>delivery_id</c>（如 <c>SF</c> / <c>YTO</c>）是发货端点
    /// <c>delivery_list[].delivery_id</c> 的唯一合法来源；官方原文「将影响物流信息查询」。
    /// </para>
    /// <para><b>只读</b>：可安全重试，建议宿主缓存（列表变化极低频）。</para>
    /// </remarks>
    [Post("/shop/delivery/get_company_list")]
    Task<WxaShopDeliveryCompanyListResponse> GetDeliveryCompanyListAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 确认收货（<c>POST /shop/delivery/receive</c>，官方页文件名拼作 <c>recieve.html</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/delivery/recieve.html"/>。
    /// </summary>
    /// <param name="request">订单定位键 + <c>openid</c>，见 <see cref="WxaShopDeliveryReceiveRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体；<c>order_id</c> 与 <c>out_order_id</c> 二选一、<c>openid</c> 必填；
    /// 本域官方无端点级频率数字。
    /// </para>
    /// <para>
    /// <b>状态不可逆</b>：确认收货后订单完成并启动售后/结算计时，官方无回退端点 ⇒ 非幂等、不得自动重试。
    /// </para>
    /// </remarks>
    [Post("/shop/delivery/receive")]
    Task<WxaResponse> ReceiveDeliveryAsync(
        [Body] WxaShopDeliveryReceiveRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发货（<c>POST /shop/delivery/send</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent2/API/delivery/send.html"/>。
    /// </summary>
    /// <param name="request">发货请求体，见 <see cref="WxaShopDeliverySendRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON 请求体；<c>finish_all_delivery</c> 必填（0 未发完 / 1 已发完），
    /// <c>delivery_type=1</c>（快递）时 <c>delivery_list</c> 必填，<c>finish_all_delivery=1</c> 时
    /// <c>ship_done_time</c> 必传；本域官方无端点级频率数字。
    /// </para>
    /// <para>
    /// <b>分批发货</b>：<c>finish_all_delivery=0</c> 使订单进入 <c>21</c>（部分发货），再次调用补齐；
    /// 每次调用都是对订单的发货事实追加，<b>非幂等</b>、不得放进自动重试管道。
    /// </para>
    /// <para>
    /// <b>官方示例类型不一致</b>：<c>product_info_list[].out_product_id</c> / <c>out_sku_id</c> 官方参数表为
    /// <c>string</c>，示例却写成数字 <c>1</c> / <c>2</c> ⇒ 本仓按参数表建模为 <c>string</c>，宿主须传字符串。
    /// </para>
    /// </remarks>
    [Post("/shop/delivery/send")]
    Task<WxaResponse> SendDeliveryAsync(
        [Body] WxaShopDeliverySendRequest request,
        CancellationToken cancellationToken = default);
}
