// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 查询视图响应体（<c>/cgi-bin/wedoc/smartsheet/get_views</c>）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class GetSmartSheetViewsResponse : WechatWorkResponse
{
    /// <summary>获取或设置符合筛选条件的视图总数（官方 <c>total</c>）。</summary>
    [JsonPropertyName("total")]
    public int? Total { get; set; }

    /// <summary>获取或设置是否还有更多项（官方 <c>has_more</c>）。</summary>
    [JsonPropertyName("has_more")]
    public bool? HasMore { get; set; }

    /// <summary>获取或设置下次下一个搜索结果的偏移量（官方 <c>next</c>）。</summary>
    [JsonPropertyName("next")]
    public int? Next { get; set; }

    /// <summary>获取或设置视图数据（官方 <c>views</c>）。</summary>
    [JsonPropertyName("views")]
    public List<SmartSheetView>? Views { get; set; }
}
