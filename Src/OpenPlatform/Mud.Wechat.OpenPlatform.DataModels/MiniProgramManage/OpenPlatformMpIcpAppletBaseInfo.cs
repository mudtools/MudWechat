// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// ICP 备案小程序基础信息（<c>icp_applets.base_info</c>，各备案接口共享结构）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/record/applyIcpFiling.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpIcpAppletBaseInfo
{
    /// <summary>小程序 appid（官方 <c>appid</c>）。</summary>
    [JsonPropertyName("appid")]
    public string? AppId { get; set; }

    /// <summary>小程序名称（官方 <c>name</c>）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>服务内容类型列表（官方 <c>service_content_types</c>，number 数组）。</summary>
    [JsonPropertyName("service_content_types")]
    public List<long>? ServiceContentTypes { get; set; }

    /// <summary>前置审批项明细列表（官方 <c>nrlx_details</c>，array）。</summary>
    [JsonPropertyName("nrlx_details")]
    public List<OpenPlatformMpIcpNrlxDetail>? NrlxDetails { get; set; }

    /// <summary>备注（官方 <c>comment</c>）。</summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    /// <summary>小程序备案号（官方 <c>record_number</c>）。</summary>
    [JsonPropertyName("record_number")]
    public string? RecordNumber { get; set; }
}
