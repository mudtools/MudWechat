// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.DataModels.Talent;

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「带货助手（达人）」域 SDK（4 端点：佣金单 2 / 达人橱窗商品 2）。
/// </summary>
/// <remarks>
/// <para>
/// <b>账号类型与令牌归属</b>：本域四端点的适用范围官方均写「<b>带货助手（需申请）</b>」，
/// 即凭据为带货助手的 <c>access_token</c>，或第三方服务商代调用时的 <c>authorizer_access_token</c>
/// （权限集 id：佣金单 <c>190</c> / 橱窗 <c>189</c>）。令牌类型仍为本线的
/// <see cref="ChannelsTokenTypes.AccessToken"/> —— 与 League（联盟机构）、小店商家侧共用同一令牌类型，
/// 「哪一种账号」由应用配置里的 AppID/凭据区分，<b>不按账号类型拆分令牌类型</b>（同 League 域先例裁决）。
/// </para>
/// <para>
/// <b>与 Window / League 两域的严格区分（三处近邻，均不合并）</b>：
/// ① 本域橱窗面为 <c>/channels/ec/talent/window/product/*</c>（带货助手视角，2 端点），
/// 与 <see cref="IChannelsWindowService"/> 的 <c>/channels/ec/window/product/*</c>（视频号助手视角）
/// 是<b>两套独立官方契约</b>——请求体、响应体、错误码、权限集全部不同，不得合并；
/// ② 本域佣金单面为 <c>/channels/ec/talent/get_order_detail</c> 与 <c>/get_order_list</c>
/// （达人/带货助手查自己的佣金单），与 <see cref="IChannelsLeagueService"/> 的
/// <c>/channels/ec/league/headsupplier/order/*</c>（机构查全部佣金单）不同前缀不同归属；
/// ③ 本域订单状态与佣金单状态枚举与 League 侧同名字段<b>是两张表</b>，故常量类分建
/// （见 <see cref="ChannelsTalentOrderStatuses"/>、<see cref="ChannelsTalentCommissionOrderStates"/>）。
/// </para>
/// <para>
/// <b>官方文档矛盾照录（本域逐页核验后的建模口径写在每个端点的注释里）</b>：详情响应示例的
/// <c>actual_payment</c> 为 "99.99" 而参数表注明「单位：分」；列表条目 <c>product_id</c> 参数表标 number、
/// 示例为字符串；官方请求示例存在畸形 JSON（<c>"order_id": 123""</c>）。
/// </para>
/// <para>
/// MUD005 已知接受风险：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Talent", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsTalentService
{
    /// <summary>
    /// 获取佣金单详情（<c>get_order_detail</c>，带货助手查自己单笔佣金单的订单 / 佣金 / 下单通道 / 推客机构 / 商品信息）。
    /// </summary>
    /// <param name="request">查询请求（<c>order_id</c> 与 <c>sku_id</c> 必填，均可从获取佣金单列表接口获得；<c>special_id</c> 选填，回传订单列表返回的额外参数）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单基础信息（<c>base_info</c>）、佣金信息（<c>commission_info</c>）、下单通道信息（<c>channel_info</c>）、内容推广推客机构信息（<c>promotion_head_supplier_info</c>）与商品信息（<c>product_info</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/talent/openapi/order/api_get_order_detail.html"/></para>
    /// <para>
    /// 官方业务限制：①<b>不支持云调用</b>；②支持第三方平台代带货助手调用，所属权限集 id 为 <c>190</c>
    /// （服务商获得授权后用 <c>authorizer_access_token</c> 代调用）；③仅「带货助手（需申请）」账号类型可调用；
    /// ④专属错误码 <c>10024000</c>（参数错误，请检查传递参数）/ <c>10024006</c>（不存在该佣金单）；
    /// ⑤官方注明「本接口无特殊注意事项」。
    /// </para>
    /// <para>
    /// <b>官方文档矛盾照录</b>：①响应参数表 <c>base_info.actual_payment</c> 注明「单位：分」、
    /// 返回示例为 <c>"99.99"</c>（元形态）——SDK 照抄官方类型标注建模为字符串、不做单位换算；
    /// ②返回示例的 <c>errcode</c> 是字符串 <c>"0"</c> 而参数表标注 number——按参数表与各线统一响应基座处置；
    /// ③请求示例为畸形 JSON（<c>"order_id": 123""</c>，多一个引号）；④返回示例未出现
    /// <c>promotion_head_supplier_info</c> 与 <c>order_pay_time</c>/<c>settle_payment</c>（后两者为变更日志新增字段）；
    /// ⑤「注意事项」小节编号漂移（正文为 <c># 4. 注意事项</c> 而错误码起于 <c># 7</c>）。
    /// </para>
    /// <para>接口变更日志：2026-01-15 新增 <c>settle_payment</c>（外部对齐佣金单结算基数）；2025-11-17 新增 <c>order_pay_time</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/talent/get_order_detail")]
    Task<ChannelsTalentGetOrderDetailResponse> GetOrderDetailAsync(
        [Body] ChannelsTalentGetOrderDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取佣金单列表（<c>get_order_list</c>，按创建时间或更新时间范围顺序翻页拉取自己的佣金单）。
    /// </summary>
    /// <param name="request">查询请求（<c>page_size</c> 必填且<b>不超过 10</b>；<c>create_time_gt</c>/<c>create_time_lt</c> 与 <c>update_time_gt</c>/<c>update_time_lt</c> <b>两组二选一</b>；<c>order_id</c>/<c>spu_id</c> 选填过滤；<c>next_key</c> 顺序翻页上下文）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>佣金单三元组列表（<c>order_list</c>，每项 <c>order_id</c>/<c>sku_id</c>/<c>special_id</c>）、是否还有剩余订单（<c>has_more</c>）与翻页上下文（<c>next_key</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/talent/openapi/order/api_get_order_list.html"/></para>
    /// <para>
    /// 官方业务限制：①<b>不支持云调用</b>；②支持第三方平台代带货助手调用，所属权限集 id 为 <c>190</c>；
    /// ③仅「带货助手（需申请）」账号类型可调用；④<b>page_size 上限 10</b>（本线其他列表面普遍为 30/100/500，本域为最小档）；
    /// ⑤<b>创建时间范围与更新时间范围二选一</b>（官方对四个时间参数逐条标注）；⑥官方要求「使用前请阅读开发者指南」，
    /// 本接口无特殊错误码（走通用错误码）。
    /// </para>
    /// <para>
    /// <b>官方文档矛盾照录</b>：响应参数表把 <c>order_list</c> 的类型列渲染为带模板标记的
    /// <c>" data-v-…&gt;objarray</c>（文档渲染缺陷，非类型信息）——SDK 按「Res.order_list(Array) Object Payload」
    /// 小节建模为对象数组；返回示例未出现 <c>next_key</c>（仅 <c>has_more</c>）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/talent/get_order_list")]
    Task<ChannelsTalentGetOrderListResponse> GetOrderListAsync(
        [Body] ChannelsTalentGetOrderListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取达人橱窗商品详情（<c>get_product_detail</c>，带货助手视角的橱窗商品单查）。
    /// </summary>
    /// <param name="request">查询请求（<c>product_id</c> 必填，橱窗商品 ID，从获取达人橱窗商品列表接口获得）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>橱窗商品详情（<c>product</c>：商品 ID / 来源店铺 appid / 货源商品 ID / 标题 / 头图 / 叶子类目 / 状态 / 售价 / 库存 / 销量 / 是否隐藏 / 跟佣跳转串）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/talent/openapi/window/api_get_product_detail.html"/></para>
    /// <para>
    /// 官方业务限制：①<b>不支持云调用</b>；②支持第三方平台代带货助手调用，所属权限集 id 为 <c>189</c>；
    /// ③仅「带货助手（需申请）」账号类型可调用；④<b>商品状态枚举仅两值</b>：<c>1</c> 生效中、<c>2</c> 被禁止售卖
    /// （见 <see cref="ChannelsTalentWindowProductStatuses"/>，与视频号助手橱窗面不是同一张表）；
    /// ⑤专属错误码 <c>10200006</c>（达人平台账号异常）/ <c>10200007</c>（商品未在达人平台橱窗）/
    /// <c>10200008</c>（商品状态异常），三者官方均要求「请登录达人平台后台确认」。
    /// </para>
    /// <para>
    /// <b>与 Window 域不合并（照录裁决）</b>：本端点 <c>/channels/ec/talent/window/product/get</c> 与
    /// <see cref="IChannelsWindowService"/> 的 <c>/channels/ec/window/product/get</c> 请求体同为 <c>product_id</c>，
    /// 但响应体字段集、状态枚举、错误码与权限集（189 对 133/177）全部不同，属两套契约，SDK 各自建模。
    /// </para>
    /// <para>
    /// <b>官方文档矛盾照录</b>：<c>selling_price</c> 官方注明「价格区间最小值（单位分）（销售价）」、
    /// 示例为 <c>999</c>，与同域列表侧无对应字段；SDK 按参数表 number 建模为 <see cref="long"/>、不做单位换算。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/talent/window/product/get")]
    Task<ChannelsTalentGetWindowProductDetailResponse> GetWindowProductDetailAsync(
        [Body] ChannelsTalentGetWindowProductDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取达人橱窗商品列表（<c>get_product_list</c>，支持页码分页与 <c>last_buffer</c> 顺序翻页两种互斥方式）。
    /// </summary>
    /// <param name="request">查询请求（<c>page_size</c> 必填且<b>不超过 500</b>；<c>page_index</c> 从 1 开始，<b>与 <c>last_buffer</c> 互斥、不可同时填写</b>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>橱窗商品列表（<c>products</c>，每项含 <c>product_id</c> 与来源标识）与顺序翻页上下文（<c>last_buffer</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/talent/openapi/window/api_get_product_list.html"/></para>
    /// <para>
    /// 官方业务限制：①<b>不支持云调用</b>；②支持第三方平台代带货助手调用，所属权限集 id 为 <c>189</c>；
    /// ③仅「带货助手（需申请）」账号类型可调用；④<b>两种翻页方式互斥</b>：<c>page_size</c>+<c>page_index</c>
    /// 受 <b>page_size × page_index ≤ 10000</b> 约束，<c>last_buffer</c> 顺序翻页无数量限制，
    /// 官方明确「当数据量较大时建议优先使用 last_buffer 顺序翻页」；⑤<b>商品来源二分</b>：
    /// <c>product_source</c>=1 来源店铺自营商品（此时返回 <c>appid</c>）、=2 来源选品中心带货商品
    /// （此时返回 <c>out_product_id</c>），见 <see cref="ChannelsTalentWindowProductSources"/>；
    /// ⑥专属错误码 <c>10200003</c>（page_size 超过上限 500）/ <c>10200004</c>（page_index 与 last_buffer 同时存在）/
    /// <c>10200005</c>（page_size × page_index 超过 10000，建议改用 last_buffer）。
    /// </para>
    /// <para>
    /// <b>官方文档矛盾照录</b>：列表条目 <c>product_id</c> 的「Res.products(Array) Object Payload」参数表标注
    /// number，而返回示例为带引号字符串（<c>"1000000000000"</c>）——SDK <b>按参数表建模为 number</b> 并在 DTO 注释照录该分歧；
    /// 详情侧（<c>/channels/ec/talent/window/product/get</c>）参数表与示例均为 string，故两处类型不一致属官方原文，不做强统一。
    /// 另响应参数表把 <c>products</c> 的类型列渲染为带模板标记的 <c>" data-v-…&gt;objarray</c>（文档渲染缺陷）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/talent/window/product/list/get")]
    Task<ChannelsTalentGetWindowProductListResponse> GetWindowProductListAsync(
        [Body] ChannelsTalentGetWindowProductListRequest request,
        CancellationToken cancellationToken = default);
}
