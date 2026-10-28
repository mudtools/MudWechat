// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 企业法人认证信息（<c>verifybetaweapp</c> 请求体 <c>verify_info</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/register-management/fast-regist-beta/verfifyBetaMiniprogram.html"/>。
/// </summary>
/// <remarks>
/// <para><c>new_version=true</c>（新版本）时，<c>code_type</c>/<c>legal_persona_wechat</c>/<c>legal_persona_name</c>/<c>legal_persona_idcard</c> 均可不填；
/// 仅 <c>enterprise_name</c>/<c>code</c>/<c>component_phone</c> 恒必填。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpBetaVerifyInfo
{
    /// <summary>企业名（需与工商登记一致；无主体名称个体工商户填「个体户+法人姓名」）（官方必填 <c>enterprise_name</c>，string）。</summary>
    [JsonPropertyName("enterprise_name")]
    public string? EnterpriseName { get; set; }

    /// <summary>企业代码（官方必填 <c>code</c>，string）。</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>企业代码类型：1 统一社会信用代码(18) / 2 组织机构代码(9) / 3 营业执照注册号(15)，新版本可不填（官方选填 <c>code_type</c>，number）。</summary>
    [JsonPropertyName("code_type")]
    public long? CodeType { get; set; }

    /// <summary>法人微信号，新版本可不填（官方选填 <c>legal_persona_wechat</c>，string）。</summary>
    [JsonPropertyName("legal_persona_wechat")]
    public string? LegalPersonaWechat { get; set; }

    /// <summary>法人姓名（绑定银行卡），新版本可不填（官方选填 <c>legal_persona_name</c>，string）。</summary>
    [JsonPropertyName("legal_persona_name")]
    public string? LegalPersonaName { get; set; }

    /// <summary>第三方联系电话（官方必填 <c>component_phone</c>，string）。</summary>
    [JsonPropertyName("component_phone")]
    public string? ComponentPhone { get; set; }

    /// <summary>法人身份证号，新版本可不填（官方选填 <c>legal_persona_idcard</c>，string）。</summary>
    [JsonPropertyName("legal_persona_idcard")]
    public string? LegalPersonaIdcard { get; set; }
}
