// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.ShortLink;

/// <summary>
/// 长信息转短链（<c>POST /cgi-bin/shorten/gen</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>short_key</c>（<b>15 字节，base62 编码</b>（0-9/a-z/A-Z））。
/// </para>
/// <para>
/// 官方错误码：<c>-1</c> / <c>40001</c> / <c>44002</c>（empty post data）/ <c>47001</c>（data format error）/
/// <c>47003</c>（参数错误，没有传入 <c>long_data</c>）/ <c>9410010</c>（long_data 长度超限）/
/// <c>9410011</c>（expire_seconds 超限）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "ShortLink")]
public class MpShortenGenResponse : MpResponse
{
    /// <summary>获取或设置短 key（官方 <c>short_key</c>，15 字节 base62）。</summary>
    [JsonPropertyName("short_key")]
    public string? ShortKey { get; set; }
}
