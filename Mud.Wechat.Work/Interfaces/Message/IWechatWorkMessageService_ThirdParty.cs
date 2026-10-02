// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Message;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「消息推送」模块发送应用消息族第三方应用 SDK。
/// <para>
/// 除继承公共父接口 <see cref="IWechatWorkMessageService"/> 的 3 个端点外，
/// 第三方应用官方另支持 <c>template_msg</c> 模板消息类型
/// （<see cref="SendTemplateMsgAsync"/>，经发送应用消息端点 <c>/cgi-bin/message/send</c> 发送，文档 94515；
/// 自建应用与服务商代开发均无对应文档，不设差异端点）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费授权企业级 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// 官方权限口径：服务商需在管理端申请模版（正式授权的应用需审批通过），
/// 接口传参的内容必须与申请的模版匹配；管理员授权模式下仅能给可见范围之内的成员推送消息，
/// 成员授权模式下可通过 selected_ticket_list 向可见范围外的成员推送。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Message",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkMessageService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkThirdPartyMessageService : IWechatWorkMessageService
{
    /// <summary>
    /// 发送模板消息（仅第三方应用）
    /// <para>msgtype = <c>template_msg</c>：固定格式的模板消息，经发送应用消息端点发送；
    /// template_id 为服务商管理端创建模板后获得（正式授权的应用需审批通过），最长 64 字节，
    /// 接口传参的内容必须与申请的模版匹配；url 与 miniprogram 至少要填一个，都填时优先 miniprogram。</para>
    /// <para>成员授权模式下，对于不在可见范围内的成员，可通过传入合法且未过期的
    /// selected_ticket_list（选人 sdk / 选人 jsapi 返回，不超过 10 个）推送模板消息；
    /// 仅向未授权用户发送时可开启 only_unauth（此时自动忽略 touser / toparty / totag）。</para>
    /// </summary>
    /// <param name="request">发送请求体（<see cref="MessageSendTemplateMsgRequest"/>：公共信封 + selected_ticket_list / only_unauth + template_msg 消息体）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>发送结果（invaliduser / invalidparty / invalidtag / unlicenseduser / msgid）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94515"/>（模板消息经发送应用消息端点 <c>/cgi-bin/message/send</c> 发送，官方页面未单独标注路由，以正文表述为准）。</para>
    /// </remarks>
    [Post("/cgi-bin/message/send")]
    Task<MessageSendResponse> SendTemplateMsgAsync(
        [Body] MessageSendTemplateMsgRequest request,
        CancellationToken cancellationToken = default);
}
