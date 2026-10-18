// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「基础交易工具（原标准版交易组件 V1）」域——支持的配送方式面接口（2 端点：读取 / 设置）。
/// </summary>
/// <remarks>
/// <para><b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。</para>
/// <para><b>读写同形</b>：两支官方都用 <c>supported_shipping_methods</c> 单对象包装（内含 <c>express</c> / <c>same_city</c> / <c>pickup</c> 三个 number 开关），复用同一个 <see cref="WxaProductSupportedShippingMethods"/>。</para>
/// <para><b>覆盖式设置 + 前置校验</b>：读取侧请求参数表为空；设置侧三个开关<b>全部必填</b>，且官方要求「开启同城需先配同城发货地址、开启自提需先配自提地址」，否则报 <c>500001</c>/<c>500002</c>/<c>500003</c>。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ProductCatalog", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaProductShipMethodService
{
    /// <summary>
    /// 获取支持的配送方式（<c>POST /product/shipmethods/get</c>，请求参数表为空）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/ship/get_shipping_methods.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>配送方式开关，见 <see cref="WxaProductShippingMethodGetResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/shipmethods/get</c>；请求参数表<b>零字段</b> ⇒ 不设请求 DTO。</para>
    /// <para>应答包在 <c>supported_shipping_methods</c> 下：<c>express</c>（快递）、<c>same_city</c>（同城配送）、<c>pickup</c>（线下自提）三个 number 开关（是否开启）。</para>
    /// </remarks>
    [Post("/product/shipmethods/get")]
    Task<WxaProductShippingMethodGetResponse> GetShippingMethodsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置支持的配送方式（<c>POST /product/shipmethods/set</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/ship/set_shipping_methods.html"/>。
    /// </summary>
    /// <param name="request">请求体（包装键 <c>supported_shipping_methods</c>，三开关必填），见 <see cref="WxaProductShippingMethodSetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方返回体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/shipmethods/set</c>；请求体是 <c>{"supported_shipping_methods":{express,same_city,pickup}}</c> 单对象包装，三个开关<b>全部必填</b>（整面覆盖）。</para>
    /// <para><b>域内错误码</b>：<c>500001</c> 无配送方式、<c>500002</c> 无同城发货地址、<c>500003</c> 无线下自提地址 ⇒ 开启同城 / 自提前须先配置对应模板（<see cref="IWxaProductFreightService"/>、<see cref="IWxaProductOfflineService"/>）。</para>
    /// </remarks>
    [Post("/product/shipmethods/set")]
    Task<WxaResponse> SetShippingMethodsAsync(
        [Body] WxaProductShippingMethodSetRequest request,
        CancellationToken cancellationToken = default);
}
