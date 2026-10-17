// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.DataModels.Funds;
using Mud.Wechat.Channels.DataModels.League;

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「优选联盟」域 SDK（41 端点：商家侧带货者 4 + 商品操作 7 + 联盟带货机构资金 3 + 机构商品 6 + 机构商家合作 3 + 机构佣金单 2 + 机构橱窗与授权 6 + 机构商品订阅 3 + 机构达人佣金计划 2 + 机构帮卖供货 2 + 机构线索订单 2 + 机构直播保护 1）。
/// </summary>
/// <remarks>
/// <para>
/// <b>同域双视角（路径前缀即视角，不得合并）</b>：本域内混居两套官方视角——①<b>小店商家侧</b>
/// （<c>/channels/ec/league/{promoter,talent,item,promotion}/*</c>，店铺 <c>access_token</c>，第三方权限集 141）；
/// ②<b>联盟带货机构（headsupplier）侧</b>（<c>/channels/ec/league/headsupplier/*</c>，机构自身 <c>access_token</c>，
/// <b>不支持第三方平台代调用</b>）。两视角的字段与错误码各自成表，机构侧资金端点与 Funds 域「小店资金」
/// 分建 DTO（详见各方法 remarks）。
/// </para>
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

    /// <summary>
    /// 获取机构账户余额（<c>headsupplier/funds/balance/get</c>，联盟带货机构视角）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>可提现余额与待结算余额（<c>available_amount</c>/<c>pending_amount</c>，单位分）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/balance/api_getbalance.html"/>（2026-10-17 逐页核验）</para>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体参数表为<b>无</b>（官方注意事项原文「调用接口时传空的 json 串即可」）
    /// ⇒ SDK 不造请求体 DTO，守卫 CLG2 锁定本端点不得携带 <c>[Body]</c>。
    /// </para>
    /// <para>
    /// 官方适用范围：本接口支持「<b>联盟带货机构</b>」账号类型调用（机构自身凭据视角，与本域商家侧
    /// <c>/channels/ec/league/{promoter,item,talent,promotion}/*</c> 族不同）；<b>支持云调用</b>
    /// （<c>channels.ec.league.headsupplier.funds.balance.get</c>）；<b>不支持第三方平台调用</b>。
    /// </para>
    /// <para>官方错误码：本接口无特殊错误码（走通用错误码）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/funds/balance/get")]
    Task<ChannelsLeagueHeadSupplierGetBalanceResponse> GetHeadSupplierBalanceAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取机构资金流水列表（<c>headsupplier/funds/flowlist/get</c>，只回流水号，详情须逐单查）。
    /// </summary>
    /// <param name="request">查询请求（全部选填：<c>page</c>/<c>page_size</c>/<c>next_key</c>/<c>start_time</c>/<c>end_time</c>/<c>funds_type</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>流水单号列表与翻页标志（<c>flow_ids</c>/<c>has_more</c>/<c>next_key</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/balance/api_getfundsflowlist.html"/>（2026-10-17 逐页核验）</para>
    /// <para>
    /// 官方业务限制：①深翻页约束——<c>page * page_size &gt;= 5000</c> 时 <c>next_key</c> <b>必填</b>
    /// （官方参数表原文），SDK 不做本地拦截，越界由 <c>10024000</c> 表达；②<c>next_key</c> 仅在
    /// 「<c>page</c> 为上一页加一且 <c>page_size</c> 与上一页相同」时才生效；③<c>page_size</c> 不填默认 <c>10</c>；
    /// ④本接口列表<b>只返回流水单号</b>，金额与关联单据须经 <see cref="GetHeadSupplierFundsFlowDetailAsync"/> 逐单获取。
    /// </para>
    /// <para>官方枚举：<c>funds_type</c> 1 提现 / 2 分账 / 3 联盟技术服务费（2026-03-04 变更日志新增）/ 4 分账回退（2026-06-01 变更日志新增），见 <see cref="ChannelsLeagueHeadSupplierFundsTypes"/>。</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；<b>支持云调用</b>（<c>channels.ec.league.headsupplier.funds.flowlist.get</c>）；<b>不支持第三方平台调用</b>。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）。</para>
    /// <para>
    /// 响应侧复用裁决：本接口返回参数表（<c>flow_ids</c>/<c>has_more</c>/<c>next_key</c>）与小店商家侧
    /// <c>merchant/funds/flowlist/get</c> 逐字段一致 ⇒ 复用 Funds 域
    /// <c>ChannelsGetFundsFlowListResponse</c>，不分建（守卫 CLG5 锁类型同一）；请求侧因机构侧多
    /// <c>funds_type</c>、无 <c>transaction_id</c> ⇒ 分建。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/funds/flowlist/get")]
    Task<ChannelsGetFundsFlowListResponse> GetHeadSupplierFundsFlowListAsync(
        [Body] ChannelsLeagueHeadSupplierGetFundsFlowListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取机构资金流水详情（<c>headsupplier/funds/flowdetail/get</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>flow_id</c> 必填，经流水列表接口获取）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>流水信息（<c>funds_flow</c>：资金类型 / 金额 / 余额 / 记账时间 / 备注 / 关联订单号 / 关联提现单号）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/balance/api_getfundsflowdetail.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；<b>支持云调用</b>（<c>channels.ec.league.headsupplier.funds.flowdetail.get</c>）；<b>不支持第三方平台调用</b>。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10024008</c>（不存在该流水）。</para>
    /// <para>
    /// 分建与复用裁决：请求侧 <c>flow_id</c> 单字段与商家侧一致 ⇒ 复用 <c>ChannelsGetFundsFlowDetailRequest</c>；
    /// <c>funds_flow</c> 元素字段集机构侧含 <c>order_id</c>/<c>withdraw_id</c>、无 <c>flow_type</c>/
    /// <c>related_info_list</c>/<c>funds_type_desc</c> ⇒ 响应与元素类型分建（守卫 CLG5 锁两侧元素类型不同）。
    /// </para>
    /// <para>官方变更日志：2026-03-04 与 2026-06-01 分别新增 <c>funds_type</c> 枚举值 <c>3</c>、<c>4</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/funds/flowdetail/get")]
    Task<ChannelsLeagueHeadSupplierGetFundsFlowDetailResponse> GetHeadSupplierFundsFlowDetailAsync(
        [Body] ChannelsGetFundsFlowDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 机构添加合作商品（<c>headsupplier/cooperativeitem/add</c>，把选品广场的商品加入机构合作计划）。
    /// </summary>
    /// <param name="request">请求体（<c>head_supplier_item_link</c> 必填，<c>hidden_flag</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>机构合作计划 id（<c>cooperative_item_id</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/getshop/api_addcooperativeitem.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；<b>不支持云调用</b>；<b>不支持第三方平台调用</b>。</para>
    /// <para>官方业务限制：<c>head_supplier_item_link</c> 须取自「获取机构选品广场商品列表」或「获取合作商品列表」回包，形如 <c>weixinstorehs/123456</c>；<c>hidden_flag</c> 控制是否在机构主页隐藏。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10024024</c>（该计划未在待选品状态）/ <c>10024025</c>（商品计划不在上架中）—— 后两枚为官方变更日志 2025-11-24 新增。</para>
    /// <para>官方注意事项：本接口已升级迭代，请开发者尽快适配。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/cooperativeitem/add")]
    Task<ChannelsLeagueHeadSupplierAddCooperativeItemResponse> AddHeadSupplierCooperativeItemAsync(
        [Body] ChannelsLeagueHeadSupplierAddCooperativeItemRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取合作商品列表（<c>headsupplier/cooperativeitem/list/get</c>，机构在选品广场添加过的商品）。
    /// </summary>
    /// <param name="request">查询请求（<c>commission_type</c>/<c>page_size</c>/<c>next_key</c> 必填，<c>listing_time_range</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>合作商品列表（<c>list</c>：<c>product_id</c>/<c>cooperative_item_id</c>/<c>head_supplier_item_link</c>）与下一页 <c>next_key</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/getshop/api_getcooperativeitemlist.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；<b>不支持云调用</b>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方业务限制：①<b>翻页为 <c>next_key</c> 游标式</b>（第一页传空串，之后写入上一页返回值），<c>page_size</c> 最大 <c>20</c>；
    /// ②<c>commission_type</c> 语义 0 商家指定达人佣金 / 1 机构指定达人佣金，与推广详情的 <c>plan_type</c>（1 定向 / 2 公开）是<b>两套不同枚举</b>；
    /// ③推客商品的获取不走本接口，官方要求改用「推客带货 / 商品获取 / 获取可推广的商品 id 列表」。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）。</para>
    /// <para>官方变更日志：2025-11-03 <c>Res.list</c> 的 <c>product_id</c> 类型由 <c>string</c> 变更为 <c>number</c>；本仓按新类型建模。</para>
    /// <para>旧接口 <c>headsupplier/item/list/get</c>（获取合作商品列表-旧）已从官方目录树移除 ⇒ 本仓<b>不实现</b>，由守卫 CLG7 锁定零声明。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/cooperativeitem/list/get")]
    Task<ChannelsLeagueHeadSupplierGetCooperativeItemListResponse> GetHeadSupplierCooperativeItemListAsync(
        [Body] ChannelsLeagueHeadSupplierGetCooperativeItemListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商品推广参数详情（<c>headsupplier/item/promotiondetail/get</c>，佣金率、推广链接等推广参数）。
    /// </summary>
    /// <param name="request">查询请求（<c>head_supplier_item_link</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>推广详情（<c>item</c>：商品与店铺 / 供货来源 / 佣金信息 / 供货机构信息 / 合作信息）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/getshop/api_getitempromotiondetail.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；<b>不支持云调用</b>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方业务限制：①本接口只返回<b>推广参数</b>，商品基础信息（图、类目等）须走 <c>productdetail/get</c>；
    /// ②<c>commission_type = 1</c>（机构指定达人佣金）时该计划无达人佣金率，须先创建子计划（<c>subitem/add</c>）才能上橱窗；
    /// ③<c>link</c>（带货链接）仅在「商家指定达人佣金」且机构商品上架中时返回；
    /// ④佣金与服务费率取值 <c>[0, 1000000]</c>，<c>1000000</c> 表示全额。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10024025</c>（商品计划不在上架中，2025-12-19 新增）。</para>
    /// <para>官方变更日志：2025-11-03 <c>head_supplier_info</c> 新增 <c>head_img</c>；2025-12-19 新增错误码 <c>10024025</c>；2026-02-09 更新 <c>link</c> 描述。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/item/promotiondetail/get")]
    Task<ChannelsLeagueHeadSupplierGetItemPromotionDetailResponse> GetHeadSupplierItemPromotionDetailAsync(
        [Body] ChannelsLeagueHeadSupplierGetItemPromotionDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取机构选品广场商品列表（<c>headsupplier/selectionproducts/list/get</c>，拉取属于机构的联盟公开 / 定向商品池）。
    /// </summary>
    /// <param name="request">查询请求（<c>plan_type</c> 必填：1 定向计划 / 2 公开计划；可带属性、类目、关键词筛选与游标翻页）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品列表（<c>product_list</c>：<c>product_id</c> / <c>shop_appid</c> / <c>head_supplier_item_link</c>）与下一页 <c>next_key</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/getshop/api_listselectionproducts.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；<b>不支持云调用</b>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方业务限制：①翻页为 <c>next_key</c> 游标式（第一页传空），<c>page_size</c> 最大 <c>20</c>；
    /// ②<b>类目筛选（<c>category</c>）定向计划不支持</b>；③<b><c>create_time_range</c> 与 <c>keyword</c> 不能共用</b>；
    /// ④<c>spu_item_condition</c> 内六个区间对象字段集完全同形（<c>min</c>/<c>max</c>），服务费率与佣金率区间单位为 <b>10 万分之</b>；
    /// ⑤<c>plan_type</c>（1 定向 / 2 公开）与「获取合作商品列表」的 <c>commission_type</c>（0/1）是两套不同枚举，本域 <c>commission_type</c> 又为 1/2 ⇒ 常量类分建。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）。</para>
    /// <para>
    /// 官方变更日志：2026-03-09 新增 <c>commission_type</c>（佣金类型查询条件）与 <c>from_head_supplier_appid</c>（来源机构筛选）
    /// ——但同日核验证的请求参数表内<b>无</b> <c>from_head_supplier_appid</c> ⇒ 本仓照参数表建模、不回填日志提及字段。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/selectionproducts/list/get")]
    Task<ChannelsLeagueHeadSupplierGetSelectionProductsResponse> GetHeadSupplierSelectionProductsAsync(
        [Body] ChannelsLeagueHeadSupplierGetSelectionProductsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商品基础详情（<c>headsupplier/productdetail/get</c>，机构视角查询合作小店的完整商品信息）。
    /// </summary>
    /// <param name="request">查询请求（<c>shop_appid</c> 与 <c>product_id</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品详情（<c>item</c>：所属小店 / 商品 ID / <c>product_info</c> 完整商品信息 / <c>shop</c> 店铺信息）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/shop/api_getproductdetail.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；<b>不支持云调用</b>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方业务限制：①本接口是「机构查别人的商品」，必须同时给商品<b>所属小店 appid</b>，与小店商家侧 <c>product/get</c>（本店令牌 + <c>product_id</c>）
    /// 是两套不同契约；②返回的 <c>freight_info</c> 是完整物流模板（含条件包邮、分地区计费、不发货区域），与商家侧 <c>express_info</c>（<c>template_id</c> 三元组）字段集完全不同 ⇒ 分建 DTO；
    /// ③<c>is_hot_brand</c> 官方已废弃，改看 <c>is_brand_good</c>。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10024003</c>（不合法的 appid）/ <c>10024004</c>（不存在该商品）/ <c>10024043</c>（商品不属于该店铺，2026-07-10 新增）。</para>
    /// <para>
    /// 官方文档自身差异（照录不改）：返回参数表根字段为 <c>item</c>、代码示例写 <c>product</c>；
    /// 参数表有 <c>freight_info</c>、代码示例写 <c>express_info</c> + <c>statistical_data</c> ⇒ 本仓<b>以参数表为权威</b>。
    /// </para>
    /// <para>官方变更日志：2026-02-04 新增 <c>cats_v2</c> / <c>extra_service</c> / <c>product_spring_festival_delivery_config</c>；2026-03-09 新增 <c>freight_info</c> 等一批字段与「发货时间期限」枚举；2026-05-09 <c>is_hot_brand</c> 废弃、新增 <c>is_brand_good</c>；2026-09-16 sku 级新增用户普惠价。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/productdetail/get")]
    Task<ChannelsLeagueHeadSupplierGetProductDetailResponse> GetHeadSupplierProductDetailAsync(
        [Body] ChannelsLeagueHeadSupplierGetProductDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商品活动补贴信息（<c>headsupplier/product/getactivity</c>，品牌日 / 通用货补活动的活动价与平台补贴）。
    /// </summary>
    /// <param name="request">查询请求（<c>product_id</c> 与 <c>shop_appid</c> 均必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>是否存在活动（<c>has_activity</c>）与活动列表（<c>product_activity_list</c>：活动价 / 平台补贴金额与件数 / 活动子类型 / 结束时间 / 库存模式与数量）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/shop/api_league_get_product_activity_info.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；<b>不支持云调用</b>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方业务限制：<c>activity_sub_type</c> <b>目前仅支持「3 品牌日」「4 通用货补」</b>，其他活动类型不返回；
    /// <c>stock_mode</c> 为 <c>0</c> 活动库存 / <c>1</c> 商品自营库存（2026-03-27 官方优化该字段描述）；金额字段单位均为分。
    /// </para>
    /// <para>官方错误码：<c>-1</c>（system error）/ <c>10024000</c>（参数错误）/ <c>10024009</c>（不合法的商品 ID）/ <c>10024012</c>（获取店铺信息失败）。</para>
    /// <para>官方变更日志：2026-03-09 上线；2026-03-27 优化库存描述。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/product/getactivity")]
    Task<ChannelsLeagueHeadSupplierGetProductActivityResponse> GetHeadSupplierProductActivityAsync(
        [Body] ChannelsLeagueHeadSupplierGetProductActivityRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商家合作详情（<c>headsupplier/shop/get</c>，机构视角的单个小店基础信息 + 合作统计数据 + 合作状态）。
    /// </summary>
    /// <param name="request">查询请求（<c>appid</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>小店详情（<c>shop_detail</c>：基础信息、统计数据、合作状态、开始合作时间戳）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/headsupplier/api_getshop.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；支持云调用 <c>channels.ec.league.headsupplier.shop.get</c>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方业务限制：①返回的 <c>data_info</c> 中 <c>gmv</c> 与 <c>unsettle_amount</c> 单位为分；
    /// ②列表接口 <c>shop/list/get</c> 的列表项只有 <c>base_info</c> + <c>status</c>（无统计数据与合作时间）⇒ 两形态分建、
    /// <c>base_info</c> 同形复用；③请求体参数表只有 <c>appid</c>，代码示例残留 <c>page_size</c>（属文档陈旧）⇒ 不建模。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10024007</c>（与商家无关联关系）。</para>
    /// <para>官方变更日志：2025-11-24 返回根字段 <c>shop_list</c> 修正为 <c>shop_detail</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/shop/get")]
    Task<ChannelsLeagueHeadSupplierGetShopResponse> GetHeadSupplierShopAsync(
        [Body] ChannelsLeagueHeadSupplierGetShopRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 分页获取合作商家列表（<c>headsupplier/shop/list/get</c>，机构视角的合作小店列表）。
    /// </summary>
    /// <param name="request">查询请求（<c>page_size</c> 必填且不超过 30，翻页传 <c>next_key</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商家列表（<c>shop_list</c>：基础信息 + 合作状态）、翻页上下文与 <c>has_more</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/headsupplier/api_getshoplist.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；支持云调用 <c>channels.ec.league.headsupplier.shop.list.get</c>；<b>不支持第三方平台调用</b>。</para>
    /// <para>官方业务限制：<c>page_size</c> 必填且不超过 30；<c>next_key</c> 由上次请求返回、仅用于顺序翻页。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/shop/list/get")]
    Task<ChannelsLeagueHeadSupplierGetShopListResponse> GetHeadSupplierShopListAsync(
        [Body] ChannelsLeagueHeadSupplierGetShopListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新机构合作关系（<c>headsupplier/headsupplierrelation/update</c>：邀请 / 通过 / 拒绝 / 取消邀请 / 取消合作）。
    /// </summary>
    /// <param name="request">更新请求（<c>object_type</c> 必填；机构侧须传 <c>to_head_supplier_appid</c> + <c>hs_update_type</c>，店铺侧须传 <c>to_shop_appid</c> + <c>shop_update_type</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方返回体仅 <c>errcode</c> / <c>errmsg</c>，复用 <see cref="ChannelsResponse"/>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/headsupplier/api_update_hs_relation.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；<b>不支持云调用</b>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方业务限制：<c>object_type</c> 须为 <c>1</c>（机构）或 <c>2</c>（店铺），两套必填参数按对象类型分流
    /// （<c>hs_update_type</c> 值域 1-5、<c>shop_update_type</c> 值域 1-3）；返回体无业务字段。
    /// </para>
    /// <para>官方错误码：<c>-1</c>（系统错误）/ <c>10024000</c>（参数错误）/ <c>10024035</c>（已经合作，无需重复申请）/ <c>10024037</c>（当前关系状态不允许该操作）/ <c>10024040</c>（已经接受该团长的邀请，无需重复操作）。</para>
    /// <para>官方变更日志：2026-03-09 上线。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/headsupplierrelation/update")]
    Task<ChannelsResponse> UpdateHeadSupplierRelationAsync(
        [Body] ChannelsLeagueHeadSupplierUpdateRelationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取佣金单列表（<c>headsupplier/order/list/get</c>，按时间范围 / 订单号 / 视频号 / 推客翻页捞取佣金单标识）。
    /// </summary>
    /// <param name="request">查询请求（<c>page_size</c> 必填且不超过 30；<c>create_time_range</c> 与 <c>update_time_range</c> 必填一个且互斥）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>佣金单标识列表（<c>list</c>：订单号 + skuid）、翻页上下文与 <c>has_more</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/API/getorderlist.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；支持云调用 <c>channels.ec.league.headsupplier.order.list.get</c>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方业务限制：①<b>创建时间范围与更新时间范围必填一个</b>（填了其一则不填另一）；②填 <c>order_id</c> 后其他过滤参数无效；
    /// ③本接口只返回 <c>order_id</c> + <c>sku_id</c>，详情须再调 <c>order/get</c>；④时间范围为秒级时间戳。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10024001</c>（不合法的 finder_id）/ <c>10024003</c>（不合法的 appid）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/order/list/get")]
    Task<ChannelsLeagueHeadSupplierGetOrderListResponse> GetHeadSupplierOrderListAsync(
        [Body] ChannelsLeagueHeadSupplierGetOrderListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取佣金单详情（<c>headsupplier/order/get</c>：订单、商品、买卖双方与多层分佣明细）。
    /// </summary>
    /// <param name="request">查询请求（<c>order_id</c> 与 <c>sku_id</c> 均必填、均为 number）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>佣金单结构（官方根字段名为 <c>commssion_order</c>，照抄官方漏字母 i 的拼写）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/API/getorder.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；支持云调用 <c>channels.ec.league.headsupplier.order.get</c>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方业务限制：①本接口是「联盟带货机构」视角的分佣单，与小店商家侧 <c>order/get</c>（交易单）不是同一契约；
    /// ②<b>两张同名 <c>status</c> 枚举表</b>：佣金单状态（20/100/200）与订单状态（10/12/17/20/21/30/100/200/250）⇒ 分建常量类；
    /// ③费率类字段（<c>ratio</c> / <c>service_ratio</c>）区间 <c>[0, 1000000]</c>（十万分比）、金额类字段单位为分；
    /// ④帮卖 / 推客 / 供货机构三处 <c>*_agency_info</c> 字段集完全同形 ⇒ 复用一支类型；
    /// ⑤<strong>推广信息内层的 <c>finder_info</c> / <c>talent_info</c> 与外层同名字段字段集不同</strong>（无 ratio/amount）⇒ 各建一支类型。
    /// </para>
    /// <para>官方文档自身差异（照录不改）：返回根字段 <c>commssion_order</c> 拼写漏字母 i；参数表定 <c>order_id</c> 为 string、<c>sku_id</c> 为 number，代码示例把两者都写成字符串；参数表 <c>order_info.status</c> 在示例里写 <c>order_status</c>；机构三处说明均复制为「帮卖机构昵称」⇒ 本仓<b>以参数表为权威</b>。</para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10024006</c>（不存在该佣金单）/ <c>10024034</c>（订单已换 sku，当前 sku 不存在分佣单）。</para>
    /// <para>官方变更日志：2026-04-23 更新 <c>service_amount</c> 描述、新增 <c>platform_commission_status</c> / <c>service_total_amount</c>、更新 <c>status</c> 枚举值；2026-03-31 新增 <c>product_cnt</c>；2026-03-12 新增 <c>custom_token</c>；2026-03-06 新增 <c>platform_amount</c>；2026-02-26 新增错误码与错误信息。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/order/get")]
    Task<ChannelsLeagueHeadSupplierGetOrderResponse> GetHeadSupplierOrderAsync(
        [Body] ChannelsLeagueHeadSupplierGetOrderRequest request,
        CancellationToken cancellationToken = default);
    /// <summary>
    /// 添加机构商品到橱窗（<c>headsupplier/window/add</c>，机构把优选联盟商品挂到达人橱窗）。
    /// </summary>
    /// <param name="request">请求（<c>product_id</c> 必填；机构类商品须传 <c>product_link</c>；达人定位走 <c>openfinderid</c> 与 <c>opentalentid</c> 二选一）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方返回体仅 <c>errcode</c> / <c>errmsg</c>，复用 <see cref="ChannelsResponse"/>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/window/api_addwindow.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；支持云调用 <c>channels.ec.league.headsupplier.window.add</c>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方业务限制：①<c>product_link</c> 取「获取合作商品列表」中<b>商家指定达人佣金</b>类型的 <c>head_supplier_item_link</c>，
    /// 「机构指定达人佣金」类型须先创建子计划再取其 <c>head_supplier_item_link</c>；
    /// ②<c>openfinderid</c>（视频号橱窗）与 <c>opentalentid</c>（带货助手橱窗）二选一；③<c>finder_id</c> 官方标注后续废弃；
    /// ④返回体无业务字段。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10024001</c>（不合法的 finder_id）/ <c>10024003</c>（不合法的 appid）/ <c>10024009</c>（不合法的 product_id）/ <c>10024010</c>（橱窗操作失败）/ <c>10024011</c>（未获得橱窗授权）/ <c>10024021</c>（橱窗上架商品失败，官方同值两条描述：含「当前视频号未绑定带货者账号」）/ <c>10024023</c>（解析商品链接失败）。</para>
    /// <para>官方变更日志：2025-11-24 新增错误码 10024021；2026-02-25 更新 <c>product_link</c> 字段描述。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/window/add")]
    Task<ChannelsResponse> AddHeadSupplierWindowAsync(
        [Body] ChannelsLeagueHeadSupplierAddWindowRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询橱窗上机构商品列表（<c>headsupplier/window/getall</c>，按 <c>offset</c> 偏移翻页）。
    /// </summary>
    /// <param name="request">查询请求（<c>offset</c> 必填从 0 开始，<c>page_size</c> 默认 100、最大 500，需总数时传 <c>need_total_num</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品概要列表（<c>list</c>：机构 appid + 机构商品 ID + 橱窗商品 ID + 是否可操作）、<c>next_offset</c>、<c>has_more</c> 与 <c>total_num</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/window/api_getallwindow.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；支持云调用 <c>channels.ec.league.headsupplier.window.getall</c>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方业务限制：①本族翻页是 <c>offset</c> / <c>next_offset</c>，<b>与机构侧其余列表接口的 <c>next_key</c> 形态不同</b>；
    /// ②<c>is_get_all=true</c> 返回达人橱窗<b>全部</b>商品（含其它机构与达人自加商品），只想看本机构商品时不得置真；
    /// ③移除操作的权限校验按「商品归属 + 添加操作者」判定（官方注意事项表），故列表项带 <c>is_operation_allowed</c>。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10024001</c>（不合法的 finder_id）/ <c>10024003</c>（不合法的 appid）/ <c>10024009</c>（不合法的 product_id）/ <c>10024010</c>（橱窗操作失败）/ <c>10024011</c>（未获得橱窗授权）/ <c>10024012</c>（不合法的参数，查询不到对应账号，2025-11-24 新增）。</para>
    /// <para>官方变更日志：2025-11-24 新增错误码 10024012；另官方正文标注 2025-02 起支持查询达人全部橱窗商品。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/window/getall")]
    Task<ChannelsLeagueHeadSupplierGetWindowListResponse> GetHeadSupplierWindowListAsync(
        [Body] ChannelsLeagueHeadSupplierGetWindowListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询橱窗上机构商品详情（<c>headsupplier/window/getdetail</c>，橱窗视角的商品快照）。
    /// </summary>
    /// <param name="request">查询请求（达人定位 <c>openfinderid</c> / <c>opentalentid</c> 二选一；商品定位 <c>product_id</c> / <c>product_window_id</c> 二选一，官方推荐后者）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品所有信息（<c>product_detail</c>：所属小店 appid、商品 id 与标题 / 副标题 / 主图 / 详情 / 新旧类目树 / 推广链接）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/window/api_getwindowdetail.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；支持云调用 <c>channels.ec.league.headsupplier.window.getdetail</c>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方业务限制：①本接口是<b>橱窗快照</b>，字段集远小于 <c>headsupplier/productdetail/get</c>（无 sku / 物流 / 服务保障）⇒ 分建 DTO；
    /// ②官方启用新多级类目树：<c>cats</c> 为旧三级结构、<c>cats_v2</c> 为多级结构，过渡期两者兼容返回，官方建议切换 <c>cats_v2</c>；
    /// ③本接口 <c>cats</c> / <c>cats_v2</c> 元素<b>只有 <c>cat_id</c></b>（机构商品详情的 <c>cats_v2</c> 另含 <c>cat_name</c>）⇒ 复用 Product 域类目类型。
    /// </para>
    /// <para>官方错误码：<c>2</c>（该商品已移除）/ <c>3</c>（该商品已永久删除）/ <c>10024000</c>（参数错误）/ <c>10024001</c>（不合法的 finder_id）/ <c>10024003</c>（不合法的 appid）/ <c>10024009</c>（不合法的 product_id）/ <c>10024010</c>（橱窗操作失败）/ <c>10024011</c>（未获得橱窗授权，2025-11-21 新增）/ <c>10024020</c>（橱窗不存在该商品）。</para>
    /// <para>官方变更日志：2025-11-21 新增错误码 10024011。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/window/getdetail")]
    Task<ChannelsLeagueHeadSupplierGetWindowProductDetailResponse> GetHeadSupplierWindowProductDetailAsync(
        [Body] ChannelsLeagueHeadSupplierGetWindowProductDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 从橱窗移除机构商品（<c>headsupplier/window/remove</c>）。
    /// </summary>
    /// <param name="request">请求（达人定位二选一；商品定位 <c>product_id</c> / <c>product_window_id</c> 二选一）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方返回体仅 <c>errcode</c> / <c>errmsg</c>，复用 <see cref="ChannelsResponse"/>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/window/api_removewindow.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；支持云调用 <c>channels.ec.league.headsupplier.window.remove</c>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方业务限制：①请求体无 <c>appid</c>（与 <c>window/getdetail</c> 不同）；②权限面见 <c>window/getall</c> 的「商品归属 + 添加操作者」判定，
    /// 机构商品若由达人自行添加则不可移出；③返回体无业务字段。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10024010</c>（橱窗操作失败）/ <c>10024011</c>（未获得橱窗授权）/ <c>10024020</c>（橱窗不存在该商品）等，同族橱窗错误码。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/window/remove")]
    Task<ChannelsResponse> RemoveHeadSupplierWindowAsync(
        [Body] ChannelsLeagueHeadSupplierRemoveWindowRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取带货达人橱窗授权链接（<c>headsupplier/windowauth/get</c>，机构引导达人完成橱窗授权）。
    /// </summary>
    /// <param name="request">请求（<c>finder_id</c> / <c>openid</c> / <c>talent_appid</c> 三选一）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>授权链接信息（<c>auth_info</c>：授权链接、小程序 name / appid / 路径）与 <c>openfinderid</c> / <c>opentalentid</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/window/api_getwindowauth.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；支持云调用 <c>channels.ec.league.headsupplier.windowauth.get</c>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方业务限制：①三选一（<c>openid</c> 须为已在机构端绑定小程序的用户）；
    /// ②官方参数表把三行都标「必填 = 是」，实为三选一 ⇒ 本仓全部建模为可空；
    /// ③返回体的 <c>openfinderid</c> / <c>opentalentid</c> 在参数表位于根、代码示例只出现 <c>opentalentid</c> ⇒ 以参数表为权威。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10024001</c>（不合法的 finder_id）/ <c>10024002</c>（达人已同意授权无需重复申请授权）/ <c>10024003</c>（不合法的 appid）/ <c>10024014</c>（该用户不存在对应视频号）/ <c>10024018</c>（未绑定小程序）/ <c>10024019</c>（不合法的 openid）/ <c>10024031</c>（该达人平台未开通商户号）。</para>
    /// <para>官方变更日志：2025-12-19 新增错误码 10024031、10024003。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/windowauth/get")]
    Task<ChannelsLeagueHeadSupplierGetWindowAuthResponse> GetHeadSupplierWindowAuthAsync(
        [Body] ChannelsLeagueHeadSupplierGetWindowAuthRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取带货达人橱窗授权状态（<c>headsupplier/windowauth/status/get</c>）。
    /// </summary>
    /// <param name="request">请求（<c>openfinderid</c> / <c>talent_appid</c> / <c>opentalentid</c> 三选一，另可传官方标注即将废弃的 <c>finder_id</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>授权状态（<c>window_auth_status</c>：0 未授权 / 1 已授权）与对应的 <c>openfinderid</c> / <c>opentalentid</c> / 达人昵称与头像。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/window/api_getwindowauthstatus.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；支持云调用 <c>channels.ec.league.headsupplier.windowauth.status.get</c>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方业务限制：①授权状态是 <c>number</c>（0/1）而非 boolean，字段名带 <c>status</c> ⇒ 建常量类；
    /// ②返回的达人昵称 / 头像（<c>nickname</c> / <c>head_img_url</c>）在代码示例中未出现，但参数表列出 ⇒ 以参数表为权威；
    /// ③本接口是 <c>windowauth/get</c> 的配套轮询面。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10024001</c>（不合法的 finder_id）/ <c>10024012</c>（不合法的参数，查询不到对应账号，2025-12-22 新增）。</para>
    /// <para>官方变更日志：2025-12-22 新增错误码 10024012。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/windowauth/status/get")]
    Task<ChannelsLeagueHeadSupplierGetWindowAuthStatusResponse> GetHeadSupplierWindowAuthStatusAsync(
        [Body] ChannelsLeagueHeadSupplierGetWindowAuthStatusRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取订阅的商品列表（<c>headsupplier/subscription/getsubscribe</c>，机构视角的主动 + 被动订阅商品）。
    /// </summary>
    /// <param name="request">请求（<c>page_size</c> 必填且不超过 <c>1000</c>；<c>filter</c> / <c>next_key</c> / <c>need_total_num</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>订阅商品信息列表（<c>subscribe_info_list</c>：<c>product_id</c> + <c>shop_appid</c>）、<c>total_num</c> 与翻页游标。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/shop/api_getsubscribeproduct.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；<b>不支持云调用</b>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方业务限制：①订阅商品分两类——<b>主动订阅</b>（调用 <see cref="SubscribeHeadSupplierProductAsync"/> 订阅的商品）
    /// 与<b>被动订阅</b>（优选联盟后台合作商品中所有已合作商品，含商家定向 / 商家公开 / 机构定向 / 机构普通四种计划）；
    /// ②<c>page_size</c> 上限 <c>1000</c>（本域其余列表接口上限为 20 / 100，勿套用）；
    /// ③官方 <c>filter</c> 子表把 <c>product_id</c> / <c>product_name</c> / <c>appid</c> 三行都标「必填 = 是」，
    /// 实为筛选条件 ⇒ 本仓全部建模为可空；
    /// ④<c>need_total_num</c> 的官方说明原文为「否需要返回符合条件的记录总数」（「是」字缺字，照录）。
    /// </para>
    /// <para>
    /// 官方返回体自相矛盾（照录）：<c>subscribe_info_list</c> 参数表类型为 <c>objarray</c>（对象数组），
    /// 但代码示例把它写成<b>单个对象</b> ⇒ 以参数表为权威建模为列表；<c>has_more</c> 参数表为 <c>boolean</c>、
    /// 示例值为字符串 <c>"false"</c> ⇒ 以参数表为准。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/subscription/getsubscribe")]
    Task<ChannelsLeagueHeadSupplierGetSubscribeProductsResponse> GetHeadSupplierSubscribeProductsAsync(
        [Body] ChannelsLeagueHeadSupplierGetSubscribeProductsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商品订阅（<c>headsupplier/subscription/subscribe</c>）。
    /// </summary>
    /// <param name="request">请求（<c>product_id</c> 必填，官方示例值 <c>10000207528319</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅含 <c>errcode</c> / <c>errmsg</c> 的通用响应（官方返回参数表无业务字段）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/shop/api_subscribeproduct.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；<b>不支持云调用</b>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方业务限制：订阅后，商品的<b>基础信息或计划信息</b>发生变化时会通过<b>回调</b>通知机构
    /// ⇒ 本端点与订阅回调是配套面；取消订阅见 <see cref="UnsubscribeHeadSupplierProductAsync"/>，
    /// 两端点请求体字段集完全同形（单 <c>product_id</c>）⇒ 共用一支请求 DTO。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（其他参数错误）/ <c>10024009</c>（不合法的商品 ID 或未加入团长推广）。</para>
    /// <para>官方变更日志：2025-11-21 修改错误码 <c>10024009</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/subscription/subscribe")]
    Task<ChannelsResponse> SubscribeHeadSupplierProductAsync(
        [Body] ChannelsLeagueHeadSupplierSubscribeProductRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消商品订阅（<c>headsupplier/subscription/unsubscribe</c>）。
    /// </summary>
    /// <param name="request">请求（<c>product_id</c> 必填，要取消订阅的商品 ID）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅含 <c>errcode</c> / <c>errmsg</c> 的通用响应（官方返回参数表无业务字段）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/shop/api_unsubscribeproduct.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；<b>不支持云调用</b>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方请求示例的 JSON 结尾多一个逗号（<c>"product_id": 10000207528319,</c>），为官方笔误 ⇒
    /// 字段集以参数表为准；与 <see cref="SubscribeHeadSupplierProductAsync"/> 同形，共用请求 DTO。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/subscription/unsubscribe")]
    Task<ChannelsResponse> UnsubscribeHeadSupplierProductAsync(
        [Body] ChannelsLeagueHeadSupplierSubscribeProductRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 配置达人佣金率（<c>headsupplier/subitem/add</c>，为「机构分配达人佣金」的商品分配达人佣金）。
    /// </summary>
    /// <param name="request">请求（<c>cooperative_item_id</c> 必填 + <c>ratio</c> / <c>ratio_ext</c> 二选一）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>机构配置达人佣金计划 ID（<c>sub_item_id</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/getshop/api_addsubitem.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；<b>不支持云调用</b>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方业务限制：①<b>两个佣金率字段二选一</b>——<c>ratio</c> 为整数佣金率 <c>[0, 90]%</c>、
    /// <c>ratio_ext</c> 为精确佣金率 <c>[0, 900000]</c> 且 <c>10000 = 1%</c>（配置含小数点佣金率时使用）；
    /// 官方参数表把两者都标「必填 = 是」⇒ 本仓建模为可空并在守卫 CLG5 锁定两者并存；
    /// ②分配完成后可把链接给到达人带货（列表见 <see cref="GetHeadSupplierSubItemListAsync"/>）；
    /// ③「商家指定达人佣金」的商品不走本接口，直接调获取商品推广参数详情。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10024025</c>（商品计划不在上架中）/ <c>10024026</c>（自由佣金计划比例重复）/ <c>10024027</c>（佣金比例超过限制）。</para>
    /// <para>官方变更日志：2026-03-23 <c>ratio_ext</c> 字段描述更新；2025-11-24 新增错误码 <c>10024025</c>、<c>10024026</c> 等。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/subitem/add")]
    Task<ChannelsLeagueHeadSupplierAddSubItemResponse> AddHeadSupplierSubItemAsync(
        [Body] ChannelsLeagueHeadSupplierAddSubItemRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取机构配置达人佣金计划列表（<c>headsupplier/subitem/list/get</c>）。
    /// </summary>
    /// <param name="request">请求（<c>cooperative_item_id</c> / <c>page_size</c>（最大 20）/ <c>next_key</c> / <c>need_qr_code</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>达人佣金计划列表（<c>list</c>：计划 ID / 计划链接 / 状态 / 三档佣金率 / 推广二维码短链）与下一页游标 <c>next_key</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/getshop/api_getsubitemlist.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；<b>不支持云调用</b>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方业务限制：①<b>返回体无 <c>has_more</c></b>（本域多数列表接口有）⇒ 翻页只靠 <c>next_key</c>，
    /// 守卫 CLG5 已锁该字段不得出现；②<c>page_size</c> 最大 <c>20</c>；
    /// ③<c>next_key</c> 官方标「必填 = 是」并注明第一页传空串 ⇒ 建模为可空字符串；
    /// ④<c>need_qr_code</c> 官方原文提示「如不确定需不需要则不填，因为会增加耗时」⇒ 建模为可空；
    /// ⑤<c>qr_code_url</c> 是推广二维码<b>对应短链</b>，官方要求自行转成二维码后方可扫码。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）。</para>
    /// <para>官方变更日志：2026-03-23 <c>ratio_ext</c> 字段描述更新；2026-03-09 新增 <c>need_qr_code</c> 请求字段与 <c>qr_code_url</c> 返回字段。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/subitem/list/get")]
    Task<ChannelsLeagueHeadSupplierGetSubItemListResponse> GetHeadSupplierSubItemListAsync(
        [Body] ChannelsLeagueHeadSupplierGetSubItemListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 机构给帮卖机构供货（<c>headsupplier/secondheadsupplieritem/add</c>，创建机构管理页面的「机构推广」）。
    /// </summary>
    /// <param name="request">请求（<c>cooperative_item_id</c> 与 <c>head_supplier_item_link</c> 必填其一 + <c>plan_type</c> + <c>service_ratio</c>，定向计划另填 <c>to_head_supplier_appid</c> / 起止时间）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅含 <c>errcode</c> / <c>errmsg</c> 的通用响应（官方返回参数表无业务字段，示例 <c>errmsg</c> 为 <c>success</c> 而非 <c>ok</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/getshop/api_add_secondheadsupplier_item.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；<b>不支持云调用</b>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方业务限制：①<b>仅商家定向的合作商品</b>才支持通过本接口提供给帮卖机构，商家公开 / 机构定向 / 机构公开均不允许（官方注意事项原文）；
    /// ②<c>plan_type</c>：<c>1</c> 定向计划 / <c>2</c> 公开计划，官方特别提示<b>设置定向计划之前要先设置公开计划</b>；
    /// ③<c>service_ratio</c> 是帮卖服务费率，单位为<b>百万分之 N</b>（<c>10000</c> 表示 1%），且数值需<b>能被 1000 整除</b>
    /// —— 与 <see cref="AddHeadSupplierSubItemAsync"/> 的 <c>ratio_ext</c>（万分之 N）<b>量纲不同</b>，勿互相套用；
    /// ④<c>cooperative_item_id</c> 填写时官方声明会<b>忽略</b> <c>head_supplier_item_link</c>；
    /// ⑤<c>start_time</c> / <c>end_time</c> 为秒级时间戳，仅定向计划可填。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误，解决方案「检查参数」）/ <c>10024003</c>（不合法的 appid）。</para>
    /// <para>官方变更日志：2025-12-30 更新 <c>plan_type</c> 字段描述；2025-12-19 新增本接口、新增错误码 <c>10024003</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/secondheadsupplieritem/add")]
    Task<ChannelsResponse> AddHeadSupplierHelpSellItemAsync(
        [Body] ChannelsLeagueHeadSupplierAddHelpSellItemRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 机构更新或删除已添加的推广计划（<c>headsupplier/secondheadsupplieritem/update</c>，对应后台「机构推广」改佣金率或结束推广）。
    /// </summary>
    /// <param name="request">请求（<c>plan_type</c> + <c>update_type</c> 必填，商品定位键 <c>cooperative_item_id</c> / <c>head_supplier_item_link</c> 必填其一，更新佣金率时填 <c>service_ratio</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅含 <c>errcode</c> / <c>errmsg</c> 的通用响应（官方返回参数表无业务字段）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/opentalent/getshop/api_update_secondheadsupplier_item.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；<b>不支持云调用</b>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方业务限制：①<c>update_type</c> 为<b>操作类别</b>而非布尔：<c>1</c> 更新佣金率 / <c>2</c> 结束推广，
    /// 官方声明 <c>service_ratio</c> 仅在「更新」情况下必填 ⇒ 建模为可空；
    /// ②<c>to_head_supplier_appid</c> 官方声明「定向计划的情况必填」⇒ 建模为可空；
    /// ③本接口是 <see cref="AddHeadSupplierHelpSellItemAsync"/> 的改/撤面，字段集<b>不含</b> 起止时间
    /// （与添加接口的差异点，守卫 CLG5 已锁）。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误，解决方案「检查参数」）。</para>
    /// <para>官方变更日志：2025-11-28 新增本 API。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/secondheadsupplieritem/update")]
    Task<ChannelsResponse> UpdateHeadSupplierHelpSellItemAsync(
        [Body] ChannelsLeagueHeadSupplierUpdateHelpSellItemRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 推客机构获取内容推广的自营商品订单详情（<c>headsupplier/clue/get</c>，即「线索订单」详情）。
    /// </summary>
    /// <param name="request">请求（<c>order_id</c> 与 <c>sku_id</c> 均为 <c>string</c> 且必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>线索订单结构（官方根字段名 <c>commssion_clue</c>）：订单号 / skuid / 起止时间戳 + <c>clue_detail</c>（商家 / 商品 / 订单 / 推客 / 买家信息）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/clue/api_getclue.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；<b>不支持云调用</b>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方业务语义：直播间等内容分销场景存在挂<b>自营商品</b>的情况，该情况<b>不给带货机构分佣</b>，
    /// 平台改为给出<b>销售订单线索</b>，达人和机构自行协商 ⇒ 本族与佣金单（<see cref="GetHeadSupplierOrderAsync"/>）是两套订单面。
    /// </para>
    /// <para>
    /// 官方文档自相矛盾（照录，均以参数表为权威）：①根字段名官方拼作 <c>commssion_clue</c>（少一个 <c>i</c>），
    /// 属性名按正确拼写命名、<c>JsonPropertyName</c> 照抄官方；②<b>参数表无根 <c>status</c></b>，代码示例却有
    /// <c>"status": 20</c> ⇒ 不建模（订单状态只按参数表落 <c>clue_detail.order_info.order_status</c>）；
    /// ③参数表买家信息键为 <c>buyer_info</c>、示例却写 <c>buyer</c> ⇒ 以参数表为准；
    /// ④<b>请求侧</b> <c>sku_id</c> 是 <c>string</c>，<b>返回体</b> <c>commssion_clue.sku_id</c> 参数表却标 <c>number</c>（示例又写字符串）⇒ 返回侧按参数表建为 <c>long</c>；
    /// ⑤官方「注意事项」给出订单状态枚举表（<c>10/20/21/30/100/200/250</c>），标题却写作「status 订单状态」。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误，请开发人员检查传递参数是否正确）/ <c>10024006</c>（不存在该线索订单）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/clue/get")]
    Task<ChannelsLeagueHeadSupplierGetClueResponse> GetHeadSupplierClueAsync(
        [Body] ChannelsLeagueHeadSupplierGetClueRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 推客机构获取内容推广的自营商品订单列表（<c>headsupplier/clue/list/get</c>，只回线索单键）。
    /// </summary>
    /// <param name="request">请求（<c>create_time_range</c>（必填）+ <c>page_size</c>（不超过 100），<c>appid</c> / <c>sharer_appid</c> / <c>next_key</c> 选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>线索订单列表项（<c>list</c>：<c>order_id</c> + <c>sku_id</c>）与翻页上下文 <c>next_key</c> / <c>has_more</c>。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/clue/api_getcluelist.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；<b>不支持云调用</b>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方业务限制：①<b>时间区间自 2026-05-22 起改为必填</b>（官方变更日志原文「调整查询时间区间为必填」）
    /// ⇒ 本仓把 <c>create_time_range</c> 建模为可空引用类型但文档注释标明必填，且不为其造校验（保持 SDK 透传）；
    /// ②列表项 <c>order_id</c> 参数表类型为 <c>number</c>，而详情接口请求侧为 <c>string</c>、示例又写字符串
    /// ⇒ 按参数表建为 <c>long</c>，与佣金单列表的 <c>string</c> <c>order_id</c> <b>不同形</b>（守卫 CLG5 锁定差异，防误共用）；
    /// ③时间区间形态与本域其它接口共用 <see cref="ChannelsLeagueHeadSupplierTimeRange"/>；
    /// ④详情须用 <see cref="GetHeadSupplierClueAsync"/> 逐单拉取。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误，请开发人员检查传递参数是否正确）/ <c>10024003</c>（不合法的 appid）。</para>
    /// <para>官方变更日志：2026-05-22 调整查询时间区间为必填。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/clue/list/get")]
    Task<ChannelsLeagueHeadSupplierGetClueListResponse> GetHeadSupplierClueListAsync(
        [Body] ChannelsLeagueHeadSupplierGetClueListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取直播大场保护的提报二维码（<c>headsupplier/liveprotection/getqrcode</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>提报二维码 URL（<c>qrcode_url</c>，官方示例为 <c>https://mp.weixin.qq.com/</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/leagueheadsupplier/api/sharer/live/api_getliveprotectionqrcode.html"/>（2026-10-17 逐页核验）</para>
    /// <para>官方适用范围：「联盟带货机构」账号类型；<b>不支持云调用</b>；<b>不支持第三方平台调用</b>。</para>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体参数表为<b>无</b>（示例请求体为 <c>{}</c>）
    /// ⇒ SDK 不造请求体 DTO，守卫 CLG2 锁定本端点不得携带 <c>[Body]</c>。
    /// </para>
    /// <para>
    /// 大场保护机制（官方原文口径）：针对高 GMV 的重点直播场次，平台对非安全底线的违规行为实行
    /// <b>「先直播后处置」</b>策略，直播期间暂缓执行处罚、结束后统一处理。
    /// </para>
    /// <para>
    /// 官方使用限制：①机构<b>近 30 天 GMV</b> 符合激励条件（统计数据 T+1 生效）—— 与错误码 <c>10024030</c>
    /// 的「近 7 天推客带货 GMV」<b>表述不一致</b>，两处均照录；②每个自然月内单个达人仅有 <b>1 次</b>报备资格，
    /// 已通过高星达人权益通道报备的不可重复使用本接口；③提报达人绑定的视频号需为合作机构<b>近 7 天 GMV 前 2 名</b>。
    /// </para>
    /// <para>官方错误码：<c>10024000</c>（参数错误）/ <c>10024029</c>（二维码生成失败）/ <c>10024030</c>（近 7 天推客带货 GMV 符合激励条件的机构才能使用提报能力）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/league/headsupplier/liveprotection/getqrcode")]
    Task<ChannelsLeagueHeadSupplierGetLiveProtectionQrCodeResponse> GetHeadSupplierLiveProtectionQrCodeAsync(
        CancellationToken cancellationToken = default);

}
