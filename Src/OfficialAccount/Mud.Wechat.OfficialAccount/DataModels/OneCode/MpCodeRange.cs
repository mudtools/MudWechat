// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.OneCode;

/// <summary>
/// 二维码码段（官方 <c>code_generate_list[]</c> 与下载/激活端点的 <c>code_start</c>/<c>code_end</c> 共用语义）。
/// </summary>
/// <remarks>官方字段表：<c>code_start</c> 开始位置 / <c>code_end</c> 结束位置。</remarks>
[HttpJsonSerializable(SerializerClassName = "OneCode")]
public class MpCodeRange
{
    /// <summary>获取或设置开始位置（官方 <c>code_start</c>）。</summary>
    [JsonPropertyName("code_start")]
    public int? CodeStart { get; set; }

    /// <summary>获取或设置结束位置（官方 <c>code_end</c>）。</summary>
    [JsonPropertyName("code_end")]
    public int? CodeEnd { get; set; }
}
