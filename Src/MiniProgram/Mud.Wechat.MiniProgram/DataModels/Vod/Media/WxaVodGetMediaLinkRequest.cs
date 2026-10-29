// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 「获取媒资播放链接」请求体（<c>POST /wxa/sec/vod/getmedialink</c>）：一整套播放防盗链参数。
/// </summary>
/// <remarks>
/// <para><b>参数名极短且不成体系（照录官方，勿「对齐」成语义名）</b>：<c>t</c> 过期时间戳、<c>us</c> 链接标识、<c>exper</c> 试看时长、<c>rlimit</c> 限 IP 数、<c>whref</c> 允许域名、<c>bkref</c> 禁止域名。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/vod_media/api_getmedialink.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodGetMediaLinkRequest
{
    /// <summary>media_id（官方类型 <c>number</c>，<b>必填</b>）：媒资文件id</summary>
    [JsonPropertyName("media_id")]
    public long? MediaId { get; set; }

    /// <summary>t（官方类型 <c>number</c>，<b>必填</b>）：播放地址的过期时间戳。有效的时间最长不能超过2小时后</summary>
    [JsonPropertyName("t")]
    public long? T { get; set; }

    /// <summary>us（官方类型 <c>string</c>）：链接标识。平台默认会生成一个仅包含小写字母和数字的字符串用于增强链接的唯一性(如us=647488c4792c15185b8fd2a6)。如开发者需要增加自己的标识，比如区分播放的渠道，可使用该参数，该参数最终的值是开发者标识-平台标识（如开发者传入abcd，则最终的临时链接中us=abcd-647488c4792c15185b8fd2a6）</summary>
    [JsonPropertyName("us")]
    public string? Us { get; set; }

    /// <summary>exper（官方类型 <c>number</c>）：试看时长，单位：秒，最大值不能超过视频长度</summary>
    [JsonPropertyName("exper")]
    public long? Exper { get; set; }

    /// <summary>rlimit（官方类型 <c>number</c>）：最多允许多少个不同 IP 的终端播放，以十进制表示，最大值为9，不填表示不做限制。当限制 URL 只能被1个人播放时，建议 rlimit 不要严格限制成1（例如可设置为3），因为移动端断网后重连 IP 可能改变</summary>
    [JsonPropertyName("rlimit")]
    public long? Rlimit { get; set; }

    /// <summary>whref（官方类型 <c>string</c>）：允许访问的域名列表，支持1条 - 10条，用半角逗号分隔。域名前不要带协议名（http://和https://），域名为前缀匹配（如填写 abc.com，则 abc.com/123 和 abc.com.cn也会匹配），且支持通配符（如 *.abc.com）</summary>
    [JsonPropertyName("whref")]
    public string? Whref { get; set; }

    /// <summary>bkref（官方类型 <c>string</c>）：禁止访问的域名列表，支持1条 - 10条，用半角逗号分隔。域名前不要带协议名（http://和https://），域名为前缀匹配（如填写 abc.com，则 abc.com/123 和 abc.com.cn也会匹配），且支持通配符（如 *.abc.com）</summary>
    [JsonPropertyName("bkref")]
    public string? Bkref { get; set; }
}
