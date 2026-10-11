// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.DataModels.Vip;

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「会员营销」域 SDK（5 端点：会员用户积分 / 信息 /
/// 列表 / 积分流水 + 关联小程序信息）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与「企微会员」「小程序会员」概念严格区分</b>：本域是小店会员（<c>/channels/ec/vip/*</c>，
/// 小店 <c>access_token</c>）；其中 <c>v3/wxa/info/get</c> 是「小程序会员服务」族落在小店侧的
/// 查询端点（查询小店绑定的会员服务小程序），令牌仍为小店 <c>access_token</c>（权限集 id 186）。
/// </para>
/// <para>
/// <b>适用范围</b>：4 个 <c>user/*</c> 端点仅「微信小店（需开通）」账号类型可调用且店铺须已开通会员功能
/// （<c>10100003</c> 信息不存在常见于未开通会员 / 用户非会员）；不支持云调用、不支持第三方平台调用。
/// </para>
/// <para>
/// <b>积分口径</b>：单点查询（<c>score/get</c>）返回的积分为<b>字符串</b>（官方参数表类型 string），
/// 流水中的积分变动值为<b>数值</b>（number）——两处类型不一致为官方原文，照录建模。
/// </para>
/// <para>
/// <b>MUD005 已知接受风险</b>：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Vip", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsVipService
{
    /// <summary>
    /// 获取用户积分（<c>score/get</c>，会员当前积分余额）。
    /// </summary>
    /// <param name="request">查询请求（<c>openid</c> 必填；与获取用户信息请求体字段集一致 ⇒ 共用 <see cref="ChannelsVipOpenidRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>用户积分信息（<c>info.score</c> 当前积分余额，官方类型为 string）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/vip/api_getvipuserscore"/></para>
    /// <para>官方业务限制：官方注意事项原文「本接口无特殊注意事项」；仅「微信小店（需开通）」账号类型可调用。</para>
    /// <para>官方错误码：<c>10100001</c>（系统错误）/ <c>10100002</c>（参数错误，检查 openid）/
    /// <c>10100003</c>（信息不存在，确认该用户是否为店铺会员）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/vip/user/score/get")]
    Task<ChannelsVipGetUserScoreResponse> GetUserScoreAsync(
        [Body] ChannelsVipOpenidRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户信息（<c>info/get</c>，openid / unionid / 会员等级与经验值）。
    /// </summary>
    /// <param name="request">查询请求（<c>openid</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>会员用户信息（<c>info</c>：openid / unionid / 等级信息 <c>user_grade_info</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/vip/api_getuserinfo"/></para>
    /// <para>官方业务限制：官方注意事项原文「本接口无特殊注意事项」；仅「微信小店（需开通）」账号类型可调用。</para>
    /// <para>官方错误码：<c>10071003</c>（参数错误，检查请求参数格式）/ <c>10100001</c>（系统错误）/
    /// <c>10100002</c>（参数错误，检查 openid）/ <c>10100003</c>（信息不存在，确认该用户是否为店铺会员）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/vip/user/info/get")]
    Task<ChannelsVipGetUserInfoResponse> GetUserInfoAsync(
        [Body] ChannelsVipOpenidRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户列表（<c>list/get</c>，分页获取店铺会员用户）。
    /// </summary>
    /// <param name="request">查询请求（<c>page_num</c> / <c>page_size</c> 必填，page_size 最大 200）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>会员用户列表（<c>list</c>，元素同单点用户信息 <see cref="ChannelsVipUserInfo"/>）与会员用户总数（<c>total_num</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/vip/api_getuserlist"/></para>
    /// <para>官方业务限制：①官方注意事项原文「本接口无特殊注意事项」；②<c>page_size</c> 最大 200，
    /// <c>page_num</c> 从 1 开始；③店铺须已开通会员功能。官方无游标字段，分页由 page_num/page_size 控制。</para>
    /// <para>官方错误码：<c>10100001</c>（系统错误）/ <c>10100002</c>（参数错误，检查 page_num 与 page_size）/
    /// <c>10100003</c>（信息不存在，确认店铺是否已开通会员功能）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/vip/user/list/get")]
    Task<ChannelsVipGetUserListResponse> GetUserListAsync(
        [Body] ChannelsVipGetUserListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取用户积分流水（<c>score/flowrecord/get</c>，分页查询积分变动明细）。
    /// </summary>
    /// <param name="request">查询请求（<c>openid</c> / <c>page_num</c> / <c>page_size</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>积分流水列表（<c>list</c>：积分变动值 / 来源 / 备注）与流水记录总数（<c>total_num</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/vip/api_getuserscoreflowrecord"/></para>
    /// <para>官方业务限制：官方注意事项原文「本接口无特殊注意事项」；积分变动值正数为增加、负数为扣减，
    /// 来源枚举见 <see cref="ChannelsVipScoreFlowSources"/>。</para>
    /// <para>官方错误码：<c>10100001</c>（系统错误）/ <c>10100002</c>（参数错误，检查 openid 与分页字段）/
    /// <c>10100003</c>（信息不存在，确认该用户是否为店铺会员）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/vip/user/score/flowrecord/get")]
    Task<ChannelsVipGetUserScoreFlowRecordResponse> GetUserScoreFlowRecordAsync(
        [Body] ChannelsVipGetUserScoreFlowRecordRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 小店获取关联小程序信息（<c>v3/wxa/info/get</c>，小程序会员服务的注册 / 权益页面路径与白名单）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>关联小程序信息（<c>wxa_appid</c> / <c>info.paths</c> 注册与权益页路径 /
    /// <c>info.whitelist_openids</c> 测试白名单 / <c>info.display_info.order_page_display</c> 下单页权益开关 /
    /// <c>state</c> 上线状态）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/wxamember/shop/api_v3_getwxainfo"/></para>
    /// <para>官方契约：<b>POST</b> + 空请求体；令牌为小店 access_token（第三方代调用权限集 id 186）。</para>
    /// <para>
    /// 官方业务限制：①<c>state</c> 为 2（未上线）/ 3（上线前测试状态）时，只有白名单用户可以在小店看到会员入口；
    /// ②原有的「设置关联小程序信息」与「删除关联小程序信息」接口已停用，改为小店电脑管理端操作。
    /// </para>
    /// <para>官方错误码：<c>1</c>（参数异常）/ <c>2</c>（小店没绑定小程序）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/vip/v3/wxa/info/get")]
    Task<ChannelsVipGetWxaInfoResponse> GetWxaInfoAsync(CancellationToken cancellationToken = default);
}
