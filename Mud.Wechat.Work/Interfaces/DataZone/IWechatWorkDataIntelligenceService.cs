// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.DataZone;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「数据与智能」功能族公共 SDK（获取会话记录 <c>/cgi-bin/data/get_conversation_records</c> +
/// 获取消息统计 <c>/cgi-bin/data/get_message_statistics</c>）。
/// <para>
/// 路由前缀 <c>/cgi-bin/data/*</c> 与本模块既有基础接口域的 <c>/cgi-bin/chatdata/*</c>、
/// <c>/cgi-bin/docdata/*</c> 不同前缀，属「数据与智能专区」另一功能族，
/// 故独立成族接口（不复用 <see cref="IWechatWorkDataZoneService"/> 父接口），共用
/// <see cref="WechatModule.DataZone"/> 模块注册组（<c>AddDataZoneApi()</c>，不新增模块枚举）。
/// </para>
/// <para>
/// 自建应用见 <see cref="IWechatWorkInternalDataIntelligenceService"/>；
/// 第三方应用见 <see cref="IWechatWorkThirdPartyDataIntelligenceService"/>；
/// 服务商代开发见 <see cref="IWechatWorkProviderDataIntelligenceService"/>（均为零差异端点空标记）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键说明：自建应用消费应用自身 access_token；第三方 / 代开发消费授权企业级 access_token
/// （scope = authCorpId）——与既有 DataZone 两族一致，由多应用基座按当前应用上下文（AppKey + scope）路由。
/// </para>
/// <para>
/// 官方约束：两端点官方即 POST；「获取会话记录」以 <c>chatid</c> + 时间范围 + 游标（cursor/limit，
/// limit 最大 1000）分页拉取；「获取消息统计」以时间范围 + 统计粒度（day/week/month）聚合。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkDataIntelligenceService
{
    /// <summary>
    /// 获取会话记录
    /// <para>通过时间范围与游标分页拉取指定会话（单聊/群聊）的消息记录数据，用于数据分析与智能化应用场景。</para>
    /// </summary>
    /// <param name="request">
    /// 请求体（<see cref="Mud.Wechat.Work.DataModels.DataZone.GetConversationRecordsRequest"/>：
    /// chatid / starttime / endtime / cursor / limit）。
    /// </param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>会话记录分页结果（has_more / next_cursor / records）。</returns>
    /// <remarks>
    /// <para><b>数据与智能·获取会话记录</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99864"/></para>
    /// <para>
    /// 官方约束：官方即 POST；分页拉取以 <c>cursor</c> 游标推进（初始传入为空），
    /// <c>limit</c> 为单次拉取数量限制（最大 1000，默认 100），
    /// 是否拉完以 <c>has_more</c> 判定（后续请求以 <c>next_cursor</c> 续拉）。
    /// </para>
    /// </remarks>
    [Post("/cgi-bin/data/get_conversation_records")]
    Task<GetConversationRecordsResponse> GetConversationRecordsAsync(
        [Body] GetConversationRecordsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取消息统计
    /// <para>通过时间范围与统计粒度获取企业消息统计数据（发送/接收总量、各类型消息数量、活跃度等），用于分析沟通趋势和活跃度。</para>
    /// </summary>
    /// <param name="request">
    /// 请求体（<see cref="Mud.Wechat.Work.DataModels.DataZone.GetMessageStatisticsRequest"/>：
    /// starttime / endtime / type / agentid / userids）。
    /// </param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>消息统计数据列表（statistics）。</returns>
    /// <remarks>
    /// <para><b>数据与智能·获取消息统计</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99824"/></para>
    /// <para>官方约束：官方即 POST；统计粒度 <c>type</c> 支持 day（按天，默认）/ week（按周）/ month（按月）。</para>
    /// </remarks>
    [Post("/cgi-bin/data/get_message_statistics")]
    Task<GetMessageStatisticsResponse> GetMessageStatisticsAsync(
        [Body] GetMessageStatisticsRequest request,
        CancellationToken cancellationToken = default);
}
