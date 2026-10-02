// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Security;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「操作日志」域企业自建应用 SDK：官方仅向自建应用开放本域端点
/// （获取成员操作记录 / 获取管理端操作日志），全部声明于本接口。
/// <para>第三方应用与服务商代开发官方无对应文档，不设子接口。</para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。官方权限口径：
/// 成员操作记录需具有「获取成员操作记录」权限且操作人在应用可见范围内；
/// 管理端操作日志需具有「获取管理端操作日志」权限（默认返回全部管理端日志，不按可见范围筛选）。
/// 两接口均为<b>增量读取</b>接口（时间跨度不超过 7 天、最早可读 180 天前），
/// 频率限制 600 次/分钟，返回条数不保证等于 limit，须以 has_more 判断是否继续请求。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Security",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkSecurityOperLogService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalSecurityOperLogService : IWechatWorkSecurityOperLogService
{
    /// <summary>
    /// 获取成员操作记录
    /// <para>增量读取成员操作记录（管理端「使用分析」数据）；时间跨度不超过 7 天，最早不早于 180 天前。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="ListMemberOperLogsRequest"/>；limit 取值 1~400）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>成员操作记录分页结果（has_more / next_cursor / record_list）。</returns>
    /// <remarks>
    /// <para>频率限制 600 次/分钟；不同过滤条件的游标不能混用。</para>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100178"/></para>
    /// </remarks>
    [Post("/cgi-bin/security/member_oper_log/list")]
    Task<ListMemberOperLogsResponse> ListMemberOperLogsAsync(
        [Body] ListMemberOperLogsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取管理端操作日志
    /// <para>增量读取管理端操作日志（管理端「使用分析」数据）；时间跨度不超过 7 天，最早不早于 180 天前。</para>
    /// </summary>
    /// <param name="request">查询请求体（<see cref="ListAdminOperLogsRequest"/>；limit 取值 1~400）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>管理端操作日志分页结果（has_more / next_cursor / record_list）。</returns>
    /// <remarks>
    /// <para>频率限制 600 次/分钟；接口调用本身产生的日志（如通讯录同步助手）不会返回；
    /// 官方参数表将分页游标拼写为 <c>cusor</c>，本 SDK 以官方请求示例为准使用 <c>cursor</c>。</para>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/100179"/></para>
    /// </remarks>
    [Post("/cgi-bin/security/admin_oper_log/list")]
    Task<ListAdminOperLogsResponse> ListAdminOperLogsAsync(
        [Body] ListAdminOperLogsRequest request,
        CancellationToken cancellationToken = default);
}
