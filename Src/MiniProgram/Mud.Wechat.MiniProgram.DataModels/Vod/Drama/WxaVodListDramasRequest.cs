// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「获取剧目列表」请求体（<c>POST /wxa/sec/vod/listdramas</c>）：只有分页两参数。
/// </summary>
/// <remarks>
/// <para><b>上限（官方原文）</b>：<c>limit</c> 最大值 100；两者皆选填。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_listdramas.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodListDramasRequest
{
    /// <summary>limit（官方类型 <c>number</c>）：分页拉取的最大返回结果数。最大值：100。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }

    /// <summary>offset（官方类型 <c>number</c>）：分页拉取的起始偏移量。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }
}
