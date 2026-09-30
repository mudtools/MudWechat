// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.Abstractions.Authentication.Models;

namespace Mud.Wechat.Work.Services.Authorization;

/// <summary>
/// 官方授权 DTO → 领域模型（<see cref="WechatCorpAuthorization"/>）映射。
/// </summary>
/// <remarks>
/// 官方 DTO 字段随版本演进震荡（如 <c>corp_wxqrcode</c> 已回收），故不直接持久化 DTO；
/// 映射收口在本类，宿主仓储结构不被官方变更击穿。
/// </remarks>
internal static class WechatAuthorizationMapper
{
    /// <summary><c>get_permanent_code</c> 响应 → 企业授权聚合。</summary>
    /// <param name="appKey">归属应用键。</param>
    /// <param name="response">永久授权码响应。</param>
    /// <param name="updatedAt">更新时间（Unix 毫秒）。</param>
    public static WechatCorpAuthorization FromPermanentCode(
        string appKey, GetPermanentCodeResponse response, long updatedAt)
    {
        var auth = new WechatCorpAuthorization
        {
            AppKey = appKey ?? string.Empty,
            AuthCorpId = response.AuthCorpInfo?.CorpId ?? string.Empty,
            PermanentCode = response.PermanentCode ?? string.Empty,
            State = response.State,
            CorpInfo = MapCorpInfo(response.AuthCorpInfo, null),
            AuthUser = MapAuthUser(response.AuthUserInfo),
            DealerCorp = MapDealer(response.DealerCorpInfo),
            UpdatedAt = updatedAt,
        };

        ApplyAgents(auth, response.AuthInfo?.Agents);
        return auth;
    }

    /// <summary><c>get_auth_info</c> 响应合并进既有聚合（仅覆盖非空字段，保留 <c>State</c> 与 <c>PermanentCode</c>）。</summary>
    /// <param name="target">既有聚合（原地更新）。</param>
    /// <param name="response">授权信息响应。</param>
    /// <param name="updatedAt">更新时间（Unix 毫秒）。</param>
    public static void ApplyAuthInfo(WechatCorpAuthorization target, GetAuthInfoResponse response, long updatedAt)
    {
        if (response.AuthCorpInfo != null)
        {
            target.CorpInfo = MergeCorpInfo(
                target.CorpInfo, MapCorpInfo(response.AuthCorpInfo, response.AuthCorpInfo.CorpExName));
        }

        ApplyAgents(target, response.AuthInfo?.Agents);

        if (!string.IsNullOrEmpty(response.DealerCorpInfo?.CorpId))
        {
            target.DealerCorp = MapDealer(response.DealerCorpInfo);
        }

        // PermanentCode 以仓储现值优先，不被响应覆盖；State 同理保留。
        target.UpdatedAt = updatedAt;
    }

    private static void ApplyAgents(WechatCorpAuthorization target, List<Agent>? agents)
    {
        if (agents == null || agents.Count == 0)
        {
            return;
        }

        var mapped = agents.Select(MapAgent).ToList();
        target.Agents = mapped;
        target.AuthMode = mapped[0].AuthMode;
        target.IsCustomizedApp = mapped[0].IsCustomizedApp;
    }

    private static WechatAuthAgent MapAgent(Agent src) => new()
    {
        AgentId = src.AgentId,
        Name = src.Name,
        SquareLogoUrl = src.SquareLogoUrl,
        RoundLogoUrl = src.RoundLogoUrl,
        AppId = src.AppId,
        AuthMode = src.AuthMode ?? 0,
        IsCustomizedApp = src.IsCustomizedApp ?? false,
        AuthFromThirdApp = src.AuthFromThirdApp ?? false,
        Privilege = src.Privilege == null ? null : new WechatAuthPrivilege
        {
            Level = src.Privilege.Level,
            AllowParty = src.Privilege.AllowParty?.ToList() ?? new List<int>(),
            AllowUser = MapUserList(src.Privilege.AllowUser),
            AllowTag = src.Privilege.AllowTag?.ToList() ?? new List<int>(),
            ExtraParty = src.Privilege.ExtraParty?.ToList() ?? new List<int>(),
            ExtraUser = MapUserList(src.Privilege.ExtraUser),
            ExtraTag = src.Privilege.ExtraTag?.ToList() ?? new List<int>(),
        },
        SharedFrom = src.SharedFrom?.CorpId is { Length: > 0 } sharedCorpId
            ? new WechatAuthSharedFrom { CorpId = sharedCorpId, ShareType = src.SharedFrom.ShareType }
            : null,
    };

    private static IList<string> MapUserList(List<string?>? source)
        => source == null
            ? new List<string>()
            : source.Where(value => value != null).Select(value => value!).ToList();

    private static WechatAuthCorpInfo? MapCorpInfo(AuthCorpDetailInfo? src, CorpExName? exName)
    {
        if (src == null)
        {
            return null;
        }

        return new WechatAuthCorpInfo
        {
            CorpId = src.CorpId,
            CorpName = src.CorpName,
            CorpType = src.CorpType,
            CorpSquareLogoUrl = src.CorpSquareLogoUrl,
            CorpUserMax = src.CorpUserMax,
            CorpFullName = src.CorpFullName,
            SubjectType = src.SubjectType,
            VerifiedEndTime = src.VerifiedEndTime,
            CorpScale = src.CorpScale,
            CorpIndustry = src.CorpIndustry,
            CorpSubIndustry = src.CorpSubIndustry,
            CorpExName = exName == null
                ? null
                : new WechatCorpExName
                {
                    NameList = exName.NameList == null
                        ? new List<string>()
                        : exName.NameList.Where(name => name != null).Select(name => name!).ToList(),
                },
        };
    }

    private static WechatAuthCorpInfo? MergeCorpInfo(WechatAuthCorpInfo? existing, WechatAuthCorpInfo? mapped)
    {
        if (mapped == null)
        {
            return existing;
        }

        if (existing == null)
        {
            return mapped;
        }

        existing.CorpId = mapped.CorpId ?? existing.CorpId;
        existing.CorpName = mapped.CorpName ?? existing.CorpName;
        existing.CorpType = mapped.CorpType ?? existing.CorpType;
        existing.CorpSquareLogoUrl = mapped.CorpSquareLogoUrl ?? existing.CorpSquareLogoUrl;
        existing.CorpFullName = mapped.CorpFullName ?? existing.CorpFullName;
        existing.CorpScale = mapped.CorpScale ?? existing.CorpScale;
        existing.CorpIndustry = mapped.CorpIndustry ?? existing.CorpIndustry;
        existing.CorpSubIndustry = mapped.CorpSubIndustry ?? existing.CorpSubIndustry;
        existing.CorpExName = mapped.CorpExName ?? existing.CorpExName;
        if (mapped.CorpUserMax > 0)
        {
            existing.CorpUserMax = mapped.CorpUserMax;
        }

        if (mapped.SubjectType > 0)
        {
            existing.SubjectType = mapped.SubjectType;
        }

        if (mapped.VerifiedEndTime > 0)
        {
            existing.VerifiedEndTime = mapped.VerifiedEndTime;
        }

        return existing;
    }

    private static WechatAuthUserInfo? MapAuthUser(AuthUserInfo? src)
        => src == null || (src.UserId == null && src.OpenUserId == null && src.Name == null)
            ? null
            : new WechatAuthUserInfo
            {
                UserId = src.UserId,
                OpenUserId = src.OpenUserId,
                Name = src.Name,
                Avatar = src.Avatar,
            };

    private static WechatDealerCorpInfo? MapDealer(DealerCorpInfo? src)
        => src == null || string.IsNullOrEmpty(src.CorpId)
            ? null
            : new WechatDealerCorpInfo { CorpId = src.CorpId, CorpName = src.CorpName };
}