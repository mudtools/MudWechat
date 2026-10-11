// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.DataModels.PlatformKf;

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「平台客诉工单」域 SDK（4 端点：工单分类配置 /
/// 商家工单列表 / 工单详情 / 商户处理工单）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与「商家客服」概念严格区分</b>：本域是平台侧客诉工单（<c>/channels/ec/platformkf/*</c>，
/// 平台发起、商户响应），<b>不是</b>商家客服会话（<see cref="IChannelsKfService"/>，
/// <c>/channels/ec/commkf/*</c>）。
/// </para>
/// <para>
/// <b>工单流转契约</b>：工单状态为「1 待商家处理 / 2 待平台处理 / 3 已完结」（见
/// <see cref="ChannelsPlatformKfTicketStates"/>）；当前仅开放「催发货」一种分类类型（type 9，见
/// <see cref="ChannelsPlatformKfTicketClassTypes"/>）。
/// </para>
/// <para>
/// <b>第三方代调用</b>：本域 4 端点官方权限集 id 均为 <c>131</c>，服务商获得授权后可用
/// <c>authorizer_access_token</c> 代微信小店商家调用（多应用基座按当前应用上下文路由）。
/// </para>
/// <para>
/// <b>MUD005 已知接受风险</b>：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "PlatformKf", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsPlatformKfService
{
    /// <summary>
    /// 查询工单分类配置（<c>getticketclassconfig</c>，商家回复选项与必填项配置）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>商家回复选项配置（<c>mch_reply_options.conf_list</c>：工单分类类型 / 名称 + 可选回复项
    /// 的选项 id / 名称 / 补充说明·凭证·协商时间各自是否必填）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/platformkf/api_getticketclassconfig"/></para>
    /// <para>官方契约：<b>POST</b> + 空请求体。</para>
    /// <para>
    /// 官方业务限制：官方注意事项原文「本接口无特殊注意事项」；当前仅开放一种工单分类类型
    /// （催发货，type 9）；处理工单（<c>operatemchticket</c>）前须先经本接口取得
    /// <c>option_id</c> 与各必填项开关。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/platformkf/getticketclassconfig")]
    Task<ChannelsPlatformKfGetTicketClassConfigResponse> GetTicketClassConfigAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取商家工单列表（<c>getmchticketlist</c>，分页查询商家名下工单）。
    /// </summary>
    /// <param name="request">查询请求（全字段选填；不传 <c>ticket_state</c> 返回所有状态，默认按创建时间降序）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>工单列表（<c>ticket_item_list</c>，元素结构见 <see cref="ChannelsPlatformKfTicketItem"/>）与总数（<c>total</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/platformkf/api_getmchticketlist"/></para>
    /// <para>
    /// 官方业务限制：①官方注意事项原文「本接口无特殊注意事项」；②<c>limit</c> 默认 10，
    /// 最大建议不超过 50；③<c>search_ticket_id</c> 支持工单 id / 订单 id 任意其一；
    /// ④时间过滤针对工单的创建时间（Unix 秒级时间戳）。
    /// </para>
    /// <para>官方错误码：<c>40097</c>（参数错误）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/platformkf/getmchticketlist")]
    Task<ChannelsPlatformKfGetTicketListResponse> GetMerchantTicketListAsync(
        [Body] ChannelsPlatformKfGetTicketListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询工单详情（<c>getmchticketdetail</c>，单个工单全量信息）。
    /// </summary>
    /// <param name="request">查询请求（<c>ticket_id</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>工单详细信息（<c>merchant_ticket</c>，结构同工单列表元素 <see cref="ChannelsPlatformKfTicketItem"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/platformkf/api_getmchticketdetail"/></para>
    /// <para>官方业务限制：官方注意事项原文「本接口无特殊注意事项」；官方错误码 <c>40097</c>（参数错误）。</para>
    /// <para>官方契约：详情对象结构与工单列表元素一致 ⇒ SDK 共用 <see cref="ChannelsPlatformKfTicketItem"/> 建模。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/platformkf/getmchticketdetail")]
    Task<ChannelsPlatformKfGetTicketDetailResponse> GetMerchantTicketDetailAsync(
        [Body] ChannelsPlatformKfGetTicketDetailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 商户处理商家工单（<c>operatemchticket</c>，回复选项 + 补充说明 + 协商时间）。
    /// </summary>
    /// <param name="request">处理请求（<c>ticket_id</c> / <c>reply_option</c> 必填；<c>description</c> /
    /// <c>confer_time</c> 在对应选项的 <c>is_must_description</c> / <c>is_must_confer_time</c> 为 true 时必填，
    /// 选项配置经「查询工单分类配置」接口取得）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/platformkf/api_operatemchticket"/></para>
    /// <para>官方业务限制：官方注意事项原文「本接口无特殊注意事项」；同一工单不可重复处理（1600001 该工单已处理）。</para>
    /// <para>官方错误码：<c>40097</c>（参数错误）/ <c>1600001</c>（该工单已处理）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>131</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/platformkf/operatemchticket")]
    Task<ChannelsResponse> OperateMerchantTicketAsync(
        [Body] ChannelsPlatformKfOperateTicketRequest request,
        CancellationToken cancellationToken = default);
}
