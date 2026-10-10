// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Abstractions.Authentication.Models;

/// <summary>授权方企业信息（领域模型；对应官方 <c>auth_corp_info</c>）。</summary>
[HttpJsonSerializable(SerializerClassName = "Authentication")]
public sealed class WechatAuthCorpInfo
{
    /// <summary>授权方企业微信 id。</summary>
    public string? CorpId { get; set; }

    /// <summary>授权方企业名称（企业简称）。</summary>
    public string? CorpName { get; set; }

    /// <summary>授权方企业类型：认证号 verified / 注册号 unverified。</summary>
    public string? CorpType { get; set; }

    /// <summary>授权方企业方形头像。</summary>
    public string? CorpSquareLogoUrl { get; set; }

    /// <summary>授权方企业用户规模。</summary>
    public int CorpUserMax { get; set; }

    /// <summary>企业主体名称（仅认证/验证过的企业有）。</summary>
    public string? CorpFullName { get; set; }

    /// <summary>企业类型：1 企业；2 政府及事业单位；3 其他组织；4 团队号。</summary>
    public int SubjectType { get; set; }

    /// <summary>认证到期时间（Unix 秒）。</summary>
    public long VerifiedEndTime { get; set; }

    /// <summary>企业规模（未设置时为空）。</summary>
    public string? CorpScale { get; set; }

    /// <summary>企业所属行业（未设置时为空）。</summary>
    public string? CorpIndustry { get; set; }

    /// <summary>企业所属子行业（未设置时为空）。</summary>
    public string? CorpSubIndustry { get; set; }

    /// <summary>企业其他认证名称（仅认证企业有）。</summary>
    public WechatCorpExName? CorpExName { get; set; }
}
