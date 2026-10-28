// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Auth;

/// <summary>检查加密信息请求体（<c>POST /wxa/business/checkencryptedmsg</c>）。</summary>
/// <remarks>
/// <para>官方文档：<c>user-info/basic-info/api_checkencrypteddata.html</c>。</para>
/// <para>
/// <b>能力边界（官方原文）</b>：当前只支持<b>手机号加密数据</b>，且只能检测<b>最近 3 天</b>生成的加密数据。
/// <c>encrypted_msg_hash</c> 为 <c>to_hexstr(sha1(encrypted_msg))</c>（对加密消息原文做 SHA1 后转十六进制）。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Auth")]
public class WxaCheckEncryptedMsgRequest
{
    /// <summary>加密消息哈希（<c>encrypted_msg_hash</c>，必填；<c>SHA1(encrypted_msg)</c> 的十六进制串）。</summary>
    [JsonPropertyName("encrypted_msg_hash")]
    public string? EncryptedMsgHash { get; set; }

    /// <summary>用户唯一标识（<c>openid</c>，必填）。</summary>
    [JsonPropertyName("openid")]
    public string? OpenId { get; set; }
}
