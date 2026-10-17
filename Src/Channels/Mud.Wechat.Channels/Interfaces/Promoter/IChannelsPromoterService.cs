// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.DataModels.Promoter;

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「联盟带货机构推客（promoter）」域 SDK（41 端点：推客账号与分佣 7 + 机构优惠券 8 + 短视频 4 + 直播 13 + 文章 2 + 商品与商品分佣 7）。
/// </summary>
/// <remarks>
/// <para>
/// <b>视角与令牌归属（与 League 域不得合并）</b>：本域全部路由为 <c>/channels/ec/promoter/*</c>，
/// 由「联盟带货机构（headsupplier）」以<b>机构自身 <c>access_token</c></b> 调用，官方适用范围恒为
/// 「本接口支持『联盟带货机构』账号类型调用。其他账号类型如无特殊说明，均不可调用」，且逐页均声明
/// <b>不支持云调用</b> 与 <b>不支持第三方平台调用</b>——与 League 域小店商家侧（<c>/channels/ec/league/*</c>、
/// 第三方权限集 141 可代调用）是两套公开面，跨线代调用一律不可用。
/// </para>
/// <para>
/// <b>「promoter」一词在本仓两处语义不同</b>：本域路径段 <c>promoter</c> 指<b>推客</b>
/// （<c>sharer_appid</c> / <c>sharer_openid</c>，微信电商平台的推广身份），而 League 域
/// <c>/channels/ec/league/promoter/*</c> 的 <c>promoter</c> 指<b>带货者</b>（小店商家侧推广对象）。
/// 路径前缀即视角，字段与错误码各自成表，不得因同名而合并或复用 DTO。
/// </para>
/// <para>
/// <b>同域双视角（带货者侧 vs 小店关联账号侧，成对端点不得合并）</b>：短视频、直播、文章三族各有成对端点——
/// ①<b>带货者（talent）侧</b>（<c>get_feed_list</c> / <c>get_live_*</c> / <c>get_talent_biz_article_list</c>，
/// 定位键 <c>talent_appid</c>）；②<b>小店关联账号（shop）侧</b>（<c>get_shop_*</c>，定位键为
/// <c>shop_appid</c> + <c>promoter_id</c> + <c>promoter_type</c> 三元组，取自
/// <see cref="GetBindShopListAsync"/> 与 <see cref="GetBindShopPromoterListAsync"/>）。
/// 两视角响应子结构字段集不同（shop 侧条目无 <c>talent_appid</c>、直播商品条目多出 <c>commission_rate</c>），
/// 且 shop 侧独有错误码 <c>10071055</c>（关联账号 id 错误）——故 DTO 分建，仅官方参数表逐字段一致的类型共用。
/// </para>
/// <para>
/// <b><c>promoter_type</c> 取值按族收敛</b>：短视频与直播族官方要求「需要是视频号」（<c>1</c>），
/// 文章族要求「需要是公众号或服务号」（<c>4</c> / <c>5</c>），枚举见 <see cref="ChannelsPromoterShopPromoterTypes"/>；
/// 官方 2025-12-25 变更日志仅更新该字段描述、未改值域。
/// <c>promoter_type</c> 在 <c>get_shop_feed_promotion_info</c> 的参数表中标为<b>选填</b>，而同族其余 shop 侧端点均必填，
/// 属官方文档不一致，已在对应方法 remarks 照录。
/// </para>
/// <para>
/// <b>翻页形态三套并存</b>：①多数列表走 <c>next_key</c> + <c>page_size</c>（首屏 <c>next_key</c> 传空，
/// 上限逐端点不同：推客 <c>20</c> / 券 <c>200</c> / 可推广商品 <c>20</c> / 其余 <c>10</c>）；
/// ②<c>get_coupon_to_headsupplier</c> 走 <c>limit</c> + <c>page_context</c> 且响应回 <c>total_count</c>；
/// ③直播预约 / 直播列表族官方<b>无分页参数</b>。另 <see cref="GetBindSharerListAsync"/> 响应
/// <b>不含</b> <c>has_more</c> 而 <see cref="GetCouponDetailAsync"/>（详情端点）反而带 <c>next_key</c> + <c>has_more</c>，
/// 两处均属官方参数表原样，不得「顺手对齐」。
/// </para>
/// <para>
/// <b>参数表与代码示例类型矛盾（本域按官方参数表定类型，矛盾逐条照录于对应方法 remarks 并由守卫锁定）</b>：
/// 例 <c>commission_info.status</c>（表 string / 枚举与示例 number）、<c>category_id</c>（表 number /
/// 示例值超 Int64 范围）、<c>sku_attrs</c>（表 object / 示例 array）、
/// <c>set_sharer_product_commission_info.commission_ratio</c>（表 string / 同义兄弟端点 number）、
/// 列表侧 <c>product_id</c>（string）与详情侧 <c>product_id</c>（number）等。
/// 另：官方代码示例的 <c>errcode</c> 恒写作字符串 <c>"0"</c>，与参数表 <c>number</c> 冲突，本仓统一按参数表取
/// <see cref="ChannelsResponse.ErrorCode"/> 的整型形态。
/// </para>
/// <para>
/// <b>MUD005 已知接受风险</b>：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Promoter", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsPromoterService
{
    /// <summary>
    /// 获取机构绑定的推客信息（<c>promoter/get_bind_sharer_list</c>，推客列表分页查询）。
    /// </summary>
    /// <param name="request">查询请求（<c>next_key</c> 与 <c>page_size</c> 必填；<c>sharer_appid</c> / <c>sharer_openid</c> 选填且二选一）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>推客列表与下一页参数（<c>sharer_info_list</c> / <c>next_key</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/account/api_getbindsharerlist.html"/></para>
    /// <para>
    /// 官方业务限制：①<c>page_size</c> 一页获取多少个推客，最大 <c>20</c>；②<c>next_key</c> 分页参数，第一页为空，
    /// 后面返回前面一页返回的数据；③<c>sharer_openid</c> <b>必须是机构绑定小程序的用户 openid</b>，
    /// 而不能是其他小程序的用户 openid。
    /// </para>
    /// <para>官方参数表照录：响应体<b>只有</b> <c>sharer_info_list</c> 与 <c>next_key</c>，<b>不含</b> <c>has_more</c>（同域其余 next_key 分页端点均回 <c>has_more</c>）。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071003</c>（参数错误）/ <c>10071056</c>（请求参数（如 sharer_openid）对应的小程序未绑定到当前机构）。</para>
    /// <para>接口变更日志：2025 年 12 月 22 日新增错误码 <c>10071003</c>。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_bind_sharer_list")]
    Task<ChannelsPromoterGetBindSharerListResponse> GetBindSharerListAsync(
        [Body] ChannelsPromoterGetBindSharerListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取合作的小店列表（<c>promoter/get_bind_shop_list</c>，亦可在「微信小店·联盟带货机构 - 关系管理 - 合作商家」查看）。
    /// </summary>
    /// <param name="request">查询请求（<c>next_key</c> 与 <c>page_size</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>合作小店列表、下一页参数与翻页标志（<c>shop_list</c> / <c>next_key</c> / <c>has_more</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/account/api_getbindshoplist.html"/></para>
    /// <para>官方业务限制：<c>page_size</c> 一页获取多少个合作小店，最大 <c>10</c>；<c>next_key</c> 第一页为空，后续传上一页返回的值。</para>
    /// <para>返回的 <c>shop_appid</c> 是本店侧（<c>get_shop_*</c>）端点定位三元组的第一项，配合 <see cref="GetBindShopPromoterListAsync"/> 取 <c>promoter_id</c>。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_bind_shop_list")]
    Task<ChannelsPromoterGetBindShopListResponse> GetBindShopListAsync(
        [Body] ChannelsPromoterGetBindShopListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取合作小店的关联账号列表（<c>promoter/get_bind_shop_promoter_list</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>next_key</c> / <c>page_size</c> / <c>shop_appid</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>关联账号列表、下一页参数与翻页标志（<c>promoter_list</c> / <c>next_key</c> / <c>has_more</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/account/api_getbindshoppromoterlist.html"/></para>
    /// <para>
    /// 官方业务限制：①<c>page_size</c> 一页获取多少个关联账号，最大 <c>10</c>；②<c>shop_appid</c> 必填且须与机构存在合作关系；
    /// ③<c>promoter_type</c> 枚举见 <see cref="ChannelsPromoterShopPromoterTypes"/>（<c>1</c> 视频号 / <c>4</c> 公众号 / <c>5</c> 服务号），
    /// 官方描述「仅视频号类型有直播和短视频内容」——故文章族用 <c>4</c>/<c>5</c>，短视频与直播族用 <c>1</c>。
    /// </para>
    /// <para>返回的 <c>promoter_id</c> + <c>promoter_type</c> 是本店侧（<c>get_shop_*</c>）端点定位三元组的后两项。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071057</c>（shop_appid 无合作关系）。</para>
    /// <para>接口变更日志：2025 年 12 月 25 日更新 <c>promoter_type</c> 字段描述。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_bind_shop_promoter_list")]
    Task<ChannelsPromoterGetBindShopPromoterListResponse> GetBindShopPromoterListAsync(
        [Body] ChannelsPromoterGetBindShopPromoterListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取合作的带货者列表（<c>promoter/get_bind_talent_list</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>next_key</c> / <c>page_size</c> 必填；<c>talent_appid</c> 选填，用于查询单个合作带货者）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>合作带货者列表、下一页参数与翻页标志（<c>talent_list</c> / <c>next_key</c> / <c>has_more</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/account/api_getbindtalentlist.html"/></para>
    /// <para>
    /// 官方业务限制：<c>page_size</c> 一页获取多少个合作带货者，最大 <c>10</c>。
    /// 条目内 <c>related_finder_exportname_list</c>（关联视频号 username）与 <c>related_mp_biz_appid_list</c>（关联公众号 appid）
    /// 是 <see cref="GetFeedListAsync"/> 的 <c>finder_exportusername</c> 与 <see cref="GetTalentBizArticleListAsync"/> 的 <c>biz_appid</c> 的官方获取方式
    /// （「全部内容」合作方式必填，旧「部分内容」合作方式可不填）。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071048</c>（参数异常，请检查入参）。</para>
    /// <para>接口变更日志：2025 年 11 月 24 日新增错误码 <c>10071048</c>。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_bind_talent_list")]
    Task<ChannelsPromoterGetBindTalentListResponse> GetBindTalentListAsync(
        [Body] ChannelsPromoterGetBindTalentListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取推客的注册状态以及和机构的绑定状态（<c>promoter/get_promoter_register_and_bind_status</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>sharer_openid</c> 与 <c>sharer_appid</c> 选填；<c>is_simple_register</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>注册与绑定状态、注册/绑定业务类型与跳转串，以及命中推客的 <c>sharer_appid</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/account/api_getpromoterregisterandbindstatus.html"/></para>
    /// <para>
    /// 官方业务限制：①<c>register_status</c> 枚举见 <see cref="ChannelsPromoterRegisterStatuses"/>
    /// （<c>0</c> 未注册 / <c>1</c> 注册中 / <c>2</c> 已注册 / <c>3</c> 需实名并绑定银行卡）；
    /// ②<c>register_query_string</c> / <c>bind_query_string</c> 为官方给出的跳转业务串，由宿主拼接到小程序跳转路径；
    /// ③以 <c>sharer_openid</c> 查询时须为机构绑定小程序的用户 openid。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071003</c>（参数错误）/ <c>10071049</c>（推客功能已关闭）/ <c>10071056</c>（请求参数（如 sharer_openid）对应的小程序未绑定到当前机构）。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_promoter_register_and_bind_status")]
    Task<ChannelsPromoterGetRegisterAndBindStatusResponse> GetPromoterRegisterAndBindStatusAsync(
        [Body] ChannelsPromoterGetRegisterAndBindStatusRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 机构解绑推客（<c>promoter/headsupplier_unbind_promoter</c>）。
    /// </summary>
    /// <param name="request">解绑请求（<c>sharer_appid</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方仅返回 <c>errcode</c> / <c>errmsg</c>，故回 <see cref="ChannelsResponse"/>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/account/api_mmecpromoter_headsupplier_unbind_promoter.html"/></para>
    /// <para>官方业务限制：本接口无特殊注意事项；解绑为不可逆操作，须由宿主自行评估（解绑后该推客的分佣关系与推广参数全部失效）。</para>
    /// <para>官方错误码：<c>10071003</c>（参数错误）/ <c>10071058</c>（推客没有绑定机构）。</para>
    /// <para>接口变更日志：2025 年 11 月 24 日新增错误码 <c>10071003</c>。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/headsupplier_unbind_promoter")]
    Task<ChannelsResponse> HeadsupplierUnbindPromoterAsync(
        [Body] ChannelsPromoterUnbindPromoterRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置推客的分佣类型和比例信息（<c>promoter/set_sharer_commission_info</c>）。
    /// </summary>
    /// <param name="request">设置请求（<c>sharer_appid</c> / <c>commission_ratio</c> / <c>commission_type</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方仅返回 <c>errcode</c> / <c>errmsg</c>，故回 <see cref="ChannelsResponse"/>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/account/api_setsharercommissioninfo.html"/></para>
    /// <para>
    /// 官方业务限制：①<c>commission_ratio</c> 为「平台分佣时的分佣比例」，范围 <c>[100000, 1000000]</c>，代表 10%-100%；
    /// ②<c>commission_type</c> 枚举见 <see cref="ChannelsPromoterCommissionTypes"/>（<c>0</c> 平台分佣 / <c>1</c> 机构自己分佣）。
    /// </para>
    /// <para>
    /// 官方参数表矛盾照录：本端点 <c>commission_ratio</c> 上界为 <c>1000000</c>（100%），
    /// 而同域 <c>get_sharer_product_commission_info</c> 的商品级比例区间写作 <c>[100000, 900000]</c>（10%-90%）——两表上界不同，各自照录、不做统一校验。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071003</c>（参数错误）/ <c>10071029</c>（推客和机构未绑定）。</para>
    /// <para>接口变更日志：2025 年 11 月 24 日新增错误码 <c>10071029</c>。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/set_sharer_commission_info")]
    Task<ChannelsResponse> SetSharerCommissionInfoAsync(
        [Body] ChannelsPromoterSetSharerCommissionInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取定向券 ID 列表（<c>promoter/get_cooperative_coupon_list</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>next_key</c> / <c>page_size</c> 必填；<c>shop_appid</c>、<c>is_from_head_supplier</c>、<c>from_head_supplier_appid</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>定向券列表（条目含 <c>from_head_supplier_appid</c>）、下一页参数与翻页标志。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/coupon/api_getpromotecooperativecouponlist.html"/></para>
    /// <para>
    /// 官方业务限制：①<c>page_size</c> 券数量，不能超过 <c>200</c>；②<c>shop_appid</c> 填了只返回属于该店铺的券；
    /// ③<c>from_head_supplier_appid</c>（查询特定机构提供的券）<b>仅在 <c>is_from_head_supplier</c> 为 <c>true</c> 时有效</b>，
    /// 来源机构的分发关系见 <see cref="SetCouponToHeadSupplierAsync"/>。
    /// </para>
    /// <para>与 <see cref="GetPublicCouponListAsync"/> 的条目<b>分建</b>：定向券条目含 <c>from_head_supplier_appid</c>，公开券条目仅含 <c>coupon_id</c>（官方参数表逐字段不同）。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071003</c>（参数错误）。</para>
    /// <para>接口变更日志：2025 年 11 月 10 日新增 <c>is_from_head_supplier</c>、<c>from_head_supplier_appid</c> 请求字段，补充错误码。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_cooperative_coupon_list")]
    Task<ChannelsPromoterGetCooperativeCouponListResponse> GetCooperativeCouponListAsync(
        [Body] ChannelsPromoterGetCooperativeCouponListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取公开券 ID 列表（<c>promoter/get_public_coupon_list</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>next_key</c> / <c>page_size</c> 必填；<c>shop_appid</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>公开券列表（条目仅 <c>coupon_id</c>）、下一页参数与翻页标志。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/coupon/api_getpromotepublishcouponlist.html"/></para>
    /// <para>官方业务限制：<c>page_size</c> 券数量，不能超过 <c>200</c>；<c>shop_appid</c> 填了只返回属于该店铺的券。</para>
    /// <para>
    /// 路由与文档名不对称照录：路由为 <c>get_public_coupon_list</c>，官方文档页名为 <c>api_getpromotepublishcouponlist</c>（「公开券」写作 publish），
    /// 二者指同一端点，本仓以官方请求路径为准。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_public_coupon_list")]
    Task<ChannelsPromoterGetPublicCouponListResponse> GetPublicCouponListAsync(
        [Body] ChannelsPromoterGetPublicCouponListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取券 ID 对应的详情内容（<c>promoter/get_coupon_detail</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>coupon_id</c> 必填；<c>from_head_supplier_appid</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>券详情（<c>coupon</c>：类型 / 状态 / 优惠 / 领取 / 有效期 / 库存等子结构）及 <c>next_key</c> / <c>has_more</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/coupon/api_getpromotecoupondetail.html"/></para>
    /// <para>
    /// 官方业务限制：①<c>from_head_supplier_appid</c> 为「来源的机构」，指该机构通过 <see cref="SetCouponToHeadSupplierAsync"/>
    /// 给予本机构优惠券的情形；②券类型枚举见 <see cref="ChannelsPromoterCouponTypes"/>（<c>1</c>/<c>2</c>/<c>3</c>/<c>4</c>/<c>101</c>/<c>102</c>/<c>103</c>/<c>104</c>）、
    /// 状态枚举见 <see cref="ChannelsPromoterCouponStatuses"/>（<c>1</c>~<c>5</c>）、有效期类型见 <see cref="ChannelsPromoterCouponValidTypes"/>（<c>1</c> 固定时间 / <c>2</c> 领取后生效）。
    /// </para>
    /// <para>官方参数表照录：详情端点的返回体仍带 <c>next_key</c> 与 <c>has_more</c> 两个翻页字段（本域唯一），按原样建模、不做裁剪。</para>
    /// <para>官方参数表矛盾照录：<c>ext_info</c> 子结构的 <c>valid_time</c> 与 <c>invalid_time</c> 两行说明在官方表内互换，本仓按字段名原样映射。</para>
    /// <para>与 Marketing 域 <c>ChannelsCoupon</c> 子树<b>分建</b>：本域券子树含 <c>shop_appid</c> 与机构分发相关字段，Marketing 域券子树含 <c>promote_info</c>，字段集不同。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071003</c>（参数错误）/ <c>10071034</c>（获取优惠券失败）。</para>
    /// <para>接口变更日志：2025 年 12 月 26 日新增 <c>is_can_up</c>、<c>can_up_times</c>、<c>discount_limit</c> 字段；2025 年 11 月 24 日新增错误码 <c>10071034</c>；2025 年 11 月 10 日新增 <c>from_head_supplier_appid</c> 字段、新增「参数错误」枚举值。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_coupon_detail")]
    Task<ChannelsPromoterGetCouponDetailResponse> GetCouponDetailAsync(
        [Body] ChannelsPromoterGetCouponDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取某个券 ID 对应的小程序内嵌微信小店优惠券推广参数（<c>promoter/get_coupon_promoter_share_link</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>coupon_id</c> 必填；<c>sharer_appid</c>、<c>from_head_supplier_appid</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>小程序内嵌用的推广参数（<c>promoter_share_link</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/coupon/api_getpromotecouponpromotersharelink.html"/></para>
    /// <para>官方业务限制：<c>sharer_appid</c> 不传时生成「只带机构参数」的推广参数（即不归属具体推客，无法按推客结算佣金）。</para>
    /// <para>
    /// 请求 DTO <see cref="ChannelsPromoterCouponPromotionRequest"/> 与 <see cref="GetCouponQrCodeAsync"/>、
    /// <see cref="GetCouponShortLinkAsync"/> 共用（官方三支参数表逐字段一致）；响应各自分建，因为返回字段名不同
    /// （<c>promoter_share_link</c> / <c>qrcode_url</c> / <c>short_link</c>）。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071003</c>（参数错误）/ <c>10071035</c>（优惠券非生效中）。</para>
    /// <para>接口变更日志：2025 年 11 月 24 日新增错误码 <c>10071035</c>；2025 年 11 月 10 日新增 <c>from_head_supplier_appid</c> 字段。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_coupon_promoter_share_link")]
    Task<ChannelsPromoterCouponShareLinkResponse> GetCouponPromoterShareLinkAsync(
        [Body] ChannelsPromoterCouponPromotionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取某个券 ID 对应的推广二维码（<c>promoter/get_coupon_qr_code</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>coupon_id</c> 必填；<c>sharer_appid</c>、<c>from_head_supplier_appid</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>推广二维码地址（<c>qrcode_url</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/coupon/api_getpromotecouponqrcode.html"/></para>
    /// <para>官方业务限制：<c>sharer_appid</c> 不传时生成「只带机构参数」的券短链对应的二维码。</para>
    /// <para>请求 DTO 与 <see cref="GetCouponPromoterShareLinkAsync"/>、<see cref="GetCouponShortLinkAsync"/> 共用，理由见该方法的 remarks。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071003</c>（参数错误）/ <c>10071034</c>（获取优惠券失败）/ <c>10071035</c>（优惠券非生效中）。</para>
    /// <para>接口变更日志：2026 年 01 月 28 日新增 <c>from_head_supplier_appid</c> 字段，补充错误码 <c>10071003</c>、<c>10071035</c>、<c>10071034</c>。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_coupon_qr_code")]
    Task<ChannelsPromoterCouponQrCodeResponse> GetCouponQrCodeAsync(
        [Body] ChannelsPromoterCouponPromotionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取某个券 ID 对应的推广短链（<c>promoter/get_coupon_short_link</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>coupon_id</c> 必填；<c>sharer_appid</c>、<c>from_head_supplier_appid</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>推广短链（<c>short_link</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/coupon/api_getpromotecouponshortlink.html"/></para>
    /// <para>官方业务限制：<c>sharer_appid</c> 不传时生成「只带机构参数」的券短链。</para>
    /// <para>请求 DTO 与 <see cref="GetCouponPromoterShareLinkAsync"/>、<see cref="GetCouponQrCodeAsync"/> 共用，理由见该方法的 remarks。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071003</c>（参数错误）/ <c>10071034</c>（获取优惠券失败）/ <c>10071035</c>（优惠券非生效中）。</para>
    /// <para>接口变更日志：2025 年 12 月 22 日新增错误码 <c>10071034</c>；2025 年 11 月 24 日新增错误码 <c>10071035</c>；2025 年 11 月 10 日新增 <c>from_head_supplier_appid</c> 字段。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_coupon_short_link")]
    Task<ChannelsPromoterCouponShortLinkResponse> GetCouponShortLinkAsync(
        [Body] ChannelsPromoterCouponPromotionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取分发给其他机构的优惠券（<c>promoter/get_coupon_to_headsupplier</c>，按接收机构维度分页查询分发记录）。
    /// </summary>
    /// <param name="request">查询请求（<c>to_head_supplier_appid</c> / <c>limit</c> / <c>page_context</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分发记录列表、翻页上下文与符合条件的记录总数。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/coupon/api_get_coupon_to_headsupplier.html"/></para>
    /// <para>
    /// 官方业务限制：本端点是本域<b>唯一</b>使用 <c>limit</c> + <c>page_context</c> 翻页形态的列表（其余列表端点用 <c>next_key</c> + <c>page_size</c>），
    /// <c>page_context</c> 第一页放空、后续传上一页返回值；响应含 <c>total_count</c>。
    /// </para>
    /// <para>与 <see cref="SetCouponToHeadSupplierAsync"/> 成对：前者查询分发记录，后者执行分发。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071003</c>（参数错误）。</para>
    /// <para>接口变更日志：2025 年 11 月 10 日新增「获取分发给其他机构的优惠券」接口。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_coupon_to_headsupplier")]
    Task<ChannelsPromoterGetCouponToHeadSupplierResponse> GetCouponToHeadSupplierAsync(
        [Body] ChannelsPromoterGetCouponToHeadSupplierRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 分发优惠券给其他机构（<c>promoter/set_coupon_to_headsupplier</c>）。
    /// </summary>
    /// <param name="request">分发请求（<c>to_head_supplier_appid</c> 与 <c>coupon_id</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方仅返回 <c>errcode</c> / <c>errmsg</c>，故回 <see cref="ChannelsResponse"/>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/coupon/api_set_coupon_to_headsupplier.html"/></para>
    /// <para>
    /// 官方注意事项（原文照录，属不可逆业务约束）：①「只能店铺设置给当前机构的定向券才能分发给其他机构」，
    /// 即 <c>coupon_id</c> 只能取自 <see cref="GetCooperativeCouponListAsync"/>（定向券列表），公开券不可分发；
    /// ②「设置之后，目前无法取消，请自行评估」——本仓不提供撤销端点，因为官方无对应开放面。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071003</c>（参数错误）。</para>
    /// <para>接口变更日志：2025 年 11 月 10 日新增「分发优惠券给其他机构」接口。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/set_coupon_to_headsupplier")]
    Task<ChannelsResponse> SetCouponToHeadSupplierAsync(
        [Body] ChannelsPromoterSetCouponToHeadSupplierRequest request,
        CancellationToken cancellationToken = default);
    /// <summary>
    /// 获取带货者推广的短视频信息（<c>promoter/get_feed_list</c>，带货者侧短视频列表）。
    /// </summary>
    /// <param name="request">查询请求（<c>talent_appid</c> / <c>next_key</c> / <c>page_size</c> 必填；<c>finder_exportusername</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>短视频列表（条目含商品信息与预计佣金）、下一页参数与翻页标志。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/video/api_getfeedlist.html"/></para>
    /// <para>
    /// 官方业务限制：①<c>page_size</c> 一页获取多少个数据，最大 <c>10</c>；②<c>finder_exportusername</c>
    /// 「当合作方式是『全部内容合作』时，需要指定视频号 username，如果是之前的『部分内容』的合作方式，则可以不填」，
    /// 获得方式为 <see cref="GetBindTalentListAsync"/> 返回的 <c>related_finder_exportname_list</c>；
    /// ③官方注意事项原文：「已兼容新老内容合作方式——后台查询时，会先查询是否有『全部内容』的合作，如果没有才查询『部分内容』的合作」。
    /// </para>
    /// <para>与 <see cref="GetShopFeedListAsync"/> 的条目 DTO <b>分建</b>：本店侧条目无 <c>talent_appid</c>；商品子结构 <c>product_info</c> 逐字段一致故共用 <see cref="ChannelsPromoterFeedProductInfo"/>。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071049</c>（推客功能已关闭）。</para>
    /// <para>接口变更日志：2025 年 11 月 24 日新增错误码 <c>10071049</c>。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_feed_list")]
    Task<ChannelsPromoterGetFeedListResponse> GetFeedListAsync(
        [Body] ChannelsPromoterGetFeedListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 生成某个带货者的某些短视频内嵌短视频卡片需要的 feedtoken（<c>promoter/get_feed_promotion_info</c>）。
    /// </summary>
    /// <param name="request">生成请求（<c>feed_list</c> / <c>mini_program_appid</c> / <c>talent_appid</c> 必填；<c>sharer_appid</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>短视频 token 列表（<c>export_id</c> / <c>feed_token</c> / <c>promoter_share_link</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/video/api_getfeedpromotioninfo.html"/></para>
    /// <para>
    /// 官方业务限制：①<c>mini_program_appid</c> 「需要挂在的小程序 appid【需要和机构绑定】」，未绑定时报 <c>10071046</c>；
    /// ②<c>sharer_appid</c> 选填，「为了生成某个推客推广的 link」，不传则生成不含推客归属的推广参数；
    /// ③请求条目 <c>feed_list[].export_id</c> 在本端点为必填。
    /// </para>
    /// <para>响应 DTO <see cref="ChannelsPromoterFeedPromotionInfoResponse"/> 与 <see cref="GetShopFeedPromotionInfoAsync"/> 共用（官方返回参数表逐字段一致）。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071003</c>（参数错误）/ <c>10071046</c>（小程序没有绑定机构）/ <c>10071048</c>（参数异常，请检查入参）/ <c>10071049</c>（推客功能已关闭）。</para>
    /// <para>接口变更日志：2025 年 11 月 24 日新增错误码 <c>10071003</c>、<c>10071046</c>、<c>10071049</c>。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_feed_promotion_info")]
    Task<ChannelsPromoterFeedPromotionInfoResponse> GetFeedPromotionInfoAsync(
        [Body] ChannelsPromoterGetFeedPromotionInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取小店关联账号的短视频列表（<c>promoter/get_shop_feed_list</c>，本店侧短视频列表）。
    /// </summary>
    /// <param name="request">查询请求（<c>next_key</c> / <c>page_size</c> / <c>shop_appid</c> / <c>promoter_id</c> / <c>promoter_type</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>短视频列表（条目无 <c>talent_appid</c>）、下一页参数与翻页标志。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/video/api_getshopfeedlist.html"/></para>
    /// <para>
    /// 官方业务限制：①<c>page_size</c> 最大 <c>10</c>；②定位三元组 <c>shop_appid</c> + <c>promoter_id</c> + <c>promoter_type</c>
    /// 分别取自 <see cref="GetBindShopListAsync"/> 与 <see cref="GetBindShopPromoterListAsync"/>；
    /// ③<c>promoter_type</c> 官方要求「需要是视频号」（值 <c>1</c>，见 <see cref="ChannelsPromoterShopPromoterTypes"/>），
    /// 传公众号/服务号会落 <c>10071055</c>（关联账号 id 错误）。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071048</c>（参数异常，请检查入参）/ <c>10071049</c>（推客功能已关闭）/ <c>10071055</c>（关联账号 id 错误）。</para>
    /// <para>接口变更日志：2025 年 11 月 24 日新增错误码 <c>10071048</c>、<c>10071049</c>、<c>10071055</c>。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_shop_feed_list")]
    Task<ChannelsPromoterGetShopFeedListResponse> GetShopFeedListAsync(
        [Body] ChannelsPromoterGetShopFeedListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 生成小店关联账号的短视频内嵌小程序推广信息（<c>promoter/get_shop_feed_promotion_info</c>）。
    /// </summary>
    /// <param name="request">生成请求（<c>shop_appid</c> / <c>promoter_id</c> / <c>mini_program_appid</c> / <c>feed_list</c> / <c>sharer_appid</c> 必填；<c>promoter_type</c> 官方标选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>短视频 token 列表（与 <see cref="GetFeedPromotionInfoAsync"/> 共用响应类型）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/video/api_getshopfeedpromotioninfo.html"/></para>
    /// <para>
    /// 官方业务限制：①<c>mini_program_appid</c> 需要和机构绑定，未绑定报 <c>10071046</c>；
    /// ②<c>sharer_appid</c> 在本端点为<b>必填</b>（带货者侧 <see cref="GetFeedPromotionInfoAsync"/> 为选填）；
    /// ③<c>promoter_type</c> 需要是视频号。
    /// </para>
    /// <para>
    /// 官方参数表不一致照录（两处，与本族其余端点冲突，本仓按参数表原样建模、不做运行时校验）：
    /// ①<c>promoter_type</c> 在本端点标「否」，而同族 <c>get_shop_feed_list</c> 等均为「是」；
    /// ②请求条目 <c>feed_list[].export_id</c> 在本端点标「否」，而 <c>get_feed_promotion_info</c> 标「是」，
    /// 二者共用 <see cref="ChannelsPromoterFeedExportItem"/>，故该字段建模为可空字符串。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071046</c>（小程序没有绑定机构）/ <c>10071048</c>（参数异常，请检查入参）/ <c>10071049</c>（推客功能已关闭）/ <c>10071055</c>（关联账号 id 错误）。</para>
    /// <para>接口变更日志：2025 年 12 月 22 日新增错误码 <c>10071049</c>、<c>10071055</c>；2025 年 11 月 24 日新增错误码 <c>10071048</c>。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_shop_feed_promotion_info")]
    Task<ChannelsPromoterFeedPromotionInfoResponse> GetShopFeedPromotionInfoAsync(
        [Body] ChannelsPromoterGetShopFeedPromotionInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取某个带货者当前直播的带货商品列表（<c>promoter/get_live_commission_product_list</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>talent_appid</c> / <c>next_key</c> / <c>page_size</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>直播带货商品列表、下一页参数与翻页标志。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getlivecommissionproductlist.html"/></para>
    /// <para>官方业务限制：<c>page_size</c> 一页获取多少个商品，最大 <c>10</c>；只返回「当前直播」的商品，须带货者正在开播。</para>
    /// <para>与 <see cref="GetShopLiveCommissionProductListAsync"/> 的条目 DTO <b>分建</b>：本店侧条目多出 <c>commission_rate</c>（小店自营商品的分佣比例）。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071048</c>（参数异常，请检查入参）/ <c>10071049</c>（推客功能已关闭）。</para>
    /// <para>接口变更日志：2025 年 11 月 24 日新增错误码 <c>10071049</c>、<c>10071048</c>。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_live_commission_product_list")]
    Task<ChannelsPromoterGetLiveCommissionProductListResponse> GetLiveCommissionProductListAsync(
        [Body] ChannelsPromoterGetLiveCommissionProductListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取小店关联账号的当前直播的自营商品列表（<c>promoter/get_shop_live_commission_product_list</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>next_key</c> / <c>page_size</c> / <c>shop_appid</c> / <c>promoter_id</c> / <c>promoter_type</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>直播自营商品列表（条目含 <c>commission_rate</c>）、下一页参数与翻页标志。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getshoplivecommissionproductlist.html"/></para>
    /// <para>官方业务限制：①<c>page_size</c> 最大 <c>10</c>；②<c>promoter_type</c> 需要是视频号（<c>1</c>）；③定位三元组取自 <see cref="GetBindShopPromoterListAsync"/>。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071047</c>（没有相关的合作内容）/ <c>10071048</c>（参数异常，请检查入参）/ <c>10071049</c>（推客功能已关闭）/ <c>10071055</c>（关联账号 id 错误）。</para>
    /// <para>接口变更日志：2025 年 12 月 22 日新增错误码 <c>10071049</c>；2025 年 11 月 24 日新增错误码 <c>10071055</c>、<c>10071048</c> 等。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_shop_live_commission_product_list")]
    Task<ChannelsPromoterGetShopLiveCommissionProductListResponse> GetShopLiveCommissionProductListAsync(
        [Body] ChannelsPromoterGetShopLiveCommissionProductListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 生成某个带货者的用于直播预约的推广参数（<c>promoter/get_live_notice_promoter_share_link</c>）。
    /// </summary>
    /// <param name="request">生成请求（<c>mini_program_appid</c> / <c>notice_id</c> / <c>talent_appid</c> / <c>sharer_appid</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>直播预约推广参数（<c>promoter_share_link</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getlivenoticepromotersharelink.html"/></para>
    /// <para>
    /// 官方注意事项（原文）：小程序侧配合 <c>jsapi</c> 使用——
    /// <c>wx.reserveChannelsLive({ noticeId, promoterShareLink })</c>，即本端点返回的 <c>promoter_share_link</c> 须与 <c>notice_id</c> 一并传给该 API 才能完成归因。
    /// </para>
    /// <para>响应 DTO <see cref="ChannelsPromoterLiveShareLinkResponse"/> 与 <see cref="GetShopLiveNoticePromoterShareLinkAsync"/> 共用（官方返回参数表一致）。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071030</c>（获取带货者账号错误，官方解决方案：检查 <c>talent_appid</c> 参数）/ <c>10071048</c>（参数异常，请检查入参）。</para>
    /// <para>接口变更日志：2025 年 11 月 24 日新增错误码 <c>10071048</c>。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_live_notice_promoter_share_link")]
    Task<ChannelsPromoterLiveShareLinkResponse> GetLiveNoticePromoterShareLinkAsync(
        [Body] ChannelsPromoterGetLiveNoticePromoterShareLinkRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 为推客获取小店关联账号直播预约的推广参数（<c>promoter/get_shop_live_notice_promoter_share_link</c>）。
    /// </summary>
    /// <param name="request">生成请求（<c>shop_appid</c> / <c>promoter_id</c> / <c>promoter_type</c> / <c>notice_id</c> / <c>sharer_appid</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>直播预约推广参数（与 <see cref="GetLiveNoticePromoterShareLinkAsync"/> 共用响应类型）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getshoplivenoticepromotersharelink.html"/></para>
    /// <para>官方业务限制：<c>promoter_type</c> 官方示例为 <c>1</c>（需要是视频号）；<c>notice_id</c> 取自 <see cref="GetShopLiveNoticeRecordListAsync"/>。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071048</c>（参数异常，请检查入参）。</para>
    /// <para>接口变更日志：2025 年 11 月 24 日新增错误码 <c>10071048</c>。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_shop_live_notice_promoter_share_link")]
    Task<ChannelsPromoterLiveShareLinkResponse> GetShopLiveNoticePromoterShareLinkAsync(
        [Body] ChannelsPromoterGetShopLiveNoticePromoterShareLinkRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取带货者的直播预约列表（<c>promoter/get_live_notice_record_list</c>，官方无分页参数）。
    /// </summary>
    /// <param name="request">查询请求（<c>talent_appid</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>直播预约数据（<c>notice_id</c> / <c>description</c> / <c>start_time</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getlivenoticerecordlist.html"/></para>
    /// <para>官方业务限制：本端点官方<b>只有</b> <c>talent_appid</c> 一个请求参数，<b>无分页</b>；返回的 <c>notice_id</c> 是 <see cref="GetLiveNoticePromoterShareLinkAsync"/>、<see cref="GetLiveNoticeRecordQrCodeAsync"/> 与 <see cref="GetLiveNoticeReservationInfoAsync"/> 的定位键（后者参数名为 <c>live_notice_id</c>）。</para>
    /// <para>响应 DTO <see cref="ChannelsPromoterLiveNoticeRecordListResponse"/> 与 <see cref="GetShopLiveNoticeRecordListAsync"/> 共用（官方返回参数表逐字段一致）。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071047</c>（没有相关的合作内容）/ <c>10071048</c>（参数异常，请检查入参）/ <c>10071049</c>（推客功能已关闭）。</para>
    /// <para>接口变更日志：2025 年 11 月 24 日新增错误码 <c>10071049</c>。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_live_notice_record_list")]
    Task<ChannelsPromoterLiveNoticeRecordListResponse> GetLiveNoticeRecordListAsync(
        [Body] ChannelsPromoterGetLiveNoticeRecordListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取小店关联账号的预约直播列表（<c>promoter/get_shop_live_notice_record_list</c>，官方无分页参数）。
    /// </summary>
    /// <param name="request">查询请求（<c>shop_appid</c> / <c>promoter_id</c> / <c>promoter_type</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>直播预约数据（与 <see cref="GetLiveNoticeRecordListAsync"/> 共用响应类型）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getshoplivenoticerecordlist.html"/></para>
    /// <para>官方业务限制：<c>promoter_type</c> 需要是视频号（<c>1</c>）；本端点无分页参数。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071047</c>（没有相关的合作内容）/ <c>10071048</c>（参数异常，请检查入参）/ <c>10071049</c>（推客功能已关闭）/ <c>10071055</c>（关联账号 id 错误）。</para>
    /// <para>接口变更日志：2025 年 11 月 24 日新增错误码 <c>10071055</c>、<c>10071049</c> 等。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_shop_live_notice_record_list")]
    Task<ChannelsPromoterLiveNoticeRecordListResponse> GetShopLiveNoticeRecordListAsync(
        [Body] ChannelsPromoterGetShopLiveNoticeRecordListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 为某个推客生成单个直播预约的推广二维码（<c>promoter/get_live_notice_record_qr_code</c>）。
    /// </summary>
    /// <param name="request">生成请求（<c>talent_appid</c> / <c>notice_id</c> / <c>sharer_appid</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>推广二维码地址（<c>qrcode_url</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getlivenoticerecordqrcode.html"/></para>
    /// <para>官方业务限制：本端点三支参数全部必填（与短视频推广参数端点的「<c>sharer_appid</c> 选填」不同）。</para>
    /// <para>响应 DTO <see cref="ChannelsPromoterLiveQrCodeResponse"/> 为本域直播族四支二维码端点共用（官方返回参数表恒为 <c>qrcode_url</c> 单字段）。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071048</c>（参数异常，请检查入参）。</para>
    /// <para>接口变更日志：2025 年 11 月 24 日新增错误码 <c>10071048</c>。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_live_notice_record_qr_code")]
    Task<ChannelsPromoterLiveQrCodeResponse> GetLiveNoticeRecordQrCodeAsync(
        [Body] ChannelsPromoterGetLiveNoticeRecordQrCodeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 为推客生成小店关联账号直播预约的推广二维码（<c>promoter/get_shop_live_notice_record_qr_code</c>）。
    /// </summary>
    /// <param name="request">生成请求（<c>shop_appid</c> / <c>promoter_id</c> / <c>notice_id</c> / <c>sharer_appid</c> / <c>promoter_type</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>推广二维码地址（与 <see cref="GetLiveNoticeRecordQrCodeAsync"/> 共用响应类型）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getshoplivenoticerecordqrcode.html"/></para>
    /// <para>官方业务限制：<c>promoter_type</c> 需要是视频号（<c>1</c>）。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071048</c>（参数异常，请检查入参）。</para>
    /// <para>接口变更日志：2025 年 11 月 24 日新增错误码 <c>10071048</c>。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_shop_live_notice_record_qr_code")]
    Task<ChannelsPromoterLiveQrCodeResponse> GetShopLiveNoticeRecordQrCodeAsync(
        [Body] ChannelsPromoterGetShopLiveNoticeRecordQrCodeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取直播预约的单个推客的推广预约人数（<c>promoter/get_live_notice_reservation_info</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>live_notice_id</c> 与 <c>sharer_appid</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>该推客带来的预约人数（<c>count</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getlivenoticereservationinfo.html"/></para>
    /// <para>
    /// 官方参数表照录：预约 id 在本端点写作 <c>live_notice_id</c>，而兄弟端点 <c>get_live_notice_promoter_share_link</c> 与
    /// <c>get_live_notice_record_qr_code</c> 写作 <c>notice_id</c>，值同为 <see cref="GetLiveNoticeRecordListAsync"/> 返回的预约 id——
    /// 字段名不统一属官方现状，本仓逐端点照抄、不做别名收敛。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071003</c>（参数错误）/ <c>10071030</c>（获取推客 talentuin 失败）。</para>
    /// <para>接口变更日志：2025 年 11 月 24 日新增错误码 <c>10071030</c>、<c>10071003</c>。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_live_notice_reservation_info")]
    Task<ChannelsPromoterGetLiveNoticeReservationInfoResponse> GetLiveNoticeReservationInfoAsync(
        [Body] ChannelsPromoterGetLiveNoticeReservationInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取带货者的直播列表（<c>promoter/get_live_record_list</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>talent_appid</c> / <c>mini_program_appid</c> 必填；<c>sharer_appid</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>直播数据（<c>export_id</c> / <c>description</c> / <c>promoter_share_link</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getliverecordlist.html"/></para>
    /// <para>官方业务限制：①<c>mini_program_appid</c> 「需要挂载的小程序 appid【要和机构绑定】」，未绑定报 <c>10071046</c>；②<c>sharer_appid</c> 选填，「为了生成某个推客推广的 link」；③本端点无分页参数。</para>
    /// <para>响应 DTO <see cref="ChannelsPromoterLiveRecordListResponse"/> 与 <see cref="GetShopLiveRecordListAsync"/> 共用（官方返回参数表逐字段一致）。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071046</c>（小程序没有绑定机构）/ <c>10071047</c>（没有相关的合作内容）/ <c>10071048</c>（参数异常，请检查入参）/ <c>10071049</c>（推客功能已关闭）。</para>
    /// <para>接口变更日志：2025 年 11 月 24 日新增错误码 <c>10071046</c>、<c>10071047</c> 等。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_live_record_list")]
    Task<ChannelsPromoterLiveRecordListResponse> GetLiveRecordListAsync(
        [Body] ChannelsPromoterGetLiveRecordListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取小店关联账号的直播列表（<c>promoter/get_shop_live_record_list</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>shop_appid</c> / <c>promoter_id</c> / <c>mini_program_appid</c> / <c>sharer_appid</c> / <c>promoter_type</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>直播数据（与 <see cref="GetLiveRecordListAsync"/> 共用响应类型）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getshopliverecordlist.html"/></para>
    /// <para>官方业务限制：<c>sharer_appid</c> 在本端点为<b>必填</b>（带货者侧 <see cref="GetLiveRecordListAsync"/> 为选填）；<c>promoter_type</c> 需要是视频号。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071046</c>（小程序没有绑定机构）/ <c>10071047</c>（没有相关的合作内容）/ <c>10071048</c>（参数异常，请检查入参）/ <c>10071049</c>（推客功能已关闭）/ <c>10071055</c>（关联账号 id 错误）。</para>
    /// <para>接口变更日志：2025 年 11 月 24 日新增错误码 <c>10071046</c>、<c>10071047</c> 等。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_shop_live_record_list")]
    Task<ChannelsPromoterLiveRecordListResponse> GetShopLiveRecordListAsync(
        [Body] ChannelsPromoterGetShopLiveRecordListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 为某个推客生成单个直播的推广二维码（<c>promoter/get_live_record_qr_code</c>）。
    /// </summary>
    /// <param name="request">生成请求（<c>talent_appid</c> / <c>export_id</c> / <c>sharer_appid</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>推广二维码地址（与 <see cref="GetLiveNoticeRecordQrCodeAsync"/> 共用响应类型）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getliverecordqrcode.html"/></para>
    /// <para>官方业务限制：<c>export_id</c> 为直播 id，取自 <see cref="GetLiveRecordListAsync"/> 的返回条目。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071003</c>（参数错误）/ <c>10071047</c>（没有相关的合作内容）/ <c>10071048</c>（参数异常，请检查入参）。</para>
    /// <para>接口变更日志：2025 年 11 月 24 日新增错误码 <c>10071003</c>、<c>10071047</c> 等。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_live_record_qr_code")]
    Task<ChannelsPromoterLiveQrCodeResponse> GetLiveRecordQrCodeAsync(
        [Body] ChannelsPromoterGetLiveRecordQrCodeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 为推客生成小店关联账号直播的推广二维码（<c>promoter/get_shop_live_record_qr_code</c>）。
    /// </summary>
    /// <param name="request">生成请求（<c>shop_appid</c> / <c>promoter_id</c> / <c>export_id</c> / <c>sharer_appid</c> / <c>promoter_type</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>推广二维码地址（与 <see cref="GetLiveRecordQrCodeAsync"/> 共用响应类型）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getshopliverecordqrcode.html"/></para>
    /// <para>官方业务限制：<c>export_id</c> 取自 <see cref="GetShopLiveRecordListAsync"/>；<c>promoter_type</c> 需要是视频号。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071047</c>（没有相关的合作内容）。</para>
    /// <para>接口变更日志：2025 年 11 月 24 日新增错误码 <c>10071047</c>。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_shop_live_record_qr_code")]
    Task<ChannelsPromoterLiveQrCodeResponse> GetShopLiveRecordQrCodeAsync(
        [Body] ChannelsPromoterGetShopLiveRecordQrCodeRequest request,
        CancellationToken cancellationToken = default);
    /// <summary>
    /// 获取小店关联账号的文章列表（<c>promoter/get_shop_mp_article_list</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>next_key</c> / <c>page_size</c> / <c>shop_appid</c> / <c>promoter_id</c> / <c>promoter_type</c> / <c>sharer_appid</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>文章列表（含文章基础信息、商品列表与创建/更新时间）、下一页参数与翻页标志。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/article/api_getshopmparticlelist.html"/></para>
    /// <para>
    /// 官方业务限制：①<c>page_size</c> 最大 <c>10</c>；②<c>promoter_type</c> 官方要求「需要是公众号或服务号」
    /// （<c>4</c> / <c>5</c>，见 <see cref="ChannelsPromoterShopPromoterTypes"/>）——与短视频、直播两族的「需要是视频号」相反，
    /// 传 <c>1</c> 会落 <c>10071055</c> 类错误；③文章基础信息子结构 <c>article_info</c>（标题 / 封面 / 链接 / 推广参数）与带货者侧共用
    /// <see cref="ChannelsPromoterArticleInfo"/>。
    /// </para>
    /// <para>与 <see cref="GetTalentBizArticleListAsync"/> 的条目与商品子结构<b>分建</b>：本店侧条目商品子结构用 <c>commission_rate</c>、数组名为 <c>product_list</c>；带货者侧用 <c>product_mini_price</c>、数组名为 <c>product_info_list</c>。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071048</c>（参数异常，请检查入参）/ <c>10071049</c>（推客功能已关闭）。</para>
    /// <para>接口变更日志：2025 年 12 月 23 日新增错误码 <c>10071049</c>；2025 年 11 月 24 日新增错误码 <c>10071048</c>。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_shop_mp_article_list")]
    Task<ChannelsPromoterGetShopMpArticleListResponse> GetShopMpArticleListAsync(
        [Body] ChannelsPromoterGetShopMpArticleListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取某个带货者可推广文章列表（<c>promoter/get_talent_biz_article_list</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>talent_appid</c> / <c>next_key</c> / <c>page_size</c> / <c>sharer_appid</c> 必填；<c>biz_appid</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>文章列表（条目商品子结构数组名为 <c>product_info_list</c>）、下一页参数与翻页标志。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/article/api_gettalentbizarticlelist.html"/></para>
    /// <para>
    /// 官方业务限制：①<c>page_size</c> 最大 <c>10</c>；②<c>biz_appid</c>「当合作方式是『全部内容合作』时，需要指定公众号 appid。
    /// 如果是之前的『部分内容』的合作方式，则可以不填」，获得方式为 <see cref="GetBindTalentListAsync"/> 的 <c>related_mp_biz_appid_list</c>；
    /// ③官方注意事项原文：「已兼容新老内容合作方式——后台查询时，会先查询是否有『全部内容』的合作，如果没有才查询『部分内容』的合作」。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071049</c>（推客功能已关闭）。</para>
    /// <para>接口变更日志：2025 年 12 月 23 日新增错误码 <c>10071049</c>。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_talent_biz_article_list")]
    Task<ChannelsPromoterGetTalentBizArticleListResponse> GetTalentBizArticleListAsync(
        [Body] ChannelsPromoterGetTalentBizArticleListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取推客对某个商品的推广短链（<c>promoter/get_product_promotion_link_info</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>sharer_appid</c> 与 <c>sharer_openid</c> 二选一；<c>product_id</c> + <c>shop_appid</c> 与 <c>product_short_link</c> 二选一；<c>head_supplier_appid</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>推广短链（<c>short_link</c> / <c>http_short_link</c>）及命中商品的 <c>product_id</c> 与 <c>shop_appid</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/getshop/api_getproductpromotionlinkinfo.html"/></para>
    /// <para>
    /// 官方业务限制：①<c>sharer_openid</c>「必须是机构绑定小程序的用户 openid，而不能是其他小程序的用户 openid」，
    /// 否则报 <c>10071056</c>；②使用 <c>product_id</c> 时必须同时传入 <c>shop_appid</c>；③<c>product_short_link</c> 与 <c>product_id</c> 二选一。
    /// </para>
    /// <para>
    /// 请求 DTO <see cref="ChannelsPromoterProductPromotionRequest"/> 与 <see cref="GetProductPromotionQrCodeInfoAsync"/> 共用
    /// （官方两支参数表逐字段一致）；与 <see cref="GetPromoterSingleProductPromotionInfoAsync"/> <b>分建</b>——后者多一个 <c>custom_token</c> 字段，
    /// 本端点参数表<b>无</b> <c>custom_token</c>，即推广短链/二维码不支持携带用户自定义关联参数，只有商品卡内嵌链接支持。
    /// </para>
    /// <para>接口变更日志：2025 年 12 月 22 日新增错误码 <c>10071049</c>；2025 年 12 月 22 日返回参数新增商品 id 和店铺 id；2025 年 11 月 24 日新增错误码 <c>10071053</c>、<c>10071030</c> 等。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071003</c>（参数错误）/ <c>10071029</c>（推客和机构未绑定）/ <c>10071030</c>（获取推客 talentuin 失败）/ <c>10071048</c>（参数异常，请检查入参）/ <c>10071049</c>（推客功能已关闭）/ <c>10071053</c>（商品不在架）/ <c>10071056</c>（请求参数（如 sharer_openid）对应的小程序未绑定到当前机构）。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_product_promotion_link_info")]
    Task<ChannelsPromoterProductPromotionLinkInfoResponse> GetProductPromotionLinkInfoAsync(
        [Body] ChannelsPromoterProductPromotionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取推客对某个商品的推广二维码（<c>promoter/get_product_promotion_qrcode_info</c>）。
    /// </summary>
    /// <param name="request">查询请求（字段与二选一约束同 <see cref="GetProductPromotionLinkInfoAsync"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>推广二维码地址（<c>qrcode_url</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/getshop/api_getproductpromotionqrcodeinfo.html"/></para>
    /// <para>官方业务限制：请求参数表与 <see cref="GetProductPromotionLinkInfoAsync"/> 逐字段一致，故共用 <see cref="ChannelsPromoterProductPromotionRequest"/>；本端点参数表同样<b>无</b> <c>custom_token</c>。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071003</c>（参数错误）/ <c>10071029</c>（推客和机构未绑定）/ <c>10071030</c>（获取推客 talentuin 失败）/ <c>10071048</c>（参数异常，请检查入参）/ <c>10071049</c>（推客功能已关闭）/ <c>10071053</c>（商品不在架）/ <c>10071056</c>（请求参数（如 sharer_openid）对应的小程序未绑定到当前机构）。</para>
    /// <para>接口变更日志：2025 年 12 月 22 日新增错误码 <c>10071048</c>、<c>10071049</c>；2025 年 11 月 24 日新增错误码 <c>10071053</c>、<c>10071048</c> 等。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_product_promotion_qrcode_info")]
    Task<ChannelsPromoterProductPromotionQrCodeResponse> GetProductPromotionQrCodeInfoAsync(
        [Body] ChannelsPromoterProductPromotionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取某个推客某个商品的内嵌商品卡片 product_promotion_link（<c>promoter/get_promoter_single_product_promotion_info</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>sharer_appid</c>/<c>sharer_openid</c> 二选一、<c>product_id</c>+<c>shop_appid</c> 与 <c>product_short_link</c> 二选一，另可带 <c>custom_token</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>内嵌商品卡片用的推广链接（<c>product_promotion_link</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/getshop/api_getpromotersingleproductpromotioninfo.html"/></para>
    /// <para>
    /// 官方业务限制（<c>custom_token</c> 原文照录）：「长度要求 &lt;=128，只有商品卡的分享直接成交的才生效，间接锁客的不生效，
    /// 当前不支持二维码和短链；这个关联参数直接成交时，会在订单详情中返回」——即归因参数只在商品卡场景与直接成交链路可见。
    /// </para>
    /// <para>请求 DTO 与 <see cref="GetProductPromotionLinkInfoAsync"/> / <see cref="GetProductPromotionQrCodeInfoAsync"/> <b>分建</b>，唯一差异即 <c>custom_token</c>。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071003</c>（参数错误）/ <c>10071029</c>（推客和机构未绑定）/ <c>10071030</c>（获取推客 talentuin 失败）/ <c>10071048</c>（参数异常，请检查入参）/ <c>10071056</c>（请求参数（如 sharer_openid）对应的小程序未绑定到当前机构）。</para>
    /// <para>接口变更日志：2026 年 03 月 12 日新增 <c>custom_token</c> 字段，用户自定义关联参数；2025 年 11 月 24 日新增错误码 <c>10071003</c>、<c>10071029</c> 等。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_promoter_single_product_promotion_info")]
    Task<ChannelsPromoterGetSingleProductPromotionInfoResponse> GetPromoterSingleProductPromotionInfoAsync(
        [Body] ChannelsPromoterGetSingleProductPromotionInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取可推广的商品详情（<c>promoter/get_promote_product_detail</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>shop_appid</c> / <c>plan_type</c> / <c>product_id</c> 必填；<c>get_available_coupon</c>、<c>head_supplier_appid</c>、<c>custom_token</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品详情（<c>product</c>：标题 / 头图 / 描述 / 类目 / SKU / 推广链接 / 状态 / 佣金与供货机构信息）及可用机构券（公开券、定向券两个数组）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/getshop/api_getpromoteproductdetail.html"/></para>
    /// <para>
    /// 官方业务限制：①<c>plan_type</c> 枚举见 <see cref="ChannelsPromoterPlanTypes"/>
    /// （<c>1</c> 定向计划 / <c>2</c> 公开计划 / <c>3</c> 机构定向计划 / <c>4</c> 机构普通计划），
    /// 且「当 <c>plan_type</c> 为 <c>3</c> 和 <c>4</c> 时，<c>head_supplier_appid</c> 这个参数必传」；
    /// ②<c>get_available_coupon</c> 填 <c>true</c> 才返回商品可用的机构券；
    /// ③<c>custom_token</c> 约束同 <see cref="GetPromoterSingleProductPromotionInfoAsync"/>（长度 &lt;=128，仅商品卡直接成交生效）；
    /// ④保障标枚举见 <see cref="ChannelsPromoterProductFlags"/>，商品状态枚举见 <see cref="ChannelsPromoterProductStatuses"/>，
    /// 推广状态枚举见 <see cref="ChannelsPromoterProductPromotionStatuses"/>。
    /// </para>
    /// <para>
    /// 官方文档矛盾与裁剪照录：①<c>commission_info.status</c> 参数表类型为 string、但其枚举与代码示例均为数字，本仓取 <c>int?</c>；
    /// ②<see cref="ChannelsPromoterPromoteProductSku.UserInclusivePrice"/>（用户视角普惠价）为 2026-09-16 变更新增；
    /// ③官方代码示例额外出现 <c>express_info</c>、<c>extra_service</c>、<c>statistical_data</c>、<c>normal_commission_info</c>
    /// 四个子树，但官方<b>参数表未定义其字段</b>，本仓不建模（避免凭示例臆造契约面），需要时读原始 JSON。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误，请开发人员检查传递参数是否正确）/ <c>10024003</c>（不合法的 appid）/ <c>10024004</c>（不存在该商品）/ <c>10071003</c>（参数错误）/ <c>10071008</c>（该商品不存在）/ <c>10071033</c>（shop_appid 错误）/ <c>10071044</c>（推客功能保证金不足）/ <c>10071048</c>（参数异常，请检查入参）/ <c>10071049</c>（推客功能已关闭）/ <c>10071053</c>（商品不在架）/ <c>10071056</c>（请求参数（如 sharer_openid）对应的小程序未绑定到当前机构）。</para>
    /// <para>接口变更日志：2026 年 09 月 16 日增加用户视角普惠价；2026 年 03 月 12 日新增 <c>custom_token</c> 字段；2025 年 11 月 24 日新增错误码 <c>10071008</c>、<c>10071033</c>、<c>10071056</c>、<c>10071049</c> 等。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_promote_product_detail")]
    Task<ChannelsPromoterGetPromoteProductDetailResponse> GetPromoteProductDetailAsync(
        [Body] ChannelsPromoterGetPromoteProductDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取可推广的商品 id 列表（<c>promoter/get_promote_product_list</c>，支持多维筛选）。
    /// </summary>
    /// <param name="request">查询请求（<c>plan_type</c> / <c>page_size</c> / <c>next_key</c> 必填；<c>shop_appid</c>、<c>spu_item_condition</c>、<c>category</c>、<c>keyword</c>、<c>from_head_supplier_appid</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品列表条目（<c>product_id</c> / <c>shop_appid</c> / <c>head_supplier_appid</c>）与下一页参数。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/getshop/api_getpromoteproductlist.html"/></para>
    /// <para>
    /// 官方业务限制：①<c>page_size</c> 一页获取多少个商品，最大 <c>20</c>（本域唯一的 20 上限商品列表）；
    /// ②<c>plan_type</c> 见 <see cref="ChannelsPromoterPlanTypes"/>；③<c>category</c>「商品类目查询条件（<b>定向计划不支持</b>）」；
    /// ④<c>from_head_supplier_appid</c>「来源机构筛选，当商品是机构公开或定向时有效」；
    /// ⑤<see cref="ChannelsPromoterSpuItemCondition"/> 各区间单位由官方逐字段标注：售卖价与 <c>selling_price_range</c> 为「分」，
    /// 服务费率与佣金率区间为「10 万分之」，保障标 <c>flags</c> 见 <see cref="ChannelsPromoterProductFlags"/>（其中 <c>2</c> 品牌官方标注已废弃）。
    /// </para>
    /// <para>区间子结构 <see cref="ChannelsPromoterPromotionRange"/>（<c>min</c>/<c>max</c>）由 <c>spu_item_condition</c> 内八个区间字段共用——官方八张子表逐字段同名同型。</para>
    /// <para>
    /// 官方参数表矛盾照录：①列表条目 <c>product_id</c> 为 string，而详情端点 <c>get_promote_product_detail</c> 的 <c>product_id</c> 为 number；
    /// ②<c>category.category_id</c> 参数表为 number，但官方示例值 <c>11099070301657575487</c> 超出 <see cref="long"/> 范围，本仓取 <c>string?</c>；
    /// ③<c>category_ids_1/2/3</c> 官方类型列写作「-」且说明与示例在「单值 number / 数组」间不一致，本仓按示例取 <c>List&lt;string&gt;?</c>。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071003</c>（参数错误）/ <c>10071033</c>（shop_appid 错误）/ <c>10071044</c>（推客功能保证金不足）/ <c>10071048</c>（参数异常，请检查入参）/ <c>10071053</c>（商品不在架）。</para>
    /// <para>接口变更日志：2026 年 03 月 09 日新增 <c>from_head_supplier_appid</c> 字段。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_promote_product_list")]
    Task<ChannelsPromoterGetPromoteProductListResponse> GetPromoteProductListAsync(
        [Body] ChannelsPromoterGetPromoteProductListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取推客的某个商品的推广分佣比例（<c>promoter/get_sharer_product_commission_info</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>sharer_appid</c> 与 <c>product_id</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分佣比例（<c>commission_ratio</c>）与是否已设置该商品佣金率的标志（<c>is_set</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/getshop/api_getsharerproductcommissioninfo.html"/></para>
    /// <para>
    /// 官方参数表矛盾照录（两处，本仓均按参数表建模、不做区间校验）：
    /// ①<c>commission_ratio</c> 参数表说明为「范围为【100000 - 900000】，代表【10%-90%】」，而其代码示例返回 <c>10</c>，两者不可同时成立；
    /// ②本端点 <c>commission_ratio</c> 为 number，写入端点 <see cref="SetSharerProductCommissionInfoAsync"/> 的同类字段为 string。
    /// </para>
    /// <para>官方 <c>product_id</c> 说明写作「要设置的商品 id」（读取端点沿用写入端点的措辞），按原样照录。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/get_sharer_product_commission_info")]
    Task<ChannelsPromoterGetSharerProductCommissionInfoResponse> GetSharerProductCommissionInfoAsync(
        [Body] ChannelsPromoterGetSharerProductCommissionInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 设置推客的单个商品的分佣比例信息（<c>promoter/set_sharer_product_commission_info</c>）。
    /// </summary>
    /// <param name="request">设置请求（<c>sharer_appid</c> / <c>commission_ratio</c> / <c>product_id</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方仅返回 <c>errcode</c> / <c>errmsg</c>，故回 <see cref="ChannelsResponse"/>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/getshop/api_setsharerproductcommissioninfo.html"/></para>
    /// <para>
    /// 官方业务限制：<c>commission_ratio</c>「平台分佣时的分佣比例，范围为【100000 - 1000000】，代表【10%-100%】」，
    /// 官方参数表类型为 <b>string</b>（读取端点 <see cref="GetSharerProductCommissionInfoAsync"/> 为 number，且其区间上界写作 900000），
    /// 本仓按各自参数表原样建模并在 DTO remarks 照录。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10071048</c>（参数异常，请检查入参）。</para>
    /// <para>接口变更日志：2025 年 11 月 24 日新增错误码 <c>10071048</c>。</para>
    /// <para>适用范围：「联盟带货机构」账号类型；不支持云调用；不支持第三方平台调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/promoter/set_sharer_product_commission_info")]
    Task<ChannelsResponse> SetSharerProductCommissionInfoAsync(
        [Body] ChannelsPromoterSetSharerProductCommissionInfoRequest request,
        CancellationToken cancellationToken = default);
}
