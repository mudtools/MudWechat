// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.DataModels.Compass;

namespace Mud.Wechat.Channels;

/// <summary>
/// 视频号（channels 生态）「罗盘」域 SDK（14 端点：罗盘商家版 <c>/channels/ec/compass/shop/*</c> 10 +
/// 罗盘达人版 <c>/channels/ec/compass/finder/*</c> 4）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与「直播」域概念严格区分</b>：本域是经营分析指标（电商概览 / 带货达人 / 商品明细 / 人群画像），
/// 直播原始数据（记录 / 预约 / 留资 / 大屏）走 <see cref="IChannelsLiveService"/>。
/// </para>
/// <para>
/// <b>双前缀并存（官方原文，照抄不得改写）</b>：商家版 10 端点走 <c>/channels/ec/compass/shop/*</c>
/// （凭据为微信小店商家 <c>access_token</c>，第三方代调用权限集 id 175）；达人版 4 端点走
/// <c>/channels/ec/compass/finder/*</c>（凭据为视频号助手 <c>access_token</c>，权限集 id 176）。
/// 同构端点（商家版/达人版）字段集与类型官方不一致（如商品明细 string vs number），各自落端点不合并。
/// </para>
/// <para>
/// <b>数值口径</b>：罗盘接口官方参数表的 number/string 标注与示例互相矛盾处照录建模
/// （详见各 DTO remarks），金额字段单位均为<b>分</b>。
/// </para>
/// <para>
/// <b>MUD005 已知接受风险</b>：微信小店/视频号助手官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Compass", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsCompassService
{
    /// <summary>
    /// 获取授权视频号列表（<c>getshopfinderauthorizationlist</c>，罗盘商家版）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>授权视频号 id 列表（<c>authorized_finder_id_list</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/compass/api_getshopfinderauthorizationlist"/></para>
    /// <para>官方契约：<b>POST</b> + 空请求体（官方示例请求体为 <c>{}</c>）。</para>
    /// <para>官方业务限制：本接口无特殊注意事项；不支持云调用；第三方平台代微信小店商家调用权限集 id 为 175；仅「微信小店」账号类型可调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/compass/shop/finder/authorization/list/get")]
    Task<ChannelsCompassShopFinderAuthorizationListResponse> GetShopFinderAuthorizationListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取带货达人列表（<c>getshopfinderlist</c>，罗盘商家版，仅返回当日有带货数据的达人）。
    /// </summary>
    /// <param name="request">查询请求（<c>ds</c> 必填，格式 YYYYMMDD；<c>version</c> 选填，填 <c>v2</c> 会返回带货者身份的 id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>视频号列表（<c>finder_list</c>：视频号 id/昵称、成交数据、带货者与公众号信息）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/compass/api_getshopfinderlist"/></para>
    /// <para>
    /// 官方业务限制：①本接口无特殊注意事项；②<b>仅返回当日有带货数据的达人</b>（官方原文）；
    /// ③不支持云调用；④第三方平台代调用权限集 id 为 175；⑤仅「微信小店」账号类型可调用。
    /// </para>
    /// <para>
    /// <b>官方文档矛盾照录</b>：成交数据 <c>data</c> 子表类型标注为 number，官方示例为字符串
    /// （如 <c>"pay_gmv": "1"</c>）——SDK 按参数表（number）建模。
    /// </para>
    /// <para>官方错误码：<c>10021069</c>（请求参数日期格式有误）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/compass/shop/finder/list/get")]
    Task<ChannelsCompassShopFinderListResponse> GetShopFinderListAsync(
        [Body] ChannelsCompassShopFinderListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取带货数据概览（<c>getshopfinderoverall</c>，罗盘商家版）。
    /// </summary>
    /// <param name="request">查询请求（<c>ds</c> 必填，格式 YYYYMMDD）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>带货数据（<c>data</c>：成交金额 / 动销达人数 / 动销商品数 / 点击-成交转化率）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/compass/api_getshopfinderoverall"/></para>
    /// <para>
    /// 官方业务限制：①本接口无特殊注意事项；②不支持云调用；③第三方平台代调用权限集 id 为 175；
    /// ④仅「微信小店」账号类型可调用；⑤数据分析口径参见官方「数据分析开发指南」。
    /// </para>
    /// <para>
    /// <b>官方文档矛盾照录</b>：参数表类型标注为 number，官方示例为字符串（<c>"pay_gmv": "0"</c>）——SDK 按参数表（number）建模。
    /// </para>
    /// <para>官方错误码：<c>10021069</c>（请求参数日期格式有误）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/compass/shop/finder/overall/get")]
    Task<ChannelsCompassShopFinderOverallResponse> GetShopFinderOverallAsync(
        [Body] ChannelsCompassDsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取带货达人商品列表（<c>getshopfinderproductlist</c>，罗盘商家版）。
    /// </summary>
    /// <param name="request">查询请求（<c>ds</c> 必填；<c>finder_id</c>/<c>talent_id</c>/<c>mp_id</c> 三者必填其一）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品信息列表（<c>product_list</c>：商品 id / 头图 / 标题 / 价格 / 三级类目 / 佣金数据）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/compass/api_getshopfinderproductlist"/></para>
    /// <para>
    /// 官方业务限制：①本接口无特殊注意事项；②<c>finder_id</c>、<c>talent_id</c>、<c>mp_id</c> 三者必填其一；
    /// ③不支持云调用；④第三方平台代调用权限集 id 为 175；⑤仅「微信小店」账号类型可调用。
    /// </para>
    /// <para>官方错误码：<c>886</c>（登录账号类型错误，例如达人的接口收到店铺账号的请求）/
    /// <c>10021069</c>（请求参数日期格式有误）/ <c>10023001</c>（finder_id 有误）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/compass/shop/finder/product/list/get")]
    Task<ChannelsCompassShopFinderProductListResponse> GetShopFinderProductListAsync(
        [Body] ChannelsCompassShopFinderProductRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取带货达人详情（<c>getshopfinderproductoverall</c>，罗盘商家版）。
    /// </summary>
    /// <param name="request">查询请求（<c>ds</c> 必填；<c>finder_id</c>/<c>talent_id</c>/<c>mp_id</c> 三者必填其一）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>带货数据（<c>data</c>：成交金额 / 动销商品数 / 成交人数 / 退款金额 / 成交退款金额，官方类型 string）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/compass/api_getshopfinderproductoverall"/></para>
    /// <para>
    /// 官方业务限制：①本接口无特殊注意事项；②<c>finder_id</c>、<c>talent_id</c>、<c>mp_id</c> 三者必填其一；
    /// ③不支持云调用；④第三方平台代调用权限集 id 为 175；⑤仅「微信小店」账号类型可调用；
    /// ⑥返回字段官方类型为 string（示例为空串），与 <c>getshopfinderlist</c> 的 number 口径不同 ⇒ 不共用成交数据 DTO。
    /// </para>
    /// <para>官方错误码：<c>10021069</c>（请求参数日期格式有误）/ <c>10023001</c>（finder_id 有误）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/compass/shop/finder/product/overall/get")]
    Task<ChannelsCompassShopFinderProductOverallResponse> GetShopFinderProductOverallAsync(
        [Body] ChannelsCompassShopFinderProductRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取店铺开播列表（<c>getshoplivelist</c>，罗盘商家版）。
    /// </summary>
    /// <param name="request">查询请求（<c>ds</c> 与 <c>finder_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>开播列表（<c>live_list</c>：直播 id / 标题 / 开播时间 / 直播时长 / 封面）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/compass/api_getshoplivelist"/></para>
    /// <para>
    /// 官方业务限制：①本接口无特殊注意事项；②不支持云调用；③第三方平台代调用权限集 id 为 175；
    /// ④仅「微信小店」账号类型可调用；⑤<c>live_time</c>/<c>live_duration</c> 官方类型为 string
    /// （Unix 时间戳/时长以字符串返回），照抄建模。
    /// </para>
    /// <para>官方错误码：<c>10023001</c>（finder_id 有误）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/compass/shop/live/list/get")]
    Task<ChannelsCompassShopLiveListResponse> GetShopLiveListAsync(
        [Body] ChannelsCompassShopLiveListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取电商数据概览（<c>getshopoverall</c>，罗盘商家版）。
    /// </summary>
    /// <param name="request">查询请求（<c>ds</c> 必填，格式 YYYYMMDD）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>电商数据（<c>data</c>：成交金额 / 成交人数 / 成交订单数 / 成交退款金额 / 直播与短视频成交金额 / 商品点击人数）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/compass/api_getshopoverall"/></para>
    /// <para>官方业务限制：本接口无特殊注意事项；不支持云调用；第三方平台代调用权限集 id 为 175；仅「微信小店」账号类型可调用；返回字段官方类型为 string。</para>
    /// <para>官方错误码：<c>10021069</c>（请求参数日期格式有误）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/compass/shop/overall/get")]
    Task<ChannelsCompassShopOverallResponse> GetShopOverallAsync(
        [Body] ChannelsCompassDsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商品详细信息（<c>getshopproductdata</c>，罗盘商家版，仅历史数据暂无当日数据）。
    /// </summary>
    /// <param name="request">查询请求（<c>ds</c> 必填；<c>product_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品详细信息（<c>product_info</c>：商品档案 + <c>data</c> 成交/下单/退款/结算/佣金明细）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/compass/api_getshopproductdata"/></para>
    /// <para>
    /// 官方业务限制：①本接口无特殊注意事项；②<b>仅历史数据，暂无当日数据</b>（官方原文）；
    /// ③不支持云调用；④第三方平台代调用权限集 id 为 175；⑤仅「微信小店」账号类型可调用。
    /// </para>
    /// <para>
    /// <b>官方文档矛盾照录</b>：<c>pay_refund_ratio</c>/<c>pay_refund_after_send_ratio</c>/
    /// <c>pay_refund_before_send_ratio</c> 与 <c>refund_gmv</c> 的官方类型列标注为 <c>null</c>（无类型），
    /// 示例中比率为数值、<c>refund_gmv</c> 为字符串——比率按 <c>double</c>、<c>refund_gmv</c> 按 string 建模。
    /// </para>
    /// <para>官方错误码：<c>10021069</c>（请求参数日期格式有误）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/compass/shop/product/data/get")]
    Task<ChannelsCompassShopProductDataResponse> GetShopProductDataAsync(
        [Body] ChannelsCompassShopProductDataRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商品列表（<c>getshopproductlist</c>，罗盘商家版，仅历史数据暂无当日数据）。
    /// </summary>
    /// <param name="request">查询请求（<c>ds</c> 必填；<c>limit</c>/<c>offset</c> 选填，不填默认只返回 10 条商品数据）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品列表（<c>product_list</c>）与商品总数（<c>total_count</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/compass/api_getshopproductlist"/></para>
    /// <para>
    /// 官方业务限制：①本接口无特殊注意事项；②<b>仅历史数据，暂无当日数据</b>（官方原文）；
    /// ③<c>limit</c>/<c>offset</c> 不填默认只返回 10 条；④不支持云调用；⑤第三方平台代调用权限集 id 为 175；
    /// ⑥仅「微信小店」账号类型可调用；⑦官方变更日志 2025-11-24：新增 <c>pay_refund_cnt</c> 字段、删除废弃字段。
    /// </para>
    /// <para>
    /// <b>官方文档矛盾照录</b>：返回示例含商品档案字段（<c>head_img_url</c>/<c>title</c>/<c>price</c>/三级类目）
    /// 而参数表未列——按参数表建模（仅 <c>product_id</c> + <c>data</c>）；<c>refund_gmv</c> 官方说明写「退款件数」
    /// （疑为笔误，退款件数为 <c>refund_product_cnt</c>），照录；<c>pay_refund_ratio</c> 类型列标注 null、示例为数值，按 double 建模。
    /// </para>
    /// <para>官方错误码：<c>10021069</c>（请求参数日期格式有误）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/compass/shop/product/list/get")]
    Task<ChannelsCompassShopProductListResponse> GetShopProductListAsync(
        [Body] ChannelsCompassShopProductListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取店铺人群数据（<c>getshopsaleprofiledata</c>，罗盘商家版，仅历史数据暂无当日数据）。
    /// </summary>
    /// <param name="request">查询请求（<c>ds</c> 与 <c>type</c> 必填，用户类型见 <see cref="ChannelsCompassShopSaleProfileUserTypes"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>店铺人群数据（<c>data.field_list</c>：维度类别 + 维度指标键值对列表）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/compass/api_getshopsaleprofiledata"/></para>
    /// <para>
    /// 官方业务限制：①<b>仅历史数据，暂无当日数据</b>（官方原文）；②不支持云调用；
    /// ③第三方平台代调用权限集 id 为 175；④仅「微信小店」账号类型可调用。
    /// </para>
    /// <para>官方错误码：<c>10021069</c>（请求参数日期格式有误）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/compass/shop/sale/profile/data/get")]
    Task<ChannelsCompassShopSaleProfileResponse> GetShopSaleProfileDataAsync(
        [Body] ChannelsCompassSaleProfileRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取电商概览数据（<c>getfinderoverall</c>，罗盘达人版）。
    /// </summary>
    /// <param name="request">查询请求（<c>ds</c> 必填，格式 YYYYMMDD）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>电商概览数据（<c>data</c>：总/直播/短视频/橱窗/商品分享/其他渠道成交金额）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/channels/compass/api_getfinderoverall"/></para>
    /// <para>官方业务限制：本接口无特殊注意事项；不支持云调用；第三方平台代商家调用权限集 id 为 176；仅「视频号助手」账号类型可调用。</para>
    /// <para>官方错误码：<c>48001</c>（接口权限未开启，需前往视频号助手-开放能力-开放场景打开开关）/
    /// <c>10021069</c>（请求参数日期格式有误）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/compass/finder/overall/get")]
    Task<ChannelsCompassFinderOverallResponse> GetFinderOverallAsync(
        [Body] ChannelsCompassDsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取带货商品数据（<c>getfinderproductdata</c>，罗盘达人版）。
    /// </summary>
    /// <param name="request">查询请求（<c>ds</c> 必填，格式 YYYYMMDD）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商品数据（<c>product_info</c>：商品档案 + <c>data</c> 成交/下单/退款/佣金明细，官方类型均为 number）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/channels/compass/api_getfinderproductdata"/></para>
    /// <para>官方业务限制：本接口无特殊注意事项；不支持云调用；第三方平台代商家调用权限集 id 为 176；仅「视频号助手」账号类型可调用。</para>
    /// <para>官方错误码：<c>48001</c>（接口权限未开启）/ <c>10021069</c>（请求参数日期格式有误）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/compass/finder/product/data/get")]
    Task<ChannelsCompassFinderProductDataResponse> GetFinderProductDataAsync(
        [Body] ChannelsCompassDsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取带货商品列表（<c>getfinderproductlist</c>，罗盘达人版）。
    /// </summary>
    /// <param name="request">查询请求（<c>ds</c> 必填，格式 YYYYMMDD）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>带货商品列表（<c>product_list</c>，元素与带货商品数据 <c>product_info</c> 同构）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/channels/compass/api_getfinderproductlist"/></para>
    /// <para>
    /// 官方业务限制：①本接口无特殊注意事项；②不支持云调用；③第三方平台代商家调用权限集 id 为 176；
    /// ④仅「视频号助手」账号类型可调用。
    /// </para>
    /// <para>
    /// <b>官方文档矛盾照录</b>：<c>product_list</c> 参数表标注为 objarray，官方返回示例为单对象——
    /// 按参数表（列表）建模；元素与 <c>getfinderproductdata</c> 的 <c>product_info</c> 字段集一致 ⇒ 共用
    /// <see cref="ChannelsCompassFinderProductInfo"/>。
    /// </para>
    /// <para>官方错误码：<c>48001</c>（接口权限未开启）/ <c>10021069</c>（请求参数日期格式有误）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/compass/finder/product/list/get")]
    Task<ChannelsCompassFinderProductListResponse> GetFinderProductListAsync(
        [Body] ChannelsCompassDsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取带货人群数据（<c>getfindersaleprofiledata</c>，罗盘达人版，仅历史数据暂无当日数据）。
    /// </summary>
    /// <param name="request">查询请求（<c>ds</c> 与 <c>type</c> 必填，用户类型见 <see cref="ChannelsCompassFinderSaleProfileUserTypes"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>带货人群数据（<c>data.field_list</c>：维度类别 + 维度指标键值对列表）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/channels/api/channels/compass/api_getfindersaleprofiledata"/></para>
    /// <para>
    /// 官方业务限制：①<b>仅历史数据，暂无当日数据</b>（官方原文）；②不支持云调用；
    /// ③第三方平台代商家调用权限集 id 为 176；④仅「视频号助手」账号类型可调用。
    /// </para>
    /// <para>
    /// <b>官方文档矛盾照录</b>：<c>dim_value</c> 参数表标注为 number，官方示例为字符串（如 <c>"3"</c>）——
    /// 按参数表（number）建模；与商家版人群数据（官方类型 string）类型口径不同 ⇒ 不共用 DTO。
    /// </para>
    /// <para>官方错误码：<c>48001</c>（接口权限未开启）/ <c>10021069</c>（请求参数日期格式有误）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/compass/finder/sale/profile/data/get")]
    Task<ChannelsCompassFinderSaleProfileResponse> GetFinderSaleProfileDataAsync(
        [Body] ChannelsCompassSaleProfileRequest request,
        CancellationToken cancellationToken = default);
}
