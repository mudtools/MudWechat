// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 收集表图片/文件题的数量和大小限制信息（官方 <c>upload_image_limit</c> / <c>upload_file_limit</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocFormUploadLimit
{
    /// <summary>
    /// 获取或设置数量限制类型（官方 <c>count_limit_type</c>）。
    /// 官方取值：<c>0</c> 等于 count 数量、<c>1</c> 小于等于 count 数量。
    /// </summary>
    [JsonPropertyName("count_limit_type")]
    public uint? CountLimitType { get; set; }

    /// <summary>获取或设置限制数量（官方 <c>count</c>），范围 [1, 9]，默认 9。</summary>
    [JsonPropertyName("count")]
    public uint? Count { get; set; }

    /// <summary>获取或设置单个文件大小限制 MB（官方 <c>max_size</c>），不填表示无限制，最大值 3000。</summary>
    [JsonPropertyName("max_size")]
    public ulong? MaxSize { get; set; }
}
