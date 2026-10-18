// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「基础交易工具（原标准版交易组件 V1）」域——物流公司与发货面接口（2 端点：获取快递公司列表 / 订单发货）。
/// </summary>
/// <remarks>
/// <para><b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。</para>
/// <para><b>路由命名反直觉点</b>：官方文档页位于 <c>API/delivery/*</c>，但请求地址为 <c>/product/delivery/get_company_list</c>（获取快递公司列表）
/// 与 <c>/product/delivery/send</c>（发货，页面文件名却是 <c>send_delivery.html</c>）⇒ 路由以官方请求地址为准，<b>不是</b> <c>/product/delivery/send_delivery</c>。</para>
/// <para><b>幂等性二分</b>：<c>get_company_list</c> 只读可重试；<c>send</c> 是<b>状态推进</b>动作，官方明确
/// 「当前订单已经发货完成，不能重复发货（<c>109001</c>）」⇒ 不得放进自动重试管道。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ProductCatalog", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaProductDeliveryService
{
    /// <summary>
    /// 获取快递公司列表（<c>POST /product/delivery/get_company_list</c>，请求参数表为空）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/delivery/get_delivery_company_list.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>快递公司列表，见 <see cref="WxaProductDeliveryCompanyListResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/delivery/get_company_list</c>；请求参数表<b>零字段</b> ⇒ 不设请求 DTO。</para>
    /// <para>应答 <c>company_list[]</c> 每项是 <c>delivery_id</c>（string）/ <c>delivery_name</c>（string）。
    /// <b>这里的 <c>delivery_id</c> 是「快递公司 id」</b>，与 <see cref="SendDeliveryAsync"/> 里 <c>delivery_list[].delivery_id</c>（填本列表的 id）同一含义，
    /// 但与运费模板 / 同城模版里的 <c>delivery_id</c>（同为快递公司 id）是不同上下文，值域可交叉但语义各自独立。</para>
    /// </remarks>
    [Post("/product/delivery/get_company_list")]
    Task<WxaProductDeliveryCompanyListResponse> GetDeliveryCompanyListAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 订单发货（<c>POST /product/delivery/send</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/delivery/send_delivery.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>order_id</c> 必填 + <c>delivery_list[]</c>），见 <see cref="WxaProductDeliverySendRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方返回体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/delivery/send</c>；顶层仅 <c>order_id</c>（number，必填）与 <c>delivery_list[]</c> 两支。</para>
    /// <para>
    /// <b>条件必填集（照官方说明逐字建模）</b>：<c>delivery_id</c> 与 <c>waybill_id</c>「<b>快递配送必填</b>，同城、线下自提不用填」；
    /// <c>is_all_product</c> 不填默认为 <c>true</c>，拆单发货须填 <c>false</c>，且填 <c>false</c> 时 <c>product_infos[]</c> 内的
    /// <c>product_cnt</c> / <c>product_id</c> / <c>sku_id</c> 三者转为<b>必填</b>。
    /// </para>
    /// <para><b>域内错误码（本端点独有，非通用码表）</b>：<c>48001</c> 无权调用本 api、<c>100002</c> 订单不存在、
    /// <c>100003</c> 订单更新过于频繁、<c>101100</c> 当前订单状态不允许发货、<c>108009</c> 当前订单有未完成售后单、
    /// <c>109000</c> 发货失败（商品未正确填写）、<c>109001</c> 当前订单已发货完成不能重复发货、
    /// <c>109002</c> 当前发货请求未带上发货商品、<c>109205</c> 已下单在线物流单不允许自寄发货、
    /// <c>800001</c> 同一种 sku 的所有商品必须在一个包裹里寄出（另有通用 <c>-1</c> / <c>-2</c>）。</para>
    /// <para><b>敏感面</b>：运单号 <c>waybill_id</c> 与订单维度数据可关联到收件人 ⇒ <b>不得</b>写入日志、遥测或异常消息。</para>
    /// </remarks>
    [Post("/product/delivery/send")]
    Task<WxaResponse> SendDeliveryAsync(
        [Body] WxaProductDeliverySendRequest request,
        CancellationToken cancellationToken = default);
}
