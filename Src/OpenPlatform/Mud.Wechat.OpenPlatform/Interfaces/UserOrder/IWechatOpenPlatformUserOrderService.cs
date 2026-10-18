// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.OpenPlatform.Abstractions.Authentication;
using Mud.Wechat.OpenPlatform.DataModels;
using Mud.Wechat.OpenPlatform.DataModels.UserOrder;

namespace Mud.Wechat.OpenPlatform;

/// <summary>
/// 微信开放平台「小程序购物订单」域 SDK（<b>7 端点</b>：上传购物详情 / 上传物流信息 / 上传合单购物详情 /
/// 上传合单物流信息 / 验证上传结果 / 开通产品权限 / 提交接入审核）。
/// </summary>
/// <remarks>
/// <para><b>令牌面</b>：全域恒 <c>authorizer_access_token</c>（Query 参数名 <c>access_token</c>）——
/// 第三方平台代授权方小程序上传订单数据，调用前须经
/// <see cref="IComponentAppContextSwitcher.UseAuthorizerScope(string)"/> 进入目标授权方作用域。</para>
/// <para><b>业务前置</b>：上传类端点要求小程序已开通「购物订单」产品权限（由本域
/// <c>orders-permission/open</c> 开通、<c>orders-permission/confirm</c> 提交审核）；订单信息一旦关联成功<b>不可修改</b>，
/// <c>upload_time</c> 须晚于历史请求（RFC3339）以保证更新顺序。</para>
/// <para><b>落位</b>：<c>Interfaces/UserOrder/IWechatOpenPlatformUserOrderService.cs</c>；DTO 命名空间
/// <c>Mud.Wechat.OpenPlatform.DataModels.UserOrder</c>；<c>OpenPlatformModule.UserOrder</c>，注册组 <c>UserOrder</c>。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "UserOrder", TokenManage = nameof(IOpenPlatformAppManager))]
[Token(TokenType = OpenPlatformTokenTypes.AuthorizerAccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWechatOpenPlatformUserOrderService
{
    /// <summary>
    /// 上传购物详情（<c>POST /user-order/orders</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/shopping-orders/uploadShoppingInfo.html"/>。
    /// </summary>
    /// <param name="request">请求体（订单标识 + 购物详情列表 + 支付者），见 <see cref="OpenPlatformUserOrderUploadRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方约束：<c>order_list</c> 最多 50 条、<c>item_list</c> 最多 50 条、<c>image_url</c> 最多 3 个；
    /// <c>upload_time</c> 为 RFC3339 字符串且须晚于上次请求；关联成功后订单信息不可修改。</para>
    /// </remarks>
    [Post("/user-order/orders")]
    Task<OpenPlatformResponse> UploadOrderAsync(
        [Body] OpenPlatformUserOrderUploadRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传物流信息（<c>POST /user-order/orders/shippings</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/shopping-orders/uploadShippingInfo.html"/>。
    /// </summary>
    /// <param name="request">请求体（订单标识 + 发货模式 + 物流列表），见 <see cref="OpenPlatformUserOrderShippingUploadRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方约束：须在支付成功后调用；<c>delivery_mode</c> 取 <c>UNIFIED_DELIVERY</c>（统一发货）或
    /// <c>SPLIT_DELIVERY</c>（分拆发货）；顺丰速运必须填写 <c>contact</c> 联系方式。</para>
    /// </remarks>
    [Post("/user-order/orders/shippings")]
    Task<OpenPlatformResponse> UploadShippingAsync(
        [Body] OpenPlatformUserOrderShippingUploadRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传合单购物详情（<c>POST /user-order/combine-orders</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/shopping-orders/uploadCombinedShoppingInfo.html"/>。
    /// </summary>
    /// <param name="request">请求体（合单标识 + 子单购物详情 + 支付者），见 <see cref="OpenPlatformUserOrderCombineUploadRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方约束：合单场景下各子单共用同一合单标识；<c>upload_time</c> 为 RFC3339 字符串；
    /// 支付成功后不可修改已关联的订单信息。</para>
    /// </remarks>
    [Post("/user-order/combine-orders")]
    Task<OpenPlatformResponse> UploadCombinedOrderAsync(
        [Body] OpenPlatformUserOrderCombineUploadRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传合单物流信息（<c>POST /user-order/combine-orders/shippings</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/shopping-orders/uploadCombinedShippingInfo.html"/>。
    /// </summary>
    /// <param name="request">请求体（合单标识 + 子单物流信息 + 支付者），见 <see cref="OpenPlatformUserOrderCombineShippingUploadRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>官方约束：各子单须分别给出 <c>order_key</c> 与 <c>delivery_mode</c>；<c>upload_time</c> 为 RFC3339 字符串。</remarks>
    [Post("/user-order/combine-orders/shippings")]
    Task<OpenPlatformResponse> UploadCombinedShippingAsync(
        [Body] OpenPlatformUserOrderCombineShippingUploadRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 验证购物订单上传结果（<c>POST /user-order/shoppinginfo/verify</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/shopping-orders/ShoppingInfoVerifyUploadResult.html"/>。
    /// </summary>
    /// <param name="request">请求体（订单标识 + 支付者），见 <see cref="OpenPlatformUserOrderVerifyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>验证结果 <c>verify_result</c>，见 <see cref="OpenPlatformUserOrderVerifyResponse"/>。</returns>
    /// <remarks>官方约束：用于核验订单上传是否已生效；须在服务端调用。</remarks>
    [Post("/user-order/shoppinginfo/verify")]
    Task<OpenPlatformUserOrderVerifyResponse> VerifyShoppingInfoAsync(
        [Body] OpenPlatformUserOrderVerifyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 开通购物订单产品权限（<c>POST /user-order/orders-permission/open</c>，无请求体）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/shopping-orders/openShoppingOrderProductPermission.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks>官方约束：官方请求示例无 JSON 包体，操作对象即当前授权方上下文自身；开通后仍须提交接入审核。</remarks>
    [Post("/user-order/orders-permission/open")]
    Task<OpenPlatformResponse> OpenProductPermissionAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 提交购物订单接入审核（<c>POST /user-order/orders-permission/confirm</c>，无请求体）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/shopping-orders/confirmProductPermission.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>最近一次审核结果 <c>last_result</c>，见 <see cref="OpenPlatformUserOrderPermissionConfirmResponse"/>。</returns>
    /// <remarks>官方约束：官方请求示例无 JSON 包体；审核结果由应答的 <c>last_result</c> 承载。</remarks>
    [Post("/user-order/orders-permission/confirm")]
    Task<OpenPlatformUserOrderPermissionConfirmResponse> ConfirmProductPermissionAsync(
        CancellationToken cancellationToken = default);
}
