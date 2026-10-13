// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「查询 CDN 用量数据」响应。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/usagedata/api_getcdnusagedata.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodGetCdnUsageDataResponse : WxaResponse
{
    /// <summary>data_interval（官方类型 <c>number</c>）：时间粒度，单位：分钟。</summary>
    [JsonPropertyName("data_interval")]
    public long? DataInterval { get; set; }

    /// <summary>item_list（官方类型 <c>objarray</c>）：CDN 统计数据。</summary>
    [JsonPropertyName("item_list")]
    public List<WxaVodCdnItem>? ItemList { get; set; }
}
