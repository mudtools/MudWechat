// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.MiniProgram.DataModels.ProductCatalog;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「基础交易工具（原标准版交易组件 V1）」域——运费模板面接口（3 端点：查询 / 新增 / 更新）。
/// </summary>
/// <remarks>
/// <para><b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入 <c>access_token</c>。</para>
/// <para>
/// <b>路由前缀与文档目录不同源</b>：官方文档页在 <c>API/freight/*</c>，请求地址却统一在 <c>/product/delivery/*</c>
/// （<c>get_freight_template</c> / <c>insert_freight_template</c> / <c>update_freight_template</c>）⇒ 守卫以官方请求地址为准。
/// </para>
/// <para>
/// <b>请求体包装键 <c>freight_template</c></b>：新增与更新两支都把全部业务字段包在 <c>freight_template</c> 单对象下，
/// <b>不是</b>平铺；更新侧比新增侧多一个<b>必填</b>的 <c>template_id</c>（定位要改哪张模板）。
/// </para>
/// <para><b>覆盖式写入</b>：<c>update_freight_template</c> 是整面覆盖（不是字段级 patch），未提交的子结构会被清除 ⇒ 宿主须先读全量再回写。</para>
/// <para><b>单位口径</b>：金额字段（<c>first_price</c> / <c>second_price</c> / <c>min_amount</c>）单位为<b>分</b>，重量（<c>first_val_amount</c> / <c>second_val_amount</c>）单位为 <b>kg</b>。</para>
/// <para><b>官方类型笔误留档</b>：<c>valuation_type</c> 官方类型列原文拼作 <c>nunber</c>（应为 number）⇒ 建模仍取数值型，本条仅留档、不改契约。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ProductCatalog", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaProductFreightService
{
    /// <summary>
    /// 获取运费模板（<c>POST /product/delivery/get_freight_template</c>，请求参数表为空）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/freight/get_freight_template.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>运费模板列表，见 <see cref="WxaProductFreightTemplateGetResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/delivery/get_freight_template</c>；请求参数表<b>零字段</b>（一次返回全部模板）⇒ 不设请求 DTO。</para>
    /// <para>
    /// <b>读写两侧类型不对称（守卫 PC4 锁定）</b>：<c>freight_calc_method_list[].is_default</c> 在<b>本查询侧</b>是 number，
    /// 在 <see cref="InsertFreightTemplateAsync"/> / <see cref="UpdateFreightTemplateAsync"/> 的<b>请求侧</b>官方类型列写的是 <c>boolen</c>（boolean）
    /// ⇒ 查询侧与请求侧各用独立元素类型，不得为「统一」而合并 DTO。
    /// </para>
    /// <para>查询侧模板元素另含 <c>create_time</c> / <c>update_time</c> 两支时间戳（请求侧没有）。</para>
    /// </remarks>
    [Post("/product/delivery/get_freight_template")]
    Task<WxaProductFreightTemplateGetResponse> GetFreightTemplateListAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 添加运费模板（<c>POST /product/delivery/insert_freight_template</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/freight/insert_freight_template.html"/>。
    /// </summary>
    /// <param name="request">请求体（包装键 <c>freight_template</c>，不含 <c>template_id</c>），见 <see cref="WxaProductFreightTemplateInsertRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方返回体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/delivery/insert_freight_template</c>；请求体为 <c>{"freight_template":{…}}</c> 单对象包装。</para>
    /// <para><b>必填集</b>：<c>name</c> / <c>valuation_type</c>（1 按件、2 按重量）/ <c>send_time</c> / <c>delivery_type</c>（1 快递）
    /// / <c>shipping_method</c>（1 包邮、2 条件包邮、3 不包邮）为必填；官方注明「不包邮时需要在 <c>all_freight_calc_method</c> 指定默认运费」。</para>
    /// <para><b>新增侧不得传 <c>template_id</c></b>：模板 id 由查询侧回读，新增侧官方请求表无该列。</para>
    /// </remarks>
    [Post("/product/delivery/insert_freight_template")]
    Task<WxaResponse> InsertFreightTemplateAsync(
        [Body] WxaProductFreightTemplateInsertRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新运费模板（<c>POST /product/delivery/update_freight_template</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/freight/update_freight_template.html"/>。
    /// </summary>
    /// <param name="request">请求体（包装键 <c>freight_template</c>，含必填 <c>template_id</c>），见 <see cref="WxaProductFreightTemplateUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方返回体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 返回基类 <see cref="WxaResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/product/delivery/update_freight_template</c>；请求体结构与新增侧同源，
    /// 但<b>额外必填 <c>template_id</c></b>（number）用于定位待更新模板。</para>
    /// <para><b>覆盖式语义</b>：本端点整面替换模板配置，不是增量更新 ⇒ 少传即清空，宿主应先 <see cref="GetFreightTemplateListAsync"/> 读全量再回写。</para>
    /// </remarks>
    [Post("/product/delivery/update_freight_template")]
    Task<WxaResponse> UpdateFreightTemplateAsync(
        [Body] WxaProductFreightTemplateUpdateRequest request,
        CancellationToken cancellationToken = default);
}
