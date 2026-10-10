// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Message;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「消息推送」模块智能表格自动化创建的群聊域企业自建应用 SDK：官方仅向自建应用开放本域端点
/// （获取群聊列表 / 获取群聊会话 / 修改群聊会话），全部声明于本接口。
/// <para>第三方应用与服务商代开发官方无对应文档，不设子接口。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方权限口径：
/// 需配置到文档「可调用应用」列表中的应用，可见范围需包含根部门；操作的智能表格须为当前应用创建。
/// 并发限制：获取群聊列表 / 获取群聊会话为 20，修改群聊会话为 10；
/// 修改群聊会话每企业变更次数不可超过 1000 次/小时，且<b>同一群聊的修改需要串行执行</b>。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Message",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkSmartSheetGroupChatService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalSmartSheetGroupChatService : IWechatWorkSmartSheetGroupChatService
{
    /// <summary>
    /// 获取智能表格自动化创建的群聊列表
    /// <para>获取当前应用创建的智能表格经自动化规则创建的群聊列表（分页）。</para>
    /// <para>官方不保证每次返回的数据刚好为 limit，必须以返回的 has_more 判断是否继续请求；
    /// 并发限制 20。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetSmartSheetGroupChatListRequest"/>：docid / cursor / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>群聊 chatid 列表（chat_id_list）、是否还有更多数据（has_more）与下一页游标（next_cursor）。</returns>
    /// <remarks>
    /// <para>群聊会话详情经 <see cref="GetSmartSheetGroupChatAsync"/> 查询。</para>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100989"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/groupchat/list")]
    Task<GetSmartSheetGroupChatListResponse> GetSmartSheetGroupChatListAsync(
        [Body] GetSmartSheetGroupChatListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取智能表格自动化创建的群聊会话
    /// <para>通过智能表格 ID 与群聊 ID 获取群聊会话信息（群聊名称、群主、群成员列表）。</para>
    /// <para>群成员列表仅返回当前企业成员；并发限制 20。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetSmartSheetGroupChatRequest"/>：docid / chat_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>群聊会话信息（name / owner / user_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101028"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/groupchat/get")]
    Task<GetSmartSheetGroupChatResponse> GetSmartSheetGroupChatAsync(
        [Body] GetSmartSheetGroupChatRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改智能表格自动化创建的群聊会话
    /// <para>变更群主与群成员（add_user_list / del_user_list 一次最多各 500 人）；
    /// del_user_list 包含群主时 owner 必填，群主需为本企业成员，操作的成员需为对应智能表格中的成员。</para>
    /// <para>群成员人数不可超过 2000 人；每企业变更群的次数不可超过 1000 次/小时；
    /// <b>同一群聊的修改需要串行执行</b>（勿并发调用）；并发限制 10。</para>
    /// </summary>
    /// <param name="request">修改请求体（<see cref="UpdateSmartSheetGroupChatRequest"/>：docid / chat_id / owner / add_user_list / del_user_list）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>修改结果（仅 errcode / errmsg）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101029"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/smartsheet/groupchat/update")]
    Task<UpdateSmartSheetGroupChatResponse> UpdateSmartSheetGroupChatAsync(
        [Body] UpdateSmartSheetGroupChatRequest request,
        CancellationToken cancellationToken = default);
}
