// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.AccountId;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「账号ID」域「ID 转换」接口族服务商代开发 SDK（含「群 ID 升级」差异端点）。
/// <para>
/// 继承公共父接口 <see cref="IWechatWorkAccountIdService"/> 的 9 个端点；另持官方仅向代开发应用开放的
/// 「群 ID 升级」2 个差异端点（申请群 ID 的升级 + 群 ID 转换，99601）——
/// 升级后代开发应用获取的群 ID 与第三方应用一致。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "AccountId",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkAccountIdService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkProviderAccountIdService : IWechatWorkAccountIdService
{
    /// <summary>
    /// 申请群 ID 的升级（对已授权企业）
    /// <para>设置代开发应用群 ID 完成升级的时间；生效前可使用新旧两种群 ID 调用相关接口（输出仍返回旧 ID），
    /// 生效后必须使用升级后的群 ID。</para>
    /// <para>仅代开发应用可调用；应用需具有「客户联系 -&gt; 基础客户信息」权限。</para>
    /// </summary>
    /// <param name="request">申请请求体（<see cref="ApplyToUpgradeChatIdRequest"/>；upgrade_time 不得早于当前时间或 7 天之后）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>申请结果（errcode/errmsg）。</returns>
    /// <remarks>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99601"/></para>
    /// </remarks>
    [Post("/cgi-bin/idconvert/apply_to_upgrade_chatid")]
    Task<WechatWorkResponse> ApplyToUpgradeChatIdAsync(
        [Body] ApplyToUpgradeChatIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 群 ID 转换
    /// <para>将升级前企业主体的群 ID 转换成升级后服务商主体的群 ID；传入升级后的群 ID 则原样返回；
    /// 到达指定的升级时间后无法调用本接口。</para>
    /// <para>仅代开发应用可调用；需先调用申请群 ID 的升级接口；应用需具有「客户联系 -&gt; 基础客户信息」权限；
    /// 群主需在应用的可见范围中。</para>
    /// </summary>
    /// <param name="request">转换请求体（<see cref="ConvertChatIdRequest"/>；最多输入 100 个）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>转换结果列表（items：chat_id + new_chat_id）及无法转换列表（invalid_chat_id_list）。</returns>
    /// <remarks>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/99601"/></para>
    /// </remarks>
    [Post("/cgi-bin/idconvert/chatid")]
    Task<ConvertChatIdResponse> ConvertChatIdAsync(
        [Body] ConvertChatIdRequest request,
        CancellationToken cancellationToken = default);
}
