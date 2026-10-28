// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Media;

/// <summary>
/// 获取永久素材总数（<c>material/get_materialcount</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方契约（逐页核验 2026-10-07）：<b>GET</b>、无请求体；路径为 <c>get_materialcount</c>
/// （<c>count</c> 前无下划线，照抄官方）。成功响应不含 <c>errcode</c>（缺省 0 视为成功）。
/// </para>
/// <para>
/// <b>数量上限（官方「注意事项」原文）</b>：「图片和图文消息素材（包括单图文和多图文）的总数上限为
/// <b>100000</b>，其他素材的总数上限为 <b>1000</b>」；「永久素材的总数包含公众平台官网素材管理中的素材」。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Media")]
public class MpGetMaterialCountResponse : MpResponse
{
    /// <summary>获取或设置语音总数量（官方 <c>voice_count</c>；上限 1000）。</summary>
    [JsonPropertyName("voice_count")]
    public int VoiceCount { get; set; }

    /// <summary>获取或设置视频总数量（官方 <c>video_count</c>；上限 1000）。</summary>
    [JsonPropertyName("video_count")]
    public int VideoCount { get; set; }

    /// <summary>获取或设置图片总数量（官方 <c>image_count</c>；上限 100000）。</summary>
    [JsonPropertyName("image_count")]
    public int ImageCount { get; set; }

    /// <summary>获取或设置图文总数量（官方 <c>news_count</c>；上限 100000）。</summary>
    [JsonPropertyName("news_count")]
    public int NewsCount { get; set; }
}
