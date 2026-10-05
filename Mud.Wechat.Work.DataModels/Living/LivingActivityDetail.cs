// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Living;

/// <summary>
/// 活动直播详情信息（创建预约直播请求 <c>activity_detail</c> 参数）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Living")]
public class LivingActivityDetail
{
    /// <summary>获取或设置活动直播简介（非活动类型的直播不用传）。</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>获取或设置活动直播附图的 mediaId 列表（最多支持传 5 张，超过五张取前五张）。</summary>
    [JsonPropertyName("image_list")]
    public List<string>? ImageList { get; set; }
}
