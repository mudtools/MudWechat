// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「卡券 · 主体生命周期与投放」域 SDK（11 端点）。
/// </summary>
/// <remarks>
/// <para>
/// <b>对齐基准（重要，勿弱化）</b>：官方文档站（<c>developers.weixin.qq.com</c>）正文为 SPA，
/// 本批实施期（2026-10-10）<b>不可达</b> ⇒ 路由与字段面以本地 SKIT 源码
/// <c>SKIT.FlurlHttpClient.Wechat.Api/Models/Card/*.cs</c> 为对齐依据，官方 URL 只给到
/// 服务端 API 索引页（<see href="https://developers.weixin.qq.com/doc/service/api/"/> → 卡券管理）。
/// <b>逐页深链、频率上限、字段长度与错误码表均标注「待官方逐页核验」</b>，恢复可达后须同步
/// <c>MpCardContractGuards</c>（CD1~CD8）与本 remarks。
/// </para>
/// <para>
/// <b>业务闭环（SDK 只提供端点、不编排）</b>：
/// <see cref="CreateCardAsync"/>（建模板，得 <c>card_id</c>）→ <see cref="SetTestWhiteListAsync"/>
/// （审核期灰度白名单）→ <see cref="CreateCardQrCodeAsync"/> / <see cref="CreateCardLandingPageAsync"/>
/// （投放）→ <see cref="GetCardAsync"/> / <see cref="BatchGetCardsAsync"/>（查询）→
/// <see cref="UpdateCardAsync"/> / <see cref="ModifyCardStockAsync"/>（运营）→ <see cref="DeleteCardAsync"/>；
/// 「支付即会员」<see cref="SetPayCellAsync"/> 与「自助核销」<see cref="SetSelfConsumeCellAsync"/>
/// 是<b>卡面组件开关</b>，与 <c>/card/code/*</c> 核销面（<see cref="IMpCardCodeService"/>）配合使用。
/// </para>
/// <para>
/// <b>域级约束</b>：
/// </para>
/// <list type="bullet">
/// <item><b>11 端点全部为 POST + JSON 请求体</b>（本域无 GET 端点，路由前缀统一 <c>/card/</c>）。</item>
/// <item><b>建卡方向与修改方向的请求形态不同构</b>：建卡为 <c>card</c> 包装 + <c>card_type</c> 判别，
/// 修改为 <c>card_id</c> + 分支键<b>平级</b>且<b>无</b> <c>advanced_info</c>（高级信息建卡后不可改）。
/// 二者分别由 <see cref="MpCardCreateRequest"/> 与
/// <see cref="MpCardUpdateRequest"/> 承载，<b>不得互相替换</b>（守卫 CD2/CD4）。</item>
/// <item><b>库存只增不减地走独立端点</b>：<c>sku.quantity</c> 仅建卡时给定，后续调整一律经
/// <see cref="ModifyCardStockAsync"/>（官方键名 <c>increase_stock_value</c> / <c>reduce_stock_value</c>，
/// <b>不是</b> <c>*_quantity</c>）。</item>
/// <item><b>票据体系</b>：卡券<b>前端</b>取卡（JS SDK / 小程序）所需的 <c>api_ticket</c>
/// 已由 Abstractions 的 <c>IMpTicketService</c>（<c>/cgi-bin/ticket/getticket</c>，<c>type = card</c>）承载，
/// <b>本域 11 端点自身只消费应用级 <c>access_token</c></b> ⇒ 不引入第二套票据面。</item>
/// <item><b>支付耦合</b>：<c>sub_merchant_info.merchant_id</c> 与「支付即会员」组件均<b>只透传</b>
/// 微信支付商户号，本域不调用支付接口、不建模支付错误码。</item>
/// <item><b>账号门槛与频率上限</b>：<b>待官方逐页核验</b>（SDK 不做账号类型本地闸，越界由官方 errcode 表达）。</item>
/// </list>
/// <para>
/// <b>本域端点取舍（守卫 CD8 留档）</b>：SKIT <c>Models/Card</c> 全族为 53 端点，本批只落地
/// 「主体生命周期 + 投放 + 组件开关」11 条 + 核销面 3 条（见 <see cref="IMpCardCodeService"/>）。
/// 未建模的 10 族共 39 条（券码运维 5、会员卡 6 + 用户卡包 1、<c>giftcard</c> 11、<c>paygiftcard</c> 4、
/// <c>generalcard/updateuser</c> 1、特殊票券 3、子商户 4、门店小程序 2、卡券图文 1、协议查询 1（GET））
/// 各自都需要独立的票据 / 支付 / 资质核验，<b>刻意不在本批塞进同一注册组</b>；
/// 新增时须同批扩展本接口或新建接口并更新 CD1/CD8 与 <c>MpRouteCountGuard</c>。
/// </para>
/// <para>
/// <b>令牌路由</b>：全部端点消费 <see cref="MpTokenTypes.AccessToken"/> 并以 Query 注入
/// <c>access_token</c>（官方契约；MUD005 已知接受风险，白名单由 <c>MpQueryTokenWhitelistGuard</c> QT1 锁定）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Card", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpCardService
{
    /// <summary>
    /// 创建卡券。官方文档：<see href="https://developers.weixin.qq.com/doc/offiaccount/Cards_and_Offer/Create_a_Coupon_Voucher_or_Card.html"/>
    /// （补齐方案 §1.3 所引页面；正文<b>待官方逐页核验</b>）。
    /// </summary>
    /// <param name="request">建卡请求（<c>card.card_type</c> + 11 个券型分支之一）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新建卡券的模板编号（<c>card_id</c>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + 请求体 <c>{"card": { "card_type": …, "groupon": { … } }}</c>。</para>
    /// <para>
    /// <b>分支互斥</b>：一次只传与 <c>card_type</c> 对应的那一支；<c>gift</c> 是「兑换券」、
    /// <c>general_card</c> 才是「礼品卡」（易错点，照官方键名，见 <see cref="MpCardCreateBody"/>）。
    /// </para>
    /// <para><b>建卡后不可改的三组</b>：券码类型族（<c>code_type</c> / <c>use_custom_code</c> /
    /// <c>get_custom_code_mode</c> / <c>bind_openid</c>）、库存（走 <see cref="ModifyCardStockAsync"/>）、
    /// 子商户归属（<c>sub_merchant_info</c>）⇒ SDK 在修改方向不提供这些字段。</para>
    /// <para>频率限制与账号门槛<b>待官方逐页核验</b>；MUD005：令牌强制走 Query 参数 <c>access_token</c>。</para>
    /// </remarks>
    [Post("/card/create")]
    Task<MpCardCreateResponse> CreateCardAsync(
        [Body] MpCardCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询卡券详情。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/"/> → 卡券管理（深链<b>待补</b>）。
    /// </summary>
    /// <param name="request">查询请求（仅 <c>card_id</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>card</c> 对象（含 <c>card_type</c> 与对应券型分支，状态在 <c>base_info.status</c>）。</returns>
    /// <remarks>
    /// <para>
    /// <b>应答分支集与建卡不同</b>：查询方向<b>无</b> <c>general_card</c>（礼品卡详情由
    /// <c>/card/giftcard/*</c> 族承载，本域未建模）⇒ 见 <see cref="MpCardQueryDetail"/>。
    /// </para>
    /// <para><b>卡券状态</b>为字符串（官方枚举表<b>待逐页核验</b>，SDK 不建常量、不做状态机校验）。</para>
    /// </remarks>
    [Post("/card/get")]
    Task<MpCardGetResponse> GetCardAsync(
        [Body] MpCardGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取卡券列表。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/"/> → 卡券管理（深链<b>待补</b>）。
    /// </summary>
    /// <param name="request">分页请求（<c>offset</c> + <c>count</c>，可选 <c>status_list</c> 状态筛选）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>本页模板编号列表（<c>card_id_list</c>）与总数（<c>total_num</c>）。</returns>
    /// <remarks>
    /// <para>
    /// <b>分页形态</b>：<c>offset</c> / <c>count</c>，<b>无游标、无 page_info</b>（与素材域 <c>batchget</c> 不同）；
    /// <b>应答键名为 <c>card_id_list</c> / <c>total_num</c></b>，不是 <c>card_list</c> / <c>total_count</c>。
    /// </para>
    /// <para>本接口只返回编号列表 ⇒ 逐张详情须再走 <see cref="GetCardAsync"/>（SDK 不编排 N+1 调用）。</para>
    /// </remarks>
    [Post("/card/batchget")]
    Task<MpCardBatchGetResponse> BatchGetCardsAsync(
        [Body] MpCardBatchGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改卡券。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/"/> → 卡券管理（深链<b>待补</b>）。
    /// </summary>
    /// <param name="request">修改请求（<c>card_id</c> + 券型分支<b>平级</b>，不传即不改）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否需要提交审核（<c>send_check</c>）。</returns>
    /// <remarks>
    /// <para>
    /// <b>请求形态与建卡不同构</b>（对齐基准 SKIT，<b>待官方逐页核验</b>）：<c>card_id</c> 与分支键同层，
    /// <b>无</b> <c>card</c> 包装、<b>无</b> <c>card_type</c>、各分支<b>无</b> <c>advanced_info</c>。
    /// </para>
    /// <para>
    /// <b>应答不回传 <c>card_id</c></b>；<c>send_check</c> 为真表示该次修改进入官方审核流程
    /// （卡券修改后可能需重新审核，故与 <see cref="SetTestWhiteListAsync"/> 的灰度语义相关）。
    /// </para>
    /// </remarks>
    [Post("/card/update")]
    Task<MpCardUpdateResponse> UpdateCardAsync(
        [Body] MpCardUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改卡券库存。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/"/> → 卡券管理（深链<b>待补</b>）。
    /// </summary>
    /// <param name="request">库存增减请求（<c>increase_stock_value</c> / <c>reduce_stock_value</c> 均为增量语义）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方 <c>errcode</c> / <c>errmsg</c>（本端点无业务字段应答）。</returns>
    /// <remarks>
    /// <para>
    /// <b>键名钉死</b>：<c>increase_stock_value</c> / <c>reduce_stock_value</c>（守卫 CD6）——
    /// 不是 <c>increase_quantity</c>，也不是「目标总量」语义。两字段可同传，SDK <b>不做互斥校验</b>。
    /// </para>
    /// <para>库存下限（是否允许减至 0）与单批上限<b>待官方逐页核验</b>，越界由官方错误码表达。</para>
    /// </remarks>
    [Post("/card/modifystock")]
    Task<MpResponse> ModifyCardStockAsync(
        [Body] MpCardModifyStockRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除卡券。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/"/> → 卡券管理（深链<b>待补</b>）。
    /// </summary>
    /// <param name="request">删除请求（仅 <c>card_id</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>删除语义待核</b>：官方对「已发放的卡券能否删除」给出的是错误码面而非前置状态表，
    /// SDK 不做本地状态校验，删除失败由官方 errcode 表达。
    /// </para>
    /// </remarks>
    [Post("/card/delete")]
    Task<MpResponse> DeleteCardAsync(
        [Body] MpCardDeleteRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建卡券 / 礼品码二维码。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/"/> → 卡券管理（深链<b>待补</b>）。
    /// </summary>
    /// <param name="request">二维码创建请求（<c>action_name</c> + <c>action_info.card</c> 或 <c>action_info.multiple_card</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>票据（<c>ticket</c>）/ 跳转链接（<c>url</c>）/ 可直接展示的图片地址（<c>show_qrcode_url</c>）/ 有效秒数。</returns>
    /// <remarks>
    /// <para>
    /// <b>与带参二维码域的分工</b>：本端点 URI 为 <c>/card/qrcode/create</c>，
    /// <see cref="IMpQrcodeService"/> 为 <c>/cgi-bin/qrcode/create</c> —— 两个不同官方契约，
    /// 前者投放<b>卡券领取</b>、后者投放<b>公众号场景值</b>；路由不重叠（<c>MpRouteCountGuard</c> RC3）。
    /// </para>
    /// <para><b>单卡与多卡互斥</b>由 <c>action_name</c> 判别（取值表<b>待官方逐页核验</b>）。</para>
    /// <para><b>无图片通道</b>：仅返回 <c>show_qrcode_url</c> 字符串，取图由宿主自行下载。</para>
    /// </remarks>
    [Post("/card/qrcode/create")]
    Task<MpCardQrcodeCreateResponse> CreateCardQrCodeAsync(
        [Body] MpCardQrcodeCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 生成卡券落地页。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/"/> → 卡券管理（深链<b>待补</b>）。
    /// </summary>
    /// <param name="request">落地页请求（<c>banner</c> / <c>page_title</c> / <c>can_share</c> / <c>scene</c> / <c>card_list</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>落地页编号（<c>page_id</c>）与链接（<c>url</c>）。</returns>
    /// <remarks>
    /// <para><b>顶层平级形态</b>（对齐 SKIT）：<c>card_list[]</c> 项仅 <c>card_id</c> + <c>thumb_url</c>。</para>
    /// <para><c>scene</c> 取值表与页面生命周期（是否可覆盖重建）<b>待官方逐页核验</b>。</para>
    /// </remarks>
    [Post("/card/landingpage/create")]
    Task<MpCardLandingPageCreateResponse> CreateCardLandingPageAsync(
        [Body] MpCardLandingPageCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 开通或关闭「支付后开卡」组件。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/"/> → 卡券管理（深链<b>待补</b>）。
    /// </summary>
    /// <param name="request">开关请求（<c>card_id</c> + <c>is_open</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>与支付体系的关系</b>：本开关只作用在<b>卡券模板</b>上（微信支付完成后引导领卡），
    /// 本域<b>不调用</b>支付接口 ⇒ 支付侧前置条件（商户号、支付即会员签约）由宿主保障。
    /// </para>
    /// </remarks>
    [Post("/card/paycell/set")]
    Task<MpResponse> SetPayCellAsync(
        [Body] MpCardPayCellSetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 开通或关闭「自助核销」组件。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/"/> → 卡券管理（深链<b>待补</b>）。
    /// </summary>
    /// <param name="request">开关请求（<c>card_id</c> + <c>is_open</c> + 两个可选校验开关）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>高风险字段拼写</b>：官方（SKIT 对齐基准）键名为 <c>need_verify_cod</c>（<b>缺尾字母 <c>e</c></b>），
    /// SDK <b>照录而不修正</b>；该拼写<b>待官方逐页核验</b>，若官方实为 <c>need_verify_code</c>
    /// 则须同批改 <see cref="MpCardSelfConsumeCellSetRequest"/> 与守卫 CD6。
    /// </para>
    /// </remarks>
    [Post("/card/selfconsumecell/set")]
    Task<MpResponse> SetSelfConsumeCellAsync(
        [Body] MpCardSelfConsumeCellSetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置卡券测试白名单。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/"/> → 卡券管理（深链<b>待补</b>）。
    /// </summary>
    /// <param name="request">白名单请求（<c>openid</c> 或 <c>username</c>，二选一）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>
    /// <b>用途</b>：卡券模板在未通过官方审核期间仅对白名单可见 ⇒ 本端点是联调前置步骤，
    /// 与 <see cref="UpdateCardAsync"/> 的 <c>send_check</c>（送审）语义联动。
    /// </para>
    /// <para><b>覆盖还是追加待核</b>：官方是否「整体替换白名单」本次不可达，SDK 不编造语义，
    /// 核验前请按「可能是覆盖」对待（若为覆盖，误传空表即清空线上灰度名单）。</para>
    /// </remarks>
    [Post("/card/testwhitelist/set")]
    Task<MpResponse> SetTestWhiteListAsync(
        [Body] MpCardTestWhiteListSetRequest request,
        CancellationToken cancellationToken = default);
}
