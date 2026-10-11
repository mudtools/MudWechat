// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号「会员卡」域接口（B2c 落地：用户信息 / 激活表单 / 激活链路 6 端点 + 用户卡包 1 端点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域内双接口先例（照 OpenApi/Card 形态）</b>：卡券族按功能面拆分接口但共用注册组 <c>Card</c>；
/// 激活链路三支（activate / geturl / activatetempinfo）为「小程序快速注册」闭环，须成对使用。
/// </para>
/// <para>MUD005 已知接受风险：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Card", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IMpCardMemberService
{
    /// <summary>查询会员卡用户信息。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/Membership_Cards/Manage_Member_Card.html#1"/>（官方接口英文名 <c>memberCardUserInfoGet</c>）。</summary>
    /// <param name="request">查询请求（card_id + code 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>用户资料 / 积分 / 余额 / 卡状态。</returns>
    /// <remarks><para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/membercard/userinfo/get")]
    Task<MpCardMemberUserInfoGetResponse> GetMemberCardUserInfoAsync(
        [Body] MpCardMemberUserInfoGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>设置会员卡激活表单。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/Membership_Cards/Create_a_membership_card.html#16"/>（官方接口英文名 <c>activateUserFormSet</c>）。</summary>
    /// <param name="request">表单请求（card_id 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks><para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/membercard/activateuserform/set")]
    Task<MpResponse> SetMemberCardActivateUserFormAsync(
        [Body] MpCardMemberActivateUserFormSetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>更新会员卡用户信息。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/Membership_Cards/Create_a_membership_card.html#18"/>（官方接口英文名 <c>updateUser</c>）。</summary>
    /// <param name="request">更新请求（card_id + code 必填；<c>bonus</c>/<c>add_bonus</c> 等覆盖与增量语义见 DTO）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>变动后积分 / 余额。</returns>
    /// <remarks><para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/membercard/updateuser")]
    Task<MpCardMemberUpdateUserResponse> UpdateMemberCardUserAsync(
        [Body] MpCardMemberUpdateUserRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>接口激活会员卡。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/Coupons-Mini_Program_Start_Up.html"/>（官方接口英文名 <c>activate</c>）。</summary>
    /// <param name="request">激活请求（code + membership_number 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks><para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/membercard/activate")]
    Task<MpResponse> ActivateMemberCardAsync(
        [Body] MpCardMemberActivateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>获取会员卡激活链接。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/Coupons-Mini_Program_Start_Up.html"/>（官方接口英文名 <c>activateGetUrl</c>）。</summary>
    /// <param name="request">取链接请求（card_id 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>激活跳转链接。</returns>
    /// <remarks><para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/membercard/activate/geturl")]
    Task<MpCardMemberActivateGetUrlResponse> GetMemberCardActivateUrlAsync(
        [Body] MpCardMemberActivateGetUrlRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>获取用户填写的激活信息。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/Coupons-Mini_Program_Start_Up.html"/>（官方接口英文名 <c>activateTempInfoGet</c>）。</summary>
    /// <param name="request">查询请求（activate_ticket 来自事件推送）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>用户填写的表单信息。</returns>
    /// <remarks><para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/membercard/activatetempinfo/get")]
    Task<MpCardMemberActivateTempInfoGetResponse> GetMemberCardActivateTempInfoAsync(
        [Body] MpCardMemberActivateTempInfoGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>获取用户卡包。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/Managing_Coupons_Vouchers_and_Cards.html#1"/>（官方接口英文名 <c>getCardList</c>；用户卡包族归本接口承载）。</summary>
    /// <param name="request">查询请求（openid 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>用户持卡列表。</returns>
    /// <remarks><para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/user/getcardlist")]
    Task<MpCardUserGetCardListResponse> GetUserCardListAsync(
        [Body] MpCardUserGetCardListRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 微信公众号「礼品卡 / 付款礼品卡」域接口（B2c 落地：货架 / 订单 / 支付耦合 11 + 规则 4 端点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>业务准入</b>：礼品卡需开通微信支付礼品卡能力（<c>pay/submch/bind</c> 与白名单面）；订单退款有 <b>7 天有效期</b>；
/// 付款礼品卡规则需商户号与支付资质。两种形态（<c>giftcard</c> / <c>paygiftcard</c>）为官方两族，勿混用。
/// </para>
/// <para>MUD005 已知接受风险：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Card", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IMpCardGiftService
{
    /// <summary>设置礼品卡货架维护。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/gift_card.html"/>（官方接口英文名 <c>maintainSet</c>）。</summary>
    /// <param name="request">维护请求（maintain 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>维护状态控制信息。</returns>
    /// <remarks><para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/giftcard/maintain/set")]
    Task<MpCardGiftMaintainSetResponse> SetGiftCardMaintainAsync(
        [Body] MpCardGiftMaintainSetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>拉取礼品卡订单列表。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/gift_card.html"/>（官方接口英文名 <c>orderBatchGet</c>）。</summary>
    /// <param name="request">分页请求（offset/count 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单列表与总数。</returns>
    /// <remarks><para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/giftcard/order/batchget")]
    Task<MpCardGiftOrderBatchGetResponse> BatchGetGiftCardOrdersAsync(
        [Body] MpCardGiftOrderBatchGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>查询礼品卡订单。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/gift_card.html"/>（官方接口英文名 <c>orderGet</c>）。</summary>
    /// <param name="request">查询请求（order_id 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订单详情。</returns>
    /// <remarks><para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/giftcard/order/get")]
    Task<MpCardGiftOrderGetResponse> GetGiftCardOrderAsync(
        [Body] MpCardGiftOrderGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>礼品卡订单退款。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/gift_card.html"/>（官方接口英文名 <c>orderRefund</c>）。</summary>
    /// <param name="request">退款请求（order_id 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks><para><b>7 天有效期（官方约束）</b>：超期订单不可退。MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/giftcard/order/refund")]
    Task<MpResponse> RefundGiftCardOrderAsync(
        [Body] MpCardGiftOrderRefundRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>创建礼品卡货架。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/gift_card.html"/>（官方接口英文名 <c>pageAdd</c>）。</summary>
    /// <param name="request">创建请求（page 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>货架 ID。</returns>
    /// <remarks><para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/giftcard/page/add")]
    Task<MpCardGiftPageAddResponse> AddGiftCardPageAsync(
        [Body] MpCardGiftPageAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>拉取礼品卡货架列表。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/gift_card.html"/>（官方接口英文名 <c>pageBatchGet</c>）。</summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>货架 ID 列表。</returns>
    /// <remarks><para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/giftcard/page/batchget")]
    Task<MpCardGiftPageBatchGetResponse> BatchGetGiftCardPagesAsync(
        [Body] MpCardGiftPageBatchGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>查询礼品卡货架。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/gift_card.html"/>（官方接口英文名 <c>pageGet</c>）。</summary>
    /// <param name="request">查询请求（page_id 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>货架详情。</returns>
    /// <remarks><para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/giftcard/page/get")]
    Task<MpCardGiftPageGetResponse> GetGiftCardPageAsync(
        [Body] MpCardGiftPageGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>更新礼品卡货架。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/gift_card.html"/>（官方接口英文名 <c>pageUpdate</c>）。</summary>
    /// <param name="request">更新请求（page 须携带 page_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks><para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/giftcard/page/update")]
    Task<MpResponse> UpdateGiftCardPageAsync(
        [Body] MpCardGiftPageUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>绑定子商户。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/gift_card.html"/>（官方接口英文名 <c>paySubMchBind</c>；礼品卡支付耦合面）。</summary>
    /// <param name="request">绑定请求（sub_mch_id + wxa_appid 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks><para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/giftcard/pay/submch/bind")]
    Task<MpResponse> BindGiftCardPaySubMerchantAsync(
        [Body] MpCardGiftPaySubMerchantBindRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>添加白名单。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/gift_card.html"/>（官方接口英文名 <c>payWhitelistAdd</c>）。</summary>
    /// <param name="request">白名单请求（sub_mch_id 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>白名单页面地址。</returns>
    /// <remarks><para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/giftcard/pay/whitelist/add")]
    Task<MpCardGiftPayWhitelistAddResponse> AddGiftCardPayWhitelistAsync(
        [Body] MpCardGiftPayWhitelistAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>设置礼品卡小程序跳转。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/gift_card.html"/>（官方接口英文名 <c>wxaSet</c>）。</summary>
    /// <param name="request">设置请求（wxa_appid + page_id 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks><para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/giftcard/wxa/set")]
    Task<MpResponse> SetGiftCardWxaAsync(
        [Body] MpCardGiftWxaSetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>创建付款礼品卡规则。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/Membership_Cards/Manage_Member_Card.html#4"/>（官方接口英文名 <c>payGiftCardAdd</c>）。</summary>
    /// <param name="request">创建请求（rule_info 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>规则 ID 与成败商户列表。</returns>
    /// <remarks><para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/paygiftcard/add")]
    Task<MpCardPayGiftAddResponse> AddPayGiftCardRuleAsync(
        [Body] MpCardPayGiftAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>批量查询付款礼品卡规则。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/Membership_Cards/Manage_Member_Card.html#4"/>（官方接口英文名 <c>payGiftCardBatchGet</c>）。</summary>
    /// <param name="request">分页请求（effective/offset/count 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>规则列表与总数。</returns>
    /// <remarks><para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/paygiftcard/batchget")]
    Task<MpCardPayGiftBatchGetResponse> BatchGetPayGiftCardRulesAsync(
        [Body] MpCardPayGiftBatchGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>删除付款礼品卡规则。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/Membership_Cards/Manage_Member_Card.html#4"/>（官方接口英文名 <c>payGiftCardDelete</c>）。</summary>
    /// <param name="request">删除请求（rule_id 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>。</returns>
    /// <remarks><para><b>覆盖删除语义</b>：规则删除即失效。MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/paygiftcard/delete")]
    Task<MpResponse> DeletePayGiftCardRuleAsync(
        [Body] MpCardPayGiftDeleteRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>查询付款礼品卡规则。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/Membership_Cards/Manage_Member_Card.html#4"/>（官方接口英文名 <c>payGiftCardGetById</c>）。</summary>
    /// <param name="request">查询请求（rule_id 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>规则详情（含状态与时间戳）。</returns>
    /// <remarks><para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/paygiftcard/getbyid")]
    Task<MpCardPayGiftGetByIdResponse> GetPayGiftCardRuleAsync(
        [Body] MpCardPayGiftGetByIdRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 微信公众号「卡券子商户」域接口（B2c 落地：进件 / 更新 / 查询 4 端点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>仅第三方平台 / 代开发场景可用</b>（官方子商户模式为第三方代开发者设计；
/// 类目与资质面先经 <see cref="IMpCardService.GetApplyProtocolAsync"/> 查询）。
/// </para>
/// <para>MUD005 已知接受风险：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Card", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IMpCardSubMerchantService
{
    /// <summary>创建子商户。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/Third-party_developer_mode.html#1"/>（官方接口英文名 <c>subMerchantSubmit</c>）。</summary>
    /// <param name="request">提交请求（info 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>子商户信息（含审核状态）。</returns>
    /// <remarks><para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/submerchant/submit")]
    Task<MpCardSubMerchantInfoResponse> SubmitSubMerchantAsync(
        [Body] MpCardSubMerchantSubmitRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>更新子商户。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/Third-party_developer_mode.html#1"/>（官方接口英文名 <c>subMerchantUpdate</c>）。</summary>
    /// <param name="request">更新请求（info 须携带 merchant_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>子商户信息。</returns>
    /// <remarks><para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/submerchant/update")]
    Task<MpCardSubMerchantInfoResponse> UpdateSubMerchantAsync(
        [Body] MpCardSubMerchantUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>查询子商户。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/Third-party_developer_mode.html#5"/>（官方接口英文名 <c>subMerchantGet</c>）。</summary>
    /// <param name="request">查询请求（merchant_id 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>子商户信息。</returns>
    /// <remarks><para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/submerchant/get")]
    Task<MpCardSubMerchantInfoResponse> GetSubMerchantAsync(
        [Body] MpCardSubMerchantGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>批量查询子商户。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/Third-party_developer_mode.html#6"/>（官方接口英文名 <c>subMerchantBatchGet</c>）。</summary>
    /// <param name="request">分页请求（limit ≤ 50）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>子商户列表与下一页起始 ID。</returns>
    /// <remarks><para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约）。</para></remarks>
    [Post("/card/submerchant/batchget")]
    Task<MpCardSubMerchantBatchGetResponse> BatchGetSubMerchantsAsync(
        [Body] MpCardSubMerchantBatchGetRequest request,
        CancellationToken cancellationToken = default);
}
