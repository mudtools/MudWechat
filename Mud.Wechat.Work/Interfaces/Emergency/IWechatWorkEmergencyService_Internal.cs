// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Emergency;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「紧急通知」模块紧急通知域企业自建应用 SDK：
/// 官方仅向自建应用开放本域 2 个端点（官方权限表对代开发 / 第三方均标注「暂不支持」），全部声明于本接口
/// （形态对齐 <see cref="IWechatWorkInternalIdentityTfaService"/>：零端点父接口 + 唯一自建子接口承载端点）。
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方权限口径：
/// 自建应用须配置到「紧急通知 - 可调用接口的应用」中。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Emergency",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkEmergencyService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalEmergencyService : IWechatWorkEmergencyService
{
    /// <summary>
    /// 发起语音电话
    /// <para>通过紧急通知应用向企业成员发起自动语音来电提醒（提醒员工查看应用推送的重要消息）。</para>
    /// <para>官方限制：callee_userid 官方必填且不能为空。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="EmergencyCallRequest"/>：callee_userid 被呼叫人 userid 列表，官方必填且不能为空）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>呼叫结果列表（states：code 呼叫结果状态 / callid 唯一标识一通呼叫的 id / userid 被呼叫人 userid）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91627"/></para>
    /// <para>官方权限：自建应用须配置到「紧急通知 - 可调用接口的应用」中；代开发应用暂不支持；第三方应用暂不支持。</para>
    /// <para>官方专属错误码：301049 调用接口的应用未在紧急通知应用中关联；301050 紧急通知应用未开启；301051 紧急通知应用余额不足。</para>
    /// </remarks>
    [Post("/cgi-bin/pstncc/call")]
    Task<EmergencyCallResponse> CallAsync(
        [Body] EmergencyCallRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取接听状态
    /// <para>按发起语音电话返回的 callid 查询指定被呼叫人的接听状态。</para>
    /// <para>官方限制：callee_userid 与 callid 均官方必填且不能为空；仅支持查询七天内的 callid 状态。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="EmergencyGetCallStatesRequest"/>：callee_userid 用户 id / callid 发起自动语音来电 callid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>接听状态（istalked 是否接听 / calltime 呼叫发起时间戳 / talktime 通话时长（秒） / reason 呼叫结果状态）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/91628"/></para>
    /// <para>官方权限：自建应用须配置到「紧急通知 - 可调用接口的应用」中；代开发应用暂不支持；第三方应用暂不支持。</para>
    /// </remarks>
    [Post("/cgi-bin/pstncc/getstates")]
    Task<EmergencyGetCallStatesResponse> GetCallStatesAsync(
        [Body] EmergencyGetCallStatesRequest request,
        CancellationToken cancellationToken = default);
}
