// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Store;

// ---------------------------------------------------------------- 创建 / 修改门店小程序主体（POST /wxa/apply_merchant、/wxa/modify_merchant）

/// <summary>
/// 创建门店小程序（<c>POST /wxa/apply_merchant</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表（逐页核验 2026-10-07）：<c>first_catid</c> / <c>second_catid</c> /
/// <c>headimg_mediaid</c> / <c>nickname</c> / <c>intro</c> / <c>qualification_list</c> 为必填；
/// <c>org_code</c> / <c>other_files</c> 为非必填（官方错误码 <c>85024</c>「需要补充资料」表明
/// 特定场景下二者必填——必填性矛盾照录，SDK 不本地拦截）。
/// </para>
/// <para>
/// <b>开放面极窄（官方错误码 43104 原文，勿弱化）</b>：「this appid does not have permission__无调用权限，
/// <b>仅开放给电商类目（一级类目：电商平台、商家自营、跨境电商）</b>」——非电商类目主体调用本域即返回
/// <c>43104</c>（SDK 不做本地类目闸，由官方错误码表达）。
/// </para>
/// <para>
/// 官方原文约束：<c>nickname</c> 长度 <b>4~30 字符</b>（中文算两个字符），不得含特殊字符及「微信」等保留字。
/// </para>
/// <para>
/// SDK 调用前置（官方错误码表达，SDK 不编排）：需先经 <c>get_merchant_category</c> 取类目 id；
/// 需公众号管理员确认后进入审核流程（结果经 <c>get_merchant_audit_info</c> 查询）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpApplyMerchantRequest
{
    /// <summary>获取或设置一级类目 id（官方 <c>first_catid</c>，必填）。</summary>
    [JsonPropertyName("first_catid")]
    public int FirstCategoryId { get; set; }

    /// <summary>获取或设置二级类目 id（官方 <c>second_catid</c>，必填）。</summary>
    [JsonPropertyName("second_catid")]
    public int SecondCategoryId { get; set; }

    /// <summary>获取或设置头像临时素材 mediaid（官方 <c>headimg_mediaid</c>，必填；支持 jpg、png）。</summary>
    [JsonPropertyName("headimg_mediaid")]
    public string HeadImageMediaId { get; set; } = string.Empty;

    /// <summary>获取或设置门店小程序昵称（官方 <c>nickname</c>，必填；长度 4~30 字符，中文算两个字符）。</summary>
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = string.Empty;

    /// <summary>获取或设置门店小程序介绍（官方 <c>intro</c>，必填）。</summary>
    [JsonPropertyName("intro")]
    public string Intro { get; set; } = string.Empty;

    /// <summary>获取或设置类目相关证件（官方 <c>qualification_list</c>，必填；临时素材 mediaid）。</summary>
    [JsonPropertyName("qualification_list")]
    public string QualificationList { get; set; } = string.Empty;

    /// <summary>获取或设置营业执照或组织代码证（官方 <c>org_code</c>；临时素材 mediaid；官方错误码 85024 表明特定场景必填）。</summary>
    [JsonPropertyName("org_code")]
    public string? OrgCode { get; set; }

    /// <summary>获取或设置补充材料（官方 <c>other_files</c>；临时素材 mediaid；官方错误码 85024 表明特定场景必填）。</summary>
    [JsonPropertyName("other_files")]
    public string? OtherFiles { get; set; }
}
