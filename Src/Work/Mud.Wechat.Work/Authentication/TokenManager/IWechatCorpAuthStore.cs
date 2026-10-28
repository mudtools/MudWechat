// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Authentication.Models;

namespace Mud.Wechat.Work.Abstractions.Authentication.TokenManager;

/// <summary>
/// 企业授权仓储（授权企业信息 + 永久授权码）。
/// </summary>
/// <remarks>
/// 键布局为复合键 <c>(AppKey, AuthCorpId)</c>：同一进程可托管多个套件 / 代开发模板，
/// 各应用对同一授权企业的授权信息互相隔离，与令牌缓存
/// （<c>TokenKey = {type}:{appKey}</c> + scope = authCorpId）语义一致。
/// </remarks>
public interface IWechatCorpAuthStore
{
    /// <summary>按（应用键, 授权企业 CorpId）读取授权（不存在时返回 null）。</summary>
    Task<WechatCorpAuthorization?> GetAsync(string appKey, string authCorpId, CancellationToken cancellationToken = default);

    /// <summary>写入 / 更新授权（幂等，按复合键覆盖）。</summary>
    Task SetAsync(WechatCorpAuthorization auth, CancellationToken cancellationToken = default);

    /// <summary>移除授权（取消授权 / 卸载应用）。</summary>
    Task RemoveAsync(string appKey, string authCorpId, CancellationToken cancellationToken = default);

    /// <summary>枚举指定应用已授权的全部企业（SaaS 批量运维、后台同步用）。</summary>
    Task<IReadOnlyList<WechatCorpAuthorization>> ListAsync(string appKey, CancellationToken cancellationToken = default);
}
