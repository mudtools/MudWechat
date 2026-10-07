// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.HttpUtils.Attributes;

namespace Mud.Wechat.OfficialAccount.Abstractions.Callback.Payloads;

/// <summary>
/// 卡券审核事件载荷（<c>card_pass_check</c> / <c>card_not_pass_check</c>；官方卡券事件推送页，V2 已核验）。
/// </summary>
/// <remarks>两键共用同一结构；<c>RefuseReason</c> 仅审核不通过时携带。</remarks>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[]
{
    MpCardEventTypes.CardPassCheck,
    MpCardEventTypes.CardNotPassCheck,
})]
public sealed partial class MpCardCheckEventPayload : MpCallbackPayload
{
    /// <summary>卡券 ID（官方 <c>CardId</c>）。</summary>
    [PayloadField("CardId")]
    public string? CardId { get; set; }

    /// <summary>审核不通过原因（官方 <c>RefuseReason</c>；通过时为空）。</summary>
    [PayloadField("RefuseReason")]
    public string? RefuseReason { get; set; }
}

/// <summary>
/// 卡券领取事件载荷（<c>user_get_card</c>；V2 已核验，卡券族字段最多的一个）。
/// </summary>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[] { MpCardEventTypes.UserGetCard })]
public sealed partial class MpCardGetEventPayload : MpCallbackPayload
{
    /// <summary>卡券 ID（官方 <c>CardId</c>）。</summary>
    [PayloadField("CardId")]
    public string? CardId { get; set; }

    /// <summary>是否转赠领取（<c>IsGiveByFriend</c>：1 是 / 0 否）。</summary>
    [PayloadField("IsGiveByFriend")]
    public long? IsGiveByFriend { get; set; }

    /// <summary>code 序列号（官方 <c>UserCardCode</c>）。</summary>
    [PayloadField("UserCardCode")]
    public string? UserCardCode { get; set; }

    /// <summary>发起转赠用户的 openid（仅 <c>IsGiveByFriend = 1</c> 时填入）。</summary>
    [PayloadField("FriendUserName")]
    public string? FriendUserName { get; set; }

    /// <summary>开发者自定义的卡券外层编号（官方 <c>OuterId</c>）。</summary>
    [PayloadField("OuterId")]
    public string? OuterId { get; set; }

    /// <summary>转赠前的 code（官方 <c>OldUserCardCode</c>；转赠会变更 code）。</summary>
    [PayloadField("OldUserCardCode")]
    public string? OldUserCardCode { get; set; }

    /// <summary>领取场景值（官方 <c>OuterStr</c>；用于领取渠道统计，可在生成二维码/Addcard 接口自定义）。</summary>
    [PayloadField("OuterStr")]
    public string? OuterStr { get; set; }

    /// <summary>是否「删除会员卡后重新找回」（<c>IsRestoreMemberCard</c>：1 是 / 0 否）。</summary>
    [PayloadField("IsRestoreMemberCard")]
    public long? IsRestoreMemberCard { get; set; }

    /// <summary>是否通过好友推荐领取（官方 <c>IsRecommendByFriend</c>）。</summary>
    [PayloadField("IsRecommendByFriend")]
    public long? IsRecommendByFriend { get; set; }

    /// <summary>领券用户的 UnionId（官方 <c>UnionId</c>，注意大小写与授权族的 <c>UnionID</c> 不同）。</summary>
    [PayloadField("UnionId")]
    public string? UnionId { get; set; }
}

/// <summary>
/// 卡券转赠事件载荷（<c>user_gifting_card</c>；V2 已核验）。
/// </summary>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[] { MpCardEventTypes.UserGiftingCard })]
public sealed partial class MpCardGiftingEventPayload : MpCallbackPayload
{
    /// <summary>卡券 ID。</summary>
    [PayloadField("CardId")]
    public string? CardId { get; set; }

    /// <summary>code 序列号。</summary>
    [PayloadField("UserCardCode")]
    public string? UserCardCode { get; set; }

    /// <summary>是否转赠退回（<c>IsReturnBack</c>：0 否 / 1 是）。</summary>
    [PayloadField("IsReturnBack")]
    public long? IsReturnBack { get; set; }

    /// <summary>接收卡券用户的 openid（官方 <c>FriendUserName</c>）。</summary>
    [PayloadField("FriendUserName")]
    public string? FriendUserName { get; set; }

    /// <summary>是否群转赠（官方 <c>IsChatRoom</c>）。</summary>
    [PayloadField("IsChatRoom")]
    public long? IsChatRoom { get; set; }
}

/// <summary>
/// 卡券核销事件载荷（<c>user_consume_card</c>；V2 已核验）。
/// </summary>
/// <remarks>
/// <c>ConsumeSource</c> 取值：<c>FROM_API</c>（开发者 API 核销）/ <c>FROM_MP</c>（公众平台核销）/
/// <c>FROM_MOBILE_HELPER</c>（卡券商户助手核销）；<c>LocationName</c> 仅自助核销与买单核销时出现，
/// <c>StaffOpenId</c> 仅卡券商户助手核销时出现，<c>VerifyCode</c>/<c>RemarkAmount</c> 仅自助核销时出现。
/// </remarks>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[] { MpCardEventTypes.UserConsumeCard })]
public sealed partial class MpCardConsumeEventPayload : MpCallbackPayload
{
    /// <summary>卡券 ID。</summary>
    [PayloadField("CardId")]
    public string? CardId { get; set; }

    /// <summary>code 序列号。</summary>
    [PayloadField("UserCardCode")]
    public string? UserCardCode { get; set; }

    /// <summary>核销来源（官方 <c>ConsumeSource</c>：<c>FROM_API</c> / <c>FROM_MP</c> / <c>FROM_MOBILE_HELPER</c>）。</summary>
    [PayloadField("ConsumeSource")]
    public string? ConsumeSource { get; set; }

    /// <summary>门店名称（仅自助核销与买单核销携带）。</summary>
    [PayloadField("LocationName")]
    public string? LocationName { get; set; }

    /// <summary>核销员 openid（仅通过卡券商户助手核销时携带）。</summary>
    [PayloadField("StaffOpenId")]
    public string? StaffOpenId { get; set; }

    /// <summary>自助核销时用户输入的验证码。</summary>
    [PayloadField("VerifyCode")]
    public string? VerifyCode { get; set; }

    /// <summary>自助核销时用户输入的备注金额。</summary>
    [PayloadField("RemarkAmount")]
    public string? RemarkAmount { get; set; }

    /// <summary>开发者发起核销时传入的自定义参数（核销渠道统计）。</summary>
    [PayloadField("OuterStr")]
    public string? OuterStr { get; set; }
}

/// <summary>
/// 微信买单事件载荷（<c>user_pay_from_pay_cell</c>；V2 已核验）。
/// </summary>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[] { MpCardEventTypes.UserPayFromPayCell })]
public sealed partial class MpCardPayCellEventPayload : MpCallbackPayload
{
    /// <summary>卡券 ID。</summary>
    [PayloadField("CardId")]
    public string? CardId { get; set; }

    /// <summary>code 序列号。</summary>
    [PayloadField("UserCardCode")]
    public string? UserCardCode { get; set; }

    /// <summary>微信支付交易订单号（仅使用买单功能核销的卡券携带）。</summary>
    [PayloadField("TransId")]
    public string? TransId { get; set; }

    /// <summary>门店 ID（仅卡券商户助手与买单核销时携带）。</summary>
    [PayloadField("LocationId")]
    public long? LocationId { get; set; }

    /// <summary>实付金额（单位：分）。</summary>
    [PayloadField("Fee")]
    public long? Fee { get; set; }

    /// <summary>应付金额（单位：分）。</summary>
    [PayloadField("OriginalFee")]
    public long? OriginalFee { get; set; }
}

/// <summary>
/// 会员卡内容更新事件载荷（<c>update_member_card</c>；V2 已核验）。
/// </summary>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[] { MpCardEventTypes.UpdateMemberCard })]
public sealed partial class MpCardMemberUpdateEventPayload : MpCallbackPayload
{
    /// <summary>卡券 ID。</summary>
    [PayloadField("CardId")]
    public string? CardId { get; set; }

    /// <summary>code 序列号。</summary>
    [PayloadField("UserCardCode")]
    public string? UserCardCode { get; set; }

    /// <summary>变动的积分值（官方 <c>ModifyBonus</c>）。</summary>
    [PayloadField("ModifyBonus")]
    public long? ModifyBonus { get; set; }

    /// <summary>变动的余额值（官方 <c>ModifyBalance</c>）。</summary>
    [PayloadField("ModifyBalance")]
    public long? ModifyBalance { get; set; }
}

/// <summary>
/// 券点流水详情事件载荷（<c>card_pay_order</c>；V2 已核验）。
/// </summary>
/// <remarks>
/// <b>注意</b>：本事件**不携带 <c>CardId</c>**（官方报文仅有订单与券点字段），故与卡券族其它载荷不同构。
/// <c>Status</c>/<c>OrderType</c> 为官方闭合值域（见属性注释）。
/// </remarks>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[] { MpCardEventTypes.CardPayOrder })]
public sealed partial class MpCardPayOrderEventPayload : MpCallbackPayload
{
    /// <summary>本次推送对应的订单号（官方 <c>OrderId</c>）。</summary>
    [PayloadField("OrderId")]
    public string? OrderId { get; set; }

    /// <summary>
    /// 订单状态（官方 <c>Status</c>）：<c>ORDER_STATUS_WAITING</c> 等待支付 / <c>ORDER_STATUS_SUCC</c> 支付成功 /
    /// <c>ORDER_STATUS_FINANCE_SUCC</c> 加代币成功 / <c>ORDER_STATUS_QUANTITY_SUCC</c> 加库存成功 /
    /// <c>ORDER_STATUS_HAS_REFUND</c> 已退币 / <c>ORDER_STATUS_REFUND_WAITING</c> 等待退币确认 /
    /// <c>ORDER_STATUS_ROLLBACK</c> 已回退 / <c>ORDER_STATUS_HAS_RECEIPT</c> 已开发票。
    /// </summary>
    [PayloadField("Status")]
    public string? Status { get; set; }

    /// <summary>支付二维码生成时间（官方 <c>CreateOrderTime</c>）。</summary>
    [PayloadField("CreateOrderTime")]
    public long? CreateOrderTime { get; set; }

    /// <summary>实际支付成功时间（官方 <c>PayFinishTime</c>）。</summary>
    [PayloadField("PayFinishTime")]
    public long? PayFinishTime { get; set; }

    /// <summary>支付方式（一般为微信支付充值）。</summary>
    [PayloadField("Desc")]
    public string? Desc { get; set; }

    /// <summary>剩余免费券点。</summary>
    [PayloadField("FreeCoinCount")]
    public long? FreeCoinCount { get; set; }

    /// <summary>剩余付费券点。</summary>
    [PayloadField("PayCoinCount")]
    public long? PayCoinCount { get; set; }

    /// <summary>本次变动免费券点。</summary>
    [PayloadField("RefundFreeCoinCount")]
    public long? RefundFreeCoinCount { get; set; }

    /// <summary>本次变动付费券点。</summary>
    [PayloadField("RefundPayCoinCount")]
    public long? RefundPayCoinCount { get; set; }

    /// <summary>
    /// 订单类型（官方 <c>OrderType</c>）：<c>ORDER_TYPE_SYS_ADD</c> 平台赠送 /
    /// <c>ORDER_TYPE_WXPAY</c> 充值 / <c>ORDER_TYPE_REFUND</c> 库存未使用回退 /
    /// <c>ORDER_TYPE_REDUCE</c> 兑换库存 / <c>ORDER_TYPE_SYS_REDUCE</c> 平台扣减。
    /// </summary>
    [PayloadField("OrderType")]
    public string? OrderType { get; set; }

    /// <summary>系统备注（官方 <c>Memo</c>）。</summary>
    [PayloadField("Memo")]
    public string? Memo { get; set; }

    /// <summary>所开发票详情（官方 <c>ReceiptInfo</c>）。</summary>
    [PayloadField("ReceiptInfo")]
    public string? ReceiptInfo { get; set; }
}

/// <summary>
/// 卡券简式事件载荷（<c>user_del_card</c> / <c>user_enter_session_from_card</c> /
/// <c>submit_membercard_user_info</c>；V2 已核验，三键字段集一致 = <c>CardId</c> + <c>UserCardCode</c>）。
/// </summary>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[]
{
    MpCardEventTypes.UserDelCard,
    MpCardEventTypes.UserEnterSessionFromCard,
    MpCardEventTypes.SubmitMemberCardUserInfo,
})]
public sealed partial class MpCardSimpleEventPayload : MpCallbackPayload
{
    /// <summary>卡券 ID。</summary>
    [PayloadField("CardId")]
    public string? CardId { get; set; }

    /// <summary>code 序列号（官方 <c>UserCardCode</c>；自定义 code 场景下可能为空串）。</summary>
    [PayloadField("UserCardCode")]
    public string? UserCardCode { get; set; }
}

/// <summary>
/// 进入会员卡事件载荷（<c>user_view_card</c>；V2 已核验）。
/// </summary>
/// <remarks>仅当创建会员卡时设置 <c>need_push_on_view = true</c> 才会推送（开发者须自行评估服务器压力）。</remarks>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[] { MpCardEventTypes.UserViewCard })]
public sealed partial class MpCardViewEventPayload : MpCallbackPayload
{
    /// <summary>卡券 ID。</summary>
    [PayloadField("CardId")]
    public string? CardId { get; set; }

    /// <summary>商户自定义 code 值（非自定义 code 场景为空串）。</summary>
    [PayloadField("UserCardCode")]
    public string? UserCardCode { get; set; }

    /// <summary>商户自定义二维码渠道参数（标识本次进入会员卡的来源渠道）。</summary>
    [PayloadField("OuterStr")]
    public string? OuterStr { get; set; }
}

/// <summary>
/// 库存报警事件载荷（<c>card_sku_remind</c>；V2 已核验）。
/// </summary>
/// <remarks>
/// 触发条件：某 <c>card_id</c> 初始库存 &gt; 200 且当前库存 ≤ 100，用户尝试领券时触发，**每 12 小时**推送一次；
/// 该事件的 <c>FromUserName</c> 发送方为「微信」。
/// </remarks>
[PayloadContract(Converter = typeof(MpPayloadConverter))]
[MpCallbackContract(EventTypes = new[] { MpCardEventTypes.CardSkuRemind })]
public sealed partial class MpCardSkuRemindEventPayload : MpCallbackPayload
{
    /// <summary>卡券 ID。</summary>
    [PayloadField("CardId")]
    public string? CardId { get; set; }

    /// <summary>报警详细信息（官方 <c>Detail</c>）。</summary>
    [PayloadField("Detail")]
    public string? Detail { get; set; }
}
