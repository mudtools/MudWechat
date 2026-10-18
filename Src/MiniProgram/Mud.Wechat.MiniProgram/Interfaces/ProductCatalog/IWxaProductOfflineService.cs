// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「基础交易工具（原标准版交易组件 V1）」域——线下自提与同城配送模板面接口（4 端点：自提查 / 自提改 / 同城查 / 同城改）。
/// </summary>
/// <remarks>
/// <para><b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。</para>
/// <para><b>两支「查询」请求参数表为空</b>（<c>get_pickup_template</c> / <c>get_same_city_template</c>）⇒ 不设请求 DTO；
/// 两支「更新」为<b>整面覆盖</b>（少传即清空），且返回体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 复用基类 <see cref="WxaResponse"/>。</para>
/// <para>
/// <b>读写类型不对称（守卫 PC4 锁定）</b>：同城模版的 <c>shipper</c>（配送方）、<c>same_city_valuation.type</c>、
/// <c>special_time.type</c> 在<b>查询侧</b>官方类型为 string（返回 <c>Shipper_*</c> / <c>ValuationType_*</c> 枚举名），
/// 在<b>更新请求侧</b>官方类型为 number（传 <c>0/1/2</c>）⇒ 查询侧与请求侧各用独立类型，不得合并 DTO。
/// </para>
/// <para><b>时间单位</b>：自提备货 / 可预定时间以「分钟」计，开店关店以「时 + 分」两字段计；同城送达范围以「米」计、金额以「分」计。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ProductCatalog", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaProductOfflineService
{
    /// <summary>
    /// 查询线下自提模版（<c>POST /product/offline/get_pickup_template</c>，请求参数表为空）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/offline/get_pickup_template.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>线下自提模版，见 <see cref="WxaProductPickupTemplateGetResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/offline/get_pickup_template</c>；请求参数表<b>零字段</b> ⇒ 不设请求 DTO。</para>
    /// <para>应答包在 <c>pickup_template</c> 下（<c>template_id</c> + <c>pickup_time</c>）；<c>pickup_time</c> 的读写同形，复用 <see cref="WxaProductPickupTemplate"/>。</para>
    /// </remarks>
    [Post("/product/offline/get_pickup_template")]
    Task<WxaProductPickupTemplateGetResponse> GetPickupTemplateAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新线下自提模版（<c>POST /product/offline/update_pickup_template</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/offline/update_pickup_template.html"/>。
    /// </summary>
    /// <param name="request">请求体（包装键 <c>pickup_template</c>，<c>template_id</c> 与 <c>pickup_time</c> 皆必填），见 <see cref="WxaProductPickupTemplateUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方返回体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/offline/update_pickup_template</c>；<c>pickup_time</c> 的六个时间字段（开店时/分、关店时/分、备货分钟、可预定分钟）官方标<b>全部必填</b>。</para>
    /// <para><b>覆盖式</b>：整面替换自提时间配置，须一次性提交完整。</para>
    /// </remarks>
    [Post("/product/offline/update_pickup_template")]
    Task<WxaResponse> UpdatePickupTemplateAsync(
        [Body] WxaProductPickupTemplateUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询同城配送模版（<c>POST /product/offline/get_same_city_template</c>，请求参数表为空）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/offline/get_same_city_template.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>同城配送模版，见 <see cref="WxaProductSameCityTemplateGetResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/offline/get_same_city_template</c>；请求参数表<b>零字段</b> ⇒ 不设请求 DTO。</para>
    /// <para>应答包在 <c>same_city_template</c> 下；查询侧 <c>shipper</c> 与两个 <c>type</c> 均为 <b>string</b>（枚举名），
    /// 与 <see cref="UpdateSameCityTemplateAsync"/> 请求侧的 number 形态不同源。</para>
    /// </remarks>
    [Post("/product/offline/get_same_city_template")]
    Task<WxaProductSameCityTemplateGetResponse> GetSameCityTemplateAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新同城配送模版（<c>POST /product/offline/update_same_city_template</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/offline/update_same_city_template.html"/>。
    /// </summary>
    /// <param name="request">请求体（包装键 <c>same_city_template</c>，六支顶层字段全必填），见 <see cref="WxaProductSameCityTemplateUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方返回体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/offline/update_same_city_template</c>；顶层 <c>template_id</c> / <c>shipper</c> /
    /// <c>scope</c> / <c>same_city_valuation</c> / <c>special_time</c> / <c>delivery_time</c> 官方标<b>全部必填</b>。</para>
    /// <para><b>请求侧枚举取 number</b>：<c>shipper</c> 0 未知 / 1 商家配送 / 2 第三方配送；<c>same_city_valuation.type</c>
    /// 0 免运费 / 1 条件免运费 / 2 阶梯计费；<c>special_time.type</c> 0 无附加费用 / 1 额外增加费用。填 <c>1</c>/<c>2</c> 时才需对应子对象。</para>
    /// <para><b>覆盖式</b>：整面替换同城配置；金额单位为分、距离单位为米。</para>
    /// </remarks>
    [Post("/product/offline/update_same_city_template")]
    Task<WxaResponse> UpdateSameCityTemplateAsync(
        [Body] WxaProductSameCityTemplateUpdateRequest request,
        CancellationToken cancellationToken = default);
}
