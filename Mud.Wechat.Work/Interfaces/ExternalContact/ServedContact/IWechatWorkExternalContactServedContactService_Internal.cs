// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.ExternalContact.ServedContact;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「客户联系」模块「获取已服务的外部联系人」域企业自建应用 SDK：
/// 官方仅向自建应用开放本域端点（第三方应用与服务商代开发暂不支持），声明于本接口。
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方权限口径：
/// 须使用配置到「客户联系 可调用接口的应用」中的 secret 获取的 access_token 调用。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "ExternalContact",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkExternalContactServedContactService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalExternalContactServedContactService : IWechatWorkExternalContactServedContactService
{
    /// <summary>
    /// 获取已服务的外部联系人
    /// <para>分页获取所有已服务的外部联系人及其添加人和加入的群聊；
    /// 客户返回临时 id + external_userid，其他外部联系人只返回临时 id 与脱敏昵称。</para>
    /// <para>外部联系人临时 id 仅在一轮完整遍历查询中唯一，每次请求首个分页（cursor 为空）时，
    /// 返回的临时 id 和 next_cursor 都会变化；cursor 有有效期，请勿缓存后使用。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="GetServedContactListRequest"/>：cursor / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>记录列表（info_list）与下一页游标（next_cursor，有效期 4 小时）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99434"/></para>
    /// </remarks>
    [Post("/cgi-bin/externalcontact/contact_list")]
    Task<GetServedContactListResponse> GetServedContactListAsync(
        [Body] GetServedContactListRequest request,
        CancellationToken cancellationToken = default);
}
