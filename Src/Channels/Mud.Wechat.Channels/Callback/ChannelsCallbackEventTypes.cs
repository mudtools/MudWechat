// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

// 微信小店回调事件键常量（官方解密明文顶层 Event 取值，P3 载荷批次逐页核验）。
// 命名空间与载荷契约特性（ChannelsCallbackContractAttribute）、载荷 DTO 同域——
// 载荷类的事件键声明直接引用本类常量（编译期常量，改名即编译错）。

namespace Mud.Wechat.Channels.DataModels.Callback;

/// <summary>
/// 微信小店回调事件键常量（官方解密明文 JSON 顶层 <c>Event</c> 字段取值，共 42 键）。
/// </summary>
/// <remarks>
/// <para>
/// <b>落位说明（勿"修正"到 Abstractions）</b>：本类与 <see cref="ChannelsCallbackContractAttribute"/>
/// 及全部载荷 DTO 同程序集（Mud.Wechat.Channels.DataModels）——本线依赖方向为
/// Channels.Abstractions → Channels.DataModels（与企微线相反：企微载荷在 Callback 包、可上行引用
/// Abstractions 的 <c>WechatCallbackEventTypes</c>；小店载荷是 JSON DTO、必须落 DataModels 承接
/// 源生成 JSON 上下文），故事件键常量只能落本程序集，载荷特性声明才可引用。
/// </para>
/// <para>
/// <b>键值来源</b>：官方回调文档逐页核验（基础 URL
/// <c>https://developers.weixin.qq.com/doc/store/shop/notify/</c>），XML 注释逐键标注中文名与官方页路径。
/// 两处官方易错点：<c>channels_ec_order_ext_info_update</c> 的页面文件名是
/// <c>channels_ec_order_ext_info.html</c>（Event 值以参数表为准）；<c>channels_ec_user_coupon_use</c> 的
/// 参数表 Event 行误写为 <c>channels_ec_user_coupon_unuse</c>（复制粘贴错误，以页面标题、头部事件名与
/// 示例三者一致值为准）。
/// </para>
/// </remarks>
public static class ChannelsCallbackEventTypes
{
    /// <summary>API变更日志回调（官方页：common_callback/API_change_log_callback.html）。</summary>
    public const string ApiDiff = "api_diff";

    /// <summary>小店状态变更回调事件（官方页：storebasics_callback/channels_ec_basic_change_status.html）。</summary>
    public const string ShopChangeStatus = "channels_ec_change_status";

    /// <summary>小店最终注销回调事件（官方页：storebasics_callback/channels_ec_basic_close_store.html）。</summary>
    public const string ShopCloseStore = "channels_ec_close_store";

    /// <summary>小店修改名称事件（官方页：storebasics_callback/set_shop_nickname.html）。</summary>
    public const string SetShopNickname = "set_shop_nickname";

    /// <summary>小店修改头像事件（官方页：storebasics_callback/set_headimg.html）。</summary>
    public const string SetShopHeadimg = "set_shop_headimg";

    /// <summary>商品审核（官方页：product_callback/ProductSpuAudit.html）。</summary>
    public const string ProductSpuAudit = "product_spu_audit";

    /// <summary>商品上下架（官方页：product_callback/ProductSpuListing.html）。</summary>
    public const string ProductSpuListing = "product_spu_listing";

    /// <summary>商品更新（官方页：product_callback/ProductSpuUpdate.html）。</summary>
    public const string ProductSpuUpdate = "product_spu_update";

    /// <summary>类目审核结果通知（官方页：category_callback/ProductCategoryAudit.html）。</summary>
    public const string ProductCategoryAudit = "product_category_audit";

    /// <summary>订单下单（官方页：order_callback/channels_ec_order_new.html）。</summary>
    public const string OrderNew = "channels_ec_order_new";

    /// <summary>订单取消（官方页：order_callback/channels_ec_order_cancel.html）。</summary>
    public const string OrderCancel = "channels_ec_order_cancel";

    /// <summary>订单支付成功（官方页：order_callback/channels_ec_order_pay.html）。</summary>
    public const string OrderPay = "channels_ec_order_pay";

    /// <summary>订单待发货（官方页：order_callback/channels_ec_order_wait_shipping.html）。</summary>
    public const string OrderWaitShipping = "channels_ec_order_wait_shipping";

    /// <summary>订单发货（官方页：order_callback/channels_ec_order_deliver.html）。</summary>
    public const string OrderDeliver = "channels_ec_order_deliver";

    /// <summary>订单确认收货（官方页：order_callback/channels_ec_order_confirm.html）。</summary>
    public const string OrderConfirm = "channels_ec_order_confirm";

    /// <summary>订单结算成功（官方页：order_callback/channels_ec_order_settle.html）。</summary>
    public const string OrderSettle = "channels_ec_order_settle";

    /// <summary>订单其他信息更新（官方页：order_callback/channels_ec_order_ext_info.html；Event 值以参数表为准）。</summary>
    public const string OrderExtInfoUpdate = "channels_ec_order_ext_info_update";

    /// <summary>结算账户变更回调（官方页：funds_callback/channels_ec_acct_notify.html）。</summary>
    public const string AcctNotify = "channels_ec_acct_notify";

    /// <summary>提现回调（官方页：funds_callback/channels_ec_withdraw_notify.html）。</summary>
    public const string WithdrawNotify = "channels_ec_withdraw_notify";

    /// <summary>提现二维码回调（官方页：funds_callback/qrcode_status.html）。</summary>
    public const string QrcodeStatus = "qrcode_status";

    /// <summary>领取优惠券通知（官方页：coupon_callback/channels_ec_coupon_receive.html）。</summary>
    public const string CouponReceive = "channels_ec_coupon_receive";

    /// <summary>创建优惠券通知（官方页：coupon_callback/channels_ec_coupon_create.html）。</summary>
    public const string CouponCreate = "channels_ec_coupon_create";

    /// <summary>删除优惠券通知（官方页：coupon_callback/channels_ec_coupon_delete.html）。</summary>
    public const string CouponDelete = "channels_ec_coupon_delete";

    /// <summary>优惠券过期通知（官方页：coupon_callback/channels_ec_coupon_expire.html）。</summary>
    public const string CouponExpire = "channels_ec_coupon_expire";

    /// <summary>优惠券信息更新通知（官方页：coupon_callback/channels_ec_coupon_info_change.html）。</summary>
    public const string CouponInfoChange = "channels_ec_coupon_info_change";

    /// <summary>作废优惠券通知（官方页：coupon_callback/channels_ec_coupon_invalid.html）。</summary>
    public const string CouponInvalid = "channels_ec_coupon_invalid";

    /// <summary>用户优惠券过期通知（官方页：coupon_callback/channels_ec_user_coupon_expire.html）。</summary>
    public const string UserCouponExpire = "channels_ec_user_coupon_expire";

    /// <summary>优惠券返还通知（官方页：coupon_callback/channels_ec_user_coupon_unuse.html）。</summary>
    public const string UserCouponUnuse = "channels_ec_user_coupon_unuse";

    /// <summary>优惠券核销通知（官方页：coupon_callback/channels_ec_user_coupon_use.html；参数表 Event 行误写 unuse，以页面标题/头部事件名/示例为准）。</summary>
    public const string UserCouponUse = "channels_ec_user_coupon_use";

    /// <summary>售后单更新通知（官方页：aftersale_callback/channels_ec_aftersale_update.html）。</summary>
    public const string AftersaleUpdate = "channels_ec_aftersale_update";

    /// <summary>纠纷单更新通知（官方页：aftersale_callback/channels_ec_complaint_update.html）。</summary>
    public const string ComplaintUpdate = "channels_ec_complaint_update";

    /// <summary>保障单更新通知（官方页：aftersale_callback/channels_ec_guarantee_update.html）。</summary>
    public const string GuaranteeUpdate = "channels_ec_guarantee_update";

    /// <summary>客服消息事件（官方页：kf_callback/kf_msg_event.html）。</summary>
    public const string KfMsgEvent = "commkf_send_msg_to_kf";

    /// <summary>物流轨迹推送通知（官方页：merchant_callback/ewaybill_push_path.html）。</summary>
    public const string EwaybillPushPath = "ewaybill_push_path";

    /// <summary>品牌资质事件通知（官方页：brand_callback/brand_event.html）。</summary>
    public const string BrandEvent = "channels_ec_brand";

    /// <summary>静默关联时获取小程序会员信息（官方页：wxa_callback/channels_ec_vip_user_info_get.html；应答须为结构化 JSON）。</summary>
    public const string VipUserInfoGet = "channels_ec_vip_user_info_get";

    /// <summary>用户静默关联小店会员通知（官方页：wxa_callback/channels_ec_vip_user_auto_add.html）。</summary>
    public const string VipUserAutoAdd = "channels_ec_vip_user_auto_add";

    /// <summary>用户进入下单页获取初始会员权益（官方页：wxa_callback/channels_ec_gen_order_info_init.html；应答须为结构化 JSON）。</summary>
    public const string GenOrderInfoInit = "channels_ec_gen_order_info_init";

    /// <summary>用户下单使用会员权益（官方页：wxa_callback/channels_ec_order_use_vip_discount.html；应答须为结构化 JSON）。</summary>
    public const string OrderUseVipDiscount = "channels_ec_order_use_vip_discount";

    /// <summary>用户取消使用会员权益（官方页：wxa_callback/channels_ec_order_unuse_vip_discount.html）。</summary>
    public const string OrderUnuseVipDiscount = "channels_ec_order_unuse_vip_discount";

    /// <summary>工单创建通知（官方页：platformkf/ec_platform_kf_create_mch_ticket.html）。</summary>
    public const string PlatformKfCreateMchTicket = "ec_platform_kf_create_mch_ticket";

    /// <summary>工单超时通知（官方页：platformkf/ec_platform_kf_ticket_timeout.html）。</summary>
    public const string PlatformKfTicketTimeout = "ec_platform_kf_ticket_timeout";
}
