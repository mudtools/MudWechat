// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 获取内容块列表响应体（<c>/cgi-bin/wedoc/smartdoc/get_block_list</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class GetSmartDocBlockListResponse : WechatWorkResponse
{
    /// <summary>获取或设置返回的 Block 列表（官方 <c>blocks</c>）。</summary>
    [JsonPropertyName("blocks")]
    public List<SmartDocBlockInfo>? Blocks { get; set; }

    /// <summary>
    /// 获取或设置是否还有更多数据（官方 <c>has_more</c>）。
    /// <para><b>官方类型为字符串</b>而非布尔：<c>"true"</c> 表示还有更多、<c>"false"</c> 表示没有更多，
    /// 故本模型以字符串承载。</para>
    /// </summary>
    [JsonPropertyName("has_more")]
    public string? HasMore { get; set; }

    /// <summary>获取或设置下一批数据的起始点（官方 <c>next_start</c>）。</summary>
    [JsonPropertyName("next_start")]
    public int? NextStart { get; set; }
}
