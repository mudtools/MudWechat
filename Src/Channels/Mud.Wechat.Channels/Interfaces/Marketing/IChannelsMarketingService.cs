// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.DataModels.Marketing;

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「营销管理-优惠券」域 SDK（7 端点：创建 / 详情 / ID 列表 /
/// 用户券详情 / 用户券 ID 列表 / 更新内容 / 更新状态，路由前缀 <c>/channels/ec/coupon/*</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与「本地生活团购券」（<see cref="IChannelsLocallifeService"/>，<c>/channels/ec/voucher/*</c>）
/// 概念严格区分</b>：本域是小店电商优惠券（coupon，满减/折扣/直减），团购券是到店核销凭证
/// （voucher，code 核销 + 分账结算），两域字段体系互不复用。
/// </para>
/// <para>
/// <b>官方第三方代调用</b>：本域全部端点所属权限集 id 为 <c>132</c>（服务商可用
/// <c>authorizer_access_token</c> 代小店商家调用）。
/// </para>
/// <para>
/// <b>MUD005 已知接受风险</b>：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Marketing", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsMarketingService
{
    /// <summary>
    /// 创建优惠券（<c>createcoupon</c>，官方路由 <c>/channels/ec/coupon/create</c>）。
    /// </summary>
    /// <param name="request">创建请求（<c>type</c>/<c>name</c>/<c>promote_info</c>/<c>receive_info</c> 必填，其余选填；类型决定必填的优惠字段组）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新优惠券 ID（<c>data.coupon_id</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/coupon/api_createcoupon"/></para>
    /// <para>
    /// 官方业务限制：①创建成功后 <c>status=1</c>（编辑中），需经「更新优惠券状态」流转到生效；
    /// ②名称最长 10 个中文字符；③商品券 <c>product_ids</c> 必填且最多 200 个；
    /// ④折扣数范围 1000~10000（5000=5 折）且必须是 100 的整数倍、不可低于 2 折；
    /// ⑤直减券减免金额（分）不可超过 200 元，商品券优惠后不低于原价 2 折、店铺满减券不低于门槛 2 折；
    /// ⑥件数门槛与价格门槛不能同时设置；⑦所有时间字段均为秒级时间戳。
    /// </para>
    /// <para>官方错误码（节选）：10021005 名称太长 / 10021014 推广类型不对 / 10021080 置券时传了非上架商品。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>132</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/coupon/create")]
    Task<ChannelsCouponDataResponse> CreateCouponAsync(
        [Body] ChannelsCouponCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取优惠券详情（<c>getcoupon</c>，官方路由 <c>/channels/ec/coupon/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>coupon_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>优惠券全量信息（<c>coupon</c>：类型 / 状态 / 内容 <c>coupon_info</c> / 库存 <c>stock_info</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/coupon/api_getcoupon"/></para>
    /// <para>官方业务限制：本接口无特殊注意事项；优惠券类型见 <see cref="ChannelsCouponTypes"/>、状态见 <see cref="ChannelsCouponStatuses"/>。</para>
    /// <para>官方错误码：<c>10021037</c>（优惠券不存在）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>132</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/coupon/get")]
    Task<ChannelsGetCouponResponse> GetCouponAsync(
        [Body] ChannelsGetCouponRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取优惠券 ID 列表（<c>getcouponlist</c>，官方路由 <c>/channels/ec/coupon/get_list</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>status</c>/<c>page</c>/<c>page_size</c>/<c>page_ctx</c> 必填；<c>page_ctx</c> 首次请求填空串）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>优惠券 ID 列表（<c>coupons</c>）、总数（<c>total_num</c>）与翻页上下文（<c>page_ctx</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/coupon/api_getcouponlist"/></para>
    /// <para>
    /// 官方业务限制：①每次请求的页码间隔不能超过 10；②第一次请求的页码要小于 10，或者是最后一页；
    /// ③一页最大 200 条；④状态筛选见 <see cref="ChannelsCouponStatuses"/>（含官方 <c>200=过期或作废</c> 复合值）。
    /// </para>
    /// <para>官方错误码：<c>10021001</c>（优惠券状态不对）/ <c>10021036</c>（页码与上次请求相差过大）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>132</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/coupon/get_list")]
    Task<ChannelsGetCouponListResponse> GetCouponListAsync(
        [Body] ChannelsGetCouponListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户优惠券详情（<c>getusercoupon</c>，官方路由 <c>/channels/ec/coupon/get_user_coupon</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>user_coupon_id</c> 与 <c>openid</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>用户券详情（<c>user_coupon</c>：状态 / 生效失效时间 / 使用的订单 ID），外层含 <c>openid</c>/<c>unionid</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/coupon/api_getusercoupon"/></para>
    /// <para>
    /// 官方业务限制：本接口无特殊注意事项；用户券状态见 <see cref="ChannelsUserCouponStatuses"/>（100/101/102，与商家券状态枚举不同轴）。
    /// 官方变更日志（2026-06-03）：响应参数 <c>user_coupon_list</c> 修正为 <c>user_coupon</c>。
    /// </para>
    /// <para>官方错误码：<c>40003</c>（OpenID 不正确）/ <c>10021037</c>（优惠券不存在）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>132</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/coupon/get_user_coupon")]
    Task<ChannelsGetUserCouponResponse> GetUserCouponAsync(
        [Body] ChannelsGetUserCouponRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户优惠券 ID 列表（<c>getusercouponlist</c>，官方路由 <c>/channels/ec/coupon/get_user_coupon_list</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>openid</c>/<c>page</c>/<c>page_size</c>/<c>page_ctx</c> 必填；<c>status</c> 选填，不填查全部状态）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>用户券 ID 列表（<c>user_coupon_list</c>）与翻页上下文（<c>page_ctx</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/coupon/api_getusercouponlist"/></para>
    /// <para>
    /// 官方业务限制：①每次请求的页码间隔不能超过 10；②第一次请求的页码要小于 10，或者是最后一页；
    /// ③一页最大 200 条；④用户券状态筛选见 <see cref="ChannelsUserCouponStatuses"/>。
    /// </para>
    /// <para>官方错误码：<c>40003</c> / <c>10021001</c> / <c>10021036</c> / <c>10021037</c>。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>132</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/coupon/get_user_coupon_list")]
    Task<ChannelsGetUserCouponListResponse> GetUserCouponListAsync(
        [Body] ChannelsGetUserCouponListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新优惠券内容（<c>updatecoupon</c>，官方路由 <c>/channels/ec/coupon/update</c>）。
    /// </summary>
    /// <param name="request">更新请求（<c>coupon_id</c>/<c>type</c>/<c>name</c>/<c>promote_info</c>/<c>receive_info</c>/<c>valid_info</c> 必填；更新形态无 <c>auto_valid_info</c>，官方参数表原文如此）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>优惠券 ID（<c>data.coupon_id</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/coupon/api_updatecoupon"/></para>
    /// <para>
    /// 官方业务限制：与创建同源的优惠规则（折扣数 1000~10000 且 100 的整数倍不可低于 2 折、
    /// 直减金额不可超过 200 元、product_ids 最多 200 个、件数/价格门槛互斥）；
    /// 与创建的差异：更新形态 <c>valid_info</c> 必填且不支持 <c>auto_valid_info</c>。
    /// </para>
    /// <para>官方错误码（节选）：10021005 / 10021006 / 10021007 / 10021014 / 10021037。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>132</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/coupon/update")]
    Task<ChannelsCouponDataResponse> UpdateCouponAsync(
        [Body] ChannelsCouponUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新优惠券状态（<c>updatecouponstatus</c>，官方路由 <c>/channels/ec/coupon/update_status</c>）。
    /// </summary>
    /// <param name="request">状态更新请求（<c>coupon_id</c>/<c>status</c> 必填；目标状态 2=生效、4=作废、5=删除）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/coupon/api_updatecouponstatus"/></para>
    /// <para>
    /// 官方业务限制：优惠券创建后 <c>status=1</c>（编辑中），仅可流转到 2（生效）、4（作废）、5（删除）；
    /// 目标状态是 <see cref="ChannelsCouponStatuses"/> 的子集。
    /// </para>
    /// <para>官方错误码：<c>10021001</c>（优惠券状态不对）/ <c>10021003</c>（优惠券还没过期）/ <c>10021034</c>（优惠券重入失败）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>132</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/coupon/update_status")]
    Task<ChannelsResponse> UpdateCouponStatusAsync(
        [Body] ChannelsCouponUpdateStatusRequest request,
        CancellationToken cancellationToken = default);
}
