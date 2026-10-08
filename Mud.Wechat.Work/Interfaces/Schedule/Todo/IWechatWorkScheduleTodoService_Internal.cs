// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Schedule;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「日程」模块待办域企业自建应用 SDK：
/// 官方仅向自建应用开放本域 2 个端点（第三方应用开发与服务商代开发均无对应 API），全部声明于本接口
/// （形态对齐 <see cref="IWechatWorkInternalEmergencyService"/>：零端点父接口 + 唯一自建子接口承载端点）。
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，Query 注入 <c>access_token</c>）。
/// 官方权限口径（更新待办状态）：调用应用的可见范围需要包含根部门；仅允许修改当前应用创建的待办；不允许修改已删除的待办。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Schedule",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkScheduleTodoService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalScheduleTodoService : IWechatWorkScheduleTodoService
{
    /// <summary>
    /// 获取待办详情
    /// <para>获取指定的待办详情（待办内容 / 创建人 / 整体状态 / 参与人及其状态 / 截止时间 / 提醒列表）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetScheduleTodoRequest"/>：todo_id 待办 ID）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>待办详情（content 待办内容 / creator 待办创建人 ID / status 待办状态 / create_time 待办创建时间戳 / attendees 参与人列表 / end_time 待办截止时间戳 / reminders 提醒列表）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101524"/></para>
    /// <para>官方开放面：第三方应用开发与服务商代开发均无对应 API。</para>
    /// </remarks>
    [Post("/cgi-bin/todo/get")]
    Task<GetScheduleTodoResponse> GetTodoAsync(
        [Body] GetScheduleTodoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新待办状态
    /// <para>修改指定的待办信息，支持修改待办整体状态、待办参与人及其状态。</para>
    /// <para>官方限制：<c>attendees</c> 最多支持 20 个参与人，可不传或传空数组；不传或为空时不修改参与人列表；
    /// <c>status</c> 与 <c>attendees</c> 均官方选填，按需传入要修改的字段。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="UpdateScheduleTodoRequest"/>：todo_id 待办 ID / status 待办整体状态 / attendees 待办参与人列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101534"/></para>
    /// <para>官方开放面：第三方应用开发与服务商代开发均无对应 API。</para>
    /// <para>官方权限：调用应用的可见范围需要包含根部门；仅允许修改当前应用创建的待办；不允许修改已删除的待办。</para>
    /// <para>官方契约陷阱：官方请求示例中 attendees.status 出现值 2，与参数表仅列出 0/1 两种状态不一致，以参数表为准。</para>
    /// </remarks>
    [Post("/cgi-bin/todo/update")]
    Task<WechatWorkResponse> UpdateTodoAsync(
        [Body] UpdateScheduleTodoRequest request,
        CancellationToken cancellationToken = default);
}
