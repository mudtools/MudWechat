// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.DataModels.League;

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「优选联盟」域 SDK（11 端点：带货者 4 + 商品操作 7）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与「达人合作（合作管理）」「罗盘带货数据」概念严格区分</b>：本域是小店商家侧的
/// <b>优选联盟推广计划管理</b>（<c>/channels/ec/league/*</c>，小店 <c>access_token</c>，
/// 第三方代调用权限集 id 141，店铺须先开通优选联盟，未开通报 <c>10023007</c>）；
/// 达人数据看板为 Compass 域，企微「渠道二维码 / 联系我」与联盟无关。
/// </para>
/// <para>
/// <b>带货者语义（官方改版照录）</b>：官方自 2026-06 起将推广对象统一为「带货者」
/// （<c>talent</c>，带货者 appid），旧「达人 finder / promoter」字段已由变更日志声明删除
/// （如 <c>finder_ids</c> / <c>promoter_ids</c> / <c>last_buffer</c> 等）——本域字段以官方
/// <b>参数表</b>为准（talent 系命名），与官方代码示例中残留的旧字段名不一致处在各方法 remarks 照录。
/// </para>
/// <para>
/// <b>「type」一字两表</b>：请求侧「获取商品推广类别」（<c>type</c>：<c>100</c> 全部带货者计划 /
/// <c>300</c> 指定带货者计划，见 <see cref="ChannelsLeaguePlanQueryTypes"/>）与计划/佣金侧的
/// 「普通 / 定向 / 专属」三分类（值 1/2/3，见 <see cref="ChannelsLeaguePromotionTypes"/>）是
/// <b>两套不同枚举</b>，勿混用。
/// </para>
/// <para>
/// <b>MUD005 已知接受风险</b>：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "League", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsLeagueService
{
    /// <summary>
    /// 获取带货者详情信息（<c>promoter/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>talent_appid</c> 与 <c>talent_search_key</c> 均选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>带货者信息（<c>talent</c>：基础信息 / 数据信息 / 特征信息，含累计与近 30 日销售额、订单数、橱窗商品数、等级与拉黑状态）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/league/promoter/api_getpromoter"/></para>
    /// <para>
    /// 官方业务限制：①带货者等级枚举见 <see cref="ChannelsLeagueTalentLevels"/>（0 带货新人 ~ 5 五星带货者）；
    /// ②<see cref="ChannelsGetPromoterRequest.TalentSearchKey"/> 支持带货者 appid 和视频号 finder_id 两种搜索参数。
    /// </para>
    /// <para>官方错误码：<c>10023000</c>（参数错误）/ <c>10023001</c>（不合法的 finder_id）/ <c>10023007</c>（请先开通优选联盟）/ <c>10024003</c>（不合法的 appid）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>141</c>；账号类型须为「微信小店（需申请）」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/promoter/get")]
    Task<ChannelsGetPromoterResponse> GetPromoterAsync(
        [Body] ChannelsGetPromoterRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取带货者列表（<c>promoter/list/get</c>，商家带货者列表分页查询）。
    /// </summary>
    /// <param name="request">查询请求（<c>page_size</c> 必填且不超过 20；<c>page_index</c> 选填，下标从 1 开始）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>带货者总数、翻页标志与列表（<c>total_num</c>/<c>continue_flag</c>/<c>list</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/league/promoter/api_getpromoterlist"/></para>
    /// <para>官方业务限制：<c>page_size</c> 单页带货者数不超过 20；<c>page_index</c> 下标从 1 开始，默认为 1；
    /// <c>continue_flag</c> 为 false 表示已结束。</para>
    /// <para>官方错误码：<c>10023000</c>（参数错误）/ <c>10023007</c>（请先开通优选联盟）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>141</c>；账号类型须为「微信小店（需申请）」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/promoter/list/get")]
    Task<ChannelsGetPromoterListResponse> GetPromoterListAsync(
        [Body] ChannelsGetPromoterListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取带货者商品数据列表（<c>talent/getproductdatalist</c>，合作管理列表）。
    /// </summary>
    /// <param name="request">查询请求（<c>get_data_type</c>/<c>page_index</c>/<c>page_size</c>/<c>talent_appid</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品总数、翻页标志与商品数据列表（<c>total_num</c>/<c>continue_flag</c>/<c>list</c>，元素含 <c>spu_id</c> / 商品数据 / 带货者佣金信息）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/league/promoter/api_gettalentproductdata"/></para>
    /// <para>
    /// 官方业务限制：①<c>get_data_type</c> 获取的商品数据类型枚举见
    /// <see cref="ChannelsLeagueGetDataTypes"/>（1 近 30 天日成交商品数据 / 2 当前橱窗商品数据）；
    /// ②<c>page_size</c> 不超过 20；③佣金率 <c>ratio</c> 取值 <c>[0, 1000000]</c>，1000000 = 100%；
    /// ④佣金类型枚举见 <see cref="ChannelsLeaguePromotionTypes"/>（1 普通计划 / 2 定向计划 / 3 专属计划）。
    /// </para>
    /// <para>官方错误码：无特殊错误码（走通用错误码）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>141</c>；不支持云调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/talent/getproductdatalist")]
    Task<ChannelsGetTalentProductDataListResponse> GetTalentProductDataListAsync(
        [Body] ChannelsGetTalentProductDataListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 编辑带货者（<c>talent/update</c>，拉黑 / 取消拉黑）。
    /// </summary>
    /// <param name="request">编辑请求（<c>talent_appid</c>/<c>upd_talent_type</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/league/promoter/api_updatetalent"/></para>
    /// <para>官方业务限制：编辑操作类型枚举见 <see cref="ChannelsLeagueUpdTalentTypes"/>
    /// （1 拉黑带货者 / 2 取消拉黑带货者）；官方变更日志 2026-03-30 新增该接口。</para>
    /// <para>官方错误码：无特殊错误码（走通用错误码）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>141</c>；不支持云调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/talent/update")]
    Task<ChannelsResponse> UpdateTalentAsync(
        [Body] ChannelsUpdateTalentRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量新增联盟商品（<c>item/batchadd</c>，新增推广计划）。
    /// </summary>
    /// <param name="request">新增请求（<c>type</c>/<c>list</c> 必填，商品不可重复且数量不超过 20）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>逐商品添加结果列表（<c>result_info_list</c>：单商品错误码 / 推广计划 id / 商品 id / 推广失败带货者列表）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/league/item/api_batchadditem"/></para>
    /// <para>
    /// 官方业务限制：①「获取商品推广类别」枚举见 <see cref="ChannelsLeaguePlanQueryTypes"/>
    /// （100 全部带货者计划 / 300 指定带货者计划）；②商品列表 <c>product_id</c> 不可重复、数量不超过 20；
    /// ③推广佣金 <c>ratio</c> 取值 <c>[0, 90]</c>%；④普通/定向计划与专属计划互斥（<c>10023105</c>）。
    /// </para>
    /// <para>
    /// <b>官方文档矛盾照录</b>：请求体参数表字段为 <c>talent_appids</c>（带货者 appid 列表），
    /// 代码示例写作 <c>promoter_ids</c>；响应参数表字段为 <c>promotion_plan_id</c>，示例写作
    /// <c>info_id</c>/<c>ratio</c>——SDK 按参数表建模（官方变更日志 2026-06-17：删除
    /// finder_ids/promoter_ids/fail_promoter_ids/fail_finder_ids/info_id，新增
    /// talent_appids/promotion_plan_id/fail_talent_appids）。
    /// </para>
    /// <para>
    /// 官方错误码：<c>10023001</c> / <c>10023007</c> / <c>10024003</c>（不合法的 appid）/
    /// <c>10023100</c>（type 类型错误）/ <c>10023101</c>（不允许相同 product_id）/ <c>10023102</c>（操作数量超过限制）/
    /// <c>10023103</c>（不存在该商品）/ <c>10023104</c>（该小店商品未上架）/ <c>10023105</c>（普通/定向计划与专属计划互斥）/
    /// <c>10023106</c> / <c>10023107</c> / <c>10023108</c> / <c>10023109</c>（佣金数值非法）/ <c>10023110</c>（新增达人数量超过计划上限）/
    /// <c>10023111</c>（未达到准入标准）/ <c>10023112</c>（不存在合作关系）/ <c>10023113</c>（计划持续时间非法）/
    /// <c>10023114</c>（运营处罚）/ <c>10023115</c>（类目禁售）/ <c>10023117</c>（预售中）/ <c>10023121</c>（店铺新手村限制）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>141</c>；账号类型须为「微信小店（需申请）」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/item/batchadd")]
    Task<ChannelsBatchAddLeagueItemResponse> BatchAddLeagueItemAsync(
        [Body] ChannelsBatchAddLeagueItemRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取联盟推广计划详情（<c>item/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>type</c>/<c>product_id</c> 必填，<c>promotion_plan_id</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>推广计划信息（<c>item</c>：商品 id / 推广类别 / 推广状态 / 佣金 / 特殊推广信息 / 扩展信息）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/league/item/api_getitem"/></para>
    /// <para>
    /// 官方业务限制：①响应 <c>item.type</c>（普通推广商品 / 定向推广商品 / 专属推广商品）为
    /// <see cref="ChannelsLeaguePromotionTypes"/>，与请求侧 <see cref="ChannelsLeaguePlanQueryTypes"/>
    /// 是两套枚举；②推广状态枚举见 <see cref="ChannelsLeagueItemStatuses"/>
    /// （2026-07-15 新增 7 商品下架终止 / 8 计划过期终止 / 9 商品被封禁下架终止三个终止推广选项）；
    /// ③<see cref="ChannelsLeagueItemExtInfo.IsBanned"/> 标识商品是否被官方封禁。
    /// </para>
    /// <para>
    /// <b>官方文档矛盾照录</b>：响应 <c>exclusive_info</c> 参数表字段为
    /// <c>promotion_plan_id</c>/<c>begin_time</c>/<c>end_time</c>/<c>is_forerver</c>/<c>talent_num</c>，
    /// 代码示例写作 <c>info_id</c>/<c>finder_ids</c>/<c>finder_num</c>/<c>finder_info_list</c>；
    /// <c>ext_info</c> 参数表为 <c>is_banned</c>、示例为 <c>is_sale_forbidden</c>——SDK 均按参数表建模
    /// （官方变更日志 2026-06-17：新增 promotion_plan_id、删除 info_id）。
    /// </para>
    /// <para>
    /// 官方错误码：<c>10023000</c>（参数错误）/ <c>10023001</c>（不合法的 finder_id）/ <c>10023007</c>（请先开通优选联盟）/
    /// <c>10023100</c>（type 类型错误）/ <c>10023103</c>（不存在该商品）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>141</c>；账号类型须为「微信小店（需申请）」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/item/get")]
    Task<ChannelsGetLeagueItemResponse> GetLeagueItemAsync(
        [Body] ChannelsGetLeagueItemRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量新增联盟机构推广（<c>item/headsupplier/batchadd</c>，机构带货计划）。
    /// </summary>
    /// <param name="request">新增请求（<c>headsupplier_appid</c>/<c>list</c> 必填；元素须带 <c>ratio</c> 与 <c>service_ratio</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>逐商品添加结果列表（<c>result_info_list</c>：单商品错误码 / 商品 id）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/league/item/api_batchaddheadsupplieritem"/></para>
    /// <para>
    /// 官方业务限制：①<c>headsupplier_appid</c> 为机构开放唯一凭证（账号 ID），机构可在
    /// 「微信小店·联盟带货机构管理平台 - 设置 - 开放信息」中获得，须已完成机构开通且账号无异常状态；
    /// ②推广佣金 <c>ratio</c> 与推广服务费 <c>service_ratio</c> 取值均为 <c>[0, 90]</c>%；
    /// ③机构计划类型枚举见 <see cref="ChannelsLeagueHeadSupplierPlanTypes"/>
    /// （0 商家指定达人佣金 / 1 机构指定达人佣金【总包计划类型】）。
    /// </para>
    /// <para>
    /// 官方错误码：<c>10023000</c> / <c>10023007</c> / <c>10024003</c>（机构 appid 不存在）/
    /// <c>10023100</c>~<c>10023121</c>（与批量新增联盟商品同族，含 <c>10023101</c> 不允许相同 product_id、
    /// <c>10023102</c> 操作数量超过限制、<c>10023109</c> 佣金数值非法等）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>141</c>；不支持云调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/item/headsupplier/batchadd")]
    Task<ChannelsBatchAddHeadSupplierResponse> BatchAddHeadSupplierItemAsync(
        [Body] ChannelsBatchAddHeadSupplierItemRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取联盟商品列表（<c>item/list/get</c>，分页拉取本店联盟推广商品）。
    /// </summary>
    /// <param name="request">查询请求（<c>page_size</c> 必填且不超过 100；其余选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品信息列表、翻页标志与总数（<c>items</c>/<c>has_more</c>/<c>total_num</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/league/item/api_getitemlist"/></para>
    /// <para>
    /// 官方业务限制：①<c>page_size</c> 单页商品数不超过 100；②<b>接口限制了 <c>page_size × page_index ≤ 5000</c></b>；
    /// ③商品状态枚举见 <see cref="ChannelsLeagueItemStatuses"/>（与推广计划详情同值域）。
    /// </para>
    /// <para>
    /// <b>官方文档矛盾照录</b>：注意事项建议「命中限制时改用传 <c>last_buffer</c> 顺序翻页」，
    /// 但官方变更日志 2026-06-17 已删除 <c>last_buffer</c> 等字段、当前参数表无此入参——SDK 按参数表建模（无 last_buffer）。
    /// </para>
    /// <para>
    /// 官方错误码：<c>10023000</c>（参数错误）/ <c>10023001</c>（不合法的 finder_id）/ <c>10023007</c>（请先开通优选联盟）/
    /// <c>10023100</c>（type 类型错误）/ <c>10023103</c>（不存在该推广商品）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>141</c>；账号类型须为「微信小店（需申请）」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/item/list/get")]
    Task<ChannelsGetLeagueItemListResponse> GetLeagueItemListAsync(
        [Body] ChannelsGetLeagueItemListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新联盟推广计划信息（<c>item/upd</c>，编辑并上架 / 上架 / 下架）。
    /// </summary>
    /// <param name="request">更新请求（<c>type</c>/<c>product_id</c>/<c>promotion_plan_id</c>/<c>operate_type</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>推广计划 id 与更新失败达人列表（<c>promotion_plan_id</c>/<c>fail_talent_appids</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/league/item/api_upditem"/></para>
    /// <para>
    /// 官方业务限制：①更新操作类别枚举见 <see cref="ChannelsLeagueOperateTypes"/>
    /// （1 编辑并上架 / 2 下架 / 4 上架）；②<c>ratio</c> 操作为编辑时必填，取值 <c>[0, 90]</c>%；
    /// ③<c>exclusive_info</c> 内新增 / 删除推广达人列表各不超过 20 个。
    /// </para>
    /// <para>
    /// 官方错误码：<c>10023000</c> / <c>10023001</c> / <c>10023007</c> / <c>10023100</c>（type 类型错误）/
    /// <c>10023102</c> / <c>10023103</c> / <c>10023104</c> / <c>10023105</c>~<c>10023115</c> /
    /// <c>10023116</c>（operate_type 非法）/ <c>10023117</c> / <c>10023121</c>。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>141</c>；账号类型须为「微信小店（需申请）」。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/item/upd")]
    Task<ChannelsUpdateLeagueItemResponse> UpdateLeagueItemAsync(
        [Body] ChannelsUpdateLeagueItemRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商品推广计划列表（<c>promotion/plan/list</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>product_id</c>/<c>page_index</c>/<c>page_size</c> 必填，<c>promotion_plan_type</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>计划总数、翻页标志与计划列表（<c>total_num</c>/<c>continue_flag</c>/<c>list</c>，元素含 <c>promotion_plan_id</c>/<c>promotion_plan_type</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/league/item/api_listspupromotionplans"/></para>
    /// <para>
    /// 官方业务限制：①<c>page_size</c> 不超过 20；②推广计划类型枚举见
    /// <see cref="ChannelsLeaguePromotionPlanTypes"/>（100 全部带货者 / 200 圈选带货者 / 300 指定带货者）。
    /// </para>
    /// <para>官方错误码：无特殊错误码（走通用错误码）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>141</c>；不支持云调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/promotion/plan/list")]
    Task<ChannelsListSpuPromotionPlansResponse> ListSpuPromotionPlansAsync(
        [Body] ChannelsListSpuPromotionPlansRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商品推广带货者列表（<c>promotion/talent/list</c>，按推广计划拉取带货者）。
    /// </summary>
    /// <param name="request">查询请求（<c>product_id</c>/<c>page_index</c>/<c>page_size</c>/<c>promotion_plan_id</c> 必填，<c>talent_appid</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>带货者总数、翻页标志与列表（<c>total_num</c>/<c>continue_flag</c>/<c>list</c>，元素含基础信息 / 佣金信息 / 推广状态）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/league/item/api_listspupromotiontalents"/></para>
    /// <para>
    /// 官方业务限制：①<c>page_size</c> 单页带货者数不超过 10；②带货者推广状态枚举见
    /// <see cref="ChannelsLeagueTalentRelationStatuses"/>（1 推广中 / 4 已移除 / 10 已被拉黑）；
    /// ③佣金率 <c>ratio</c> 取值 <c>[0, 1000000]</c>，1000000 = 100%。
    /// </para>
    /// <para>官方错误码：无特殊错误码（走通用错误码）；官方变更日志 2026-06-11 新增该接口、2026-07-28 更新 page_size 描述。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>141</c>；不支持云调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/promotion/talent/list")]
    Task<ChannelsListSpuPromotionTalentsResponse> ListSpuPromotionTalentsAsync(
        [Body] ChannelsListSpuPromotionTalentsRequest request,
        CancellationToken cancellationToken = default);
}
