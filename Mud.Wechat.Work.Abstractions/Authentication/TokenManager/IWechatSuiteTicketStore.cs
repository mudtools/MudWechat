// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication.TokenManager;

/// <summary>
/// suite_ticket 仓储接口。suite_ticket 由企业微信每 10 分钟推送到回调地址，
/// 不能主动获取——由回调处理器解析后按 suiteId 写入，<c>SuiteTokenManager</c> 经
/// <see cref="IWechatSuiteTicketProvider"/> 读取。
/// </summary>
/// <remarks>
/// 按 <c>suiteId</c> 分槽：一个进程可托管多个套件 / 代开发模板（代开发模板 id 即 suite_id），
/// 各套件的票据互不覆盖。
/// </remarks>
public interface IWechatSuiteTicketStore
{
    /// <summary>读取指定套件最新推送的 suite_ticket（未入库时返回 null）。</summary>
    Task<string?> GetAsync(string suiteId, CancellationToken cancellationToken = default);

    /// <summary>写入指定套件最新推送的 suite_ticket（覆盖该套件旧值）。</summary>
    Task SetAsync(string suiteId, string ticket, CancellationToken cancellationToken = default);
}
