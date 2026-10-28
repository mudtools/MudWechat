// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「查询流量包详情」请求体（<c>POST /wxa/sec/vod/listpackages</c>）。
/// </summary>
/// <remarks>
/// <para><b>三参数官方均标必填</b>（含过滤用的 <c>status</c>，不过滤时传 <c>0</c>）；<c>limit</c> 最大值 100。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/usagedata/api_listpackages.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodListPackagesRequest
{
    /// <summary>status（官方类型 <c>number</c>，<b>必填</b>）：按流量包状态过滤，取值有：0-不过滤，1-有效的，2-无效的，3-过期的</summary>
    [JsonPropertyName("status")]
    public long? Status { get; set; }

    /// <summary>offset（官方类型 <c>number</c>，<b>必填</b>）：偏移值，分页请求用</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>limit（官方类型 <c>number</c>，<b>必填</b>）：分页大小，最大值为100</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>query_type（官方类型 <c>number</c>）：查询类型，0：通用流量；1：定向流量。默认值为0</summary>
    [JsonPropertyName("query_type")]
    public long? QueryType { get; set; }
}
