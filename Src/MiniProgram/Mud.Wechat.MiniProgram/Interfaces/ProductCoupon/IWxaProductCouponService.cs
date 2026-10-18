// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;
using Mud.Wechat.MiniProgram.DataModels.ProductCoupon;
using Mud.Wechat.OfficialAccount.Abstractions.Authentication;

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「基础交易工具（原标准版交易组件 V1）」·优惠券域（7 端点：创建 / 查询 / 列表 /
/// 用户券 / 定向发放 / 更新 / 更新状态；路由族 <c>/product/coupon/*</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>读写两侧字段类型不同源</b>：请求侧时间为 Unix 秒级戳、<c>promote_type</c> / <c>valid_type</c>
/// 为数字；响应侧时间转 <c>yyyy-MM-dd HH:mm:ss</c> 字符串、枚举转
/// <c>PROMOTE_TYPE_*</c> / <c>COUPON_VALID_TYPE_*</c> 符号串 ⇒ SDK 为两侧分别建模，
/// 不得共用同一 DTO。
/// </para>
/// <para>
/// <b>官方页面自身存在冲突（诚实留档）</b>：create 页 <c>valid_type</c> 为 1 指定时间 / 2 生效天数，
/// update 页为 0 指定时间 / 1 生效天数；update 页参数表与示例均<b>未列</b> <c>coupon_id</c> ⇒
/// 本仓在更新请求中补齐定位键并钉住该取舍。
/// </para>
/// <para>
/// <b>金额口径</b>：<c>discount_fee</c> 为分；<c>discount_num</c> 为折扣千分比（5000 = 5 折，
/// 范围 1000~10000、必须为 100 的整数倍）。
/// </para>
/// <para>
/// <b>个人信息红线</b>：<c>openid</c> 与用户券明细属个人信息 ⇒ 禁入日志、遥测与异常消息。
/// </para>
/// <para>
/// <b>调用限制（诚实留档）</b>：本族 7 页官方正文未给端点级频率数字、权限集编号与云调用方法名
/// （2026-10-18 逐页取证）⇒ 不得凭第三方 SDK 注释或记忆补数。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ProductCoupon", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaProductCouponService
{
    /// <summary>
    /// 创建优惠券（<c>POST /product/coupon/create</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/coupon/create_coupon.html"/>。
    /// </summary>
    /// <param name="request">请求体（券型 + 券面 + 领取 / 有效期设置），见 <see cref="WxaProductCouponCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>data.coupon_id</c>，见 <see cref="WxaProductCouponCreateResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + JSON；<c>type</c> / <c>name</c> / <c>discount_info</c> /
    /// <c>ext_info</c> / <c>promote_info</c> / <c>receive_info</c> / <c>valid_info</c> 为顶层必填面，
    /// 内部字段按 <see cref="WxaProductCouponTypes"/> 的券型组合。
    /// </para>
    /// <para><b>非幂等</b>：重复提交同一券面会创建多张券 ⇒ 不得自动重试。</para>
    /// </remarks>
    [Post("/product/coupon/create")]
    Task<WxaProductCouponCreateResponse> CreateCouponAsync(
        [Body] WxaProductCouponCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取优惠券详情（<c>POST /product/coupon/get</c>，官方「查询」类端点仍是 POST）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/coupon/get.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>coupon_id</c>），见 <see cref="WxaProductCouponGetRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>顶层 <c>coupon</c> 单对象，见 <see cref="WxaProductCouponGetResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>响应侧枚举为符号串</b>：<c>promote_type</c> / <c>valid_type</c> 为
    /// <c>PROMOTE_TYPE_*</c> / <c>COUPON_VALID_TYPE_*</c>，<c>status</c> 仍为数值。
    /// </para>
    /// <para><b>库存可能为空对象</b>：官方示例 <c>stock_info</c> 为 <c>{}</c> ⇒ 各计数均可空。</para>
    /// </remarks>
    [Post("/product/coupon/get")]
    Task<WxaProductCouponGetResponse> GetCouponAsync(
        [Body] WxaProductCouponGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取优惠券列表（<c>POST /product/coupon/get_list</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/coupon/get_coupon.html"/>。
    /// </summary>
    /// <param name="request">请求体（创建时间窗口 + 状态 + 翻页），见 <see cref="WxaProductCouponGetListRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>顶层 <c>coupons[]</c> + <c>total_num</c>，见 <see cref="WxaProductCouponGetListResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>时间窗口与状态均必填</b>：<c>start_create_time</c> / <c>end_create_time</c> 缺一不可，
    /// <c>page_size</c> 官方上限 200（本域最小上限，create 页无翻页概念）。
    /// </para>
    /// </remarks>
    [Post("/product/coupon/get_list")]
    Task<WxaProductCouponGetListResponse> GetCouponListAsync(
        [Body] WxaProductCouponGetListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户优惠券（<c>POST /product/coupon/get_user</c>，官方「查询」类端点仍是 POST）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/coupon/get_user_coupon.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>openid</c> + 可选状态），见 <see cref="WxaProductCouponGetUserRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>顶层 <c>user_coupon_list[]</c>，见 <see cref="WxaProductCouponGetUserResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>请求数值 / 响应符号串（照录）</b>：请求 <c>status</c> 取 100/101/102，
    /// 响应 <c>status</c> 为 <c>COUPON_USER_STATUS_*</c> ⇒ 两侧不共用类型。
    /// </para>
    /// <para><b>无分页</b>：官方只回列表，不分页、无 total。</para>
    /// <para><b>敏感值</b>：<c>openid</c> 与用户券明细禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/product/coupon/get_user")]
    Task<WxaProductCouponGetUserResponse> GetUserCouponAsync(
        [Body] WxaProductCouponGetUserRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 定向发放优惠券（<c>POST /product/coupon/push</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/coupon/push_coupon.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>openid</c> + <c>coupon_id</c>），见 <see cref="WxaProductCouponPushRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>库存与限领在服务端裁决</b>：109100 状态不对 / 109101 库存不足 / 109102 未过期 /
    /// 109103 限领 ⇒ 失败须原样回传调用方。
    /// </para>
    /// <para>
    /// <b>可重试性依错误码而定</b>：109101 属暂时性资源不足可稍后重试；109103 为确定性限领，
    /// 重试无意义。本方法不做内部重试。
    /// </para>
    /// <para><b>敏感值</b>：<c>openid</c> 禁入日志、遥测与异常消息。</para>
    /// </remarks>
    [Post("/product/coupon/push")]
    Task<WxaResponse> PushCouponAsync(
        [Body] WxaProductCouponPushRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新优惠券（<c>POST /product/coupon/update</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/coupon/update_coupon.html"/>。
    /// </summary>
    /// <param name="request">请求体（券面字段 + SDK 补齐的 <c>coupon_id</c>），见 <see cref="WxaProductCouponUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>data.coupon_id</c>，见 <see cref="WxaProductCouponCreateResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// <b>官方本页缺定位键（诚实留档）</b>：参数表与示例均未列出 <c>coupon_id</c> ⇒ 本仓补齐，
    /// 调用时须携带；守卫锁定这一补键取舍。
    /// </para>
    /// <para>
    /// <b><c>valid_type</c> 枚举与本域 create 页冲突</b>：本页为 0/1、create 页为 1/2，
    /// 见 <see cref="WxaProductCouponValidTypes"/> 的双套常量。
    /// </para>
    /// </remarks>
    [Post("/product/coupon/update")]
    Task<WxaProductCouponCreateResponse> UpdateCouponAsync(
        [Body] WxaProductCouponUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新优惠券状态（<c>POST /product/coupon/update_status</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/platform-capabilities/business-capabilities/ministore/minishopopencomponent/API/coupon/update_coupon_status.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>coupon_id</c> + 目标状态），见 <see cref="WxaProductCouponUpdateStatusRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>只接受 2 / 4 / 5</b>（生效 / 作废 / 删除）：状态推进单向、删除不可撤销
    /// ⇒ 非幂等安全，不得自动重试。
    /// </para>
    /// </remarks>
    [Post("/product/coupon/update_status")]
    Task<WxaResponse> UpdateCouponStatusAsync(
        [Body] WxaProductCouponUpdateStatusRequest request,
        CancellationToken cancellationToken = default);
}
