// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.MsgAudit;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「会话内容存档」模块会话同意情况域企业自建应用 SDK
/// （单聊会话同意情况 + 群聊会话同意情况）。
/// <para>
/// 官方仅向自建应用开放本域 2 个端点（代开发应用与第三方应用均暂不支持），
/// 全部声明于本接口（形态对齐 <see cref="IWechatWorkInternalPayMchApplyService"/>）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。
/// 官方权限口径：access_token 必须由「会话内容存档」应用 secret 获取；
/// 参见「客户同意进行聊天内容存档事件回调」同步同意状态。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "MsgAudit",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkMsgAuditAgreeService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalMsgAuditAgreeService : IWechatWorkMsgAuditAgreeService
{
    /// <summary>
    /// 获取单聊会话同意情况
    /// <para>查询内部成员与外部成员单聊会话的外部成员同意情况。</para>
    /// <para>官方业务限制：一次请求最多支持 100 个查询条目，超出会被拦截；
    /// 调用频率不可超过 2500 次/分钟。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="CheckMsgAuditSingleAgreeRequest"/>：
    /// info（userid + exteranalopenid 条目数组，最多 100 条））。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>同意情况列表（agreeinfo：userid / exteranalopenid / agree_status / status_change_time）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91782"/></para>
    /// <para>官方业务限制：代开发应用、第三方应用均暂不支持本接口。</para>
    /// </remarks>
    [Post("/cgi-bin/msgaudit/check_single_agree")]
    Task<CheckMsgAuditAgreeResponse> CheckSingleAgreeAsync(
        [Body] CheckMsgAuditSingleAgreeRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取群聊会话同意情况
    /// <para>查询对应 roomid 里所有外企业外部联系人的同意情况。</para>
    /// <para>官方业务限制：调用频率不可超过 1500 次/分钟；
    /// 群聊响应的 agreeinfo 不返回内部成员 userid 字段。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="CheckMsgAuditRoomAgreeRequest"/>：roomid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>同意情况列表（agreeinfo：exteranalopenid / agree_status / status_change_time）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91782"/></para>
    /// <para>官方业务限制：代开发应用、第三方应用均暂不支持本接口。</para>
    /// </remarks>
    [Post("/cgi-bin/msgaudit/check_room_agree")]
    Task<CheckMsgAuditAgreeResponse> CheckRoomAgreeAsync(
        [Body] CheckMsgAuditRoomAgreeRequest request,
        CancellationToken cancellationToken = default);
}
