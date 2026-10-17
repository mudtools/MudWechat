// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.DataModels.Delivery;

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「代发与供货」域 SDK（23 端点：代发管理 13 + 供货管理 3 + 供货商账户 2 + 供货商侧代发履约 5）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域职责（官方「代发管理」+「供货管理」两目录）</b>：本域覆盖 ①供货商关联关系
/// （<c>/channels/ec/supplier/relation/*</c>）、②自动分配设置（同前缀）、③代发单管理
/// （<c>/channels/ec/order/dropship/*</c>）、④供货单管理（<c>/channels/ec/order/supplyorder/*</c>）、
/// ⑤<b>供货商视角</b>的代发单查询与发货履约（<c>/channels/ec/order/dropship/supplier/*</c>）。
/// 与 Order 域（订单交易管理）互不重叠：Order 域的 <c>order/deliveryinfo</c>/<c>order/deliverynegotiation</c>
/// 为发货履约端点，本域的 <c>order/dropship</c>/<c>order/supplyorder</c> 为代发链路端点，路由互斥。
/// </para>
/// <para>
/// <b>角色与视角</b>：供货商关联 / 自动分配 / 代发单三族为<b>商家（小店）视角</b>调用
/// （把订单分配给已关联供货商代发）；供货单族同样凭商家 <c>access_token</c> 调用，
/// 用于商家查询供货商侧产生的供货单详情；<c>order/dropship/supplier/*</c> 五支与「供货商账户」两支为
/// <b>小店供货商视角</b>（须以供货商自身 <c>access_token</c> 调用，权限集 id <c>192</c>），
/// 与商家视角同名端点互为同一单据的两端、参数表存在视角差异，故各建 DTO、不得互相对齐。
/// </para>
/// <para>
/// <b>使用前置条件</b>：店铺须开通代发管理能力（官方错误码 <c>9700232</c> 小店尚未开通代发管理）；
/// 供货商须先经「申请关联供货商」建立关联（官方错误码 <c>9700230</c> 供货商和小店未绑定）。
/// </para>
/// <para>
/// <b>MUD005 已知接受风险</b>：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Delivery", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsDeliveryService
{
    /// <summary>
    /// 获取供货商列表（<c>get_supplier_list</c>，已关联供货商分页查询）。
    /// </summary>
    /// <param name="request">查询请求（<c>offset</c>/<c>limit</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>供货商列表（<c>name</c>/<c>appid</c>/<c>status</c>/<c>bind_audit_status</c>/<c>update_time</c>）与总数。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/supplier/relation/api_get_supplier_list"/></para>
    /// <para>官方业务限制：本接口无特殊注意事项；状态枚举见 <see cref="ChannelsSupplierStatuses"/>，
    /// 绑定审核状态枚举见 <see cref="ChannelsSupplierBindAuditStatuses"/>；无专属错误码。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/supplier/relation/get_supplier_list")]
    Task<ChannelsGetSupplierListResponse> GetSupplierListAsync(
        [Body] ChannelsGetSupplierListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 申请关联供货商（<c>invite_supplier</c>，商家向供货商发起关联邀请）。
    /// </summary>
    /// <param name="request">邀请请求（<c>supplier_appid</c> 必填；第三方代调用时 <c>service_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/supplier/relation/api_invite_supplier"/></para>
    /// <para>
    /// 官方业务限制：①代调用须经商家明示同意，平台会向商家展示第三方服务名称及 <c>remark</c>；
    /// ②平台按第三方平台主体维度考核该接口的「负反馈率」，高负反馈将被中止调用权限；
    /// ③商家只能向<b>未关联</b>的供货商发起申请，已有待供货商处理的申请时不可重复发起；
    /// ④暂无解除关联的 API，需登录商家后台手动操作；⑤第三方代调用时 <c>service_id</c> 必填且平台校验其与实际调用方一致。
    /// </para>
    /// <para>
    /// 官方错误码：<c>9700228</c>（非法 APPID）/ <c>9700229</c>（非法的邀请）/ <c>9700232</c>（小店尚未开通代发管理）/
    /// <c>268580000</c>（参数错误）/ <c>268580002</c>（商家已关联供货商）/ <c>268580003</c>（服务和第三方平台关系无效）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/supplier/relation/invite_supplier")]
    Task<ChannelsResponse> InviteSupplierAsync(
        [Body] ChannelsInviteSupplierRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取分配方式（<c>get_distribute</c>，全店当前的代发分配模式）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分配类型（<c>distribute_type</c>，见 <see cref="ChannelsDistributeTypes"/>）与全店自动分配供货商信息。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/supplier/auto/api_get_distribution"/></para>
    /// <para>官方契约：<b>POST</b> + 空请求体（官方参数表无 Body 参数）。</para>
    /// <para>官方业务限制：本接口无特殊注意事项；专属错误码 <c>9700232</c>（小店尚未开通代发管理）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/supplier/relation/get_distribute")]
    Task<ChannelsGetDistributeResponse> GetDistributeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置全店订单手动分配（<c>set_manually_distribute</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/supplier/auto/api_set_manually_distribution"/></para>
    /// <para>官方契约：<b>POST</b> + 空请求体（官方参数表无 Body 参数）。</para>
    /// <para>
    /// 官方业务限制：<b>对新增订单生效</b>——设置生效前产生的存量订单维持原有分配方式；
    /// 专属错误码 <c>9700232</c>（小店尚未开通代发管理）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/supplier/relation/set_manually_distribute")]
    Task<ChannelsResponse> SetManuallyDistributeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置全店订单自动分配（<c>set_all_distribution</c>，自动分配给单一供货商）。
    /// </summary>
    /// <param name="request">设置请求（<c>supplier_appid</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/supplier/auto/api_set_all_distribution"/></para>
    /// <para>
    /// 官方业务限制：<b>对新增订单生效</b>——设置生效前产生的存量订单维持原有分配方式；
    /// 专属错误码 <c>9700230</c>（供货商和小店未绑定）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/supplier/relation/set_all_distribution")]
    Task<ChannelsResponse> SetAllDistributionAsync(
        [Body] ChannelsSetAllDistributionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置按商品自动分配（<c>set_product_distribute</c>，按商品 ID 分配给指定供货商）。
    /// </summary>
    /// <param name="request">设置请求（<c>list</c> 必填，数量不超过 200）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/supplier/auto/api_set_product_distribution"/></para>
    /// <para>
    /// 官方业务限制：①<b><c>list</c> 的数量不超过 200</b>；②对新增订单生效，存量订单维持原有分配方式；
    /// ③商品默认强制同步自动分配关系到赠品——单独为赠品设置代发关系时，须将 <c>apply_to_gift</c> 设为 <c>false</c>。
    /// </para>
    /// <para>官方业务限制：本接口无专属错误码（走通用错误码）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/supplier/relation/set_product_distribute")]
    Task<ChannelsResponse> SetProductDistributeAsync(
        [Body] ChannelsSetProductDistributeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商品对应的自动分配供货商（<c>get_product_default_distribute</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>product_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否已设置自动分配（<c>exist_default_supplier</c>）与供货商 appid（<c>supplier_appid</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/supplier/auto/api_get_product_default_distribution"/></para>
    /// <para>官方业务限制：本接口无特殊注意事项；无专属错误码（走通用错误码）；
    /// <c>supplier_appid</c> 当且仅当 <c>exist_default_supplier = true</c> 时有意义。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/supplier/relation/get_product_default_distribute")]
    Task<ChannelsGetProductDefaultDistributeResponse> GetProductDefaultDistributeAsync(
        [Body] ChannelsGetProductDefaultDistributeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取按商品自动分配的商品列表（<c>get_product_list</c>）。
    /// </summary>
    /// <param name="request">查询请求（全字段选填；过滤开关与过滤值须成对使用）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品分配状态列表（<c>product_id</c>/<c>distribute_status</c>/<c>supplier_appid</c>/<c>supplier_name</c>）与分页上下文。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/supplier/auto/api_get_product_list"/></para>
    /// <para>
    /// 官方业务限制：①本接口无特殊注意事项、无专属错误码；②分页方式官方原文：第一次请求 <c>page_context</c>
    /// 传空串，请求下一页时将上一次返回的 <c>page_context</c> 填充到本次请求，直到 <c>page_context</c> 为空结束请求。
    /// </para>
    /// <para>
    /// <b>官方文档矛盾照录</b>：官方请求示例中出现 <c>is_filter_product_status</c>/<c>product_status</c>，
    /// 参数表未列出——SDK 按参数表建模（见 <see cref="ChannelsGetProductListRequest"/>）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/supplier/relation/get_product_list")]
    Task<ChannelsGetProductListResponse> GetProductListAsync(
        [Body] ChannelsGetProductListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 分配订单代发（<c>dropship/assign</c>，把订单（部分商品）分配给供货商代发）。
    /// </summary>
    /// <param name="request">分配请求（<c>order_id</c>/<c>supplier_appid</c>/<c>dropship_product_list</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/supplier/order/api_dropship_assign"/></para>
    /// <para>
    /// 官方业务限制：本接口无特殊注意事项；<c>dropship_product_list</c> 自官方 2026-05-07 变更起为必填。
    /// 专属错误码系列：<c>610000</c>（未绑定代发关系）/ <c>610001</c>（订单非待发货状态）/ <c>610002</c>（订单没有代发标记）/
    /// <c>610003</c>（订单所有商品已发货）/ <c>610004</c>（该商品已发货）/ <c>610005</c>（该商品已售后）/
    /// <c>610006</c>（该商品换款申请中）/ <c>610007</c>（该商品未找到）/ <c>610008</c>（供货商冲突）/
    /// <c>610009</c>（代发单订单 ID 冲突）/ <c>610011</c>（代发单不存在）/ <c>610012</c>（该商品已打单）/
    /// <c>610013</c>（订单已分配供货商）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/dropship/assign")]
    Task<ChannelsResponse> AssignDropshipAsync(
        [Body] ChannelsDropshipAssignRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消分配代发单（<c>dropship/cancel</c>）。
    /// </summary>
    /// <param name="request">取消请求（<c>order_id</c>/<c>ds_order_id</c>/<c>dropship_product_list</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/supplier/order/api_dropship_cancel"/></para>
    /// <para>官方业务限制：本接口无特殊注意事项；专属错误码 <c>610004</c>（该商品已发货）/ <c>610009</c>（代发单订单 ID 冲突）。</para>
    /// <para>
    /// <b>官方文档矛盾照录</b>：官方请求示例中出现 <c>supplier_appid</c>，参数表未列出——SDK 按参数表建模
    /// （见 <see cref="ChannelsDropshipCancelRequest"/>）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/dropship/cancel")]
    Task<ChannelsResponse> CancelDropshipAsync(
        [Body] ChannelsDropshipCancelRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询代发单详情（<c>dropship/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>ds_order_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>代发单结构（<c>dropship_order</c>：状态 / 供货商 / 商品明细 / 发货信息 / 扩展信息）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/supplier/order/api_dropship_get"/></para>
    /// <para>官方业务限制：本接口无特殊注意事项；专属错误码 <c>610011</c>（代发单不存在）；
    /// 代发单状态枚举见 <see cref="ChannelsDropshipOrderStatuses"/>，发货状态枚举见 <see cref="ChannelsDeliveryStatuses"/>。</para>
    /// <para>
    /// <b>官方形态照录</b>：<c>ds_order_id</c> 在请求侧官方类型为 string、在响应侧为 number——按官方各侧参数表分别建模。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/dropship/get")]
    Task<ChannelsDropshipGetResponse> GetDropshipOrderAsync(
        [Body] ChannelsDropshipGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 拉取代发单列表（<c>dropship/list</c>）。
    /// </summary>
    /// <param name="request">查询请求（全字段选填；时间范围起止距离不可超过 7 天）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>代发单号列表（<c>ds_order_id_list</c>）、分页参数（<c>next_page</c>）、总数与翻页标记（<c>has_more</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/supplier/order/api_dropship_list"/></para>
    /// <para>
    /// 官方业务限制：①本接口无特殊注意事项；②<c>create_time_range</c>/<c>update_time_range</c> 起止距离不可超过 7 天
    /// （秒级时间戳）；③推荐用 <c>next_key</c> 翻页（代替 <c>offset</c>，性能更优：首页传空，后续用响应 <c>next_page</c> 回填）；
    /// ④专属错误码 <c>40097</c>（请求体参数不正确）/ <c>47001</c>（请求体格式不正确）。
    /// </para>
    /// <para>
    /// <b>官方文档矛盾照录</b>：参数表 <c>order_id</c> 类型为 number，请求示例为字符串——SDK 按参数表建模（long）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/dropship/list")]
    Task<ChannelsDropshipOrderListResponse> GetDropshipOrderListAsync(
        [Body] ChannelsDropshipListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 搜索代发单（<c>dropship/search</c>，多条件组合搜索）。
    /// </summary>
    /// <param name="request">搜索请求（<c>search_condition</c> 必填；<c>page_info.limit</c> &lt;= 10）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>代发单号列表（<c>ds_order_id_list</c>）、分页参数（<c>next_page</c>）、总数与翻页标记（<c>has_more</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/supplier/order/api_dropship_search"/></para>
    /// <para>
    /// 官方业务限制：①本接口无特殊注意事项；②各时间范围起止距离不可超过 7 天（秒级时间戳）；
    /// ③本端点 <c>page_info.limit</c> 官方限制 &lt;= 10（严于其余拉单端点的 100）；④<c>waybill_id</c> 只可独立查询；
    /// ⑤专属错误码 <c>40097</c>（请求体参数不正确）/ <c>47001</c>（请求体格式不正确）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/dropship/search")]
    Task<ChannelsDropshipOrderListResponse> SearchDropshipOrdersAsync(
        [Body] ChannelsDropshipSearchRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取供货单列表（<c>supplyorder/list</c>）。
    /// </summary>
    /// <param name="request">查询请求（全字段选填；时间范围至少填一个且起止距离不可超过 7 天）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>供货单号列表（<c>supply_order_id_list</c>）、分页参数（<c>next_page</c>）、总数与翻页标记（<c>has_more</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/supply/api_supplyorder_list"/></para>
    /// <para>
    /// 官方业务限制：①本接口无特殊注意事项、无专属错误码；②时间范围至少填一个且起止距离不可超过 7 天（秒级时间戳）；
    /// ③推荐用 <c>next_key</c> 翻页（首页传空，后续用响应 <c>next_page</c> 回填）。
    /// </para>
    /// <para>
    /// <b>官方变更日志照录</b>：2026-05-25 起 <c>shop_appid</c> 字段已删除、新增 <c>supplier_appid</c>——
    /// SDK 按变更后参数表建模（见 <see cref="ChannelsSupplyOrderListRequest"/>）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/supplyorder/list")]
    Task<ChannelsSupplyOrderListResponse> GetSupplyOrderListAsync(
        [Body] ChannelsSupplyOrderListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询供货单详情（<c>supplyorder/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>supply_order_id</c> 必填，可从获取供货单列表接口获取）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>供货单结构（<c>supply_order</c>：状态 / 供货商 / 商品明细 / 价格信息 / 发货信息 / 售后信息）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/supply/api_supplyorder_get"/></para>
    /// <para>官方业务限制：本接口无特殊注意事项；专属错误码 <c>610011</c>（供货单不存在）；
    /// 供货单状态枚举见 <see cref="ChannelsSupplyOrderStatuses"/>，发货状态枚举见 <see cref="ChannelsDeliveryStatuses"/>，
    /// 售后状态枚举见 <see cref="ChannelsSupplyAftersaleStatuses"/>；
    /// 金额字段（<c>sale_price</c>/<c>supply_price</c>/<c>freight</c>/<c>total_price</c>）单位均为<b>分</b>。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/supplyorder/get")]
    Task<ChannelsSupplyOrderGetResponse> GetSupplyOrderAsync(
        [Body] ChannelsSupplyOrderGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 搜索供货单（<c>supplyorder/search</c>，多条件组合搜索）。
    /// </summary>
    /// <param name="request">搜索请求（<c>search_condition</c> 与 <c>page_info</c> 均必填；<c>search_condition</c> 至少设置一个字段）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>供货单号列表（<c>supply_order_id_list</c>）、分页参数（<c>next_page</c>）、总数与翻页标记（<c>has_more</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/supply/api_supplyorder_search"/></para>
    /// <para>
    /// 官方业务限制：①本接口无特殊注意事项、无专属错误码；②<c>search_condition</c> 各字段必须至少设置一个，否则接口报错；
    /// ③各时间范围起止距离不可超过 7 天（秒级时间戳）；④<c>limit</c> &lt;= 100。
    /// </para>
    /// <para>
    /// <b>官方文档矛盾照录</b>：官方请求示例中出现 <c>tel_last4</c>/<c>user_name</c>，参数表未列出——SDK 按参数表建模
    /// （见 <see cref="ChannelsSupplyOrderSearchRequest"/>）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/supplyorder/search")]
    Task<ChannelsSupplyOrderListResponse> SearchSupplyOrdersAsync(
        [Body] ChannelsSupplyOrderSearchRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取供货商基础信息（<c>get_supplier_base_info</c>，名称 / 头像 / 主体类型 / 可用经营模式）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>供货商名称（<c>name</c>）、头像（<c>head_img</c>，个人主体无）、主体类型（<c>subject_type</c>：2 企业 / 2500 个人）与经营模式（<c>business_mode</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/supplier/api/account/api_get_supplier_base_info.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方契约：<b>POST</b>、请求体参数表为空；<b>不支持云调用</b>。</para>
    /// <para>
    /// 官方适用范围：本接口支持「<b>小店供货商</b>」账号类型调用（供货商自身凭据视角，与商家视角的
    /// <c>supplier/relation/*</c> 关联族不同）；代发模式（<c>is_dropshipping</c>）个人 / 企业主体均可用、
    /// 供货模式（<c>is_supply</c>）仅企业主体可用（官方原文，SDK 不做本地判定）。
    /// </para>
    /// <para>官方错误码：<c>44002</c>（请求内容为空）/ <c>47001</c>（请求格式错误）/ <c>9700233</c>（供货商不存在）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/supplier/base/get_supplier_base_info")]
    Task<ChannelsGetSupplierBaseInfoResponse> GetSupplierBaseInfoAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取关联的商家列表（<c>get_ec_list</c>，供货商视角：已关联 / 申请关联 / 已取消的商家分页查询）。
    /// </summary>
    /// <param name="request">查询请求（<c>offset</c> / <c>limit</c> 必填；<c>is_filter_type = true</c> 时 <c>type</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商家列表（<c>list</c>）与总数（<c>total_count</c>）；<c>list</c> 官方表例矛盾见响应 DTO remarks。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/supplier/api/related/api_get_ec_list.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方契约：<b>POST</b>；<b>不支持云调用</b>。</para>
    /// <para>
    /// 官方业务限制：①<b>官方暂不提供解除商家关联的 API</b>，解除操作须登录供货商后台手动完成 ⇒
    /// SDK 不提供对应方法、亦不得以「删除路由」变通；②官方适用范围为「<b>小店供货商</b>」账号类型
    /// （供货商自身凭据视角，与本域商家视角 <c>get_supplier_list</c> 互为反向查询）；
    /// ③筛选参数为条件必填（<c>is_filter_type = true</c> 时 <c>type</c> 必填），SDK 不做本地拦截，
    /// 越界由错误码 <c>268580000</c>（参数错误）表达。
    /// </para>
    /// <para>官方状态枚举：<c>status</c> 0 关联申请 / 1 已关联 / 2 已取消关联；<c>bind_audit_status</c> 0 待审核 / 1 已关联 / 2 已主动拒绝 / 3 超时自动拒绝 / 5 已取消关联（官方无 4，照录）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/supplier/relation/get_ec_list")]
    Task<ChannelsGetEcListResponse> GetEcListAsync(
        [Body] ChannelsGetEcListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 供货商视角查询代发单详情（<c>dropship/supplier/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>ds_order_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>代发单结构（<c>dropship_order</c>：状态 / <b>商家</b>标识 / 商品明细 / 发货信息 / 扩展信息）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/supplier/api/dropship/dropship-order/api_dropship_get.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方契约：<b>POST</b>；<b>不支持云调用</b>；支持第三方平台代供货商调用。</para>
    /// <para>
    /// 官方适用范围：本接口支持「<b>小店供货商</b>」账号类型调用（供货商自身凭据视角，查询发起到本供货商的代发单），
    /// 与商家视角的 <c>order/dropship/get</c> 互为同一单据的两端。
    /// </para>
    /// <para>
    /// <b>两视角参数表差异（照官方原文，禁止互改）</b>：本视角响应以 <c>shop_appid</c>/<c>shop_name</c> 标识商家
    /// （商家侧为 <c>supplier_appid</c>/<c>supplier_name</c>）、<c>ds_order_id</c>/<c>order_id</c> 为 <b>string</b>
    /// （商家侧参数表为 number）、商品明细含 <c>sku_attrs</c>（商家侧参数表无）⇒ 故落独立响应 DTO
    /// <see cref="ChannelsSupplierDropshipGetResponse"/>，请求侧与商家侧同形、复用 <see cref="ChannelsDropshipGetRequest"/>。
    /// </para>
    /// <para>
    /// 官方枚举：代发单状态 <c>status</c> 100 待发货 / 199 部分发货 / 200 已发货 / 300 已取消
    /// （见 <see cref="ChannelsDropshipOrderStatuses"/>）；发货状态 <c>delivery_status</c> 10 / 19 / 20 / 30
    /// （见 <see cref="ChannelsDeliveryStatuses"/>）；预售类型 <c>full_payment_presale_delivery_type</c> 0 付款后 n 天发货 /
    /// 1 预售结束后 n 天发货；定制类型 <c>custom_type</c> 1 文字定制 / 2 图片定制。
    /// </para>
    /// <para>官方错误码：<c>610011</c>（代发单不存在）。变更日志：2026-01-29 补充代发单发货状态字段枚举值。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/dropship/supplier/get")]
    Task<ChannelsSupplierDropshipGetResponse> GetSupplierDropshipOrderAsync(
        [Body] ChannelsDropshipGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 供货商视角拉取代发单列表（<c>dropship/supplier/list</c>）。
    /// </summary>
    /// <param name="request">查询请求（全字段选填；时间范围起止距离不可超过 7 天）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>代发单号列表（<c>ds_order_id_list</c>）、分页参数（<c>next_page</c>）、总数与翻页标记（<c>has_more</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/supplier/api/dropship/dropship-order/api_dropship_list.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方契约：<b>POST</b>；<b>不支持云调用</b>；支持第三方平台代供货商调用。</para>
    /// <para>
    /// 官方业务限制：①<c>create_time_range</c>/<c>update_time_range</c> 秒级时间戳且起止距离不可超过 7 天；
    /// ②<c>page_info.limit</c> &lt;= 100；③推荐用 <c>next_key</c> 翻页（代替 <c>offset</c>：首页传空，后续用响应
    /// <c>next_page</c> 回填）；④可用 <c>order_id</c> 反查代发单。
    /// </para>
    /// <para>
    /// <b>与商家侧 <c>dropship/list</c> 的参数表差异</b>：本视角过滤商家为 <c>shop_appid</c>（商家侧为
    /// <c>supplier_appid</c>）、<c>order_id</c> 官方为 <b>string</b>（商家侧参数表为 number）⇒ 落独立请求 DTO
    /// <see cref="ChannelsSupplierDropshipListRequest"/>；响应与商家侧同形，复用 <see cref="ChannelsDropshipOrderListResponse"/>。
    /// </para>
    /// <para>官方枚举：代发单状态 <c>status</c> 100 待发货 / 199 部分发货 / 200 已发货 / 300 已取消。</para>
    /// <para>
    /// 官方错误码：<c>40097</c>（请求体参数不正确）/ <c>47001</c>（请求体格式不正确）。
    /// 变更日志：2026-06-23 <c>offset</c> 改为非必填、新增 <c>next_key</c> 分页标识；2025-12-25 新增 <c>update_time_range</c>。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/dropship/supplier/list")]
    Task<ChannelsDropshipOrderListResponse> GetSupplierDropshipOrderListAsync(
        [Body] ChannelsSupplierDropshipListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 供货商视角搜索代发单（<c>dropship/supplier/search</c>，多条件组合搜索）。
    /// </summary>
    /// <param name="request">搜索请求（<c>search_condition</c> 必填且至少设置一个字段；<c>page_info</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>代发单号列表（<c>ds_order_id_list</c>）、分页参数（<c>next_page</c>）、总数与翻页标记（<c>has_more</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/supplier/api/dropship/dropship-order/api_dropship_search.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方契约：<b>POST</b>；<b>不支持云调用</b>；支持第三方平台代供货商调用。</para>
    /// <para>
    /// 官方业务限制：①<c>search_condition</c> <b>至少设置一个字段</b>，否则接口报错（SDK 不做本地字段数拦截，
    /// 由 <c>40097</c> 表达）；②<c>page_info</c> 必填（2025-10-31 变更日志）且 <c>limit</c> &lt;= 100；
    /// ③<c>waybill_id</c> 官方限定「<b>只可独立查询</b>」，不得与其它条件同传；④各时间范围秒级时间戳、起止距离不可超过 7 天。
    /// </para>
    /// <para>
    /// <b>与商家侧 <c>dropship/search</c> 的参数表差异</b>：本视角为 <c>shop_appid</c>/<c>shop_appname</c>
    /// （商家侧为 <c>supplier_appid</c>/<c>supplier_appname</c>）⇒ 落独立条件 DTO
    /// <see cref="ChannelsSupplierDropshipSearchCondition"/>；响应与商家侧同形，复用 <see cref="ChannelsDropshipOrderListResponse"/>。
    /// </para>
    /// <para>官方枚举：代发单状态 <c>status</c> 100 待发货 / 199 部分发货 / 200 已发货 / 300 已取消。</para>
    /// <para>
    /// 官方错误码：<c>40097</c>（请求体参数不正确）/ <c>47001</c>（请求体格式不正确）。
    /// 变更日志：2025-12-25 新增 <c>update_time_range</c>；2025-11-04 增加筛选条件；2025-10-31 <c>page_info</c> 改为必填。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/dropship/supplier/search")]
    Task<ChannelsDropshipOrderListResponse> SearchSupplierDropshipOrdersAsync(
        [Body] ChannelsSupplierDropshipSearchRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 代发单 / 供货单发货（<c>dropship/supplier/delivery/send</c>）。
    /// </summary>
    /// <param name="request">发货请求（<c>ds_order_id</c> 与 <c>supply_order_id</c> 选传其一；<c>delivery_list</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方应答仅含 <c>errcode</c> / <c>errmsg</c>，无业务字段。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/supplier/api/delivery/shipping/api_delivery_send.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方契约：<b>POST</b>；<b>不支持云调用</b>；支持第三方平台代供货商调用。</para>
    /// <para>
    /// 官方业务限制：①<b>代发单的发货参数是商品信息、供货单的发货参数是货品信息</b>（字段名同一、语义随单据类型而变）；
    /// ②官方声明「本接口无特殊注意事项」；③代发货单必须使用微信小店「电子面单」运单号
    /// （官方错误码 <c>606046</c>：新运单号同步可能延迟，可稍后重试）。
    /// </para>
    /// <para>
    /// <b>官方参数表矛盾照录</b>：<c>ds_order_id</c> 与 <c>supply_order_id</c> 同标「必填=是」，说明却为「选传其一」⇒
    /// 两字段均建模为可空、SDK 不做「恰好其一」的本地拦截，非法组合由官方 <c>40097</c> 表达。
    /// </para>
    /// <para>
    /// 官方错误码（择要）：<c>40097</c> / <c>47001</c> / <c>48001</c>（无权调用）/ <c>100003</c>（订单更新过于频繁）/
    /// <c>101100</c>（当前订单状态不允许发货）/ <c>108009</c>（有未完成售后单）/ <c>109000</c>（商品或数量填写不正确）/
    /// <c>109001</c>（已发货完成，不能重复发货）/ <c>606003</c>（商家没有默认退货地址）/ <c>606004</c>（delivery_id 不合法）/
    /// <c>606005</c>（waybill_id 不合法）/ <c>606031</c>（product_id 或 sku_id 填写不正确）/ <c>606046</c>（须用电子面单运单号）/
    /// <c>606049</c>（订单存在风险）/ <c>606052</c>（代发单已取消）/ <c>606053</c>（代发单不存在）/ <c>606057</c>（代发单与订单不匹配）/
    /// <c>606059</c>（电子面单信息不匹配）/ <c>6060479</c>（快递单号与物流公司正则不匹配）/ <c>10020115</c>（用户申请修改收货地址，待商家处理）/
    /// <c>10020276</c>（存在发货前更换 sku 请求，待商家处理）。
    /// </para>
    /// <para>官方适用范围：本接口支持「小店供货商」账号类型调用。变更日志：2026-05-18 新增 <c>supply_order_id</c> 支持供货单发货。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/dropship/supplier/delivery/send")]
    Task<ChannelsResponse> SendSupplierDeliveryAsync(
        [Body] ChannelsSupplierDeliverySendRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改代发单 / 供货单物流（<c>dropship/supplier/delivery/update</c>）。
    /// </summary>
    /// <param name="request">修改请求（<c>ds_order_id</c> 与 <c>supply_order_id</c> 选传其一；<c>change_infos</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方应答仅含 <c>errcode</c> / <c>errmsg</c>，无业务字段。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/supplier/api/delivery/shipping/api_delivery_update.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方契约：<b>POST</b>；<b>不支持云调用</b>；支持第三方平台代供货商调用。</para>
    /// <para>
    /// 官方业务限制：①<b>只允许修改运单号</b>，商品信息必须保持一致，且<b>修改次数有限</b>（官方错误码 <c>606041</c>：
    /// 修改物流次数已达上限）；②<b>只支持修改为通过微信小店电子面单取号的运单号</b>，不支持修改为外部获取的运单号。
    /// </para>
    /// <para>
    /// <b>官方参数表矛盾照录</b>：<c>ds_order_id</c> 与 <c>supply_order_id</c> 同标「必填=是」，说明为「选传其一」⇒
    /// 两字段均可空、SDK 不拦截；<c>old</c> / <c>new</c> 官方结构逐字段一致 ⇒ 两侧复用同一包裹类型
    /// <see cref="ChannelsSupplierDeliveryPackage"/>。
    /// </para>
    /// <para>
    /// 官方错误码：<c>40097</c> / <c>47001</c> / <c>48001</c> / <c>606041</c>（修改次数已达上限）/
    /// <c>606058</c>（原运单不存在）/ <c>606059</c>（电子面单信息不匹配）。
    /// 变更日志：2026-05-18 新增 <c>supply_order_id</c> 支持供货单修改物流。
    /// </para>
    /// <para>官方适用范围：本接口支持「小店供货商」账号类型调用。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>192</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/order/dropship/supplier/delivery/update")]
    Task<ChannelsResponse> UpdateSupplierDeliveryAsync(
        [Body] ChannelsSupplierDeliveryUpdateRequest request,
        CancellationToken cancellationToken = default);
}
