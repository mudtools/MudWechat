// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// ICP 备案主体基础信息（<c>icp_subject.base_info</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/applyIcpFiling.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpIcpSubjectBaseInfo
{
    /// <summary>主体类型（官方 <c>type</c>，number）。</summary>
    [JsonPropertyName("type")]
    public long? Type { get; set; }

    /// <summary>主体名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>主体所在省份（官方 <c>province</c>）。</summary>
    [JsonPropertyName("province")]
    public string? Province { get; set; }

    /// <summary>主体所在城市（官方 <c>city</c>）。</summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>主体所在区县（官方 <c>district</c>）。</summary>
    [JsonPropertyName("district")]
    public string? District { get; set; }

    /// <summary>主体详细地址（官方 <c>address</c>）。</summary>
    [JsonPropertyName("address")]
    public string? Address { get; set; }

    /// <summary>备注（官方 <c>comment</c>）。</summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    /// <summary>主体备案号（官方 <c>record_number</c>）。</summary>
    [JsonPropertyName("record_number")]
    public string? RecordNumber { get; set; }
}
