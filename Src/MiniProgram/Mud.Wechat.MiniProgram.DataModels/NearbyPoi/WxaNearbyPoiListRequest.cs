// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.NearbyPoi;

/// <summary>查看附近小程序地点列表请求体（<c>POST /wxa/getnearbypoilist</c>）。</summary>
/// <remarks>官方文档：<c>nearby-poi/api_getnearbypoilist.html</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "NearbyPoi")]
public class WxaNearbyPoiListRequest
{
    /// <summary>页码（<c>page</c>，必填；从 <c>0</c> 开始）。</summary>
    [JsonPropertyName("page")]
    public long? Page { get; set; }

    /// <summary>每页条数（<c>page_rows</c>，必填；上限以官方页面为准）。</summary>
    [JsonPropertyName("page_rows")]
    public long? PageRows { get; set; }
}
