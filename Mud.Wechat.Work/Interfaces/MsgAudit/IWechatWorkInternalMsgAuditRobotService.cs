// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.MsgAudit;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「会话内容存档」模块机器人信息域企业自建应用 SDK（获取机器人信息）。
/// <para>
/// 官方仅向自建应用开放本域 1 个端点（代开发应用与第三方应用均暂不支持），
/// 声明于本接口（形态对齐 <see cref="IWechatWorkInternalPayFundFlowService"/>）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。
/// 官方权限口径：只能通过会话内容存档的 access_token 获取。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "MsgAudit",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkMsgAuditRobotService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalMsgAuditRobotService : IWechatWorkMsgAuditRobotService
{
    /// <summary>
    /// 获取机器人信息
    /// <para>按机器人 ID 查询机器人名称与创建者。</para>
    /// <para>官方业务限制：调用频率不可超过 600 次/分钟；
    /// 只能通过会话内容存档的 access_token 获取。</para>
    /// </summary>
    /// <param name="robotId">机器人 ID（官方必填，以 <c>robot_id</c> Query 参数传递）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>机器人信息（robot_id / name / creator_userid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91614"/></para>
    /// <para>官方业务限制：代开发应用、第三方应用均暂不支持本接口。</para>
    /// </remarks>
    [Get("/cgi-bin/msgaudit/get_robot_info")]
    Task<GetMsgAuditRobotInfoResponse> GetRobotInfoAsync(
        [Query("robot_id")] string robotId,
        CancellationToken cancellationToken = default);
}
